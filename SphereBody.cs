using UnityEngine;
using UnityEngine.UIElements;

public class SphereBody : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int resolution = 100;

    [SerializeField]
    //MeshFilter meshFilter;

    static readonly float tau = (1f + Mathf.Sqrt(5f)) / 2f;
    static readonly Vector3[] initialVertices = new Vector3[12]
    {
        new Vector3 (1, 0, tau),
            new Vector3 (1, 0, -tau),
            new Vector3 (-1, 0, tau),
            new Vector3 (-1, 0, -tau),
        new Vector3 (tau, 1, 0),
            new Vector3 (-tau, 1, 0),
            new Vector3 (tau, -1, 0),
            new Vector3 (-tau, -1, 0),
            new Vector3 (0, tau, 1),
            new Vector3 (0, -tau, 1),
            new Vector3 (0, tau, -1),
            new Vector3 (0, -tau, -1)

    };
    static readonly Color[] colors = new Color[12]
    {
        new Color(1, 0, 0, 1), //red0
        new Color(1, 0.5f, 0, 1), //orange1
        new Color(1, 1, 0, 1), //yellow2
        new Color(0.5f, 1, 0, 1), //yllowgren3
        new Color(0, 1, 0, 1), //gren4
        new Color(0, 1, 0.5f, 1), //green blue5
        new Color(0, 1, 1, 1), //light blue6
        new Color(0, 0.5f, 1, 1), //blue7
        new Color(0, 0, 1, 1), //dark blue8
        new Color(0.5f, 0, 1, 1), //purple9
        new Color(1, 0, 1, 1), //light purlple10
        new Color(1, 0, 0.5f, 1) //hotpink11
    };
    private void OnValidate()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        MeshFilter meshFilter = new MeshFilter();
        SphereMesh sphereMesh = new SphereMesh(resolution);

        mesh.vertices = sphereMesh.Vertices;
        mesh.triangles = sphereMesh.Triangles;

        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;
        
    }

    private void OnDrawGizmos()
    {
        if ( initialVertices == null) { return; }
        for (int i = 0; i < initialVertices.Length; i++)
        {
            //if (i == 0 ||i == 8 || i ==2)
            //{
            //    Gizmos.color = colors[i];
            //    Gizmos.DrawSphere(initialVertices[i], .1f);
            //}
            Gizmos.color = colors[i];
            Gizmos.DrawSphere(initialVertices[i], .1f);
        }
    }
}
