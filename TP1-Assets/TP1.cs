using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Gizmos;

[ExecuteInEditMode]
public class TP1 : MonoBehaviour
{
    enum Shape
    {
        Cylindre = 1,
        Triangle = 2,
        Sphere = 3,
    }

    [SerializeField] Shape ShapeType;


    void drawSphere(float rayon, int n_parallels, int n_meridiens)
    {
        if (n_parallels < 2 || n_meridiens < 3) return;
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] sphereVertices = new Vector3[n_meridiens * n_parallels + 2];
        sphereVertices[n_meridiens * n_parallels] = new Vector3(0, rayon, 0); // North pole
        sphereVertices[n_meridiens * n_parallels + 1] = new Vector3(0, -rayon, 0); // South pole
        List<int> sphereTriangles = new List<int>();

        for (int i = 0; i < n_parallels; i++)
        {
            for (int j = 0; j < n_meridiens; j++)
            {
                float theta_i = (2 * Mathf.PI * j) / n_meridiens;
                float zeta_i = (Mathf.PI * i) / n_parallels;

                float x = rayon * Mathf.Sin(zeta_i) * Mathf.Cos(theta_i);
                float y = rayon * Mathf.Sin(zeta_i) * Mathf.Sin(theta_i);
                float z = rayon * Mathf.Cos(zeta_i);

                // Reversing z,y compare to slides, because Unity axis aren't the same
                sphereVertices[i * n_meridiens + j] = new Vector3(x, z, y);

                // North pole case
                if (i == 0)
                {
                    sphereTriangles.Add(n_meridiens * n_parallels); // North pole if i==0, if not south pole
                    sphereTriangles.Add((i * n_meridiens + ((j + 1) % n_meridiens)));
                    sphereTriangles.Add(i * n_meridiens + j);
                }
                else
                {
                    sphereTriangles.Add((i - 1) * n_meridiens + j);
                    sphereTriangles.Add((i * n_meridiens + ((j + 1) % n_meridiens)));
                    sphereTriangles.Add((i * n_meridiens + j));

                    sphereTriangles.Add((i - 1) * n_meridiens + j);
                    sphereTriangles.Add(((i - 1) * n_meridiens + ((j + 1) % n_meridiens)));
                    sphereTriangles.Add((i * n_meridiens + ((j + 1) % n_meridiens)));
                }
            }
        }

        // Adding south pole
        int k = n_parallels - 1;
        for (int j = 0; j < n_meridiens; j++)
        {
            sphereTriangles.Add(n_meridiens * n_parallels + 1);
            sphereTriangles.Add(k * n_meridiens + j);
            sphereTriangles.Add((k * n_meridiens + ((j + 1) % n_meridiens)));
        }

        mesh.vertices = sphereVertices;
        mesh.triangles = sphereTriangles.ToArray();
    }

    void drawCylindre(float rayon, float height, int n_meridiens)
    {
        if (n_meridiens == 0) return;

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] cylindreVertices = new Vector3[n_meridiens * 2];
        List<int> cylindreTriangles = new List<int>(); // 2 triangles per planes, two planes per meridiens

        float theta_i = 0;
        for (int i = 0; i < n_meridiens; i++)
        {
            theta_i = 2 * Mathf.PI * i / (n_meridiens);
            cylindreVertices[i * 2] = new Vector3(rayon * Mathf.Cos(theta_i), -height / 2, rayon * Mathf.Sin(theta_i));
            cylindreVertices[i * 2 + 1] = new Vector3(rayon * Mathf.Cos(theta_i), height / 2, rayon * Mathf.Sin(theta_i));

            cylindreTriangles.Add(i * 2);
            cylindreTriangles.Add(i * 2 + 1);
            cylindreTriangles.Add(((i + 1) % n_meridiens) * 2);

            cylindreTriangles.Add(((i + 1) % n_meridiens) * 2 + 1);
            cylindreTriangles.Add(((i + 1) % n_meridiens) * 2);
            cylindreTriangles.Add(i * 2 + 1);
        }

        // Fan method to draw upper/lower face, we'll use (n_meridiens)_idx and (n_meridiens)_ixd + 1 as a fixed points
        for (int i = 0; i < n_meridiens; i++)
        {
            cylindreTriangles.Add(n_meridiens);
            cylindreTriangles.Add(i * 2);
            cylindreTriangles.Add(((i + 1) % n_meridiens) * 2);

            cylindreTriangles.Add(n_meridiens + 1);
            cylindreTriangles.Add(((i + 1) % n_meridiens) * 2 + 1);
            cylindreTriangles.Add(i * 2 + 1);
        }

        mesh.vertices = cylindreVertices;
        mesh.triangles = cylindreTriangles.ToArray();
    }

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
        if (ShapeType == Shape.Cylindre)
        {
            drawCylindre(2, 3, 20);
        }
        else if (ShapeType == Shape.Triangle)
        {
            drawTriangles(4, 5);
        }
        else if (ShapeType == Shape.Sphere)
        {
            drawSphere(2, 20, 30);
        }
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
