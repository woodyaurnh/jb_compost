using UnityEngine;
using System.Collections;

public class CameraMatrixLookat : MonoBehaviour {

	public float zOffset = 4.0F;

	public GMatrixDeform gmatrix;

	// Use this for initialization
	void Start () {
		float ht = transform.position.y;
		if(gmatrix)
		{
		Vector3 camPos = new Vector3(0,ht,-(gmatrix.matrixSize / 2.0F + zOffset)); 
		transform.position = camPos;
		transform.LookAt(gmatrix.transform);
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
