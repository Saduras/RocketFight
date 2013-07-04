using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScoreDisplay : MonoBehaviour {
	
	public List<GameObject> scoreElements = new List<GameObject>();
	
	public Netman nman;
	
	void OnEnable() {
		for( int i=0; i<scoreElements.Count; i++ ) {
			if( nman.playerList.Count > i ) {
				scoreElements[i].GetComponentInChildren<UISlicedSprite>().color = nman.playerList[i].color;
			} else {
				scoreElements[i].SetActive(false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		for( int i=0; i<nman.playerList.Count; i++ ) {
			if(!scoreElements[i].activeSelf)
				scoreElements[i].SetActive(true);
			
			scoreElements[i].GetComponentInChildren<UILabel>().text = nman.playerList[i].photonPlayer.name + ": " + nman.playerList[i].score;
		}
	}
}
