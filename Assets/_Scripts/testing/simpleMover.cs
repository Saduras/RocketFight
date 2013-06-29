using UnityEngine;
using System.Collections;

public class simpleMover : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();
		controller.Move( new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical") ));
	}
}
