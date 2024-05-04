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
        //stairsMesh.Clear();
        if(stairsMesh != null)
        {
            //StartCoroutine(CollectMeshDataCoroutine());
            CollectMeshData();
            //Debug.Log("Inside true stairMesh");
            // using (var dataArray = Mesh.AcquireReadOnlyMeshData(stairsMesh))
            // {
            //     var data = dataArray[0];
            //     // prints "2"
            //     Debug.Log(data.vertexCount);
            //     var gotVertices = new NativeArray<Vector3>(stairsMesh.vertexCount, Allocator.TempJob);
            //     data.GetVertices(gotVertices);
            //     // prints "(1.0, 1.0, 1.0)" and "(0.0, 0.0, 0.0)"
            //     foreach (var v in gotVertices)  
            //         Debug.Log(v);
            //     gotVertices.Dispose();  
            // }
        }else
        {
            Debug.Log("Nenhuma Mesh atrelada");
        }
    }
    void Start()
    {
        List<Vector3> listVertices = new List<Vector3>(vertices);

        stairsMesh.GetVertices(listVertices);
        

        // Debug.Log($"Vertices list length: {listVertices.Count()}");
        // Debug.Log($"Vertices  length: {vertices.Length}");
        // Debug.Log($"Triangles length: {triangles.Length}");
        
    }

    // IEnumerator CollectMeshDataCoroutine()
    // {
    //     // Wait until end of frame to ensure mesh data is fully initialized
    //     yield return new WaitForEndOfFrame();

    //     // Collect mesh data
    //     CollectMeshData();

    //     // Output mesh data to console
    //     Debug.Log("Inside true stairMesh");
    //     Debug.Log($"Vertices length: {vertices.Length}");
    //     Debug.Log($"Triangles length: {triangles.Length}");
    // }
    
    void CollectMeshData()
    {
        vertices = stairsMesh.vertices;
        triangles = stairsMesh.triangles;
        
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
