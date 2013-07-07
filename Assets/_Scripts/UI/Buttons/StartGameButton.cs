using UnityEngine;
using System.Collections;

public class StartGameButton : MonoBehaviour {

	public void OnEnable() {
		if(!PhotonNetwork.isMasterClient) {
			gameObject.SetActive(false);	
		}
	}
	
	public void OnClick() {
		Netman nman = GameObject.Find("PhotonNetman").GetComponent<Netman>();
//		nman.OrganizeSpawning();
		PhotonNetwork.room.open = false;
		PhotonNetwork.room.visible = false;
	}
}
