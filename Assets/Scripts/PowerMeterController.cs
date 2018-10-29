using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerMeterController : MonoBehaviour {

	private bool active = false;
	public float minForce = 0.0f;
	public float maxForce = 100.0f;
	private float currentForce;

	public float powerChangeSpeed = 10.0f;

	private Image meterImg;

	private void Start () {
		meterImg = GetComponent<Image> ();
	}

	public void Activate () {
		active = true;
		currentForce = 0.0f;
	}

	public float GetForce () {
		return currentForce;
	}

	public void Deactivate () {
		active = false;
		currentForce = 0.0f;
	}

	private void Update () {
		if (active) {
			/*float y = Input.GetAxis ("Vertical");
			currentForce += y * powerChangeSpeed * Time.deltaTime;*/

			if (Input.GetKey (KeyCode.RightShift) || Input.GetButton ("Jump")) {
				currentForce += powerChangeSpeed * Time.deltaTime;
			} else if (Input.GetKey (KeyCode.LeftShift) || Input.GetButton ("Jump2")) {
				currentForce -= powerChangeSpeed * Time.deltaTime;
			}

			currentForce = Mathf.Clamp (currentForce, minForce, maxForce);

			meterImg.fillAmount = (currentForce - minForce) / (maxForce - minForce);
		}
	}
}
