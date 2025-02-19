using Mono.Cecil;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.LowLevelPhysics;
using UnityEngine.Rendering;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class SphereMesh
{
    public readonly Vector3[] Vertices;
    public readonly int[] Triangles;
    public readonly int Resolution;

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

    FixedSizeList<Vector3> vertices;
    FixedSizeList<int> triangles;
    int numOfDivisions;
    int numOfVertsPerFace;
    int numOfTrisPerFace;

    public SphereMesh(int resolution)
    {
        this.Resolution = resolution;
        numOfDivisions = Mathf.Max(0, resolution);
        numOfVertsPerFace = ((numOfDivisions + 3) * (numOfDivisions + 3) - (numOfDivisions + 3)) / 2;
        numOfTrisPerFace = (numOfDivisions + 1) * (numOfDivisions + 1);
        vertices = new FixedSizeList<Vector3>(10 * numOfDivisions * numOfDivisions + 20 * numOfDivisions + 12);
        triangles = new FixedSizeList<int>(numOfTrisPerFace * initialTriangles.Length);

        vertices.AddRange(initialVertices);

        Dictionary<string, int[]> edgeMap = new Dictionary<string, int[]>(30);
        int[] edgeAC, edgeAB, edgeCB;

        for (int i = 0; i < initialTriangles.Length; i+=3)
        {
            int vertexIndiceA = initialTriangles[i];
            int vertexIndiceB = initialTriangles[i+1];
            int vertexIndiceC = initialTriangles[i+2];

            edgeAC = CreateEdge(Mathf.Min(vertexIndiceA, vertexIndiceC), Mathf.Max(vertexIndiceA, vertexIndiceC), edgeMap, i);
            edgeAB = CreateEdge(Mathf.Min(vertexIndiceA, vertexIndiceB), Mathf.Max(vertexIndiceA, vertexIndiceB), edgeMap, i);          
            edgeCB = CreateEdge(Mathf.Min(vertexIndiceC, vertexIndiceB), Mathf.Max(vertexIndiceC, vertexIndiceB), edgeMap, i);

            edgeAC = CheckReverseEdge(edgeAC, vertexIndiceA);
            edgeAB = CheckReverseEdge(edgeAB, vertexIndiceA);
            edgeCB = CheckReverseEdge(edgeCB, vertexIndiceC);

            CreateFace(edgeAC, edgeAB, edgeCB);
        }
        Vertices = vertices.items;
        Triangles = triangles.items;
    }

    int[] CreateEdge(int firstVertex, int secondVertex, Dictionary<string, int[]> edgeMap, int edgeIndex)
    {
        int[] edge = new int[numOfDivisions + 2];
        edge[0] = firstVertex;
        edge[edge.Length - 1] = secondVertex;
        string edgeKey = $"{edge[0]}_{edge[edge.Length - 1]}";

        if (!edgeMap.ContainsKey(edgeKey))
        {
            for (int i = 0; i < numOfDivisions; i++)
            {
                float distBetweenVerts = (i + 1f) / (numOfDivisions + 1f);
                edge[i + 1] = vertices.nextIndex;
                vertices.Add(Vector3.Slerp(vertices.items[firstVertex], vertices.items[secondVertex], distBetweenVerts));
                
            }
            edgeMap.Add(edgeKey, edge);
        }

        else if (edgeMap.ContainsKey(edgeKey))
        {
            edge = edgeMap[edgeKey];
        }

        return edge;
    } // Creates or fetches edge based if it exists inside the dictionary

    int[] CheckReverseEdge(int[] edge, int vertex)
    {
        if (edge[0] != vertex)
        {
            System.Array.Reverse(edge);
        }
        return edge;
    } // Checks if the edge is lined up with the others, if not, reverses it

    void CreateFace(int[] sideA, int[] sideB, int[] sideC)
    {

        FixedSizeList<int> faceVertices = new FixedSizeList<int>(numOfVertsPerFace);
        int numOfPointsinEdge = (numOfDivisions + 2);

        faceVertices.Add(sideA[0]);
        for (int i = 1; i < numOfPointsinEdge - 1; i++) // Creates inner vertices
        {
            faceVertices.Add(sideA[i]);
            int numOfInnerPoints = i - 1;
            Vector3 sideAVertex = vertices.items[sideA[i]];
            Vector3 sideBVertex = vertices.items[sideB[i]];
            for (int j = 0; j < numOfInnerPoints; j++)
            {
                
                float distBetweenVerts = (j + 1f) / (numOfInnerPoints + 1f);
                faceVertices.Add(vertices.nextIndex);
                vertices.Add(Vector3.Slerp(sideAVertex, sideBVertex, distBetweenVerts));
            }
            faceVertices.Add(sideB[i]);

        }

        for (int i = 0; i < numOfPointsinEdge; i++) // Adds bottom vertices
        {
            faceVertices.Add(sideC[i]);
        }

        // Triangulation

        int numOfRows = numOfDivisions + 1;
        for (int row = 0; row < numOfRows; row++) // Loops through every new triangle row inside the face
        {   
            int topVertexIndice = (row * row + row) / 2;
            int bottomVertexIndice = (((row + 1) * (row + 1) + (row + 1)) / 2) + 1;
            int numOfTrisInRow = (row * 2) + 1;

            for (int tri = 0; tri < numOfTrisInRow; tri++) // Loops through every triangle in the row
            {
                int vertexIndiceA, vertexIndiceB, vertexIndiceC;

                if (tri % 2 == 0) // Checks whether the triangle is upside or not
                {
                    vertexIndiceA = topVertexIndice;
                    vertexIndiceB = bottomVertexIndice;
                    vertexIndiceC = bottomVertexIndice - 1;
                    topVertexIndice++;
                    bottomVertexIndice++;

                } else
                {
                    vertexIndiceA = topVertexIndice;
                    vertexIndiceB = bottomVertexIndice -1;
                    vertexIndiceC = topVertexIndice - 1;

                }

                triangles.Add(faceVertices.items[vertexIndiceA]);
                triangles.Add(faceVertices.items[vertexIndiceB]);
                triangles.Add(faceVertices.items[vertexIndiceC]);
            }
        }

    }


    // Convenience classes: - Sebastian Lague SphereMesh Episode 2
    public class FixedSizeList<T>
    {
        public T[] items;
        public int nextIndex;

        public FixedSizeList(int size)
        {
            items = new T[size];
        }

        public void Add(T item)
        {
            items[nextIndex] = item;
            nextIndex++;
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }

}
