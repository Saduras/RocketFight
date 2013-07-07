using UnityEngine;
using System.Collections;

public class DisconnectButton : MonoBehaviour {

	public void OnClick() {
		Debug.Log("Disconnecting");
		PhotonNetwork.Disconnect();	
	}
}
