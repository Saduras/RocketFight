using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ScorePanel : MonoBehaviour {
	
	public UIPlayerScoreSlot[] scoreSlots;
	private Match match;
	
	private Vector3[] positions;
	private int[] place;

	// Use this for initialization
	void Start () {
		match = GameObject.Find("PhotonNetman").GetComponent<Match>();
		
		// active one slot for each player in the current match
		List<RocketFightPlayer> playerList = match.GetPlayerList();
		for( int i=0; i<playerList.Count; i++) {
			scoreSlots[i].SetName( playerList[i].photonPlayer.name );
		}
		
		// save inital position of the slots and there order
		positions = new Vector3[scoreSlots.Length];
		place = new int[scoreSlots.Length];
		for( int i=0; i<scoreSlots.Length; i++)  {
			positions[i] = scoreSlots[i].transform.position;
			place[i] = i;
		}
	}
	
	public void UpdateDisplay() {
		// disable all labels
		for( int i=0; i<scoreSlots.Length; i++ ) {
			scoreSlots[i].Deactivate();
		}
		
		// enable all labels for current playerlist
		List<RocketFightPlayer> playerList = match.GetPlayerList();
		for( int i=0; i< playerList.Count; i++) {
			// set player label
			scoreSlots[i].SetName( playerList[i].photonPlayer.name );
		}
		
		UpdateScore();
	}
	
	public void UpdateScore() {
		List<RocketFightPlayer> playerList = match.GetPlayerList();
		
		// update score values
		for( int i=0; i<playerList.Count; i++ ) {
			scoreSlots[i].SetScore( playerList[i].score );	
		}
		
		// sort places
		SortPlacesByScore();
		
		// order slots in world by places
		for( int i=0; i<playerList.Count; i++ ) {
			scoreSlots[place[i]].transform.position = positions[i];	
		}
	}
	
	private void SortPlacesByScore() {
		bool swap = false;
		
		do {
			swap = false;
			for( int i=1; i<match.GetPlayerList().Count; i++ ) {
				// if 2nd slot is bigger than 1st slot; swap places
				if( scoreSlots[place[i-1]].GetScore() < scoreSlots[place[i]].GetScore() ) {
					int tmpPlace = place[i-1];
					place[i-1] = place[i];
					place[i] = tmpPlace;
					swap = true;
				}
			}
		} while( swap );
	}
}
