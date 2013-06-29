using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour {
	
	public void OnTriggerEnter( Collider other  ) {
		other.gameObject.SendMessage ("OnDeath", SendMessageOptions.DontRequireReceiver);
	}
}
