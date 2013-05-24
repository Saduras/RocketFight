using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class NetmanGUI : Photon.MonoBehaviour {

	public int guiSpace = 5;
	public string setting = "1";

	public void OnGUI()
    {
        GUILayout.Space(guiSpace);

        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
        {
            if (GUILayout.Button("Connect"))
            {
                PhotonNetwork.ConnectUsingSettings( setting );
            }
        }
        else
        {
            if (GUILayout.Button("Disconnect"))
            {
                PhotonNetwork.Disconnect();
            }
        }
		
		Netman nman = this.gameObject.GetComponent<Netman>();
		
		if (PhotonNetwork.room != null && !nman.hasSpawn) {
			if (GUILayout.Button("Spawn")) {
				this.gameObject.GetComponent<Netman>().SpawnPlayer(0);	
			}
		}
    }
}
