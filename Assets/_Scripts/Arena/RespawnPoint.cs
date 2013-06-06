using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class RespawnPoint : MonoBehaviour {
	
	public bool free = true;
	public PhotonPlayer player;
	private PhotonView photonView;
	
	public virtual void Awake () {
		photonView = this.gameObject.GetPhotonView();	
	}
	
	/**
	 * Check if the respawn point is still free.
	 */
	public bool IsFree() {
		return free;	
	}
	
	public void StartAnimation() {
		NcCurveAnimation[] animations = gameObject.GetComponentsInChildren<NcCurveAnimation>();	
		foreach( NcCurveAnimation ani  in animations ) {
			ani.ResetAnimation();	
		}
		Debug.Log("Respawn Animation start");
	}
	
	/**
	 * Assign this respawn point to the given player if it is free.
	 * The respawn point is not free anymore.
	 */
	[RPC]
	public bool AssignTo(PhotonPlayer assignedPlayer) {
		if( !free ) {
			Debug.LogError("Spawnpoint is not free!");
			return false;
		}
		player = assignedPlayer;
		free = false;
		//photonView.RPC("AssignTo",PhotonTargets.OthersBuffered, assignedPlayer);
		Debug.Log("Assigned respawn point to: " + player.name);
		return true;
	}
	
	/**
	 * Free this respawn point from it assigned player.
	 */
	[RPC]
	public void SetFree() {
		free = true;
		player = null;
		photonView.RPC("SetFree",PhotonTargets.OthersBuffered);
	}
}
