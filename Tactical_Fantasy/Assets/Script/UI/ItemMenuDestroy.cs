using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMenuDestroy : MonoBehaviour, IPointerDownHandler
{
   

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.player.CanGrabItem())
        {
            GameManager.player.itemMenu = null;
            Destroy(this.gameObject);
        }

    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        GameManager.player.itemMenu = null;
        Destroy(this.gameObject);
    }
}
