using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class ExplosionAnimation : Photon.MonoBehaviour {
	
	public float growSpeed = 5;
	public float lifeTime = 1;
	private Vector3 initScale;
	private float birthTime;
	
	// Use this for initialization
	void Start () {
		initScale = this.transform.localScale;
		birthTime = Time.time;
		this.renderer.material.shader = Shader.Find("Transparent/Diffuse");
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.localScale +=  initScale * growSpeed * Time.deltaTime;
		Color tmpColor = this.renderer.material.color;
		tmpColor.a = 1 - (Time.time - birthTime)  / lifeTime;
		this.renderer.material.color = tmpColor;
		
		if( (photonView.owner == PhotonNetwork.player) && (birthTime + lifeTime < Time.time) ) {
			PhotonNetwork.Destroy( this.gameObject );	
		}
	}
}
