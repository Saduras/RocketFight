using UnityEngine;
using System.Collections;

public class MouseExplosion : Photon.MonoBehaviour {
	
	public float radius = 1;
	public float explosionForce = 4;
	
	// Use this for initialization
	void Awake () {
		Explode();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Explode() {
//		if(!photonView.owner == PhotonNetwork.player)
//			return;
		
		GameObject[] gos = GameObject.FindGameObjectsWithTag( "Player" );
		foreach( GameObject playerGo in gos ) {
			Vector3 direction = playerGo.transform.position - this.transform.position;
			direction.y = 0;
			if( direction.magnitude < radius ) {
				Vector3 playerForce = direction.normalized * explosionForce;
				Debug.Log("Explosion strength: " + playerForce.magnitude );
				
				if( photonView.owner == PhotonNetwork.player ) {
					playerGo.gameObject.GetPhotonView().RPC("AddForce",PhotonTargets.OthersBuffered,playerForce, GetHashCode());	
				} else if( playerGo.GetPhotonView().owner != photonView.owner ) {
					playerGo.GetComponent<Predictor>().AddForce(playerForce, GetHashCode());
					break;
				}
			}
		}
	}
}
