using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public int itemCode;
    public Item item;
    public MerchantUI merchantUI;
    
    public void ShopItemClick()
    {
        merchantUI.SelectItem(this);
    }
}
