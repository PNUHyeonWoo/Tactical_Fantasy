using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBed : EventInteraction
{
    public void Start()
    {
        dis = 100;
    }
    public override void Interact()
    {
        GameManager.cameraManager.PlayUISound(1);
        GameManager.player.HP = GameManager.player.maxHP;
        GameManager.player.blood = 0;
        GameManager.player.poison = 0;
        GameManager.saveManager.Save(0);
    }
}
