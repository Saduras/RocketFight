using UnityEngine;
using System.Collections;

public class ChangeUIStateButton : MonoBehaviour {

	public UIMenu uiMenu;
	public UIMenu.UIState newState;
	
	void OnClick() {
		uiMenu.ChanceState(newState);
	}
}
