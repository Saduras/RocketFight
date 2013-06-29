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
		
		DisplayPlayerList();
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
			//playerList.Remove( rfp );	
			photonView.RPC("RemovePlayer",PhotonTargets.All, rfp.photonPlayer.ID);
		}
		
		// remove free color
		foreach( int playerID in validIDs ) {
			Color col = usedColors[playerID];
			freeColors.Add( col );
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
			// create RocketFightPlayer
			//playerList.Add(new RocketFightPlayer( PhotonPlayer.Find(playerID) ));
			
			if( freeColors.Count > 0 ) {
				Color col = freeColors[ freeColors.Count - 1 ];
				freeColors.Remove( col );
				usedColors[playerID] = col;
				
				// masterclient assigns color to player
				if( PhotonNetwork.isMasterClient ) {
					Vector3 rgb = new Vector3( col.r, col.g, col.b );
					
					foreach( RocketFightPlayer rfp in playerList ) {
						Vector3 colVec = new Vector3( rfp.color.r, rfp.color.g, rfp.color.b );
						photonView.RPC("AddPlayer",PhotonPlayer.Find( playerID ), rfp.photonPlayer, colVec );	
					}
					
					photonView.RPC("AddPlayer",PhotonTargets.All, PhotonPlayer.Find( playerID ), rgb );
				}
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
		PhotonNetwork.LoadLevel(gameScene);
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
			
			// free spawnpoints
			GameObject[] gos = GameObject.FindGameObjectsWithTag(respawnTag);
			for( int i=0; i<gos.Length; i++) {
				gos[i].GetPhotonView().RPC("SetFree",PhotonTargets.AllBuffered);
			}
		}
	}
	
	private void DisplayPlayerList() {
		string labelString = "Playerlist:\n";
		foreach( RocketFightPlayer rfp in playerList ) {
			labelString += "[" + ColorX.RGBToHex(rfp.color) + "]";
			labelString += rfp.photonPlayer.name;
			labelString += "[ffffff]";
			if( rfp.photonPlayer.isMasterClient ) 
				labelString += " (Master)";
			labelString += "\n";
		}
		
		playerListLabel.text = labelString;
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
	
	public void OrganizeSpawning() {
		if( Application.loadedLevelName == gameScene) {
			// find spawnpoints
			GameObject[] gos = GameObject.FindGameObjectsWithTag(respawnTag);
			List<GameObject> spawnPoints = new List<GameObject>(gos);
			
			foreach( RocketFightPlayer rfp in playerList ) {
				// choose random spawn point
				int randIndex = Mathf.RoundToInt(Random.Range(0, spawnPoints.Count));
				GameObject sp = spawnPoints[randIndex];
				spawnPoints.RemoveAt(randIndex);
					
				// assign spawpoint
				sp.GetPhotonView().RPC("AssignTo",PhotonTargets.AllBuffered,rfp.photonPlayer);
				
				// spawn player incl. color
				Vector3 rgb = new Vector3( rfp.color.r, rfp.color.g,rfp.color.b );
				photonView.RPC("SpawnPlayer",rfp.photonPlayer,sp.transform.position, rgb);
				// init score
				photonView.RPC("SetScore",PhotonTargets.AllBuffered, rfp.photonPlayer.ID, 0);
			}
		}
	}
	
	[RPC]
	public void SetScore(int playerID, int val) {
		playerScores[playerID] = val;
	}
	
	[RPC]
	public void IncreaseScore(int playerID) {
		foreach( RocketFightPlayer rfp in playerList ) {
			if( rfp.photonPlayer.ID == playerID ) {
				rfp.score++;	
			}
		}
	}
	
	/**
	 * Called when new level was loaded.
	 * If we loaded the game scene, find a respawn point for you.
	 * Then spawn your player object at this spawn point.
	 */
	[RPC]
	public void SpawnPlayer(Vector3 spawnPt, Vector3 rgb) {
		if( Application.loadedLevelName == gameScene) {
			startTime = Time.time;
			Debug.Log("Instatiate player at " + spawnPt);
			GameObject handle = PhotonNetwork.Instantiate(playerPrefab.name,spawnPt,Quaternion.identity,0);
			handle.GetComponent<InputManager>().SendMessage("SetPlayer", PhotonNetwork.player);
			handle.GetComponent<PlayerManager>().SendMessage("SetSpawnPoint", spawnPt);
			handle.GetPhotonView().RPC("SetColor",PhotonTargets.AllBuffered,rgb);
			hasSpawn = true;
		}
		
	}
	
	[RPC]
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
	}
	
	
	
	
}
