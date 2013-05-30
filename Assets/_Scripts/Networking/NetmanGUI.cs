using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class NetmanGUI : Photon.MonoBehaviour {

	public int guiSpace = 5;
	public string setting = "1";
	
	public GUIStyle errorStyle;
	
	private string playerName = "";
	private bool displayError = false;
	private string errorMsg = "";

	public void OnGUI()
    {
        GUILayout.Space(guiSpace);
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
        {
            if (GUILayout.Button("Connect"))
            {
				if( playerName != "" ) {
                	PhotonNetwork.ConnectUsingSettings( setting );
					PhotonNetwork.player.name = playerName;
					
					displayError = false;
				} else {
					displayError = true;
					errorMsg = "Choose a Username!";
				}
            }
        }
        else
        {
            if (GUILayout.Button("Disconnect"))
            {
                PhotonNetwork.Disconnect();
            }
        }
		
		Netman nman = this.gameObject.GetComponent<Netman>();
		
		if (PhotonNetwork.room != null && !nman.hasSpawn) {
			if (GUILayout.Button("Spawn")) {
				this.gameObject.GetComponent<Netman>().SpawnPlayer(0);
				PhotonNetwork.room.open = false;
				PhotonNetwork.room.visible = false;
			}
		}
		
		// Display ping in the upper right corner
		int ping = PhotonNetwork.GetPing();
		GUILayout.BeginArea( new Rect(Screen.width - 100, 40, 100, 40));
			GUILayout.TextArea("Ping: " + ping);
		GUILayout.EndArea();
		
		GUILayout.BeginArea( new Rect(Screen.width/2 - 100, Screen.height/2 - 100, 200, 200));
			// Display errors here if there are any
			if (displayError) {
				GUILayout.TextArea(errorMsg, errorStyle);
			}
		
			// Displayer Inputfiel to choos player name
			if( PhotonNetwork.connectionState == ConnectionState.Disconnected ) {
				playerName = GUILayout.TextField(playerName, 32);
			}
		
			// Display player list in room
			if (PhotonNetwork.room != null && !nman.hasSpawn) {
				GUILayout.TextArea("Player list: ");
				PhotonPlayer[] playerList = PhotonNetwork.playerList;
				foreach( PhotonPlayer player in playerList ) {
					string playerString = "Player: " + player.name + " [" + player.ID + "]";
					if( player.isMasterClient ) {
						playerString += " *";
					}
					GUILayout.TextArea(playerString);
				}
			}
		GUILayout.EndArea();
    }
}
