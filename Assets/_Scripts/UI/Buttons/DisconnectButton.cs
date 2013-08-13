using UnityEngine;
using System.Collections;

public class DisconnectButton : MonoBehaviour {
	
	/**
	 * Disconnect from photon network on click
	 */ 
	public void OnClick() {
		Debug.Log("Disconnecting");
		PhotonNetwork.Disconnect();	
		GameObject.Find("PhotonNetman").GetComponent<Match>().Reset();
	}
}
