using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour {
	
	public Color sceneViewDisplayColor = new Color(0.9f, 0.0f, 0.0f, 0.5f);
	
	public void OnTriggerEnter( Collider other  ) {
		other.gameObject.SendMessage ("OnDeath", SendMessageOptions.DontRequireReceiver);
		Debug.Log("Dead");
	}
	
	public void OnDrawGizmos () {
		Bounds bounds = GetComponent<BoxCollider>().bounds;
		Transform transform = GetComponent<Transform>();
		
		Vector3 topFrontRight = (bounds.center + bounds.extents);
		Vector3 topFrontLeft = (bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, 1, 1)));
		Vector3 topBackRight = (bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, 1, -1)));
		Vector3 topBackLeft = (bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, 1, -1)));
		Vector3 bottomFrontRight = (bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, -1, 1)));
		Vector3 bottomFrontLeft = (bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, -1, 1)));
		Vector3 bottomBackRight = (bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, -1, -1)));
		Vector3 bottomBackLeft = (bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, -1, -1)));
		
		Gizmos.color = sceneViewDisplayColor;
		Gizmos.DrawLine(topFrontRight,topFrontLeft);
		Gizmos.DrawLine(topFrontRight,topBackRight);
		Gizmos.DrawLine(topBackLeft,topFrontLeft);
		Gizmos.DrawLine(topBackLeft,topBackRight);
		Gizmos.DrawLine(topBackLeft,topFrontRight);
		Gizmos.DrawLine(topBackRight,topFrontLeft);
		Gizmos.DrawLine(bottomFrontRight,bottomFrontLeft);
		Gizmos.DrawLine(bottomFrontRight,bottomBackRight);
		Gizmos.DrawLine(bottomBackLeft,bottomFrontLeft);
		Gizmos.DrawLine(bottomBackLeft,bottomBackRight);
		Gizmos.DrawLine(bottomBackLeft,bottomFrontRight);
		Gizmos.DrawLine(bottomBackRight,bottomFrontLeft);
		Gizmos.DrawLine(topFrontLeft,bottomFrontLeft);
		Gizmos.DrawLine(topFrontRight,bottomFrontRight);
		Gizmos.DrawLine(topBackLeft,bottomBackLeft);
		Gizmos.DrawLine(topBackRight,bottomBackRight);
		
	}
}
