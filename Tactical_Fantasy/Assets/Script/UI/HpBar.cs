using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HpBar : MonoBehaviour
{
    public Transform hp;
    public Transform Stamina;

    public Sprite blood;
    public Sprite poison;

    public Image[] statusEffect;

    // Update is called once per frame
    void Update()
    {
        float nowHp = GameManager.player.HP / GameManager.player.maxHP;
        if (nowHp < 0)
            nowHp = 0;
        hp.localScale = new Vector3(nowHp, 1, 1);

        float nowStamina = GameManager.player.stamina / GameManager.player.maxStamina;
        if (nowStamina < 0)
            nowStamina = 0;
        Stamina.localScale = new Vector3(nowStamina, 1, 1);

        foreach (Image image in statusEffect)
            image.color = Color.clear;

        int i = 0;

        if(GameManager.player.poison > 0)
        {
            statusEffect[i].color = Color.white;
            statusEffect[i].sprite = poison;
            i++;
        }

        if (GameManager.player.blood > 0)
        {
            statusEffect[i].color = Color.white;
            statusEffect[i].sprite = blood;
            i++;
        }


    }
}
