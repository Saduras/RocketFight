using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {
	
	public string arenaScene = "Arena";
	public UIPanel mainMenuPanel;
	public UIPanel enterNamePanel;
	public UIPanel lobbyPanel;
	
	private UIState currentState;
	private bool arenaLoaded = false;
	private bool sent = false;
	
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
			if(!arenaLoaded) {
				Application.LoadLevelAdditive(arenaScene);
				arenaLoaded = true;
			}
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
