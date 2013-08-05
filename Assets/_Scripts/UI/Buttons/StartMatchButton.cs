using UnityEngine;
using System.Collections;

public class StartMatchButton : MonoBehaviour {
	
	// we want to hide/unhide the button depending if we are master client or not
	public UISprite targetBackground;
	public UISprite targetSprite;
	
	/**
	 * Check if the button should be displayed and hide it otherwise
	 */ 
	void Update() {
		if(PhotonNetwork.isMasterClient && PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
			// we are allowed to start a match: show button
			targetBackground.enabled = true;
			targetBackground.gameObject.GetComponent<BoxCollider>().enabled = true;
			targetSprite.enabled = true;
		} else {
			// we are not allowed to start a match: hide button
			targetBackground.enabled = false;
			targetBackground.gameObject.GetComponent<BoxCollider>().enabled = false;
			targetSprite.enabled = false;
		}
	}
	
	/**
	 * Lock & hide room on match start
	 */ 
	public void OnClick() {
		PhotonNetwork.room.open = false;
		PhotonNetwork.room.visible = false;
	}
}
