using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDestructShuriken : Photon.MonoBehaviour {
	
	public bool OnlyDeactivate;
	
	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}
	
	/**
	 * Check every 500ms if the particlesystem is still alive
	 * if not destroy gameobject
	 */ 
	IEnumerator CheckIfAlive ()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			if(!particleSystem.IsAlive(true))
			{
				if(OnlyDeactivate)
				{
					#if UNITY_4_0
							this.gameObject.SetActive(false);
					#elif UNITY_3_5
							this.gameObject.SetActiveRecursively(false);
					#endif
				}
				else
					if( photonView.owner == PhotonNetwork.player )
						PhotonNetwork.Destroy(this.gameObject);
				break;
			}
		}
	}
}
