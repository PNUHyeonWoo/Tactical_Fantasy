using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttachmentItemCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public ItemManager itemManager;

    void Start()
    {
        itemManager = GameManager.itemManager;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        itemManager.NowDestinationType = (int)ItemManager.TypeOfDestination.AttachmentCell;
        itemManager.NowDestinationObject = this;

    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        itemManager.NowDestinationType = (int)ItemManager.TypeOfDestination.None;
        itemManager.NowDestinationObject = null;
    }

}
