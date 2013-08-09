using UnityEngine;
using System.Collections;

public class ExitGameToggle : MonoBehaviour {
	
	public Match match;
	public UIPanel exitGamePanel;
	
	// Update is called once per frame
	void Update () {
		// toggle exitGame panel acitvation when ESC pressed and match is running
		if( Input.GetButtonDown("Exit") && match.IsRunning() ) {
			exitGamePanel.gameObject.SetActive( !exitGamePanel.gameObject.activeSelf );
		}
	}
}
