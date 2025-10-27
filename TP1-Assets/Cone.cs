using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Gizmos;

[ExecuteInEditMode]
public class Cone : MonoBehaviour
{
    [SerializeField] private float m_rayon;
    [SerializeField] private float m_height;
    [SerializeField] private float m_truncatedHeight;
    [SerializeField] private int m_nmeridiens;
    [SerializeField] private float m_truncatedAngle;

    [SerializeField] private bool m_isTruncated;

    void drawCone()
    {
        if (m_nmeridiens == 0 || m_rayon < 0 || m_height < 0) return;

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] coneVertices = new Vector3[m_nmeridiens * 2 + 2];
        List<int> coneTriangles = new List<int>(); // 2 triangles per planes, two planes per meridiens

        float theta_i = 0;
        Vector3 upperVertice = new Vector3(0, m_height - m_truncatedHeight);
        Vector3 lowerVertice = new Vector3(0, 0, 0);

        coneVertices[m_nmeridiens * 2] = lowerVertice;
        coneVertices[m_nmeridiens * 2 + 1] = upperVertice;

        // Height_Bottom (m_height) / Rayon_Bottom (m_rayon) == Height_Top (m_truncatedHeight) / Rayon_Top (top_rayon)
        float top_rayon = (m_rayon / m_height) * m_truncatedHeight;  // (m_height/ration) is the same at any m_height

        for (int i = 0; i < m_nmeridiens; i++)
        {
            theta_i = 2 * Mathf.PI * i / (m_nmeridiens);
            coneVertices[i * 2] = new Vector3(m_rayon * Mathf.Cos(theta_i), 0, m_rayon * Mathf.Sin(theta_i));
            coneVertices[i * 2 + 1] = new Vector3(top_rayon * Mathf.Cos(theta_i), m_height - m_truncatedHeight, top_rayon * Mathf.Sin(theta_i));

            coneTriangles.Add(i * 2);
            coneTriangles.Add(i * 2 + 1);
            coneTriangles.Add(((i + 1) % m_nmeridiens) * 2);

            coneTriangles.Add(((i + 1) % m_nmeridiens) * 2 + 1);
            coneTriangles.Add(((i + 1) % m_nmeridiens) * 2);
            coneTriangles.Add(i * 2 + 1);
        }

        // Fan method to draw upper/lower face, we'll use (m_nmeridiens)_idx and (m_nmeridiens)_ixd + 1 as a fixed points
        for (int i = 0; i < m_nmeridiens; i++)
        {
            coneTriangles.Add(m_nmeridiens * 2);
            coneTriangles.Add(i * 2);
            coneTriangles.Add(((i + 1) % m_nmeridiens) * 2);

            coneTriangles.Add(m_nmeridiens * 2 + 1);
            coneTriangles.Add(((i + 1) % m_nmeridiens) * 2 + 1);
            coneTriangles.Add(i * 2 + 1);
        }

        mesh.vertices = coneVertices;
        mesh.triangles = coneTriangles.ToArray();
    }

    void drawConeTruncated()
    {
        if (m_nmeridiens == 0) return;
        if (m_truncatedAngle < 0.0f || m_truncatedAngle > 2 * Mathf.PI)
            drawCone();

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] coneVertices = new Vector3[m_nmeridiens * 2 + 2];
        List<int> coneTriangles = new List<int>(); // 2 triangles per planes, two planes per meridiens

        Vector3 upperVertice = new Vector3(0, m_height - m_truncatedHeight);
        Vector3 lowerVertice = new Vector3(0, 0, 0);

        coneVertices[m_nmeridiens * 2] = lowerVertice;
        coneVertices[m_nmeridiens * 2 + 1] = upperVertice;

        float theta_i = 0;

        // Height_Bottom (m_height) / Rayon_Bottom (m_rayon) == Height_Top (m_truncatedHeight) / Rayon_Top (top_rayon)
        float top_rayon = (m_rayon / m_height) * m_truncatedHeight;  // (m_height/ration) is the same at any m_height

        for (int i = 0; i < m_nmeridiens; i++)
        {
            theta_i = ((2 * Mathf.PI) - m_truncatedAngle) * i / (m_nmeridiens - 1);
            coneVertices[i * 2] = new Vector3(m_rayon * Mathf.Cos(theta_i), 0, m_rayon * Mathf.Sin(theta_i));
            coneVertices[i * 2 + 1] = new Vector3(top_rayon * Mathf.Cos(theta_i), m_height - m_truncatedHeight, top_rayon * Mathf.Sin(theta_i));

            if (i < m_nmeridiens - 1) {
                coneTriangles.Add(i * 2);
                coneTriangles.Add(i * 2 + 1);
                coneTriangles.Add(((i + 1) % m_nmeridiens) * 2);

                coneTriangles.Add(((i + 1) % m_nmeridiens) * 2 + 1);
                coneTriangles.Add(((i + 1) % m_nmeridiens) * 2);
                coneTriangles.Add(i * 2 + 1);
            } else
            {
                coneTriangles.Add(i * 2);
                coneTriangles.Add(i * 2 + 1);
                coneTriangles.Add(m_nmeridiens * 2);

                coneTriangles.Add(m_nmeridiens * 2 + 1);
                coneTriangles.Add(m_nmeridiens * 2);
                coneTriangles.Add(i * 2 + 1);

                coneTriangles.Add(0);
                coneTriangles.Add(m_nmeridiens * 2);
                coneTriangles.Add(1);

                coneTriangles.Add(m_nmeridiens * 2 + 1);
                coneTriangles.Add(1);
                coneTriangles.Add(m_nmeridiens * 2);           
            }
        }

        // Fan method to draw upper/lower face, we'll use (m_nmeridiens)_idx and (m_nmeridiens)_ixd + 1 as a fixed points
        for (int i = 0; i < m_nmeridiens - 1; i++)
        {
            coneTriangles.Add(m_nmeridiens * 2);
            coneTriangles.Add(i * 2);
            coneTriangles.Add(((i + 1) % m_nmeridiens) * 2);

            coneTriangles.Add(m_nmeridiens * 2 + 1);
            coneTriangles.Add(((i + 1) % m_nmeridiens) * 2 + 1);
            coneTriangles.Add(i * 2 + 1);
        }

        mesh.vertices = coneVertices;
        mesh.triangles = coneTriangles.ToArray();
    }

    void drawShape()
    {
        if (m_isTruncated)
            drawConeTruncated();
        else
            drawCone();
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
