using UnityEngine;
using System.Collections;

public class ChangeUIStateButton : MonoBehaviour {

	private UIMenu uiMenu;
	// the UIState you want to change to on click
	public UIMenu.UIState newState;
	
	/**
	 * Initalize references
	 */ 
	void Start() {
		uiMenu = GameObject.FindGameObjectWithTag("UICamera").GetComponent<UIMenu>();
	}
	
	/**
	 * Call ChangeState(), wich hides all panels and unhides the requested one
	 */ 
	void OnClick() {
		uiMenu.ChanceState(newState);
	}
}
