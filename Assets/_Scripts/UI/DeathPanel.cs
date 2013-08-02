using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathPanel : MonoBehaviour {
	
	public UILabel label;
	
	void Start() {	
	}

	public void Activate(PhotonPlayer player) {
		Match match = GameObject.Find("PhotonNetman").GetComponent<Match>();
		List<RocketFightPlayer> playerList = match.GetPlayerList();
		RocketFightPlayer rfplayer = null;
		foreach( RocketFightPlayer rfp in playerList ) {
			if( rfp.photonPlayer == player ) {
				rfplayer = rfp;
				break;
			}
		}
		
		if( !label.gameObject.activeSelf )
			label.gameObject.SetActive( true );
		
		label.color = Color.white;
		label.text = "[" + ColorX.RGBToHex(rfplayer.color) + "] " + rfplayer.photonPlayer.name + " [ffffff]has killed you!";
		TweenColor.Begin(label.gameObject, 1.5f, new Color(1,1,1,0));
	}
}
