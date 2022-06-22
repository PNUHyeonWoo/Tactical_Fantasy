using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAttackTarget : AttackTarget
{
    float hp = 0;
    public float defense = 0;
    public GameObject bloodPrefab = null;
    public Color32 bloodColor = new Color32(0,0,0,255);
    override public HitInformation Hit(HitInformation hitInformation)
    {
        if (IsHitThisAttack(hitInformation))
        {
            DefaultHit(ref hitInformation, ref hp, defense);

            if (bloodPrefab != null)
            {
                GameObject blood = GameObject.Instantiate(bloodPrefab);
                blood.transform.position = new Vector3(hitInformation.hitPoint.x, hitInformation.hitPoint.y, transform.position.z - 0.1f);
                blood.transform.rotation = Quaternion.Euler(0, 0, AI.VectorToDegree(hitInformation.hitDirect) + 180);
            }
            else
            {
                GameObject blood = GameObject.Instantiate(GameManager.itemManager.wallDefaultBloodPrefab);
                blood.transform.position = new Vector3(hitInformation.hitPoint.x, hitInformation.hitPoint.y, transform.position.z - 0.1f);
                blood.transform.rotation = Quaternion.Euler(0, 0, AI.VectorToDegree(hitInformation.hitDirect) + 180);
                blood.GetComponent<SpriteRenderer>().color = bloodColor;
            }

            return hitInformation;
        }
        else
        {
            return NoHit(hitInformation);
        }
    }
}
