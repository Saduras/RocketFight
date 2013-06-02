using UnityEngine;
using System.Collections;

public class PlayerPhysic : Photon.MonoBehaviour {
	
	public float fadeTime = 1f;
	public bool controlableWhileForce = false;
	private float forceTimeStart;
	private Vector3 force;
	private float epsilon = 0.02f;
	private InputManager inman;
	private float curvePower = 2f;
	
	// Use this for initialization
	void Start () {
		if (!(photonView.owner == PhotonNetwork.player) ) {
			this.enabled = false;
			
		} else {
			inman = this.gameObject.GetComponent<InputManager>();	
		}
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.time - forceTimeStart;
		if( t + Time.deltaTime < fadeTime ) {
			Vector3 dv = force * Time.deltaTime / fadeTime * (2f * Mathf.Pow(1 - t / fadeTime, curvePower) );
			Vector3 oldPos = transform.position;
			this.transform.Translate( dv, Space.World );
			Vector3 newPos = transform.position;
			inman.AddToMoveTo( dv);
			
			if( !controlableWhileForce && (this.gameObject.GetComponent<InputManager>().enabled == true) ) {
				this.gameObject.GetComponent<InputManager>().enabled = false;
			}
		} else {
			if( this.gameObject.GetComponent<InputManager>().enabled == false) {
				this.gameObject.GetComponent<InputManager>().enabled = true;
			}
		}
	}
	
	[RPC]
	public void ApplyForce( Vector3 newForce) {
		if (photonView.owner == PhotonNetwork.player) {
				force = newForce;
		}
		forceTimeStart = Time.time;
	}
}
