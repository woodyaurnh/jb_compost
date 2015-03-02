using UnityEngine;
using System;
using System.Collections;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;




public class UserManager : MonoBehaviour {

	// public vars
	public bool isAuthenticated = false;

	public int startingSunshine = 50;

	// Dictionary of Parse Plants for lookup
	public Dictionary<string, ParseObject> ppDict;
	


	// private vars
	private string _userName = string.Empty;
	private int _sunshine = 0;
	private string _userID = string.Empty;
	

	public void SetCurrentUser(string name){
		PlayerPrefs.SetString("playerName", name);
	}

	public string GetCurrentUser(){
		return _userName;
	}

	public void SetSunshine(int sunshine){
		PlayerPrefs.SetInt("sunshineAmt", sunshine);
	}
	
	public int GetSunshine(){
		//return PlayerPrefs.GetInt("sunshineAmt");
		if(ParseUser.CurrentUser != null){
		return (int)(ParseUser.CurrentUser.Get<int>("sunshine"));
		}else
			return startingSunshine;
	}
	

	public void Start(){
		ppDict = new Dictionary<string,ParseObject >();
	}


	public void CreateNewUser(string userName, string password)
		{
			var user = new ParseUser()
			{
				Username = userName,
				Password = password
			};


			user.SignUpAsync();
			_userName = userName;
		}

	public bool AuthenticateUser(string username, string password){

		LogOut();
		ParseUser.LogInAsync(username, password).ContinueWith(t =>
		                                                      {
			Debug.Log ("AuthenicateUser");
			if (t.IsFaulted || t.IsCanceled)
			{
				// The login failed. Check t.Exception to see why.
				isAuthenticated = false;
				Debug.Log ("AuthenicateUser: failed");
				return false;
			}
			else
			{
				// Login was successful.
				isAuthenticated = true;
				Debug.Log ("AuthenicateUser: Succeeded");
				_userName = username;
				Debug.Log ("GetCurrentUser: " + GetCurrentUser() );
				return true;
			}
		});
		// ?????
		return false;
	}


	public void FakeLogin(){
		// to use when we want to test game flow but do not have an internet connection for login
		_userName = "FakeLogin";
		GameManager.Instance.SetGameState(GameManager.GameState.MainMenu);
	}


	public IEnumerator Login(string username,string passwd) {
		var loginTask = ParseUser.LogInAsync(username, passwd).ContinueWith(t =>
	                                                      {
			if (t.IsFaulted || t.IsCanceled)
			{
				// The login failed. Check the error to see why.
				Debug.Log ("Login failed: Name- " + username);
				isAuthenticated = false;

			}
			else
			{
				Debug.Log ("Login succeeded: Name- " + username);
				// Login was successful.
				isAuthenticated = true;
				if((UserExists()) ){
					Debug.Log ("User Exists test");
				}


			}
		});
		while(!loginTask.IsCompleted) yield return null;
		if(isAuthenticated){
			_userName = username;

			// get the stored plants ....
			StartCoroutine(GetStoredPlants());

			GameManager.Instance.SetGameState(GameManager.GameState.MainMenu);
		}
	}

	public void SignIn(string username, string passwd){
		Debug.Log ("SignIn called");
		CreateNewUser(username,passwd);
		if((UserExists()) ){
			GameManager.Instance.SetGameState(GameManager.GameState.MainMenu);
		}

	}

	public IEnumerator SignInCoR(string username, string passwd){
		Debug.Log ("SignInCoR  username: " + username + " passwd: " + passwd);
		var user = new ParseUser()
		{
			Username = username,
			Password = passwd
		};


		var signInTask = user.SignUpAsync().ContinueWith(t =>
		                                                 {
			if (t.IsFaulted || t.IsCanceled)
			{
				// The login failed. Check the error to see why.
				Debug.Log ("SignIn failed: Name- " + username);
			}
			else
			{
				Debug.Log ("SigIn succeeded: Name- " + username);
			}
		});
		Debug.Log ("Before wait");
		while (!signInTask.IsCompleted) yield return null;
		Debug.Log ("After wait");

		// add any user fields
		user["sunshine"] = startingSunshine;


		var modTask = user.SaveAsync();
		while (!modTask.IsCompleted) yield return null;
		Debug.Log ("After sunshine added");

		

		_userName = username;
		if((UserExists()) ){
			Debug.Log ("User Exists test");
			GameManager.Instance.SetGameState(GameManager.GameState.MainMenu);
		}
		

	}


