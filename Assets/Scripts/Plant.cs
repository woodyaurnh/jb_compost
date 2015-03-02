using UnityEngine;
using System;
using System.Collections;
using Parse;

public class Plant : MonoBehaviour {

	// public vars

	// Index which should match the index of the button that chooses plant for planting
	public int plantIdx = -1;

	// Time it takes plant to grow in seconds
	public float maturityAge = 30.0f;
	

	// state variables
	public bool isMature = false;
	public bool isGrowing = false;

	public int x;
	public int z;
	public int yRot;

	// array of child plant stage objects
	public GameObject[] growStages = null;

	// Parse object representing the plant
	public ParseObject pPlant = null;

	public int maxSunshine = 50;
	public int sunshine = 0;
	public int sunshineIncValue = 1;
	public float sunTickIncrement = 3.0F;


	// for debugging 
	public float startTime = 0.0f;

	//-------------------------
	// private vars, some may be temp public to see in inspector

	// current stage -1 means not started growing
	// zero-indexed, so the first stage is 0
	public int _curStage = -1;

	private float _stageDuration = 0.0f;


	// error delta
	const float EDELTA = 0.001f;


	private void UpdateFromParseObj(){
	}


	public bool DoGrow(float startTime){
		// if no stages, don't grow
		if(growStages.Length < 1) {
			Debug.Log ("No Grow Stages");
			return false;
		}
		// only one stage, no need to calculate anything
		if (growStages.Length == 1 ){
			isGrowing = true;

			Debug.Log("Only one stage");
			isGrowing = true;
			_curStage = 0;
			TransitionToStage(_curStage);
			if(!isMature){
				GameManager.Notifications.PostNotification(this,"OnPlantMatured");
				isMature = true;
			}
			return true;
		}
		isGrowing = true;

		// calculate how much time plant has to reach maturity
		float growTimeLeft = maturityAge - startTime;

		// calc starting stage and time left in that stage- 
		// note: first stage is immediate, has no duration
		_stageDuration = maturityAge / (growStages.Length -1.0f);
		Debug.Log ("Stage Duration: " + _stageDuration);
		float tmpTimeLeft = growTimeLeft;
		int tmpStage = growStages.Length;
		Debug.Log ("Begin: tmptime: " + tmpTimeLeft + " tmpStage: " + tmpStage);
		while(tmpTimeLeft > (0 + EDELTA)){
			tmpTimeLeft -= _stageDuration;
			tmpStage--;
			Debug.Log ("Partial: tmptime: " + tmpTimeLeft + " tmpStage: " + tmpStage);
		}
		_curStage = tmpStage -1;
		float stageTimeLeft = _stageDuration + tmpTimeLeft;
		Debug.Log ("Final: Stage: " + _curStage + " Time Left in Stage " + stageTimeLeft);

		// turn on the current stage
		growStages[_curStage].SetActive (true);
		StartCoroutine(Grow(_curStage,stageTimeLeft));




		return true;

	}

	public void Tapped(){
		Debug.Log ("Tapped");
		if (!isGrowing){
			Debug.Log ("call DoGrow");
			 DoGrow(0.0F);
		}
	}

