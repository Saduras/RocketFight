using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Match : Photon.MonoBehaviour {
	
	// list of all players in this match
	private List<RocketFightPlayer> playerList = new List<RocketFightPlayer>();
	
	// current match state datas
	private MatchState currentState;
	private bool allReady = false;
	private bool startRequest = false;
	private double startTime;
	private float gameTime;
	// countdown start time
	private double countdownTime;
	// match properties
	public int maxPlayerCount = 4;
	public float matchLength = 5;
	public int countdownLength;
	public string arenaScene = "Arena";
	public string respawnTag = "Respawn";
	
	// stuff for instantiate spawn points
	public GameObject spawnPointPrefab;
	public List<Vector3> positions = new List<Vector3>();
	
	// UI objects that will be updated from match
	public UILabel countdownLabel;
	public UILabel finalScoreLabel;
	public UIPlayerLobbySlot[] uiPlayerLobbySlots;
	public ScorePanel scoreBoard;
	public UIMenu uiMenu;
	
	// prefab we spawn as player
	public GameObject playerPrefab;
	// prefab for spawning item
	public GameObject itemPrefab;
	// colors we use the distinguish player
	public List<Color> freeColors = new List<Color>();
	private List<Color> usedColors = new List<Color>();
	
	// sound & music stuff
	public AudioSource matchMusic;
	public AudioSource countdownSound;
	public AudioSource ambientWind;
	
	private bool arenaLoaded = false;
	private bool sent = false;
	
	// player who is holding the item at the moment
	private PhotonPlayer itemHolder = null;
	
	public enum MatchState {
		SETTINGUP, // lobby
		COUNTDOWN, // countdown is running
		RUNNING, // match is running
		FINISHED // match is over
	}
	
	void Start() {
		if(!arenaLoaded) {
			Application.LoadLevelAdditive(arenaScene);
			arenaLoaded = true;
			allReady = true;
		}	
	}
	
	/**
	 * Each frame, update gameTime, countdown or wait for start - depending on match state
	 */ 
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
				// init countdown if start is requested
				if(startRequest && allReady && PhotonNetwork.isMasterClient) {
					photonView.RPC("InitCountdown",PhotonTargets.AllBuffered, PhotonNetwork.time);
					startRequest = false;
				}
				break;
			case MatchState.COUNTDOWN:
				// update countdown
				int currentStep = Mathf.FloorToInt( (float) (countdownTime + countdownLength + 1 - PhotonNetwork.time) );
				if( currentStep > 0)
					countdownLabel.text = currentStep.ToString();
				if( currentStep == 0 )
					countdownLabel.text = "go";
				// start match after countdown has finished
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
	
	/**
	 * Reset match states
	 */ 
	public void Reset() {
		ReloadPlayerList();
		
		currentState = MatchState.SETTINGUP;
		startRequest = false;
		gameTime = matchLength;
		UpdateLobbyUI();
	}
	
	/**
	 * Initalize gameTime and player scores.
	 */ 
	public void Init() {
		gameTime = matchLength;
		foreach( RocketFightPlayer rfp in playerList ) {
			rfp.score = 0;	
		}
	}
	
	/**
	 * Start countdown and update score board.
	 * Start spawning if we are master client.
	 */ 
	[RPC]
	public void InitCountdown(double timestamp) {
		countdownTime = timestamp;
		currentState = MatchState.COUNTDOWN;
		countdownLabel.gameObject.SetActive(true);
		countdownSound.Play();
		
		if( scoreBoard == null )
			scoreBoard = GameObject.Find("ScorePanel").GetComponent<ScorePanel>();
		
		scoreBoard.gameObject.SetActive( true );
		scoreBoard.UpdateDisplay();
		
		if( PhotonNetwork.isMasterClient )
			OrganizeSpawning();
	}
	
	/**
	 * Add new player to the player list.
	 */ 
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
		UpdateLobbyUI();
	}
	
	/**
	 * Check if the match is already running.
	 */ 
	public bool IsRunning() {
		return (currentState == MatchState.RUNNING);	
	}
	
	/**
	 * Get the time left to play.
	 */ 
	public float GetGameTime() {
		return gameTime;	
	}
	
	/**
	 * Get list of player with all parameter like score and color.
	 */ 
	public List<RocketFightPlayer> GetPlayerList() {
		return playerList;	
	}
	
	/**
	 * Remove an player from the player list.
	 */ 
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
				UpdateLobbyUI();
				return;
			}
		}
	}
	
	/**
	 * Clear player list an recreate it from the player in this room.
	 */ 
	[RPC]
	public void ReloadPlayerList() {
		// request to kick each player without name
		foreach( PhotonPlayer player in PhotonNetwork.playerList ) {
			if( player.name == "")
				PhotonNetwork.CloseConnection(player);
		}
		
		// reset colors
		for( int i=usedColors.Count-1; i>=0; i-- ) {
			freeColors.Add( usedColors[i] );	
		}
		usedColors.Clear();
		
		// reload player list
		playerList.Clear();
		List<PhotonPlayer> sortedList = SortPlayerListByID( new List<PhotonPlayer>( PhotonNetwork.playerList ) );
		
		foreach( PhotonPlayer player in sortedList ) {
			if( player.name != "")
				AddPlayer( player );	
		}
	}
	
	/**
	 * Deactivate all player slots in the lobby and re-activate one for each player in the playerlist
	 */ 
	public void UpdateLobbyUI() {
		// deactivate all slots in lobby
		foreach( UIPlayerLobbySlot slot in uiPlayerLobbySlots ) {
			slot.Deactivate();	
		}
		
		// set fill one slot for each player in playerlist with data
		for( int i=0; i<playerList.Count; i++ ) {
			uiPlayerLobbySlots[i].Set( 
				playerList[i].photonPlayer.name, 
				(PhotonNetwork.masterClient == playerList[i].photonPlayer),
				(PhotonNetwork.player == playerList[i].photonPlayer));
		}
		
	}
	
	/**
	 * Tell's you a given player has finished loading the arena
	 */ 
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
	
	/**
	 * Request match start. But wait for all to load level and then start count down
	 */ 
	[RPC]
	public void RequestStart() {
		// load arena scene additive
		if(!arenaLoaded) {
			Application.LoadLevelAdditive(arenaScene);
			arenaLoaded = true;
		}
		startRequest = true;
	}
	
	/**
	 * Start the match.
	 */ 
	[RPC]
	public void StartMatch(double timestamp) {
		Debug.Log("Match started!");
		currentState = MatchState.RUNNING;
		startTime = timestamp;
		Init();
		// play background music
		matchMusic.Play();
		ambientWind.Play ();
		countdownLabel.gameObject.SetActive( false );
		
		GameObject[] characters = GameObject.FindGameObjectsWithTag("Player");
		foreach( GameObject go in characters ) {
			if( go.GetPhotonView().owner == PhotonNetwork.player )
				go.GetComponent<InputManager>().enabled = true;
		}
	}
	
	/**
	 * End the current match and destroy player instances.
	 */ 
	private void GameOver() {
		currentState = MatchState.FINISHED;
		if(PhotonNetwork.isMasterClient) {
			foreach( RocketFightPlayer rfp in playerList ) {
				PhotonNetwork.DestroyPlayerObjects(rfp.photonPlayer);	
			}
		}
		uiMenu.ChanceState(UIMenu.UIState.MATCHOVER);
		
		playerList = SortPlayerListByScore( playerList );
		// stop background music
//		matchMusic.Stop();
		ambientWind.Stop();
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
		handle.GetPhotonView().RPC("SetPlayer",PhotonTargets.AllBuffered, PhotonNetwork.player);
		handle.GetPhotonView().RPC("SetColor",PhotonTargets.AllBuffered,rgb);
	}
	
	/**
	 * Create spawnpoint at an given position and set it's properties according to it's owner
	 * and spawn player character afterwards.
	 */ 
	[RPC]
	public void SetSpawnPoint( Vector3 pos, PhotonPlayer player, Vector3 rgb ) {
		GameObject handle = PhotonNetwork.Instantiate(spawnPointPrefab.name,pos, Quaternion.identity,0);
		handle.GetPhotonView().RPC("AssignTo",PhotonTargets.AllBuffered,player);
		handle.GetPhotonView().RPC("SetColor",PhotonTargets.AllBuffered,rgb);
		
		// spawn player & set color
		photonView.RPC("SpawnPlayer",player,handle.transform.position, rgb);
		// init score
		photonView.RPC("SetScore",PhotonTargets.AllBuffered, player, 0);
	}
	
	/**
	 * Only master client ist allwoed to do this! Tell all clients where to spawn there respawnpoints and player character.
	 */ 
	public void OrganizeSpawning() {
		if( !PhotonNetwork.isMasterClient ) 
			return;
		
		for( int i=0; i<playerList.Count; i++ ) {
			// instatiate spawn points
			Vector3 rgb = new Vector3( playerList[i].color.r, playerList[i].color.g,playerList[i].color.b );
			photonView.RPC("SetSpawnPoint",playerList[i].photonPlayer,positions[i],playerList[i].photonPlayer,rgb);	
		}
		// spawn item
		PhotonNetwork.Instantiate(itemPrefab.name,new Vector3(0,0.5f,0),Quaternion.identity,0);
	}
	
	/**
	 * Increase the score of a given player by a given value.
	 * Will spawn a score notification above the player character.
	 */ 
	[RPC]
	public void IncreaseScore(int playerID, int val) {
		// increase score value
		foreach( RocketFightPlayer rfp in playerList ) {
			if( rfp.photonPlayer.ID == playerID ) {
				// apply multiplier effect by item if the playertarget is the item holder
				if( rfp.photonPlayer == itemHolder ) 
					val *= 2;
				
				// increase score in playerlist
				rfp.score += val;
				
				// display score increase over character if its my char
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
		// update scoreboard
		scoreBoard.UpdateScore();
	}
	
	/**
	 * Set score of an player to a given value.
	 * There will be no notifcation.
	 */ 
	[RPC]
	public void SetScore(PhotonPlayer target, int val) {
		foreach( RocketFightPlayer rfp in playerList ) {
			if( rfp.photonPlayer == target ) {
				rfp.score = val;
				return;
			}
		}
		// update scoreboard
		scoreBoard.UpdateScore();
	}
	
	/**
	 * Get list of colors which are assigned to player in this match.
	 */ 
	public List<Color> GetUsedColors() {
		return usedColors;	
	}
	
	/**
	 * Sort a copy of the given playerlist by ID with QuickSort.
	 */ 
	private List<PhotonPlayer> SortPlayerListByID( List<PhotonPlayer> unsortedPlayerList ) {
		QuickSortPlayerID( ref unsortedPlayerList, 0, unsortedPlayerList.Count - 1);
		return unsortedPlayerList;
	}
	
	/**
	 * Recursivly walk through the list and split it into right end left part by an
	 * calculated pivot element.
	 */ 
	private void QuickSortPlayerID( ref List<PhotonPlayer> list, int left, int right) {
		if( left < right ) {
			int divisior = QuickSortDivideID(ref list, left, right);
			QuickSortPlayerID(ref list, left, divisior -1);
			QuickSortPlayerID(ref list, divisior + 1, right);
		}
	}
	
	/**
	 * Select the last element as pivot element and sort it that way,
	 * that all elemts on the left of pivot are smaller and all
	 * elements on the right are bigger then pivot.
	 * Return position of the pivot element
	 */
	private int QuickSortDivideID( ref List<PhotonPlayer> list, int left, int right ) {
		int i = left;
        // start with j left beside pivot element
        int j = right - 1;
        int pivot = list[right].ID;

        do {
            // search element greater then pivot starting from left
            while (list[i].ID <= pivot && i < right) 
                    i++;

            // seach element smaller then pivot starting from right
            while (list[j].ID >= pivot && j > left) 
                    j--;

            if (i < j) {
					// swap list[i] and list[j]
                    PhotonPlayer z = list[i];
                    list[i] = list[j];
                    list[j] = z;
            }

        } while (i < j);
        // as long as i < j

		// swap pivot element (list[right]) with new final positoin (list[i])
        if (list[i].ID > pivot) {
                PhotonPlayer z = list[i];
                list[i] = list[right];
                list[right] = z;
        }
        return i; // return position of pivot element
	}
	
	/**
	 * Sort a copy of the given playerlist by Score with QuickSort.
	 */ 
	private List<RocketFightPlayer> SortPlayerListByScore( List<RocketFightPlayer> unsortedPlayerList ) {
		QuickSortPlayerScore( ref unsortedPlayerList, 0, unsortedPlayerList.Count - 1);
		return unsortedPlayerList;
	}
	
	/**
	 * Recursivly walk through the list and split it into right end left part by an
	 * calculated pivot element.
	 */ 
	private void QuickSortPlayerScore( ref List<RocketFightPlayer> list, int left, int right) {
		if( left < right ) {
			int divisior = QuickSortDivideScore(ref list, left, right);
			QuickSortPlayerScore(ref list, left, divisior -1);
			QuickSortPlayerScore(ref list, divisior + 1, right);
		}
	}
	
	/**
	 * Select the last element as pivot element and sort it that way,
	 * that all elemts on the left of pivot are smaller and all
	 * elements on the right are bigger then pivot.
	 * Return position of the pivot element
	 */
	private int QuickSortDivideScore( ref List<RocketFightPlayer> list, int left, int right ) {
		int i = left;
        // start with j left beside pivot element
        int j = right - 1;
        int pivot = list[right].score;

        do {
                // search element greater then pivot starting from left
                while (list[i].score >= pivot && i < right) 
                        i++;

                // seach element smaller then pivot starting from right
                while (list[j].score <= pivot && j > left) 
                        j--;

                if (i < j) {
						// swap list[i] and list[j]
                        RocketFightPlayer z = list[i];
                        list[i] = list[j];
                        list[j] = z;
                }

        } while (i < j); // as long as i < j
		
		// swap pivot element (list[right]) with new final positoin (list[i])
        if (list[i].score < pivot) {
                RocketFightPlayer z = list[i];
                list[i] = list[right];
                list[right] = z;
        }
        return i; // return position of pivot element
	}
	
	/**
	 * Set who is holding the item to enable score multiplier.
	 */
	[RPC]
	public void SetItemHolder( PhotonPlayer player ) {
		Debug.Log("New item holder: " + player.name + "[" + player.ID + "]" );
		itemHolder = player;
	}
	
	/**
	 * Clear item holde, so nobody get the score multiplier.
	 */
	[RPC]
	public void ClearItem() {
		itemHolder = null;	
	}
}
