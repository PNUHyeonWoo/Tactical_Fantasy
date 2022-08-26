using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshWall : MonoBehaviour
{
    MeshRenderer mr;
    MeshFilter mf;
    Mesh mesh;
    float x = 0;
    void Start()
    {
        mr = gameObject.GetComponent<MeshRenderer>();
        mf = gameObject.GetComponent<MeshFilter>();
        mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 0),
            new Vector3(-5, 10, 0),
            new Vector3(15, 10, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        mf.mesh = mesh;

    }

    // Update is called once per frame
    void Update()
    {
        x +=  10 * Time.deltaTime;
        Vector3[] vertices = new Vector3[4]
          {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 0),
            new Vector3(-5, 10, 0),
            new Vector3(15+x, 10+x, 0)
          };
        mesh.SetVertices(vertices);
    }
}
