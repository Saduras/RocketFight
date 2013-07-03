using UnityEngine;
using System.Collections;

public class PlayerManager : Photon.MonoBehaviour {
	
	public GameObject scorePopup;
	
	private Color color;
	private PhotonPlayer lastHit;
	private Netman netman;
	private GameObject spawnPointObj;
	
	
	public float respawnTime = 3f;
	private float deathTime;
	private bool requestSpawn = false;
	
	private CharacterMover mover;
	
	
	void Start () {
		if (photonView.owner == PhotonNetwork.player) {	
			netman = GameObject.Find("PhotonNetman").GetComponent<Netman>();
			mover = GetComponent<CharacterMover>();
		}
	}
	
	void Update() {	
		if( requestSpawn && photonView.owner == PhotonNetwork.player ) {
			if( Time.time > deathTime + respawnTime ) {
				Respawn();
				requestSpawn = false;	
			}
		}
	}
	
	[RPC]
	public void SetColor( Vector3 rgb ) {
		color = new Color(rgb[0],rgb[1],rgb[2], 1.0f);
		GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_Color",color);
	}
	
	[RPC]
	public void HitBy( PhotonPlayer player ) {
		lastHit = player;	
	}
	
	[RPC]
	public void PopupScore(int score) {
	 	GameObject handle = (GameObject) Instantiate(scorePopup, transform.position + Vector3.up, Quaternion.identity);
		handle.GetComponent<UILabel>().text = "+" + score;
		handle.GetComponent<UILabel>().color = color;
		handle.transform.parent = GameObject.Find("UI Root 3D").transform;	
	}
	
	public void OnDeath() {
		if( photonView.owner == PhotonNetwork.player ) {
			if( requestSpawn == false ) {
				if ( lastHit != null ) {
					Debug.Log("Killed by " + lastHit.name + " [" + lastHit.ID + "]");
					if(lastHit != photonView.owner)
						netman.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,lastHit.ID);
				}
				if( spawnPointObj == null ) {
					GameObject[] gos = GameObject.FindGameObjectsWithTag("Respawn");
					foreach( GameObject go in gos ) {
						if ( go.GetComponent<RespawnPoint>().player == photonView.owner ) {
							spawnPointObj = go;
							break;
						}
					}
				}
				
				deathTime = Time.time;
				requestSpawn = true;
				mover.controlable = false;
				mover.SetControllerMovement( Vector3.zero );
				
				// Reset buff if we carry it
				ScoreBuff sb = gameObject.GetComponentInChildren<ScoreBuff>();
				if( sb != null )
					sb.Reset();
				}
		}
	}
	
	private void Respawn() {
			mover.Teleport( spawnPointObj.transform.position );
			transform.rotation = Quaternion.identity;
			//rigidbody.velocity = Vector3.zero;
			spawnPointObj.GetPhotonView().RPC("StartAnimation",PhotonTargets.All);
			
			mover.SetPhysicMovement( Vector3.zero );
			mover.controlable = true;
	}
}
