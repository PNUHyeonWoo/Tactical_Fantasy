using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : AttackTarget
{
    public float maxHp;
    float hp;
    public float defense;
    bool isDead = false;
    public float damage;
    public float Poison;
    public float speed;
    float nowSpeed;

    public Vector2 destination = new Vector2();
    public Vector2 beforePos = new Vector2();
    public float stopCool = 5;
    public float attackCool = 3;

    AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip deadSound;

    int[] attackTarget = new int[1] { 0 };
    public Collider2D puke;
    public GameObject dead;
    public override HitInformation Hit(HitInformation hitInformation)
    {
        if (IsHitThisAttack(hitInformation))
        {
            if (hitInformation.isLow)
            {
                Vector2 des = hitInformation.destination;
                RaycastHit2D[] rayhits = Physics2D.RaycastAll(des, Vector2.zero);

                bool isHit = false;

                foreach(RaycastHit2D hit in rayhits)
                {
                    if(hit.transform.gameObject == gameObject)
                    {
                        isHit = true;
                        break;
                    }
                }

                if (isHit)//로우 타격 성공
                {
                    DefaultHit(ref hitInformation, ref hp, defense);

                    if(hp <= 0 && !isDead)
                    {
                        isDead = true;

                        GameObject deadObj = GameObject.Instantiate(dead, new Vector3(des.x, des.y, 3), Quaternion.Euler(0, 0, AI.VectorToDegree(hitInformation.hitDirect)));
                        deadObj.GetComponent<AudioSource>().clip = deadSound;
                        deadObj.GetComponent<AudioSource>().Play();
                        Destroy(gameObject);
                    }

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
    void Start()
    {
        destination = transform.position;
        beforePos = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        AI.DecreaseCool(ref attackCool);

        if(AI.CheckStop(ref stopCool,5,transform,ref beforePos,10))
            destination += new Vector2(Random.Range(-40, 40), Random.Range(-40, 40));

        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {

            if (Vector2.Distance(destination, transform.position) > 5)
            {
                AI.MoveToTargetWithFrontDetect(transform, destination, ref nowSpeed, speed, 0, speed, 5, 8, 40);
            }
            else
                destination += new Vector2(Random.Range(-40, 40), Random.Range(-40, 40));

            if (attackCool <= 0 && AI.CheckSightSensor(transform, GameManager.player.transform, 25, 30) >= 0)
            {
                GetComponent<Animator>().Play("Attack");
                attackCool = 3;
            }

        }

       

    }

    public void PukeAttack()
    {
        Attack.AttackWithCastAOE(puke,true, damage, 0,Attack.AttackEffect.Poison,Poison, gameObject, attackTarget, false);
        audioSource.clip = attackSound;
        audioSource.Play();
    }
}
