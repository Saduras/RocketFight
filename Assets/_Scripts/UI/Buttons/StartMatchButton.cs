using UnityEngine;
using System.Collections;

public class StartMatchButton : MonoBehaviour {

	void Update() {
		if(PhotonNetwork.isMasterClient && PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
			GetComponent<UISprite>().enabled = true;
			GetComponent<BoxCollider>().enabled = true;
		} else {
			GetComponent<UISprite>().enabled = false;
			GetComponent<BoxCollider>().enabled = false;
		}
	}
	
	public void OnClick() {
		PhotonNetwork.room.open = false;
		PhotonNetwork.room.visible = false;
	}
}