	public void OldPlant(float startTime){
		DoGrow(startTime);
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
	//-----------------------------------------
	// Grow() interface should be
	// Grow(int startStage, float timeInFirstStage)

	public IEnumerator Grow(int startingStage, float timeInFirstStage){
		Debug.Log ("Grow");
		// run first stage for time left

		yield return new WaitForSeconds(timeInFirstStage);
		TransitionToStage(_curStage++);
		while(_curStage < growStages.Length){
			yield return new WaitForSeconds(_stageDuration);
			TransitionToStage(_curStage++);
		}
			yield break;
	}

	//---------------------
	// separate out the transition in case we want to do anything fancier between stages
	// such as dissolve between for example
	private void TransitionToStage(int stageNumber){
		Debug.Log ("TtS: " + transform.parent.name + " -- to stage " + stageNumber);
		if (stageNumber > 0){
			growStages[stageNumber -1].SetActive (false);
		}
		if(stageNumber < growStages.Length){ // avoid weird out-of-range error
		growStages[stageNumber].SetActive (true);
		pPlant["curStage"] = _curStage;
		pPlant.SaveAsync();

		}
		if (stageNumber >= (growStages.Length - 1)){
			if(!isMature){
				GameManager.Notifications.PostNotification(this,"OnPlantMatured");
				isMature = true;
				UpdateMature();
			}
		}

	}

	private void UpdateMature(){
		if (pPlant != null){
			pPlant["mature"] = true;
			pPlant.SaveAsync();
		}
		StartCoroutine(IncrementSunshine(sunTickIncrement));
	}
	
public void InitFromStored(){
		if(pPlant != null){
			DateTime currentDate = DateTime.Now.ToUniversalTime();			
			DateTime? updated = pPlant.UpdatedAt;
			DateTime? created = pPlant.CreatedAt;

			//get ticks per second
			float sunTicksPS = (float)sunshineIncValue / sunTickIncrement;

			isMature = pPlant.Get<bool>("mature");
			if(!isMature) {
				// calculate the age of the plant AND sunshine if it has matured while offline
				if(created.HasValue){
					DateTime createdDate = created.Value;
					long ticksSinceCreated = (currentDate.Ticks - createdDate.Ticks);
					Debug.Log ("curticks: " + currentDate.Ticks + " createdTicks: " + createdDate.Ticks);
					TimeSpan createSpan  = new TimeSpan(ticksSinceCreated);
					double secsSinceCreated = createSpan.TotalSeconds;
					if (secsSinceCreated < maturityAge)
					{
						//  do Grow with calced age
						Debug.Log ("Calling DoGrow with start Time " + secsSinceCreated);
						DoGrow((float)secsSinceCreated);
					}
					else isMature = true;
					}
				else {
					Debug.Log ("Error: no value in the parse CreatedAt field");
				}
			}

			if(isMature){
				// just calc sunshine added since last time logged in
				if( updated.HasValue ){
					DateTime updatedDate = updated.Value;
					long ticksSinceUpdated = (currentDate.Ticks - updatedDate.Ticks);
					TimeSpan updateSpan  = new TimeSpan(ticksSinceUpdated);
					double secsSinceUpdated = updateSpan.TotalSeconds;
					double sunSinceUpdated = (secsSinceUpdated / sunTicksPS);
					sunshine = sunshine + (int)sunSinceUpdated ;
					UpdateMature();
					_curStage = growStages.Length - 1;
					TransitionToStage(_curStage);

				}
				else {
					Debug.Log ("Error: no value in the parse UpdatedAt field");
				}
			}

		}
		else {
			Debug.Log ("Error: attempting to restore plant from ParseObject that is NULL");
		}

	}

	IEnumerator IncrementSunshine(float sunTickIncrement){
		int stmp = sunshine;
		while(true){
			yield return new WaitForSeconds(sunTickIncrement);
			sunshine += sunshineIncValue;
			if (pPlant != null){
				pPlant["sunshine"] = sunshine;
			}

			if ((sunshine - stmp ) > 10){
				pPlant.SaveAsync();
				stmp = sunshine;
			}
			if (sunshine > maxSunshine){
				sunshine = maxSunshine;
				yield break;
			}
			
		}
		
	}

	public ParseObject StorePlant(){
		Debug.Log ("Plant:StorePlant");
		if (pPlant == null){
			Debug.Log ("Plant:StorePlant: Error! This Plant has no ParseObject pPlant to store");
		}
		pPlant["owner"] = ParseUser.CurrentUser;

		pPlant["plantIdx"] = plantIdx;
		pPlant["mature"] = isMature;
		pPlant["maturityAge"] = maturityAge;
		pPlant["sunshine"] = sunshine;
		pPlant["curStage"] = _curStage;
		
		pPlant.SaveAsync();
		return pPlant;	

	}
	


}
