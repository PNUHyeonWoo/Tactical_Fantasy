using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{


    public static void Play(string animationName)
    {
        if(GameManager.player.selectGun == 0)
        {
            GameManager.player.GetComponent<Animation>().Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_" + animationName);
        }
        else if(GameManager.player.selectGun == 1)
        {
            GameManager.player.GetComponent<Animation>().Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_" + animationName);
        }
        else if(GameManager.player.selectGun == 2)
        {
            GameManager.player.GetComponent<Animation>().Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[2])).gunAnimation.ToString() + "_" + animationName);
        }
        else
        {
            GameManager.player.GetComponent<Animation>().Play("Knife" + "_" + animationName);
        }
    }
}
