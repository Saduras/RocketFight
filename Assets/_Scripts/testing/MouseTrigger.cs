using UnityEngine;
using System.Collections;

public class MouseTrigger : MonoBehaviour {
	
	public GameObject explosion;
	
	// Update is called once per frame
	void Update () {
		// on left clickt spawn explosion
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit[] hitList = Physics.RaycastAll( ray );
			foreach( RaycastHit hit in hitList ) {
				if( hit.collider.CompareTag("Ground") ) {
					Vector3 pos = hit.point;
					pos.y = 0.5f;
					
					PhotonNetwork.Instantiate( explosion.name, pos, Quaternion.identity, 0);
				}
			}
		}
	}
}
