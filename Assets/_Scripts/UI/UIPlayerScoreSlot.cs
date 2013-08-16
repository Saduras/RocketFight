using UnityEngine;
using System.Collections;
using System;

public class UIPlayerScoreSlot : MonoBehaviour {

	public UISlicedSprite slotBackground;
	public UILabel slotNameLabel;
	public UILabel slotScoreLabel;
	
	public Color inactiveColor;
	public Color activeBackgroundColor;
	
	private int playerID = 0;

	// Use this for initialization
	void Awake() {
		// deactivate this slot as default
		Deactivate();
	}
	
	/**
	 * Active this player slot and update name.
	 */
	public void SetName( PhotonPlayer player) {
		// enable label and set text
		slotNameLabel.enabled = true;
		slotNameLabel.text = player.name;
		playerID = player.ID;
		slotScoreLabel.enabled = true;
		slotScoreLabel.text = "0";
		
		// set background color to active color
		slotBackground.color = activeBackgroundColor;
	}
	
	/**
	 * Set the value of the score label
	 */
	public void SetScore( int scoreValue ) {
		slotScoreLabel.text = scoreValue.ToString();
	}
	
	/**
	 * Return the value of the score label as int
	 */ 
	public int GetScore() {
		return Int32.Parse( slotScoreLabel.text );
	}
	
	/**
	 * Hide icon and label and change background to inactive color
	 */
	public void Deactivate() {
		// hide label
		slotNameLabel.enabled = false;
		slotScoreLabel.enabled = false;
		
		// change background color to inactive
		slotBackground.color = inactiveColor;
	}
	
	public int GetID() {
		return playerID;
	}
}
