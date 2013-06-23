using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMover))]
public class InputManager : Photon.MonoBehaviour {
	
	public float speed = 5;
	public bool controlable = true;
	
	private Vector3 moveTo;
	private float moveEpsilon = 0.05f;
	
	public float cooldown = 0.5f;
	
	public GameObject projectile;
	public GameObject muzzleFlash;
	public string groundTag = "Ground";
	private float lastShot = 0;
	private Vector3 shotDir = Vector3.forward;
	
	private Animator anim;
	private PlayerMover mover;
	
	
	// The client who controls this character
	private PhotonPlayer controllingPlayer;
	//private PhotonView photonView;
	
	// Custom cursor texture
	public Texture2D cursorTex;
	public int cursorSizeX = 32;  // set to width of your cursor texture
	public int cursorSizeY= 32;  // set to height of your cursor texture
	
	// Use this for initialization
	public void Awake () {
		Screen.showCursor = false;
		moveTo = this.transform.position;
		anim = GetComponent<Animator>();
		anim.speed = speed / 2;
		mover = GetComponent<PlayerMover>();
	}
	
	// Update is called once per frame
	public void Update () {
		// Check for input updates
		if( (PhotonNetwork.player == controllingPlayer && controlable) ) {
			if( Time.time - lastShot < 0.2f ) {
				transform.LookAt( shotDir );
			} else {
				// Get movement input.
				Vector3 hitPoint;
				// calculate movement vector from keyboard input
				Vector3 movement = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
				movement.Normalize();
				// apply previous calculated movement
				mover.SetInputMovement( movement );
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
						Vector3 pos = this.transform.position + viewDirection.normalized * 0.7f + Vector3.up * 0.5f;
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
						shotDir = transform.position + viewDirection;
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
