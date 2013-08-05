using UnityEngine;
using System.Collections;

public class GameTimerLabel : MonoBehaviour {
	
	
	private UILabel label;
	public Match match;
	
	// Use this for initialization
	void Start () {
		label = gameObject.GetComponent<UILabel>();
	}
	
	
	/**
	 * Split game time into secounds and minutes and display them as
	 * mm:ss
	 */
	void Update () {
		float timer = match.GetGameTime();
		string minutes = Mathf.Floor(timer / 60).ToString("00");
		string seconds = (timer % 60).ToString("00");
		label.text = "Time: " + minutes + ":" + seconds;
	}
}
