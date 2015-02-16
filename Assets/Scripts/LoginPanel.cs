using UnityEngine;
using System.Collections;

public class LoginPanel : MonoBehaviour {

	private UIPanel thisPanel;
	public UIInput userNameInput;
	public UIInput passwdInput;

	// Use this for initialization
	void Start () {
		
		thisPanel = GetComponent<UIPanel>();
		Debug.Log ("UIPAnel: " + thisPanel);
		GameManager.Notifications.AddListener (this, "OnStateChange");

		
		
		
	}
	

	// Update is called once per frame
	void Update () {
	
	}

	public void OnLoginSubmit(){
		// HERE!

		StartCoroutine(GameManager.UserMgr.Login(userNameInput.value,passwdInput.value));

		// testing with no connection
		//GameManager.UserMgr.FakeLogin();
	}

	public void OnSignIn(){
		GameManager.Instance.SetGameState(GameManager.GameState.SignIn);
	}

	public void OnStateChange(Component Sender)
	{
		Debug.Log("start Panel OnStateChange: " + GameManager.Instance.gameState);
		if (GameManager.Instance.gameState == GameManager.GameState.Login){
			thisPanel.alpha = 1;
		}
		else{
			thisPanel.alpha = 0;
			// set panels Alpha to 0
		}
	}

}
