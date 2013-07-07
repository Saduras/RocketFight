using UnityEngine;
using System.Collections;

public class GameTimerLabel : MonoBehaviour {
	
	
	private UILabel label;
	public Match match;
	
	// Use this for initialization
	void Start () {
		label = gameObject.GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
		float timer = match.gameTime;
		string minutes = Mathf.Floor(timer / 60).ToString("00");
		string seconds = (timer % 60).ToString("00");
		label.text = "Time: " + minutes + ":" + seconds;
	}
}
