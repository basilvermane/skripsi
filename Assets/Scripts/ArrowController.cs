using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour {
	public BallController targetBall;
	public bool ShowOnStart = false;

	private MeshRenderer mesh;
	
	private void Start () {
		mesh = GetComponent<MeshRenderer> ();
		mesh.enabled = ShowOnStart;
	}

	public void SetMeshVisible (bool visible) {
		mesh.enabled = visible;
	}

	public Vector3 GetDirection () {
		return transform.up;
	}

	public void SetTransform (Vector3 pos) {
		//auto rotate sesuai posisi bola
		transform.position = pos + targetBall.transform.position;
		transform.LookAt (targetBall.transform.position);
		transform.Rotate (new Vector3 (90.0f, 0.0f, 0.0f));
	}

	public void SetTransform (Vector3 pos, Quaternion rot) {
		//local position, global rotation
		transform.position = pos + targetBall.transform.position;
		transform.rotation = rot;
	}

	public Vector3 GetPosition () {
		//local position
		return transform.position - targetBall.transform.position;
	}

	public Quaternion GetRotation () {
		//global rotation
		return transform.rotation;
	}

	public float GetArrowLength () {
		return transform.localScale.y;
	}

	public void SetArrowLength (float l) {
		Vector3 newScale = transform.localScale;
		newScale.y = l;
		transform.localScale = newScale;
	}
}
