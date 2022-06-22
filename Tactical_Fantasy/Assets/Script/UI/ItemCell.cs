using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCell : MonoBehaviour,IPointerEnterHandler ,IPointerExitHandler
{ 
    public GameObject cellGroup;
    public int cellX;
    public int cellY;
    public ItemManager itemManager;


    void Start()
    {
        itemManager = GameManager.itemManager;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        itemManager.NowDestinationType = (int)ItemManager.TypeOfDestination.ContainerCell;
        itemManager.NowDestinationObject = this;

    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        itemManager.NowDestinationType = (int)ItemManager.TypeOfDestination.None;
        itemManager.NowDestinationObject = null;
    }
}


