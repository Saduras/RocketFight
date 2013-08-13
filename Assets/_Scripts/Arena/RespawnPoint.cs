using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class RespawnPoint : Photon.MonoBehaviour {
	
	// sound & VFX references
	public GameObject particleEffectSystem;
	public AudioSource respawnSound;
	public GameObject positionMarker;
	
	public Vector2[] limits;
	
	private Color color;
	private PhotonPlayer player;
	private PlayerManager pman;
	
	// paramteres used to move this object around
	private Vector3 target = new Vector3(999,999,999);
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
				photonView.RPC("SetMarkerActive",PhotonTargets.AllBuffered,true);
			} else {
				photonView.RPC("SetMarkerActive",PhotonTargets.AllBuffered,false);
				target = transform.localPosition;
			}
		}
		
		// move respawnpoint with given speed to target until we a very close
		if( target != new Vector3(999,999,999) && (target - transform.position).magnitude > 0.2f ) {
			// check x
			if( target.x < limits[0].x )
				target.x = limits[0].x;
			if( target.x > limits[1].x )
				target.x = limits[1].x;
			// check y
			if( target.z > limits[0].y )
				target.z = limits[0].y;
			if( target.z < limits[1].y )
				target.z = limits[1].y;
			lastMove = (target - transform.position).normalized * speed * Time.deltaTime;
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
	
	void OnDrawGizmos() {
		Vector3 topLeft 	= new Vector3(limits[0].x, 1, limits[0].y);
		Vector3 topRight 	= new Vector3(limits[1].x, 1,limits[0].y);
		Vector3 botLeft		= new Vector3(limits[0].x, 1, limits[1].y);
		Vector3 botRight	= new Vector3(limits[1].x, 1, limits[1].y);
		
		Gizmos.DrawLine(topLeft,topRight);
		Gizmos.DrawLine(topRight,botRight);
		Gizmos.DrawLine(botRight,botLeft);
		Gizmos.DrawLine(botLeft,topLeft);
	}
}
