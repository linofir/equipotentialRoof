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
    private Vector3 xAxisUnitVector = new Vector3(1, 0, 0);
    private Vector3 yAxisUnitVector = new Vector3(0, 1, 0);
    private Vector3 zAxisUnitVector = new Vector3(0, 0, 1);
    private Dictionary<string, List<Vector3[]>> faceTriangles = new Dictionary<string, List<Vector3[]>>();
    private Dictionary<string, List<Vector3>> normalsOfTriangles = new Dictionary<string, List<Vector3>>(); 

    
    // Start is called before the first frame update
    void Awake()
    {
        // stairsMesh = new Mesh();
        stairsMesh = GetComponent<MeshFilter>().mesh;

        if(stairsMesh != null)
        {
            Debug.Log("Inside true stairMesh");
            CollectMeshData();
            ReOrganizeData();
           
        }else
        {
            Debug.Log("Nenhuma Mesh atrelada");
        }
    }
    void Start()
    {
        VisualizeTrianglesOfFace("threadFace");
        List<Vector3[]> triangles = faceTriangles["threadFace"];
        triangles.ForEach(vertex => VisualizeNormalsTrianglesOfFace("threadFace", vertex[0], vertex[1], vertex[2]));
    }
    
    void CollectMeshData()
    {
        vertices = stairsMesh.vertices;
        triangles = stairsMesh.triangles;
        normals = stairsMesh.normals;
    }

    public void PrintVerticesOfAllTriangle(Vector3[] triangle)
    {
        foreach (var vertex in triangle)
        {
            InstantiatePoint(vertex);
            
        }
    }

    public void ReOrganizeData()
    {
        for (int i = 0; i < triangles.Length; i++)
        {
            Vector3[] newTriangle = CollectVertexOfTriangle(i);
            Vector3 newNormal = CalculateNormalOfTriangle(newTriangle[0], newTriangle[1], newTriangle[2]);

            float normalAngleWithX = CalculateAngle(xAxisUnitVector, newNormal);
            float normalAngleWithY = CalculateAngle(yAxisUnitVector, newNormal);
            float normalAngleWithZ = CalculateAngle(zAxisUnitVector, newNormal);
            // Debug.Log($"normal de indice {i} o angulo com x é `{normalAngleWithX}");
            // Debug.Log($"normal de indice {i} o angulo com y é `{normalAngleWithY}");
            // Debug.Log($"normal de indice {i} o angulo com z é `{normalAngleWithZ}");

            if( normalAngleWithX == 0)
            {
                AddTriangleToFace("leftStructureFace", newTriangle);
                AddTriangleNormal("leftStructureFace", newNormal);
            }else if ( normalAngleWithX == 180)
            {
                AddTriangleToFace("rightStructureFace", newTriangle);
                AddTriangleNormal("rightStructureFace", newNormal);
            }

            if( normalAngleWithY == 180)
            {
                // Debug.Log($"normal de indice {i} pertence a  estrutura baixo");
                AddTriangleToFace("bottomStructureFace", newTriangle);
                AddTriangleNormal("bottomStructureFace", newNormal);

            }else if ( normalAngleWithY == 0)
            {
                // Debug.Log($"normal de indice {i} pertence a Thread");
                AddTriangleToFace("threadFace", newTriangle);
                AddTriangleNormal("threadFace", newNormal);
            }

            if( normalAngleWithZ == 0)
            {
                // Debug.Log($"normal de indice {i} pertence a riser ");
                AddTriangleToFace("riserFace", newTriangle);
                AddTriangleNormal("riserFace", newNormal);

            }else if ( normalAngleWithZ == 180)
            {
                // Debug.Log($"normal de indice {i} pertence a estrutura fundo");
                AddTriangleToFace("topStructureFace", newTriangle);
                AddTriangleNormal("topStructureFace", newNormal);
            }

            // - Tread 0° de unity , 90° z, 90° x
            // - Riser 90° de unity , 0° z, 90° x
            // - Estrutura lateral esquerda 90° y, 90° z, 0°x 
            // - Estrutura Direita 90° y, 90° z, 180°x
            // - Estrutura fundo 90° de unity , 180° z, 90° x
            // - Estrutura baixo 180 y, 90 z, 90 x 


        }
        
    }

    public Vector3 CalculateNormalOfTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        //Calculate  normal
        Vector3 baseVectorA = vertex1 -vertex2;
        Vector3 baseVectorB = vertex1 -vertex3;
            
        Vector3 newNormal = Vector3.Cross(baseVectorA, baseVectorB).normalized;
        Vector3 centroid = CalculateCentroid(vertex1, vertex2, vertex3);

        //InstantiatePoint(newNormal);
        // InstantiatePoint(centroid);
        Vector3 visualVec = newNormal + centroid;
        // InstantiateLine(centroid, visualVec);

        float angleX = CalculateAngle(newNormal, xAxisUnitVector);
        float angleY = CalculateAngle(newNormal, yAxisUnitVector);
        float angleZ = CalculateAngle(newNormal, zAxisUnitVector);

        // Debug.Log($"x= {angleX}, y={angleY}, z= {angleZ}");
            
        return newNormal;
    }

    public Vector3 CalculateCentroid(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        // Encontre os pontos médios dos lados do triângulo
        Vector3 midPoint1 = (vertex1 + vertex2) / 2f;
        Vector3 midPoint2 = (vertex2 + vertex3) / 2f;
        Vector3 midPoint3 = (vertex3 + vertex1) / 2f;

        // Calcule a centróide como a média dos pontos médios
        Vector3 centroid = (midPoint1 + midPoint2 + midPoint3) / 3f;

        return centroid;
    }

    public Vector3[] CollectVertexOfTriangle(int i)
    {
        int triangleIndex =  i / 3; // Calcula o índice do triângulo associado à normal
        int triangleOffset = triangleIndex * 3; // Calcula o deslocamento no array de triângulos
            
        // Obtém os índices dos vértices do triângulo atual
        int vertexIndex1 = triangles[triangleOffset];
        int vertexIndex2 = triangles[triangleOffset + 1];
        int vertexIndex3 = triangles[triangleOffset + 2];

        Vector3 vertex1 = vertices[vertexIndex1];
        Vector3 vertex2 = vertices[vertexIndex2];
        Vector3 vertex3 = vertices[vertexIndex3];

        Vector3[] newTriangle = new Vector3[3] { vertex1, vertex2, vertex3 };
        
        return newTriangle;
    }

    // void ClassifyTrianglesNormals()
    // {
    //     //for each vector check for the angle
    //     for( int i = 0; i < normals.Length; i++)
    //     {
    //         float normalAngleWithX = CalculateAngle(xAxisUnitVector, normals[i]);
    //         float normalAngleWithY = CalculateAngle(yAxisUnitVector, normals[i]);
    //         float normalAngleWithZ = CalculateAngle(zAxisUnitVector, normals[i]);
    //         // Debug.Log($"normal de indice {i} o angulo com x é `{normalAngleWithX}");
    //         // Debug.Log($"normal de indice {i} o angulo com y é `{normalAngleWithY}");
    //         // Debug.Log($"normal de indice {i} o angulo com z é `{normalAngleWithZ}");

    //         if( normalAngleWithX == 0)
    //         {
    //             Debug.Log($"angulox {normalAngleWithX}");
    //             Debug.Log($"anguloy {normalAngleWithY}");
    //             Debug.Log($"anguloz {normalAngleWithZ}");
    //             AddTriangleToFace("leftStructureFace", i);
    //             // AddTriangleNormal("leftStructureFace", i);
    //         }else if ( normalAngleWithX == 180)
    //         {
    //             // Debug.Log($"normal de indice {i} pertence a estrutura lateral direita");
    //             //AddTriangleToFace("rightStructureFace", i);
    //             // AddTriangleNormal("rightStructureFace", i);
    //         }

            // if( normalAngleWithZ == 180)
            // {
            //     // Debug.Log($"normal de indice {i} pertence a  estrutura baixo");
            //     AddTriangleToFace("bottomStructureFace", i);
            //     // AddTriangleNormal("bottomStructureFace", i);

            // }else if ( normalAngleWithZ == 0)
            // {
            //     // Debug.Log($"normal de indice {i} pertence a Thread");
            //     AddTriangleToFace("threadFace", i);
            //     // AddTriangleNormal("threadFace", i);
            // }

            // if( normalAngleWithY == 180)
            // {
            //     // Debug.Log($"normal de indice {i} pertence a riser ");
            //     AddTriangleToFace("riserFace", i);
            //     // AddTriangleNormal("riserFace", i);

            // }else if ( normalAngleWithY == 0)
            // {
            //     // Debug.Log($"normal de indice {i} pertence a estrutura fundo");
            //     AddTriangleToFace("topStructureFace", i);
            //     // AddTriangleNormal("topStructureFace", i);
            // }

            // - Tread 0° de unity , 90° z, 90° x
            // - Riser 90° de unity , 180° z, 90° x
            // - Estrutura lateral esquerda 90° y, 90° z, 180°x 
            // - Estrutura Direita 90° y, 90° z, 0°x
            // - Estrutura fundo 90° de unity , 0° z, 90° x
            // - Estrutura baixo 180 y, 90 z, 90 x 
  


        // }

        // group the triangle normals in a specific face( list) of the scructure
    // }

    public float CalculateAngle(Vector3 vectorA, Vector3 vectorB)
    {
        float dotProduct = Vector3.Dot(vectorA, vectorB);
        
        float arcCosine = Mathf.Acos(dotProduct);

        float angleDeg = arcCosine * Mathf.Rad2Deg;
        
        return angleDeg;
    }

    // void ShowNormalsByFace(List<Vector3> )

    // public Vector3[] CreateTrianglesListWithVertices( int i)
    // {
    //     int triangleIndex = i / 3; // Calcula o índice do triângulo associado à normal
    //     int triangleOffset = triangleIndex * 3; // Calcula o deslocamento no array de triângulos
        
    //     // Obtém os índices dos vértices do triângulo atual
    //     int vertexIndex1 = triangles[triangleOffset];
    //     int vertexIndex2 = triangles[triangleOffset + 1];
    //     int vertexIndex3 = triangles[triangleOffset + 2];
        
    //     // Exibe os índices dos vértices que formam o triângulo
    //     // Debug.Log("Normal " + i + " pertence ao triângulo com os índices: " +
    //     //           vertexIndex1 + ", " + vertexIndex2 + ", " + vertexIndex3);
    //     //FindTriangleVertices(vertexIndex1, vertexIndex2, vertexIndex3);
    //     Vector3 vertex1 = vertices[vertexIndex1];
    //     Vector3 vertex2 = vertices[vertexIndex2];
    //     Vector3 vertex3 = vertices[vertexIndex3];

    //     Vector3[] newTriangle = {vertex1,vertex2, vertex3};
    //     return newTriangle;
    // }


    public void AddTriangleToFace(string faceName, Vector3[] newTriangle)
    {
        List<Vector3[]> listOfTrianglesAtFace = new List<Vector3[]>{newTriangle};
        // Verifica se a face já existe no dicionário
        if (faceTriangles.ContainsKey(faceName))
        {
            // Se a face já existe, adiciona o triângulo à lista de triângulos dessa face
            faceTriangles[faceName].Add(newTriangle);
        }
        else
        {
            // Se a face ainda não existe, cria uma nova face o adiciona o triangulo
            faceTriangles.Add(faceName, listOfTrianglesAtFace);
        }
    }

    public void AddTriangleNormal(string faceName, Vector3 newNormal)
    {
        Vector3 normal = newNormal;
        List<Vector3> listOfNormalsAtFace = new List<Vector3>{newNormal};
        if (normalsOfTriangles.ContainsKey(faceName))
        {
            // Se a face já existe, adiciona o triângulo à lista de triângulos dessa face
            normalsOfTriangles[faceName].Add(newNormal); 
        }
        else
        {
            // Se a face ainda não existe, cria uma nova face o adiciona o triangulo
            normalsOfTriangles.Add(faceName, listOfNormalsAtFace);
        }

    }

    public void VisualizeTrianglesOfFace(string faceName)
    {
        if (faceTriangles.ContainsKey(faceName))
        {
            Debug.Log("Triângulos da face " + faceName + ":");
            List<Vector3[]> triangles = faceTriangles[faceName];
            foreach (Vector3[] vertex in triangles)
            {
                Debug.Log("Vertices do triângulo: " + vertex[0] + ", " + vertex[1] + ", " + vertex[2]);
                InstantiatePoint(vertex[0]);
                InstantiatePoint(vertex[1]);
                InstantiatePoint(vertex[2]);
            }
        }
        else
        {
            Debug.Log("A face " + faceName + " não existe no dicionário.");
        }
    }

    public void VisualizeNormalsTrianglesOfFace(string faceName, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        if (faceTriangles.ContainsKey(faceName))
        {
            Debug.Log("Triângulos da face " + faceName + ":");
            List<Vector3> normals = normalsOfTriangles[faceName];
            foreach (Vector3 normal in normals)
            {
                Debug.Log($"Vertices do triângulo:  + {normal}");
                Vector3 centroid = CalculateCentroid(vertex1, vertex2, vertex3);
                Vector3 visualVec = centroid + normal;
                InstantiateLine(centroid, visualVec);
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
        pointPrefab.transform.localScale = new Vector3(0.2f , 0.2f, 0.2f);

        //Acessar o renderer
        Renderer rend = pointPrefab.GetComponent<Renderer>();
    
        // Definir a cor do objeto
        rend.material.color = Color.red;
    }

    public void InstantiateLine(Vector3 startPoint, Vector3 endPoint)
    {
       // Crie um objeto para representar o vetor
        GameObject vectorObject = new GameObject("Vector");

        // Adicione um LineRenderer ao objeto
        LineRenderer lineRenderer = vectorObject.AddComponent<LineRenderer>();

        // Defina a largura da linha
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Defina os pontos inicial e final da linha
        lineRenderer.SetPositions(new Vector3[] { startPoint, endPoint });

        // Defina a cor da linha
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.green;
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
