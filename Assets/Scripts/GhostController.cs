using UnityEngine;
using System.Collections;

public class GhostController : MonoBehaviour {
	private static float alphaDiff = 0.1f;

	private MeshRenderer mesh;

	private void Start () {
		mesh = GetComponent<MeshRenderer> ();
		mesh.enabled = false;
	}

	public void SetNumber (int number) {
		Color oriColor = mesh.material.color;
		oriColor.a = Mathf.Clamp01 (1.0f - (number * alphaDiff));
		mesh.material.color = oriColor;
	}

	public void SetActive (bool active) {
		mesh.enabled = active;
	}

	public void SetPosition (Vector3 pos) {
		transform.position = pos;
	}
}
