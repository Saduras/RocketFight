using UnityEngine;
using System.Collections;

public class ChangeUIStateButton : MonoBehaviour {

	private UIMenu uiMenu;
	public UIMenu.UIState newState;
	
	void Start() {
		uiMenu = GameObject.FindGameObjectWithTag("UICamera").GetComponent<UIMenu>();
	}
	
	void OnClick() {
		uiMenu.ChanceState(newState);
	}
}
