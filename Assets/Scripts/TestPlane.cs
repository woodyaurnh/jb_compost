using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class planePtGroup {
	public List<int> ptnums;
	public float height;
	public Vector2 gridPosition;
}

public class TestPlane : MonoBehaviour {

	Mesh mesh;
	Material mat;
	List<Vector3> Points;
	List<Vector3> Verts;
	List<int> Tris;
	List<Vector2> UVs;

	float size = 0.5f;
	public int xSize = 10;
	public int gridRes = 4;

	private float ctrOffset = 0.0f;
	private planePtGroup[,] ptGroups;




	// Use this for initialization
	void Start ()
	{
		Points = new List<Vector3>();
		UVs = new List<Vector2>();

		gridRes++;
		int ord = 1;
		float curX = 0.0f;
		float curZ = 0.0f;
		float celWidth = (float)(xSize )/ (gridRes -1);
		float UVinc = 1.0f / (float)(gridRes - 1);

		ctrOffset = (float)xSize / 2.0f;

		Debug.Log ("UV increment: " + UVinc);

		for (int i = 0; i < gridRes; i++)
		{
			for (int j = 0; j < gridRes; j++)
			{
				curX = -celWidth * j;
				Debug.Log (ord++ + ": Vert: " + curX + " , " + curZ);
				//Points.Add(new Vector3(curX + ctrOffset,0.0f,curZ - ctrOffset));
				Points.Add(new Vector3(curX ,0.0f,curZ ));

				Vector2 curUV = new Vector2((float)i * UVinc, (float)j * UVinc);
				UVs.Add(curUV);

			}
			curZ += celWidth;
		}

		Debug.Log ("Points Count: " + Points.Count);

		// put point indices into point groups based on position
		ptGroups = new planePtGroup[xSize,xSize];
		
		int ptnum = 0;
		int pointsPerGrid = (gridRes -1) / xSize;	
		Debug.Log("Points per Grid Sq: " + pointsPerGrid);
		for (int i = 0; i <= xSize  ; i++)
		{
			for (int j = 0; j <= xSize ; j++)
			{
				planePtGroup ppg  = new planePtGroup();
				ppg.ptnums = new List<int>();
				ptGroups[i,j] = ppg;
				for (int k = 0; k < pointsPerGrid; k++)
				{
						planePtGroup a = ptGroups[i,j];
						a.ptnums.Add(ptnum);
						a.ptnums.Add(ptnum * pointsPerGrid);
						ptnum++;
	
				}
			}
		}

		for (int i = 0; i < xSize; i++)
		{
			Debug.Log("PtGroup " + i +  " : " + ptGroups[i,0]);
			for (int j = 0; j < xSize; j++)
			{
				Debug.Log("PtGroup " + i + "-" + j + " : " + ptGroups[i,j].ptnums.ToString());
			}
		}


		Verts = new List<Vector3>();
		Tris = new List<int>();

		CreateMesh();

		

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButton(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast (ray, out hit))
			{
				RaisePoint(FindNearestPoint(hit.point));
			}
		}
		if(Input.GetMouseButton(1))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast (ray, out hit))
			{
				LowerPoint(FindNearestPoint(hit.point));
			}
		}
	}

	private Vector3 FindNearestPoint(Vector3 point)
	{
		Vector3 NearestPoint = new Vector3();
		float lastDistance = 99999999f;

		for(int i = 0; i < Points.Count; i++)
		{
			float distance = Vector3.Distance(point, Points[i]);
			if ( distance < lastDistance)
			{
				lastDistance = distance;
				NearestPoint = Points[i];
			}
		}
		return NearestPoint;
	}

	private void RaisePoint(Vector3 point)
	{
		int index = -1;
		for(int i = 0; i < Points.Count; i++)
		{
			if (Points[i] == point)
			{
				index = i;
				break;
			}
		}
		if (index == -1)
			Debug.LogError("could not match points!");
		else{
			Vector3 newPoint = Points[index];
			newPoint.y += 0.01f;
			Points[index] = newPoint;
			UpdateMesh();
		}
	}

	private void LowerPoint(Vector3 point)
	{
		int index = -1;
		for(int i = 0; i < Points.Count; i++)
		{
			if (Points[i] == point)
			{
				index = i;
				break;
			}
		}
		if (index == -1)
			Debug.LogError("could not match points!");
		else{
			Vector3 newPoint = Points[index];
			newPoint.y -= 0.01f;
			Points[index] = newPoint;
			UpdateMesh();
		}
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
