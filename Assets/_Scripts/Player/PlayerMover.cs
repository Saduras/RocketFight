using UnityEngine;
using System.Collections;

/**
 * This class holds all information about current player movement and executes them.
 */
public class PlayerMover : MonoBehaviour {
	
	Vector3 inputMovement = Vector3.zero;
	Vector3 physicMovement = Vector3.zero;
	public float movementSpeed = 5;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 frameMove = inputMovement * movementSpeed;
		frameMove += physicMovement;
		transform.Translate( frameMove * Time.deltaTime,Space.World);
	}
	
	public void SetInputMovement( Vector3 vector ) {
		if( vector.magnitude != 1 )
			vector.Normalize();
		
		if( vector != inputMovement )
			inputMovement = vector;
	}
	
	public void SetPhysicMovement( Vector3 vector ) {
		if( vector != physicMovement )
			physicMovement = vector;
	}
	
	public void Teleport( Vector3 pos ) {
		transform.position = pos;	
	}
}
