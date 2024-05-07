using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class MeshManipulation : MonoBehaviour
{
    private Mesh stairsMesh;
    private Vector3[] vertices;
    private int[] triangles;

    
    // Start is called before the first frame update
    void Awake()
    {
        stairsMesh = new Mesh();
        stairsMesh = GetComponent<MeshFilter>().mesh;

        if(stairsMesh != null)
        {
            CollectMeshData();
            Debug.Log("Inside true stairMesh");
           
        }else
        {
            Debug.Log("Nenhuma Mesh atrelada");
        }
    }
    // void Start()
    // {
    //     List<Vector3> listVertices = new List<Vector3>(vertices);
    //     stairsMesh.GetVertices(listVertices);
        

    //     // Debug.Log($"Vertices list length: {listVertices.Count()}");
    //     // Debug.Log($"Vertices  length: {vertices.Length}");
    //     // Debug.Log($"Triangles length: {triangles.Length}");
        
    // }
    
    void CollectMeshData()
    {
        vertices = stairsMesh.vertices;
        triangles = stairsMesh.triangles;
        Debug.Log("Inside true collect");
        Debug.Log($"Vertices length: {vertices.Length}");
        Debug.Log($"Triangles length: {triangles.Length}");
        
    }

    

    void GetVerticesOfAllTriangles()
    {
        Debug.Log("Inside Get Vertices");
        for(int i = 0; i <= stairsMesh.triangles.Count(); i +=1)
        {
            //int triangleIndex = i/3;
            int verticeIndex = triangles[i];
            // int verticeIndex2 = triangles[i + 1];
            // int verticeIndex3 = triangles[i + 2];

            Debug.Log($"Triangulo {verticeIndex} ");
            //Debug.Log($"Vertex1: {verticeIndex1} ");
            //Debug.Log($"Vertex2: {verticeIndex2} ");
            //Debug.Log($"Vertex3: {verticeIndex3} ");

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
