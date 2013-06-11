using UnityEngine;
using System.Collections;

public class InputManager : Photon.MonoBehaviour {
	
	public float speed = 5;
	public Controls moveControl = Controls.mouse;
	
	private Vector3 moveTo;
	private float moveEpsilon = 0.05f;
	
	public float cooldown = 0.5f;
	
	public GameObject projectile;
	public GameObject muzzleFlash;
	public string groundTag = "Ground";
	private float lastShot = 0;
	private Vector3 shotDir = Vector3.forward;
	
	private Animator anim;
	
	
	// The client who controls this character
	private PhotonPlayer controllingPlayer;
	//private PhotonView photonView;
	
	// Custom cursor texture
	public Texture2D cursorTex;
	public int cursorSizeX = 32;  // set to width of your cursor texture
	public int cursorSizeY= 32;  // set to height of your cursor texture
	
	public enum Controls {
		mouse,
		keyboard
	}
	
	// Use this for initialization
	public void Awake () {
		Screen.showCursor = false;
		moveTo = this.transform.position;
		anim = GetComponent<Animator>();
		anim.speed = speed / 2;
	}
	
	// Update is called once per frame
	public void Update () {
		// Check for input updates
		if( (PhotonNetwork.player == controllingPlayer) ) {
			if( Time.time - lastShot < 0.2f ) {
				transform.LookAt( shotDir );	
			} else {
				// Get movement input.
				Vector3 movement = Vector3.zero;
				Vector3 hitPoint;
				switch(moveControl) {
					case Controls.keyboard:
						// calculate movement vector from keyboard input
						Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"),0);
						movement = new Vector3(moveDirection.x, 0, moveDirection.y);
						movement.Normalize();
						break;
					case Controls.mouse:
						// update destination (projected to the x-z-plane)
						if( Input.GetButton("Fire2") ) {
							moveTo = GetMouseHitPoint();
						}
						// project current position tp the x-z plane
						Vector3 pos = this.transform.position;
						pos.y = 0;
						moveTo.y = 0;
						// calculate movement vector to destination if we are not already there
						if( Vector3.Distance(moveTo,pos) > moveEpsilon ) {
							movement = moveTo - pos;
							movement.y = 0;
							movement.Normalize();
						} else {
							movement = Vector3.zero;					
						}
						break;
				}
				// apply previous calculated movement
				this.transform.Translate(movement * speed * Time.deltaTime, Space.World);
				anim.SetFloat("speed", movement.magnitude );
				
				// Get rotation input.
				hitPoint = GetMouseHitPoint();
				Vector3 viewDirection = hitPoint - this.transform.position;
				viewDirection.y = 0;
				viewDirection.Normalize();
				if( movement.magnitude > 0.1)
					this.transform.LookAt(this.transform.position + movement);
	
				if( Input.GetButton("Fire1") ) {
					if( Time.time > lastShot + cooldown ) {
						Vector3 pos = this.transform.position + viewDirection.normalized + Vector3.up * 0.5f;
						PhotonNetwork.Instantiate(muzzleFlash.name,
													pos, 
													this.transform.rotation, 0);
						GameObject handle = PhotonNetwork.Instantiate(projectile.name, 
													pos, 
													this.transform.rotation, 0);
						// handle.transform.LookAt( handle.transform.position + viewDirection );
						// hack for strange model...
						handle.transform.LookAt( handle.transform.position - viewDirection );
						
						handle.SendMessage("SetRange", Vector3.Distance(pos, hitPoint) );
						lastShot = Time.time;
						shotDir = handle.transform.position + viewDirection;
					}
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
	
	public void AddToMoveTo( Vector3 vec ) {
		if( moveControl == Controls.mouse) {
			moveTo += vec;	
		}
	}
	
	public void ResetMoveTo() {
		if( moveControl == Controls.mouse) {
			moveTo = this.transform.position;	
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
