using UnityEngine;
using System.Collections;

public class UIMenu : Photon.MonoBehaviour {
	
	public Match match;
	public UIPanel mainMenuPanel;
	public UIPanel controlPanel;
	public UIPanel enterNamePanel;
	public UIPanel connectingPanel;
	public UIPanel lobbyPanel;
	public UIPanel inGamePanel;
	public UIPanel afterMatchPanel;
	
	private UIState currentState;
	
	public enum UIState {
		MAINMENU,
		CONTROLS,
		ENTERNAME,
		LOBBY,
		CONNECTING,
		INGAME,
		MATCHOVER,
		QUIT
	}
	
	public void ChanceState(UIState newState) {
		switch(newState) {
		case UIState.MAINMENU:
			mainMenuPanel.gameObject.SetActive(true);
			controlPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(false);
			connectingPanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(false);
			inGamePanel.gameObject.SetActive(false);
			afterMatchPanel.gameObject.SetActive(false);
			break;
		case UIState.CONTROLS:
			mainMenuPanel.gameObject.SetActive(false);
			controlPanel.gameObject.SetActive(true);
			enterNamePanel.gameObject.SetActive(false);
			connectingPanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(false);
			inGamePanel.gameObject.SetActive(false);
			afterMatchPanel.gameObject.SetActive(false);
			break;
		case UIState.ENTERNAME:
			mainMenuPanel.gameObject.SetActive(false);
			controlPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(true);
			connectingPanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(false);
			inGamePanel.gameObject.SetActive(false);
			afterMatchPanel.gameObject.SetActive(false);
			break;
		case UIState.CONNECTING:
			mainMenuPanel.gameObject.SetActive(false);
			controlPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(false);
			connectingPanel.gameObject.SetActive(true);
			lobbyPanel.gameObject.SetActive(false);
			inGamePanel.gameObject.SetActive(false);
			afterMatchPanel.gameObject.SetActive(false);
			break;
		case UIState.LOBBY:
			mainMenuPanel.gameObject.SetActive(false);
			controlPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(false);
			connectingPanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(true);
			inGamePanel.gameObject.SetActive(false);
			afterMatchPanel.gameObject.SetActive(false);
			match.UpdateUIPlayerList();
			break;
		case UIState.INGAME:
			mainMenuPanel.gameObject.SetActive(false);
			controlPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(false);
			connectingPanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(false);
			inGamePanel.gameObject.SetActive(true);
			afterMatchPanel.gameObject.SetActive(false);
			
			if(PhotonNetwork.isMasterClient) {
				photonView.RPC("ChangeToInGame",PhotonTargets.Others);
				match.photonView.RPC("RequestStart",PhotonTargets.AllBuffered);
			}
			break;
		case UIState.MATCHOVER:
			mainMenuPanel.gameObject.SetActive(false);
			controlPanel.gameObject.SetActive(false);
			enterNamePanel.gameObject.SetActive(false);
			connectingPanel.gameObject.SetActive(false);
			lobbyPanel.gameObject.SetActive(false);
			inGamePanel.gameObject.SetActive(false);
			afterMatchPanel.gameObject.SetActive(true);
			match.UpdateUIPlayerList();
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
