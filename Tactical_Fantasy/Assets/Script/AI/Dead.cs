using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead : MonoBehaviour
{
    public float moveTime;

    void Update()
    {
        if(AI.DecreaseCool(ref moveTime))
        {
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

            Destroy(this);
        }

    }


}
