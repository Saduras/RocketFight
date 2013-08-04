using UnityEngine;
using System.Collections;

public class CursorBehaviour : MonoBehaviour {

	public Texture2D menuCursorTex;
	public Match match;
	
	// crosshair components
	public UISprite ring;
	public UISprite topArrow;
	public UISprite rightArrow;
	public UISprite botArrow;
	public UISprite leftArrow;
	
	// these are used to calculate the uiOffset
	private static float targetAspectRation = 16.0f/9.0f;
	private float screenAspectRation;
	private static float uiHeight = 900;
	private Vector2 uiOffset;
	
	// Use this for initialization
	void Start () {
		Cursor.SetCursor(menuCursorTex, Vector2.zero, CursorMode.Auto);
	}
	
	// Update is called once per frame
	void Update () {
		if( match.IsRunning() ) {
			if( Screen.showCursor ) {
				ChangeToCrossHair();
				CalculateOffset();
			}
			// map mouse position into ui space using the ui offset
			Vector3 pos = new Vector3(0,0,-1);
			pos.x = Input.mousePosition.x / Screen.width * (uiHeight * targetAspectRation + 2*uiOffset.x);
			pos.y = Input.mousePosition.y / Screen.height * (uiHeight + 2*uiOffset.y);
			pos = pos - new Vector3(uiOffset.x, uiOffset.y);
			
			// check if pos is inside the ui camera and correct if necessary
			if( pos.x < 0 )
				pos.x = 0;
			if( pos.y < 0 )
				pos.y = 0;
			if( pos.x > uiHeight * targetAspectRation )
				pos.x = uiHeight * targetAspectRation;
			if( pos.y > uiHeight )
				pos.y = uiHeight;
			
			// apply calculates position
			transform.localPosition = pos;
		} else {
			if( !Screen.showCursor )
				ChangeToMenuCursor();
		}
	}
	
	/**
	 * Disable menu cursor und activate all crosshair components
	 */
	private void ChangeToCrossHair() {
		Screen.showCursor = false;
		ring.gameObject.SetActive( true );
		topArrow.gameObject.SetActive( true );
		rightArrow.gameObject.SetActive( true );
		botArrow.gameObject.SetActive( true );
		leftArrow.gameObject.SetActive( true );
	}
	
	/**
	 * Deactivate all components of the crosshair and re-enable menu cursor
	 */
	private void ChangeToMenuCursor() {
		Screen.showCursor = true;
		ring.gameObject.SetActive( false );
		topArrow.gameObject.SetActive( false );
		rightArrow.gameObject.SetActive( false );
		botArrow.gameObject.SetActive( false );
		leftArrow.gameObject.SetActive( false );
	}
	
	/**
	 * Calculate the ui offset which will used to map the mouse position to ui space
	 */
	private void CalculateOffset() {
		screenAspectRation = Screen.width / Screen.height;
		if( screenAspectRation / targetAspectRation < 1.0f ) {
			// we are using full width but not full height
			
			// calulate the camera height in screen coordiantes
			float realHeight = Screen.width/targetAspectRation;
			// now calculate the offset in ui coordiantese
			uiOffset = new Vector2(0, (Screen.height - realHeight)/2  *(uiHeight * targetAspectRation / Screen.width) );
		} else {
			//we are using full height but not full width
			
			// calulate the camera width in screen coordiantes
			float realWidth = Screen.height * targetAspectRation;
			// now calculate the offset in ui coordiantes
			uiOffset = new Vector2((Screen.width - realWidth)/2  *(uiHeight/ Screen.height),0 );			
		}
	}
}
