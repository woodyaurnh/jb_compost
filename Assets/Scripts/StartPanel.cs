using UnityEngine;
using System.Collections;

public class StartPanel : MonoBehaviour {
	private UIPanel thisPanel;
	private UILabel greetingLabel;
	
	// Use this for initialization
	void Start () {

		thisPanel = GetComponent<UIPanel>();
		//Debug.Log ("UIPAnel: " + thisPanel);
		GameManager.Notifications.AddListener (this, "OnStateChange");
		greetingLabel = GetComponentInChildren<UILabel>();


		
	}
	
	
	public void OnStateChange(Component Sender)
		{
		//Debug.Log("Login Panel OnStateChange: " + GameManager.Instance.gameState);
		//Debug.Log("state: " + GameManager.Instance.gameState);
		if (GameManager.Instance.gameState == GameManager.GameState.MainMenu){
			Debug.Log ("StartPanel OnStateChange: " + GameManager.Instance.gameState);
			greetingLabel.text = "Welcome " + GameManager.UserMgr.GetCurrentUser();
			thisPanel.alpha = 1;
		}
		else{
			thisPanel.alpha = 0;
			// set panels Alpha to 0
		}
	}
	
	
}
