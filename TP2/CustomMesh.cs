using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Gizmos;
using System;
using System.IO;
using System.Globalization;

public class CustomMesh : MonoBehaviour
{
    [SerializeField] string filepath;

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
    
    public void loadMesh() {
	meshfilter = GetComponent<MeshFilter>();
	meshCoords = new List<Vector3>();
	meshGravityCenter = Vector3.zero;
	facesList = new List<Face>();
	float maxNorm = -1.0f;

	
	string[] m_lines = File.ReadAllLines(filepath);
	if (m_lines.Length <= 1) return; // print error here
	if (m_lines[0] != "OFF") return;
	string[] mesh_specs = m_lines[1].Split(" ");
	if (mesh_specs.Length != 3) return;
	nvertices = Int32.Parse(mesh_specs[0]);
	nfaces = Int32.Parse(mesh_specs[1]);
	nedges = Int32.Parse(mesh_specs[2]);
	if (m_lines.Length != (nvertices + nfaces + 2)) {
	    Debug.Log("missing info");
	    return; // print error here
	}
	    
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
	Vector3 centeringTranslation = - meshGravityCenter;
	for (int i = 0; i < nvertices; i++) {
	    if (Mathf.Abs(meshCoords[i].magnitude) > maxNorm)
		maxNorm = Mathf.Abs(meshCoords[i].magnitude);
	    meshCoords[i] = meshCoords[i] + centeringTranslation;
	}

	// normalization of the mesh
	for (int i = 0; i < nvertices; i++) {
	    meshCoords[i] = meshCoords[i] / maxNorm;
	}
	
	string[] face_line;
	List<Vector3> faces_normals = new List<Vector3>();
	Vector3 edge1,edge2;
	for (int i = nvertices + 2; i < nfaces + nvertices + 2; i++) {
	    face_line = m_lines[i].Split(" ");
	    Face f = new Face();
	    f.m_nvertices = Int32.Parse(face_line[0]);
	    f.m_verticesIndexes = new List<int>();
	    for (int j = 0; j < f.m_nvertices; j++) {
		f.m_verticesIndexes.Add(Int32.Parse(face_line[j+1]));
	    }
	    facesList.Add(f);
	    edge1 = meshCoords[f.m_verticesIndexes[1]] - meshCoords[f.m_verticesIndexes[0]];
	    edge2 = meshCoords[f.m_verticesIndexes[2]] - meshCoords[f.m_verticesIndexes[0]];
	    f.m_normal = Vector3.Cross(edge2, edge1);
	    f.m_normal.Normalize();
	    faces_normals.Add(f.m_normal);
	}

	List<int> triangles = new List<int>();
	for (int i = 0; i < nfaces; i++) {
	    if (facesList[i].m_nvertices != 3) return; // error case
	    triangles.Add(facesList[i].m_verticesIndexes[0]);
	    triangles.Add(facesList[i].m_verticesIndexes[1]);
	    triangles.Add(facesList[i].m_verticesIndexes[2]);
	}

	List<Vector3> vertices_normals = new List<Vector3>();
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
	    vertices_normals.Add(edgeNormal);
	}
	
	meshfilter.mesh.vertices = meshCoords.ToArray();
	meshfilter.mesh.triangles = triangles.ToArray();
	meshfilter.mesh.RecalculateNormals();
	meshfilter.mesh.normals = vertices_normals.ToArray();
	//meshfilter.transform.position = meshGravityCenter;
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
	printInfo();
    }
    
    public void Update() {
	loadMesh();
	//printInfo();
    }

    public void OnRenderObject() {
	loadMesh();
    }

    public void Start() {
	loadMesh();
	printInfo();
    }
}
