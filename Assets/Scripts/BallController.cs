using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

	public ArrowController arrowTransform;
	private float yaw = 0.0f, pitch = 0.0f;
	private Vector3 currentRot;
	public float mouseSensitivity = 10.0f;
	public float minPitch = -90.0f;
	public float maxPitch = 90.0f;

	public float ballMass = 1.0f;

	public float arrowLengthModifier = 0.2f;

	private Rigidbody rigid;

	private Vector3 savedVelo, savedAngVelo;
	//private float savedGravity;
	private Vector3 savedForce;

	public ArrowController gravArrow;
	public ArrowController[] forceArrows;
	public ArrowController[] veloArrows;
	public ArrowController goalArrow;
	/* 0 = main
	 * 1 = x
	 * 2 = y
	 * 3 = z
	 */

	private PowerMeterController powerMeter;

	[SerializeField]
	private GhostController[] ghosts;

	// Use this for initialization
	private void Start () {
		rigid = GetComponent<Rigidbody> ();
		rigid.mass = ballMass;
	}

	private void Update () {
		//input
		if (GameplayManager.Instance.ShootMode == ShootMode.AIM) {
			//print ("shootmode");
			//ambil input mouse untuk arah lihat
			float mouseX, mouseY;
			mouseX = Input.GetAxis ("Horr") + Input.GetAxis ("Horizontal");
			mouseY = Input.GetAxis ("Verr") + Input.GetAxis ("Vertical");
			if (!(Mathf.Abs (mouseX) <= 0.1f && Mathf.Abs (mouseY) <= 0.1f)) {
				yaw += mouseX * mouseSensitivity;
				pitch -= mouseY * mouseSensitivity;
			}
			pitch = Mathf.Clamp (pitch, minPitch, maxPitch);

			//tentukan arah panah
			Quaternion oriRot = arrowTransform.GetRotation ();
			currentRot = new Vector3 (pitch, yaw);
			oriRot.eulerAngles = currentRot;
			oriRot *= Quaternion.AngleAxis (90.0f, Vector3.right);

			//offset position berdasarkan arah panah
			Vector3 oriPos = transform.localPosition + arrowTransform.GetDirection () * arrowTransform.GetArrowLength ();

			arrowTransform.SetTransform (oriPos, oriRot);

			//tampilkan ghost balls
			float forceMagnitude = powerMeter.GetForce ();
			float velo = forceMagnitude / GetMass ();
			float grav = Physics.gravity.magnitude;

			Vector3 force = (arrowTransform.GetDirection () * forceMagnitude);
			for (int i = 1; i <= 10; i++) {
				float t = i * 0.5f;

				float yPos = (force.y * t) - (grav * t * t / 2.0f);
				float xPos = force.x * t;
				float zPos = force.z * t;

				ghosts[i - 1].SetPosition (new Vector3 (xPos, yPos, zPos) + transform.position);
				//GhostController ghost = Instantiate (ghostBall, new Vector3 (xPos, yPos, zPos) + transform.position, Quaternion.identity).GetComponent<GhostController> ();
				ghosts[i - 1].SetActive (true);
				ghosts[i - 1].SetNumber (i);
			}
		}
	}

	public void SetGoalArrow (Vector3 goalPos) {
		Vector3 distVector = (goalPos - transform.position);
		float distance = distVector.magnitude;
		goalArrow.SetArrowLength (distance * arrowLengthModifier);
		Vector3 goalArrowPos = distVector * arrowLengthModifier;
		goalArrow.SetTransform (goalArrowPos);

		CanvasController.Instances[(int) CanvasType.DISTANCE].SetText (System.Math.Round (distance, 2) + " m");
		//DEBUG-COMMENT
		//CanvasController.Instances[(int) CanvasType.DISTANCE].SetVisible (true);
	}

	private void LateUpdate () {
		//get physics
		Vector3 currentVelo = rigid.velocity;
		Vector3 currentGrav = Physics.gravity;
		float mass = rigid.mass;

		//velocity arrow
		float length = currentVelo.magnitude;
		veloArrows[0].SetArrowLength (length * arrowLengthModifier);
		Vector3 veloPos = currentVelo * arrowLengthModifier;
		veloArrows[0].SetTransform (veloPos);

		//velocity text
		CanvasController.Instances[(int) CanvasType.VELO_ARROW].SetText (System.Math.Round (length, 2) + " m/s");
		//DEBUG-COMMENT
		//CanvasController.Instances[(int) CanvasType.VELO_ARROW].SetVisible (true);
	}

	public void ChangeShootMode (ShootMode sm) {
		if (sm == ShootMode.AIM) {
			arrowTransform.SetMeshVisible (true);
		} else {
			arrowTransform.SetMeshVisible (false);
		}
	}

	private void SaveState () {
		savedVelo = rigid.velocity;
		savedAngVelo = rigid.angularVelocity;
		savedForce = Vector3.zero;
		rigid.useGravity = false;
		rigid.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
		rigid.angularVelocity = new Vector3 (0.0f, 0.0f, 0.0f);
	}

	private void LoadState () {
		rigid.useGravity = true;
		if (savedForce != Vector3.zero) {
			rigid.AddForce (savedForce, ForceMode.Impulse);
		} else {
			rigid.velocity = savedVelo;
			rigid.angularVelocity = savedAngVelo;
		}
	}

	public void TimeFreeze (bool tf) {
		if (tf) {
			SaveState ();
		} else {
			LoadState ();
		}
	}

	public void Shoot (float forceMagnitude) {
		rigid.velocity = Vector3.zero;
		Vector3 dir = arrowTransform.GetDirection ();
		//print (dir);
		if (GameplayManager.Instance.TimeFreeze) {
			savedForce = dir * forceMagnitude;
		} else {
			//print (dir * forceMagnitude);
			rigid.AddForce (dir * forceMagnitude, ForceMode.Impulse);
		}

		foreach (GhostController ghost in ghosts) {
			ghost.SetActive (false);
		}
	}

	public float GetMass () {
		return rigid.mass;
	}

	public float GetSpeedMagnitude () {
		return rigid.velocity.magnitude;
	}

	public Vector3 GetSpeedVectors () {
		return rigid.velocity;
	}

	public void SetPosition (Vector3 pos) {
		transform.position = pos;
		rigid.velocity = Vector3.zero;
	}

	public void TogglePhysicsVision (bool pv) {
		gravArrow.SetMeshVisible (pv);
		foreach (ArrowController ac in forceArrows) {
			ac.SetMeshVisible (pv);
		}
		foreach (ArrowController ac in veloArrows) {
			ac.SetMeshVisible (pv);
		}
		goalArrow.SetMeshVisible (pv);

		for (int i = 2; i < (int) CanvasType.Length; i++) {
			if (CanvasController.Instances[i] != null) {
				CanvasController.Instances[i].SetVisible (pv);
				print ("canvas " + i + " aktif");
			}
		}
	}

	public void SetPowerMeter (PowerMeterController pM) {
		powerMeter = pM;
	}
}
