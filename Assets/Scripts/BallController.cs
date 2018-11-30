using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {
	private const float TIMESTEP = 0.02f;

	public ArrowController arrowTransform;
	private float yaw = 0.0f, pitch = 0.0f;
	private Vector3 currentRot;
	public float mouseSensitivity = 1.0f;
	public float minPitch = -90.0f;
	public float maxPitch = 90.0f;

	private float ballMass = 1.0f;

	public float arrowLengthModifier = 0.2f;
	
	private Rigidbody rigid;

	private Vector3 savedVelo, savedAngVelo;
	//private float savedGravity;
	private Vector3 savedForce;

	//public ArrowController gravArrow;
	//public ArrowController forceArrows;
	public ArrowController veloArrows;
	public ArrowController goalArrow;
	/* 0 = main
	 * 1 = x
	 * 2 = y
	 * 3 = z
	 */

	private PowerMeterController powerMeter;

	[SerializeField]
	private GhostController[] ghosts;

	private bool switchSetMass = false;
	private float savedMass;

	// Use this for initialization
	private void Start () {
		rigid = GetComponent<Rigidbody> ();
		rigid.mass = ballMass;
	}

	private void Update () {
		//delayed initialization
		if (switchSetMass) {
			if (rigid != null) {
				rigid.mass = savedMass;
				switchSetMass = false;
			}
		}

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

			Vector3 force = (arrowTransform.GetDirection () * velo);
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

			if (GameplayManager.Instance.PhysicsVision) {
				//tampilkan besaran fisika
				float forceN = forceMagnitude / TIMESTEP;

				CanvasController.Instances[(int) CanvasType.GAME].SetText (System.Math.Round(forceN, 2) + " N");
			} else {
				CanvasController.Instances[(int) CanvasType.GAME].SetText ("");
			}
		}
	}

	public float GetForce () {
		if (GameplayManager.Instance.ShootMode != ShootMode.AIM) {
			return 0.0f;
		}

		float forceMagnitude = powerMeter.GetForce ();
		float velo = forceMagnitude / GetMass ();
		float grav = Physics.gravity.magnitude;
		float forceN = forceMagnitude / TIMESTEP;

		return forceN;
	}

	public float GetContactTime () {
		return TIMESTEP;
	}
	
	public float GetImpulse () {
		return powerMeter.GetForce ();
	}

	public float GetAngle () {
		Vector3 arrowV = arrowTransform.transform.up;
		Vector3 horizontalV = arrowV;
		horizontalV.y = 0.0f;
		return Vector3.Angle (horizontalV, arrowV);
	}

	public void SetGoalArrow (Vector3 goalPos) {
		Vector3 distVector = (goalPos - transform.position);
		float distance = distVector.magnitude;
		goalArrow.SetArrowLength (distance * arrowLengthModifier);
		Vector3 goalArrowPos = distVector * arrowLengthModifier;
		goalArrow.SetTransform (goalArrowPos);

		CanvasController.Instances[(int) CanvasType.DISTANCE].SetText (System.Math.Round (distance, 2) + " m");
		//CanvasController.Instances[(int) CanvasType.DISTANCE].SetVisible (true);
	}

	private void LateUpdate () {
		//get physics
		Vector3 currentVelo = rigid.velocity;
		Vector3 currentGrav = Physics.gravity;
		float mass = rigid.mass;

		//velocity arrow
		float length = currentVelo.magnitude;
		veloArrows.SetArrowLength (length * arrowLengthModifier);
		Vector3 veloPos = currentVelo * arrowLengthModifier;
		veloArrows.SetTransform (veloPos);

		//velocity text
		CanvasController.Instances[(int) CanvasType.VELO_ARROW].SetText (System.Math.Round (length, 2) + " m/s");
		//CanvasController.Instances[(int) CanvasType.VELO_ARROW].SetVisible (true);
	}

	public void ChangeShootMode (ShootMode sm) {
		if (sm == ShootMode.AIM) {
			arrowTransform.SetMeshVisible (true);
		} else {
			arrowTransform.SetMeshVisible (false);
			foreach (GhostController ghost in ghosts) {
				ghost.SetActive (false);
			}
		}
	}

	private void SaveState (bool overwriteState) {
		if (overwriteState) {
			savedVelo = rigid.velocity;
			savedAngVelo = rigid.angularVelocity;
			savedForce = Vector3.zero;
		}
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

	public void TimeFreeze (bool tf, bool fromSandBox = false, bool sbState = false) {
		if (fromSandBox) {
			//dari sandbox canvas
			if (tf) {
				if (sbState) {
					SaveState (false);
				}
			} else {
				if (sbState) {
					SaveState (true);
				} else {
					LoadState ();
				}
			}
		} else {
			//dari timefreeze
			if (tf) {
				SaveState (true);
			} else {
				LoadState ();
			}
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
		if (rigid != null) {
			return rigid.mass;
		} else {
			return -1.0f;
		}
	}

	public void SetMass (float m) {
		if (rigid != null)
			rigid.mass = m;
		else {
			switchSetMass = true;
			savedMass = m;
		}
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
		//gravArrow.SetMeshVisible (pv);
		/*foreach (ArrowController ac in forceArrows) {
			ac.SetMeshVisible (pv);
		}*/
		veloArrows.SetMeshVisible (pv);
		/*foreach (ArrowController ac in veloArrows) {
			ac.SetMeshVisible (pv);
		}*/
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
