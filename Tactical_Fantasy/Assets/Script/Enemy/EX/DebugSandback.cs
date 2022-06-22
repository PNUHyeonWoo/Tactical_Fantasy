using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSandback : AttackTarget
{
    public float hp = 100;
    public float defense = 2;
    override public HitInformation Hit(HitInformation hitInformation)
    {
        if (IsHitThisAttack(hitInformation))
        {
            DefaultHit(ref hitInformation, ref hp, defense);

            Debug.Log(hitInformation.hitResult.damage);

            return hitInformation;
        }
        else
        {
            return NoHit(hitInformation);
        }
    }
}
