using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseChest : MonoBehaviour
{
    public int savePLCcode;
    void Start()
    {
        GetComponent<Chest>().thisContainer = new Container(savePLCcode, true, 17, 14);
    }
}
