using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Gizmos;

[ExecuteInEditMode]
public class Sphere : MonoBehaviour
{
    [SerializeField] private float m_rayon;
    [SerializeField] private int m_nmeridiens;
    [SerializeField] private int m_nparallels;
    [SerializeField] private float m_truncatedAngle;
    [SerializeField] private bool m_isTruncated;

    void drawSphere()
    {
        if (m_nparallels < 2 || m_nmeridiens < 3 || m_rayon < 0) return;
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] sphereVertices = new Vector3[m_nmeridiens * m_nparallels + 2];
        sphereVertices[m_nmeridiens * m_nparallels] = new Vector3(0, m_rayon, 0); // North pole
        sphereVertices[m_nmeridiens * m_nparallels + 1] = new Vector3(0, -m_rayon, 0); // South pole
        List<int> sphereTriangles = new List<int>();

        for (int i = 0; i < m_nparallels; i++)
        {
            for (int j = 0; j < m_nmeridiens; j++)
            {
                float theta_i = (2 * Mathf.PI * j) / m_nmeridiens;
                float zeta_i = (Mathf.PI * (i + 1)) / (m_nparallels + 1); // our zeta doesn't include 0 and Mathf.PI

                float x = m_rayon * Mathf.Sin(zeta_i) * Mathf.Cos(theta_i);
                float y = m_rayon * Mathf.Sin(zeta_i) * Mathf.Sin(theta_i);
                float z = m_rayon * Mathf.Cos(zeta_i);

                // Reversing z,y compare to slides, because Unity axis aren't the same
                sphereVertices[i * m_nmeridiens + j] = new Vector3(x, z, y);

                // North pole case
                if (i == 0)
                {
                    sphereTriangles.Add(m_nmeridiens * m_nparallels); // North pole
                    sphereTriangles.Add((i * m_nmeridiens + ((j + 1) % m_nmeridiens)));
                    sphereTriangles.Add(i * m_nmeridiens + j);
                }
                else
                {
                    sphereTriangles.Add((i - 1) * m_nmeridiens + j);
                    sphereTriangles.Add((i * m_nmeridiens + ((j + 1) % m_nmeridiens)));
                    sphereTriangles.Add((i * m_nmeridiens + j));

                    sphereTriangles.Add((i - 1) * m_nmeridiens + j);
                    sphereTriangles.Add(((i - 1) * m_nmeridiens + ((j + 1) % m_nmeridiens)));
                    sphereTriangles.Add((i * m_nmeridiens + ((j + 1) % m_nmeridiens)));
                }
            }
        }

        // Adding south pole
        int k = m_nparallels - 1;
        for (int j = 0; j < m_nmeridiens; j++)
        {
            sphereTriangles.Add(m_nmeridiens * m_nparallels + 1);
            sphereTriangles.Add(k * m_nmeridiens + j);
            sphereTriangles.Add((k * m_nmeridiens + ((j + 1) % m_nmeridiens)));
        }

