using UnityEngine;
using System.Collections;

public class PlayerManager : Photon.MonoBehaviour {

	private GameObject spawnPoint;
	private InputManager inman;
	
	void Start () {
		if (!(photonView.owner == PhotonNetwork.player) ) {
			this.enabled = false;
			
		} else {
			inman = this.gameObject.GetComponent<InputManager>();	
		}
	}
	
	public void SetSpawnPoint( GameObject go ) {
		this.spawnPoint = go;	
	}
	
	public void OnDeath() {
		this.transform.position = this.spawnPoint.transform.position;	
		inman.ResetMoveTo();
	}
}
