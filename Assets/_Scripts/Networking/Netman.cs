using UnityEngine;
using System.Collections;

public class Netman : Photon.MonoBehaviour {
	
	// Name of the game scene. Will be loaded on joining a room.
	public string gameScene = "Arena";
	// Tag that identifies spawn points.
	public string respawnTag = "Respawn";
	// The prefabe we will spawn for the player.
	public GameObject playerPrefab;
	// Reference to the spawn point assigned to you.
	private GameObject spawnPoint;
	
	/**
	 * Initialize PhotonNetwork settings.
	 */
    public virtual void Start ()
    {
		// Call OnConnectedToMaster() instead of auto join a the lobby
        PhotonNetwork.autoJoinLobby = false;
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
        PhotonNetwork.CreateRoom(null, true, true, 4);
    }
	
	/**
	 * Called on joining a room.
	 * Find a spawnpoint in the room for you and instatiate the player prefab.
	 */
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		Application.LoadLevel(gameScene);
		//PlayerManager pman = handle.gameObject.GetComponent<PlayerManager>();
		//pman.SendMessage("SetPlayer",PhotonNetwork.player);
    }
	
	/**
	 * Called when new level was loaded.
	 * If we loaded the game scene, find a respawn point for you.
	 * Then spawn your player object at this spawn point.
	 */
	public void OnLevelWasLoaded(int level) {
		if( Application.loadedLevelName == gameScene ) {
			GameObject[] gos = GameObject.FindGameObjectsWithTag(respawnTag);
			foreach( GameObject spawngo in gos) {
				if( spawngo.GetComponent<RespawnPoint>().IsFree() ) {
					spawnPoint = spawngo;
					spawngo.GetComponent<RespawnPoint>().AssignTo(PhotonNetwork.player);
					break;
				}
			}
			
			if( spawnPoint == null ) 
				Debug.Log("No free spawnpoint found!");
			
			Debug.Log("Intatiate player at " + spawnPoint.transform.position);
			GameObject handle = PhotonNetwork.Instantiate(playerPrefab.name,spawnPoint.transform.position,Quaternion.identity,0);
			handle.GetComponent<InputManager>().SendMessage("SetPlayer", PhotonNetwork.player);
		}
		
	}
}
