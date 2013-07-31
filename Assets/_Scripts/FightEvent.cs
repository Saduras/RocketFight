using UnityEngine;
using System.Collections;

public struct FightEvent {
	
	public FightEventType type;
	public PhotonPlayer source;
	public PhotonPlayer target;
	public double timestamp;
	
	public enum FightEventType {
		KILL,
		ASSIST,
		SUICID
	}
	
	public FightEvent( FightEventType feType, PhotonPlayer srcPlayer, PhotonPlayer trgPlayer) {
		type = feType;
		source = srcPlayer;
		target = trgPlayer;
		timestamp = PhotonNetwork.time;
	}
	
	public FightEvent( FightEventType feType, PhotonPlayer srcPlayer, PhotonPlayer trgPlayer, double networkTimestamp) {
		type = feType;
		source = srcPlayer;
		target = trgPlayer;
		timestamp = networkTimestamp;
	}
	
	/**
	 * Check if a specified player was involved in this event.
	 */
	public bool IsInvolved( PhotonPlayer player ) {
		return (player == source) || (player == target);	
	}
	
	/**
	 * Print all values of this struct into a readable string.
	 */
	public override string ToString () {
		 return "Event: " + type 
			+ " from " + source.name + "[" + source.ID + "]" 
			+ " to " + target.name + "[" + target.ID + "]"
			+ " at " + timestamp;
	}
}
