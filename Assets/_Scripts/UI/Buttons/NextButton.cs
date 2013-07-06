using UnityEngine;
using System.Collections;

public class NextButton : MonoBehaviour {
	
	public UILabel playerNameLabel;
	public UIMenu uiMenu;
	
	
	void OnClick() {
		// set playername from input
		PlayerPrefs.SetString("Playername",playerNameLabel.text);
		PhotonNetwork.player.name = PlayerPrefs.GetString("Playername");
		// start connection process
		PhotonNetwork.ConnectUsingSettings( "1" );
		// change UI state
		uiMenu.ChanceState(UIMenu.UIState.LOBBY);
	}
}
