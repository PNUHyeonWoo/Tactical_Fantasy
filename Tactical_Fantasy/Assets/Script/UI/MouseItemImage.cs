using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseItemImage : MonoBehaviour
{
    Transform mousePointer;
    void Start()
    {
        mousePointer = GameManager.mousePointer.transform;

        GameManager.cameraManager.PlayUISound(0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.SetSiblingIndex(mousePointer.GetSiblingIndex() - 1);
    }
}
