using UnityEngine;
using System.Collections;

public class Poster : MonoBehaviour {

	public NotificationsManager Notifications = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	// check for keyboard input
	if(Input.anyKeyDown && Notifications != null){
			Notifications.PostNotification(this, "OnKeyboardInput");
			//Debug.Log ("Key is down");
		}
	
	}
}
