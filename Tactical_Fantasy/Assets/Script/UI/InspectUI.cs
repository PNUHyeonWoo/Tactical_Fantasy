using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            Destroy(this.gameObject);
        else if(!GameManager.player.CanGrabItem())
            Destroy(this.gameObject);

    }
}
