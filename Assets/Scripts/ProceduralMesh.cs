using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ProceduralMesh : MonoBehaviour
{
    private Mesh stairMesh;
    private Vector3[] vertices;
    private int[] triangles;


    void Awake()
    {
        stairMesh = GetComponent<MeshFilter>().mesh;
    }
    // Start is called before the first frame update
    void Start()
    {
        MakeMeshData();
        CreateMesh();
        //Debug.Log($"Vertices length: {vertices.Length}");

    }


    private void MakeMeshData()
    {
        //create array of vertices
        vertices = new Vector3[]{new Vector3(2, 0, 4), new Vector3((float)6.421789, 4, (float)0.8), new Vector3(2, (float)0.8, 4)};
        //create an array of integers, defines a triangles
        triangles = new int[]{ 0,1,2 };
    }

    private void CreateMesh()
    {
        stairMesh.Clear();
        stairMesh.vertices = vertices;
        stairMesh.triangles = triangles;
    }



}

/*
{2, 4, 0}
{6.421789, 4, 0.8}
{2, 4, 0.8}



*/
