using UnityEngine;
using System.Collections;

public class BugButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClick(){
		Debug.Log ("Bug was clicked");
		GameManager.Instance.SetGameState(GameManager.GameState.DEBUG);
	}
}
