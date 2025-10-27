using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Gizmos;
enum Shape

{
    Cylindre = 1,
    Triangles = 2,
    Sphere = 3,
    Cone = 4,
    CylindreTruncated = 5,
    SphereTruncated = 6,
}

[ExecuteInEditMode]
public class TP1 : MonoBehaviour
{
    [SerializeField] private Shape ShapeType;

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
        sphereTriangles.Add(n_meridiens* (n_parallels - 1) + (n_meridiens - 1));
        sphereTriangles.Add(n_meridiens * n_parallels + 1 + n_parallels);

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
            } else
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
                triangles.Add(((i+1) * (nbColonnes + 1) + j));
                triangles.Add(((i+1) * (nbColonnes + 1) + j + 1));

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
        if (ShapeType == Shape.Cylindre) {
            drawCylindre(2, 3, 20);
        } else if (ShapeType == Shape.Triangles)
        {
            drawTriangles(4, 5);
        } else if (ShapeType == Shape.Sphere)
        {
            drawSphere(2, 10, 15);
        } else if (ShapeType == Shape.Cone)
        {
            drawCone(2, 3, 3/100 , 20);
        } else if (ShapeType == Shape.CylindreTruncated)
        {
            drawCylindreTruncated(2, 3, 20, Mathf.PI / 2);
        } else if (ShapeType == Shape.SphereTruncated)
        {
            drawSphereTruncated(2, 10, 15, Mathf.PI / 2);
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
