using UnityEngine;
using System.Collections;

public class plantButton : MonoBehaviour {

	public int plantID = 0;

	public void OnClick(){
		Debug.Log ("Clicked " + gameObject.name);
		GameManager.Notifications.PostNotification(this,"OnPlantChosen");
		if(GameManager.Instance.currentSpot == null){
			Debug.Log ("plantButton:OnClick:Error! Current spot is null");
		}
		else{
			GameManager.Instance.currentSpot.AddNewPlant();
		}

	}

}
