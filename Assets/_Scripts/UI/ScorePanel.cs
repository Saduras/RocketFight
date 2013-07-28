using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ScorePanel : MonoBehaviour {
	
	public UILabel[] playerLabels;
	public UILabel[] scoreLabels;
	public UISlicedSprite boardBackground;
	private Match match;

	// Use this for initialization
	void Start () {
		match = GameObject.Find("PhotonNetman").GetComponent<Match>();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void UpdateDisplay() {
		// disable all labels
		for( int i=0; i<playerLabels.Length; i++ ) {
			playerLabels[i].gameObject.SetActive( false );	
			scoreLabels[i].gameObject.SetActive( false );
		}
		
		// enable all labels for current playerlist
		List<RocketFightPlayer> playerList = match.GetPlayerList();
		
		for( int i=0; i< playerList.Count; i++) {
			// set player label
			playerLabels[i].gameObject.SetActive( true );
			playerLabels[i].color = playerList[i].color;
			playerLabels[i].text = playerList[i].photonPlayer.name;
			
			// set player score
			scoreLabels[i].gameObject.SetActive( true );
			scoreLabels[i].color = playerList[i].color;
			scoreLabels[i].text = playerList[i].score.ToString();
		}
		
		if( playerList.Count <= 2 ) {
			Vector3 tmpScale = boardBackground.transform.localScale;
			tmpScale.y = 700;
			boardBackground.transform.localScale = tmpScale;
		} else {
			Vector3 tmpScale = boardBackground.transform.localScale;
			tmpScale.y = 1400;
			boardBackground.transform.localScale = tmpScale;
		}
	}
	
	public void UpdateScore() {
		List<RocketFightPlayer> playerList = match.GetPlayerList();
		
		for( int i=0; i<playerList.Count; i++ ) {
			scoreLabels[i].text = playerList[i].score.ToString();	
		}
	}
}
