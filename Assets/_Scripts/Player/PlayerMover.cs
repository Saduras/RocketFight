using UnityEngine;
using System.Collections;

/**
 * This class holds all information about current player movement and executes them.
 */
public class PlayerMover : MonoBehaviour {
	
	// direction we get from the player input
	Vector3 inputMovement = Vector3.zero;
	// sum of all forces which influence the character now
	Vector3 physicMovement = Vector3.zero;
	
	// speed of the character movement through player input
	public float movementSpeed = 5;

	/**
	 * Update is called once per frame
	 * Apply movement from input and physics to the player
	 */
	void Update () {
		Vector3 frameMove = inputMovement * movementSpeed;
		frameMove += physicMovement;
		transform.Translate( frameMove * Time.deltaTime,Space.World);
	}
	
	/**
	 * Set the vector value of inputMovement.
	 * It will be normalized, if it isn't already since we only use the direction.
	 */
	public void SetInputMovement( Vector3 vector ) {
		if( vector.magnitude != 1 )
			vector.Normalize();
		
		if( vector != inputMovement )
			inputMovement = vector;
	}
	
	/**
	 * Set the sum of the physical effects moving the character.
	 */
	public void SetPhysicMovement( Vector3 vector ) {
		if( vector != physicMovement )
			physicMovement = vector;
	}
	
	/**
	 * Immediatly set the charater to a new position.
	 */
	public void Teleport( Vector3 pos ) {
		transform.position = pos;	
	}
	
	/**
	 * Gives the max speed of character movement through play input.
	 */
	public float GetMovementSpeed() {
		return movementSpeed;	
	}
}
