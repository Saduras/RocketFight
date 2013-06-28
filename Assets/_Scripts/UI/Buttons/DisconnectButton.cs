using UnityEngine;
using System.Collections;

public class DisconnectButton : MonoBehaviour {

	public void OnClick() {
		PhotonNetwork.Disconnect();	
	}
}
