using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

	public Transform arrowTransform;
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

	// Use this for initialization
	void Start () {
		rigid = GetComponent<Rigidbody> ();
		rigid.mass = ballMass;
	}

	private void Update () {
		//input
		if (GameplayManager.instance.ShootMode == ShootMode.AIM) {
			//print ("shootmode");
			//ambil input mouse untuk arah lihat
			float mouseX, mouseY;
			//DEBUG-UNCOMMENT
			//mouseX = Input.GetAxis ("Horr");
			//mouseY = Input.GetAxis ("Verr");
			//DEBUG-COMMENT
			mouseX = Input.GetAxis ("Horizontal");
			mouseY = Input.GetAxis ("Vertical");
			if (!(Mathf.Abs (mouseX) <= 0.1f && Mathf.Abs (mouseY) <= 0.1f)) {
				yaw += mouseX * mouseSensitivity;
				pitch -= mouseY * mouseSensitivity;
			}
			pitch = Mathf.Clamp (pitch, minPitch, maxPitch);

			//tentukan arah panah
			currentRot = new Vector3 (pitch, yaw);
			arrowTransform.eulerAngles = currentRot;
			arrowTransform.Rotate (new Vector3 (90.0f, 0.0f, 0.0f));

			//offset position berdasarkan arah panah
			arrowTransform.position = transform.position + arrowTransform.up * distance;
		}
		GetPhysics ();
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
		Vector3 dir = arrowTransform.up.normalized;
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

	private void GetPhysics () {
		Vector3 currentVelo = rigid.velocity;
		Vector3 currentGrav = Physics.gravity;
		float mass = rigid.mass;
		//print (currentVelo + " " + currentGrav + " " + mass);
	}
}
