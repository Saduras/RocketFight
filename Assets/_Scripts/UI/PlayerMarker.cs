using UnityEngine;
using System.Collections;

public class PlayerMarker : Photon.MonoBehaviour {
	
	private Transform parent;
	private Animation anim;
	
	// Use this for initialization
	void Start () {
		anim = GetComponentInChildren<Animation>();
		foreach( AnimationState state in anim ) {
			state.speed = 0.3f;	
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = parent.position + Vector3.up * 1.5f + Vector3.forward * 0.5f;
		
		if( !anim.isPlaying )
			Destroy( gameObject );
	}
	
	public void SetParent( Transform transform ) {
		parent = transform;
		GetComponentInChildren<Renderer>().material.color = parent.GetComponentInChildren<SkinnedMeshRenderer>().material.color;
	}	
}
