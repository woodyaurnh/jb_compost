using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System.Text.RegularExpressions;



public class GMatrix : MonoBehaviour {

	public Transform spot;

	public int matrixSize = 4;

	private ParseObject[] _ppArray;

	// replace this with a 3D array later
	private Transform[] _gArray;
	private Transform testXform;
	private bool isStarted = false;

	private IEnumerable<ParseObject> results;


	// Use this for initialization
	void Start () {
		
		GameManager.Notifications.AddListener (this, "OnStateChange");
		GameManager.Notifications.AddListener (this, "OnPlantsLoaded");
		GameManager.Notifications.AddListener (this, "OnSpotPicked");
		_ppArray = new ParseObject[matrixSize * matrixSize];
	}


	// Use this for initialization
	void BuildMatrix () {
		_gArray = new Transform[matrixSize * matrixSize];
		Debug.Log ("_gArray Length: " + _gArray.Length);
		GameObject matrixParent = new GameObject("matrixParent");
		StartCoroutine(LoadMatrix ());
		Debug.Log ("Test Matrix");
		int aIndex = 0;
		for (int i = 0;i < matrixSize; i++){
			for(int j = 0; j < matrixSize; j++){

				if(_gArray[aIndex] == null){
					//Debug.Log ("Melement : " + i + "   " + j + " is null");
					_gArray[aIndex] = Instantiate(spot,new Vector3(i,0,j),Quaternion.identity) as Transform;
					Transform tt = _gArray[aIndex];
					//Debug.Log (">>> " + _gArray[aIndex].position);
					tt.name = "Spot_" + i + "_" + j;
					GSpot gs = tt.GetComponent<GSpot>();
					gs.spotX = i;
					gs.spotZ = j;
					//tt.GetChild(0).transform.rotation = Quaternion.Euler(0, Random.Range(0.0F, 360.0F), 0);
					tt.gameObject.SetActive(true);
					tt.transform.parent = matrixParent.transform;

				}
				else{
				Debug.Log ("Melement : " + i + "   " + j + "   " + _gArray[aIndex]);

				}
				aIndex++;
			}
		}

		float scale = 4.0f / matrixSize;
		float offset = ((matrixSize / 2) - 0.5F) * scale;
		//Debug.Log("offset: " + offset + " scale: " + scale);
		matrixParent.transform.position = new Vector3(-offset,0,-offset);
		matrixParent.transform.localScale = new Vector3(scale,scale,scale);


	}


	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator LoadMatrix(){
		// dummy. should load plants 
		// and have hooks for progress bar
		Debug.Log("Load Matrix");
		yield return new WaitForSeconds(0.2f);
	}

	public void OnStateChange(Component Sender)
	{
		//Debug.Log("Login Panel OnStateChange: " + GameManager.Instance.gameState);
		//Debug.Log("state: " + GameManager.Instance.gameState);
		if (GameManager.Instance.gameState == GameManager.GameState.MainMenu){
			Debug.Log ("GMatrix OnStateChange: " + GameManager.Instance.gameState);
			BuildMatrix ();

		}
	}

	public void OnPlantsLoaded(Component Sender)
	{
		Debug.Log ("GMatrix OnPlantsLoaded: " + GameManager.Instance.gameState);
		FillStoredPlants();
	}
	
	public void OnSpotPicked(Component Sender)
	{
		Debug.Log ("GMatrix OnSpotPicked: " + GameManager.Instance.gameState);
		BlankSpots();
	}
	

	private void FillStoredPlants(){
		Debug.Log ("GMatrix: FillStoredPlants called");
		foreach (var pair in GameManager.UserMgr.ppDict){
			ParseObject po = (ParseObject)pair.Value;
			string gridKey = (string)pair.Key;
			int pIdx = po.Get<int>("plantIdx");
			int sunshine = po.Get<int>("sunshine");
			float yRot = po.Get<float>("yRot");
			int ppx = po.Get<int>("x");
			int ppz = po.Get<int>("z");
			int curStage = po.Get<int>("curStage");
			bool mature = po.Get<bool>("mature");

			var numbers = Regex.Split(gridKey, @"\D+");
			int px = int.Parse (numbers[0]);
			int pz = int.Parse (numbers[1]);
			Debug.Log ("FillStoredPlants: getting plant at " + px + "," + pz);
			Transform tt = _gArray[(pz % matrixSize ) + (px * matrixSize)];
			GSpot gs = tt.GetComponent<GSpot>();
			if (gs != null){
				Debug.Log ("addplant!!!");
				gs.AddStoredPlant(po, 0.0F);
				gs.spotFilled = true;
			}

		}
		Debug.Log ("GMatrix: FillStoredPlants END");
	}


	public void SaveAllPlants(){
		for(int i = 0; i < _gArray.Length; i++){
		
			GSpot gs = _gArray[i].GetComponent<GSpot>();
			if(gs.plant != null){
				Plant p = gs.plant.GetComponent<Plant>();
				p.StorePlant();
			}

		}
	}


	/*
	private void GetStoredPlants(){
		Debug.Log ("GMatrix: GetStoredPlants called");
		var query = ParseObject.GetQuery("Plant")
			.WhereEqualTo("owner", ParseUser.CurrentUser);
		query.FindAsync().ContinueWith(t =>
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
					int aindex = (pz%matrixSize) + px*matrixSize;
					_ppArray[aindex] = (ParseObject)obj;
					var idx = obj.Get<int>("plantIdx");
					var mature = obj.Get<bool>("mature");
					var yRot = obj.Get<float>("yRot");
					string matStr = " is not ";
					if(mature)
						matStr = " is ";
					Debug.Log("Plant: " + idx + " at " + px + "," + pz + matStr + "mature");
					// convert x and z to single dim array index

					// check name against 2D array position
					//Transform tt = _gArray[aindex];

					// can't call GetComponent out of main thread! 
					//GSpot gs = tt.GetComponent<GSpot>();
					//gs.AddPlant (idx,yRot,0.0F);

				}
			}
			
		});
		
	}

*/

	void TestIndex(int x, int z){
		int aindex = (z%matrixSize) + x*matrixSize;
		// check name against 2D array position
		Transform gs = _gArray[aindex];
		Debug.Log (gs.name + " at " + x + "," + z + " Index: " + aindex);

	}

	void BlankSpots(){
		foreach (Transform spot in _gArray)
			spot.renderer.enabled = false;
	}
}
