using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Parse;

public class MatrixPtGroup {
	public List<int> ptnums;
	public float height;
}


public class GMatrixDeform: MonoBehaviour {

	public Transform spot;

	// deformable mesh params
	public int matrixSize = 10;
	public int gridRes = 10;


	Mesh mesh;
	Material mat;
	List<Vector3> Points;
	List<Vector3> Verts;
	List<int> Tris;
	List<Vector2> UVs;
	
	float size = 0.5f;
	private float ctrOffset = 0.0f;

	// end of deformable mesh params

	// dictionary of matrixPtGroup
	private Dictionary<int, MatrixPtGroup> ptGrpDict;
	private ParseObject[] _ppArray;

	// replace this with a 3D array later
	private Transform[] _gArray;
	private Transform testXform;
	private bool isStarted = false;

	private IEnumerable<ParseObject> results;


	// Use this for initialization
	void Start () {
		
		GameManager.Notifications.AddListener (this, "OnStateChange");
		GameManager.Notifications.AddListener (this, "OnPlantsLoaded");
		GameManager.Notifications.AddListener (this, "OnSpotPicked");
		_ppArray = new ParseObject[matrixSize * matrixSize];
		InitMesh();
		ptGrpDict = new Dictionary<int,MatrixPtGroup>();
		// this is temporary, should happen as a result of state change to game state
		BuildMatrix();
	}

	void Update(){
		//chose a gspot, get its point group, transform it
		Transform gst = _gArray[3];
		GSpot gs = gst.GetComponent<GSpot>();
		int key = gs.meshKey;
		MatrixPtGroup pgrp = ptGrpDict[key];
		List<int> ptnums = pgrp.ptnums;
		foreach(int ptIndex in ptnums)
		{
			//Debug.Log ("---> " + ptIndex);
			Vector3 newPoint = Points[ptIndex];
			newPoint.y += 0.05f * Time.deltaTime;
			Points[ptIndex] = newPoint;
		}
		UpdateMesh();

	}

	// Use this for initialization

	void InitMesh()
	{
		Points = new List<Vector3>();
		UVs = new List<Vector2>();
		
		gridRes++;
		int ord = 1;
		float curX = 0.0f;
		float curZ = 0.0f;
		float celWidth = (float)(matrixSize )/ (gridRes -1);
		float UVinc = 1.0f / (float)(gridRes - 1);
		
		ctrOffset = (float)matrixSize / 2.0f;
		
		Debug.Log ("UV increment: " + UVinc);
		
		for (int i = 0; i < gridRes; i++)
		{
			for (int j = 0; j < gridRes; j++)
			{
				curX = -celWidth * j;
				Debug.Log (ord++ + ": Vert: " + curX + " , " + curZ);
				Points.Add(new Vector3(curX + ctrOffset,0.0f,curZ - ctrOffset));
				//Points.Add(new Vector3(curX ,0.0f,curZ ));
				
				Vector2 curUV = new Vector2((float)i * UVinc, (float)j * UVinc);
				UVs.Add(curUV);
				
			}
			curZ += celWidth;
		}
		
		Debug.Log ("Points Count: " + Points.Count);
		

		
		Verts = new List<Vector3>();
		Tris = new List<int>();
		
		CreateMesh();
		
		
		
	}



	void BuildMatrix () 
	{
		_gArray = new Transform[matrixSize * matrixSize];
		Debug.Log ("_gArray Length: " + _gArray.Length);
		//GameObject matrixParent = new GameObject("matrixParent");
		int spotMeshKey = 1;
		int aIndex = 0;
		for (int i = 0;i < matrixSize; i++){
			for(int j = 0; j < matrixSize; j++){

				if(_gArray[aIndex] == null){
					//Debug.Log ("Melement : " + i + "   " + j + " is null");
					_gArray[aIndex] = Instantiate(spot,new Vector3((float)i- ctrOffset + .5f,0,(float)j - ctrOffset + .5f),Quaternion.identity) as Transform;
					Transform tt = _gArray[aIndex];
					//Debug.Log (">>> " + _gArray[aIndex].position);
					tt.name = "Spot_" + i + "_" + j;
					GSpot gs = tt.GetComponent<GSpot>();
					gs.spotX = i;
					gs.spotZ = j;
					gs.meshKey = spotMeshKey++;
					AssignMeshPoints(gs,1.2F);
					//tt.GetChild(0).transform.rotation = Quaternion.Euler(0, Random.Range(0.0F, 360.0F), 0);
					tt.gameObject.SetActive(true);
					tt.transform.parent = this.transform;

				}
				else{
				Debug.Log ("Melement : " + i + "   " + j + "   " + _gArray[aIndex]);

				}
				aIndex++;
			}
		}

/*		float scale = targetSize / matrixSize;
		float offset = ((matrixSize / 2) - 0.5F) * scale;
		//Debug.Log("offset: " + offset + " scale: " + scale);
		//this.transform.position = new Vector3(-offset,0,-offset);
		this.transform.localScale = new Vector3(scale,scale,scale);
*/

	}

