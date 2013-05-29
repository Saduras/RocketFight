using UnityEngine;
using System.Collections;

public class PlayerPhysic : Photon.MonoBehaviour {
	
	public float forceReduction = 0.5f;
	public float forceTime = 1f;
	public bool controlableWhileForce = false;
	private float forceTimeStart;
	private Vector3 forceStart;
	private Vector3 force;
	private float epsilon = 0.1f;
	private Vector3 startPos;
	
	// Use this for initialization
	void Start () {
		if (!(photonView.owner == PhotonNetwork.player) ) {
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.time - forceTimeStart;
		if( t < forceTime ) {
			Vector3 dv = forceStart * Time.deltaTime / forceTime;
			this.transform.Translate( dv, Space.World );
			force -= dv;
			//Debug.Log ("t: " + (forceTime - t)/forceTime + " force: " + dv);
			
			if( !controlableWhileForce && (this.gameObject.GetComponent<InputManager>().enabled == true) ) {
				this.gameObject.GetComponent<InputManager>().enabled = false;
			}
		} else {
			if( this.gameObject.GetComponent<InputManager>().enabled == false) {
				this.gameObject.GetComponent<InputManager>().enabled = true;
				Debug.Log("Distance traveled: " + (transform.position - startPos).magnitude);
			}
		}
	}
	
	[RPC]
	public void ApplyForce( Vector3 newForce) {
		if (photonView.owner == PhotonNetwork.player) {
				//force += newForce;
				forceStart = newForce;
				force = forceStart;
				startPos = transform.position;
		}
		forceTimeStart = Time.time;
	}
}
