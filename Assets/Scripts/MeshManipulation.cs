using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MeshManipulation : MonoBehaviour
{
    private Mesh stairsMesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector3[] normals;
    private Vector3 orignPoint = new Vector3(0, 0, 0);
    private Vector3 xAxisUnitVector = new Vector3(1, 0, 0);
    private Vector3 yAxisUnitVector = new Vector3(0, 1, 0);
    private Vector3 zAxisUnitVector = new Vector3(0, 0, 1);
    private Dictionary<string, List<Vector3[]>> faceTriangles = new Dictionary<string, List<Vector3[]>>();
    private Dictionary<string, List<Vector3>> normalsOfTriangles = new Dictionary<string, List<Vector3>>(); 
    public static List<Vector3> bleachersPointsFromMesh = new List<Vector3>();

    void Awake()
    {
        // stairsMesh = new Mesh();
        stairsMesh = GetComponent<MeshFilter>().mesh;
        OriginReference();

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
        // VisualizeTrianglesOfFace("threadFace");
        // List<Vector3[]> triangles = faceTriangles["threadFace"];
        // triangles.ForEach(vertex => VisualizeNormalsTrianglesOfFace("threadFace", vertex[0], vertex[1], vertex[2]));
        // OrganizeThreadsOfStairs(0);
        // CalculateMidPointOfThread(1);
        CreateBleachersPointsFromMeshList();
    }
    void CollectMeshData()
    {
        vertices = stairsMesh.vertices;
        triangles = stairsMesh.triangles;
        normals = stairsMesh.normals;
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
                AddTriangleToFace("bottomStructureFace", newTriangle);
                AddTriangleNormal("bottomStructureFace", newNormal);

            }else if ( normalAngleWithY == 0)
            {
                AddTriangleToFace("threadFace", newTriangle);
                AddTriangleNormal("threadFace", newNormal);
            }

            if( normalAngleWithZ == 0)
            {
                AddTriangleToFace("riserFace", newTriangle);
                AddTriangleNormal("riserFace", newNormal);

            }else if ( normalAngleWithZ == 180)
            {
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
        Vector3 baseVectorA = vertex1 -vertex2;
        Vector3 baseVectorB = vertex1 -vertex3;
        Vector3 newNormal = Vector3.Cross(baseVectorA, baseVectorB).normalized;

        return newNormal;
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
    public void CreateBleachersPointsFromMeshList()
    {
        Dictionary<int, List<Vector3[]>> organizedThreadList = OrganizeThreadsOfStairs();
        // foreach (var key in organizedThreadList.Keys)
        // { 
        //     PrintVerticesOfTriangle(organizedThreadList[key]);
        // }
        // PrintVerticesOfTriangle(organizedThreadList[1]);
        // PrintVerticesOfTriangle(organizedThreadList[4]);

        organizedThreadList.Keys.ToList().ForEach(key => bleachersPointsFromMesh.Add(CalculateMidPointOfThread(key)));
       
        foreach (Vector3 point in bleachersPointsFromMesh)
        {
            InstantiatePoint(point);
        }
    }
    public Vector3 CalculateMidPointOfThread(int selectedThread)
    {
        List<Vector3[]> thread = OrganizeThreadsOfStairs()[selectedThread];
        List<Vector3[]> lefttStructureTriangles = faceTriangles["leftStructureFace"];
        List<Vector3[]> rightStructureTriangles = faceTriangles["rightStructureFace"];

        List<Vector3> leftCommonVertices = new List<Vector3>();
        List<Vector3> rightCommonVertices = new List<Vector3>();

        leftCommonVertices = thread.SelectMany(threadTriangle => lefttStructureTriangles.SelectMany(leftTriangle =>
            threadTriangle.Where(vertex => leftTriangle.Contains(vertex)))).Distinct().ToList();

        rightCommonVertices = thread.SelectMany(threadTriangle => rightStructureTriangles.SelectMany(rightTriangle =>
            threadTriangle.Where(vertex => rightTriangle.Contains(vertex)))).Distinct().ToList();

        List<Vector3> retangleVertices = leftCommonVertices.Union(rightCommonVertices).OrderBy(vertex => vertex.magnitude).ToList();
        InstantiatePoint(retangleVertices[2]);
        InstantiatePoint(retangleVertices[1]);
        Vector3 diagonal1 = retangleVertices[2] - retangleVertices[1]; // Diagonal do retângulo

        Vector3 midPoint = retangleVertices[1] + diagonal1 * 0.5f ;
        InstantiatePoint(midPoint);
        return midPoint;
    }
    public Dictionary<int, List<Vector3[]>> OrganizeThreadsOfStairs()
    {
        List<Vector3[]> desorganizedThreadList = faceTriangles["threadFace"];

        var trianglesOderedByYAxis = desorganizedThreadList.GroupBy( triangle => triangle.Select(vertex => vertex[1]).Average())
        .OrderBy(group => group.Key)
        .Select((group, index) => new { Key = index, Group = group.ToList()});

        //converte tupla em dicionário
        Dictionary<int, List<Vector3[]>> groupedTriangles = trianglesOderedByYAxis.ToDictionary(item => item.Key, item => item.Group);

        //List<Vector3[]> thread = groupedTriangles[selectedThread];
        return groupedTriangles;
    }
    public List<Vector3[]> OrganizeRiserOfStairs(int selectedRiser)
    {
        List<Vector3[]> desorganizedRiserList = faceTriangles["riserFace"];

        var trianglesOderedByZAxis = desorganizedRiserList.GroupBy( triangle => triangle.Select(vertex => vertex[2]).Average())
        .OrderByDescending(group => group.Key)
        .Select((group, index) => new { Key = index, Group = group.ToList()});

        //converte tupla em dicionário
        var groupedTriangles = trianglesOderedByZAxis.ToDictionary(item => item.Key, item => item.Group);

        List<Vector3[]> riser = groupedTriangles[selectedRiser];
        return riser;

    }
    public void PrintVerticesOfTriangle(List<Vector3[]> triangle)
    {
        foreach (var vertex in triangle)
        {
            Debug.Log("Vertices do triângulo: " + vertex[0] + ", " + vertex[1] + ", " + vertex[2]);
            InstantiatePoint(vertex[0]);
            InstantiatePoint(vertex[1]);
            InstantiatePoint(vertex[2]);
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
    void OriginReference()
    {
        InstantiatePoint(orignPoint);
        InstantiateLine(orignPoint, xAxisUnitVector);
        InstantiateLine(orignPoint, yAxisUnitVector);
        InstantiateLine(orignPoint, zAxisUnitVector);

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
    public float CalculateAngle(Vector3 vectorA, Vector3 vectorB)
    {
        float dotProduct = Vector3.Dot(vectorA, vectorB);
        float arcCosine = Mathf.Acos(dotProduct);
        float angleDeg = arcCosine * Mathf.Rad2Deg;
        return angleDeg;
    }
    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
