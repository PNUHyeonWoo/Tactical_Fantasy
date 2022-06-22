using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleWallSetting : MonoBehaviour
{
    public float halfDiameter;
    public void SettingCircleWall()
    {
        GetComponent<CircleCollider2D>().radius = halfDiameter - 0.3f;
        transform.Find("BlackSight").GetComponent<CircleSightWall>().halfDiameter = halfDiameter - 0.3f;
        transform.Find("BlackSight/Mask").GetComponent<SpriteMask>().sprite = GetComponent<SpriteRenderer>().sprite;
        transform.Find("BlackSight/Mask").localScale = new Vector3((halfDiameter - 0.3f) / halfDiameter, (halfDiameter - 0.3f) / halfDiameter, 1);
    }
}
