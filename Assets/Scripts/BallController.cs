using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

	public ArrowController arrowTransform;
	private float yaw = 0.0f, pitch = 0.0f;
	private Vector3 currentRot;
	public float mouseSensitivity = 10.0f;
	public float minPitch = -90.0f;
	public float maxPitch = 90.0f;
	public float distance = 1.0f;

	public float ballMass = 1.0f;

	private Rigidbody rigid;

	private Vector3 savedVelo, savedAngVelo;
	//private float savedGravity;
	private Vector3 savedForce;

	public ArrowController gravArrow;
	public ArrowController[] forceArrows;
	public ArrowController[] veloArrows;
	/* 0 = main
	 * 1 = x
	 * 2 = y
	 * 3 = z
	 */

	// Use this for initialization
	private void Start () {
		rigid = GetComponent<Rigidbody> ();
		rigid.mass = ballMass;
	}

	private void Update () {
		//input
		if (GameplayManager.instance.ShootMode == ShootMode.AIM) {
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
			Transform arrowTemp = arrowTransform.GetTransform ();
			currentRot = new Vector3 (pitch, yaw);
			arrowTemp.eulerAngles = currentRot;
			arrowTemp.Rotate (new Vector3 (90.0f, 0.0f, 0.0f));

			//offset position berdasarkan arah panah
			arrowTemp.position = transform.position + arrowTemp.up * distance;

			arrowTransform.SetTransform (arrowTemp);
		}

		//physics vision
		Vector3 currentVelo = rigid.velocity;
		Vector3 currentGrav = Physics.gravity;
		float mass = rigid.mass;

		Vector3 veloNormal = currentVelo.normalized;
		Transform veloT = veloArrows[0].GetTransform ();
		veloT.localPosition = veloNormal;
		//Vector3 newRot = new Vector3 (currentVelo.y, currentVelo.x);
		//veloT.localEulerAngles = newRot;
		//veloT.Rotate (new Vector3 (90.0f, 0.0f, 0.0f));
		veloArrows[0].SetTransform (veloT);
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
		rigid.velocity = savedVelo;
		rigid.angularVelocity = savedAngVelo;
		rigid.useGravity = true;
		if (savedForce != Vector3.zero) {
			rigid.AddForce (savedForce);
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
		Vector3 dir = arrowTransform.GetDirection (true);
		print (dir);
		if (GameplayManager.instance.TimeFreeze) {
			savedForce = dir * forceMagnitude;
		} else {
			print (dir * forceMagnitude);
			rigid.AddForce (dir * forceMagnitude, ForceMode.Impulse);
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
	}
}
