using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel))]
public class ScorePopup : MonoBehaviour {
	
	public float lifeTime = 1;
	public Vector3 speed = Vector3.zero;
	public bool fade = true;
	
	private float startTime;

	// Use this for initialization
	void Awake () {
		// move to constant distance to camera
		Vector3 distanceToMainCam = transform.position - Camera.main.transform.position;
		distanceToMainCam.Normalize();
		distanceToMainCam *= 8f;
		transform.position = Camera.main.transform.position + distanceToMainCam;
		
		// rotate such that the text look at the cam
		transform.LookAt(- Camera.main.transform.position * 9000);
		
		// set starttime
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		// destroy object at the end of its lifetime
		if(Time.time > startTime + lifeTime)
			Destroy(gameObject);
		
		// move the labe according to the speed vector
		transform.Translate(speed * Time.deltaTime);
		
		// fade out (alpha) if enabled
		if(fade) {
			float alpha = 1 - (Time.time - startTime) / lifeTime;
			GetComponent<UILabel>().alpha = alpha;
		}
	}
}
