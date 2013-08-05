using UnityEngine;
using System.Collections;

public class TriggerShake : MonoBehaviour {

	
	/**
	 * On instatiation let the main camera shake a little.
	 */ 
	void Start () {
		Camera.main.SendMessage("Shake");
	}
}
