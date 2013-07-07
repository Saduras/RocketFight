using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Netman : Photon.MonoBehaviour {
	
	// Name of the game scene. Will be loaded on joining a room.
	public string gameScene = "Arena";
	// Tag that identifies spawn points.
	public string respawnTag = "Respawn";
	// The prefabe we will spawn for the player.
	public GameObject playerPrefab;
	// Tells you if the player had spawn a character or not.
	public bool hasSpawn = false;
	
	public UILabel playerListLabel;
	
	public float startTime;
	public float gameTime = 180;
	
	public Match match;
	
	public Color[] playerColors = new Color[]{Color.red, Color.blue, Color.green, Color.yellow};
	public List<Color> freeColors = new List<Color>();
	public Dictionary<int, Color> usedColors = new Dictionary<int, Color>();
	
	public Dictionary<int, int> playerScores = new Dictionary<int, int>();
	
	public List<RocketFightPlayer> playerList = new List<RocketFightPlayer>();
	
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
		
		foreach( Color col in playerColors ) {
			freeColors.Add( col );	
		}
    }
	
	public void Update() {
		if( PhotonNetwork.isMasterClient ) {			
			if( (startTime + gameTime < Time.time) && hasSpawn )
				GameOver();	
		}
		
		if( PhotonNetwork.room != null )
				if( playerCountRoom > PhotonNetwork.room.playerCount ) {
						OnPlayerDisconnect();
						playerCountRoom = PhotonNetwork.room.playerCount;
					} else if( playerCountRoom < PhotonNetwork.room.playerCount ) {
						OnPlayerConnect();
						playerCountRoom = PhotonNetwork.room.playerCount;
					}
		
		//DisplayPlayerList();
	}
	
	
	public void OnPlayerDisconnect() {
		Debug.Log("OnPlayerDisconnect");
		
		List<int> validIDs = new List<int>();
		// get list of connected player IDs
		foreach( KeyValuePair<int, Color> pair in usedColors ) {
			validIDs.Add(pair.Key);
		}
		
		// find ID of disconnected player
		foreach( PhotonPlayer p in PhotonNetwork.playerList ) {
			validIDs.Remove(p.ID);
		}
		
		List<RocketFightPlayer> removePlayer = new List<RocketFightPlayer>();
		foreach( RocketFightPlayer rfp in playerList ) {
			if( validIDs.Contains( rfp.photonPlayer.ID ) ) {
				removePlayer.Add( rfp );	
			}
		}
		
		foreach( RocketFightPlayer rfp in removePlayer ) {
			photonView.RPC("RemovePlayer",PhotonTargets.All, rfp.photonPlayer);
		}
	}
	
	public void OnPlayerConnect() {
		Debug.Log("OnPlayerConnect");
		
		// get list of connected player IDs
		List<int> validIDs = new List<int>();
		foreach( PhotonPlayer p in PhotonNetwork.playerList ) {
				validIDs.Add(p.ID);
		}
		
		// find ID of disconnected player
		foreach( KeyValuePair<int, Color> pair in usedColors ) {
			validIDs.Remove(pair.Key);
		}
		
		
		// remove assign to usedColor
		foreach( int playerID in validIDs ) {
			if( PhotonNetwork.isMasterClient ) {
				foreach( RocketFightPlayer rfp in playerList ) {
					photonView.RPC("AddPlayer",PhotonPlayer.Find( playerID ), rfp.photonPlayer );	
				}

				photonView.RPC("AddPlayer",PhotonTargets.All, PhotonPlayer.Find( playerID ) );
			}
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
        PhotonNetwork.CreateRoom(null, true, true, 4);
    }
	
	/**
	 * Called on joining a room.
	 * Find a spawnpoint in the room for you and instatiate the player prefab.
	 */
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		match.Init();
    }
	
	public void OnLeftRoom() {
		playerList.Clear();
		//Application.LoadLevel(0);
	}
	
	public void GameOver() {
		// kill player objects
		if( PhotonNetwork.isMasterClient ) {
			foreach( PhotonPlayer player in PhotonNetwork.playerList ) {
				PhotonNetwork.DestroyPlayerObjects( player );
				photonView.RPC("BackToMenu",PhotonTargets.AllBuffered);
			}
		}
	}
	
	[RPC]
	public void BackToMenu() {
		hasSpawn = false;
		Screen.showCursor = true;
	}
	
	public RocketFightPlayer GetPlayer( int playerID ) {
		foreach( RocketFightPlayer rfp in playerList ) {
			if( rfp.photonPlayer.ID == playerID )
				return rfp;
		}
		return null;
	}
	
	[RPC]
	public void SetScore(int playerID, int val) {
		playerScores[playerID] = val;
	}
	
	[RPC]
	public void IncreaseScore(int playerID, int val) {
		foreach( RocketFightPlayer rfp in playerList ) {
			if( rfp.photonPlayer.ID == playerID ) {
				rfp.score += val;
				
				if( rfp.photonPlayer == PhotonNetwork.player ) {
					GameObject[] characterObjects = GameObject.FindGameObjectsWithTag("Player");
					foreach( GameObject character in characterObjects ) {
						if( character.GetPhotonView().owner == rfp.photonPlayer ) {
							character.GetComponent<PlayerManager>().PopupScore( val );
						}
					}
				}
			}
		}
	}
	
	/*[RPC]
	public void AddPlayer(PhotonPlayer player, Vector3 rgb) {
		RocketFightPlayer rfp = new RocketFightPlayer(player);
		rfp.color = new Color(rgb.x, rgb.y, rgb.z, 1);
		
		playerList.Add( rfp);
	}
	
	[RPC]
	public void RemovePlayer(int playerID) {
		foreach( RocketFightPlayer rfp in playerList ) {
			Debug.Log( rfp.ToString() );
			if( rfp.photonPlayer.ID == playerID ) {
				playerList.Remove( rfp );	
				return;
			} 
		}
		Debug.Log("Did not found player: " + playerID);
	}*/
	
	
	
	
}
