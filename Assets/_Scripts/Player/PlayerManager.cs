using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Photon.MonoBehaviour {
	
	public GameObject scorePopup;
	public GameObject deathVFX;
	public GameObject invulnerable;
	public GameObject marker;
	public GameObject circleMarker;
	private PlayerMarker markerInstance;
	
	// stuff for explosion on respawn
	public GameObject explosion;
	public string playerTag = "Player";
	public float explosionForce = 4;
	public List<float> zoneRadii = new List<float>();
	public List<float> zoneStrength = new List<float>();
	
	public AudioSource hitSound;
	
	// materials and target for changing materials
	// depending on player color
	public Material[] playerMaterials;
	public MeshRenderer materialTarget;
	// this color is the player main color and will be used to
	// color marker and respawn VFX
	private Color color;
	
	// list of hit information for the last few hit you recived
	private List<Hit> hitList = new List<Hit>();
	public int maxHitListCount = 3;
	public float assistTime = 3f;
	
	// respawn
	public float respawnTime = 3f;
	public float invulnerableTime = 3f;
	private float deathTimestamp;
	private bool requestSpawn = false;
	
	private CharacterMover mover;
	private Match match;
	private GameObject spawnPointObj;
	
	/**
	 * Initialize all references needed and activate the circle marker if we own this player instance
	 */ 
	void Awake() {
		if (photonView.owner == PhotonNetwork.player) {	
			mover = GetComponent<CharacterMover>();
			markerInstance = ( (GameObject) Instantiate( marker ) ).GetComponent<PlayerMarker>();
			circleMarker.SetActive( true );
		}
		match = GameObject.Find("PhotonNetman").GetComponent<Match>();
	}
	
	/**
	 * Each frame check if we should respawn now and if invunablility is over
	 */ 
	void Update() {	
		if( photonView.owner == PhotonNetwork.player ) {
			// proceed respawn if respawntime it over and respawn is requested
			if( Time.time > deathTimestamp + respawnTime && requestSpawn) {
				Respawn();
				requestSpawn = false;	
			}
			// make player vunable again if time is over
			if( Time.time > deathTimestamp + respawnTime + invulnerableTime && !GetComponent<PlayerPhysic>().IsVulnerable()) {
				// become vunable again
				photonView.RPC("SetVulnerable",PhotonTargets.AllBuffered,true);
				photonView.RPC("HideInvulnerable",PhotonTargets.AllBuffered);
			}
		}
	}
	
	/**
	 * Check if the player is dead and waits for respawn.
	 */ 
	public bool IsDead() {
		return requestSpawn;	
	}
	
	/**
	 * Move respawnpoint to a given position.
	 */ 
	public void SetSpawnPoint( Vector3 position ) {
		spawnPointObj.GetComponent<RespawnPoint>().SetTargetPos( position );
	}
	
	/**
	 * Change main color of this player and choose character materials depending on
	 * Since this is an RPC we get the color as vector and need to rebuild it
	 */ 
	[RPC]
	public void SetColor( Vector3 rgb ) {
		List<Color> usedColors = match.GetUsedColors();
		// rebuild color from vector information
		color = new Color(rgb[0],rgb[1],rgb[2], 1.0f);
		// choose material by color index
		for( int i=0; i<usedColors.Count; i++) {
			if( usedColors[i] == color  && materialTarget != null) {
				materialTarget.material = playerMaterials[playerMaterials.Length - 1 - i];	
				break;
			}
		}
		
		// set color of marker if we own this instance
		if( photonView.owner == PhotonNetwork.player ) {
			markerInstance.SetParent(transform);
			circleMarker.renderer.material.SetColor("_Color",color);	
		}
	}
	
	/**
	 * Return the main color of this player.
	 */ 
	public Color GetColor() {
		return color;	
	}
	
	/**
	 * Add an player and timeStamp to the hitList at last place 
	 * and make sure there are no hits with the same player left.
	 */
	[RPC]
	public void HitBy( PhotonPlayer player ) {
		// play sound
		hitSound.Play();
		
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
	
	/**
	 * Instatiate a label above the player which shows the score increase
	 * and fades away.
	 */ 
	[RPC]
	public void PopupScore(int score) {
	 	GameObject handle = (GameObject) Instantiate(scorePopup, transform.position + Vector3.up, Quaternion.identity);
		handle.GetComponent<UILabel>().text = "+" + score;
		handle.GetComponent<UILabel>().color = color;
		handle.transform.parent = GameObject.Find("UI Root 3D").transform;	
	}
	
	/**
	 * This is recieved if we hit an death zone.
	 * Increase the score for each player in the hit list depending on killer or assistance.
	 * Request respawn and disable the player controls until spawn.
	 * Reset item if we hold it
	 */ 
	public void OnDeath() {
		if( photonView.owner == PhotonNetwork.player ) {
			if( requestSpawn == false ) {
				// give score points to killer and assistances
				if ( hitList.Count > 0 ) {
					Debug.Log("Killed by " + hitList[hitList.Count - 1].player.name + " [" + hitList[hitList.Count - 1].player.ID + "]");
					
					GameObject.Find("PanelDeath").GetComponent<DeathPanel>().Activate( hitList[hitList.Count - 1].player );
					
					match.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,hitList[hitList.Count -1].player.ID, 2);
					
					for( int i=0; i<hitList.Count-1; i++) {
						if( hitList[i] != null && hitList[i].timestamp > (Time.time - assistTime) ) {
							match.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,hitList[i].player.ID, 1);
						}
					}
				}
				// empty hitList
				hitList.Clear();
				
				// if we don't know our spawn point: find it!
				if( spawnPointObj == null ) {
					GameObject[] gos = GameObject.FindGameObjectsWithTag("Respawn");
					foreach( GameObject go in gos ) {
						if ( go.GetComponent<RespawnPoint>().IsOwner(photonView.owner) ) {
							spawnPointObj = go;
							spawnPointObj.GetComponent<RespawnPoint>().SetPMan(this);
							transform.parent = spawnPointObj.transform;
							break;
						}
					}
				}
				transform.localPosition = Vector3.zero;
				
				// request spawn and store time of death
				deathTimestamp = Time.time;
				requestSpawn = true;
				// disable controls
				mover.controlable = false;
				mover.SetControllerMovement( Vector3.zero );
				GetComponent<InputManager>().controlable = false;
				// visualise death on all clients
				photonView.RPC("ShowDeath",PhotonTargets.All);
				
				// Reset buff if we carry it
				ScoreBuff sb = gameObject.GetComponentInChildren<ScoreBuff>();
				if( sb != null )
					sb.Drop();
				}
			
			
			// Make player invulnerbale
			photonView.RPC("SetVulnerable",PhotonTargets.AllBuffered,false);
		}
	}
	
	/**
	 * Visualize death by hiding character and play VFX
	 */ 
	[RPC]
	public void ShowDeath() {
		materialTarget.enabled = false;
		circleMarker.renderer.enabled = false;
		invulnerable.SetActive( false );
		Instantiate(deathVFX,transform.position,Quaternion.identity);
	}
	
	/**
	 * Do respawn with explosion!
	 * Move player instance to respawn point and enable controls again.
	 * Call for respawn animation and reanable player render.
	 */ 
	private void Respawn() {
		// move to spawn point an reset rotation and physic movement
		mover.Teleport( spawnPointObj.transform.position );
		transform.rotation = Quaternion.identity;
		mover.SetPhysicMovement( Vector3.zero );
		// enable controls
		mover.controlable = true;
		GetComponent<InputManager>().controlable = true;
		// re-enable plyaer visuals and play VFX
		photonView.RPC("ShowRespawn",PhotonTargets.AllBuffered);
		spawnPointObj.GetPhotonView().RPC("StartAnimation",PhotonTargets.All);
		
		// explosion on respawn
		Explode();
	}
	
	/**
	 * Re-enable player rendering and activate invulnerable indicator
	 */ 
	[RPC]
	public void ShowRespawn() {
		materialTarget.enabled = true;
		circleMarker.renderer.enabled = true;
		invulnerable.SetActive( true );
		invulnerable.animation.Play();
	}
	
	/**
	 * Hide the invulnerable indicator
	 */ 
	[RPC]
	public void HideInvulnerable() {
		invulnerable.SetActive( false );
		invulnerable.animation.Stop();
	}
	
	/**
	 * Play explosion VFX and aplly hit and force to alle player around, moving them away.
	 * This happens on respawn.
	 */
	public void Explode() {
		// instiate VFX
		if( explosion != null)
			PhotonNetwork.Instantiate(explosion.name, this.transform.position, Quaternion.identity, 0);
		
		// calculate force for each player in range and send them a force
		GameObject[] gos = GameObject.FindGameObjectsWithTag( playerTag );
		foreach( GameObject playerGo in gos ) {
			// calculate force direction
			Vector3 direction = playerGo.transform.position - this.transform.position;
			direction.y = 0;
			// check each zone from inside to the outside and stop if player was in the current zone
			for( int i=0; i<zoneRadii.Count; i++ ) {
				if( direction.magnitude < zoneRadii[i] && playerGo.GetPhotonView().owner != photonView.owner ) {
					// calculate final force
					Vector3 playerForce = direction.normalized * explosionForce * zoneStrength[i];
					// send hit and force information to player instance on all clients
					playerGo.gameObject.GetPhotonView().RPC("ApplyForce",PhotonTargets.AllBuffered,playerForce);	
					playerGo.gameObject.GetPhotonView().RPC("HitBy",PhotonTargets.AllBuffered, photonView.owner);
					break; // stop if we found one zone and applied the force
				}
			}
		}
	}
	
	public float RespawnTimeNormalized() {
		return (Time.time - deathTimestamp)/respawnTime;
	}
	
	/**
	 * Struct to store timestamp and source of hits
	 */ 
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
