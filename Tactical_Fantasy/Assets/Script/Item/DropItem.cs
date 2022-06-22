using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{

    public static void CreateDropItem(int itemCode, int[] PLCCode, int amount)
    {
        Item item = Item.GetItem(itemCode);
        GameObject dropItem = GameObject.Instantiate(GameManager.itemManager.dropItemPrefabs);
        dropItem.GetComponent<Chest>().thisContainer = new Container(false, item.xsize, item.ysize, "");
        dropItem.GetComponent<Chest>().thisContainer.InsertItem(itemCode, PLCCode, amount, true, 0, 0);
        dropItem.transform.position = GameManager.player.transform.position + new Vector3(0,0,1);

   }
}
