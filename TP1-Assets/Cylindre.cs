using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Gizmos;

[ExecuteInEditMode]
public class Cylindre : MonoBehaviour
{
    [SerializeField] private float m_rayon;
    [SerializeField] private float m_height;
    [SerializeField] private int m_nmeridiens;
    [SerializeField] private float m_truncatedAngle;
    [SerializeField] private bool m_isTruncated;

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

    void drawCylindreTruncated(float rayon, float height, int n_meridiens, float truncatedAngle)
    {
        if (n_meridiens == 0) return;
        // if the truncatedAngle isn't allowed, we'll draw a regular cylindre
        if (truncatedAngle < 0.0f || truncatedAngle > (2 * Mathf.PI)) drawCylindre(rayon, height, n_meridiens);

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] cylindreVertices = new Vector3[n_meridiens * 2 + 2];
        List<int> cylindreTriangles = new List<int>(); // 2 triangles per planes, two planes per meridiens

        Vector3 upperVertice = new Vector3(0, -height / 2, 0);
        Vector3 lowerVertice = new Vector3(0, height / 2, 0);
        cylindreVertices[n_meridiens * 2] = upperVertice;
        cylindreVertices[n_meridiens * 2 + 1] = lowerVertice;

        float theta_i = 0;
        for (int i = 0; i < n_meridiens; i++)
        {
            theta_i = ((2 * Mathf.PI) - truncatedAngle) * i / (n_meridiens - 1);
            cylindreVertices[i * 2] = new Vector3(rayon * Mathf.Cos(theta_i), -height / 2, rayon * Mathf.Sin(theta_i));
            cylindreVertices[i * 2 + 1] = new Vector3(rayon * Mathf.Cos(theta_i), height / 2, rayon * Mathf.Sin(theta_i));

            if (i != (n_meridiens - 1))
            {
                cylindreTriangles.Add(i * 2);
                cylindreTriangles.Add(i * 2 + 1);
                cylindreTriangles.Add((i + 1) * 2);

                cylindreTriangles.Add((i + 1) * 2 + 1);
                cylindreTriangles.Add((i + 1) * 2);
                cylindreTriangles.Add(i * 2 + 1);
            }
            else
            {
                cylindreTriangles.Add(i * 2);
                cylindreTriangles.Add(i * 2 + 1);
                cylindreTriangles.Add(n_meridiens * 2);

                cylindreTriangles.Add(n_meridiens * 2 + 1);
                cylindreTriangles.Add(n_meridiens * 2);
                cylindreTriangles.Add(i * 2 + 1);

                cylindreTriangles.Add(0);
                cylindreTriangles.Add(n_meridiens * 2);
                cylindreTriangles.Add(1);

                cylindreTriangles.Add(n_meridiens * 2 + 1);
                cylindreTriangles.Add(1);
                cylindreTriangles.Add(n_meridiens * 2);
            }
        }

        // Truncated part


        // Center method used to draw upper/lower face, we'll use 2 additional vertices as fixed point
        // better to use that here, when we want to be able to draw a truncated cylindre
        for (int i = 0; i < n_meridiens - 1; i++)
        {
            cylindreTriangles.Add(n_meridiens * 2);
            cylindreTriangles.Add(i * 2);
            cylindreTriangles.Add(((i + 1) % n_meridiens) * 2);

            cylindreTriangles.Add(n_meridiens * 2 + 1);
            cylindreTriangles.Add(((i + 1) % n_meridiens) * 2 + 1);
            cylindreTriangles.Add(i * 2 + 1);
        }

        mesh.vertices = cylindreVertices;
        mesh.triangles = cylindreTriangles.ToArray();
    }

    void drawShape()
    {
        if (m_isTruncated)
            drawCylindreTruncated(m_rayon, m_height, m_nmeridiens, m_truncatedAngle);
        else
            drawCylindre(m_rayon, m_height, m_nmeridiens);
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
