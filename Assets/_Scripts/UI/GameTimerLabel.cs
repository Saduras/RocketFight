using UnityEngine;
using System.Collections;

public class GameTimerLabel : MonoBehaviour {
	
	
	private UILabel label;
	private Netman nman;
	
	// Use this for initialization
	void Start () {
		label = gameObject.GetComponent<UILabel>();
		nman = GameObject.Find("PhotonNetman").GetComponent<Netman>();
	}
	
	// Update is called once per frame
	void Update () {
		float timer = (nman.gameTime - Time.time + nman.startTime);
		string minutes = Mathf.Floor(timer / 60).ToString("00");
		string seconds = (timer % 60).ToString("00");
		label.text = "Time: " + minutes + ":" + seconds;
	}
}
