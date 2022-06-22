using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartridgeCase : MonoBehaviour
{
    public float life = 5f;
    public Vector2 move;
    public float moveLife = 0.15f;
    public bool isTargetZ = false;
    public float TargetZ = 0;

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
            Destroy(gameObject);

        if(moveLife > 0)
        {
            transform.Translate(600 * move * moveLife * Time.deltaTime);
            moveLife -= Time.deltaTime;
        }

        if (isTargetZ && moveLife < 0.08f)
            transform.position = new Vector3(transform.position.x, transform.position.y, TargetZ);
            

    }
}
