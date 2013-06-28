using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InGamePanelController : MonoBehaviour {
	
	public List<GameObject> inGamePanels;
	
	private Netman nman;
	
	// Use this for initialization
	void Start () {
		nman = GameObject.Find("PhotonNetman").GetComponent<Netman>();
	}
	
	// Update is called once per frame
	void Update () {
		foreach( GameObject panel in inGamePanels ) {
			panel.SetActive( nman.hasSpawn );
		}
	}
}
