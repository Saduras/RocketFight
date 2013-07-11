using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class RespawnPoint : Photon.MonoBehaviour {
	
	public GameObject particleEffectSystem;
	public AudioSource respawnSound;
	public GameObject pointer;
	public Color color;
	public PhotonPlayer player;
	
	private PlayerManager pman;

	void Update() {
		if( pman != null ) {
			if( pman.IsDead() ) {		
				photonView.RPC("SetPointer",PhotonTargets.All,true);
			} else {
				photonView.RPC("SetPointer",PhotonTargets.All,false);
			}
		}
	}
	
	[RPC]
	public void SetPointer( bool val ) {
		pointer.SetActive( val );
	}
	
	
	public void SetPMan( PlayerManager playermanager ) {
		pman = playermanager;
	}
	
	[RPC]
	public void SetColor( Vector3 rgb ) {
		color = new Color( rgb.x, rgb.y, rgb.z, 1);
		Debug.Log("Set Color");
		particleEffectSystem.SetActive( true );
		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach( ParticleSystem particle in ps ) {
			particle.startColor = color;
		}
		pointer.renderer.material.color = color;
	}
	
	[RPC]
	public void StartAnimation() {
		particleEffectSystem.SetActive(true);
		respawnSound.Play();
		Debug.Log("Respawn Animation start");
	}
	
	/**
	 * Assign this respawn point to the given player if it is free.
	 * The respawn point is not free anymore.
	 */
	[RPC]
	public void AssignTo(PhotonPlayer assignedPlayer) {
		player = assignedPlayer;
		Debug.Log("Assigned respawn point to: " + player.name);
	}
}
