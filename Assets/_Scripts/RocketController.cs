using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class RocketController : Photon.MonoBehaviour {
	
	public float speed = 10;
	public float lifetime = 3;
	public float explosionRange = 2;
	public float explosionForce = 20;
	public GameObject explosion;
	public string playerTag = "Player";
	private float birthTime;

	// Use this for initialization
	void Start () {
		birthTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Translate( Vector3.forward * speed * Time.deltaTime );
		
		if ( (birthTime + lifetime < Time.time) && (photonView.owner == PhotonNetwork.player) ) {
			PhotonNetwork.Destroy( this.gameObject );	
		}
	}
	
	void OnCollisionEnter( Collision collision ) {
		if ( photonView.owner == PhotonNetwork.player ) {
			Explode();
			PhotonNetwork.Destroy( this.gameObject );
		}
	}
	
	public void Explode() {
		if( explosion != null)
				PhotonNetwork.Instantiate(explosion.name, this.transform.position, Quaternion.identity, 0);
		
		GameObject[] gos = GameObject.FindGameObjectsWithTag( playerTag );
		foreach( GameObject playerGo in gos ) {
			Vector3 direction = playerGo.transform.position - this.transform.position;
			if( direction.magnitude <= explosionRange ) {
				float strengh = explosionForce * (1 - (direction.magnitude / explosionRange));
				Debug.Log("Explosion strength: " + strengh );
				Vector3 playerForce = direction.normalized * strengh;
				
				playerGo.gameObject.GetPhotonView().RPC("ApplyForce",PhotonTargets.AllBuffered,playerForce);	
			}
		}
	}
}
