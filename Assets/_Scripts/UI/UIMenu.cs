using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {
	
	public UIPanel mainMenuPanel;
	public UIPanel enterNamePanel;
	public UIPanel lobbyPanel;
	
	private UIState currentState;
	
	public enum UIState {
		MAINMENU,
		ENTERNAME,
		LOBBY,
		CONNECTING,
		INGAME
	}
	
	public void ChanceState(UIState newState) {
		switch(newState) {
		case UIState.MAINMENU:
			mainMenuPanel.gameObject.SetActive(true);
			enterNamePanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(false);
			break;
		case UIState.ENTERNAME:
			mainMenuPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(true);
			lobbyPanel.gameObject.SetActive(false);
			break;
		case UIState.LOBBY:
			mainMenuPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(true);
			break;
		}
		
		currentState = newState;
	}
}
