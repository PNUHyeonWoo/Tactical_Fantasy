using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewSquareSightWall : MonoBehaviour
{
    [Header("Clock Rotate A B C")]

    public Transform PointA;
    public Transform PointB;
    public Transform PointC;
    public Transform PointD;

    GameObject meshObj;
    Mesh mesh;
    MeshFilter mf;

    public float paddingX = 2;
    public float paddingY = 2;

    void Start()
    {
        GameObject centerBox = GameObject.Instantiate(GameManager.itemManager.sightObject[4]);
        centerBox.transform.parent = transform;
        centerBox.transform.localPosition = new Vector3((PointA.localPosition.x+PointB.localPosition.x)/2, (PointA.localPosition.y + PointD.localPosition.y) / 2,0);
        centerBox.transform.position = new Vector3(centerBox.transform.position.x, centerBox.transform.position.y, SightWallCommon.blackSightZPos);
        centerBox.transform.localScale = new Vector3(PointB.localPosition.x - PointA.localPosition.x, PointA.localPosition.y - PointD.localPosition.y,1);
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
        Vector2 d = PointD.position;

        Vector2 ta = a - playerVector;
        Vector2 tb = b - playerVector;
        Vector2 tc = c - playerVector;
        Vector2 td = d - playerVector;

        float ab = Vector2.Angle(ta, tb);
        float ac = Vector2.Angle(ta, tc);
        float ad = Vector2.Angle(ta, td);
        float bc = Vector2.Angle(tb, tc);
        float bd = Vector2.Angle(tb, td);
        float cd = Vector2.Angle(tc, td);

        float[] angles = new float[6] { ab, ac, ad, bc, bd ,cd};
        switch (angles.ToList<float>().IndexOf(angles.Max()))
        {
            case 0:
                RenderBlackSight(a, b);
                break;
            case 1:
                RenderBlackSight(a, c);
                break;
            case 2:
                RenderBlackSight(a, d);
                break;
            case 3:
                RenderBlackSight(b, c);
                break;
            case 4:
                RenderBlackSight(b, d);
                break;
            case 5:
                RenderBlackSight(c, d);
                break;

        }
    }

    void RenderBlackSight(Vector2 a, Vector2 b)
    {
        SightWallCommon.RenderPlayerBlackSightWithLine(a, b, mf, mesh, transform.position);
    }

    public void WallObjButton() 
    {
        float width = GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2;
        float height = GetComponent<SpriteRenderer>().sprite.bounds.size.y / 2;


        GameObject A = new GameObject("A");
        A.transform.SetParent(transform);
        A.transform.localPosition = new Vector3(-width+paddingX, height-paddingY, 0);
        GameObject B = new GameObject("B");
        B.transform.SetParent(transform);
        B.transform.localPosition = new Vector3(width-paddingX, height-paddingY, 0);
        GameObject C = new GameObject("C");
        C.transform.SetParent(transform);
        C.transform.localPosition = new Vector3(width-paddingX, -height+paddingY, 0);
        GameObject D = new GameObject("D");
        D.transform.SetParent(transform);
        D.transform.localPosition = new Vector3(-width+paddingX, -height+paddingY, 0);

        PointA = A.transform;
        PointB = B.transform;
        PointC = C.transform;
        PointD = D.transform;

    }

}
