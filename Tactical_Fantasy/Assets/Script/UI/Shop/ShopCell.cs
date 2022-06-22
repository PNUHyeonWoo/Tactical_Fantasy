using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Shop shop;
    void Start()
    {
       shop = transform.parent.parent.parent.parent.GetComponent<Shop>();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        shop.destinationCell = this;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        shop.destinationCell = null;
    }
}
