using UnityEngine;
using System.Collections;

public class ButtonManager : MonoBehaviour {

	public GameObject plantButtons;
	public GameObject actionButtons;

	// Use this for initialization
	void Start () {
		GameManager.Notifications.AddListener (this, "OnSpotPicked");
		GameManager.Notifications.AddListener (this, "OnPlantPlanted");
		GameManager.Notifications.AddListener (this, "OnPlantGathered");

	}

	public void OnSpotPicked(Component sender){
		GSpot gs = (GSpot)sender;
		//float gx = gs.spotX;
		//float gz = gs.spotZ;
		Debug.Log ("ButtonMgr: Spot at " + gs.transform.position + " picked");
		NGUITools.SetActive(plantButtons, false);
		NGUITools.SetActive(actionButtons, false);
		if(!gs.spotFilled){
			NGUITools.SetActive(plantButtons, true);
			//plantButtons.SetActive(true);
		}else{
			NGUITools.SetActive(actionButtons, true);
		}
	}

	public void OnPlantPlanted(Component sender){
		NGUITools.SetActive(plantButtons, false);
		//plantButtons.SetActive(false);
	}

	public void OnPlantGathered(Component sender){
		NGUITools.SetActive(actionButtons, false);
		//plantButtons.SetActive(false);
	}

}