	public void LogOut(){
		ParseUser.LogOut();
		GameManager.Instance.SetGameState(GameManager.GameState.Login);
		_userName = string.Empty;

	}


	public bool UserExists(){
		if (ParseUser.CurrentUser != null)
			return true;
		else
			return false;
	}

	public ParseObject StorePlant(int plantIdx, float x, float z, float yRot)
	{
		Debug.Log ("UsrMgr: StorePlant called");

		bool mature = false;
		int sunshine = 0;
		int curStage = -1;

		ParseObject pPlant =  new ParseObject("Plant");
		pPlant["owner"] = ParseUser.CurrentUser;
		//pPlant["owner"] = ParseUser.CurrentUser["username"];
		//TODO: following should be ref to Parse Spot
		//pPlant["spot"] = GameManager.Instance.currentSpot;
		pPlant["plantIdx"] = plantIdx;
		pPlant["x"] = x;
		pPlant["z"] = z;
		pPlant["yRot"] = yRot;
		pPlant["mature"] = mature;
		pPlant["sunshine"] = sunshine;
		pPlant["curStage"] = curStage;

		Task saveTask = pPlant.SaveAsync();
		return pPlant;

	}

	public ParseObject StoreGSpot(Transform gsXform)
	{
		Debug.Log ("UsrMgr: StoreGSpot called");
		if(gsXform == null)
		{
			Debug.LogError("UsrMgr:StoreGSpot: Transform arg is null");
			return null;
		}
		GSpot gs = gsXform.GetComponent<GSpot>();
		if (gs == null)
		{
			Debug.LogError("UsrMgr:StoreGSpot: Transform arg has no GSpot component");
			return null;
		}
		
		ParseObject pGSpot =  new ParseObject("GSpot");
		pGSpot["owner"] = ParseUser.CurrentUser;
		pGSpot["index"] = gs.listIndex;
		pGSpot["posX"] = gsXform.position.x;
		pGSpot["posY"] = gsXform.position.y;
		pGSpot["posZ"] = gsXform.position.z;
		pGSpot["meshKey"] = gs.meshKey;

		Task saveTask = pGSpot.SaveAsync();
		return pGSpot;
		
	}


	private IEnumerator GetStoredPlants(){
		Debug.Log ("UserManager: GetStoredPlants called");
		var query = ParseObject.GetQuery("Plant")
			.WhereEqualTo("owner", ParseUser.CurrentUser);
		var queryTask = query.FindAsync().ContinueWith(t =>
		                               {
			if (t.IsFaulted)
			{
				Debug.Log ("Parse Error");
			}
			else
			{
				IEnumerable<ParseObject> results = t.Result;
				foreach (var obj in results)
				{
					var px = obj.Get<int>("x");
					var pz = obj.Get<int>("z");
					string plantKey = px + "_" + pz;
					Debug.Log ("Got pPlant at " + plantKey);
					ppDict.Add(plantKey,(ParseObject)obj);

				}
			}
			
		});
		while(!queryTask.IsCompleted) yield return null;

		GameManager.Notifications.PostNotification(this,"OnPlantsLoaded");



	}

	private IEnumerator GetStoredGSpots(){
		Debug.Log ("UserManager: GetStoredGSpots called");
		var query = ParseObject.GetQuery("GSpot")
			.WhereEqualTo("owner", ParseUser.CurrentUser);
		var queryTask = query.FindAsync().ContinueWith(t =>
		                                               {
			if (t.IsFaulted)
			{
				Debug.Log ("Parse Error");
			}
			else
			{
				IEnumerable<ParseObject> results = t.Result;
				foreach (var obj in results)
				{
					var px = obj.Get<int>("x");
					var pz = obj.Get<int>("z");
					string plantKey = px + "_" + pz;
					Debug.Log ("Got pPlant at " + plantKey);
					ppDict.Add(plantKey,(ParseObject)obj);
					
				}
			}
			
		});
		while(!queryTask.IsCompleted) yield return null;
		
		GameManager.Notifications.PostNotification(this,"OnPlantsLoaded");
		
		
		
	}
	

}
