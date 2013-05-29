using UnityEngine;
using System.Collections;

public class PlayerPhysic : Photon.MonoBehaviour {
	
	public float forceReduction = 0.5f;
	public bool controlableWhileForce = false;
	private Vector3 force;
	private float epsilon = 0.1f;
	
	// Use this for initialization
	void Start () {
		if (!(photonView.owner == PhotonNetwork.player) ) {
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if( force.magnitude > epsilon ) {
			this.transform.Translate( force * Time.deltaTime, Space.World );
			
			Vector3 deltaForce = force * forceReduction * Time.deltaTime;
			force -= deltaForce;
			
			if( !controlableWhileForce && (this.gameObject.GetComponent<InputManager>().enabled == true) ) {
				this.gameObject.GetComponent<InputManager>().enabled = false;
			}
		} else {
			if( this.gameObject.GetComponent<InputManager>().enabled == false)
				this.gameObject.GetComponent<InputManager>().enabled = true;
		}
	}
	
	[RPC]
	public void ApplyForce( Vector3 newForce) {
		if (photonView.owner == PhotonNetwork.player) {
				force += newForce;
		}
	}
}
