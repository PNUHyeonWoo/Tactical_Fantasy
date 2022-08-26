using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : MonoBehaviour
{
    public int TargetID; // 0�� �÷��̾�
    public bool isHitFromPlayer;
    public Collider2D weakness = null;
    public struct HitInformation
    {
        public Vector2 hitPoint;
        public Vector2 hitDirect;
        public bool isLow;
        public Attack.AttackType attackType;
        public Collider2D attackColider;
        public Vector2 origin;
        public float maxDis;
        public Vector2 destination;

        public float damage;
        public float penetration; // ������� �����Ͻ� ��������ʾҴٶ�� �ǹ�
        public Attack.AttackEffect attackEffect;
        public float effectPower;

        public GameObject attackersColliderObject;
        public int[] canAttackTargetID; // length == 0 ��°��� �÷��̾��� �����̶�� �ǹ�
        public bool isAttackSelf;

        public Attack.HitResult hitResult;
    }
                
    virtual public HitInformation Hit(HitInformation hitInformation)
    {
        if(IsHitThisAttack(hitInformation))
        {
            //�������� Ÿ��ó���� �����(���� ��������) ���� �� hitresult�����Ͽ� hitinformation �����ֱ�
            hitInformation.penetration = -1;
            hitInformation.hitResult.damage = hitInformation.damage;
            hitInformation.hitResult.hitObject = gameObject;
            hitInformation.hitResult.isDie = false;
            hitInformation.hitResult.TargetID = TargetID;

            return hitInformation;
        }
        else
        {
            return NoHit(hitInformation);
        }
    }

    /* ���� �⺻���� ����
    override public HitInformation Hit(HitInformation hitInformation)
    {
        if (IsHitThisAttack(hitInformation))
        {
            DefaultHit(ref hitInformation, ref hp, defense);

            return hitInformation;
        }
        else
        {
            return NoHit(hitInformation);
        }
    }    
    
    �⺻���� only �÷��̾��� �ο� �ǰ�
    public override HitInformation Hit(HitInformation hitInformation)
    {
        if (IsHitThisAttack(hitInformation))
        {
            if (hitInformation.isLow)
            {
                Vector2 des = hitInformation.destination;
                RaycastHit2D[] rayhits = Physics2D.RaycastAll(des, Vector2.zero);

                bool isHit = false;

                foreach (RaycastHit2D hit in rayhits)
                {
                    if (hit.transform.gameObject == gameObject)
                    {
                        isHit = true;
                        break;
                    }
                }

                if (isHit)//�ο� Ÿ�� ����
                {
                    DefaultHit(ref hitInformation, ref hp, defense);
                    return hitInformation;
                }
                else
                    return NoHit(hitInformation);


            }
            else
                return NoHit(hitInformation);

        }
        else
        {
            return NoHit(hitInformation);
        }
    }
    */

    public bool IsHitThisAttack(HitInformation hitInformation)
    {
        if (hitInformation.attackersColliderObject == gameObject)
        {
            return hitInformation.isAttackSelf;
        }


        if (hitInformation.canAttackTargetID.Length == 0 && isHitFromPlayer)
            return true;
        else
        {
            foreach(int i in hitInformation.canAttackTargetID)
            {
                if (i == TargetID)
                    return true;

            }

            return false;
        }
    }

    public HitInformation NoHit(HitInformation hitInformation)
    {
        hitInformation.hitResult.TargetID = -1;
        return hitInformation;
    }

    public bool DefaultHit(ref HitInformation hitInformation,ref float hp,float defense)
    {
        bool beforeDead = false;
        if (hp <= 0)
            beforeDead = true;

        float[] damageResult = Attack.GetDamageAndPenetration(hitInformation.damage, hitInformation.penetration, defense);

        bool result = IsHitWeakness(hitInformation);

        if (!result)
            damageResult[0] /= 2;

        hp -= damageResult[0];

        hitInformation.penetration = damageResult[1];
        hitInformation.hitResult.damage = damageResult[0];
        if (hp > 0)
        {
            hitInformation.hitResult.hitObject = gameObject;
            hitInformation.hitResult.isDie = false;
        }
        else
        {
            hitInformation.hitResult.hitObject = null;
            if(!beforeDead)
                hitInformation.hitResult.isDie = true;
            else
                hitInformation.hitResult.isDie = false;
        }
        hitInformation.hitResult.TargetID = TargetID;


        return result;
    }

    public bool IsHitWeakness(HitInformation hitInformation)
    {
        if (weakness != null)
        {
            if(hitInformation.attackType == Attack.AttackType.Raycast)
            {
                RaycastHit2D[] rayhits = Physics2D.RaycastAll(hitInformation.hitPoint, hitInformation.hitDirect, hitInformation.maxDis - Vector2.Distance(hitInformation.origin,hitInformation.hitPoint));

                foreach(RaycastHit2D hit in rayhits)
                {
                    if (hit.transform.GetComponent<Collider2D>() == weakness)
                        return true;
                }
                return false;

            }
            else if(hitInformation.attackType == Attack.AttackType.ColiderCast)
            {
                return hitInformation.attackColider.IsTouching(weakness);
            }

            return true;
        }
        else
            return true;

    }



}
