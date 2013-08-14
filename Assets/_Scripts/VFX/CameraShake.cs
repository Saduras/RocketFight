using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	
	public float duration;
	public float strength;
	
	private float startTime;
	private Vector3 defaultPos;
	private float defaultStrength;

	// Use this for initialization
	void Start () {
		defaultPos = transform.position;
		defaultStrength = strength;
	}
	
	
	/**
	 * Move camera randomly around defaultPos unitl duration is over.
	 * Move to default postion if shaking time is over.
	 * Since the camera is not moving, it's not a problem to set it's postion to default each frame.
	 */ 
	void Update () {
		if( startTime + duration > Time.time ) {
			// do shaking	
			Vector3 shake = new Vector3( Random.Range(0,100), Random.Range(0,100), Random.Range(0,100) );
			shake = shake.normalized * strength;
			transform.position = defaultPos + shake;
		} else {
			// reset to default positon
			transform.position = defaultPos;
		}
	}
	
	public void Shake() {
		Debug.Log("Shake");
		startTime = Time.time;	
		strength = defaultStrength;
	}
	
	public void Shake(float str) {
		Debug.Log("Shake strong");
		startTime = Time.time;
		strength = str;
	}
}
