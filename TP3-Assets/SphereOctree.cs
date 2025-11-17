using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SphereOctree : MonoBehaviour
{
    [SerializeField] private float m_rayon;
    private Vector3 m_center;
    [SerializeField] private int m_maxDepth;

    struct Octree {
        public int _voxelVal; // 0: leaf (full square); 1: node (interior node), -1: node (exterior node)
        public int _voxelDepth;
        public float _voxelSize;
        public float _x, _y, _z;
        public List<Octree> _voxelsChilds; // empty if _val==0; if not, should be of size 8
    };
    private Octree m_octree;

    private int voxelInSphere(Octree currentVoxel, float voxelSize)
    {
        float _x, _y, _z;
        int numberOfIndexesInSphere = 0;
        for (int i = 0; i < 8; i++)
        {
            _x = ((i == 1) || (i == 2) || (i == 5) || (i == 6)) ? currentVoxel._x + voxelSize : currentVoxel._x;
            _y = (i < 4) ? currentVoxel._y + voxelSize : currentVoxel._y;
            _z = ((i % 4) - 2 >= 0) ? currentVoxel._z + voxelSize : currentVoxel._z;

            float leftComp = Mathf.Pow((_x - m_center.x), 2) + Mathf.Pow((_y - m_center.y), 2) + Mathf.Pow((_z - m_center.z), 2);
            if (leftComp < Mathf.Pow(m_rayon, 2))
            {
                numberOfIndexesInSphere++;
            }
        }

        if (numberOfIndexesInSphere == 7)
            return 1;
        else if (numberOfIndexesInSphere == 0)
            return -1;
        else
            return 0;
    }

    private void computeOctree(ref Octree currentVoxel, int currentdepth, float voxelSize)
    {
        currentVoxel._voxelDepth = currentdepth;
        currentVoxel._voxelSize = voxelSize;
        currentVoxel._voxelVal = voxelInSphere(currentVoxel, voxelSize);

        if ((currentdepth < m_maxDepth && currentVoxel._voxelVal == 0) || currentdepth == 1)
        {
            currentVoxel._voxelsChilds = new List<Octree>();
            for (int i = 0; i < 8; i++)
            {
                Octree newVoxel = new Octree();
                // need to find better equations for this
                newVoxel._x = currentVoxel._x + (((i == 1) || (i == 2) || (i == 5) || (i == 6)) ? (voxelSize / 2.0f) : 0.0f);
                newVoxel._y = currentVoxel._y + ((i < 4) ? (voxelSize / 2.0f) : 0.0f);
                newVoxel._z = currentVoxel._z + (((i % 4) - 2 >= 0) ? (voxelSize / 2.0f) : 0.0f);
                newVoxel._voxelVal = -1; // by default we'll consider a voxel as an exterior voxel

                computeOctree(ref newVoxel, currentdepth + 1, voxelSize / 2.0f);
                currentVoxel._voxelsChilds.Add(newVoxel);
            }
        }
    }

    private void printOctree(Octree currentOctree)
    {
        Debug.Log("--------------------------------------------------------------------------------");
        Debug.Log("Coords: "+currentOctree._x + " " + currentOctree._y + " " + currentOctree._z);
        Debug.Log("Value: " + currentOctree._voxelVal);
        Debug.Log("Size: " + currentOctree._voxelSize);
        Debug.Log("Depth: " + currentOctree._voxelDepth);
        if (currentOctree._voxelsChilds != null && currentOctree._voxelsChilds.Count != 0)
        {
            for (int i = 0; i < 8; i++)
            {
                Debug.Log("> Child below");
                printOctree(currentOctree._voxelsChilds[i]);
            }
        }
        Debug.Log("--------------------------------------------------------------------------------");
    }

    private void renderOctree(Octree currentOctree, int currentDepth) 
    {
        if ((currentOctree._voxelVal == 0 && currentDepth < m_maxDepth) || currentDepth == 1)
        {
            for (int i = 0; i < 8; i++)
                renderOctree(currentOctree._voxelsChilds[i], currentDepth+1);
        } else if (currentOctree._voxelVal != -1) {
            Debug.Log("Adding voxel");  
            GameObject voxelCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            voxelCube.transform.position = new Vector3(currentOctree._x + currentOctree._voxelSize / 2,
                currentOctree._y + currentOctree._voxelSize / 2,
                currentOctree._z + currentOctree._voxelSize / 2);
            voxelCube.transform.localScale = 
                new Vector3(currentOctree._voxelSize, currentOctree._voxelSize, currentOctree._voxelSize);
            voxelCube.transform.parent = this.gameObject.transform;
        } else
        {
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_center = transform.position;
        m_octree = new Octree();
        m_octree._x = 0.0f - m_rayon;
        m_octree._y = 0.0f - m_rayon;
        m_octree._z = 0.0f - m_rayon;
        computeOctree(ref m_octree, 1, m_rayon*2);
        printOctree(m_octree);
        renderOctree(m_octree, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
