using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Photon.MonoBehaviour {
	
	public GameObject scorePopup;
	
	private Color color;
	private PhotonPlayer lastHit;
	private List<Hit> hitList = new List<Hit>();
	public int maxHitListCount = 3;
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
	
	/**
	 * Add an player and timeStamp to the hitList at last place 
	 * and make sure there are no hits with the same player left.
	 */
	[RPC]
	public void HitBy( PhotonPlayer player ) {
		// return if you hit yourself
		if( player == photonView.owner )
			return;
		
		// return if last hit is equal new hit
		if( hitList.Count > 0 )
			if( hitList[hitList.Count - 1].player == player ) 
				return;
		
		// find old assist entries
		List<Hit> results = hitList.FindAll(delegate(Hit hit) {
			if(hit.player == player) 
				return true;
			else
				return false;
		});
		// remove old assist entries
		foreach( Hit oldHit in results ) {
			hitList.Remove( oldHit );	
		}
		
		// add new hit
		hitList.Add( new Hit(Time.time, player) );
		
		// trim hit list if it's to long now
		while( hitList.Count > maxHitListCount ) {
			hitList.RemoveAt(0);	
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
				// give score points to killer and assistances
				if ( hitList.Count > 0 ) {
					Debug.Log("Killed by " + hitList[hitList.Count - 1].player.name + " [" + hitList[hitList.Count - 1].player.ID + "]");
					netman.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,hitList[hitList.Count -1].player.ID, 2);
					for( int i=0; i<hitList.Count-1; i++) {
						if( hitList[i] != null && hitList[i].timestamp > (Time.time - assistTime) )
							netman.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,hitList[i].player.ID, 1);
					}
				}
				// empty hitList
				hitList.Clear();
				
				// if we don't know our spawn point: find it!
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
		
		public override string ToString() {
			return "Hit by " + player.name + " [" + player.ID + "] at time " + timestamp;
		}
	}
}
