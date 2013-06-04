using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(PhotonView))]
public class NetmanGUI : Photon.MonoBehaviour {

	public int guiSpace = 5;
	public string setting = "1";
	
	public GUIStyle errorStyle;
	
	private string playerName = "RocketFighter01";
	private bool displayError = false;
	private string errorMsg = "";
	private Netman nman;
	
	private GUIStyle[] playerStringStyle;
	private Texture2D playerStringBackground;
	
	public void Start() {
		nman  = this.gameObject.GetComponent<Netman>();
		playerStringBackground = Utility.GenerateOneColorTexture( Color.white, 1, 1 );
	}

	public void OnGUI() {
		// init playerStringStyle
		if( playerStringStyle == null ) {
			playerStringStyle = new GUIStyle[nman.playerColors.Length];
		
			for( int i=0; i<playerStringStyle.Length; i++) {
				Color color = nman.playerColors[i];
				playerStringStyle[i] = new GUIStyle(GUI.skin.textArea);
				playerStringStyle[i].normal.textColor = color;
				playerStringStyle[i].normal.background = playerStringBackground;
			}	
		}
		
		
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		
		// Display ping in the upper right corner
		int ping = PhotonNetwork.GetPing();
		GUILayout.BeginArea( new Rect(Screen.width - 70, 0, 70, 30), GUI.skin.box);
			GUILayout.Label("Ping: " + ping);
		GUILayout.EndArea();
		
		// display lobby and login information and hidde them ingame
		if( !nman.hasSpawn ) {
			GUILayout.BeginArea( new Rect(Screen.width/2 - 100, Screen.height/2 - 100, 200, 200), GUI.skin.box );
				// Display errors here if there are any
				if (displayError) {
					GUILayout.Label(errorMsg, errorStyle);
				}
			
				// Display Inputfiel to choos player name
				if( PhotonNetwork.connectionState == ConnectionState.Disconnected ) {
					GUILayout.Label("Username:");
					playerName = GUILayout.TextField(playerName, 32);
				}
			
				// Display player list in room
				if (PhotonNetwork.room != null) {
					GUILayout.Label("Player list: ");
					PhotonPlayer[] playerList = PhotonNetwork.playerList;
					// playerList = SortPlayerList(playerList);
					for( int i=0; i<playerList.Length; i++) {
						PhotonPlayer player = playerList[i];
						int score = 0;
						nman.playerScores.TryGetValue(player.ID, out score);
						string playerString = player.name + " [" + player.ID + "]: " + score;
						if( player.isMasterClient ) {
							playerString += " *";
						}
						GUILayout.TextArea(playerString, playerStringStyle[player.ID - 1]);
					}
				}
				
				GUILayout.BeginArea(new Rect(0, 175,80,25));
				// Display "start game" button
				if (PhotonNetwork.room != null && PhotonNetwork.isMasterClient) {
					if (GUILayout.Button("Start game!")) {
						this.gameObject.GetComponent<Netman>().OrganizeSpawning();
						PhotonNetwork.room.open = false;
						PhotonNetwork.room.visible = false;
					}
				}
				GUILayout.EndArea();
			
				GUILayout.BeginArea(new Rect(120, 175,80,25));
					// display Connect/Disconnect button
					if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
			        {
			            if (GUILayout.Button("Connect") 
							|| (Event.current.isKey && Event.current.keyCode == KeyCode.Return) ) {
							connectPlayer();
			            }
			        } else {
			            if (GUILayout.Button("Disconnect") && !nman.hasSpawn)
			            {
			                PhotonNetwork.Disconnect();
			            }
			        }
				GUILayout.EndArea();
			GUILayout.EndArea();
		} else {
			GUILayout.BeginArea(new Rect(Screen.width/2 - 100, 5, 200, 60));
				float timer = (nman.gameTime - Time.time + nman.startTime);
				string minutes = Mathf.Floor(timer / 60).ToString("00");
				string seconds = (timer % 60).ToString("00");
				GUILayout.Label("Time: " + minutes + ":" + seconds );
			GUILayout.EndArea();
			
			GUILayout.BeginArea(new Rect(0, Screen.height - 30, Screen.width, 30), GUI.skin.box);
				GUILayout.BeginHorizontal();
					GUILayout.Label("Score ");
					foreach( KeyValuePair<int, int> score in nman.playerScores ) {
						GUILayout.Label("" + score.Value, playerStringStyle[score.Key - 1]);
					}
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
    }
	
	/**
	 * Connect to master server if player has choosen a name.
	 * Set the player name.
	 * Set errorMsg and displayError if there was no player name set.
	 */
	private void connectPlayer() {
		if( playerName != "" ) {
        	PhotonNetwork.ConnectUsingSettings( setting );
			PhotonNetwork.player.name = playerName;
			
			displayError = false;
		} else {
			displayError = true;
			errorMsg = "Choose a Username!";
		}
	}
	
	/**
	 * Sort PhotonPlayer array by PhotonPlayer.ID using List<T>.Sort() on an integer List
	 */
	private PhotonPlayer[] SortPlayerList( PhotonPlayer[] array ) {
		List<int> playerIDs = new List<int>();
		foreach( PhotonPlayer player in array ) {
			playerIDs.Add(player.ID);
		}
		playerIDs.Sort();
		PhotonPlayer[] sortedPlayerList = new PhotonPlayer[array.Length];
		for( int i=0; i<sortedPlayerList.Length; i++) {
			sortedPlayerList[i] = PhotonPlayer.Find(playerIDs[i]);
		}
		return sortedPlayerList;
	}
}
