using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class RespawnPoint : Photon.MonoBehaviour {
	
	// sound & VFX references
	public GameObject particleEffectSystem;
	public AudioSource respawnSound;
	public GameObject positionMarker;
	
	private Color color;
	private PhotonPlayer player;
	private PlayerManager pman;
	
	// paramteres used to move this object around
	private Vector3? target;
	private float speed = 5;
	private Vector3 lastMove;
	
	/**
	 * Initialize target position to ensure respawnpoint standing still on start
	 */ 
	void Start() {
		target = transform.position;
	}
	
	void Update() {
		// enable/disable marker if the linked player is dead/alive
		if( pman != null ) {
			if( pman.IsDead() ) {
				photonView.RPC("SetMarkerActive",PhotonTargets.All,true);
				positionMarker.renderer.material.SetFloat("_Cutoff", Mathf.InverseLerp(1, 0, pman.RespawnTimeNormalized()));
			} else {
				photonView.RPC("SetMarkerActive",PhotonTargets.All,false);
				target = transform.localPosition;
			}
		}
		
		// move respawnpoint with given speed to target until we a very close
		if( target != null && (target.Value - transform.position).magnitude > 0.2f ) {
			lastMove = (target.Value - transform.position).normalized * speed * Time.deltaTime;
			transform.Translate( lastMove );
		}
	}
	
	/**
	 * Check if the given player is the owner of this respawn point.
	 */
	public bool IsOwner( PhotonPlayer checkPlayer ) {
		return player == checkPlayer;
	}
	
	/**
	 * Enable/disable marker
	 */
	[RPC]
	public void SetMarkerActive( bool val ) {
		positionMarker.SetActive( val );
	}
	
	/**
	 * Set the target of the respawn movement
	 */
	public void SetTargetPos( Vector3 pos ) {
		target = pos;
	}
	
	/**
	 * Establish reference to the player
	 */ 
	public void SetPMan( PlayerManager playermanager ) {
		pman = playermanager;
	}
	
	/**
	 * Set the color of the respawn VFX
	 */ 
	[RPC]
	public void SetColor( Vector3 rgb ) {
		color = new Color( rgb.x, rgb.y, rgb.z, 1);
		particleEffectSystem.SetActive( true );
		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach( ParticleSystem particle in ps ) {
			particle.startColor = color;
		}
		positionMarker.renderer.material.SetColor("_MainColor",color);;
	}
	
	/**
	 * Trigger respawn sound and -animation
	 */ 
	[RPC]
	public void StartAnimation() {
		particleEffectSystem.SetActive(true);
		respawnSound.Play();
	}
	
	/**
	 * Assign this respawn point to the given player.
	 */
	[RPC]
	public void AssignTo(PhotonPlayer assignedPlayer) {
		player = assignedPlayer;
	}
	
	public void OnLeaveArena() {
		Debug.Log("LeavingArena!");
		transform.Translate( -2*lastMove );
		target = transform.localPosition;
	}
}
