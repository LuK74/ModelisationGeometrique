using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Gizmos;
using System;
using System.io;

[ExecuteInEditMode]
public class CustomMesh : MonoBehaviour
{
    [SerializeField] string filepath;

    int nvertices;
    int nfaces;
    int nedges;
    List<Vector3<float>> m_meshCoords;

    private struct face {
	int m_nvertices;
	List<int> m_verticesIndexes;
    } typedef Face;
    List<Face> facesList;
    
    public void loadMesh() {
	List<string> m_lines = File.ReadLines(filepath);
	if (m_lines.Count <= 1) return; // print error here
	if (m_lines[0] != "OFF") return;
	string[] mesh_specs = m_lines[1].Split(" ");
	if (mesh_specs.Size() != 3) return;
	
    }
}
