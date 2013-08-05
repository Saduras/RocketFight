using UnityEngine;
using System.Collections;

/**
 * Animation, that changes the color of one sprite to active and
 * all other to inactive loop through all sprites in intervalls.
 * Also send you to the lobby if you successfully joined
 */ 
public class ConnectionAnimation : MonoBehaviour {
	
	// color of the single active one
	public Color activeColor;
	// color of all other sprites
	public Color inactiveColor;
	// time intervall
	public float stepTime = 1f;
	// list of sprite you want to loop throug
	public UISprite[] spriteList;
	
	public UIMenu uiMenu;
	
	private float lastStep;
	private int index;
	
	// Use this for initialization
	void Start () {
		lastStep = Time.time;
		index = 0;
		spriteList[0].color = activeColor;
	}
	
	/**
	 * Each intervall switch active-color to the next sprite.
	 * If we joined the room successfully, continue to lobby
	 */ 
	void Update () {
		if( Time.time > lastStep + stepTime ) {
			// increase index, to next one
			index++;
			if( index >= spriteList.Length )
				index = 0;
			
			// recolor sprites
			for( int i=0; i<spriteList.Length; i++ ) {
				if( i == index )
					spriteList[i].color = activeColor;
				else
					spriteList[i].color = inactiveColor;
			}
			// save time to calulate new interval
			lastStep = Time.time;
		}
		
		// continue to lobby on joined room
		if( PhotonNetwork.connectionStateDetailed == PeerState.Joined )
			uiMenu.ChanceState(UIMenu.UIState.LOBBY);
	}
}
