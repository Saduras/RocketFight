using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ScorePanel : MonoBehaviour {
	
	public UIPlayerScoreSlot[] scoreSlots;
	private Match match;
	
	// these are used to reorder the scoreSlots sorted by score
	private Vector3[] positions;
	private int[] place;

	// Use this for initialization
	void Awake() {
		match = GameObject.Find("PhotonNetman").GetComponent<Match>();
		
		// activate one slot for each player in the current match
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
	
	/**
	 * Deactivate all slots and reactivate one for each player in the playerlist.
	 * Update all score values.
	 */ 
	public void UpdateDisplay() {
		// disable all labels
		for( int i=0; i<scoreSlots.Length; i++ ) {
			scoreSlots[i].Deactivate();
		}
		
		// reset places
		place = new int[scoreSlots.Length];
		for( int i=0; i<scoreSlots.Length; i++)  {
			place[i] = i;
		}
		
		// enable all labels for current playerlist
		List<RocketFightPlayer> playerList = match.GetPlayerList();
		for( int i=0; i< playerList.Count; i++) {
			// set player label
			scoreSlots[i].SetName( playerList[i].photonPlayer.name );
		}
		
		UpdateScore();
	}
	
	/**
	 * Update score labels with values from match playerlist and sort the scoreSlots
	 * by score and reorder them
	 */ 
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
	
	/**
	 * Simple sort algorithm witch swap to neighbourd values if there are in the wrong order
	 * and check the next. Repeating this until we didn't needed to change anything in this loop.
	 */ 
	private void SortPlacesByScore() {
		bool swap = false;
		
		do {
			swap = false;
			for( int i=1; i<match.GetPlayerList().Count; i++ ) {
				// if current slot is bigger than last slot swap places
				if( scoreSlots[place[i-1]].GetScore() < scoreSlots[place[i]].GetScore() ) {
					int tmpPlace = place[i-1];
					place[i-1] = place[i];
					place[i] = tmpPlace;
					swap = true;
				}
			}
		} while( swap ); // repeat until we don't swap anything
	}
}
