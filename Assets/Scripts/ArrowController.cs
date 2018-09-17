using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour {

	public bool ShowOnStart = false;

	private MeshRenderer mesh;
	
	private void Start () {
		mesh = GetComponent<MeshRenderer> ();
		mesh.enabled = ShowOnStart;
	}

	public void SetMeshVisible (bool visible) {
		mesh.enabled = visible;
	}

	public Vector3 GetDirection (bool normalized) {
		if (normalized)
			return transform.up.normalized;
		else
			return transform.up;
	}

	public void SetTransform (Transform t) {
		transform.position = t.position;
		transform.rotation = t.rotation;
	}

	public Transform GetTransform () {
		return transform;
	}
}
