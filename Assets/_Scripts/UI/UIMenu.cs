using UnityEngine;
using System.Collections;

public class UIMenu : Photon.MonoBehaviour {
	
	public Match match;
	public UIPanel mainMenuPanel;
	public UIPanel enterNamePanel;
	public UIPanel lobbyPanel;
	public UIPanel inGamePanel;
	
	private UIState currentState;
	
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
			match.UpdateUIPlayerList();
			break;
		case UIState.INGAME:
			mainMenuPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(false);
			inGamePanel.gameObject.SetActive(true);
			
			if(PhotonNetwork.isMasterClient) {
				photonView.RPC("ChangeToInGame",PhotonTargets.Others);
				match.photonView.RPC("RequestStart",PhotonTargets.AllBuffered);
			}
			break;
		case UIState.QUIT:
			Application.Quit();
			break;
		}
		currentState = newState;
	}
	
	[RPC]
	public void ChangeToInGame() {
		mainMenuPanel.gameObject.SetActive(false);
		enterNamePanel.gameObject.SetActive(false);
		lobbyPanel.gameObject.SetActive(false);
		inGamePanel.gameObject.SetActive(true);
	}
	
}
