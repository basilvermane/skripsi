using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

[RequireComponent (typeof (Animator))]
public class FormulaCanvasController : MonoBehaviour {
	private static FormulaCanvasController mainInstance, gameInstance;

	public static FormulaCanvasController MainInstance {
		get {
			return mainInstance;
		}
	}

	public static FormulaCanvasController GameInstance {
		get {
			return gameInstance;
		}
	}

	public bool isMainInstance; //main instance = di menu
	public float distanceFromCam = 0.5f;

	private void Awake () {
		if (isMainInstance) {
			if (mainInstance == null) {
				mainInstance = this;
			} else {
				Destroy (gameObject);
			}
		} else {
			if (gameInstance == null) {
				gameInstance = this;
			} else {
				Destroy (gameObject);
			}
		}
	}

	private void OnEnable () {
		SceneManager.sceneLoaded += SceneLoadCheck;
	}

	private void SceneLoadCheck (Scene scene, LoadSceneMode mode) {
		if (scene.name.Equals ("main")) {
			try {
				if (mainInstance != null)
					Destroy (mainInstance.gameObject);
			} finally {
				mainInstance = null;
			}
		} else if (scene.name.Equals ("menu")) {
			try {
				if (gameInstance != null)
					Destroy (gameInstance.gameObject);
			} finally {
				gameInstance = null;
			}
		}
	}

	private Animator anim;
	private Transform player;

	private bool isShown = false;

	private void Start () {
		anim = GetComponent<Animator> ();
	}
	
	public void SetVisible (bool show) {
		isShown = show;
		anim.SetBool ("show", show);
	}

	public void SetPlayer (Transform p) {
		player = p;
	}

	private void Update () {
		if (isShown) {
			if (!isMainInstance) {
				//DEBUG-UNCOMMENT
				//float yRot = InputTracking.GetLocalRotation (XRNode.CenterEye).eulerAngles.y;
				//DEBUG-COMMENT
				float yRot = player.eulerAngles.y;
				Vector3 playerFwd = Quaternion.AngleAxis (yRot, Vector3.up) * Vector3.forward;
				Vector3 pos = player.position + (playerFwd * distanceFromCam);

				//letakkan canvas
				transform.position = pos;
				transform.LookAt (player);
				transform.Rotate (Vector3.up, 180.0f);
			}
		}

		if (isShown && isMainInstance && (Input.GetButtonDown ("Cancel") || Input.GetKeyDown (KeyCode.JoystickButton3))) {
			SetVisible (false);
		}
	}

	public bool IsShown {
		get {
			return isShown;
		}
	}
}
