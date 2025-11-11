using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Gizmos;
using System;
using System.IO;
using System.Globalization;
using Microsoft.Unity.VisualStudio.Editor;

[ExecuteInEditMode]
public class MyCustomMesh : MonoBehaviour
{
    [SerializeField] string filepath;
	[SerializeField] string output_filepath;

    int nvertices;
    int nfaces;
    int nedges;
    List<Vector3> meshCoords;
    Vector3 meshGravityCenter;
    
    private struct Face {
	public int m_nvertices;
	public List<int> m_verticesIndexes;
	public Vector3 m_normal;
    };
    List<Face> facesList;

    private MeshFilter meshfilter;
    
    public void printInfo() {
	Debug.Log(nvertices);
	Debug.Log(nfaces);
	Debug.Log(nedges);
	Debug.Log(meshCoords.Count);
	Debug.Log(facesList.Count);
    }

	// Errors in the .off file are treated by simple "returning" before doing the computation    
    public void loadMesh() {
		meshfilter = GetComponent<MeshFilter>();
		meshCoords = new List<Vector3>();
		meshGravityCenter = Vector3.zero;
		facesList = new List<Face>();
		float maxNorm = -1.0f;

		// Loading mesh from file pointed by "filepath"
		string[] m_lines = File.ReadAllLines(filepath);
		if (m_lines.Length <= 1) return; // print error here
		if (m_lines[0] != "OFF") return; // if format isn't OFF, throw error, for the sake of time, we simply return;
		string[] mesh_specs = m_lines[1].Split(" ");
		if (mesh_specs.Length != 3) return; // if the specifications lines doesn't have 3 fields, return;
		nvertices = Int32.Parse(mesh_specs[0]);
		nfaces = Int32.Parse(mesh_specs[1]);
		nedges = Int32.Parse(mesh_specs[2]);
		if (m_lines.Length != (nvertices + nfaces + 2)) { // Check if their is incoherences in the specifications
			Debug.Log("missing info");
			return; // print error here
		}
	    
		// Reading all the vertices and by the same time, computing the mesh gravity center
		string[] mesh_line; 
		for (int i = 0; i < nvertices; i++) {
			mesh_line = m_lines[i+2].Split(" ");
			float x = Single.Parse(mesh_line[0], CultureInfo.InvariantCulture);
			float y = Single.Parse(mesh_line[1], CultureInfo.InvariantCulture);
			float z = Single.Parse(mesh_line[2], CultureInfo.InvariantCulture);
			Vector3 vec = new Vector3(x,y,z);

			meshGravityCenter = meshGravityCenter + vec; 
			meshCoords.Add(vec);
		}
		meshGravityCenter = meshGravityCenter / nvertices;

		// here we compute a translation allowing us to center our mesh on the mesh gravity center
		// we also use that loop to compute the maximal magnitude/length of the mesh
		Vector3 centeringTranslation = - meshGravityCenter;
		for (int i = 0; i < nvertices; i++) {
			if (Mathf.Abs(meshCoords[i].magnitude) > maxNorm)
			maxNorm = Mathf.Abs(meshCoords[i].magnitude);
			meshCoords[i] = meshCoords[i] + centeringTranslation;
		}

		// normalization of the mesh using the maxNorm computed previously
		for (int i = 0; i < nvertices; i++) {
			meshCoords[i] = meshCoords[i] / maxNorm;
		}
	
		/* Several things done here:
		 * - Reading each faces information and storing them in a private struct Face which contains
		 *   - number of vertices (which we assume to always be 3, again, for the sake of time)
		 *   - indexeses of the vertices composing the face 
		 *   - normal of the face
		 * - We compute the normal of the face by computing the cross production between edge1 and edge2, which are:
		 *	 - edge1: P(0) to P(1)
		 *	 - edge2: P(0) to P(2)
		 */  
		string[] face_line;
		Vector3 edge1,edge2;
		for (int i = nvertices + 2; i < nfaces + nvertices + 2; i++) {
			face_line = m_lines[i].Split(" ");
			Face f = new Face();
			f.m_nvertices = Int32.Parse(face_line[0]);
			f.m_verticesIndexes = new List<int>();
			for (int j = 0; j < f.m_nvertices; j++) {
			f.m_verticesIndexes.Add(Int32.Parse(face_line[j+1]));
			}
	   
			edge1 = meshCoords[f.m_verticesIndexes[1]] - meshCoords[f.m_verticesIndexes[0]];
			edge2 = meshCoords[f.m_verticesIndexes[2]] - meshCoords[f.m_verticesIndexes[0]];
			f.m_normal = Vector3.Cross(edge1, edge2);
			f.m_normal.Normalize();
			facesList.Add(f);
		}

		// Here we are computing a list of 3-tuples of indexeses which will correspond to each face (here triangles) of the mesh
		List<int> triangles = new List<int>();
		for (int i = 0; i < nfaces; i++) {
			if (facesList[i].m_nvertices != 3) return; // error case
			triangles.Add(facesList[i].m_verticesIndexes[0]);
			triangles.Add(facesList[i].m_verticesIndexes[1]);
			triangles.Add(facesList[i].m_verticesIndexes[2]);
		}

		// Unfortunately for us, Unity is using vertices normal, so we need to compute those normals using our faces normals
		List<Vector3> vertices_normals = new List<Vector3>();
		Vector3 edgeNormal = new Vector3(0,0,0);
		int edgeNFaces = 0;
		for (int i = 0; i < nvertices; i++) {
			edgeNFaces = 0;
			edgeNormal = new Vector3(0, 0, 0);
			for (int j = 0; j < nfaces; j++) {
				// We simply are summing each face normal which contains the 'i' vertices
				if (facesList[j].m_verticesIndexes.Contains(i)) {
					edgeNormal = facesList[j].m_normal + edgeNormal;
					edgeNFaces++;
				}
			}
			edgeNormal = edgeNormal / edgeNFaces;
			edgeNormal.Normalize();
			vertices_normals.Add(edgeNormal);
		}
	
		// Applying all of that to our mesh 
		meshfilter.mesh.vertices = meshCoords.ToArray();
		meshfilter.mesh.triangles = triangles.ToArray();
		meshfilter.mesh.normals = vertices_normals.ToArray();
    }

