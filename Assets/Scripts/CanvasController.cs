using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CanvasType {
	GAME = 0,
	AIM_ARROW = 1,
	GRAV_ARROW = 2,
	FORCE_ARROW = 3,
	FORCE_ARROW_X = 4,
	FORCE_ARROW_Y = 5,
	FORCE_ARROW_Z = 6,
	VELO_ARROW = 7,
	VELO_ARROW_X = 8,
	VELO_ARROW_Y = 9,
	VELO_ARROW_Z = 10,
	Length
};

[RequireComponent (typeof (Canvas))]
public class CanvasController : MonoBehaviour {

	public static CanvasController[] Instances = new CanvasController[(int) CanvasType.Length];
	public CanvasType type;

	//attach to parent canvas
	private Canvas canvas;

	public Camera targetCamera;
	private Transform targetObject;
	public GameObject[] targetUI;
	public RectTransform targetCanvas;

	private bool statusVisible;
	private RectTransform[] targetUITrans;

	public Text text;

	private float canvasWidth, canvasHeight;

	private void Awake () {
		if (Instances[(int) type] == null) {
			DontDestroyOnLoad (gameObject);
			Instances[(int) type] = this;
		} else {
			Destroy (gameObject);
		}
	}

	private void Start () {
		canvas = GetComponent<Canvas> ();
		canvas.enabled = false;

		targetUITrans = new RectTransform[targetUI.Length];

		for (int i = 0; i < targetUI.Length; i++) {
			targetUITrans[i] = targetUI[i].GetComponent<RectTransform> ();
		}

		canvasWidth = targetCanvas.sizeDelta.x;
		canvasHeight = targetCanvas.sizeDelta.y;

		statusVisible = false;
		foreach (GameObject ui in targetUI) {
			ui.SetActive (statusVisible);
		}
	}

	public void SetCanvasVisible (bool visible) {
		canvas.enabled = visible;
	}

	//toggle
	public void SetVisible () {
		if (targetObject == null || !canvas.enabled) return;
		statusVisible = !statusVisible;
		print ("inside setvisible");
		foreach (GameObject ui in targetUI) {
			ui.SetActive (statusVisible);
			print (ui + " " + statusVisible);
		}
	}

	//set value
	public void SetVisible (bool visible) {
		if (targetObject == null || !canvas.enabled) return;
		statusVisible = visible;
		print ("inside setvisible");
		foreach (GameObject ui in targetUI) {
			ui.SetActive (statusVisible);
			print (ui + " " + statusVisible);
		}
	}

	private void Update () {
		if (targetObject == null || !canvas.enabled) return;
		//if visible, reposition every frame
		if (statusVisible) {
			Vector3 targetUIPos = targetCamera.WorldToViewportPoint (targetObject.position);
			targetUIPos.z = 0.0f;
			//print ("wtvp " + targetUIPos.x + " " + targetUIPos.y);
			targetUIPos.x = (targetUIPos.x - 0.5f) * canvasWidth;
			targetUIPos.y = (targetUIPos.y - 0.5f) * canvasHeight;
			//print ("process " + targetUIPos.x + " " + targetUIPos.y);
			foreach (RectTransform trans in targetUITrans) {
				trans.anchoredPosition3D = targetUIPos;
			}
		}
	}

	//set target object di world
	public void SetTargetObject (Transform target) {
		targetObject = target;
	}

	// called first
	private void OnEnable () {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	// called second
	private void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
		if (scene.name.Equals ("main")) {
			targetCamera = Camera.main;
			canvas.enabled = true;
		} else {
			targetCamera = null;
			if (canvas != null) canvas.enabled = false;
		}
	}

	// called when the game is terminated
	private void OnDisable () {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public void SetText (string t) {
		text.text = t;
	}
}