using UnityEngine;
using System.Collections;


public class SignInPanel : MonoBehaviour {

	private UIPanel thisPanel;
	public UIInput userNameInput;
	public UIInput passwdInput;

	// Use this for initialization
	void Start () {
		
		thisPanel = GetComponent<UIPanel>();
		Debug.Log ("UIPAnel: " + thisPanel);
		GameManager.Notifications.AddListener (this, "OnStateChange");

		
		
		
	}
	

	public void OnSignInSubmit(){
		//PlayerPrefs.DeleteKey ("password");
		string n = userNameInput.value;
		string p = passwdInput.value;
		Debug.Log("Sign in --> Name: " + n + " password: " + p);
		if(n != "" && p != ""){
			StartCoroutine (GameManager.UserMgr.SignInCoR(n,p));
		}

	}

	public void OnStateChange(Component Sender)
	{
		Debug.Log("start Panel OnStateChange: " + GameManager.Instance.gameState);
		if (GameManager.Instance.gameState == GameManager.GameState.SignIn){
			thisPanel.alpha = 1;
		}
		else{
			thisPanel.alpha = 0;
			// set panels Alpha to 0
		}
	}

}
