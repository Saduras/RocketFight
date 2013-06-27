using UnityEngine;
using System.Collections;

public class MenuPanelController : MonoBehaviour {

	public GameObject mainMenu;
	
	void Update() {
		if( PhotonNetwork.connectionState == ConnectionState.Disconnected ) {
			mainMenu.SetActive(true);	
		} else {
			mainMenu.SetActive(false);	
		}
	}
}
