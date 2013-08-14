using UnityEngine;
using System.Collections;

public class AutoDestructAnimation : Photon.MonoBehaviour {
	
	public bool OnlyDeactivate;
	public bool network;
	
	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}
	
	/**
	 * Check every 500ms if the animation is still playing
	 * if not destroy gameobject
	 */ 
	IEnumerator CheckIfAlive ()	{
		while(true){
			yield return new WaitForSeconds(0.33f);
			if(!animation.isPlaying) {
				if(OnlyDeactivate) {
					this.gameObject.SetActive(false);
				} else {
					if(network) {
						if( photonView.owner == PhotonNetwork.player ) {
							PhotonNetwork.Destroy(this.gameObject);
						}
					} else { 
						Destroy(gameObject);
					}
				}
				break;
			}
		}
	}
}


