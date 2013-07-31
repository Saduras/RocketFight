using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightEventCollector : MonoBehaviour {
	
	private List<FightEvent> fEventList = new List<FightEvent>();
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// TODO: RPC ERROR
	[RPC]
	public void NewEvent( FightEvent.FightEventType type, PhotonPlayer srcPlayer, PhotonPlayer trgPlayer, double timestamp) {
		FightEvent newEvent =  new FightEvent(type,srcPlayer,trgPlayer,timestamp);
		fEventList.Add( newEvent );
		Debug.Log( newEvent );
	}
	
	public void NewEvent( int type, PhotonPlayer srcPlayer, PhotonPlayer trgPlayer, double timestamp) {
		FightEvent newEvent =  new FightEvent((FightEvent.FightEventType) type,srcPlayer,trgPlayer,timestamp);
		fEventList.Add( newEvent );
		Debug.Log( newEvent );
	}
}
