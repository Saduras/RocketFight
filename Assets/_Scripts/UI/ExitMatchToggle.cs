using UnityEngine;
using System.Collections;

public class ExitMatchToggle : MonoBehaviour {
	
	public Match match;
	public UIPanel exitMatchPanel;
	
	// Update is called once per frame
	void Update () {
		// toggle exitGame panel acitvation when ESC pressed and match is running
		if( Input.GetButtonDown("Exit") && match.IsRunning() ) {
			exitMatchPanel.gameObject.SetActive( !exitMatchPanel.gameObject.activeSelf );
		}
	}
}
