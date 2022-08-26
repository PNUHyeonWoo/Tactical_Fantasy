using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightWallCommon
{

    public const float blackSightZPos = -80;
    public const float blackSightDis = 10000;
    public static void RenderPlayerBlackSightWithLine(Vector2 a, Vector2 b , MeshFilter mf, Mesh mesh, Vector3 pt)
    {
        Vector3 a3 = new Vector3(a.x, a.y);
        Vector3 b3 = new Vector3(b.x, b.y);
        Vector2 p2 = GameManager.player.transform.position;
        Vector3 p3 = new Vector3(p2.x, p2.y);

        a3 = a3 - p3;
        b3 = b3 - p3;

        if (0 < Vector3.Cross(a3, b3).z)
        {
            p2 = a;
            a = b;
            b = p2;

            p3 = a3;
            a3 = b3;
            b3 = p3;
        }

        Vector2 ad = a3;
        Vector2 bd = b3;
        ad.Normalize();
        bd.Normalize();

        ad = a + (ad * blackSightDis);
        bd = b + (bd * blackSightDis);

        pt = new Vector3(pt.x, pt.y, 0);

        Vector3[] vertices = new Vector3[4]
       {
            new Vector3(a.x, a.y, 0) - pt,
            new Vector3(b.x, b.y, 0) - pt,
            new Vector3(ad.x, ad.y, 0) - pt,
            new Vector3(bd.x, bd.y, 0) - pt
       };


        mesh.SetVertices(vertices);
        mf.mesh = mesh;
    }
}