	void AssignMeshPoints(GSpot gs, float groupRadius)
	{
		// get spot's x and x pos, find all mesh points in groupRadius from that point
		Debug.Log("AssignMeshPoints: " + gs.name);

		// make new list to hold ptnums
		List<int> pIndexList = new List<int>();
		// get ctr from spot
		for(int i = 0; i < Points.Count; i++){
			if(Vector3.Distance (gs.transform.position, Points[i]) < groupRadius)
			{
				Debug.Log ("Found pt index to assign: " + i);
				pIndexList.Add(i);
			}

		}
		if(pIndexList.Count > 0){
			// add list to new matrixPtGroup
			MatrixPtGroup  mgrp = new MatrixPtGroup();
			mgrp.ptnums = pIndexList;
			mgrp.height = 0.0F;
			ptGrpDict.Add (gs.meshKey,mgrp);

		}
		else
		{
			Debug.LogError("Spot at " + gs.spotX +"," + gs.spotZ + " has no mesh points in radius");
		}

	}

	public void OnStateChange(Component Sender)
	{
		//Debug.Log("Login Panel OnStateChange: " + GameManager.Instance.gameState);
		//Debug.Log("state: " + GameManager.Instance.gameState);
		if (GameManager.Instance.gameState == GameManager.GameState.MainMenu){
			Debug.Log ("GMatrix OnStateChange: " + GameManager.Instance.gameState);
			BuildMatrix ();

		}
	}

	public void OnPlantsLoaded(Component Sender)
	{
		Debug.Log ("GMatrix OnPlantsLoaded: " + GameManager.Instance.gameState);
		FillStoredPlants();
	}
	
	public void OnSpotPicked(Component Sender)
	{
		Debug.Log ("GMatrix OnSpotPicked: " + GameManager.Instance.gameState);
		BlankSpots();
	}
	

	private void FillStoredPlants(){
		Debug.Log ("GMatrix: FillStoredPlants called");
		foreach (var pair in GameManager.UserMgr.ppDict){
			ParseObject po = (ParseObject)pair.Value;
			string gridKey = (string)pair.Key;
			int pIdx = po.Get<int>("plantIdx");
			int sunshine = po.Get<int>("sunshine");
			float yRot = po.Get<float>("yRot");
			int ppx = po.Get<int>("x");
			int ppz = po.Get<int>("z");
			int curStage = po.Get<int>("curStage");
			bool mature = po.Get<bool>("mature");

			var numbers = Regex.Split(gridKey, @"\D+");
			int px = int.Parse (numbers[0]);
			int pz = int.Parse (numbers[1]);
			Debug.Log ("FillStoredPlants: getting plant at " + px + "," + pz);
			Transform tt = _gArray[(pz % matrixSize ) + (px * matrixSize)];
			GSpot gs = tt.GetComponent<GSpot>();
			if (gs != null){
				Debug.Log ("addplant!!!");
				gs.AddStoredPlant(po, 0.0F);
				gs.spotFilled = true;
			}

		}
		Debug.Log ("GMatrix: FillStoredPlants END");
	}


	public void SaveAllPlants(){
		for(int i = 0; i < _gArray.Length; i++){
		
			GSpot gs = _gArray[i].GetComponent<GSpot>();
			if(gs.plant != null){
				Plant p = gs.plant.GetComponent<Plant>();
				p.StorePlant();
			}

		}
	}


