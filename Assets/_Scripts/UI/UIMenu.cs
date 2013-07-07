using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {
	
	public string arenaScene = "Arena";
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
		case UIState.INGAME:
			mainMenuPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(false);
			if( Application.loadedLevelName != arenaScene)
				Application.LoadLevel(arenaScene);
			break;
		}
		currentState = newState;
	}
}
