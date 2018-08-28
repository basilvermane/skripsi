using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	private Animator mainMenuAnim;
	private int menuSelect;
	/* 0 = play
	 * 1 = options
	 * 2 = credits
	 * 3 = exit
	 * 4 = puzzle
	 * 5 = sandbox
	 * 6 = inside options
	 * 7 = inside credits
	 * 8 = pick level
	 */
	
	public Text txt;

	public float maxInputCooldown = 0.5f;
	private float inputCooldown;

	public LevelAnimController levelAnimController;

	private void Start () {
		mainMenuAnim = GetComponent<Animator> ();
		menuSelect = 0;
		inputCooldown = 0.0f;
	}

	private bool backFromOptions = false;

	public void BackFromOptions () {
		backFromOptions = true;
	}

	private void Update () {
		if (menuSelect != 6) {

			float y = Input.GetAxis ("Verr") + Input.GetAxis ("Vertical");

			//select
			if (Input.GetButtonDown ("Jump") || Input.GetKeyDown (KeyCode.RightShift)) {
				switch (menuSelect) {
					case 0: {
							//start
							menuSelect = 4;
						}
						break;
					case 1: {
							//options
							OptionsManager.Instance.ShowOptions (this);
							menuSelect = 6;
						}
						break;
					case 2: {
							//credits
						}
						break;
					case 3: {
							//exit
							Application.Quit ();
						}
						break;
					case 4: {
							//puzzle
							menuSelect = 8;
							levelAnimController.ShowLevelSelect ();
						}
						break;
					case 5: {
							//sandbox
							StartCoroutine (LoadingManager.Instance.LoadPuzzle (LevelParameters.SandboxDefaultLevel));
						}
						break;
					case 7: {
							//credits
						}
						break;
					case 8: {
							//select level
							int level = levelAnimController.GetCurrentLevel ();
							StartCoroutine (LoadingManager.Instance.LoadPuzzle (level));
						}
						break;
				}
			}

			//back
			if (Input.GetButtonDown ("Jump2") || Input.GetKeyDown (KeyCode.LeftShift)) {
				switch (menuSelect) {
					case 0:
					case 1:
					case 2: {
							menuSelect = 3;
						}
						break;
					case 4:
					case 5: {
							menuSelect = 0;
						}
						break;
					case 7: {
							menuSelect = 2;
						}
						break;
					case 8: {
							levelAnimController.HideLevelSelect ();
							menuSelect = 4;
						}
						break;
				}
			}

			//joystick
			if (inputCooldown <= 0.0f) {
				if (y > 0.1f) { //atas
					switch (menuSelect) {
						case 0: {
								menuSelect = 3;
							}
							break;
						case 1:
						case 2:
						case 3: {
								menuSelect--;
							}
							break;
						case 4: {
								menuSelect = 5;
							}
							break;
						case 5: {
								menuSelect = 4;
							}
							break;
						default: break;
					}
					inputCooldown = maxInputCooldown;
				} else if (y < -0.1f) { //bawah
					switch (menuSelect) {
						case 0:
						case 1:
						case 2: {
								menuSelect++;
							}
							break;
						case 3: {
								menuSelect = 0;
							}
							break;
						case 4: {
								menuSelect = 5;
							}
							break;
						case 5: {
								menuSelect = 4;
							}
							break;
						default: break;
					}
					inputCooldown = maxInputCooldown;
				}

				if (menuSelect == 8) {
					float x = Input.GetAxis ("Horr") + Input.GetAxis ("Horizontal");
					if (x > 0.1f) { //kanan
						levelAnimController.ChangeCurrentLevel (true);
						inputCooldown = maxInputCooldown;
					} else if (x < -0.1f) { //kiri
						levelAnimController.ChangeCurrentLevel (false);
						inputCooldown = maxInputCooldown;
					}
				}
				//kiri
			} else {
				inputCooldown -= Time.deltaTime;
				if (inputCooldown < 0.0f)
					inputCooldown = 0.0f;
			}
		}

		if (backFromOptions) {
			if (menuSelect == 6) {
				menuSelect = 1;
			}
			backFromOptions = false;
		}

		mainMenuAnim.SetInteger ("menu", menuSelect);
	}
}
