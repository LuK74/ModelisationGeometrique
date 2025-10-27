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
    [SerializeField] private int m_nparallels;
    [SerializeField] private float m_truncatedAngle;
    [SerializeField] private bool m_isTruncated;

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
                float zeta_i = (Mathf.PI * (i + 1)) / (n_parallels + 1); // our zeta doesn't include 0 and Mathf.PI

                float x = rayon * Mathf.Sin(zeta_i) * Mathf.Cos(theta_i);
                float y = rayon * Mathf.Sin(zeta_i) * Mathf.Sin(theta_i);
                float z = rayon * Mathf.Cos(zeta_i);

                // Reversing z,y compare to slides, because Unity axis aren't the same
                sphereVertices[i * n_meridiens + j] = new Vector3(x, z, y);

                // North pole case
                if (i == 0)
                {
                    sphereTriangles.Add(n_meridiens * n_parallels); // North pole
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

    void drawSphereTruncated(float rayon, int n_parallels, int n_meridiens, float truncatedAngle)
    {
        if (n_parallels < 2 || n_meridiens < 3) return;
        if (truncatedAngle < 0.0f || truncatedAngle > 2 * Mathf.PI) drawSphere(rayon, n_parallels, n_meridiens);
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] sphereVertices = new Vector3[n_meridiens * n_parallels + 2 + n_parallels];
        sphereVertices[n_meridiens * n_parallels] = new Vector3(0, rayon, 0); // North pole
        sphereVertices[n_meridiens * n_parallels + 1] = new Vector3(0, -rayon, 0); // South pole
        List<int> sphereTriangles = new List<int>();

        // Building the truncated spheres 'out' faces
        for (int i = 0; i < n_parallels; i++)
        {
            for (int j = 0; j < n_meridiens; j++)
            {
                float theta_i = ((2 * Mathf.PI - truncatedAngle) * j) / (n_meridiens - 1);
                float zeta_i = (Mathf.PI * (i + 1)) / (n_parallels + 1); // our zeta doesn't include 0 and Mathf.PI 

                float x = rayon * Mathf.Sin(zeta_i) * Mathf.Cos(theta_i);
                float y = rayon * Mathf.Sin(zeta_i) * Mathf.Sin(theta_i);
                float z = rayon * Mathf.Cos(zeta_i);

                // Reversing z,y compare to slides, because Unity axis aren't the same
                sphereVertices[i * n_meridiens + j] = new Vector3(x, z, y);

                // North pole case
                if (i == 0 && j < (n_meridiens - 1))
                {
                    sphereTriangles.Add(n_meridiens * n_parallels); // North pole
                    sphereTriangles.Add((i * n_meridiens + ((j + 1) % n_meridiens)));
                    sphereTriangles.Add(i * n_meridiens + j);
                }
                else if (j < n_meridiens - 1)
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

        // Building the truncated part starting from North pole
        for (int i = 0; i < n_parallels; i++)
        {
            float zeta_i = Mathf.PI * i / n_parallels;

            float z = rayon * Mathf.Cos(zeta_i);

            // Reversing z,y compare to slides, because Unity axis aren't the same
            sphereVertices[n_meridiens * n_parallels + 2 + i] = new Vector3(0, z, 0);

            // North pole case
            if (i == 0)
            {
                sphereTriangles.Add(n_meridiens * n_parallels); // North pole
                sphereTriangles.Add(0);
                sphereTriangles.Add(n_meridiens * n_parallels + 2);

                sphereTriangles.Add(n_meridiens * n_parallels); // North pole
                sphereTriangles.Add(n_meridiens * n_parallels + 2);
                sphereTriangles.Add(n_meridiens - 1);
            }
            else
            {
                // Left side of the truncated face
                sphereTriangles.Add(n_meridiens * n_parallels + 1 + i);
                sphereTriangles.Add(((i - 1) * n_meridiens));
                sphereTriangles.Add(n_meridiens * n_parallels + 2 + i);

                sphereTriangles.Add(n_meridiens * n_parallels + 2 + i);
                sphereTriangles.Add(((i - 1) * n_meridiens));
                sphereTriangles.Add((i * n_meridiens));

                // Right side of the truncated face
                sphereTriangles.Add(n_meridiens * n_parallels + 1 + i);
                sphereTriangles.Add(n_meridiens * n_parallels + 2 + i);
                sphereTriangles.Add(((i - 1) * n_meridiens) + (n_meridiens - 1));

                sphereTriangles.Add(n_meridiens * n_parallels + 2 + i);
                sphereTriangles.Add((i * n_meridiens) + (n_meridiens - 1));
                sphereTriangles.Add(((i - 1) * n_meridiens) + (n_meridiens - 1));

            }
        }

        // Adding south pole lower face
        int k = n_parallels - 1;
        for (int j = 0; j < n_meridiens - 1; j++)
        {
            sphereTriangles.Add(n_meridiens * n_parallels + 1);
            sphereTriangles.Add(k * n_meridiens + j);
            sphereTriangles.Add((k * n_meridiens + ((j + 1) % n_meridiens)));
        }

        // Adding last truncated faces linked to south pole
        sphereTriangles.Add(n_meridiens * n_parallels + 1); // South pole
        sphereTriangles.Add(n_meridiens * n_parallels + 1 + n_parallels);
        sphereTriangles.Add(n_meridiens * (n_parallels - 1));

        sphereTriangles.Add(n_meridiens * n_parallels + 1); // South pole
        sphereTriangles.Add(n_meridiens * (n_parallels - 1) + (n_meridiens - 1));
        sphereTriangles.Add(n_meridiens * n_parallels + 1 + n_parallels);

        mesh.vertices = sphereVertices;
        mesh.triangles = sphereTriangles.ToArray();
    }

    void drawShape()
    {
        if (m_isTruncated)
            drawSphereTruncated(m_rayon, m_height, m_nmeridiens, m_nparallels, m_truncatedAngle);
        else
            drawSphere(m_rayon, m_height, m_nmeridiens, m_nparallels);
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
