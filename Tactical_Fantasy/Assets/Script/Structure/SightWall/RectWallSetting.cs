using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectWallSetting : MonoBehaviour
{
    public void SettingRectWall()
    {
        transform.Find("BlackSight/Mask").GetComponent<SpriteMask>().sprite = GetComponent<SpriteRenderer>().sprite;
        float width = GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2 - 0.3f;
        float height = GetComponent<SpriteRenderer>().sprite.bounds.size.y / 2 - 0.3f;
        GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<SpriteRenderer>().sprite.bounds.size.x -0.6f, GetComponent<SpriteRenderer>().sprite.bounds.size.y - 0.6f);
        transform.Find("BlackSight/Mask").transform.localScale = new Vector3(width /(width+0.3f), height / (height + 0.3f), 1);
        transform.Find("BlackSight/A").transform.localPosition = new Vector3(-width, height, 0);
        transform.Find("BlackSight/B").transform.localPosition = new Vector3(width, -height, 0);
        transform.Find("BlackSight/C").transform.localPosition = new Vector3(-width, -height, 0);
        transform.Find("BlackSight/D").transform.localPosition = new Vector3(width, height, 0);
    }
}
