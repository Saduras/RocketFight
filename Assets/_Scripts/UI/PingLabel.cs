using UnityEngine;
using System.Collections;

public class PingLabel : MonoBehaviour {
	
	
	private UILabel label;
	
	// Use this for initialization
	void Start () {
		label = gameObject.GetComponent<UILabel>();
	}
	
	/**
	 * Set label text to current network ping and set color, depending on ping value
	 * to something between green and red.
	 */ 
	void Update () {
		int ping = PhotonNetwork.GetPing();
		float r = (float)PhotonNetwork.GetPing()/300;
		float g = 1- r;
		label.text = "Ping: [" + ColorX.RGBToHex(new Color(r,g,0,1)) + "]";
		label.text += ping.ToString();
	}
}
