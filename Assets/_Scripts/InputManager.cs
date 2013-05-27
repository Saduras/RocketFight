using UnityEngine;
using System.Collections;

public class InputManager : Photon.MonoBehaviour {
	
	public float speed = 5;
	public Controls moveControl = Controls.mouse;
	
	private Vector3 moveTo;
	private float moveEpsilon = 0.2f;
	
	public float cooldown = 0.5f;
	
	public GameObject projectile;
	public string groundTag = "Ground";
	private float lastShot = 0;
	
	
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
	}
	
	// Update is called once per frame
	public void Update () {
		// Check for input updates
		if( PhotonNetwork.player == controllingPlayer) {
			// Get movement input.
			Vector3 movement = Vector3.zero;
			Vector3 hitPoint;
			switch(moveControl) {
				case Controls.keyboard:
					Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"),0);
					movement = new Vector3(moveDirection.x, 0, moveDirection.y);
					movement.Normalize();
					break;
				case Controls.mouse:
					// update destination
					if( Input.GetButton("Fire2") ) {
						moveTo = GetMouseHitPoint();
					}
					// move player to destination
					if( Vector3.Distance(moveTo, this.transform.position) > moveEpsilon ) {
						movement = moveTo - this.transform.position;
						movement.y = 0;
						movement.Normalize();
					} else {
						movement = Vector3.zero;					
					}
					break;
			}
			this.transform.Translate(movement * speed * Time.deltaTime, Space.World);
			
			// Get rotation input.
			hitPoint = GetMouseHitPoint();
			Vector3 viewDirection = hitPoint - this.transform.position;
			viewDirection.y = 0;
			viewDirection.Normalize();
			this.transform.LookAt(this.transform.position + viewDirection);

			if( Input.GetButton("Fire1") ) {
				if( Time.time > lastShot + cooldown ) {
					PhotonNetwork.Instantiate(projectile.name, this.transform.Find("RocketStart").position, this.transform.rotation, 0);
					lastShot = Time.time;
				}
			}
			// Get fire input.
			// if (Input.GetButtonDown("Fire"))
			//	networkView.RPC("ShootMissile",RPCMode.Server, true);
			// if (Input.GetButtonUp("Fire"))
			//	networkView.RPC("ShootMissile",RPCMode.Server, false);
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
		return hitPoint;
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
	
	[RPC]
	public void SetColor(Vector3 colorVec) {
		Color col = new Color(colorVec.x, colorVec.y, colorVec.z, 1);
		this.renderer.material.SetColor("_Color",col);
	}
}
