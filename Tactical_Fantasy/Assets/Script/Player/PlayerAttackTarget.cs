using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackTarget : AttackTarget
{
    public Player player;
    public float defense = 2;

    public override HitInformation Hit(HitInformation hitInformation)
    {
        if (GameManager.player.isDead)
            return NoHit(hitInformation);

        if (IsHitThisAttack(hitInformation))
        {
            bool isCritical = DefaultHit(ref hitInformation, ref player.HP, defense);

            player.cameraShake = hitInformation.hitResult.damage / 100;

            if(hitInformation.attackEffect == Attack.AttackEffect.Blood)
            {
                if(isCritical)
                    GameManager.player.blood += hitInformation.effectPower / 100;
                else
                    GameManager.player.blood += hitInformation.effectPower / 200;

            }
            else if(hitInformation.attackEffect == Attack.AttackEffect.Fire)
            {

            }
            else if (hitInformation.attackEffect == Attack.AttackEffect.Poison)
            {
                GameManager.player.poison += hitInformation.effectPower;
            }

            if (player.HP <= 0)
            {
                if(GameManager.player.isStand)
                {
                    if (Vector2.SignedAngle(GameManager.player.transform.right, hitInformation.hitDirect) > 0)
                        GameManager.player.Dead(true, AI.VectorToDegree(hitInformation.hitDirect));
                    else
                        GameManager.player.Dead(false, AI.VectorToDegree(hitInformation.hitDirect));

                    GameManager.player.GetComponent<Rigidbody2D>().velocity = hitInformation.hitDirect.normalized * hitInformation.hitResult.damage * 3;

                }
                else
                {
                    if (GameManager.player.playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Prone_Aim") && GameManager.player.playerLeg.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                        GameManager.player.Dead(false, GameManager.player.playerLeg.transform.eulerAngles.z);
                    else
                        GameManager.player.Dead(true, GameManager.player.playerLeg.transform.eulerAngles.z);

                }              
            }
            else
            {
                if (hitInformation.hitResult.damage >= 10)
                    GameManager.player.PlayVocalSound(0, 5);

                float angle = Vector2.SignedAngle(hitInformation.hitPoint - (Vector2)transform.position, hitInformation.hitDirect);
            
                float rotateAngle = hitInformation.hitResult.damage * Mathf.Sin(angle * Mathf.Deg2Rad);

                Vector2 result = new Vector2();

                Vector2 SmousePos;
                SmousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                SmousePos.x = SmousePos.x - transform.position.x;
                SmousePos.y = SmousePos.y - transform.position.y;

                result.x = Mathf.Cos(rotateAngle * Mathf.Deg2Rad) * SmousePos.x - Mathf.Sin(rotateAngle * Mathf.Deg2Rad) * SmousePos.y;
                result.y = Mathf.Sin(rotateAngle * Mathf.Deg2Rad) * SmousePos.x + Mathf.Cos(rotateAngle * Mathf.Deg2Rad) * SmousePos.y;

                Player.CursorPos mouPos = new Player.CursorPos();
                Player.GetCursorPos(out mouPos);

                Vector2 screenC = new Vector2();

                screenC.x = Screen.width / GameManager.itemManager.canvas.sizeDelta.x;
                screenC.y = Screen.height / GameManager.itemManager.canvas.sizeDelta.y;

                screenC.x = (result - SmousePos).x * screenC.x;
                screenC.y = (result - SmousePos).y * screenC.y;

#if UNITY_EDITOR
                screenC *= GameManager.player.GetGameViewScale();
#endif

                mouPos.x += (int)screenC.x;
                mouPos.y -= (int)screenC.y;

                Player.SetCursorPos(mouPos.x, mouPos.y);




            }


            return hitInformation;
        }
        else
        {
            return NoHit(hitInformation);
        }
    }

}
