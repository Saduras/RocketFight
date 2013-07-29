using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(PhotonView))]
public class Rocket : Photon.MonoBehaviour {
	
	public float speed = 10;
	public float lifetime = 3;
	public float explosionRange = 2;
	public float explosionForce = 20;
	
	public Vector3 target;
	
	public GameObject explosion;
	public string playerTag = "Player";
	
	public List<float> zoneRadii = new List<float>();
	public List<float> zoneStrength = new List<float>();
	
	private float birthTime;
	
	public enum FlightPath {
		linear,
		ballisitic,
		controlled
	}

	// Use this for initialization
	void Awake () {
		birthTime = (float) PhotonNetwork.time;
		
		if(zoneRadii.Count < 1 || zoneStrength.Count != zoneRadii.Count ) {
			Debug.LogError("You must define atleast one explosion zone (radius & strength) for the Rocket!");	
		}
	}
	
	[RPC]
	public void InstatiateTimeStamp(float timestamp) {		
		
		if( birthTime - timestamp <= 0 )
			return;
		
		Debug.Log("Forward by: " + (birthTime - timestamp));
		
		UpdateInternal( birthTime - timestamp );
	}
	
	[RPC]
	public void SetTarget(Vector3 targetPos) {
		Debug.Log("Target Pos: " + targetPos);
		target = targetPos;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateInternal( Time.deltaTime );
	}
		
	private void UpdateInternal(float deltaTime) {
		this.transform.Translate( Vector3.forward * speed * deltaTime );
		
		Vector3 pos = transform.position;
		pos.y = 0;
		if( target != null )
			if( Vector3.Dot( (target - pos), transform.rotation * Vector3.forward) <= 0 ) {
				if( photonView.owner == PhotonNetwork.player ) {
					transform.position = target;
					Explode();
				} else {
					renderer.enabled = false;
				}
			}
			
		if ( (birthTime + lifetime < PhotonNetwork.time) && ((photonView.owner == PhotonNetwork.player)) ) {
			PhotonNetwork.Destroy( this.gameObject );	
		}
	}
	
	void OnCollisionEnter( Collision collision ) {
		if ( photonView.owner == PhotonNetwork.player ) {
			Explode();
		}
	}
	
	public void Explode() {
		if( explosion != null )
				PhotonNetwork.Instantiate(explosion.name, this.transform.position, Quaternion.identity, 0);
		
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
		
		if( photonView.owner == PhotonNetwork.player )
			PhotonNetwork.Destroy( this.gameObject );
	}
	
	void OnDrawGizmos() {
		for( int i=0; i<zoneRadii.Count; i++ ) {
			Gizmos.color = new Color( 1f, 1-zoneStrength[i], 0f, 1f );
			Gizmos.DrawWireSphere( transform.position, zoneRadii[i] );	
		}
	}
}
