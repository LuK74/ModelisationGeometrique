using UnityEngine;
using static UnityEngine.Gizmos;

[ExecuteInEditMode]
public class TP1 : MonoBehaviour
{
    //private void _OnDrawGizmos()
    //{
    //   Vector3[] vertices;
    //  foreach (Vector3 t in vertices) {
    //    Gizmos.color = Color.white;
    //    Gizmos.DrawSphere(t, radius: 0.1f);
    // }
    //}

    void drawCylindre(float rayon, float height, int n_meridiens)
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        Vector3[] cylindreVertices = new Vector3[n_meridiens * 4 + 2];
        int[] cylindreTriangles = new int[6 * (n_meridiens * 2)]; // 2 triangles per planes, two planes per meridiens

        float theta_i = 2 * Mathf.PI * 0 / (n_meridiens + 1);
        cylindreVertices[0] = new Vector3(rayon * Mathf.Cos(theta_i), -height / 2, rayon * Mathf.Sin(theta_i));
        cylindreVertices[1] = new Vector3(rayon * Mathf.Cos(theta_i), height / 2, rayon * Mathf.Sin(theta_i));
        for (int i = 0; i < n_meridiens + 2; i++)
        {
            theta_i = 2 * Mathf.PI * i / (n_meridiens + 1);
            cylindreVertices[(i + 1) * 2] = new Vector3(rayon * Mathf.Cos(theta_i), -height / 2, rayon * Mathf.Sin(theta_i));
            cylindreVertices[(i + 1) * 2 + 1] = new Vector3(rayon * Mathf.Cos(theta_i), height / 2, rayon * Mathf.Sin(theta_i));

            cylindreTriangles[i * 6] = i * 2;
            cylindreTriangles[i * 6 + 1] = i * 2 + 1;
            cylindreTriangles[i * 6 + 2] = (i + 1) * 2;

            cylindreTriangles[i * 6 + 3] = (i + 1) * 2 + 1;
            cylindreTriangles[i * 6 + 4] = (i + 1) * 2;
            cylindreTriangles[i * 6 + 5] = i * 2 + 1;
        }

        int[] upperTriangles = new int[(n_meridiens * 2 + 1) * 3];
        upperTriangles[0] = 0;
        upperTriangles[1] = 2;
        upperTriangles[2] = ((n_meridiens+1) * 2);

        for (int i = 1; i < (n_meridiens * 2 + 1); i++)
        {
            upperTriangles[(i-1) * 3] = i * 2;
            upperTriangles[(i - 1) * 3 + 1] = ((i + 1)% (n_meridiens*2+1)) * 2;
            upperTriangles[(i - 1) * 3 + 2] = (i - 1) * 2;
        }

        int[] lowerTriangles = new int[(n_meridiens * 2 + 1) * 3];
        lowerTriangles[0] = 1;
        lowerTriangles[1] = 3;
        lowerTriangles[2] = ((n_meridiens + 1) * 2) + 1;

        for (int i = 1; i < (n_meridiens * 2 + 1); i++)
        {
            lowerTriangles[(i - 1) * 3] = i * 2 + 1;
            lowerTriangles[(i - 1) * 3 + 1] = ((i + 1) % (n_meridiens * 2 + 1)) * 2 + 1;
            lowerTriangles[(i - 1) * 3 + 2] = (i - 1) * 2 + 1;
        }

	int[] triangles = new int[cylindreTriangles.Length + upperTriangles.Length + lowerTriangles.Length];

	for (int i = 0; i < triangles.Length; i++) {
	    if (i < cylindreTriangles.Length)
		triangles[i] = cylindreTriangles[i];
	    else if (i < cylindreTriangles.Length + upperTriangles.Length)
		triangles[i] = upperTriangles[i - cylindreTriangles.Length];
	    else
		triangles[i] = lowerTriangles[i - (cylindreTriangles.Length + lowerTriangles.Length)];
	}
	
        mesh.vertices = cylindreVertices;
        mesh.triangles = triangles;
    }

    void drawPlane(Vector3[] vertices)
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.vertices = vertices;

        int[] planes_idxs = new int[6] { 0, 1, 3, 0, 2 ,3};
        mesh.triangles = planes_idxs;
        mesh.RecalculateNormals();

    }

    void drawTriangles(int nbLignes, int nbColonnes)
    {
        Vector3[] vertices = new Vector3[(nbColonnes + 1) * (nbLignes + 1)];
        int [] triangles = new int[6 * (nbColonnes + 1) * (nbLignes + 1)];

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
                triangles[i * nbColonnes * 6 + j * 6] = (i * (nbColonnes + 1) + j);
                triangles[i * nbColonnes * 6 + j * 6 + 1] = ((i+1) * (nbColonnes + 1) + j);
                triangles[i * nbColonnes * 6 + j * 6 + 2] = ((i+1) * (nbColonnes + 1) + j + 1);

                triangles[i * nbColonnes * 6 + j * 6 + 3] = (i * (nbColonnes + 1) + j);
                triangles[i * nbColonnes * 6 + j * 6 + 4] = (i * (nbColonnes + 1) + j + 1);
                triangles[i * nbColonnes * 6 + j * 6 + 5] = ((i + 1) * (nbColonnes + 1) + j + 1);

            }
        }



        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.RecalculateNormals();
    }

    void Start()
    {
        drawCylindre(2, 3, 4);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        drawCylindre(2, 3, 4);
    }

    void OnRenderObject()
    {
        drawCylindre(2, 3, 4);
    }


    // Update is called once per frame
    void Update()
    {
        drawCylindre(2, 3, 4);
    }
}
