using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriAngleSightWall : MonoBehaviour
{
    [Header("Clock Rotate A B C")]

    public Transform PointA;
    public Transform PointB;
    public Transform PointC;

    public bool isRect = false; 

    private GameObject centerBlackSight;
    private GameObject leftBlackSight;
    private GameObject rightBlackSight;

    private Transform leftCenterMask;
    private Transform rightCenterMask;

    private Transform centerLeftMask;
    private Transform rightLeftMask;

    private Transform centerRightMask;
    private Transform leftRightMask;

    private Transform[] triLeftMask;
    private Transform[] triRightMask;

    public const float blackSightZPos = -80;
    void Start()
    {
        centerBlackSight = GameObject.Instantiate(GameManager.itemManager.sightObject[0]);
        leftBlackSight = GameObject.Instantiate(GameManager.itemManager.sightObject[1]);
        rightBlackSight = GameObject.Instantiate(GameManager.itemManager.sightObject[1]);

        centerBlackSight.transform.SetParent(this.transform);
        leftBlackSight.transform.SetParent(this.transform);
        rightBlackSight.transform.SetParent(this.transform);

        centerBlackSight.transform.position = new Vector3(transform.position.x, transform.position.y, blackSightZPos);
        leftBlackSight.transform.position = new Vector3(transform.position.x, transform.position.y, blackSightZPos);
        rightBlackSight.transform.position = new Vector3(transform.position.x, transform.position.y, blackSightZPos);

        leftCenterMask = leftBlackSight.transform.Find("CenterMask");
        rightCenterMask = rightBlackSight.transform.Find("CenterMask");

        centerLeftMask = centerBlackSight.transform.Find("LeftMask");
        rightLeftMask = rightBlackSight.transform.Find("RightMask");

        centerRightMask = centerBlackSight.transform.Find("RightMask");
        leftRightMask = leftBlackSight.transform.Find("RightMask");

        triLeftMask = new Transform[3] { centerBlackSight.transform.Find("LeftTriMask"), leftBlackSight.transform.Find("LeftTriMask"), rightBlackSight.transform.Find("LeftTriMask") };
        triRightMask = new Transform[3] { centerBlackSight.transform.Find("RightTriMask"), leftBlackSight.transform.Find("RightTriMask"), rightBlackSight.transform.Find("RightTriMask") };

        centerBlackSight = centerBlackSight.transform.Find("CenterBlackSight").gameObject;
        leftBlackSight = leftBlackSight.transform.Find("LeftBlackSight").gameObject;
        rightBlackSight = rightBlackSight.transform.Find("LeftBlackSight").gameObject;

        if (!isRect)
        {

            Vector2 a = PointA.position;
            Vector2 b = PointB.position;
            Vector2 c = PointC.position;

            float aAngle = GetVectorAngle(b - a, c - a);
            float bAngle = GetVectorAngle(a - b, c - b);
            float cAngle = GetVectorAngle(a - c, b - c);

            if (aAngle >= bAngle && aAngle >= cAngle)
                CreateTriMask(a, b, c, bAngle, cAngle);
            else if (bAngle >= aAngle && bAngle >= cAngle)
                CreateTriMask(b, c, a, cAngle, aAngle);
            else
                CreateTriMask(c, a, b, aAngle, bAngle);
        }
        else
        {
            foreach (Transform cM in triLeftMask)
                Destroy(cM.gameObject);

            foreach (Transform cM in triRightMask)
                Destroy(cM.gameObject);
        }

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

        float ab = GetVectorAngle(ta, tb);
        float ac = GetVectorAngle(ta, tc);
        float bc = GetVectorAngle(tb, tc);
     


        if (ab >= ac && ab >= bc)
            RenderBlackSight(a, b);
        else if (ac >= ab && ac >= bc)
            RenderBlackSight(c, a);
        else
            RenderBlackSight(b, c);


    }

    public void RenderBlackSight(Vector2 a, Vector2 b)
    {
        Vector2 playerVector = GameManager.player.transform.position;
        Vector2 line = b - a;
        Vector2 cross;

 
        if (GetVectorAngle((playerVector - a), new Vector2(line.y, -line.x)) > 90)
        {
            cross = new Vector2(line.y, -line.x);
        }
        else
        {
            cross = new Vector2(-line.y, line.x);
        }

        centerBlackSight.transform.position = new Vector3((a.x + b.x) / 2, (a.y + b.y) / 2, 0);
        centerBlackSight.transform.localScale = new Vector3(Vector2.Distance(a, b) / 10, 1, 1);

        if (cross.x > 0)
            centerBlackSight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(cross.y / cross.x) * Mathf.Rad2Deg - 90);
        else if(cross.x < 0)
            centerBlackSight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(cross.y / cross.x) * Mathf.Rad2Deg + 90);
        else if(cross.y > 0)
            centerBlackSight.transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            centerBlackSight.transform.rotation = Quaternion.Euler(0, 0, 180);


        leftBlackSight.transform.position = new Vector3(a.x,a.y,0);

        leftBlackSight.transform.rotation = Quaternion.Euler(0, 0, centerBlackSight.transform.eulerAngles.z);
        

        float tan = Mathf.Tan(GetVectorAngle(a - playerVector, cross) * Mathf.Deg2Rad);
        
        if(Vector2.SignedAngle(a - playerVector, cross) < 0)
        leftBlackSight.transform.localScale = new Vector3(tan, 1, 1);
        else
        leftBlackSight.transform.localScale = new Vector3(-tan, 1, 1);



        rightBlackSight.transform.position = new Vector3(b.x, b.y, 0);

        rightBlackSight.transform.rotation = Quaternion.Euler(0, 0, centerBlackSight.transform.eulerAngles.z);

        float tanb = Mathf.Tan(GetVectorAngle(b - playerVector, cross) * Mathf.Deg2Rad);

        if (Vector2.SignedAngle(b - playerVector, cross) < 0)
            rightBlackSight.transform.localScale = new Vector3(tanb, 1, 1);
        else
            rightBlackSight.transform.localScale = new Vector3(-tanb, 1, 1);


        leftCenterMask.position = centerBlackSight.transform.position;
        leftCenterMask.rotation = centerBlackSight.transform.rotation;
        leftCenterMask.localScale = centerBlackSight.transform.localScale;
        rightCenterMask.position = centerBlackSight.transform.position;
        rightCenterMask.rotation = centerBlackSight.transform.rotation;
        rightCenterMask.localScale = centerBlackSight.transform.localScale;

        centerLeftMask.position = leftBlackSight.transform.position;
        centerLeftMask.rotation = leftBlackSight.transform.rotation;
        centerLeftMask.localScale = leftBlackSight.transform.localScale;
        rightLeftMask.position = leftBlackSight.transform.position;
        rightLeftMask.rotation = leftBlackSight.transform.rotation;
        rightLeftMask.localScale = leftBlackSight.transform.localScale;

        centerRightMask.position = rightBlackSight.transform.position;
        centerRightMask.rotation = rightBlackSight.transform.rotation;
        centerRightMask.localScale = rightBlackSight.transform.localScale;
        leftRightMask.position = rightBlackSight.transform.position;
        leftRightMask.rotation = rightBlackSight.transform.rotation;
        leftRightMask.localScale = rightBlackSight.transform.localScale;

    }

    public void CreateTriMask(Vector2 a, Vector2 b, Vector2 c,float bAngle,float cAngle)
    {
        float bTan = Mathf.Tan(bAngle * Mathf.Deg2Rad);
        float cTan = Mathf.Tan(cAngle * Mathf.Deg2Rad);

        float bc = cTan/bTan;

        Vector2 foot = (b + c * bc) / (bc + 1);

        Vector2 foot2a = a - foot;
        float r;


        if (foot2a.x > 0)
            r = Mathf.Atan(foot2a.y / foot2a.x) * Mathf.Rad2Deg - 90;
        else if (foot2a.x < 0)
            r = Mathf.Atan(foot2a.y / foot2a.x) * Mathf.Rad2Deg + 90;
        else if (foot2a.y > 0)
            r = 0;
        else
            r = 180;



        foreach (Transform nowMask in triLeftMask)
        {
            nowMask.position = new Vector3(a.x, a.y, 0);
            nowMask.rotation = Quaternion.Euler(0, 0, r);

            float yScale = Vector2.Distance(a, foot)/512;

            if (Vector2.SignedAngle(foot2a, b - foot) > 0)
                nowMask.localScale= new Vector3(yScale / bTan, -yScale, 1);
            else
                nowMask.localScale = new Vector3(-yScale / bTan, -yScale, 1);

        }

        foreach (Transform nowMask in triRightMask)
        {
            nowMask.position = new Vector3(a.x, a.y, 0);
            nowMask.rotation = Quaternion.Euler(0, 0, r);

            float yScale = Vector2.Distance(a, foot) / 512;

            if (Vector2.SignedAngle(foot2a, c - foot) > 0)
                nowMask.localScale = new Vector3(yScale / cTan, -yScale, 1);
            else
                nowMask.localScale = new Vector3(-yScale / cTan, -yScale, 1);

        }


    }


    public float GetVectorAngle(Vector2 a, Vector2 b)
    {

        return Vector2.Angle(a, b);

    }



}
