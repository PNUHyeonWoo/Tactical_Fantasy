using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : AttackTarget , SoundSensor
{
    public GameObject bloodPrefabs;
    public GameObject[] DeadPrefabs;
    AudioSource audioSource;
    AudioSource hitAudioSource;
    public AudioClip runSound;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip lickHitSound;
    public AudioClip deadSound;

    public float maxHp;
    float hp;
    public float damage;
    public float defense;
    bool isDead = false;

    public float acceleration;
    public float minSpeed;
    public float slowSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float turnSpeed;
    float nowSpeed;
    AI.LastObtainedPath lastObtainedPath = new AI.LastObtainedPath(0);

    public float detectWidth;
    public float sightDis;
    public float sightAngle;
    public float detectPlayer = 0;
    public float detectCool = 0;
    public Vector2 lastPlayerPosition = new Vector2();

    public const float suspicion = 0.4f;
    public const float battle = 0.7f;

    public float detectPlus = 5;
    public float detectSub = 0.05f;

    public float checkSecond = 3;
    public Vector2 beforPos = new Vector2();

    public Collider2D head;
    public int[] attackTargets = new int[1] { 0 };
    public float attackSecond = 3;

    public WolfGroup group = null;

    public override HitInformation Hit(HitInformation hitInformation)
    {
        if (IsHitThisAttack(hitInformation))
        {
            bool isHitWeakness = DefaultHit(ref hitInformation, ref hp, defense);

            if (isHitWeakness)
            {
                hitAudioSource.clip = hitSound;
            }
            else
                hitAudioSource.clip = lickHitSound;

            hitAudioSource.Play();


            detectPlayer = 1;
            detectCool = 10;
            lastPlayerPosition = GameManager.player.transform.position;

            GameObject blood = GameObject.Instantiate(bloodPrefabs);
            blood.transform.position = new Vector3(hitInformation.hitPoint.x, hitInformation.hitPoint.y, transform.position.z - 0.1f);
            blood.transform.rotation = Quaternion.Euler(0, 0, AI.VectorToDegree(hitInformation.hitDirect) + 180);

            if (hp <= 0 && !isDead)
            {
                isDead = true;

                if (group != null)
                    group.setDetect();

                if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    if(nowSpeed < 50)
                    {                        
                        GameObject dead = AI.CreateDead(DeadPrefabs[0], new Vector3(transform.position.x, transform.position.y, 2), hitInformation.hitDirect, hitInformation.hitDirect.normalized * 30, 2,isHitWeakness,hitSound,lickHitSound,deadSound);
                        if (Vector2.SignedAngle(transform.up, hitInformation.hitDirect) > 0)
                            dead.transform.Rotate(new Vector3(0, 180, 0));

                        Destroy(gameObject);
                    }
                    else
                    {
                        AI.CreateDead(DeadPrefabs[1], new Vector3(transform.position.x, transform.position.y, 2), transform.up, transform.up * nowSpeed, 1, isHitWeakness, hitSound, lickHitSound, deadSound);
                        Destroy(gameObject);
                    }

                }
                else
                {
                    AI.CreateDead(DeadPrefabs[1], new Vector3(transform.position.x, transform.position.y, 2), transform.up, GetComponent<Rigidbody2D>().velocity, 1, isHitWeakness, hitSound, lickHitSound, deadSound);
                    Destroy(gameObject);
                }



            }

            return hitInformation;
        }
        else
        {
            return NoHit(hitInformation);
        }
    }

    public void Hear(AI.HearInformation hearInformation)
    {
        if(hearInformation.targetID == 0)
        {
            detectPlayer += hearInformation.loudness / 10;
            if (detectPlayer > 1)
                detectPlayer = 1;
            detectCool = 0;
            if (hearInformation.loudness > 1)
                lastPlayerPosition = GameManager.player.transform.position;

        }
    }

    void Start()
    {
        lastPlayerPosition = transform.position;
        beforPos = transform.position;
        hp = maxHp;
        audioSource = GetComponent<AudioSource>();
        hitAudioSource = transform.Find("Head").GetComponent<AudioSource>();
    }
    void Update()
    {
        if (GameManager.PauseAmount > 0)
            return;

        AI.DecreaseCool(ref detectCool);
        AI.DecreaseCool(ref attackSecond);

        if (AI.CheckStop(ref checkSecond, 3, transform, ref beforPos, 15))
            AI.SetRandomDestination(ref lastPlayerPosition, 80, 80);

        bool isDetect = false;
        float detectDis = AI.CheckSightSensor(transform, GameManager.player.transform, sightDis, sightAngle);    
        if(detectDis >= 0)
        {
            AI.IncreaseDetectWithSight(ref detectPlayer, detectPlus, detectDis, sightDis);
            detectCool = 10;
            if(detectPlayer > suspicion)
            lastPlayerPosition = GameManager.player.transform.position;
            isDetect = true;
        }
        else
            AI.DecreaseDetect(ref detectPlayer, detectSub, detectCool, suspicion, battle);

        if (group == null)
        {

            if (detectPlayer >= battle)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = runSound;
                    audioSource.Play();
                }
            }
        }
        else
        {
            if (detectPlayer >= battle)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = runSound;
                    audioSource.PlayDelayed(group.wolfs.IndexOf(this) * 0.5f);
                }
            }

        }

        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (detectPlayer < suspicion)
            {
                if (Vector2.Distance(lastPlayerPosition, transform.position) > 10)
                {
                    AI.MoveToTargetWithAstar(transform, lastPlayerPosition, ref nowSpeed, acceleration, minSpeed, slowSpeed, turnSpeed, detectWidth, 150,ref lastObtainedPath);
                    GetComponent<Animator>().speed = nowSpeed / 40;
                }
                else
                    AI.SetRandomDestination(ref lastPlayerPosition, 80, 80);
            }
            else if (detectPlayer < battle)
            {
                if (Vector2.Distance(lastPlayerPosition, transform.position) > 10)
                {                    
                    AI.MoveToTargetWithAstar(transform, lastPlayerPosition, ref nowSpeed, acceleration, minSpeed, walkSpeed, turnSpeed, detectWidth, 150,ref lastObtainedPath);
                    GetComponent<Animator>().speed = nowSpeed / 40;
                }
                else if (!isDetect)
                    AI.SetRandomDestination(ref lastPlayerPosition, 80, 80);
            }
            else
            {
                if (Vector2.Distance(lastPlayerPosition, transform.position) < 100 && attackSecond < 0 && AI.CheckSightSensor(transform, GameManager.player.transform, 100, 20) >= 0)
                {
                    GetComponent<Animator>().speed = 1;
                    GetComponent<Animator>().Play("Attack");
                }
                else
                {

                    if (Vector2.Distance(lastPlayerPosition, transform.position) > 10)
                    {
                        AI.MoveToTargetWithAstar(transform, lastPlayerPosition, ref nowSpeed, acceleration, minSpeed, runSpeed, turnSpeed, detectWidth, 150, ref lastObtainedPath);
                        GetComponent<Animator>().speed = nowSpeed / 40;
                    }
                    else if (!isDetect)
                        AI.SetRandomDestination(ref lastPlayerPosition, 80, 80);

                }
            }

        }

        if (Input.GetKeyDown(KeyCode.N))
            Debug.Log(group.wolfs.IndexOf(this));

    }

    void DashStart()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * 300;
    }

    void DashEnd()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        attackSecond = 3;
    }

    void HeadAttack()
    {
        Attack.HitResult[] results = Attack.AttackWithCastAOE(head, false, damage, 4,Attack.AttackEffect.Blood,damage, gameObject, attackTargets, false);
        foreach(Attack.HitResult hit in results)
        {
            if(hit.TargetID == 0)
            {
                GameManager.player.PlayHitSound(attackSound, 3);
                break;
            }
        }

    }

}
