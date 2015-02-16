using UnityEngine;
using System.Collections;

public class spotButton : MonoBehaviour {

	public void OnClick(){
		Debug.Log ("Spot Button Pressed " + gameObject.name);
		GameManager.Notifications.PostNotification(this,"SpotButtonPressed");

	}

}
