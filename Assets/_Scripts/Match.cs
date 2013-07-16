using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Match : Photon.MonoBehaviour {
	
	private List<RocketFightPlayer> playerList = new List<RocketFightPlayer>();
	
	private MatchState currentState;
	private bool allReady = false;
	private bool startRequest = false;
	private double startTime;
	private float gameTime;
	
	private double countdownTime;
	
	public int maxPlayerCount = 4;
	public float matchLength = 5;
	public int countdownLength;
	public string arenaScene = "Arena";
	public string respawnTag = "Respawn";
	
	// stuff for instantiate spawn points
	public GameObject spawnPointPrefab;
	public List<Vector3> positions = new List<Vector3>();
	
	public UILabel countdownLabel;
	public UILabel playerListLabel;
	public UIMenu uiMenu;
	public GameObject playerPrefab;
	public List<Color> freeColors = new List<Color>();
	
	// sound & music stuff
	public AudioSource matchMusic;
	public AudioSource countdownSound;
	
	private List<Color> usedColors = new List<Color>();
	private bool arenaLoaded = false;
	private bool sent = false;
	
	public enum MatchState {
		SETTINGUP,
		COUNTDOWN,
		RUNNING,
		FINISHED
	}
	
	
	void Update() {
		switch(currentState) {
		case MatchState.RUNNING:
			// use network time to calculate game time
			// this ensures everyone has the same time
			gameTime = (float) (matchLength + startTime - PhotonNetwork.time);
			if( gameTime <= 0 ) {
				GameOver();
			}
			break;
		case MatchState.SETTINGUP:
			if(startRequest && allReady && PhotonNetwork.isMasterClient) {
				photonView.RPC("InitCountdown",PhotonTargets.AllBuffered, PhotonNetwork.time);
				startRequest = false;
			}
			break;
		case MatchState.COUNTDOWN:
			int currentStep = Mathf.FloorToInt( (float) (countdownTime + countdownLength + 1 - PhotonNetwork.time) );
			if( currentStep > 0)
				countdownLabel.text = currentStep.ToString();
			if( currentStep == 0 )
				countdownLabel.text = "go";
			if( currentStep < 0 && PhotonNetwork.isMasterClient )
				photonView.RPC("StartMatch",PhotonTargets.AllBuffered, PhotonNetwork.time);
			break;
		}
		
		// finshed loading level
		if(!Application.isLoadingLevel && arenaLoaded && !sent) {	
			photonView.RPC("LoadingFinished",PhotonTargets.AllBuffered,PhotonNetwork.player);
			sent = true;
		}
	}
	
	public void Reset() {
		// reset colors
		freeColors.AddRange( usedColors );
		usedColors.Clear();
		
		// reload player list
		playerList.Clear();
		foreach( PhotonPlayer player in PhotonNetwork.playerList ) {
			AddPlayer( player );	
		}
		
		currentState = MatchState.SETTINGUP;
		startRequest = false;
		gameTime = matchLength;
		UpdateUIPlayerList();
	}
	
	public void Init() {
		gameTime = matchLength;
		foreach( RocketFightPlayer rfp in playerList ) {
			rfp.score = 0;	
		}
	}
	
	[RPC]
	public void InitCountdown(double timestamp) {
		countdownTime = timestamp;
		currentState = MatchState.COUNTDOWN;
		countdownLabel.gameObject.SetActive( true );
		countdownSound.Play();
	}
	
	[RPC]
	public void AddPlayer(PhotonPlayer newPlayer) {
		// check playerlist count
		if( playerList.Count >= maxPlayerCount ) 
			return;
		
		// ensure the player isn't already in the list
		List<RocketFightPlayer> results = playerList.FindAll(delegate(RocketFightPlayer rfp) {
			if(rfp.photonPlayer == newPlayer)
				return true;
			else
				return false;
		});
		if( results.Count > 0 ) return;
		
		// get Color
		Color playerColor = freeColors[freeColors.Count - 1];
		freeColors.RemoveAt(freeColors.Count - 1);
		usedColors.Add(playerColor);
		
		// add new RocketFightPlayer
		playerList.Add( new RocketFightPlayer(newPlayer, playerColor));
		
		// update UI
		UpdateUIPlayerList();
	}
	
	public bool IsRunning() {
		return (currentState == MatchState.RUNNING);	
	}
	
	public float GetGameTime() {
		return gameTime;	
	}
	
	public List<RocketFightPlayer> GetPlayerList() {
		return playerList;	
	}
	
	[RPC]
	public void RemovePlayer(PhotonPlayer player) {
		// find player
		foreach(RocketFightPlayer rfp in playerList) {
			if( rfp.photonPlayer == player ) {
				// free color
				usedColors.Remove(rfp.color);
				freeColors.Add(rfp.color);
				// fremove from list
				playerList.Remove(rfp);
				// update UI
				UpdateUIPlayerList();
				return;
			}
		}
	}
	
	public void UpdateUIPlayerList() {
		string labelString = "";
		foreach( RocketFightPlayer rfp in playerList ) {
			labelString += "[" + ColorX.RGBToHex(rfp.color) + "]";
			labelString += rfp.photonPlayer.name + " : (" + rfp.score + ")";
			labelString += "[ffffff]";
			if( rfp.photonPlayer.isMasterClient ) 
				labelString += " (Master)";
			labelString += "\n";
		}
		
		playerListLabel.text = labelString;	
	}
	
	[RPC]
	public void LoadingFinished(PhotonPlayer player) {
		Debug.Log("Loading finished by: " + player.name + " [" + player.ID + "]");
		allReady = true;
		
		// set levelLoaded = true
		foreach( RocketFightPlayer rfp in playerList ) {
			if( rfp.photonPlayer == player ){
				rfp.levelLoaded = true;	
			}
			// check if someone is not finished yet
			if( !rfp.levelLoaded) {
				allReady = false;	
			}
		}
	}
	
	[RPC]
	public void RequestStart() {
		if(!arenaLoaded) {
			Application.LoadLevelAdditive(arenaScene);
			arenaLoaded = true;
		}
		startRequest = true;
	}
	
	[RPC]
	public void StartMatch(double timestamp) {
		Debug.Log("Match started!");
		currentState = MatchState.RUNNING;
		startTime = timestamp;
		Init();
		// play background music
		matchMusic.Play();
		countdownLabel.gameObject.SetActive( false );
		
		if( PhotonNetwork.isMasterClient )
			OrganizeSpawning();
	}
	
	private void GameOver() {
		currentState = MatchState.FINISHED;
		if(PhotonNetwork.isMasterClient) {
			foreach( RocketFightPlayer rfp in playerList ) {
				PhotonNetwork.DestroyPlayerObjects(rfp.photonPlayer);	
			}
		}
		uiMenu.ChanceState(UIMenu.UIState.MATCHOVER);
		
		// stop background music
		matchMusic.Stop();
	}
	
	/**
	 * Called when new level was loaded.
	 * If we loaded the game scene, find a respawn point for you.
	 * Then spawn your player object at this spawn point.
	 */
	[RPC]
	public void SpawnPlayer(Vector3 spawnPt, Vector3 rgb) {
		Debug.Log("Instatiate player at " + spawnPt);
		GameObject handle = PhotonNetwork.Instantiate(playerPrefab.name,spawnPt,Quaternion.identity,0);
		handle.GetComponent<InputManager>().SendMessage("SetPlayer", PhotonNetwork.player);
		handle.GetPhotonView().RPC("SetColor",PhotonTargets.AllBuffered,rgb);
	}
	
	public void OrganizeSpawning() {
		if( !PhotonNetwork.isMasterClient ) 
			return;
		
		// instatiate spawn points
		foreach( Vector3 pos in positions) {
			PhotonNetwork.Instantiate(spawnPointPrefab.name,pos, Quaternion.identity,0);	
		}
		
		// find spawnpoints
		GameObject[] gos = GameObject.FindGameObjectsWithTag(respawnTag);
		List<GameObject> spawnPoints = new List<GameObject>(gos);
		Debug.Log("Spawnpoints : " + spawnPoints.Count);
		
		foreach( RocketFightPlayer rfp in playerList ) {
			// choose random spawn point
			int randIndex = Mathf.RoundToInt(Random.Range(0, spawnPoints.Count));
			GameObject sp = spawnPoints[randIndex];
			spawnPoints.RemoveAt(randIndex);
				
			Vector3 rgb = new Vector3( rfp.color.r, rfp.color.g,rfp.color.b );
			
			// assign spawpoint
			sp.GetPhotonView().RPC("AssignTo",PhotonTargets.AllBuffered,rfp.photonPlayer);
			sp.GetPhotonView().RPC("SetColor",PhotonTargets.AllBuffered,rgb);
			
			// spawn player incl. color
			photonView.RPC("SpawnPlayer",rfp.photonPlayer,sp.transform.position, rgb);
			// init score
			photonView.RPC("SetScore",PhotonTargets.AllBuffered, rfp.photonPlayer, 0);
		}
	}
	
	[RPC]
	public void IncreaseScore(int playerID, int val) {
		foreach( RocketFightPlayer rfp in playerList ) {
			if( rfp.photonPlayer.ID == playerID ) {
				rfp.score += val;
				
				if( rfp.photonPlayer == PhotonNetwork.player ) {
					GameObject[] characterObjects = GameObject.FindGameObjectsWithTag("Player");
					foreach( GameObject character in characterObjects ) {
						if( character.GetPhotonView().owner == rfp.photonPlayer ) {
							character.GetComponent<PlayerManager>().PopupScore( val );
						}
					}
				}
			}
		}
	}
	
	[RPC]
	public void SetScore(PhotonPlayer target, int val) {
		foreach( RocketFightPlayer rfp in playerList ) {
			if( rfp.photonPlayer == target ) {
				rfp.score = val;
				return;
			}
		}
	}
}
