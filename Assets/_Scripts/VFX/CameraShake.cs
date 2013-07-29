using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	
	public float duration;
	public float strength;
	
	private float startTime;
	private Vector3 defaultPos;

	// Use this for initialization
	void Start () {
		defaultPos = transform.position;
	}
	
	// Update is called once per frame
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
		startTime = Time.time;	
	}
}
