using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDoor : Interaction
{
    public GameObject ShopPrefab;

    public void CreateShopUI()
    {
       GameObject shopUI = GameObject.Instantiate(ShopPrefab);
       shopUI.transform.SetParent(GameManager.itemManager.canvas);
       shopUI.transform.localPosition = new Vector3(0, 0, 0);
       GameManager.player.shop = shopUI.GetComponent<Shop>();

        if (GameManager.itemManager.NowStartType != 0)
        {
            GameManager.itemManager.FailMoveItem();
        }

        if (GameManager.player.itemMenu != null)
        {
            Destroy(GameManager.player.itemMenu);
            GameManager.player.itemMenu = null;
        }
    }
}
