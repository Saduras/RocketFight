using UnityEngine;
using System.Collections;

public class UIMenu : Photon.MonoBehaviour {
	
	public Match match;
	// You need to add the panels in the same order
	// as the UIStates, i.e.:
	// 0 -> MainMenuPanel
	// 1 -> ControlsPanel
	// and so on ...
	public UIPanel[] panels;
	
	public UISprite background;
	
	private UIState currentState;
	
	public enum UIState {
		MAINMENU,
		CONTROLS,
		CREDITS,
		ENTERNAME,
		LOBBY,
		CONNECTING,
		INGAME,
		MATCHOVER,
		QUIT
	}
	
	/**
	 * Activate MainMenu on start
	 */ 
	void Start() {
		ChanceState(UIState.MAINMENU);	
	}
	
	/**
	 * Hide all panels and unhide the one for the requested UIState.
	 * Also do Quit() if this was requested.
	 */ 
	public void ChanceState(UIState newState) {
		// deactivate all panels
		foreach( UIPanel p in panels )
			p.gameObject.SetActive( false );
		// activate the requested one
		if( (int) newState < panels.Length )
			panels[(int) newState].gameObject.SetActive(true);
		
		background.enabled = true;
		
		// do additional stuff for cases INGAME and QUIT
		switch(newState) {
			case UIState.INGAME:
				// forward change tu INGAME to all other clients
				if(PhotonNetwork.isMasterClient) {
					photonView.RPC("ChangeToInGame",PhotonTargets.Others);
					match.photonView.RPC("RequestStart",PhotonTargets.AllBuffered);
				}
			background.enabled = false;
				break;
			case UIState.MATCHOVER: {
				panels[(int) UIState.MATCHOVER].GetComponentInChildren<ScorePanel>().UpdateDisplay();
				break;
			}
			case UIState.QUIT:
				Application.Quit();
				break;
		}
		currentState = newState;
	}
	
	/**
	 * This allows to change the UIState via RPC to INGAME
	 */ 
	[RPC]
	public void ChangeToInGame() {
		ChanceState(UIState.INGAME);
	}
	
	/**
	 * Return the current state of the UI.
	 */ 
	public UIState GetState() {
		return currentState;	
	}
}
