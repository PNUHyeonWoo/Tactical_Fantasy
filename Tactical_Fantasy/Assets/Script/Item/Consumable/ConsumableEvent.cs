using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableEvent : MonoBehaviour
{
    public static bool result = false;
    public static void HealPlayer(float a)
    {
        if (GameManager.player.HP == GameManager.player.maxHP)
        {
            result = false;
            return;
        }


        GameManager.player.HP += a;
        if (GameManager.player.HP > GameManager.player.maxHP)
            GameManager.player.HP = GameManager.player.maxHP;

        result = true;

    }

    public static void HealPlayerBlood(float a)
    {
        if(GameManager.player.blood <= 0)
        {
            result = false;
            return;
        }

        GameManager.player.blood -= a;
        if (GameManager.player.blood < 0)
            GameManager.player.blood = 0;

        result = true;

    }

    public static void HealPlayerPoison(float a)
    {
        if (GameManager.player.poison <= 0)
        {
            result = false;
            return;
        }

        GameManager.player.poison -= a;
        if (GameManager.player.poison < 0)
            GameManager.player.poison = 0;

        result = true;

    }
}
