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

    void drawCylindre()
    {
        if (m_nmeridiens == 0 || m_rayon < 0 || m_height < 0) return;

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] cylindreVertices = new Vector3[m_nmeridiens * 2];
        List<int> cylindreTriangles = new List<int>(); // 2 triangles per planes, two planes per meridiens

        float theta_i = 0;
        for (int i = 0; i < m_nmeridiens; i++)
        {
            theta_i = 2 * Mathf.PI * i / (m_nmeridiens);
            cylindreVertices[i * 2] = new Vector3(m_rayon * Mathf.Cos(theta_i), -m_height / 2, m_rayon * Mathf.Sin(theta_i));
            cylindreVertices[i * 2 + 1] = new Vector3(m_rayon * Mathf.Cos(theta_i), m_height / 2, m_rayon * Mathf.Sin(theta_i));

            cylindreTriangles.Add(i * 2);
            cylindreTriangles.Add(i * 2 + 1);
            cylindreTriangles.Add(((i + 1) % m_nmeridiens) * 2);

            cylindreTriangles.Add(((i + 1) % m_nmeridiens) * 2 + 1);
            cylindreTriangles.Add(((i + 1) % m_nmeridiens) * 2);
            cylindreTriangles.Add(i * 2 + 1);
        }

        // Fan method to draw upper/lower face, we'll use (m_nmeridiens)_idx and (m_nmeridiens)_ixd + 1 as a fixed points
        for (int i = 0; i < m_nmeridiens; i++)
        {
            cylindreTriangles.Add(m_nmeridiens);
            cylindreTriangles.Add(i * 2);
            cylindreTriangles.Add(((i + 1) % m_nmeridiens) * 2);

            cylindreTriangles.Add(m_nmeridiens + 1);
            cylindreTriangles.Add(((i + 1) % m_nmeridiens) * 2 + 1);
            cylindreTriangles.Add(i * 2 + 1);
        }

        mesh.vertices = cylindreVertices;
        mesh.triangles = cylindreTriangles.ToArray();
    }

    void drawCylindreTruncated()
    {
        if (m_nmeridiens == 0 || m_rayon < 0 || m_height < 0) return;
        // if the m_truncatedAngle isn't allowed, we'll draw a regular cylindre
        if (m_truncatedAngle < 0.0f || m_truncatedAngle > (2 * Mathf.PI)) drawCylindre();

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] cylindreVertices = new Vector3[m_nmeridiens * 2 + 2];
        List<int> cylindreTriangles = new List<int>(); // 2 triangles per planes, two planes per meridiens

        Vector3 upperVertice = new Vector3(0, -m_height / 2, 0);
        Vector3 lowerVertice = new Vector3(0, m_height / 2, 0);
        cylindreVertices[m_nmeridiens * 2] = upperVertice;
        cylindreVertices[m_nmeridiens * 2 + 1] = lowerVertice;

        float theta_i = 0;
        for (int i = 0; i < m_nmeridiens; i++)
        {
            theta_i = ((2 * Mathf.PI) - m_truncatedAngle) * i / (m_nmeridiens - 1);
            cylindreVertices[i * 2] = new Vector3(m_rayon * Mathf.Cos(theta_i), -m_height / 2, m_rayon * Mathf.Sin(theta_i));
            cylindreVertices[i * 2 + 1] = new Vector3(m_rayon * Mathf.Cos(theta_i), m_height / 2, m_rayon * Mathf.Sin(theta_i));

            if (i != (m_nmeridiens - 1))
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
                cylindreTriangles.Add(m_nmeridiens * 2);

                cylindreTriangles.Add(m_nmeridiens * 2 + 1);
                cylindreTriangles.Add(m_nmeridiens * 2);
                cylindreTriangles.Add(i * 2 + 1);

                cylindreTriangles.Add(0);
                cylindreTriangles.Add(m_nmeridiens * 2);
                cylindreTriangles.Add(1);

                cylindreTriangles.Add(m_nmeridiens * 2 + 1);
                cylindreTriangles.Add(1);
                cylindreTriangles.Add(m_nmeridiens * 2);
            }
        }

        // Truncated part


        // Center method used to draw upper/lower face, we'll use 2 additional vertices as fixed point
        // better to use that here, when we want to be able to draw a truncated cylindre
        for (int i = 0; i < m_nmeridiens - 1; i++)
        {
            cylindreTriangles.Add(m_nmeridiens * 2);
            cylindreTriangles.Add(i * 2);
            cylindreTriangles.Add(((i + 1) % m_nmeridiens) * 2);

            cylindreTriangles.Add(m_nmeridiens * 2 + 1);
            cylindreTriangles.Add(((i + 1) % m_nmeridiens) * 2 + 1);
            cylindreTriangles.Add(i * 2 + 1);
        }

        mesh.vertices = cylindreVertices;
        mesh.triangles = cylindreTriangles.ToArray();
    }

    void drawShape()
    {
        if (m_isTruncated)
            drawCylindreTruncated();
        else
            drawCylindre();
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
