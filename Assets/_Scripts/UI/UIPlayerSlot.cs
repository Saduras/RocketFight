using UnityEngine;
using System.Collections;

public class UIPlayerSlot : MonoBehaviour {
	
	public UISlicedSprite slotBackground;
	public UISlicedSprite slotBackgroundHighlight;
	public UILabel slotLabel;
	public UISprite masterClientIcon;
	
	public Color inactiveColor;
	public Color activeBackgroundColor;

	// Use this for initialization
	void Start () {
		// deactivate this slot as default
		Deactivate();
	}
	
	/**
	 * Active this player slot and update name. Use isMater and isLocal to
	 * enable/disable master client icon and local player high light.
	 */
	public void Set(string playerName, bool isMaster, bool isLocal) {
		// enable label and set text
		slotLabel.enabled = true;
		slotLabel.text = playerName;
		
		// activate background border if this is the local player
		if( isLocal )
			slotBackgroundHighlight.enabled = true;
		
		// show master client icon
		if( isMaster )
			masterClientIcon.enabled = true;
		
		// set background color to active color
		slotBackground.color = activeBackgroundColor;
	}
	
	/**
	 * Hide icon and label and change background to inactive color
	 */
	public void Deactivate() {
		// hide master icon, label & highlight
		masterClientIcon.enabled = false;
		slotLabel.enabled = false;
		slotBackgroundHighlight.enabled = false;
		
		// change background color to inactive
		slotBackground.color = inactiveColor;
	}
}
