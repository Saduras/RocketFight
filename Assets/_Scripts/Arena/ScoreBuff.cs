using UnityEngine;
using System.Collections;

public class ScoreBuff : MonoBehaviour {

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
	
	// Use this for initialization
	void Start () {
		startPos = transform.position;
		netman = GameObject.Find("PhotonNetman").GetComponent<Netman>();
	}
	
	// Update is called once per frame
	void Update () {
		if(pickedUp) {
			if(Time.time - pickupTime > buffDuration) {
				Reset();
			}
			if( scoreTime + buffIntervall < Time.time ) {
				netman.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,player.ID, buffScoreValue);
				scoreTime = Time.time;
			}
		}
	}
	
	public void Reset() {
		pickedUp = false;
		transform.parent = null;
		transform.position = startPos;
		staticVFX.SetActive( true );
		collider.enabled = true;
	}
	
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player") ) {
			player = other.gameObject.GetComponent<InputManager>().controllingPlayer;
			transform.parent = other.gameObject.transform;
			renderer.enabled = false;
			pickedUp = true;
			// play sound
			itemSound.Play();
			
			collider.enabled = false;
			pickupTime = Time.time;
			staticVFX.SetActive( false );
			
			Debug.Log("new parent " + transform.parent.gameObject.name);
		}
	}
}
