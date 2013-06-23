using UnityEngine;
using System.Collections;

public class PlayerManager : Photon.MonoBehaviour {
	
	public GameObject playername;
	
	private Vector3 spawnPoint;
	private InputManager inman;
	private Color color;
	private PhotonPlayer lastHit;
	private Netman netman;
	private GameObject spawnPointObj;
	
	
	public float respawnTime = 3f;
	private float deathTime;
	private bool requestSpawn = false;
	
	private PlayerMover mover;
	
	
	void Start () {
		//string name = photonView.owner.name;
		playername.GetComponent<TextMesh>().text = name;
		
		if (photonView.owner == PhotonNetwork.player) {
			inman = GetComponent<InputManager>();	
			netman = GameObject.Find("PhotonNetman").GetComponent<Netman>();
			mover = GetComponent<PlayerMover>();
		}
	}
	
	void Update() {
		playername.transform.LookAt(- 9000 *  Camera.main.transform.position);
		
		if( requestSpawn && photonView.owner == PhotonNetwork.player ) {
			if( Time.time > deathTime + respawnTime ) {
				Respawn();
				requestSpawn = false;	
			}
		}
	}
	
	public void SetSpawnPoint( Vector3 pos ) {
		spawnPoint = pos + Vector3.up;	
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
	
	public void OnDeath() {
		if( photonView.owner == PhotonNetwork.player ) {
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
			inman.controlable = false;
		}
	}
	
	private void Respawn() {
			mover.Teleport( spawnPoint );
			transform.rotation = Quaternion.identity;
			rigidbody.velocity = Vector3.zero;
			spawnPointObj.GetPhotonView().RPC("StartAnimation",PhotonTargets.All);
			inman.controlable = true;
	}
}
