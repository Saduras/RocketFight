using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayMode))]
public class PlayerPhysic : Photon.MonoBehaviour {
	
	public float fadeTime = 1f;
	public bool controlableWhileForce = false;
	
	// TODO
	public bool vulnable = true;
	
	private float curvePower = 2f;
	
	private List<Force> forceSet = new List<Force>();
	
	private CharacterMover mover;
	
	// Use this for initialization
	void Start () {
		if (!(photonView.owner == PhotonNetwork.player) ) {
			this.enabled = false;
		}
		mover = GetComponent<CharacterMover>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 frameForce = CalculateFrameForce();
		if( frameForce.magnitude != 0 ) {
			mover.SetPhysicMovement( frameForce );
			
			if( !controlableWhileForce && (this.gameObject.GetComponent<InputManager>().enabled == true) ) {
				this.gameObject.GetComponent<InputManager>().enabled = false;
			}
		} else {
			if( this.gameObject.GetComponent<InputManager>().enabled == false) {
				this.gameObject.GetComponent<InputManager>().enabled = true;
			}
		}
		CleanUpForceSet();
	}
	
	private Vector3 CalculateFrameForce() {
		Vector3 frameForce = Vector3.zero;
		
		foreach( Force force in forceSet ) {
			// add to frame force
			float liveTime = Time.time - force.timestamp;
			if( liveTime < fadeTime ) 
				frameForce += force.vector / fadeTime * (2f * Mathf.Pow(1 - liveTime / fadeTime, curvePower) );
		}
		return frameForce;
	}
	
	private void CleanUpForceSet() {
		List<Force> toCleanUp = new List<Force>();
		
		// find force which has been expired
		foreach( Force force in forceSet ) {
			if( force.timestamp < Time.time - fadeTime ) 
				toCleanUp.Add( force );
		}
		
		// remove expired forces
		foreach( Force expiredForce in toCleanUp ) {
			forceSet.Remove( expiredForce );	
		}
		
		toCleanUp.Clear();
	}
	
	[RPC]
	public void ApplyForce( Vector3 newForce) {
		if (photonView.owner == PhotonNetwork.player && vulnable) {
				forceSet.Add( new Force(newForce, Time.time) );
		}
	}
	
	private class Force{
		public Vector3 vector = Vector3.zero;
		public float timestamp;
		
		public Force( Vector3 vec, float startTime ) {
			vector = vec;	
			timestamp = startTime;
		}
		
		public override string ToString() {
			return "Force: vector = " + vector + ", timestamp = " + timestamp;	
		}
	}
}
