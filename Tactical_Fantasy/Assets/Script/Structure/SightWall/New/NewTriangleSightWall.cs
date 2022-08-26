using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTriangleSightWall : MonoBehaviour
{
    //사용법 : 오브젝트에 이 스크립트를 넣고 포인트만 채워주면 된다. 스프라이트로 중앙을 덮고싶으면 더 낮은 Z값으로 덮으면 된다. (BLACK SIGHT의 Z값은 부모 오브젝트 상관없이 절대위치로 지정된다.)

    [Header("Clock Rotate A B C")]

    public Transform PointA;
    public Transform PointB;
    public Transform PointC;

    GameObject meshObj;
    Mesh mesh;
    MeshFilter mf;

 
    void Start()
    {
        meshObj = new GameObject("black Sight");
        meshObj.transform.parent = transform;
        meshObj.transform.localPosition = new Vector3(0, 0, 0);
        meshObj.transform.position = new Vector3(meshObj.transform.position.x, meshObj.transform.position.y, SightWallCommon.blackSightZPos);
        meshObj.AddComponent<MeshRenderer>().material = GameManager.gameManager.blackSightMaterial;
        mf = meshObj.AddComponent<MeshFilter>();
        mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            0, 2, 1,
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
        Vector2 playerVector = GameManager.player.transform.position;
        Vector2 a = PointA.position;
        Vector2 b = PointB.position;
        Vector2 c = PointC.position;

        Vector2 ta = a - playerVector;
        Vector2 tb = b - playerVector;
        Vector2 tc = c - playerVector;

        float ab = Vector2.Angle(ta, tb);
        float ac = Vector2.Angle(ta, tc);
        float bc = Vector2.Angle(tb, tc);



        if (ab >= ac && ab >= bc)
            RenderBlackSight(a, b);
        else if (ac >= ab && ac >= bc)
            RenderBlackSight(c, a);
        else
            RenderBlackSight(b, c);
    }

    void RenderBlackSight(Vector2 a, Vector2 b)
    {
        SightWallCommon.RenderPlayerBlackSightWithLine(a, b, mf, mesh,transform.position);
    }

}
