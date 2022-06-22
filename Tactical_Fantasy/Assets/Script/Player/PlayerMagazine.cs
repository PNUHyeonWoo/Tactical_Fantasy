using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagazine : MonoBehaviour
{


    public void SetMagazineSprite()
    {
        string contents = GameManager.saveManager.savePLC.plusContainers[GameManager.itemManager.playerGear.GunsPLC[GameManager.player.selectGun]];
        int i = contents.Length - 1;

        while(contents[i] != 'p' && 0 < i)
        {
            i--;
        }

        if (i == 0)
            return;

        i -= 7;

        string itemCode = "";

        for(int j = 0; j < 7; j++ )
        {
            itemCode += contents[i];
            i++;
        }
        GetComponent<SpriteRenderer>().sprite = ((Magazine)(Item.GetItem(int.Parse(itemCode)))).MagazineSprite;

    }
}
