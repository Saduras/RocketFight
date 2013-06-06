using UnityEngine;
using System.Collections;

public class PlayerManager : Photon.MonoBehaviour {
	
	public GameObject playername;
	
	private Vector3 spawnPoint;
	private InputManager inman;
	private Color color;
	private PhotonPlayer lastHit;
	private Netman netman;
	
	
	void Start () {
		string name = photonView.owner.name;
		playername.GetComponent<TextMesh>().text = name;
		
		if (!(photonView.owner == PhotonNetwork.player) ) {
			this.enabled = false;
			
		} else {
			inman = gameObject.GetComponent<InputManager>();	
			netman = GameObject.Find("PhotonNetman").GetComponent<Netman>();
		}
	}
	
	void Update() {
		playername.transform.LookAt(- 9000 *  Camera.main.transform.position);	
	}
	
	public void SetSpawnPoint( Vector3 pos ) {
		spawnPoint = pos;	
	}
	
	[RPC]
	public void SetColor( Vector3 rgb ) {
		color = new Color(rgb[0],rgb[1],rgb[2], 1.0f);
		this.renderer.material.SetColor("_Color",color);
	}
	
	[RPC]
	public void HitBy( PhotonPlayer player ) {
		lastHit = player;	
	}
	
	public void OnDeath() {
		if ( lastHit != null ) {
			Debug.Log("Killed by " + lastHit.name + " [" + lastHit.ID + "]");
			if(lastHit != photonView.owner)
				netman.gameObject.GetPhotonView().RPC("IncreaseScore",PhotonTargets.AllBuffered,lastHit.ID);
		}
		this.transform.position = spawnPoint;	
		inman.ResetMoveTo();
	}
}
