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
    private Vector3[] normals;
    private Dictionary<string, List<Vector3[]>> faceTriangles = new Dictionary<string, List<Vector3[]>>();
    private List<Vector3[]> leftStructureFaceNormals;
    private List<Vector3[]> rightStructureFaceNormals;
    private List<Vector3[]> topStructureFaceNormals;
    private List<Vector3[]> bottomStructureFaceNormals;
    private List<Vector3[]> threadFaceNormals;
    private List<Vector3[]> riserFaceNormals;

    
    // Start is called before the first frame update
    void Awake()
    {
        // stairsMesh = new Mesh();
        stairsMesh = GetComponent<MeshFilter>().mesh;

        if(stairsMesh != null)
        {
            Debug.Log("Inside true stairMesh");
            CollectMeshData();
            ClassifyTrianglesNormals();
           
        }else
        {
            Debug.Log("Nenhuma Mesh atrelada");
        }
    }
    void Start()
    {
        PrintFaceTriangles("threadFace");
    }
    
    void CollectMeshData()
    {
        vertices = stairsMesh.vertices;
        triangles = stairsMesh.triangles;
        normals = stairsMesh.normals;

        Debug.Log("Inside true collect");
        Debug.Log($"Vertices length: {vertices.Length}");
        Debug.Log($"Triangles length: {triangles.Length}");
        Debug.Log($"Normals length: {normals.Length}");
    }

    void ClassifyTrianglesNormals()
    {
        //for each vector check for the angle
        for( int i = 0; i < normals.Length; i++)
        {
            Vector3 xAxisUnitVector = new Vector3(1, 0, 0);
            Vector3 yAxisUnitVector = new Vector3(0, 1, 0);
            Vector3 zAxisUnitVector = new Vector3(0, 0, 1);

            float normalAngleWithX = CalculateAngle(xAxisUnitVector, normals[i]);
            float normalAngleWithY = CalculateAngle(yAxisUnitVector, normals[i]);
            float normalAngleWithZ = CalculateAngle(zAxisUnitVector, normals[i]);
            // Debug.Log($"normal de indice {i} o angulo com x é `{normalAngleWithX}");
            // Debug.Log($"normal de indice {i} o angulo com y é `{normalAngleWithY}");
            // Debug.Log($"normal de indice {i} o angulo com z é `{normalAngleWithZ}");

            if( normalAngleWithX == 180)
            {
                // Debug.Log($"normal de indice {i} pertence a estrutura lateral esquerda");
                AddTriangleToFace("leftStructureFace", i);
            }else if ( normalAngleWithX == 0)
            {
                // Debug.Log($"normal de indice {i} pertence a estrutura lateral direita");
                AddTriangleToFace("rightStructureFace", i);
            }

            if( normalAngleWithZ == 180)
            {
                // Debug.Log($"normal de indice {i} pertence a  estrutura baixo");
                AddTriangleToFace("bottomStructureFace", i);

            }else if ( normalAngleWithZ == 0)
            {
                // Debug.Log($"normal de indice {i} pertence a Thread");
                AddTriangleToFace("threadFace", i);
            }

            if( normalAngleWithY == 180)
            {
                // Debug.Log($"normal de indice {i} pertence a riser ");
                AddTriangleToFace("riserFace", i);

            }else if ( normalAngleWithY == 0)
            {
                // Debug.Log($"normal de indice {i} pertence a estrutura fundo");
                AddTriangleToFace("topStructureFace", i);
            }

            // - Tread 0° de unity , 90° z, 90° x
            // - Riser 90° de unity , 180° z, 90° x
            // - Estrutura lateral esquerda 90° y, 90° z, 180°x 
            // - Estrutura Direita 90° y, 90° z, 0°x
            // - Estrutura fundo 90° de unity , 0° z, 90° x
            // - Estrutura baixo 180 y, 90 z, 90 x 
  


        }

        // group the triangle normals in a specific face( list) of the scructure
    }

    public float CalculateAngle(Vector3 vectorA, Vector3 vectorB)
    {
        float dotProduct = Vector3.Dot(vectorA, vectorB);
        
        float arcCosine = Mathf.Acos(dotProduct);

        float angleDeg = arcCosine * Mathf.Rad2Deg;
        
        return angleDeg;
    }

    // void ShowNormalsByFace(List<Vector3> )

    public Vector3[] CreateTrianglesListWithVertices( int i)
    {
        int triangleIndex = i / 3; // Calcula o índice do triângulo associado à normal
        int triangleOffset = triangleIndex * 3; // Calcula o deslocamento no array de triângulos
        
        // Obtém os índices dos vértices do triângulo atual
        int vertexIndex1 = triangles[triangleOffset];
        int vertexIndex2 = triangles[triangleOffset + 1];
        int vertexIndex3 = triangles[triangleOffset + 2];
        
        // Exibe os índices dos vértices que formam o triângulo
        Debug.Log("Normal " + i + " pertence ao triângulo com os índices: " +
                  vertexIndex1 + ", " + vertexIndex2 + ", " + vertexIndex3);
        //FindTriangleVertices(vertexIndex1, vertexIndex2, vertexIndex3);
        Vector3 vertex1 = vertices[vertexIndex1];
        Vector3 vertex2 = vertices[vertexIndex2];
        Vector3 vertex3 = vertices[vertexIndex3];

        Vector3[] newTriangle = {vertex1,vertex2, vertex3};
        return newTriangle;
    }


    public void AddTriangleToFace(string faceName, int i)
    {
        Vector3[] newTriangle = CreateTrianglesListWithVertices(i);
        List<Vector3[]> listOfTrianglesOfFace = new List<Vector3[]>{newTriangle};
        // Verifica se a face já existe no dicionário
        if (faceTriangles.ContainsKey(faceName))
        {
            // Se a face já existe, adiciona o triângulo à lista de triângulos dessa face
            faceTriangles[faceName].AddRange(listOfTrianglesOfFace);
        }
        else
        {
            // Se a face ainda não existe, cria uma nova face o adiciona o triangulo
            faceTriangles.Add(faceName, listOfTrianglesOfFace);
        }
    }

    public void PrintFaceTriangles(string faceName)
    {
        if (faceTriangles.ContainsKey(faceName))
        {
            Debug.Log("Triângulos da face " + faceName + ":");
            List<Vector3[]> triangles = faceTriangles[faceName];
            foreach (Vector3[] triangle in triangles)
            {
                Debug.Log("Triângulo: " + triangle[0] + ", " + triangle[1] + ", " + triangle[2]);
                //InstantiatePoint(triangle[0][0]);
            }
        }
        else
        {
            Debug.Log("A face " + faceName + " não existe no dicionário.");
        }
    }

    void InstantiatePoint(Vector3 position)
    {

        // Instanciar um prefab de ponto na posição especificada
        GameObject pointPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointPrefab.transform.position = position;
        pointPrefab.transform.localScale = new Vector3(0.5f , 0.5f, 0.5f);
    }

    void FindTriangleVertices(int vertexIndex1, int vertexIndex2, int vertexIndex3)
    {    
        // Obtém os vértices do triângulo atual
        Vector3 vertex1 = vertices[vertexIndex1];
        Vector3 vertex2 = vertices[vertexIndex2];
        Vector3 vertex3 = vertices[vertexIndex3];
            
        // Exibe os vértices que formam o triângulo
        Debug.Log(" tem vértices: " +
                vertex1 + ", " + vertex2 + ", " + vertex3);
        
    }
    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
