using UnityEngine;
using System.Collections;

public class MenuPanelController : MonoBehaviour {

	public GameObject mainMenu;
	public GameObject connectingPanel;
	public GameObject lobby;
	
	private PeerState lastPeerState;
	private Netman nman;
	
	void Start() {
		nman = GameObject.Find("PhotonNetman").GetComponent<Netman>();
	}
	
	void Update() {
		// display center GUI only if game ist 
		if( !nman.hasSpawn ) {
			
			//if( lastPeerState != PhotonNetwork.connectionStateDetailed || lastPeerState == null) {
				
				switch(PhotonNetwork.connectionStateDetailed) {
				case PeerState.Disconnected:
				case PeerState.PeerCreated:
					mainMenu.SetActive(true);
					connectingPanel.SetActive(false);
					lobby.SetActive(false);
					break;
				case PeerState.Joined:
					mainMenu.SetActive(false);
					connectingPanel.SetActive(false);
					lobby.SetActive(true);
					break;
				default:
					mainMenu.SetActive(false);
					connectingPanel.SetActive(true);
					lobby.SetActive(false);
					break;
				}
				lastPeerState = PhotonNetwork.connectionStateDetailed;
			//}
		} else {
			mainMenu.SetActive(false);
			connectingPanel.SetActive(false);
			lobby.SetActive(false);
		}
		
	}
}