	public void exportMesh()
	{
		StreamWriter sw = File.CreateText(output_filepath);
		sw.WriteLine("OFF");
		string specs = nvertices.ToString() + " " + nfaces.ToString() + " " + nedges.ToString();
		sw.WriteLine(specs);
		foreach (Vector3 point in meshCoords)
		{
			sw.WriteLine(point.x.ToString() + " " + point.y.ToString() + " " + point.z.ToString());
		}

		foreach(Face f in facesList)
		{
			string faceSpec = f.m_nvertices.ToString();
			foreach(int p in f.m_verticesIndexes)
			{
				faceSpec = faceSpec + " " + p.ToString();
			}
			sw.WriteLine(faceSpec);
		}
    }

    public void OnDrawGizmoSelected() {
	Vector3 edgeNormal = new Vector3(0,0,0);
	int edgeNFaces = 0;
	for (int i = 0; i < nvertices; i++) {
	    edgeNFaces = 0;
	    for (int j = 0; j < nfaces; j++) {
		if (facesList[j].m_verticesIndexes.Contains(i)) {
		    edgeNormal = facesList[j].m_normal + edgeNormal;
		    edgeNFaces++;
		}
	    }
	    edgeNormal = edgeNormal / edgeNFaces;
	    edgeNormal.Normalize();
	    Gizmos.DrawLine(meshfilter.transform.position + meshCoords[i], meshfilter.transform.position + -edgeNormal);
	}
    }
    
    public void Awake() {
		loadMesh();
		exportMesh();
		printInfo();
    }
    
    public void Update() {
	//loadMesh();
	//printInfo();
    }

    public void OnRenderObject() {
	//loadMesh();
    }

    public void Start() {
		loadMesh();
        exportMesh();
        printInfo();
    }
}
