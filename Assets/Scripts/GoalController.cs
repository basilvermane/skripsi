using UnityEngine;
using System.Collections;

public class GoalController : MonoBehaviour {
	private void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("Ball")) {
			GameplayManager.Instance.FinishGame ();
		}
	}
}
