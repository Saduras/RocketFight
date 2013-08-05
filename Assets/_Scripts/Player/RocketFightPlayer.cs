using UnityEngine;
using System.Collections;

/**
 * Advanced class to store player data. Holds all main data about an player in an match.
 */ 
[System.Serializable]
public class RocketFightPlayer {
	
	public PhotonPlayer photonPlayer;
	public Color color = Color.white;
	public int score = 0;
	public bool levelLoaded = false;
	
	public RocketFightPlayer( PhotonPlayer player, Color col ) {
		photonPlayer = player;
		color = col;
	}
	
	public override string ToString() {
		return "Name: " + photonPlayer.name + " photonID: " + photonPlayer.ID + " color: " + color.ToString() + " score: " + score;	
	}
	
}
