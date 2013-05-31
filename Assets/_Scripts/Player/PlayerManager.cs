using UnityEngine;
using System.Collections;

public class PlayerManager : Photon.MonoBehaviour {

	private Vector3 spawnPoint;
	private InputManager inman;
	
	void Start () {
		if (!(photonView.owner == PhotonNetwork.player) ) {
			this.enabled = false;
			
		} else {
			inman = this.gameObject.GetComponent<InputManager>();	
		}
	}
	
	public void SetSpawnPoint( Vector3 pos ) {
		this.spawnPoint = pos;	
	}
	
	public void OnDeath() {
		this.transform.position = spawnPoint;	
		inman.ResetMoveTo();
	}
}
