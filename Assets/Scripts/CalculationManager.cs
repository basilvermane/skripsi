using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.XR;

public class CalculationManager : MonoBehaviour {

	private static CalculationManager _instance;
	public static CalculationManager Instance {
		get {
			return _instance;
		}
	}

	private void Awake () {
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}

	public float distanceFromCam = 0.5f;

	private CameraController targetCam;
	private BallController targetBall;
	private Transform targetGoal;

	private Animator anim;
	private bool isShown = false;

	[SerializeField]
	private Text gravText, massText, forceText, impulseText,
		contactText, initSpeedText, angleText, verSpeedText,
		horSpeedText, verDistText, horDistText, reachText;

	public void SetCamera (CameraController t) {
		targetCam = t;
		print (targetCam);
	}

	public void SetGoal (Transform g) {
		targetGoal = g;
	}

	public void SetBall (BallController b) {
		targetBall = b;
	} 

	private void Start () {
		anim = GetComponent<Animator> ();
	}

	public void Show (bool show) {
		isShown = show;
		anim.SetBool ("show", isShown);
	}

	private void Update () {
		if (isShown) {
			//DEBUG-UNCOMMENT
			float yRot = InputTracking.GetLocalRotation (XRNode.CenterEye).eulerAngles.y;
			//DEBUG-COMMENT
			//float yRot = targetCam.transform.eulerAngles.y;
			Vector3 playerFwd = Quaternion.AngleAxis (yRot, Vector3.up) * Vector3.forward;
			Vector3 pos = targetCam.transform.position + (playerFwd * distanceFromCam);

			//letakkan canvas
			transform.position = pos;
			transform.LookAt (targetCam.transform);
			transform.Rotate (Vector3.up, 180.0f);

			//calculation
			float grav = Physics.gravity.magnitude;
			float mass = targetBall.GetMass ();
			float force = targetBall.GetForce ();
			float contact = targetBall.GetContactTime ();
			float impulse = targetBall.GetImpulse ();
			float angle = targetBall.GetAngle ();
			float initSpeed = impulse / mass;
			float verSpeed = Mathf.Sin (Mathf.Deg2Rad * angle) * initSpeed;
			float horSpeed = Mathf.Cos (Mathf.Deg2Rad * angle) * initSpeed;
			float horDist = new Vector2 (
				targetBall.transform.position.x - targetGoal.transform.position.x,
				targetBall.transform.position.z - targetGoal.transform.position.z)
				.magnitude;
			float verDist = Mathf.Abs (targetGoal.transform.position.y - targetBall.transform.position.y);
			float horTime = horDist / horSpeed;
			float verPos = (verSpeed * horTime) - (grav * horTime * horTime / 2.0f);
			float verPosDiff = verDist - verPos;

			//content
			gravText.text = System.Math.Round (grav, 2) + " N";
			massText.text = System.Math.Round (mass, 2) + " kg";
			forceText.text = System.Math.Round (force, 2) + " N";
			contactText.text = contact + " s";
			impulseText.text = System.Math.Round (force, 2) + " N * " + contact + " s = "
				+ System.Math.Round (impulse, 2) + " Ns";
			initSpeedText.text = System.Math.Round (impulse, 2) + " Ns / " + System.Math.Round (mass, 2) + " kg = "
				+ System.Math.Round (initSpeed, 2) + " m/s";
			angleText.text = System.Math.Round (angle, 2) + "°";
			verSpeedText.text = "sin(" + System.Math.Round (angle, 2) + "°) * " + System.Math.Round (initSpeed, 2) + " m/s = " + System.Math.Round (verSpeed, 2) + " m/s";
			horSpeedText.text = "cos(" + System.Math.Round (angle, 2) + "°) * " + System.Math.Round (initSpeed, 2) + " m/s = " + System.Math.Round (horSpeed, 2) + " m/s";
			verDistText.text = System.Math.Round (verDist, 2) + " m";
			horDistText.text = System.Math.Round (horDist, 2) + " m";

			reachText.text = "Assuming correct directions and no obstacle:\n"
				+ "Ball will reach goal horizontally after "
				+ System.Math.Round (horTime, 2)
				+ " seconds\n"
				+ "At the same time, ball will be "
				+ System.Math.Round (Mathf.Abs (verPosDiff), 2)
				+ " meters " + (verPosDiff >= 0.0f ? "below" : "above") + " the goal";
		}
	}
}
