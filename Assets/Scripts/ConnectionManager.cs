using UnityEngine;
using System.Collections;
using System.IO;

public class ConnectionManager : MonoBehaviour {

	// pretending the file system is a remote server

	// Use this for initialization
	void Start () {

		writeFile ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void writeFile(){
		StreamWriter sw = new StreamWriter("fakeDB/tmp.txt");
		sw.Write("test content");
		sw.Close ();
	}

}
