using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class RespawnPoint : MonoBehaviour {
	
	public bool free = true;
	public PhotonPlayer player;
	public GameObject particleEffectSystem;
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
	
	[RPC]
	public void StartAnimation() {
		particleEffectSystem.SetActive(true);
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
		
		particleEffectSystem.SetActive( true );
		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		Debug.Log("size: " + ps.Length);
		foreach( ParticleSystem particle in ps ) {
			particle.startColor = GameObject.Find("PhotonNetman").GetComponent<Netman>().GetPlayer( player.ID ).color;
			Debug.Log("Set particle color");
		}
		
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
