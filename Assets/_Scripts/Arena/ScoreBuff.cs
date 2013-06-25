using UnityEngine;
using System.Collections;

public class ScoreBuff : MonoBehaviour {

	public GameObject staticVFX;
	public GameObject mobileVFX;
	
	public float buffDuration = 5;
	
	private PhotonPlayer player;
	private bool pickedUp = false;
	private Vector3 startPos;
	private float pickupTime = 0;
	private float scoreTime = 0;
	private Netman netman;
	
	// Use this for initialization
	void Start () {
		startPos = transform.position;
		netman = GameObject.Find("PhotonNetman").GetComponent<Netman>();
	}
	
	// Update is called once per frame
	void Update () {
		if(pickedUp) {
			if(Time.time - pickupTime > buffDuration) {
				pickedUp = false;
				transform.parent = null;
				transform.position = startPos;
				staticVFX.SetActive( true );
			}
			if( scoreTime + 1 < Time.time ) {
				netman.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,player.ID);
				scoreTime = Time.time;
			}
		}
	}
	
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player") ) {
			player = other.gameObject.GetComponent<InputManager>().controllingPlayer;
			transform.parent = other.gameObject.transform;
			renderer.enabled = false;
			pickedUp = true;
			pickupTime = Time.time;
			staticVFX.SetActive( false );
			
			Debug.Log("new parent " + transform.parent.gameObject.name);
		}
	}
}
