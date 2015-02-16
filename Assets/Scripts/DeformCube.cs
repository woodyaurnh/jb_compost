using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeformCube : MonoBehaviour {

	Mesh mesh;
	Material mat;
	List<Vector3> Points;
	List<Vector3> Verts;
	List<int> Tris;
	List<Vector2> UVs;

	float size = 0.5f;


	// Use this for initialization
	void Start ()
	{
		Points = new List<Vector3>();

		Points.Add(new Vector3(-size,size,-size));
		Points.Add(new Vector3(size,size,-size));
		Points.Add(new Vector3(size,-size,-size));
		Points.Add(new Vector3(-size,-size,-size));

		Points.Add(new Vector3(size,size,size));
		Points.Add(new Vector3(-size,size,size));
		Points.Add(new Vector3(-size,-size,size));
		Points.Add(new Vector3(size,-size,size));

		Verts = new List<Vector3>();
		Tris = new List<int>();
		UVs = new List<Vector2>();

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
		// Front plane
		Verts.Add(Points[0]); Verts.Add (Points[1]); Verts.Add (Points[2]); Verts.Add (Points[3]);
		// Back plane
		Verts.Add(Points[4]); Verts.Add (Points[5]); Verts.Add (Points[6]); Verts.Add (Points[7]);
		// Left plane
		Verts.Add(Points[5]); Verts.Add (Points[0]); Verts.Add (Points[3]); Verts.Add (Points[6]);
		// Right plane
		Verts.Add(Points[1]); Verts.Add (Points[4]); Verts.Add (Points[7]); Verts.Add (Points[2]);
		// Top plane
		Verts.Add(Points[5]); Verts.Add (Points[4]); Verts.Add (Points[1]); Verts.Add (Points[0]);
		// Bottom plane
		Verts.Add(Points[3]); Verts.Add (Points[2]); Verts.Add (Points[7]); Verts.Add (Points[6]);


		//
		//Front plane
		Tris.Add(0); Tris.Add(1); Tris.Add(2);
		Tris.Add(2); Tris.Add(3); Tris.Add(0);
		//Back plane
		Tris.Add(4); Tris.Add(5); Tris.Add(6);
		Tris.Add(6); Tris.Add(7); Tris.Add(4);
		//Left plane
		Tris.Add(8); Tris.Add(9); Tris.Add(10);
		Tris.Add(10); Tris.Add(11); Tris.Add(8);
		//Right plane
		Tris.Add(12); Tris.Add(13); Tris.Add(14);
		Tris.Add(14); Tris.Add(15); Tris.Add(12);
		//Top plane
		Tris.Add(16); Tris.Add(17); Tris.Add(18);
		Tris.Add(18); Tris.Add(19); Tris.Add(16);
		//Bottom plane
		Tris.Add(20); Tris.Add(21); Tris.Add(22);
		Tris.Add(22); Tris.Add(23); Tris.Add(20);

		//Front plane
		UVs.Add(new Vector2(0,1));
		UVs.Add(new Vector2(1,1));
		UVs.Add(new Vector2(1,0));
		UVs.Add(new Vector2(0,0));
		//Back plane
		UVs.Add(new Vector2(0,1));
		UVs.Add(new Vector2(1,1));
		UVs.Add(new Vector2(1,0));
		UVs.Add(new Vector2(0,0));
		//Left plane
		UVs.Add(new Vector2(0,1));
		UVs.Add(new Vector2(1,1));
		UVs.Add(new Vector2(1,0));
		UVs.Add(new Vector2(0,0));
		//Right plane
		UVs.Add(new Vector2(0,1));
		UVs.Add(new Vector2(1,1));
		UVs.Add(new Vector2(1,0));
		UVs.Add(new Vector2(0,0));
		//Top plane
		UVs.Add(new Vector2(0,1));
		UVs.Add(new Vector2(1,1));
		UVs.Add(new Vector2(1,0));
		UVs.Add(new Vector2(0,0));
		//Bottom plane
		UVs.Add(new Vector2(0,1));
		UVs.Add(new Vector2(1,1));
		UVs.Add(new Vector2(1,0));
		UVs.Add(new Vector2(0,0));

		mesh.vertices = Verts.ToArray();
		mesh.triangles = Tris.ToArray();
		mesh.uv = UVs.ToArray();

		Verts.Clear();
		Tris.Clear();
		UVs.Clear();

		MeshCollider meshCollider = GetComponent<MeshCollider>();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		RecalculateTangents(mesh);
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
