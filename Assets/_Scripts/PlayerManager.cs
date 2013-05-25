using UnityEngine;
using System.Collections;

public class PlayerManager : Photon.MonoBehaviour {

	private GameObject spawnPoint;
	
	public void SetSpawnPoint( GameObject go ) {
		this.spawnPoint = go;	
	}
	
	public void OnDeath() {
		this.transform.position = this.spawnPoint.transform.position;	
	}
}
