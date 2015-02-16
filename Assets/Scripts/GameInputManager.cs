using UnityEngine;
using System.Collections;

public class GameInputManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log ("Start Input Manger");
	
	}

	//

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Debug.Log ("InputMgr begin raycast");
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
				if (hit.collider != null){
					Debug.Log("Hit " + hit.collider.name);
					hit.collider.SendMessage("Picked",0,SendMessageOptions.DontRequireReceiver);
					}
					//hit.collider.enabled = false;
				}
			
			
		}
	}



