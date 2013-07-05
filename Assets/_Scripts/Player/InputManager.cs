using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMover))]
public class InputManager : Photon.MonoBehaviour {
	
	public bool controlable = true;
	
	public float cooldown = 0.5f;
	
	public GameObject projectile;
	public GameObject muzzleFlash;
	public string groundTag = "Ground";
	private float lastShot = 0;
	private Vector3 shotDir = Vector3.forward;
	
	private Animator anim;
	private CharacterMover mover;
	
	
	// The client who controls this character
	public PhotonPlayer controllingPlayer;
	//private PhotonView photonView;
	
	// Custom cursor texture
	public Texture2D cursorTex;
	public int cursorSizeX = 32;  // set to width of your cursor texture
	public int cursorSizeY= 32;  // set to height of your cursor texture
	
	// Use this for initialization
	public void Awake () {
		Screen.showCursor = false;
		mover = GetComponent<CharacterMover>();
		anim = GetComponent<Animator>();
		anim.speed = mover.movementSpeed / 2;
	}
	
	// Update is called once per frame
	public void Update () {
		// Check for input updates
		if( (PhotonNetwork.player == controllingPlayer && controlable) ) {
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
				if( movement.magnitude > 0.1)
					this.transform.LookAt(this.transform.position + movement);
	
				if( Input.GetButton("Fire1") ) {
					hitPoint = GetMouseHitPoint();
					Shoot(hitPoint);
				}
			}
		}
	}
	
	/**
	 * Replace mouse coursor with custom texture
	 */
	public void OnGUI(){
		GUI.DrawTexture( new Rect(Input.mousePosition.x-cursorSizeX/2, 
				(Screen.height-Input.mousePosition.y)-cursorSizeY/2, 
				cursorSizeX, 
				cursorSizeY),cursorTex);
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
		if( Time.time > lastShot + cooldown ) {
			Vector3 direction = mousePos - this.transform.position;
			direction.y = 0;
			direction.Normalize();
			
			Vector3 pos = this.transform.position + direction.normalized * 0.7f + Vector3.up * 0.5f;
			PhotonNetwork.Instantiate(muzzleFlash.name,
										pos, 
										this.transform.rotation, 0);
			GameObject handle = PhotonNetwork.Instantiate(projectile.name, 
										pos, 
										this.transform.rotation, 0);
			// handle.transform.LookAt( handle.transform.position + viewDirection );
			// hack for strange model...
			handle.transform.LookAt( handle.transform.position - direction );
			
			handle.SendMessage("SetRange", Vector3.Distance(pos, mousePos) );
			lastShot = Time.time;
			shotDir = transform.position + direction;
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
