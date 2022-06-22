using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerGearCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    public int index;
    public ItemManager itemManager;

    void Start()
    {
        itemManager = GameManager.itemManager;
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        itemManager.NowDestinationType = (int)ItemManager.TypeOfDestination.PlayerGearCell;
        itemManager.NowDestinationObject = this;

    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        itemManager.NowDestinationType = (int)ItemManager.TypeOfDestination.None;
        itemManager.NowDestinationObject = null;
    }


}
