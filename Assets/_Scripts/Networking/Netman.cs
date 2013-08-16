using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Netman : Photon.MonoBehaviour {
	
	// Name of the game scene. Will be loaded on joining a room.
	public string gameScene = "Arena";
	
	public Match match;
	public UILabel label;
	public UIMenu uimenu;
	private int playerCountRoom = 0;
	
	
	/**
	 * Initialize PhotonNetwork settings.
	 */
    public virtual void Start ()
    {
		// Call OnConnectedToMaster() instead of auto join a the lobby
        PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.sendRate = 15;
		PhotonNetwork.sendRateOnSerialize = 15;
    }
	
	/**
	 * Check each frame if player count in this room has changed
	 * and call ReloadPlayerList if so.
	 */ 
	public void Update() {
		if( PhotonNetwork.room != null )
				if( playerCountRoom != PhotonNetwork.room.playerCount ) {
						photonView.RPC("ReloadPlayerList",PhotonTargets.All);
						playerCountRoom = PhotonNetwork.room.playerCount;
					}
	}
	
	/**
	 * If player leave the match, remove all objects owned by this player
	 */ 
	public virtual void OnPhotonPlayerDisconnected(PhotonPlayer player) {
		if(PhotonNetwork.isMasterClient) {
			PhotonNetwork.DestroyPlayerObjects(player);
		}
		match.ReloadPlayerList();
		
		if(match.IsRunning()) {
			PhotonNetwork.Disconnect();
			// activate label if it is inactive
			if( !label.gameObject.activeSelf )
				label.gameObject.SetActive( true );
			
			// update text
			label.text = "Match closed,\n because player disconnected!";
			// init fade via TweenColor
			label.color = Color.white;	
			TweenColor.Begin(label.gameObject, 1.5f, new Color(1,1,1,0));
			
			uimenu.ChanceState(UIMenu.UIState.MAINMENU);
		}
	}
	
	/**
	 * This is called when the client fails to connect to the server.
	 * Print out the error message.
	 */
	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }
	
	/**
	 * Called when the connection to the master server is established.
	 * Then join a random room for now.
	 */
    public virtual void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }
	
	/**
	 * Called if joining a random room failed. 
	 * Therefore create a new room.
	 */
    public virtual void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: (null, true, true, 4);");
        PhotonNetwork.CreateRoom("RocketMatch" + Random.Range(0,10000000), true, true, 4);
    }
	
	/**
	 * Called on joining a room.
	 * Find a spawnpoint in the room for you and instatiate the player prefab.
	 */
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		match.Reset();
    }	
}
