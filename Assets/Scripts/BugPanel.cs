using UnityEngine;
using System.Collections;

public class BugPanel : MonoBehaviour {
	private UIPanel thisPanel;
	
	// Use this for initialization
	void Start () {

		thisPanel = GetComponent<UIPanel>();
		Debug.Log ("UIPAnel: " + thisPanel);
		GameManager.Notifications.AddListener (this, "OnStateChange");


		
	}
	
	
	public void OnStateChange(Component Sender)
		{
		Debug.Log("bug Panel OnStateChange: " + GameManager.Instance.gameState);
		if (GameManager.Instance.gameState == GameManager.GameState.DEBUG){
			thisPanel.alpha = 1;
		}
		else{
			thisPanel.alpha = 0;
			// set panels Alpha to 0
		}
	}


}
