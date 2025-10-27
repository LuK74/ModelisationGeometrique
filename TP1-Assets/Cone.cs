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

    [SerialzedField] private bool m_isTruncated;

    void drawCone(float rayon, float height, float truncated_height, int n_meridiens)
    {
        if (n_meridiens == 0) return;

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] coneVertices = new Vector3[n_meridiens * 2];
        List<int> coneTriangles = new List<int>(); // 2 triangles per planes, two planes per meridiens

        float theta_i = 0;

        // Height_Bottom (height) / Rayon_Bottom (rayon) == Height_Top (truncated_height) / Rayon_Top (top_rayon)
        float top_rayon = (rayon / height) * truncated_height;  // (height/ration) is the same at any height

        for (int i = 0; i < n_meridiens; i++)
        {
            theta_i = 2 * Mathf.PI * i / (n_meridiens);
            coneVertices[i * 2] = new Vector3(rayon * Mathf.Cos(theta_i), 0, rayon * Mathf.Sin(theta_i));
            coneVertices[i * 2 + 1] = new Vector3(top_rayon * Mathf.Cos(theta_i), height - truncated_height, top_rayon * Mathf.Sin(theta_i));

            coneTriangles.Add(i * 2);
            coneTriangles.Add(i * 2 + 1);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2);

            coneTriangles.Add(((i + 1) % n_meridiens) * 2 + 1);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2);
            coneTriangles.Add(i * 2 + 1);
        }

        // Fan method to draw upper/lower face, we'll use (n_meridiens)_idx and (n_meridiens)_ixd + 1 as a fixed points
        for (int i = 0; i < n_meridiens; i++)
        {
            coneTriangles.Add(n_meridiens);
            coneTriangles.Add(i * 2);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2);

            coneTriangles.Add(n_meridiens + 1);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2 + 1);
            coneTriangles.Add(i * 2 + 1);
        }

        mesh.vertices = coneVertices;
        mesh.triangles = coneTriangles.ToArray();
    }

    void drawConeTruncated(float rayon, float height, float truncated_height, int n_meridiens, float truncated_angle)
    {
        if (n_meridiens == 0) return;

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] coneVertices = new Vector3[n_meridiens * 2];
        List<int> coneTriangles = new List<int>(); // 2 triangles per planes, two planes per meridiens

        float theta_i = 0;

        // Height_Bottom (height) / Rayon_Bottom (rayon) == Height_Top (truncated_height) / Rayon_Top (top_rayon)
        float top_rayon = (rayon / height) * truncated_height;  // (height/ration) is the same at any height

        for (int i = 0; i < n_meridiens; i++)
        {
            theta_i = ((2 * Mathf.PI) - truncatedAngle) * i / (n_meridiens - 1);
            coneVertices[i * 2] = new Vector3(rayon * Mathf.Cos(theta_i), 0, rayon * Mathf.Sin(theta_i));
            coneVertices[i * 2 + 1] = new Vector3(top_rayon * Mathf.Cos(theta_i), height - truncated_height, top_rayon * Mathf.Sin(theta_i));

            coneTriangles.Add(i * 2);
            coneTriangles.Add(i * 2 + 1);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2);

            coneTriangles.Add(((i + 1) % n_meridiens) * 2 + 1);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2);
            coneTriangles.Add(i * 2 + 1);
        }

        // Fan method to draw upper/lower face, we'll use (n_meridiens)_idx and (n_meridiens)_ixd + 1 as a fixed points
        for (int i = 0; i < n_meridiens; i++)
        {
            coneTriangles.Add(n_meridiens);
            coneTriangles.Add(i * 2);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2);

            coneTriangles.Add(n_meridiens + 1);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2 + 1);
            coneTriangles.Add(i * 2 + 1);
        }

        mesh.vertices = coneVertices;
        mesh.triangles = coneTriangles.ToArray();
    }

    void drawConeTruncated(float rayon, float height, float truncated_height, int n_meridiens, float truncated_angle)
    {
        if (n_meridiens == 0) return;
        if (truncated_angle < 0.0f || truncated_angle > 2 * Mathf.PI) drawCone(truncated_angle);

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] coneVertices = new Vector3[n_meridiens * 2];
        List<int> coneTriangles = new List<int>(); // 2 triangles per planes, two planes per meridiens

        float theta_i = 0;

        // Height_Bottom (height) / Rayon_Bottom (rayon) == Height_Top (truncated_height) / Rayon_Top (top_rayon)
        float top_rayon = (rayon / height) * truncated_height;  // (height/ration) is the same at any height

        for (int i = 0; i < n_meridiens; i++)
        {
            theta_i = 2 * Mathf.PI * i / (n_meridiens);
            coneVertices[i * 2] = new Vector3(rayon * Mathf.Cos(theta_i), 0, rayon * Mathf.Sin(theta_i));
            coneVertices[i * 2 + 1] = new Vector3(top_rayon * Mathf.Cos(theta_i), height - truncated_height, top_rayon * Mathf.Sin(theta_i));

            coneTriangles.Add(i * 2);
            coneTriangles.Add(i * 2 + 1);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2);

            coneTriangles.Add(((i + 1) % n_meridiens) * 2 + 1);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2);
            coneTriangles.Add(i * 2 + 1);
        }

        // Fan method to draw upper/lower face, we'll use (n_meridiens)_idx and (n_meridiens)_ixd + 1 as a fixed points
        for (int i = 0; i < n_meridiens; i++)
        {
            coneTriangles.Add(n_meridiens);
            coneTriangles.Add(i * 2);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2);

            coneTriangles.Add(n_meridiens + 1);
            coneTriangles.Add(((i + 1) % n_meridiens) * 2 + 1);
            coneTriangles.Add(i * 2 + 1);
        }

        mesh.vertices = coneVertices;
        mesh.triangles = coneTriangles.ToArray();
    }

    void drawShape()
    {
        if (m_isTruncated)
            drawConeTruncated(m_rayon, m_height, m_truncatedHeight, m_nmeridiens, m_truncatedAngle);
        else
            drawConeTruncated(m_rayon, m_height, m_truncatedHeight, m_nmeridiens);
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
