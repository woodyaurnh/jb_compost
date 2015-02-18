//GameManager
//Singleton and persistent object to manage game state
//For high level control over game
//--------------------------------------------------------------
using UnityEngine;
using System.Collections;
using Parse;

//Game Manager requires other manager components
[RequireComponent (typeof (NotificationsManager))] //Component for sending and receiving notifications
[RequireComponent (typeof (UserManager))] //Component validating Logins
[RequireComponent (typeof (GamePanel))] //Component validating Logins

//--------------------------------------------------------------
public class GameManager : MonoBehaviour
{
	//--------------------------------------------------------------
	//public properties

	// game states
	public enum GameState { NullState, Login, SignIn, MainMenu, Game, DEBUG };

	public GameState gameState;

	public GamePanel gameUI = null;

	public int sunshineIncValue = 1;
	public float sunshineTimeInc = 0.25f;

	public GMatrixDeform gmatrix;

	public int plantButtonIndex = -1;

	public GSpot currentSpot;
	// private 
	private int _sunshineAmt = 0;




	//C# property to retrieve currently active instance of object, if any
	public static GameManager Instance
	{
		get
		{
			if (instance == null) instance = new GameObject ("GameManager").AddComponent<GameManager>(); //create game manager object if required
			return instance;
		}
	}
	//--------------------------------------------------------------
	//C# property to retrieve notifications manager
	public static NotificationsManager Notifications
	{
		get
		{
			if(notifications == null) notifications =  instance.GetComponent<NotificationsManager>();
			return notifications;
		}
	}
	//--------------------------------------------------------------
	//C# property to retrieve Login manager mightturn this to Connection manager
	public static UserManager UserMgr
	{
		get
		{
			if(userMgr == null) userMgr =  instance.GetComponent<UserManager>();
			return userMgr;
		}
	}
	//--------------------------------------------------------------
	//Private variables
	//--------------------------------------------------------------
	//Internal reference to single active instance of object - for singleton behaviour
	private static GameManager instance = null;

	//Internal reference to notifications object
	private static NotificationsManager notifications = null;
	
	//Internal reference to notifications object
	private static UserManager userMgr = null;


	
	//public variables
	//--------------------------------------------------------------
	// Called before Start on object creation
	void Awake ()
	{	
		//Check if there is an existing instance of this object
		if((instance) && (instance.GetInstanceID() != GetInstanceID()))
			DestroyImmediate(gameObject); //Delete duplicate
		else
		{
			instance = this; //Make this object the only instance
			DontDestroyOnLoad (gameObject); //Set as do not destroy
		}

	}


	// Game State 
	public void SetGameState(GameState newgameState) {
		gameState = newgameState;
		Notifications.PostNotification(this, "OnStateChange");
	}



	public void OnStateChange(){
		Debug.Log("State changed to " + gameState);
		if (gameState == GameState.Game) {
			//StartCoroutine(IncrementSunshine(sunshineTimeInc));
		}
	}

	public void OnPlantChosen(Component sender){
		Debug.Log ("Plant Chosen!  >" + sender);
		plantButton pb = (plantButton)sender;
		plantButtonIndex = pb.plantID;
		Debug.Log ("Plant Chosen!  >" + sender + " with index: " + plantButtonIndex);
	}

	public void	OnPlantMatured(Component sender){
		Debug.Log (sender.name + " just matured!");
	}


	public void OnPlantPlanted(Component sender){
		Debug.Log ("Plant Planted!  ");
		//GSpot gs = (GSpot)sender;
		//UserMgr.StorePlant(plantButtonIndex,gs.spotX,gs.spotZ);

		
	}

	public void OnSpotPicked(Component sender){
		if(sender == null){
			Debug.Log("GameMgr:OnSpotPicked:Error! Picked spot is null!");
		}
		else{
		currentSpot = (GSpot)sender;
		}

	}
	
	public void OnGatherPlant(){

		Debug.Log ("GameManager:GatherPlant");
		 _sunshineAmt =  _sunshineAmt + currentSpot.GatherPlant();
		gameUI.setSunshine(_sunshineAmt.ToString());
		ParseUser.CurrentUser["sunshine"] = _sunshineAmt;
		ParseUser.CurrentUser.SaveAsync();
	}



	//--------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		Debug.Log ("GM Start");
		if(Notifications != null){
			Notifications.AddListener (this, "OnStateChange");
			Notifications.AddListener (this, "OnPlantChosen");
			Notifications.AddListener (this, "OnPlantPlanted");
			Notifications.AddListener (this, "OnPlantMatured");
			Notifications.AddListener (this, "OnSpotPicked");
		}

		if (UserMgr.GetCurrentUser() != null && UserMgr.GetCurrentUser() != ""){
			SetGameState(GameState.MainMenu);
		}
		else{
			SetGameState (GameState.Login);
		}
		// get initial user data
		_sunshineAmt = userMgr.GetSunshine();
		Debug.Log ("starting sunshine is " + _sunshineAmt);
		// set up UI
		if(gameUI != null)
			{
			gameUI.setSunshine(_sunshineAmt.ToString());
			}	
		else
			{
			Debug.Log ("No Game UI");
			}

		//StartCoroutine(IncrementSunshine(0.5f));

	}
	//--------------------------------------------------------------
	//Restart Game
	public void RestartGame()
	{
		//Load first level

	}
	//--------------------------------------------------------------
	//Exit Game
	public void ExitGame()
	{
		Debug.Log("Exit Game");
		// save the plants
		gmatrix.SaveAllPlants();
		userMgr.SetSunshine(_sunshineAmt);
		// for editor only
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		// quit doesn't work in Editor
		Application.Quit();
	}
	//--------------------------------------------------------------

	IEnumerator IncrementSunshine(float sunTickIncrement){
		while(true){
			yield return new WaitForSeconds(sunTickIncrement);
			_sunshineAmt += sunshineIncValue;
			gameUI.setSunshine(_sunshineAmt.ToString());

		}

	}

/*	void OnApplicationPause(bool paused) {
		if (paused) ExitGame ();
		else{
		}

	}
*/

	// --------------------------------------
	//
	

}
