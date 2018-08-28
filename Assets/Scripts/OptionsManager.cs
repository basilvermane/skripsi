using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (AudioListener))]
[RequireComponent (typeof (Animator))]
public class OptionsManager : MonoBehaviour {
	public static OptionsManager Instance {
		get {
			return instance;
		}
	}

	private static OptionsManager instance = null;

	private void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
			currentBGM = -1;
		} else {
			Destroy (gameObject);
		}
	}

	private int currentBGM;
	private int bgmVolume, sfxVolume;
	private bool inOptions;

	public AudioSource[] bgm, sfx;
	private Animator anim;

	public int scrollSpeed = 1;
	public float maxInputCooldown = 0.5f;
	private float inputCooldown;
	private int optionsMenu;
	/* 0 = bgm
	 * 1 = sfx
	 * 2 = inside bgm
	 * 3 = inside sfx
	 * 4 = back
	 */

	private MainMenuController currentMMC;

	public Slider bgmSlider, sfxSlider;

	private void Start () {
		anim = GetComponent<Animator> ();
		PlayBGM (0);
		inOptions = false;
		optionsMenu = 0;
		bgmVolume = 75;
		sfxVolume = 75;
	}

	private void Update () {
		if (inOptions) {
			//ambil input
			float y = Input.GetAxis ("Verr") + Input.GetAxis ("Vertical");
			float x = Input.GetAxis ("Horr") + Input.GetAxis ("Horizontal");

			if (Input.GetButtonDown ("Jump") || Input.GetKeyDown (KeyCode.RightShift)) {
				//select
				switch (optionsMenu) {
					case 0: {
							optionsMenu = 2;
						}
						break;
					case 1: {
							optionsMenu = 3;
						}
						break;
					case 4: {
							HideOptions ();
						}
						break;
				}
			}

			if (Input.GetButtonDown ("Jump2") || Input.GetKeyDown (KeyCode.LeftShift)) {
				//back
				switch (optionsMenu) {
					case 2: {
							optionsMenu = 0;
						}
						break;
					case 3: {
							optionsMenu = 1;
						}
						break;
				}
			}

			if (optionsMenu < 2 || optionsMenu > 3) {
				//pilih bgm / sfx
				if (inputCooldown <= 0.0f) {
					if (y < -0.1f) { //atas
						if (optionsMenu == 0)
							optionsMenu = 1;
						else if (optionsMenu == 1)
							optionsMenu = 4;
						else
							optionsMenu = 0;
						inputCooldown = maxInputCooldown;
					} else if (y > 0.1f) { //bawah
						if (optionsMenu == 0)
							optionsMenu = 4;
						else if (optionsMenu == 1)
							optionsMenu = 0;
						else
							optionsMenu = 1;
						inputCooldown = maxInputCooldown;
					}
				} else {
					inputCooldown -= Time.deltaTime;
					if (inputCooldown < 0.0f)
						inputCooldown = 0.0f;
				}
			} else if (optionsMenu == 2) {
				//dalam bgm
				if (x > 0.1f) {
					bgmVolume += scrollSpeed;
					if (bgmVolume > 100)
						bgmVolume = 100;
				} else if (x < -0.1f) {
					bgmVolume -= scrollSpeed;
					if (bgmVolume < 0)
						bgmVolume = 0;
				}
			} else if (optionsMenu == 3) {
				//dalam sfx
				if (x > 0.1f) {
					sfxVolume += scrollSpeed;
					if (sfxVolume > 100)
						sfxVolume = 100;
				} else if (x < -0.1f) {
					sfxVolume -= scrollSpeed;
					if (sfxVolume < 0)
						sfxVolume = 0;
				}
			}
		}

		anim.SetFloat ("menu", optionsMenu * 0.25f);

		bgmSlider.value = bgmVolume;
		foreach (AudioSource source in bgm) {
			source.volume = (bgmVolume * 1.0f) / 100.0f;
		}

		sfxSlider.value = sfxVolume;
		foreach (AudioSource source in sfx) {
			source.volume = (sfxVolume * 1.0f) / 100.0f;
		}
	}

	public void PlaySFX (int sfxID) {
		sfx[sfxID].Play ();
	}

	public void PlayBGM (int bgmID) {
		if (currentBGM == bgmID) {
			return;
		}
		if (currentBGM >= 0)
			bgm[currentBGM].Stop ();
		currentBGM = bgmID;
		bgm[currentBGM].Play ();
	}

	public void ShowOptions (MainMenuController mmc) {
		anim.SetBool ("show", true);
		inOptions = true;
		optionsMenu = 0;
		currentMMC = mmc;
	}

	public void HideOptions () {
		currentMMC.BackFromOptions ();
		anim.SetBool ("show", false);
		inOptions = false;
	}
}
