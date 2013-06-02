using UnityEngine;
using System.Collections;

public class PhysicTest : MonoBehaviour {
	
	private float[] fadeTime = new float[] {0.5f, 1f, 1.75f};
	private Vector3[] force = new Vector3[]{ Vector3.forward * 1f, Vector3.forward * 1f, Vector3.forward * 2f, Vector3.forward * 5f, Vector3.forward * 10f};
	private float dist = 0f;
	private float deltaTimes = 0.0f;
	private Vector3 currVec;
	private float currFTime;
	private float startTime;
	private int i = 0;
	private int j = 0;
	private float epsilon = 0.05f;
	

	// Use this for initialization
	void Start () {
		currVec = force[i];
		currFTime = fadeTime[j];
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if( i < force.Length ) {
			float t = Time.time - startTime;
			if ( t + Time.deltaTime < currFTime ) {
				// move along currVec and reduce this currVec
				Vector3 dv = force[i] * Time.deltaTime / fadeTime[j] * (2f * Mathf.Pow(1 - t / fadeTime[j], 4f) );
				this.transform.Translate(dv);
				dist += dv.magnitude;
				currVec -= dv;
				deltaTimes += Time.deltaTime;
				
			} else {
				Debug.Log("Fading["+i+","+j+"] ended: \n" +
					"force: " + force[i] + "\n" +
					"dist: " + dist + "\n" +
					"realDist: " + Vector3.Distance(transform.position, Vector3.zero) + "\n" +
					"distError: " + Mathf.Abs(dist - Vector3.Distance(transform.position, Vector3.zero)) + "\n" +
					"fadeTime: " + fadeTime[j] + "\n" +
					"deltaTimes: " + deltaTimes + "\n" +
					"timeError:" + Mathf.Abs(fadeTime[j] - deltaTimes) );
				
				j++;
				if( j >= fadeTime.Length ){
					j = 0;
					i++;
				}
				if( i < force.Length ) {
					currVec = force[i];
					currFTime = fadeTime[j];
					startTime = Time.time;
					transform.position = Vector3.zero;
					dist = 0f;
					deltaTimes = 0f;
				}
			}
		}
	}
}
