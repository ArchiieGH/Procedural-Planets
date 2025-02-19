using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class SphereBody : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    [SerializeField]
    MeshFilter meshFilter;
    [SerializeField]
    MeshRenderer meshRenderer;
    [Range(0, 30)]
    public int resolution = 1;
    Vector3[] vertices;
    Mesh mesh;

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

    private void Start()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        SphereMesh sphereMesh = new SphereMesh(resolution);

        mesh.vertices = sphereMesh.Vertices;
        mesh.triangles = sphereMesh.Triangles;
        vertices = mesh.vertices;

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

    }

    //private void OnDrawGizmos()
    //{
    //    if (vertices == null) { return; }
    //    int i = 0;
    //    foreach (Vector3 vertice in vertices)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawSphere(vertice, .05f);
    //        i++;
    //    }
    //}
}
