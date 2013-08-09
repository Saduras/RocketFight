using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour {
	
	public bool isOutside;
	
	/**
	 * Send Message "OnDeath" to everything entering this trigger.
	 * Also sends Message "OnLeaveArena" if it is flagged as outside collider
	 */
	public void OnTriggerEnter( Collider other  ) {
		other.gameObject.SendMessage ("OnDeath", SendMessageOptions.DontRequireReceiver);
		if(isOutside)
			other.gameObject.SendMessage ("OnLeaveArena", SendMessageOptions.DontRequireReceiver);
		Debug.Log("Something entered deathzone");
	}
}
