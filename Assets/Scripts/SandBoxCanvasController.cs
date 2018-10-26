using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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
	/* -1 = not shown
	 * 0 = gravity
	 * 1 = map length
	 * 2 = map width
	 * 3 = max height
	 * 4 = ball mass
	 * 5 = apply
	 * 6 = back
	 * 7 = inside gravity
	 * 8 = inside map length
	 * 9 = inside map width
	 * 10 = inside max height
	 * 11 = inside ball mass
	 */

	public float maxInputCooldown = 0.5f;
	private float inputCooldown;
	public float sliderPercentage = 0.5f;

	public Slider gravSlider, widthSlider, lengthSlider, heightSlider, massSlider;

	private float gravMin, gravMax;
	private int widthMin, widthMax;
	private int lengthMin, lengthMax;
	private float heightMin, heightMax;
	private float massMin, massMax;

	public float scrollSpeedF = 1f;
	private int scrollSpeedI;

	private Transform player;

	private bool hideTrigger;
	public bool HideTrigger {
		get {
			bool temp = hideTrigger;
			hideTrigger = false;
			return temp;
		}
	}

	// Use this for initialization
	private void Start () {
		hideTrigger = false;

		anim = GetComponent<Animator> ();
		menu = -1;
		inputCooldown = 0.0f;

		gravMin = gravSlider.minValue;
		gravMax = gravSlider.maxValue;
		widthMin = Mathf.RoundToInt (widthSlider.minValue);
		widthMax = Mathf.RoundToInt (widthSlider.maxValue);
		lengthMin = Mathf.RoundToInt (lengthSlider.minValue);
		lengthMax = Mathf.RoundToInt (lengthSlider.maxValue);
		heightMin = heightSlider.minValue;
		heightMax = heightSlider.maxValue;
		massMin = massSlider.minValue;
		massMax = massSlider.maxValue;

		scrollSpeedI = Mathf.RoundToInt (scrollSpeedF);
	}

	public void SetPlayer (Transform p) {
		player = p;
	}
	
	private void Update () {
		if (LoadingManager.Instance.IsLoading) {
			return;
		}

		if (menu < 0) {
			return;
		}

		if (inputCooldown > 0.0f) {
			inputCooldown -= Time.deltaTime;
			if (inputCooldown < 0.0f) {
				inputCooldown = 0.0f;
			}
		} else {

			float mouseX, mouseY;
			mouseX = Input.GetAxis ("Horizontal") + Input.GetAxis ("Horr");
			mouseY = Input.GetAxis ("Vertical") + Input.GetAxis ("Verr");

			if (mouseY < -0.1f) { //bawah
				switch (menu) {
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
					case 5: {
							menu++;
							inputCooldown = maxInputCooldown;
						}
						break;
					case 6: {
							menu = 0;
							inputCooldown = maxInputCooldown;
						}
						break;
				}
			} else if (mouseY > 0.1f) { //atas
				switch (menu) {
					case 0: {
							menu = 6;
							inputCooldown = maxInputCooldown;
						}
						break;
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6: {
							menu--;
							inputCooldown = maxInputCooldown;
						}
						break;
				}
			}

			if (Input.GetButtonDown ("Jump") || Input.GetKeyDown (KeyCode.RightShift)) { //select
				switch (menu) {
					case 0: {
							menu = 7;
						}
						break;
					case 1: {
							menu = 8;
						}
						break;
					case 2: {
							menu = 9;
						}
						break;
					case 3: {
							menu = 10;
						}
						break;
					case 4: {
							menu = 11;
						}
						break;
					case 5: {
							ApplyChanges ();
						}
						break;
					case 6: {
							HideCanvas ();
						}
						break;
					case 7: {
							menu = 0;
						}
						break;
					case 8: {
							menu = 1;
						}
						break;
					case 9: {
							menu = 2;
						}
						break;
					case 10: {
							menu = 3;
						}
						break;
					case 11: {
							menu = 4;
						}
						break;
				}
			}

			if (Input.GetButtonDown ("Jump2") || Input.GetKeyDown (KeyCode.LeftShift)) { //back
				switch (menu) {
					case 7: {
							menu = 0;
						}
						break;
					case 8: {
							menu = 1;
						}
						break;
					case 9: {
							menu = 2;
						}
						break;
					case 10: {
							menu = 3;
						}
						break;
					case 11: {
							menu = 4;
						}
						break;
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
					case 5: {
							menu = 6;
						}
						break;
				}
			}

			if (menu >= 7 && menu <= 11) {
				if (mouseX > 0.1f) { //kanan
					switch (menu) {
						case 7: {
								float change = (gravMax - gravMin) * (scrollSpeedF / 100.0f);
								gravSlider.value = Mathf.Clamp (gravSlider.value + change, gravMin, gravMax);
							}
							break;
						case 8: {
								int change = (lengthMax - lengthMin) * scrollSpeedI / 100;
								lengthSlider.value = Mathf.Clamp (lengthSlider.value + change, lengthMin, lengthMax);
							}
							break;
						case 9: {
								int change = (widthMax - widthMin) * scrollSpeedI / 100;
								widthSlider.value = Mathf.Clamp (widthSlider.value + change, widthMin, widthMax);
							}
							break;
						case 10: {
								float change = (heightMax - heightMin) * scrollSpeedF / 100;
								heightSlider.value = Mathf.Clamp (heightSlider.value + change, heightMin, heightMax);
							}
							break;
						case 11: {
								float change = (massMax - massMin) * (scrollSpeedF / 100.0f);
								massSlider.value = Mathf.Clamp (massSlider.value + change, massMin, massMax);
							}
							break;
					}
				} else if (mouseX < -0.1f) { //kiri
					switch (menu) {
						case 7: {
								float change = (gravMax - gravMin) * (scrollSpeedF / 100.0f);
								gravSlider.value = Mathf.Clamp (gravSlider.value - change, gravMin, gravMax);
							}
							break;
						case 8: {
								int change = (lengthMax - lengthMin) * scrollSpeedI / 100;
								lengthSlider.value = Mathf.Clamp (lengthSlider.value - change, lengthMin, lengthMax);
							}
							break;
						case 9: {
								int change = (widthMax - widthMin) * scrollSpeedI / 100;
								widthSlider.value = Mathf.Clamp (widthSlider.value - change, widthMin, widthMax);
							}
							break;
						case 10: {
								float change = (heightMax - heightMin) * scrollSpeedF / 100;
								heightSlider.value = Mathf.Clamp (heightSlider.value - change, heightMin, heightMax);
							}
							break;
						case 11: {
								float change = (massMax - massMin) * (scrollSpeedF / 100.0f);
								massSlider.value = Mathf.Clamp (massSlider.value - change, massMin, massMax);
							}
							break;
					}
				}
			}
		}

		anim.SetBool ("show", menu >= 0);
		anim.SetFloat ("menu", (menu * 1.0f) / 20.0f);
	}

	public void ApplyChanges () {
		float grav = gravSlider.value;
		int mapLength = (int) lengthSlider.value;
		int mapWidth = (int) widthSlider.value;
		int mapHeight = (int) heightSlider.value * 100;
		float mass = massSlider.value;
		GameplayManager.Instance.SandboxChange (grav, mapLength, mapWidth, mapHeight, mass);
		HideCanvas ();
	}

	public void HideCanvas () {
		if (anim == null) return;
		if (menu >= 0) {
			menu = -1;
			anim.SetBool ("show", false);
			hideTrigger = true;

			GameplayManager.Instance.FreezeBall (false);
		}
	}

	public void ShowCanvas (float grav, int length, int width, int height, float mass) {
		if (menu < 0) {
			menu = 0;
			PlaceCanvas ();
			anim.SetBool ("show", true);
			anim.SetFloat ("menu", 0.0f);

			gravSlider.value = grav;
			lengthSlider.value = length;
			widthSlider.value = width;
			heightSlider.value = height;
			massSlider.value = mass;

			GameplayManager.Instance.FreezeBall (true);
		}
	}

	private void PlaceCanvas () {
		//kalkulasi target di world position
		Vector3 playerFwdTemp = player.forward;
		float yRot = player.eulerAngles.y;
		Vector3 playerFwd = Quaternion.AngleAxis (yRot, Vector3.up) * Vector3.forward;
		Vector3 pos = player.position + playerFwd;

		//letakkan canvas
		transform.position = pos;
		transform.LookAt (player);
		transform.Rotate (Vector3.up, 180.0f);
	}

	public bool IsActive {
		get {
			return menu >= 0;
		}
	}

	private void OnEnable () {
		SceneManager.sceneLoaded += SceneLoadCheck;
	}

	private void SceneLoadCheck (Scene scene, LoadSceneMode mode) {
		if (scene.name.Equals ("menu")) {
			HideCanvas ();
		}
	}
}
