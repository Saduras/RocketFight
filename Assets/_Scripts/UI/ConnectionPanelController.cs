using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectionPanelController : MonoBehaviour {

	public UILabel conncetionStateLabel;
	public List<UISprite> connectionSprites = new List<UISprite>(4);
	
	// Use this for initialization
	void Start () {
		foreach( UISprite sprite in connectionSprites ) {
			sprite.color = Color.white;	
		}
	}
	
	// Update is called once per frame
	void Update () {
		// display current connection state
		conncetionStateLabel.text = PhotonNetwork.connectionStateDetailed.ToString();
		
		// color sprite green to visualis connection progress
		int greenCount = 0;
		switch(PhotonNetwork.connectionStateDetailed) {
		case PeerState.Connected:
			greenCount = 1;
			break;
		case PeerState.ConnectedToMaster:
			greenCount = 2;
			break;
		case PeerState.ConnectedToGameserver:
			greenCount = 3;
			break;
		case PeerState.Joined:
			greenCount = 4;
			break;
		}
		for( int i=0; i<greenCount; i++ ) {
			connectionSprites[i].color = Color.green;	
		}
	}
}
