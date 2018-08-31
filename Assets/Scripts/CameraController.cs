using UnityEngine;
using System.Collections;
using UnityEngine.XR;

[RequireComponent (typeof (CharacterController))]
public class CameraController : MonoBehaviour {

	private float yaw = 0.0f, pitch = 0.0f;
	private Vector3 currentRot;
	public float mouseSensitivity = 10.0f;
	public float minPitch = -90.0f;
	public float maxPitch = 90.0f;
	public float gravity = 1.0f;
	public float jumpHeight = 1.0f;

	private CharacterController character;

	private void Start () {
		character = GetComponent<CharacterController> ();
	}

	private void Update () {
		if (SandBoxCanvasController.instance.IsActive) {
			return;
		}

		if (LoadingManager.Instance.IsLoading) {
			return;
		}

		//input gerakan kamera
		if (GameplayManager.instance.ShootMode == ShootMode.IDLE) {
			float mouseX, mouseY, hor, ver;
			mouseX = Input.GetAxis ("Mouse X");
			mouseY = Input.GetAxis ("Mouse Y");
			//DEBUG-UNCOMMENT
			//hor = Input.GetAxis ("Horr");
			//ver = Input.GetAxis ("Verr");
			//DEBUG-COMMENT
			hor = Input.GetAxis ("Horizontal");
			ver = Input.GetAxis ("Vertical");
			if (!(Mathf.Abs (mouseX) <= 0.1f && Mathf.Abs (mouseY) <= 0.1f)) {
				yaw += mouseX * mouseSensitivity;
				pitch -= mouseY * mouseSensitivity;
			}
			pitch = Mathf.Clamp (pitch, minPitch, maxPitch);

			//set arah kamera
			currentRot = new Vector3 (pitch, yaw);
			transform.eulerAngles = currentRot;

			//ambil arah kamera, hanya x dan z
			//DEBUG-UNCOMMENT
			/*float eulerY = InputTracking.GetLocalRotation (XRNode.CenterEye).eulerAngles.y;*/
			float eulerY = transform.eulerAngles.y;

			//hover atau jatuh
			float yMove = 0.0f;
			if (Input.GetButton ("Jump")) {
				//hover
				yMove = jumpHeight * Time.deltaTime;
			} else {
				//jatuh
				yMove = -gravity * Time.deltaTime;
			}

			//gerakkan kamera
			Vector3 move = new Vector3 (hor, yMove, ver);
			move = Quaternion.AngleAxis (eulerY, Vector3.up) * move;

			character.Move (move);
		}
	}

	public Vector3 getForward () {
		float eulerY = InputTracking.GetLocalRotation (XRNode.CenterEye).eulerAngles.y;
		Vector3 result = new Vector3 (0.0f, 0.0f, 1.0f);
		return Quaternion.AngleAxis (eulerY, Vector3.up) * result;
	}

	public void Teleport (Vector3 pos) {
		transform.position = pos;
	}
}
