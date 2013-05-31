using UnityEngine;
using System.Collections;

public class PlayerManager : Photon.MonoBehaviour {

	private Vector3 spawnPoint;
	private InputManager inman;
	private Color color;
	
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
	
	public void OnDeath() {
		this.transform.position = spawnPoint;	
		inman.ResetMoveTo();
	}
}
