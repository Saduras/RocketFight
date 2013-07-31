using UnityEngine;
using System.Collections;

public class ScoreBuff : Photon.MonoBehaviour {

	public GameObject staticVFX;
	public GameObject mobileVFX;
	public AudioSource itemSound;
	
	public float buffDuration = 30;
	public float buffIntervall = 5;
	public int buffScoreValue = 2;
	
	private PhotonPlayer player;
	private bool pickedUp = false;
	private Vector3 startPos;
	private float pickupTime = 0;
	private float scoreTime = 0;
	private Netman netman;
	
	private Match match;
	
	// Use this for initialization
	void Start () {
		startPos = transform.position;
		netman = GameObject.Find("PhotonNetman").GetComponent<Netman>();
		match = GameObject.Find("PhotonNetman").GetComponent<Match>();
		
		if(!PhotonNetwork.isMasterClient) 
			enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(pickedUp && PhotonNetwork.isMasterClient) {
			if(Time.time - pickupTime > buffDuration) {
				photonView.RPC("Reset",PhotonTargets.AllBuffered);
			}
			if( scoreTime + buffIntervall < Time.time && buffScoreValue > 0 ) {
				netman.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,player.ID, buffScoreValue);
				scoreTime = Time.time;
			}
		}
	}
	
	/**
	 * Reset the item to it's original place and clear item holder ref.
	 */
	public void Drop() {
		photonView.RPC("Reset",PhotonTargets.AllBuffered);
	}
	
	[RPC]
	public void Reset() {
		pickedUp = false;
		transform.parent = null;
		transform.position = startPos;
		staticVFX.SetActive( true );
		collider.enabled = true;
		match.photonView.RPC("ClearItem",PhotonTargets.AllBuffered);
	}
	
	/**
	 * If other collider is an player an we are the master client pick up the item.
	 */
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player")) {
			player = other.gameObject.GetComponent<InputManager>().controllingPlayer;
			//photonView.RPC("ItemPickUp",PhotonTargets.AllBuffered,player);
			ItemPickUp(player);
		}
	}
	
	/**
	 * Set the transform parent of the item to player transform and set item holder
	 */
	[RPC]
	public void ItemPickUp(PhotonPlayer player) {
		// find players game object
		GameObject playerGo = new GameObject();
		GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject go in gos) {
			if( go.GetComponent<InputManager>().controllingPlayer == player ) {
				playerGo = go;
				break;
			}
		}
		
		// position the item in player hierachy
		transform.parent = playerGo.transform;
		Vector3 localPos = transform.localPosition;
		localPos.x = 0;
		localPos.z = 0;
		transform.localPosition = localPos;
		
		renderer.enabled = false;
		pickedUp = true;
		// play sound
		itemSound.Play();
		
		// disable collider and static VFX
		collider.enabled = false;
		staticVFX.SetActive( false );
		
		pickupTime = Time.time;
		
		if(PhotonNetwork.isMasterClient)
			match.photonView.RPC("SetItemHolder",PhotonTargets.AllBuffered,player);
	}
}
