using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScoreDisplay : MonoBehaviour {
	
	public List<GameObject> scoreElements = new List<GameObject>();
	
	public Match match;
	
	void OnEnable() {
		for( int i=0; i<scoreElements.Count; i++ ) {
			if( match.playerList.Count > i ) {
				scoreElements[i].GetComponentInChildren<UISlicedSprite>().color = match.playerList[i].color;
			} else {
				scoreElements[i].SetActive(false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		for( int i=0; i<match.playerList.Count; i++ ) {
			if(!scoreElements[i].activeSelf)
				scoreElements[i].SetActive(true);
			
			scoreElements[i].GetComponentInChildren<UILabel>().text = match.playerList[i].photonPlayer.name + ": " + match.playerList[i].score;
		}
	}
}
