using UnityEngine;
using System.Collections;
using Parse;

public class GSpot : MonoBehaviour {

	public Transform plant;
	public Transform[] plantArray;
	public bool spotFilled = false;

	//public float spotX,spotZ = 0.0F;
	public int meshKey;
	public int listIndex;





	// private vars
	private Plant myPlant;

	// Use this for initialization
	void Start () {
		//GameManager.Notifications.AddListener(this,"OnPlantChosen");
	// cache plant component

	}
	
	// Update is called once per frame

	void Update () {
	
	}

	public void Picked()
	{
		Debug.Log("I was picked");
		GameManager.Notifications.PostNotification(this,"OnSpotPicked");
		renderer.enabled = true;

	}


	// factored from above put it in Picked
	// so that it can also be used to build array from parse data
	public void  AddNewPlant(){
		int plantIdx = GameManager.Instance.plantButtonIndex;
		if(plantIdx >= 0 & plantIdx < plantArray.Length){
			float randRot = Random.Range(0.0F, 360.0F);
			Quaternion qr = Quaternion.Euler(0.0F, randRot,0.0F);
	
			plant = Instantiate(plantArray[plantIdx],transform.position,qr) as Transform;
			// cheat to fix 2x scale 
			//TODO: WTF is up with this scale transform and why
			//plant.transform.localScale = new Vector3(plant.transform.localScale.x  * .5F,plant.transform.localScale.y  * .5F,plant.transform.localScale.z  * .5F);
			plant.parent = this.transform;
			myPlant = plant.GetComponent<Plant>();
			myPlant.plantIdx = plantIdx;
			//myPlant.pPlant = GameManager.UserMgr.StorePlant(plantIdx,spotX,spotZ,randRot);
			myPlant.DoGrow (0.0F);
			spotFilled = true;
			GameManager.Notifications.PostNotification(this,"OnPlantPlanted");

		}
	
		
	}

	public void AddStoredPlant(ParseObject po, float startTime){
		Debug.Log ("called GSpot: AddStoredPlant");
		int pIdx = po.Get<int>("plantIdx");
		int sunshine = po.Get<int>("sunshine");
		float yRot = po.Get<float>("yRot");
		int px = po.Get<int>("x");
		int pz = po.Get<int>("z");
		Debug.Log ("FillStoredPlants: getting plant at " + px + "," + pz + "w plant Index " + pIdx);

		Quaternion rot = Quaternion.Euler(0.0F, yRot,0.0F);
		plant = Instantiate(plantArray[pIdx],transform.position,rot) as Transform;
		// cheat to fix 2x scale 
		//plant.transform.localScale = new Vector3(plant.transform.localScale.x  * .5F,plant.transform.localScale.y  * .5F,plant.transform.localScale.z  * .5F);
		plant.parent = this.transform;
		myPlant = plant.GetComponent<Plant>();
		myPlant.plantIdx = pIdx;
		myPlant.pPlant = po;
		//myPlant.DoGrow (startTime);
		myPlant.InitFromStored();


	}

	public int GatherPlant(){
		int tmpSunshine = 0;
		Debug.Log ("GSpot:GatherPlant");
		// get sunshine,
		//remove plant from parse and from spot itself
		//and return sunshine
		if(myPlant != null){
			tmpSunshine = myPlant.sunshine;
			if (myPlant.pPlant != null){
				myPlant.pPlant.DeleteAsync();
			}
			myPlant = null;
			Object.Destroy(plant.gameObject, 0.0F);
			spotFilled = false;
			GameManager.Notifications.PostNotification(this,"OnPlantGathered");
		}

		return tmpSunshine;
	}


}
