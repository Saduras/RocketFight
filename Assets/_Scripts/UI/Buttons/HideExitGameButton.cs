using UnityEngine;
using System.Collections;

public class HideExitGameButton : MonoBehaviour {
	
	public UIPanel exitGamePanel;
	
	public void OnClick() {
		exitGamePanel.gameObject.SetActive( false );;
	}
}
