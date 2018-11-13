using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	private Animator mainMenuAnim;
	private int menuSelect;
	/* 0 = play
	 * 1 = options
	 * 2 = exit
	 * 3 = puzzle
	 * 4 = sandbox
	 * 5 = inside options
	 * 6 = pick level
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
		if (LoadingManager.Instance.IsLoading) {
			return;
		}

		if (menuSelect != 5) {

			float y = Input.GetAxis ("Verr") + Input.GetAxis ("Vertical");

			//select
			if (Input.GetButtonDown ("Jump") || Input.GetKeyDown (KeyCode.RightShift)) {
				switch (menuSelect) {
					case 0: {
							//start
							menuSelect = 3;
						}
						break;
					case 1: {
							//options
							OptionsManager.Instance.ShowOptions (this);
							menuSelect = 5;
						}
						break;
					case 2: {
							//exit
							Application.Quit ();
						}
						break;
					case 3: {
							//puzzle
							menuSelect = 6;
							levelAnimController.ShowLevelSelect ();
						}
						break;
					case 4: {
							//sandbox
							StartCoroutine (LoadingManager.Instance.LoadPuzzle (-1));
						}
						break;
					case 6: {
							//select level
							int level = levelAnimController.GetCurrentLevel ();
							print ("level " + level);
							StartCoroutine (LoadingManager.Instance.LoadPuzzle (level));
						}
						break;
				}
			}

			//back
			if (Input.GetButtonDown ("Jump2") || Input.GetKeyDown (KeyCode.LeftShift)) {
				switch (menuSelect) {
					case 0:
					case 1: {
							menuSelect = 2;
						}
						break;
					case 3:
					case 4: {
							menuSelect = 0;
						}
						break;
					case 6: {
							levelAnimController.HideLevelSelect ();
							menuSelect = 3;
						}
						break;
				}
			}

			//joystick
			if (inputCooldown <= 0.0f) {
				if (y > 0.1f) { //atas
					switch (menuSelect) {
						case 0: {
								menuSelect = 2;
							}
							break;
						case 1:
						case 2: {
								menuSelect--;
							}
							break;
						case 3: {
								menuSelect = 4;
							}
							break;
						case 4: {
								menuSelect = 3;
							}
							break;
						default: break;
					}
					inputCooldown = maxInputCooldown;
				} else if (y < -0.1f) { //bawah
					switch (menuSelect) {
						case 0:
						case 1: {
								menuSelect++;
							}
							break;
						case 2: {
								menuSelect = 0;
							}
							break;
						case 3: {
								menuSelect = 4;
							}
							break;
						case 4: {
								menuSelect = 3;
							}
							break;
						default: break;
					}
					inputCooldown = maxInputCooldown;
				}

				if (menuSelect == 6) {
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
			if (menuSelect == 5) {
				menuSelect = 1;
			}
			backFromOptions = false;
		}

		mainMenuAnim.SetInteger ("menu", menuSelect);
	}
}
