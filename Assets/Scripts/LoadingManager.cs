using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour {

	private static LoadingManager instance;

	public static LoadingManager Instance {
		get {
			return instance;
		}
	}

	public bool IsLoading;
	public GameObject[] loadingWalls, winScreenWalls;
	public GameObject finishText;

	private void Awake () {
		if (instance == null) {
			DontDestroyOnLoad (gameObject);
			instance = this;
			IsLoading = false;
			SetLoadingWall (false);
			SetWinWall (false);
		} else {
			Destroy (gameObject);
		}
	}

	public IEnumerator LoadPuzzle (int level) {
		IsLoading = true;
		transform.position = Camera.main.transform.position;
		SetLoadingWall (true, Camera.main.transform.position, Camera.main.transform.rotation);

		AsyncOperation async = SceneManager.LoadSceneAsync ("main");
		async.allowSceneActivation = false;
		//SceneManager.LoadScene ("main");
		while (!async.isDone) {
			if (async.progress == 0.9f) {
				if (IsLoading) {
					SetFinishText (true);
				} else {
					PlayerPrefs.SetInt ("level", level);
					async.allowSceneActivation = true;
				}
			}
			yield return null;
		}
		yield return null;
	}

	public IEnumerator WinScreen (float score) {
		IsLoading = true;
		SetWinWall (true, Camera.main.transform.position, Camera.main.transform.rotation);

		AsyncOperation async = SceneManager.LoadSceneAsync ("menu");
		async.allowSceneActivation = false;
		while (!async.isDone) {
			if (async.progress == 0.9f) {
				if (IsLoading) {
					SetFinishText (true);
				} else {
					async.allowSceneActivation = true;
				}
			}
			yield return null;
		}
		yield return null;
	}

	private void Update () {
		if (IsLoading &&
			(Input.GetButtonDown ("ShootMode") || Input.GetButtonDown ("TimeFreeze") ||
			Input.GetButtonDown ("PhysicsVision") || Input.GetButtonDown ("Cancel") ||
			Input.GetButtonDown ("Jump") || Input.GetButtonDown ("Jump2"))) {
			IsLoading = false;
		}
	}

	public void SetLoadingWall (bool active) {
		foreach(GameObject wall in loadingWalls) {
			wall.SetActive (active);
		}

		if (!active) {
			finishText.SetActive (false);
		}
	}

	public void SetLoadingWall (bool active, Vector3 pos) {
		transform.position = pos;
		foreach (GameObject wall in loadingWalls) {
			wall.SetActive (active);
		}

		if (!active) {
			finishText.SetActive (false);
		}
	}

	public void SetLoadingWall (bool active, Vector3 pos, Quaternion rot) {
		Quaternion newRot = Quaternion.identity;
		float rotY = rot.eulerAngles.y;
		newRot *= Quaternion.Euler (Vector3.up * rotY);
		transform.rotation = newRot;
		transform.position = pos;
		foreach (GameObject wall in loadingWalls) {
			wall.SetActive (active);
		}

		if (!active) {
			finishText.SetActive (false);
		}
	}

	public void SetWinWall (bool active) {
		foreach (GameObject wall in winScreenWalls) {
			wall.SetActive (active);
		}

		if (!active) {
			finishText.SetActive (false);
		}
	}

	public void SetWinWall (bool active, Vector3 pos) {
		transform.position = pos;
		foreach (GameObject wall in winScreenWalls) {
			wall.SetActive (active);
		}

		if (!active) {
			finishText.SetActive (false);
		}
	}

	public void SetWinWall (bool active, Vector3 pos, Quaternion rot) {
		Quaternion newRot = Quaternion.identity;
		float rotY = rot.eulerAngles.y;
		newRot *= Quaternion.Euler (Vector3.up * rotY);
		transform.rotation = newRot;
		transform.position = pos;
		foreach (GameObject wall in winScreenWalls) {
			wall.SetActive (active);
		}

		if (!active) {
			finishText.SetActive (false);
		}
	}

	public void SetFinishText (bool active) {
		finishText.SetActive (active);
	}
}
