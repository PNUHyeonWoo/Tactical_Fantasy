using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class QuestCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Shop shop;
    public MerchantUI merchantUI;
    public GameObject questPage;
    public int localIndex;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        shop.destinationCell = this;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        shop.destinationCell = null;
    }

}