        mesh.vertices = sphereVertices;
        mesh.triangles = sphereTriangles.ToArray();
    }

    void drawSphereTruncated()
    {
        if (m_nparallels < 2 || m_nmeridiens < 3 || m_rayon < 0) return;
        if (m_truncatedAngle < 0.0f || m_truncatedAngle > 2 * Mathf.PI) drawSphere();
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] sphereVertices = new Vector3[m_nmeridiens * m_nparallels + 2 + m_nparallels];
        sphereVertices[m_nmeridiens * m_nparallels] = new Vector3(0, m_rayon, 0); // North pole
        sphereVertices[m_nmeridiens * m_nparallels + 1] = new Vector3(0, -m_rayon, 0); // South pole
        List<int> sphereTriangles = new List<int>();

        // Building the truncated spheres 'out' faces
        for (int i = 0; i < m_nparallels; i++)
        {
            for (int j = 0; j < m_nmeridiens; j++)
            {
                float theta_i = ((2 * Mathf.PI - m_truncatedAngle) * j) / (m_nmeridiens - 1);
                float zeta_i = (Mathf.PI * (i + 1)) / (m_nparallels + 1); // our zeta doesn't include 0 and Mathf.PI 

                float x = m_rayon * Mathf.Sin(zeta_i) * Mathf.Cos(theta_i);
                float y = m_rayon * Mathf.Sin(zeta_i) * Mathf.Sin(theta_i);
                float z = m_rayon * Mathf.Cos(zeta_i);

                // Reversing z,y compare to slides, because Unity axis aren't the same
                sphereVertices[i * m_nmeridiens + j] = new Vector3(x, z, y);

                // North pole case
                if (i == 0 && j < (m_nmeridiens - 1))
                {
                    sphereTriangles.Add(m_nmeridiens * m_nparallels); // North pole
                    sphereTriangles.Add((i * m_nmeridiens + ((j + 1) % m_nmeridiens)));
                    sphereTriangles.Add(i * m_nmeridiens + j);
                }
                else if (j < m_nmeridiens - 1)
                {
                    sphereTriangles.Add((i - 1) * m_nmeridiens + j);
                    sphereTriangles.Add((i * m_nmeridiens + ((j + 1) % m_nmeridiens)));
                    sphereTriangles.Add((i * m_nmeridiens + j));

                    sphereTriangles.Add((i - 1) * m_nmeridiens + j);
                    sphereTriangles.Add(((i - 1) * m_nmeridiens + ((j + 1) % m_nmeridiens)));
                    sphereTriangles.Add((i * m_nmeridiens + ((j + 1) % m_nmeridiens)));
                }
            }
        }

        // Building the truncated part starting from North pole
        for (int i = 0; i < m_nparallels; i++)
        {
            float zeta_i = Mathf.PI * i / m_nparallels;

            float z = m_rayon * Mathf.Cos(zeta_i);

            // Reversing z,y compare to slides, because Unity axis aren't the same
            sphereVertices[m_nmeridiens * m_nparallels + 2 + i] = new Vector3(0, z, 0);

            // North pole case
            if (i == 0)
            {
                sphereTriangles.Add(m_nmeridiens * m_nparallels); // North pole
                sphereTriangles.Add(0);
                sphereTriangles.Add(m_nmeridiens * m_nparallels + 2);

                sphereTriangles.Add(m_nmeridiens * m_nparallels); // North pole
                sphereTriangles.Add(m_nmeridiens * m_nparallels + 2);
                sphereTriangles.Add(m_nmeridiens - 1);
            }
            else
            {
                // Left side of the truncated face
                sphereTriangles.Add(m_nmeridiens * m_nparallels + 1 + i);
                sphereTriangles.Add(((i - 1) * m_nmeridiens));
                sphereTriangles.Add(m_nmeridiens * m_nparallels + 2 + i);

                sphereTriangles.Add(m_nmeridiens * m_nparallels + 2 + i);
                sphereTriangles.Add(((i - 1) * m_nmeridiens));
                sphereTriangles.Add((i * m_nmeridiens));

                // Right side of the truncated face
                sphereTriangles.Add(m_nmeridiens * m_nparallels + 1 + i);
                sphereTriangles.Add(m_nmeridiens * m_nparallels + 2 + i);
                sphereTriangles.Add(((i - 1) * m_nmeridiens) + (m_nmeridiens - 1));

                sphereTriangles.Add(m_nmeridiens * m_nparallels + 2 + i);
                sphereTriangles.Add((i * m_nmeridiens) + (m_nmeridiens - 1));
                sphereTriangles.Add(((i - 1) * m_nmeridiens) + (m_nmeridiens - 1));

            }
        }

        // Adding south pole lower face
        int k = m_nparallels - 1;
        for (int j = 0; j < m_nmeridiens - 1; j++)
        {
            sphereTriangles.Add(m_nmeridiens * m_nparallels + 1);
            sphereTriangles.Add(k * m_nmeridiens + j);
            sphereTriangles.Add((k * m_nmeridiens + ((j + 1) % m_nmeridiens)));
        }

        // Adding last truncated faces linked to south pole
        sphereTriangles.Add(m_nmeridiens * m_nparallels + 1); // South pole
        sphereTriangles.Add(m_nmeridiens * m_nparallels + 1 + m_nparallels);
        sphereTriangles.Add(m_nmeridiens * (m_nparallels - 1));

        sphereTriangles.Add(m_nmeridiens * m_nparallels + 1); // South pole
        sphereTriangles.Add(m_nmeridiens * (m_nparallels - 1) + (m_nmeridiens - 1));
        sphereTriangles.Add(m_nmeridiens * m_nparallels + 1 + m_nparallels);

        mesh.vertices = sphereVertices;
        mesh.triangles = sphereTriangles.ToArray();
    }

    void drawShape()
    {
        if (m_isTruncated)
            drawSphereTruncated();
        else
            drawSphere();
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
