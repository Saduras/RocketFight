using UnityEngine;
using System.Collections;

public class CursorBehaviour : MonoBehaviour {

	public Texture2D menuCursorTex;
	public Match match;
	
	public UISprite ring;
	public UISprite topArrow;
	public UISprite rightArrow;
	public UISprite botArrow;
	public UISprite leftArrow;
	
	public Camera camera2D;
	
	// Use this for initialization
	void Start () {
		Debug.Log("Set cursor!");
		Cursor.SetCursor(menuCursorTex, Vector2.zero, CursorMode.Auto);
	}
	
	// Update is called once per frame
	void Update () {
		if( match.IsRunning() ) {
			if( Screen.showCursor ) 
				ChangeToCrossHair();
			
			Vector3 pos = new Vector3(0,0,-1);
			pos.x = Input.mousePosition.x / Screen.width * 1600;
			pos.y = Input.mousePosition.y / Screen.height * 900;
			
			
			transform.localPosition = pos;
		} else {
			if( !Screen.showCursor )
				ChangeToMenuCursor();
		}
	}
	
	private void ChangeToCrossHair() {
		Screen.showCursor = false;
		ring.gameObject.SetActive( true );
		topArrow.gameObject.SetActive( true );
		rightArrow.gameObject.SetActive( true );
		botArrow.gameObject.SetActive( true );
		leftArrow.gameObject.SetActive( true );
	}
	
	private void ChangeToMenuCursor() {
		Screen.showCursor = true;
		ring.gameObject.SetActive( false );
		topArrow.gameObject.SetActive( false );
		rightArrow.gameObject.SetActive( false );
		botArrow.gameObject.SetActive( false );
		leftArrow.gameObject.SetActive( false );
	}
}
