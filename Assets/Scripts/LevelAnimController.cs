using UnityEngine;
using System.Collections;

public class LevelAnimController : MonoBehaviour {

	private Animator anim;
	public Animator[] levelAnim;

	private int currentLevel; //<0 = not shown

	// Use this for initialization
	private void Start () {
		anim = GetComponent<Animator> ();
	}

	public void ChangeCurrentLevel (bool right) {
		if (right) { //tekan kanan
			if (currentLevel == levelAnim.Length - 1) {
				levelAnim[currentLevel].SetBool ("kanan", false);
				levelAnim[0].SetBool ("kanan", true);
				levelAnim[currentLevel].SetBool ("show", false);
				levelAnim[0].SetBool ("show", true);
				currentLevel = 0;
			} else {
				levelAnim[currentLevel].SetBool ("kanan", false);
				levelAnim[currentLevel + 1].SetBool ("kanan", true);
				levelAnim[currentLevel].SetBool ("show", false);
				levelAnim[currentLevel + 1].SetBool ("show", true);
				currentLevel++;
			}
		} else { //tekan kiri
			if (currentLevel == 0) {
				levelAnim[0].SetBool ("kanan", true);
				levelAnim[levelAnim.Length - 1].SetBool ("kanan", false);
				levelAnim[0].SetBool ("show", false);
				levelAnim[levelAnim.Length - 1].SetBool ("show", true);
				currentLevel = levelAnim.Length - 1;
			} else {
				levelAnim[currentLevel].SetBool ("kanan", true);
				levelAnim[currentLevel - 1].SetBool ("kanan", false);
				levelAnim[currentLevel].SetBool ("show", false);
				levelAnim[currentLevel - 1].SetBool ("show", true);
				currentLevel--;
			}
		}
	}

	public int GetCurrentLevel () {
		return currentLevel;
	}

	public void ShowLevelSelect () {
		currentLevel = 0;
		anim.SetBool ("show", true);
		levelAnim[currentLevel].SetTrigger ("instantShow");
	}

	public void HideLevelSelect () {
		anim.SetBool ("show", false);
		levelAnim[currentLevel].SetTrigger ("instantHide");
	}
}
