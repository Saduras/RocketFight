using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMover))]
public class InputManager : Photon.MonoBehaviour {
	
	public bool controlable = true;
	
	public float cooldown = 0.5f;
	
	public Animation crosshairAnimation;
	public GameObject projectile;
	public GameObject muzzleFlash;
	public AudioSource walkSound;
	public string groundTag = "Ground";
	private Match match;
	private float lastShot = 0;
	private Vector3 shotDir = Vector3.forward;
	
	private Animator anim;
	private CharacterMover mover;
	private PlayerManager pman;
	
	private int rageCounter = 0;
	private float rageCooldown = 0.2f;
	
	public bool respawnFree = true;
	
	
	// The client who controls this character
	public PhotonPlayer controllingPlayer;
	//private PhotonView photonView;
	
	// Use this for initialization
	public void Awake () {
		mover = GetComponent<CharacterMover>();
		anim = GetComponent<Animator>();
		anim.speed = mover.movementSpeed / 2;
		match = GameObject.Find("PhotonNetman").GetComponent<Match>();
		pman = GetComponent<PlayerManager>();
		
		crosshairAnimation = GameObject.Find("CursorController").GetComponent<Animation>();
	}
	
	// Update is called once per frame
	public void Update () {
		// Check for input updates
		if( (PhotonNetwork.player == controllingPlayer && controlable && match.IsRunning()) ) {
			if( Time.time - lastShot < 0.02f ) {
				transform.LookAt( shotDir );
				mover.SetControllerMovement( Vector3.zero );
			} else {
				// Get movement input.
				Vector3 hitPoint;
				// calculate movement vector from keyboard input
				Vector3 movement = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
				movement.Normalize();
				// apply previous calculated movement
				mover.SetControllerMovement( movement );
				anim.SetFloat("speed", movement.magnitude );
				
				// rotate in movement direction
				if( movement.magnitude > 0.1) {
					this.transform.LookAt(this.transform.position + movement);
					if( !walkSound.isPlaying )
						walkSound.Play();
				} else {
					if( walkSound.isPlaying )
						walkSound.Stop();
				}
	
				if( Input.GetButton("Fire1") ) {
					hitPoint = GetMouseHitPoint();
					Shoot(hitPoint);
				}
			}
		} else if(pman.IsDead()) {;
			rageCounter = 0;
			if( Input.GetButton("Fire1") && respawnFree ) {
				Vector3 mousePos = GetMouseHitPoint();
				pman.SetSpawnPoint( mousePos );
				//respawnFree = false;
			}
		}
	}
	
	private Vector3 GetMouseHitPoint() {
		Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] rayHits = Physics.RaycastAll(cursorRay);
		Vector3 hitPoint = Vector3.zero;
		foreach(RaycastHit hit in rayHits) {
			if (hit.collider.CompareTag(groundTag)) {
				hitPoint = hit.point;
				break;
			}
		}
		hitPoint.y = 0;
		return hitPoint;
	}
	
	/**
	 * Proceed shoot into mouse cursour direction!
	 * Check for cooldown; then intantiate and setup rocket.
	 * @param	mousePos	Current position of the mouse cursour.
	 */
	private void Shoot(Vector3 mousePos) {
		if( (Time.time > lastShot + cooldown) 
			|| (rageCounter > 0 && Time.time > lastShot + rageCooldown) ) {
			
			//animate crosshait
			if(crosshairAnimation.isPlaying)
				crosshairAnimation.Stop();
			
			crosshairAnimation.Play();
			
			Vector3 direction = mousePos - this.transform.position;
			direction.y = 0;
			direction.Normalize();
			
			Vector3 pos = this.transform.position + direction.normalized * 0.7f + Vector3.up * 0.5f;
			PhotonNetwork.Instantiate(muzzleFlash.name,
										pos, 
										Quaternion.LookRotation(direction), 0);
			GameObject handle = PhotonNetwork.Instantiate(projectile.name, 
										pos, 
										Quaternion.LookRotation(direction), 0);
			handle.GetPhotonView().RPC("InstatiateTimeStamp",PhotonTargets.AllBuffered,(float)PhotonNetwork.time);
			handle.GetPhotonView().RPC("SetTarget",PhotonTargets.AllBuffered,mousePos);
			
			lastShot = Time.time;
			shotDir = transform.position + direction;
			if( rageCounter > 0) {
				rageCounter--;
				foreach( AnimationState state in crosshairAnimation ) {
					state.speed = 1/rageCooldown;
				}
			} else {
				foreach( AnimationState state in crosshairAnimation ) {
					state.speed = 1/cooldown;
				}
			}
		}
	}
	
	[RPC]
	public void SetPlayer(PhotonPlayer player) {
		Debug.Log("Setting the controlling player: " + player.name + "[" + player.ID + "]");
		controllingPlayer = player;
		// photonView.RPC("SetPlayer",PhotonTargets.OthersBuffered, player);
	}
	
	[RPC]
	public PhotonPlayer GetPlayer() {
		return controllingPlayer;	
	}
}
