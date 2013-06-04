using UnityEngine;
using System.Collections;

public class PlayerManager : Photon.MonoBehaviour {

	private Vector3 spawnPoint;
	private InputManager inman;
	private Color color;
	private PhotonPlayer lastHit;
	
	
	void Start () {
		if (!(photonView.owner == PhotonNetwork.player) ) {
			this.enabled = false;
			
		} else {
			inman = this.gameObject.GetComponent<InputManager>();	
		}
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
		Debug.Log("Killed by " + lastHit.name + " [" + lastHit.ID + "]");
		this.transform.position = spawnPoint;	
		inman.ResetMoveTo();
	}
}
