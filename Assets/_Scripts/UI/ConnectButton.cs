using UnityEngine;
using System.Collections;

public class ConnectButton : MonoBehaviour {
	
	public UILabel usernameLabel;
	
	/** 
	 * Connect to PhotonCloud on click
	 * User the playername from label reference
	 */
	void OnClick() {
		PlayerPrefs.SetString("Playername",usernameLabel.text);
		PhotonNetwork.ConnectUsingSettings( "1" );
		PhotonNetwork.player.name = PlayerPrefs.GetString("Playername");
	}
}
