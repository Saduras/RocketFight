using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Photon.MonoBehaviour {
	
	public GameObject scorePopup;
	
	private Color color;
	private PhotonPlayer lastHit;
	private Hit[] hitList = new Hit[3];
	private Netman netman;
	private GameObject spawnPointObj;
	
	
	public float assistTime = 3f;
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
		if( player != photonView.owner) {
			for( int i=0; i<hitList.Length - 1; i++ ) {
				hitList[i+1] = hitList[i];
			}
			
			hitList[0] = new Hit(Time.time, player);
		}
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
				if ( hitList[0] != null ) {
					Debug.Log("Killed by " + hitList[0].player.name + " [" + hitList[0].player.ID + "]");
					netman.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,hitList[0].player.ID, 2);
					for( int i=1; i<hitList.Length; i++) {
						if( hitList[i] != null && hitList[i].timestamp > (Time.time - assistTime) )
							netman.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,hitList[i].player.ID, 1);
					}
					
					hitList = new Hit[3];
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
				GetComponent<InputManager>().controlable = false;
				GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
				
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
			spawnPointObj.GetPhotonView().RPC("StartAnimation",PhotonTargets.All);
			
			mover.SetPhysicMovement( Vector3.zero );
			mover.controlable = true;
			GetComponent<InputManager>().controlable = true;
			GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
	}
	
	
	private class Hit {
		public float timestamp;
		public PhotonPlayer player;
		
		public Hit( float time, PhotonPlayer pplayer) {
			timestamp = time;
			player = pplayer;
		}
	}
}
