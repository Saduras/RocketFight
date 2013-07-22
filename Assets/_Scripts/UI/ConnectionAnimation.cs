using UnityEngine;
using System.Collections;

public class ConnectionAnimation : MonoBehaviour {
	
	public Color activeColor;
	public Color inactiveColor;
	public float stepTime = 1f;
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
	
	// Update is called once per frame
	void Update () {
		if( Time.time > lastStep + stepTime ) {
			index++;
			if( index >= spriteList.Length )
				index = 0;
			
			for( int i=0; i<spriteList.Length; i++ ) {
				if( i == index )
					spriteList[i].color = activeColor;
				else
					spriteList[i].color = inactiveColor;
			}
			lastStep = Time.time;
		}
		
		if( PhotonNetwork.connectionStateDetailed == PeerState.Joined )
			uiMenu.ChanceState(UIMenu.UIState.LOBBY);
	}
}
