using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Roof : MonoBehaviour
{
    public List<Vector3> bleachersPoints = new List<Vector3>();
    public Vector3 source;
    public Vector3 firstRoofPoint;
    public List<Vector3> roofPointsList;   // Start is called before the first frame update
    void Start()
    {
        roofPointsList.Add(firstRoofPoint);
        Debug.Log($"O número de pontos da platéia é{ roofPointsList[0]}");

        foreach (var point in bleachersPoints)
        {
            InstantiatePoint(point);
        }
        InstantiatePoint(source);
        InstantiatePoint( roofPointsList[0]);

        //criar método que calcula cada ponto do teto
        //CreateRoofFaces();

       

            // //criar vetores auxiliares

            // Vector3 sourceVec = SubtractionVectors(source, roofPointsList[0]);
            // Vector3 directionSource = sourceVec.normalized;
            // float lengthRoofToSourceVec = sourceVec.magnitude;

            // CreateLine(roofPointsList[0], directionSource, lengthRoofToSourceVec );

            // Vector3 bleachersVector = SubtractionVectors(bleachersPoints[0], roofPointsList[0] );
            // Vector3 directionBlechers = bleachersVector.normalized;
            // float lengthBleachers = bleachersVector.magnitude;

            // CreateLine(roofPointsList[0], directionBlechers, lengthBleachers );

            // //cauculateAngle

            // float bisecAngle = CalculateAngle(directionSource, directionBlechers);
            // Debug.Log($"angulo entre vetores {bisecAngle}");

            // //ImageVector

            // Vector3 imageVector = CalculateImageVector(-directionBlechers, roofPointsList[0], lengthRoofToSourceVec);
            // InstantiatePoint( imageVector); 

            // //create roof line
            
            // float roofLength = 20f;
            
            // Vector3 xAxisVector = new Vector3(1, 0, 0);

            // float xAxisAngleWithBlechers = CalculateAngle(xAxisVector, directionBlechers); 
            // // Debug.Log($"Angulo entre o eixo x = {xAxisAngleWithBlechers}");
            // // Vector3 bleachersProjection = CreateVectorFromAngle(-xAxisAngleWithBlechers, directionBlechers);
            // // CreateLine(roofPointsList[0], bleachersProjection.normalized, roofLength);

            // float roofAngleWithXAxis = 90 - (bisecAngle/2) - xAxisAngleWithBlechers;

            // Vector3 directionRoof = CreateVectorFromAngle(roofAngleWithXAxis, directionBlechers);
            // CreateLine(roofPointsList[0], directionRoof, roofLength);

            // // Find intersec point
            // Vector3 imageToLastBleachersPoint = bleachersPoints[4] - imageVector;
            // Vector3 imageDirection = imageToLastBleachersPoint.normalized;
            // float lineLenght = imageToLastBleachersPoint.magnitude;
            // CreateLine(imageVector, imageDirection, lineLenght);
            // Vector3 intersecPoint = CalculateLineIntersection(directionRoof, imageDirection, roofPointsList[0], imageVector);
            // InstantiatePoint( intersecPoint);

        
    }

    private void CreateRoofFaces(List<Vector3> bleachersPointsList, List<Vector3> roofPointsList, Vector3 sourceVec)
    {

        for(int i = 0; i< bleachersPointsList.Count -1; i++ )
        {
            Debug.Log($"i ={i}");

            //criar vetores auxiliares

            sourceVec = SubtractionVectors(source, roofPointsList[i]);
            Vector3 directionSource = sourceVec.normalized;
            float lengthRoofToSourceVec = sourceVec.magnitude;

            CreateLine(roofPointsList[i], directionSource, lengthRoofToSourceVec );

            Vector3 bleachersVector = SubtractionVectors(bleachersPointsList[i], roofPointsList[i] );
            Vector3 directionBlechers = bleachersVector.normalized;
            float lengthBleachers = bleachersVector.magnitude;

            CreateLine(roofPointsList[i], directionBlechers, lengthBleachers );

            //cauculateAngle

            float bisecAngle = CalculateAngle(directionSource, directionBlechers);
            Debug.Log($"angulo entre vetores {bisecAngle}");

            //ImageVector

            Vector3 imageVector = CalculateImageVector(-directionBlechers, roofPointsList[i], lengthRoofToSourceVec);
            InstantiatePoint( imageVector); 

            //create roof line
            
            float roofLength = 20f;
            
            Vector3 xAxisVector = new Vector3(1, 0, 0);

            float xAxisAngleWithBlechers = CalculateAngle(xAxisVector, directionBlechers); 
            // Debug.Log($"Angulo entre o eixo x = {xAxisAngleWithBlechers}");
            // Vector3 bleachersProjection = CreateVectorFromAngle(-xAxisAngleWithBlechers, directionBlechers);
            // CreateLine(roofPointsList[i], bleachersProjection.normalized, roofLength);

            float roofAngleWithXAxis = 90 - (bisecAngle/2) - xAxisAngleWithBlechers;

            Vector3 directionRoof = CreateVectorFromAngle(roofAngleWithXAxis, directionBlechers);
            CreateLine(roofPointsList[i], directionRoof, roofLength);

            // Find intersec point
            Vector3 imageToLastBleachersPoint = bleachersPointsList[bleachersPointsList.Count - 1] - imageVector;
            Vector3 imageDirection = imageToLastBleachersPoint.normalized;
            float lineLenght = imageToLastBleachersPoint.magnitude;
            CreateLine(imageVector, imageDirection, lineLenght);
            Vector3 intersecPoint = CalculateLineIntersection(directionRoof, imageDirection, roofPointsList[i], imageVector);
            InstantiatePoint( intersecPoint);
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
        // Calcular o produto escalar dos vetores
        float dotProduct = Vector3.Dot(vectorA, vectorB);
        //Debug.Log($"Resultado dotProd = {dotProduct}");

        // Calcular o arccoseno do ângulo entre os vetores com os vetores normalizados
        float arcCosine = Mathf.Acos(dotProduct);
        //Debug.Log($"arc coseno é = {arcCosine}");

        // Converter o ângulo de radianos para graus
        float angleDeg = arcCosine * Mathf.Rad2Deg;
        //Debug.Log($"anulo em deg = {angleDeg}");

        // Retornar o ângulo entre os vetores em graus
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
        float newX = Mathf.Cos(angleRadians);
        float newY = Mathf.Sin(angleRadians);

        // Criar e retornar o novo vetor
        Vector3 newVector = new Vector3(newX, newY, direction.z);

        return newVector.normalized;
    }


    public Vector3 CalculateLineIntersection( Vector3 direction1, Vector3 direction2, Vector3 roofPoint, Vector3 imagePoint)
    {
        // Calcular os vetores cruzados
        Vector3 cross = Vector3.Cross(direction1, direction2);
        float denominator = direction1.x * direction2.y - direction1.y * direction2.x;

        // Verificar se os vetores são paralelos (vetor cruzado aproximadamente igual a zero)
        if (cross.sqrMagnitude < 0.0001f)
        {
            Debug.Log("As linhas são paralelas ou colineares. Não há interseção.");
            //return Vector3.positiveInfinity; // Retornar ponto de interseção indefinido
        }

        Debug.Log("não são paralelos");

        // Calcular os vetores entre os pontos
        // Vector3 p1p2 = point2 - point1;
        // Vector3 p2p1 = point1 - point2;

        // // Calcular os parâmetros t1 e t2
        // float t1;
        // float t2 = roofPoint.x - imagePoint.x + direction1.x * ()

        float numerator1 = (imagePoint.x - roofPoint.x) * direction2.y - (imagePoint.y - roofPoint.y) * direction2.x;
        float numerator2 = (imagePoint.x - roofPoint.x) * direction1.y - (imagePoint.y - roofPoint.y) * direction1.x;

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
