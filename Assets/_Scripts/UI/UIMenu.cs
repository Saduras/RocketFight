using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {
	
	public string arenaScene = "Arena";
	public Match match;
	public UIPanel mainMenuPanel;
	public UIPanel enterNamePanel;
	public UIPanel lobbyPanel;
	public UIPanel inGamePanel;
	
	private UIState currentState;
	private bool arenaLoaded = false;
	private bool sent = false;
	
	public enum UIState {
		MAINMENU,
		ENTERNAME,
		LOBBY,
		CONNECTING,
		INGAME,
		QUIT
	}
	
	public void ChanceState(UIState newState) {
		switch(newState) {
		case UIState.MAINMENU:
			mainMenuPanel.gameObject.SetActive(true);
			enterNamePanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(false);
			inGamePanel.gameObject.SetActive(false);
			break;
		case UIState.ENTERNAME:
			mainMenuPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(true);
			lobbyPanel.gameObject.SetActive(false);
			inGamePanel.gameObject.SetActive(false);
			break;
		case UIState.LOBBY:
			mainMenuPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(true);
			inGamePanel.gameObject.SetActive(false);
			break;
		case UIState.INGAME:
			mainMenuPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(false);
			inGamePanel.gameObject.SetActive(true);
			if(!arenaLoaded) {
				Application.LoadLevelAdditive(arenaScene);
				arenaLoaded = true;
			}
			match.RequestStart();
			break;
		case UIState.QUIT:
			Application.Quit();
			break;
		}
		currentState = newState;
	}
	
	void Update() {
		if(!Application.isLoadingLevel && arenaLoaded && !sent) {	
			GameObject.Find("PhotonNetman").GetPhotonView().RPC("LoadingFinished",PhotonTargets.AllBuffered,PhotonNetwork.player);
			sent = true;
		}
	}
}
