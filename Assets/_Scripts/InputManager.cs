using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	
	public static float speed = 5;
	public static string groundTag = "Ground";
	
	public GameObject projectile;
	public float cooldown = 0.5f;
	private float lastShot = 0;
	
	// The client who controls this character
	private PhotonPlayer controllingPlayer;
	private PhotonView photonView;
	
	// Custom cursor texture
	public Texture2D cursorTex;
	public int cursorSizeX = 32;  // set to width of your cursor texture
	public int cursorSizeY= 32;  // set to height of your cursor texture
	
	// Use this for initialization
	public void Awake () {
		Screen.showCursor = false;
		photonView = this.gameObject.GetPhotonView();
	}
	
	// Update is called once per frame
	public void Update () {
		// Check for input updates
		if( PhotonNetwork.player == controllingPlayer) {
			// Get movement input.
			Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"),0);
			Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y);
			movement.Normalize();
			// Debug.Log(movement);
			this.transform.Translate(movement * speed * Time.deltaTime, Space.World);
			
			// Get rotation input.
			Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] rayHits = Physics.RaycastAll(cursorRay);
			Vector3 hitPoint = Vector3.zero;
			foreach(RaycastHit hit in rayHits) {
				if (hit.collider.CompareTag(groundTag)) {
					hitPoint = hit.point;
				}
			}
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
