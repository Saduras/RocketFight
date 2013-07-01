using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class RespawnPoint : Photon.MonoBehaviour {
	
	public GameObject particleEffectSystem;
	public Color color;
	public PhotonPlayer player;

	
	[RPC]
	public void SetColor( Vector3 rgb ) {
		color = new Color( rgb.x, rgb.y, rgb.z, 1);
		Debug.Log("Set Color");
		particleEffectSystem.SetActive( true );
		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach( ParticleSystem particle in ps ) {
			particle.startColor = color;
		}
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
	public void AssignTo(PhotonPlayer assignedPlayer) {
		player = assignedPlayer;
		Debug.Log("Assigned respawn point to: " + player.name);
	}
}
