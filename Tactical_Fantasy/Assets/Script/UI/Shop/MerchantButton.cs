using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MerchantButton : MonoBehaviour
{
    Shop shop;

    public int index;
    public int needQuestIndex;
    public Sprite open;

    public int[] intimacys;
    public int[] ShopItems;


    public void Start()
    {
        shop = transform.parent.parent.GetComponent<Shop>();

        if (GetComponent<Image>().sprite == open)
            return;
      
        if (needQuestIndex == -1 || GameManager.saveManager.GetIsQuestClear(needQuestIndex))
        {
            GetComponent<Image>().sprite = open;
            GetComponent<Button>().onClick.AddListener(MerchantButtonClick);
        }
    }

    void MerchantButtonClick()
    {
        if (GameManager.PauseAmount > 0)
            return;

        shop.RenderMerchant(this);
    }
}
