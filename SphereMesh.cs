using System.CodeDom.Compiler;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class SphereMesh
{
    public readonly Vector3[] Vertices;
    public readonly int[] Triangles;
    public readonly int Resolution;


    int numOfDivisions;
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
    static readonly private int[] initialTriangles = new int[]
    {
        0, 8, 2,
        0, 2, 9,
        2, 7, 9,
        2, 5, 7,
        2, 8, 5,
        8, 10, 5,
        8, 4, 10,
        8, 0, 4,
        0, 6, 4,
        0, 9, 6,

        3, 1, 11,
        3, 11, 7,
        11, 9, 7,
        11, 6, 9,
        1, 6, 11,
        1, 4, 6,
        1, 10, 4,
        1, 3, 10,
        3, 5, 10,
        3, 7, 5
    };
    static readonly int[] vertexPairs = new int[] {

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

    public SphereMesh(int resolution)
    {
        this.Resolution = resolution;
        numOfDivisions = Mathf.Max(0, resolution);


        Vertices = initialVertices;
        Triangles = initialTriangles;
    }

    // Update is called once per frame
    public void GenerateMesh()
    {


    }

    
}
