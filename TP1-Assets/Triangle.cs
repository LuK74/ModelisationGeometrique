using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Gizmos;

[ExecuteInEditMode]
public class Triangle : MonoBehaviour
{
    [SerializeField] private int m_nbLignes;
    [SerializeField] private int m_nbColonnes;

    void drawTriangles()
    {
        if (m_nbLignes == 0 || m_nbColonnes == 0) return;
        Vector3[] vertices = new Vector3[(m_nbColonnes + 1) * (m_nbLignes + 1)];
        List<int> triangles = new List<int>();

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        for (int i = 0; i < m_nbLignes + 1; i++)
        {
            for (int j = 0; j < m_nbColonnes + 1; j++)
            {
                vertices[i * (m_nbColonnes + 1) + j] = new Vector3((float)j / (float)(m_nbColonnes), (float)i / (float)(m_nbLignes), 0);
            }
        }

        // (i,j) describe a block containing 2 triangles, so 6 indexes to add
        // (i,j,0),(i,j+1,0),(i+1,j+1,0) and (i,j,0),(i+1,j,0),(i+1,j+1,0)
        // (i,j)+0 / (i,j)+1, (i,j)+3 and (i,j)+0, (i,j)+2, (i,j)+3
        for (int i = 0; i < m_nbLignes; i++)
        {
            for (int j = 0; j < m_nbColonnes; j++)
            {
                triangles.Add((i * (m_nbColonnes + 1) + j));
                triangles.Add(((i + 1) * (m_nbColonnes + 1) + j));
                triangles.Add(((i + 1) * (m_nbColonnes + 1) + j + 1));

                triangles.Add((i * (m_nbColonnes + 1) + j));
                triangles.Add(((i + 1) * (m_nbColonnes + 1) + j + 1));
                triangles.Add((i * (m_nbColonnes + 1) + j + 1));

            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
    }

    void drawShape()
    {
           drawTriangles();
    }

    void Start()
    {
        drawShape();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        drawShape();
    }

    void OnRenderObject()
    {
        drawShape();
    }


    // Update is called once per frame
    void Update()
    {
        drawShape();
    }
}
