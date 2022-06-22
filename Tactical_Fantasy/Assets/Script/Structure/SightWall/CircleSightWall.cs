using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSightWall : MonoBehaviour
{
    public float halfDiameter;

    private GameObject centerBlackSight;
    private GameObject leftBlackSight;
    private GameObject rightBlackSight;

    void Start()
    {
        centerBlackSight = GameObject.Instantiate(GameManager.itemManager.sightObject[2]);
        leftBlackSight = GameObject.Instantiate(GameManager.itemManager.sightObject[3]);
        rightBlackSight = GameObject.Instantiate(GameManager.itemManager.sightObject[3]);

        centerBlackSight.transform.SetParent(this.transform);
        leftBlackSight.transform.SetParent(this.transform);
        rightBlackSight.transform.SetParent(this.transform);

        centerBlackSight.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        leftBlackSight.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        rightBlackSight.transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        centerBlackSight.transform.localScale = new Vector3(halfDiameter / 5, 1, 1);

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerVector = GameManager.player.transform.position;
        Vector2 circle = transform.position;

        Vector2 p2c = circle - playerVector;


        if (p2c.x > 0)
            centerBlackSight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(p2c.y / p2c.x) * Mathf.Rad2Deg - 90);
        else if (p2c.x < 0)
            centerBlackSight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(p2c.y / p2c.x) * Mathf.Rad2Deg + 90);
        else if (p2c.y > 0)
            centerBlackSight.transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            centerBlackSight.transform.rotation = Quaternion.Euler(0, 0, 180);


        float dis = Vector2.Distance(playerVector, circle);

        dis = dis - halfDiameter;
        if (dis < 0.1f)
            dis = 0.1f;

        float comeAngle = halfDiameter * 30f / dis;

        if (comeAngle > 45)
            comeAngle = 45;


        Vector2 left = new Vector2(-p2c.y, p2c.x);
        left = Quaternion.Euler(0, 0, comeAngle) * new Vector3(left.x, left.y, 0);

        left.Normalize();

        left = circle + left * halfDiameter;
        Vector2 p2l = left - playerVector;

        leftBlackSight.transform.position = new Vector3(left.x, left.y, 0);
        leftBlackSight.transform.rotation = centerBlackSight.transform.rotation;
        leftBlackSight.transform.localScale = new Vector3(Mathf.Tan(Vector2.Angle(p2c, p2l) * Mathf.Deg2Rad), 1, 1);


        Vector2 right = new Vector2(p2c.y, -p2c.x);
        right = Quaternion.Euler(0, 0, -comeAngle) * new Vector3(right.x, right.y, 0);
        right.Normalize();

        right = circle + right * halfDiameter;
        Vector2 p2r = right - playerVector;

        rightBlackSight.transform.position = new Vector3(right.x, right.y, 0);
        rightBlackSight.transform.rotation = centerBlackSight.transform.rotation;
        rightBlackSight.transform.localScale = new Vector3(-Mathf.Tan(Vector2.Angle(p2c, p2r) * Mathf.Deg2Rad), 1, 1);


    }
}
