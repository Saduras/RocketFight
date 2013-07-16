using UnityEngine;
using System.Collections;

public class FinishButton : MonoBehaviour {
	
	public Match match;
	
	void OnClick() {
		match.Reset();
	}
}
