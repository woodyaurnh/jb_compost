using UnityEngine;
using System.Collections;

public class GamePanel : MonoBehaviour {
	private UIPanel thisPanel;
	private UILabel sunshineAmt;
	
	// Use this for initialization
	void Start () {

		thisPanel = GetComponent<UIPanel>();
		//Debug.Log ("UIPAnel: " + thisPanel);
		GameManager.Notifications.AddListener (this, "OnStateChange");
		sunshineAmt = GetComponentInChildren<UILabel>();


		
	}
	
	public void setSunshine(string amt){
		sunshineAmt.text = amt;
	}


	public void OnStateChange(Component Sender)
		{
		Debug.Log("Login Panel OnStateChange: " + GameManager.Instance.gameState);
		Debug.Log("state: " + GameManager.Instance.gameState);
		if (GameManager.Instance.gameState == GameManager.GameState.Game){
			Debug.Log ("state: " + GameManager.Instance.gameState);
			thisPanel.alpha = 1;
		}
		else{
			thisPanel.alpha = 0;
			// set panels Alpha to 0
		}
	}
	
	
}
