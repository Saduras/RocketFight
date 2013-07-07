using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Match : MonoBehaviour {
	
	public List<RocketFightPlayer> playerList = new List<RocketFightPlayer>();
	public int maxPlayerCount = 4;
	public bool running;
	public float gameTime;
	
	public UILabel playerListLabel;
	public List<Color> freeColors = new List<Color>();
	
	private List<Color> usedColors = new List<Color>();
	
	
	public void Init() {
		playerList.Clear();
		running = false;
		gameTime = 180;
		UpdateUIPlayerList();
	}
	
	[RPC]
	public void AddPlayer(PhotonPlayer newPlayer) {
		// check playerlist count
		if( playerList.Count >= maxPlayerCount ) 
			return;
		
		// ensure the player isn't already in the list
		List<RocketFightPlayer> results = playerList.FindAll(delegate(RocketFightPlayer rfp) {
			if(rfp.photonPlayer == newPlayer)
				return true;
			else
				return false;
		});
		if( results.Count > 0 ) return;
		
		// get Color
		Color playerColor = freeColors[0];
		freeColors.RemoveAt(0);
		usedColors.Add(playerColor);
		
		// add new RocketFightPlayer
		playerList.Add( new RocketFightPlayer(newPlayer, playerColor));
		
		// update UI
		UpdateUIPlayerList();
	}
	
	[RPC]
	public void RemovePlayer(PhotonPlayer player) {
		// find player
		foreach(RocketFightPlayer rfp in playerList) {
			if( rfp.photonPlayer == player ) {
				// free color
				usedColors.Remove(rfp.color);
				freeColors.Add(rfp.color);
				// fremove from list
				playerList.Remove(rfp);
				// update UI
				UpdateUIPlayerList();
				return;
			}
		}
	}
	
	public void UpdateUIPlayerList() {
		string labelString = "";
		foreach( RocketFightPlayer rfp in playerList ) {
			labelString += "[" + ColorX.RGBToHex(rfp.color) + "]";
			labelString += rfp.photonPlayer.name + " : (" + rfp.score + ")";
			labelString += "[ffffff]";
			if( rfp.photonPlayer.isMasterClient ) 
				labelString += " (Master)";
			labelString += "\n";
		}
		
		playerListLabel.text = labelString;	
	}
}
