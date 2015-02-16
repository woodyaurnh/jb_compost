using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClick(){
		Debug.Log ("I was clicked");
		GameManager.Instance.SetGameState(GameManager.GameState.Game);
	}
}
