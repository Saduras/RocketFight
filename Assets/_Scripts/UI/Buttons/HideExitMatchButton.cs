using UnityEngine;
using System.Collections;

public class HideExitMatchButton : MonoBehaviour {
	
	public UIPanel exitMatchPanel;
	
	public void OnClick() {
		exitMatchPanel.gameObject.SetActive( false );
	}
}
