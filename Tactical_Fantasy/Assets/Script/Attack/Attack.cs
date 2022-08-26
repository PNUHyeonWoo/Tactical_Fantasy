using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Attack : MonoBehaviour
{
    public struct HitResult
    {
        public GameObject hitObject;
        public int TargetID; // 음수값이면 안맞았다는 뜻
        public bool isDie;
        public float damage;
    }

    public enum AttackType
    {
        Raycast = 0, 
        ColiderCast = 1,
        Projectile = 2
    }

    public enum AttackEffect
    {
        None = 0,
        Blood = 1,
        Fire = 2,
        Poison = 3
    }

   static public HitResult[] AttackWithRayCast(float originX,float originY,float directX,float directY,bool isLow,float damage,float penetration,GameObject attackerColiderObject,int[] canAttackTargetID,bool isAtackSelf, float range)
   {
        List<HitResult> hitResults = new List<HitResult>();

        AttackTarget.HitInformation hitInformation = new AttackTarget.HitInformation();
        hitInformation.hitPoint = new Vector2(originX, originY);
        hitInformation.hitDirect =  new Vector2(directX, directY);
        hitInformation.hitDirect.Normalize();
        hitInformation.isLow = isLow;
        hitInformation.attackType = AttackType.Raycast;
        hitInformation.attackColider = null;
        hitInformation.origin = new Vector2(originX, originY);
        hitInformation.maxDis = range;
        hitInformation.destination = hitInformation.origin + (hitInformation.hitDirect * range);
        hitInformation.damage = damage;
        hitInformation.penetration = penetration;
        hitInformation.attackEffect = AttackEffect.None;
        hitInformation.effectPower = 0;
        hitInformation.attackersColliderObject = attackerColiderObject;
        hitInformation.canAttackTargetID = canAttackTargetID;
        hitInformation.isAttackSelf = isAtackSelf;
        hitInformation.hitResult = new HitResult();

        RaycastHit2D[] rayhits = Physics2D.RaycastAll(hitInformation.hitPoint, hitInformation.hitDirect, range).OrderBy(h => h.distance).ToArray();

        int i = 0;

        while (i<rayhits.Length && hitInformation.penetration >= 0)
        {
            AttackTarget nowAttackTarget = rayhits[i].transform.GetComponent<AttackTarget>();

            if(nowAttackTarget != null)
            {              
                hitInformation.hitPoint = rayhits[i].point;
                hitInformation = nowAttackTarget.Hit(hitInformation);

                if (hitInformation.hitResult.TargetID >= 0)
                    hitResults.Add(hitInformation.hitResult);

            }

            i++;


        }


        return hitResults.ToArray();
   }

    static public HitResult[] AttackWithRayCast(float originX, float originY, float directX, float directY, bool isLow, float damage, float penetration,AttackEffect attackEffect,float effectPower ,GameObject attackerColiderObject, int[] canAttackTargetID, bool isAtackSelf, float range)
    {
        List<HitResult> hitResults = new List<HitResult>();

        AttackTarget.HitInformation hitInformation = new AttackTarget.HitInformation();
        hitInformation.hitPoint = new Vector2(originX, originY);
        hitInformation.hitDirect = new Vector2(directX, directY);
        hitInformation.hitDirect.Normalize();
        hitInformation.isLow = isLow;
        hitInformation.attackType = AttackType.Raycast;
        hitInformation.attackColider = null;
        hitInformation.origin = new Vector2(originX, originY);
        hitInformation.maxDis = range;
        hitInformation.destination = hitInformation.origin + (hitInformation.hitDirect * range);
        hitInformation.damage = damage;
        hitInformation.penetration = penetration;
        hitInformation.attackEffect = attackEffect;
        hitInformation.effectPower = effectPower;
        hitInformation.attackersColliderObject = attackerColiderObject;
        hitInformation.canAttackTargetID = canAttackTargetID;
        hitInformation.isAttackSelf = isAtackSelf;
        hitInformation.hitResult = new HitResult();

        RaycastHit2D[] rayhits = Physics2D.RaycastAll(hitInformation.hitPoint, hitInformation.hitDirect, range).OrderBy(h => h.distance).ToArray();

        int i = 0;

        while (i < rayhits.Length && hitInformation.penetration >= 0)
        {
            AttackTarget nowAttackTarget = rayhits[i].transform.GetComponent<AttackTarget>();

            if (nowAttackTarget != null)
            {
                hitInformation.hitPoint = rayhits[i].point;
                hitInformation = nowAttackTarget.Hit(hitInformation);

                if (hitInformation.hitResult.TargetID >= 0)
                    hitResults.Add(hitInformation.hitResult);

            }

            i++;


        }


        return hitResults.ToArray();
    }


    static public HitResult[] AttackWithCastAOE(Collider2D colider,bool isLow, float damage, float penetration, GameObject attackerColiderObject, int[] canAttackTargetID, bool isAtackSelf)
    {
        List<HitResult> hitResults = new List<HitResult>();

        AttackTarget.HitInformation hitInformation = new AttackTarget.HitInformation();
        hitInformation.isLow = isLow;
        hitInformation.attackType = AttackType.ColiderCast;
        hitInformation.attackColider = colider;
        hitInformation.origin = Vector2.zero;
        hitInformation.maxDis = 0;
        hitInformation.damage = damage;
        hitInformation.penetration = penetration;
        hitInformation.attackEffect = AttackEffect.None;
        hitInformation.effectPower = 0;
        hitInformation.attackersColliderObject = attackerColiderObject;
        hitInformation.canAttackTargetID = canAttackTargetID;
        hitInformation.isAttackSelf = isAtackSelf;
        hitInformation.hitResult = new HitResult();

        RaycastHit2D[] hits = new RaycastHit2D[10];

        int length = colider.Cast(Vector2.zero, hits, 0);

        if(length > 10)
        {
            hits = new RaycastHit2D[length];
            colider.Cast(Vector2.zero, hits, 0);
        }      
        
        for(int i = 0; i < length;i++)
        {
            AttackTarget nowAttackTarget = hits[i].transform.GetComponent<AttackTarget>();
            

            if (nowAttackTarget != null)
            {
                hitInformation.hitPoint = hits[i].point;
                hitInformation.hitDirect = hits[i].point - (Vector2)colider.transform.position;
                hitInformation = nowAttackTarget.Hit(hitInformation);
                hitInformation.penetration = penetration;

                if (hitInformation.hitResult.TargetID >= 0)
                    hitResults.Add(hitInformation.hitResult);

            }

        }

        return hitResults.ToArray();

    }

    static public HitResult[] AttackWithCastAOE(Collider2D colider, bool isLow, float damage, float penetration, AttackEffect attackEffect, float effectPower, GameObject attackerColiderObject, int[] canAttackTargetID, bool isAtackSelf)
    {
        List<HitResult> hitResults = new List<HitResult>();

        AttackTarget.HitInformation hitInformation = new AttackTarget.HitInformation();
        hitInformation.isLow = isLow;
        hitInformation.attackType = AttackType.ColiderCast;
        hitInformation.attackColider = colider;
        hitInformation.origin = Vector2.zero;
        hitInformation.maxDis = 0;
        hitInformation.damage = damage;
        hitInformation.penetration = penetration;
        hitInformation.attackEffect = attackEffect;
        hitInformation.effectPower = effectPower;
        hitInformation.attackersColliderObject = attackerColiderObject;
        hitInformation.canAttackTargetID = canAttackTargetID;
        hitInformation.isAttackSelf = isAtackSelf;
        hitInformation.hitResult = new HitResult();

        RaycastHit2D[] hits = new RaycastHit2D[10];

        int length = colider.Cast(Vector2.zero, hits, 0);

        if (length > 10)
        {
            hits = new RaycastHit2D[length];
            colider.Cast(Vector2.zero, hits, 0);
        }

        for (int i = 0; i < length; i++)
        {
            AttackTarget nowAttackTarget = hits[i].transform.GetComponent<AttackTarget>();


            if (nowAttackTarget != null)
            {
                hitInformation.hitPoint = hits[i].point;
                hitInformation.hitDirect = hits[i].point - (Vector2)colider.transform.position;
                hitInformation = nowAttackTarget.Hit(hitInformation);
                hitInformation.penetration = penetration;

                if (hitInformation.hitResult.TargetID >= 0)
                    hitResults.Add(hitInformation.hitResult);

            }

        }

        return hitResults.ToArray();

    }


    static public float[] GetDamageAndPenetration(float damage,float penetration, float defense)
    {
        float[] result = new float[2];
        float pen = penetration - defense;

        if(pen < -3) //-4이하
        {
            result[0] = damage / 10;
            result[1] = -1;
        }
        else if(pen < -2)//-3
        {
            result[0] = damage / 5;
            result[1] = -1;
        }
        else if (pen < -1)//-2
        {
            result[0] = damage / 3;
            result[1] = -1;
        }
        else if (pen < 0)//-1
        {
            result[0] = damage / 2;
            result[1] = -1;
        }
        else if (pen < 1)//0
        {
            result[0] = damage;
            result[1] = -1;
        }
        else if (pen < 2)//1
        {
            result[0] = damage;
            result[1] = penetration - 4;
        }
        else if (pen < 3)//2
        {
            result[0] = damage;
            result[1] = penetration - 3;
        }
        else if (pen < 4)//3
        {
            result[0] = damage;
            result[1] = penetration - 2;
        }
        else//4이상
        {
            result[0] = damage;
            result[1] = penetration - 1;
        }

        return result;
    }

}
