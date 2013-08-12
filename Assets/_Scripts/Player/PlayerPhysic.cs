using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayMode))]
public class PlayerPhysic : Photon.MonoBehaviour {
	
	// parameters to change force fall off behaviour
	public float fadeTime = 1f;
	// used as power for the force fall off curve
	public float curvePower = 2f;
	
	// if vulnerable is false, all physics are disabled
	private bool vulnerable = true;
	
	private List<Force> forceSet = new List<Force>();
	
	private CharacterMover mover;
	
	/**
	 * Use this for initialization
	 * Disable if we are not the owner of this gameObject
	 */
	void Start () {
//		if (!(photonView.owner == PhotonNetwork.player) ) {
//			this.enabled = false;
//		}
		mover = GetComponent<CharacterMover>();
	}
	
	/**
	 * Oncer per frame, calculate physical movement per frame and forward this to mover instance.
	 */ 
	void Update () {
		// calculate force for this frame
		Vector3 frameForce = CalculateFrameForce();
		// send calculated force to mover
		mover.SetPhysicMovement( frameForce );
		// delete forces which are already faded
		CleanUpForceSet();
	}
	
	/**
	 * Check this instance can effect by forces at the moment.
	 */ 
	public bool IsVulnerable() {
		return vulnerable;	
	}
	
	/**
	 * Change if forces have any effect or not
	 */ 
	[RPC]
	public void SetVulnerable( bool val ) {
		vulnerable = val;	
	}
	
	/**
	 * Calculate the movement of the current frame as result of all active forces
	 */ 
	private Vector3 CalculateFrameForce() {
		Vector3 frameForce = Vector3.zero;
		
		foreach( Force force in forceSet ) {
			// add to frame force
			double liveTime = PhotonNetwork.time - force.timestamp;
			if( liveTime < fadeTime ) 
				frameForce += force.vector / fadeTime * (2f * Mathf.Pow( (float)(1 - liveTime / fadeTime), curvePower) );
		}
		return frameForce;
	}
	
	/**
	 * Delete all expired forces from the force list
	 */
	private void CleanUpForceSet() {
		List<Force> toCleanUp = new List<Force>();
		
		// find force which has been expired
		foreach( Force force in forceSet ) {
			if( force.timestamp < PhotonNetwork.time - fadeTime ) 
				toCleanUp.Add( force );
		}
		
		// remove expired forces
		foreach( Force expiredForce in toCleanUp ) {
			forceSet.Remove( expiredForce );	
		}
		
		toCleanUp.Clear();
	}
	
	/**
	 * Add a new force to the force list, if it's vulnerable
	 */ 
	[RPC]
	public void ApplyForce( Vector3 newForce ) {
		if (vulnerable) {
			forceSet.Add( new Force(newForce, PhotonNetwork.time) );
		}
	}
	
	/**
	 * Structur to store force vector with the born timestamp
	 */ 
	private struct Force {
		public Vector3 vector;
		public double timestamp;
		
		public Force( Vector3 vec, double startTime ) {
			vector = vec;	
			timestamp = startTime;
		}
		
		public override string ToString() {
			return "Force: vector = " + vector + ", timestamp = " + timestamp;	
		}
	}
}
