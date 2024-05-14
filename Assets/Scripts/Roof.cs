using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Roof : MonoBehaviour
{
    private List<Vector3> bleachersPoints = MeshManipulation.bleachersPointsFromMesh;
    public Vector3 source;
    public Vector3 firstRoofPoint;
    public List<Vector3> roofPointsList;   // Start is called before the first frame update
    void Awake()
    {
        
        // bleachersPoints.AddRange(MeshManipulation.bleachersPointsFromMesh);
        Debug.Log("awake roof " + bleachersPoints.Count());

    }
    void Start()
    {
        roofPointsList.Add(firstRoofPoint);
        Debug.Log($"O número de pontos da platéia é{ roofPointsList[0]}");
        InstantiatePoint(source);
        InstantiatePoint( roofPointsList[0]);
        foreach (var point in bleachersPoints)
        {
            InstantiatePoint(point);
            Debug.Log(point);
        }

        CreateRoofFaces(bleachersPoints, roofPointsList, source);
        bleachersPoints.Clear();
        
    }

    private void CreateRoofFaces(List<Vector3> bleachersPointsList, List<Vector3> roofPoints, Vector3 source)
    {

        for(int i = 0; i< bleachersPointsList.Count -1; i++ )
        {
            Debug.Log($"i ={i}");

            //criar vetores auxiliares

            Vector3 sourceVec = SubtractionVectors(source, roofPoints[i]);
            Vector3 directionSource = sourceVec.normalized;
            float lengthRoofToSourceVec = sourceVec.magnitude;

            CreateLine(roofPoints[i], directionSource, lengthRoofToSourceVec );

            Vector3 bleachersVector = SubtractionVectors(bleachersPointsList[i], roofPoints[i] );
            Vector3 directionBlechers = bleachersVector.normalized;
            float lengthBleachers = bleachersVector.magnitude;

            CreateLine(roofPoints[i], directionBlechers, lengthBleachers );

            //cauculateAngle

            float bisecAngle = CalculateAngle(directionSource, directionBlechers);
            Debug.Log($"angulo entre vetores {bisecAngle}");

            //ImageVector

            Vector3 imageVector = CalculateImageVector(-directionBlechers, roofPoints[i], lengthRoofToSourceVec);
            InstantiatePoint( imageVector); 

            //create roof line
            
            float roofLength = 20f;
            
            Vector3 xAxisVector = new Vector3(0, 0, -1);

            float xAxisAngleWithBlechers = CalculateAngle(xAxisVector, directionBlechers); 
            float zAxisAngleWithBlechers = Vector3.Angle(xAxisVector, directionBlechers);
            Debug.Log("new z angle " + zAxisAngleWithBlechers);


            float roofAngleWithXAxis = 90 - (bisecAngle/2f) - xAxisAngleWithBlechers;
            Debug.Log("roofAngle: " + roofAngleWithXAxis + ", ZWithBleachers: "+ xAxisAngleWithBlechers + "bisec/2: " + bisecAngle/2f);
            
            Vector3 directionRoof = CreateVectorFromAngle(roofAngleWithXAxis ,  xAxisVector);
            //Vector3 directionRoof = CreateVectorFromAngle(roofAngleWithXAxis, directionBlechers);
            CreateLine(roofPoints[i], directionRoof, roofLength);

            // Find intersec point
            Vector3 imageToLastBleachersPoint = bleachersPointsList[bleachersPointsList.Count - 1] - imageVector;
            Vector3 imageDirection = imageToLastBleachersPoint.normalized;
            float lineLenght = imageToLastBleachersPoint.magnitude;
            CreateLine(imageVector, imageDirection, lineLenght);
            Vector3 intersecPoint = CalculateLineIntersection(directionRoof, imageDirection, roofPoints[i], imageVector);
            InstantiatePoint( intersecPoint);

            //add new point to roofPointsList

            roofPointsList.Add(intersecPoint);
            

        }
        
    }

    void InstantiatePoint(Vector3 position)
    {

        // Instanciar um prefab de ponto na posição especificada
        GameObject pointPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointPrefab.transform.position = position;
        pointPrefab.transform.localScale = new Vector3(0.5f , 0.5f, 0.5f);
    }

    public void CreateLine(Vector3 startPoint, Vector3 direction, float length)
    {
        // Criar um objeto vazio para armazenar a linha
        GameObject lineObject = new GameObject("Line");
        
        // Adicionar o componente LineRenderer ao objeto
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Definir as propriedades da linha
        lineRenderer.positionCount = 2; // Dois pontos para definir a linha
        lineRenderer.SetPosition(0, startPoint); // Definir o ponto inicial
        lineRenderer.SetPosition(1, startPoint + direction.normalized * length); // Definir o ponto final

        // Opcional: ajustar a largura da linha
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    Vector3 SubtractionVectors(Vector3 vectorA, Vector3 vectorB)
    {
        return vectorA - vectorB;
    }

    public float CalculateAngle(Vector3 vectorA, Vector3 vectorB)
    {
        float dotProduct = Vector3.Dot(vectorA, vectorB);
        
        float arcCosine = Mathf.Acos(dotProduct);

        float angleDeg = arcCosine * Mathf.Rad2Deg;
        
        return angleDeg;
    }

    public Vector3 CalculateImageVector(Vector3 direction, Vector3 roofPoint, float lenght)
    {
        Vector3 iVector = roofPoint + (direction * lenght);

        return iVector;
    }

   public Vector3 CreateVectorFromAngle(float angleDegrees, Vector3 direction)
    {
        // Converter o ângulo de graus para radianos
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Calcular as componentes x e y do novo vetor
        float newZ = Mathf.Cos(angleRadians);
        float newY = Mathf.Sin(angleRadians);

        // Criar e retornar o novo vetor
        Vector3 newVector = new Vector3(direction.x, newY, -newZ);
        Debug.Log(newVector);

        return newVector.normalized;
    }


    public Vector3 CalculateLineIntersection( Vector3 direction1, Vector3 direction2, Vector3 roofPoint, Vector3 imagePoint)
    {
        // Calcular os vetores cruzados
        Vector3 cross = Vector3.Cross(direction1, direction2);
        float denominator = direction1.z * direction2.y - direction1.y * direction2.z;

        // Verificar se os vetores são paralelos (vetor cruzado aproximadamente igual a zero)
        if (cross.sqrMagnitude < 0.0001f)
        {
            Debug.Log("As linhas são paralelas ou colineares. Não há interseção.");
            //return Vector3.positiveInfinity; // Retornar ponto de interseção indefinido
        }

        //cramer rule

        float numerator1 = (imagePoint.z - roofPoint.z) * direction2.y - (imagePoint.y - roofPoint.y) * direction2.z;
        float numerator2 = (imagePoint.z - roofPoint.z) * direction1.y - (imagePoint.y - roofPoint.y) * direction1.z;

        // Calcular os parâmetros t1 e t2
        float t1 = numerator1 / denominator;
        float t2 = numerator2 / denominator;
     
        // // Calcular o ponto de interseção
        Vector3 intersectionPoint = roofPoint + direction1 * t1;

        // Retornar o ponto de interseção
        return intersectionPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
