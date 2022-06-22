using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthWrite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }


}
