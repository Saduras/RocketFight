using UnityEngine;
using System.Collections;

public class TriggerShake : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Camera.main.SendMessage("Shake");
	}
}
