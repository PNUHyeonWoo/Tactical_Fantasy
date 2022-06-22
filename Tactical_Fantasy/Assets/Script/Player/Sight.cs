using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public Sprite[] sightSprite;

    enum SightSprite
    {
        Sight = 0,
        ZoomSight = 1

    }
    
    public void SetSight(int i)
    {
        GetComponent<SpriteRenderer>().sprite = sightSprite[i];
    }
}
