using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour {
	
	/**
	 * Send Message "OnDeath" to everything entering this trigger.
	 */
	public void OnTriggerEnter( Collider other  ) {
		other.gameObject.SendMessage ("OnDeath", SendMessageOptions.DontRequireReceiver);
	}
}
