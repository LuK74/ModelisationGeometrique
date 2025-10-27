using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Gizmos;

[ExecuteInEditMode]
public class Cone : MonoBehaviour
{
    [SerializeField] private int m_nbLignes;
    [SerializeField] private int m_nbColonnes;

    void drawTriangles(int nbLignes, int nbColonnes)
    {
        Vector3[] vertices = new Vector3[(nbColonnes + 1) * (nbLignes + 1)];
        List<int> triangles = new List<int>();

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        for (int i = 0; i < nbLignes + 1; i++)
        {
            for (int j = 0; j < nbColonnes + 1; j++)
            {
                vertices[i * (nbColonnes + 1) + j] = new Vector3((float)j / (float)(nbColonnes), (float)i / (float)(nbLignes), 0);
            }
        }

        // (i,j) describe a block containing 2 triangles, so 6 indexes to add
        // (i,j,0),(i,j+1,0),(i+1,j+1,0) and (i,j,0),(i+1,j,0),(i+1,j+1,0)
        // (i,j)+0 / (i,j)+1, (i,j)+3 and (i,j)+0, (i,j)+2, (i,j)+3
        for (int i = 0; i < nbLignes; i++)
        {
            for (int j = 0; j < nbColonnes; j++)
            {
                triangles.Add((i * (nbColonnes + 1) + j));
                triangles.Add(((i + 1) * (nbColonnes + 1) + j));
                triangles.Add(((i + 1) * (nbColonnes + 1) + j + 1));

                triangles.Add((i * (nbColonnes + 1) + j));
                triangles.Add(((i + 1) * (nbColonnes + 1) + j + 1));
                triangles.Add((i * (nbColonnes + 1) + j + 1));

            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
    }

    void drawShape()
    {
           drawTriangles(m_nbLignes, m_nbColonnes);
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
