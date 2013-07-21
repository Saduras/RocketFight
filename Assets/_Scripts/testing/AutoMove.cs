using UnityEngine;
using System.Collections;

public class AutoMove : Photon.MonoBehaviour {
	
	private GameObject[] wps;
	public float speed = 5;
	
	private GameObject target;
	private int index;
	private Predictor predictor;

	// Use this for initialization
	void Start () {
		if( !(photonView.owner == PhotonNetwork.player) )
			enabled = false;
		
		wps = GameObject.FindGameObjectsWithTag("Respawn");
		target = wps[0];
		index = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if( (target.transform.position - transform.position).magnitude > 0.2f ) {
			// move towards wp
			Vector3 move = (target.transform.position - transform.position) * Time.deltaTime * speed;
			transform.Translate( move );
		} else {
			// get next wp
			index++;
			if( index >= wps.Length )
				index = 0;
			
			target = wps[index];
		}
	}
}
