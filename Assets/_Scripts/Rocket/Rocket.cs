using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(PhotonView))]
public class Rocket : Photon.MonoBehaviour {
	
	// rocket properties
	public float speed = 10;
	public float lifetime = 3;
	public float explosionForce = 20;
	
	// destination where the rocket will explode (if not collide earlier
	private Vector3? target;
	
	// VFX
	public GameObject explosion;
	
	
	public string playerTag = "Player";
	
	// describe explosion behaviour
	// zones are defined in unity meter and should be orderd by size from small to big
	public List<float> zoneRadii = new List<float>();
	// strength is defined with values from 0.0 to 1.0 as percentage from explosionForce (see above)
	// i.e. strength = 0.75 means 75% of explosionForce
	public List<float> zoneStrength = new List<float>();
	
	// timestamp when this instance was created
	private float birthTime;

	/**
	 * Initalize.
	 * Store birth time and check explosion zones.
	 */ 
	void Awake () {
		birthTime = (float) PhotonNetwork.time;
		
		if(zoneRadii.Count < 1 || zoneStrength.Count != zoneRadii.Count ) {
			Debug.LogError("You must define atleast one explosion zone (radius & strength) for the Rocket!");	
		}
	}
	
	/**
	 * Get real intatiation time via RPC. If we where born to late,
	 * correct position on flightpath.
	 */ 
	[RPC]
	public void InstatiateTimeStamp(float timestamp) {		
		// only do correction if actual born time was earlier then local born time
		if( birthTime - timestamp <= 0 )
			return;
		
		//Debug.Log("Forward by: " + (birthTime - timestamp));
		
		// correct flight postion by forwarding by birth times difference
		UpdateInternal( birthTime - timestamp );
	}
	
	/**
	 * Set the target where we should explode if no collision happens.
	 */ 
	[RPC]
	public void SetTarget(Vector3 targetPos) {
		Debug.Log("Target Pos: " + targetPos);
		target = targetPos;
	}
	
	
	/**
	 * Do internal update for frame duration.
	 */ 
	void Update () {
		UpdateInternal( Time.deltaTime );
	}
	
	/**
	 * Move rocket forward for the given time interval.
	 * Check if we reached our target and explode if so.
	 * Check lifetime and destroy rocket if it's over.
	 */ 
	private void UpdateInternal(float deltaTime) {
		this.transform.Translate( Vector3.forward * speed * deltaTime );
		
		Vector3 pos = transform.position;
		pos.y = 0;
		if( target != null )
			if( Vector3.Dot( (target.Value - pos), transform.rotation * Vector3.forward) <= 0 ) {
				transform.position = target.Value;
				Explode();
			}
			
		if ( (birthTime + lifetime < PhotonNetwork.time) && ((photonView.owner == PhotonNetwork.player)) ) {
			PhotonNetwork.Destroy( this.gameObject );	
		}
	}
	
	/**
	 * Explode on collision.
	 */ 
	void OnCollisionEnter( Collision collision ) {
		Explode();
	}
	
	/**
	 * Explode when leaving the arena. Rockets outside has no use and may cause bad rendering with scoreboard...
	 */ 
	void OnLeaveArena() {
		Explode();
	}
	
	/**
	 * Play VFX and sent force and hit information to all player in explosion zones.
	 * Then destroy/disbale this rocket.
	 */ 
	public void Explode() {
		// play VFX
		if( explosion != null )
				Instantiate(explosion, this.transform.position, Quaternion.identity);
		
//		Debug.LogError("Explosion at: " + PhotonNetwork.time);
		
		if( photonView.owner == PhotonNetwork.player ) {
			GameObject[] gos = GameObject.FindGameObjectsWithTag( playerTag );
			foreach( GameObject playerGo in gos ) {
				Vector3 direction = playerGo.transform.position - this.transform.position;
				direction.y = 0;
				for( int i=0; i<zoneRadii.Count; i++ ) {
					if( direction.magnitude < zoneRadii[i] ) {
						Vector3 playerForce = direction.normalized * explosionForce * zoneStrength[i];
						Debug.Log("Explosion strength: " + playerForce.magnitude );
						
						playerGo.gameObject.GetPhotonView().RPC("ApplyForce",PhotonTargets.OthersBuffered,playerForce);	
						playerGo.gameObject.GetPhotonView().RPC("HitBy",PhotonTargets.OthersBuffered, photonView.owner);
						break;
					}
				}
			}
		}
		
		if( photonView.owner == PhotonNetwork.player )
			PhotonNetwork.Destroy( this.gameObject );
		else
			gameObject.SetActive( false );
	}
	
	/**
	 * Visualise explosion zones as sphere in Unity Editor. Use color do visualise strength.
	 * Red = 100% strength ----- yellow = 0% strength.
	 */ 
	void OnDrawGizmos() {
		for( int i=0; i<zoneRadii.Count; i++ ) {
			Gizmos.color = new Color( 1f, 1-zoneStrength[i], 0f, 1f );
			Gizmos.DrawWireSphere( transform.position, zoneRadii[i] );	
		}
	}
}
