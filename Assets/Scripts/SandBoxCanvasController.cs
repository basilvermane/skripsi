using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (Canvas))]
public class SandBoxCanvasController : MonoBehaviour {
	public static SandBoxCanvasController instance;

	private void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}

	private Animator anim;

	private int menu;

	public Slider[] sandboxSliders;

	public float maxAxisDelay = 1.0f;
	private float axisDelay;
	public float sliderPercentage = 0.5f;

	private bool inSubMenu;

	//-1 = not shown, 0 = gravity, 1 = map length, 2 = map width, 3 = max height
	//4 = move goal, 5 = move ball, 6 = ok, 7 = cancel

	// Use this for initialization
	private void Start () {
		anim = GetComponent<Animator> ();
		menu = -1;
		axisDelay = 0.0f;
		inSubMenu = false;
	}

	// Update is called once per frame
	private void Update () {
		if (menu < 0) {
			return;
		}

		if (axisDelay > 0.0f) {
			axisDelay -= Time.deltaTime;
			if (axisDelay < 0.0f) {
				axisDelay = 0.0f;
			}
		}

		float mouseX, mouseY;
		//DEBUG-UNCOMMENT
		//mouseX = Input.GetAxis ("Horr");
		//mouseY = Input.GetAxis ("Verr");
		//DEBUG-COMMENT
		mouseX = Input.GetAxis ("Horizontal");
		mouseY = Input.GetAxis ("Vertical");

		if (Mathf.Abs (mouseY) >= 0.1f && axisDelay <= 0.0f && !inSubMenu) {
			axisDelay = maxAxisDelay;
			if (mouseY > 0.0f) {
				if (menu >= 0 && menu <= 4 /*6*/) {
					menu++;
				}
			} else {
				if (menu >= 1 && menu <= 5 /*7*/) {
					menu--;
				}
			}
		}

		if (Input.GetKey (KeyCode.RightShift) && !inSubMenu) {
			switch (menu) {
				case 0:
				case 1:
				case 2:
				case 3: {
						inSubMenu = true;
					}
					break;
				case 4 /*6*/: {
						//ok
					}
					break;
				case 5 /*7*/: {
						//cancel
					}
					break;
			}
		}

		if (inSubMenu) {
			float min = sandboxSliders[menu].minValue;
			float max = sandboxSliders[menu].maxValue;
			float change = max - min * sliderPercentage / 100.0f;
			if (mouseX > 0.1f) {
				float temp = sandboxSliders[menu].value;
				sandboxSliders[menu].value += change;
				if (sandboxSliders[menu].value == temp) {
					sandboxSliders[menu].value++;
				}
			} else if (mouseX < -0.1f) {
				float temp = sandboxSliders[menu].value;
				sandboxSliders[menu].value -= change;
				if (sandboxSliders[menu].value == temp) {
					sandboxSliders[menu].value--;
				}
			}
		}

		anim.SetInteger ("menu", menu);
	}

	public void ApplyChanges () {
		float grav = sandboxSliders[0].value;
		int mapLength = (int) sandboxSliders[1].value;
		int mapWidth = (int) sandboxSliders[2].value;
		int mapHeight = (int) sandboxSliders[3].value * 100;
		GameplayManager.instance.SandboxChange (grav, mapLength, mapWidth, mapHeight);
		HideCanvas ();
	}

	public void HideCanvas () {
		if (menu >= 0) {
			anim.SetBool ("show", false);
		}
	}

	public void ShowCanvas () {
		if (menu < 0) {
			menu = 0;
			anim.SetBool ("show", true);
			anim.SetInteger ("menu", menu);
		}
	}
}
