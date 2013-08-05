using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathPanel : MonoBehaviour {
	
	// label we use to display death notification
	public UILabel label;
	
	/**
	 * Update death message with the given player as killer.
	 * Message fades away via TweenColor
	 */ 
	public void Activate(PhotonPlayer player) {
		// find the RocketFightPlayer instance for the given player to get its color
		Match match = GameObject.Find("PhotonNetman").GetComponent<Match>();
		List<RocketFightPlayer> playerList = match.GetPlayerList();
		RocketFightPlayer rfplayer = null;
		foreach( RocketFightPlayer rfp in playerList ) {
			if( rfp.photonPlayer == player ) {
				rfplayer = rfp;
				break;
			}
		}
		
		// activate label if it is inactive
		if( !label.gameObject.activeSelf )
			label.gameObject.SetActive( true );
		
		// update text
		label.text = "[" + ColorX.RGBToHex(rfplayer.color) + "] " + rfplayer.photonPlayer.name + " [ffffff]has killed you!";
		// init fade via TweenColor
		label.color = Color.white;	
		TweenColor.Begin(label.gameObject, 1.5f, new Color(1,1,1,0));
	}
}
