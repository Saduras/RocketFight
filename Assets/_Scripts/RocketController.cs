using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour {
	
	public static float speed = 10;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Translate( this.transform.forward * speed * Time.deltaTime );
	}
}
