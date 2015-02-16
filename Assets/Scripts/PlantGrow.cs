using UnityEngine;
using System.Collections;

public class PlantGrow : MonoBehaviour {

	//growDuration in milliseconds
	public float growDuration = 1.0f;
	public bool isGrowing = false;

	public GameObject[] growStages = null;
	public int _curStage = -1;



	// Use this for initialization
	void Start () {
	
	}

	public IEnumerator Tapped(){
		Debug.Log ("Tapped");
		if (isGrowing){
			Debug.Log ("call ResetGrowth");
			ResetGrowth();
			isGrowing = false;
			StartCoroutine(Grow());
			yield break;
		} 
		else {
			Debug.Log ("call Grow");
			 StartCoroutine(Grow());
		} yield break;
	}

		


	public void ResetGrowth(){
		Debug.Log ("ResetGrowth");
		StopAllCoroutines();
		// set all children inactive
		if (growStages.Length > 1){
			for(int i = 0; i < growStages.Length; i++){
				growStages[i].SetActive (false);
			}
		}
		


	}

	public IEnumerator Grow(){
		Debug.Log ("Grow");
		//ResetGrowth();


		if(growStages.Length < 1) {
			Debug.Log ("No Grow Stages");
			yield break;
		}
		if (growStages.Length == 1 ){
			Debug.Log("Only one stage");
			yield return new WaitForSeconds(growDuration);
			growStages[0].SetActive(true);
			isGrowing = true;
			_curStage = 0;
			yield break;
		}
		if (growStages.Length > 1){
			Debug.Log ("More than One Stage");
			float partialDuration = growDuration/growStages.Length;
			Debug.Log ("PD: " + partialDuration);
			isGrowing = true;
			for(int i = 0; i < growStages.Length; i++){
				yield return new WaitForSeconds(partialDuration);
				growStages[i].SetActive (true);
				_curStage = i;
				if(i > 0)
					growStages[i-1].SetActive (false);
			}
			yield break;

		}



	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
