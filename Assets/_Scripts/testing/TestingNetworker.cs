using UnityEngine;
using System.Collections;

public class TestingNetworker : Photon.MonoBehaviour {
	
	public string roomName = "testingarea";
	public UILabel connectionStatusLabel;
	public GameObject extrapolCube;
	
	// Use this for initialization
	void Start () {
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.ConnectUsingSettings( "1" );
	}
	
	public virtual void OnConnectedToMaster() {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        // check if room exists and join if so
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach( RoomInfo ri in roomList ) {
			if( ri.name ==	roomName ) {
				PhotonNetwork.JoinRoom( roomName );
			}
		}
		// room doesnt exist 
		// so go to creat it
		PhotonNetwork.CreateRoom( roomName );
    }
	
	public void OnJoinedRoom() {
		if( PhotonNetwork.isMasterClient ) {
			PhotonNetwork.Instantiate( extrapolCube.name, new Vector3(0f,0.5f,0f),Quaternion.identity,0);	
		}
	}
	
	// Update is called once per frame
	void Update () {
		connectionStatusLabel.text = PhotonNetwork.connectionStateDetailed.ToString();
		if( PhotonNetwork.room != null ) {
			connectionStatusLabel.text += "\n" + "Player count: " + PhotonNetwork.room.playerCount;
			if( PhotonNetwork.isMasterClient ) {
				connectionStatusLabel.text += "\n" + "Master";	
			} else {
				connectionStatusLabel.text += "\n" + "Client";	
			}
		}
	}
}