	/*
	private void GetStoredPlants(){
		Debug.Log ("GMatrix: GetStoredPlants called");
		var query = ParseObject.GetQuery("Plant")
			.WhereEqualTo("owner", ParseUser.CurrentUser);
		query.FindAsync().ContinueWith(t =>
		                               {
			if (t.IsFaulted)
			{
				Debug.Log ("Parse Error");
			}
			else
			{
				IEnumerable<ParseObject> results = t.Result;
				foreach (var obj in results)
				{
					var px = obj.Get<int>("x");
					var pz = obj.Get<int>("z");
					int aindex = (pz%matrixSize) + px*matrixSize;
					_ppArray[aindex] = (ParseObject)obj;
					var idx = obj.Get<int>("plantIdx");
					var mature = obj.Get<bool>("mature");
					var yRot = obj.Get<float>("yRot");
					string matStr = " is not ";
					if(mature)
						matStr = " is ";
					Debug.Log("Plant: " + idx + " at " + px + "," + pz + matStr + "mature");
					// convert x and z to single dim array index

					// check name against 2D array position
					//Transform tt = _gArray[aindex];

					// can't call GetComponent out of main thread! 
					//GSpot gs = tt.GetComponent<GSpot>();
					//gs.AddPlant (idx,yRot,0.0F);

				}
			}
			
		});
		
	}

*/

	void TestIndex(int x, int z){
		int aindex = (z%matrixSize) + x*matrixSize;
		// check name against 2D array position
		Transform gs = _gArray[aindex];
		Debug.Log (gs.name + " at " + x + "," + z + " Index: " + aindex);

	}

	void BlankSpots(){
		foreach (Transform spot in _gArray)
			spot.renderer.enabled = false;
	}

	private void CreateMesh()
	{
		gameObject.AddComponent("MeshFilter");
		gameObject.AddComponent("MeshRenderer");
		gameObject.AddComponent("MeshCollider");
		
		mat = Resources.Load ("Materials/Default") as Material;
		if(mat == null)
		{
			Debug.LogError("Material not found!");
			return;
		}
		
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if(meshFilter == null)
		{
			Debug.LogError("MeshFilter not found!");
			return;
		}
		
		mesh = meshFilter.sharedMesh;
		if(mesh == null)
		{
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.sharedMesh;
		}
		
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		if(meshCollider == null)
		{
			Debug.LogError("MeshCollider not found!");
			return;
		}
		
		mesh.Clear();
		UpdateMesh();
		
		
	}
	
	private void UpdateMesh()
	{
		
		// add the Verts from grid points
		foreach(Vector3 pt in Points){
			
			Verts.Add(pt);
		}
		
		
		
		// tris 
		
		int v = 0;
		
		for (int i = 0; i < (gridRes -1); i++)
		{
			for (int j = 0; j < (gridRes -1); j++)
			{
				
				int a = v;
				int b = v + 1;
				int c = v + gridRes;
				
				Tris.Add(v);
				Tris.Add(v + 1);
				Tris.Add(v + gridRes);
				Tris.Add(v + 1);
				Tris.Add(v + gridRes + 1);
				Tris.Add(v + gridRes);
				
				v += 1;
			}
			v +=1;
		}
		
		
		
		mesh.vertices = Verts.ToArray();
		mesh.triangles = Tris.ToArray();
		mesh.uv = UVs.ToArray();
		
		Verts.Clear();
		Tris.Clear();
		UVs.Clear();
		
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		//RecalculateTangents(mesh);
		// following is Unity quirk must be done in these two steps
		// or meshcollider not created
		meshCollider.sharedMesh = null;
		meshCollider.sharedMesh = mesh;
		renderer.material = mat;
		mesh.Optimize();
	}
	
	private static void RecalculateTangents(Mesh mesh)
	{
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
		
		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;
		
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		
		Vector4[] tangents = new Vector4[vertexCount];
		
		for(long a = 0; a < triangleCount; a += 3)
		{
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];
			
			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];
			
			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float div = s1 * t2 - s2 * t1;
			float r = div == 0.0f ? 0.0f : 1.0f / div;
			
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s2 * x1 - s1 * x2) * r, (s2 * y1 - s1 * y2) * r, (s2 * z1 - s1 * z2) * r);
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
		
		for (long a = 0; a < vertexCount; a++)
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];
			
			Vector3.OrthoNormalize(ref n, ref t );
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;
			
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n,t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
			
		}
		
		mesh.tangents = tangents;
	}


}
