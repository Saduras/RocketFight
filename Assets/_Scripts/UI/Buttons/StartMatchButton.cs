using UnityEngine;
using System.Collections;

public class StartMatchButton : MonoBehaviour {
	
	public GameObject targetBackground;
	public GameObject targetSprite;

	void Update() {
		// TODO: doenst hide properly with new prefab
		if(PhotonNetwork.isMasterClient && PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
			targetBackground.GetComponent<UISprite>().enabled = true;
			targetBackground.GetComponent<BoxCollider>().enabled = true;
			targetSprite.GetComponent<UISprite>().enabled = true;
		} else {
			targetBackground.GetComponent<UISprite>().enabled = false;
			targetBackground.GetComponent<BoxCollider>().enabled = false;
			targetSprite.GetComponentInChildren<UISprite>().enabled = false;
		}
	}
	
	public void OnClick() {
		PhotonNetwork.room.open = false;
		PhotonNetwork.room.visible = false;
	}
}
