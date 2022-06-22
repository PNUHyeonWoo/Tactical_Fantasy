using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
public class Player : MonoBehaviour 
{
    GameObject gameManager;
    PlayerGear playerGear;
    public GameObject shotFirePrefab;
    public GameObject cartridgeCasePrefabs;
    public Animator playerBody;
    public Animator playerLeg;
    public Transform fireAxis;
    public Transform fireStart;
    public GameObject playerHead;
    public Sight Playersight;
    public GameObject main1;
    public GameObject main2;
    public GameObject middleGunObject;
    public GameObject backpack;
    public Sprite standMain1;
    public Sprite standMain2;

    public float maxHP = 100;
    public float maxStamina = 100;

    public float HP= 100;
    public float stamina = 100;
    public float staminaHeal = 10;
    public float walkSpeed = 100;
    public float runAcceleration = 20;
    public float runMaximumSpeed = 100;
    public float runRotateSpeed = 90;
    public int main1FireMode = 0;
    public int main2FireMode = 0;
    public int pistolFireMode = 0;

    public float RunStamina = 10;
    public float slideSpeed = 30;
    public float slideSecond = 0;
    public float ProneSlideSpeed = 100;
    public float ProneSlideSecond = 0;
    public float RollingSpeed = 70;
    public float fireSecond = 0;
    public float fireAmount = 0;
    public float BoltSecond = 0;
    public float knighfSecond = 0;
    public float recoil = 0;
    public float cameraShake = 0;
    public float blood = 0;
    public float poison = 0;

    public float nowSpeed;

    public bool isStand = true;
    public bool isRun = false;
    public bool isZoom = false;
    public bool isLow = false;
    public bool isBB = false;
    public int selectGun = 3;
    public int middleGun = -1; 
    public bool isOpenBackpack = false;

    public Texture[] basicTextures;

    public Chest openChest = null;
    public GameObject itemMenu = null;
    public bool isInShop = false;
    public bool isDead = false;
    public Shop shop = null;
    public Dialog dialog = null;

    AudioSource audioSource;
    public AudioClip[] playerSounds;
    AudioSource bodyAudioSource;
    AudioSource vocalAudioSource;
    public AudioClip[] vocalSounds;
    AudioSource hitAudioSource;

    Collider2D knighfCollider;

    public const float sqrtHalf = (float)0.7071068118;
    public const float footBunmo = (float)3;

    public const float mainStandShoulder = -(float)3.5;
    public const float mainProneShoulder = -(float)2.5;
    public const float mainBProneShoulder = -(float)3.5;
    public const float mainLProneShoulder = (float)5.5;

    public const float pistolStandShoulder = -(float)2.5;
    public const float pistolProneShoulder = -(float)0.5;
    public const float pistolBProneShoulder = -(float)2.5;
    public const float pistolLProneShoulder = (float)6.5;

    public const float RigLine = (float)120;
    public const float GearLine = (float)325;
    public const float LeftLine = (float)210;

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out CursorPos pos);
    [StructLayout(LayoutKind.Sequential)]
    public struct CursorPos
    {
        public int x;
        public int y;
    }



    public void StartPlayer()
    {
        gameManager = GameManager.gameManager.gameObject;
        playerGear = GameManager.saveManager.playerGear;
        playerBody = transform.Find("Body").GetComponent<Animator>();
        playerLeg = GameObject.Find("PlayerLeg").GetComponent<Animator>();
        fireAxis = transform.Find("FireAxis");
        fireStart = fireAxis.transform.Find("FireStart");
        playerHead = playerBody.transform.Find("Head").gameObject;
        Playersight = playerHead.transform.Find("Sight").GetComponent<Sight>();
        main1 = playerBody.transform.Find("Main1").gameObject;
        main2 = playerBody.transform.Find("Main2").gameObject;
        middleGunObject = playerBody.transform.Find("MiddleGun").gameObject;
        backpack = playerBody.transform.Find("Backpack").gameObject;
        audioSource = GetComponent<AudioSource>();
        bodyAudioSource = playerBody.GetComponent<AudioSource>();
        vocalAudioSource = transform.Find("Vocal").GetComponent<AudioSource>();
        hitAudioSource = transform.Find("HitAudioSource").GetComponent<AudioSource>();
        knighfCollider = transform.Find("KnighfCollider").GetComponent<Collider2D>();

        playerBody.GetComponent<PlayerBody>().StartPlayerBody();

        if (playerGear.GearItems[0] != 0)
            GameManager.player.main1FireMode = Gun.GetDefaultFireMode((Gun)Item.GetItem(playerGear.GearItems[0]));
        if (playerGear.GearItems[1] != 0)
            GameManager.player.main2FireMode = Gun.GetDefaultFireMode((Gun)Item.GetItem(playerGear.GearItems[1]));
        if (playerGear.GearItems[2] != 0)
            GameManager.player.pistolFireMode = Gun.GetDefaultFireMode((Gun)Item.GetItem(playerGear.GearItems[2]));

        if (playerGear.GearItems[0] != 0)
            selectGun = 0;
        else if (playerGear.GearItems[1] != 0)
            selectGun = 1;
        else if (playerGear.GearItems[2] != 0)
            selectGun = 2;
        else
            selectGun = 3;


    }
    void Start()
    {        
        BodyPlay("Stand_Aim");
        playerLeg.Play("Stand_Walk");
        ReRenderingPlayerPart();
        SetTextureSelectGun();

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.PauseAmount > 0)
            return;

        if (isInShop)
            return;

        if (isDead)
        {
            if (cameraShake > 0)
                cameraShake -= Time.deltaTime;      

            playerLeg.transform.position = new Vector3(transform.position.x, transform.position.y, playerLeg.transform.position.z) - (playerLeg.transform.up * 7);
            return;
        }

        if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") && !isStand) // 일어나기 버그 방지 구문
        {
            isStand = true;
            playerLeg.GetComponent<SpriteRenderer>().sprite = null;
            playerLeg.transform.Find("LeftFoot").gameObject.SetActive(true);
            playerLeg.transform.Find("RightFoot").gameObject.SetActive(true);
        }

        if (!isRun)
        {
            stamina += staminaHeal * Time.deltaTime;
            if (stamina > maxStamina)
                stamina = maxStamina;
        }

        if(slideSecond > 0)
        {
            transform.Translate(0, slideSpeed * slideSecond * Time.deltaTime, 0);
            slideSecond -= Time.deltaTime;
        }

        if (fireSecond > 0)
            fireSecond -= Time.deltaTime;

        if (BoltSecond > 0)
            BoltSecond -= Time.deltaTime;

        if (knighfSecond > 0)
            knighfSecond -= Time.deltaTime;

        if(cameraShake > 0)
            cameraShake -= Time.deltaTime;

        StatusEffect();

        if (HP <= 0)
        {
            NoHitDead();
            return;
        }

        if (recoil > 0)//반동회복
        {
            if (selectGun < 3)
            {
                Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
                recoil -= nowGun.recoverRecoil * Time.deltaTime;
                if (recoil < 0)
                    recoil = 0;
            }
            else
                recoil = 0;
        }

        if (ProneSlideSecond > 0 && playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Prone") && playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.05)
        {
            GetComponent<Rigidbody2D>().velocity = transform.up * ProneSlideSpeed * ProneSlideSecond + transform.up * 7;
            ProneSlideSecond -= Time.deltaTime;
        }

        //우클릭onoff처리 -> 우클릭에 따른 마우스포인터 변화는 마우스포인터 오브젝트 update에서 처리
        if (SettingManager.isToggleZoom)
        {
            if(Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                SetZoom(!isZoom);
            }

        }
        else
        {
            if (Input.GetMouseButton(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                SetZoom(isZoom);
            }
            else
                SetZoom(!isZoom);

        }


        //시야        

        if (isStand && !isRun 
            && !playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Prone")           
            && !playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Stop")) //walk sight
        {
            playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);

            if (!EventSystem.current.IsPointerOverGameObject() && GameManager.itemManager.NowStartType == 0)
            {

                if (selectGun == 0 || selectGun == 1)
                    LookingWithShoulder(mainStandShoulder);
                else if (selectGun == 2)
                    LookingWithShoulder(pistolStandShoulder);
                else
                    LookingWithShoulder(0);

            }

        }
        else if(!isStand && !EventSystem.current.IsPointerOverGameObject() && GameManager.itemManager.NowStartType == 0) // prone sight
        {
            if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || IsAniProneFIre() || IsAniProneBolt() || isAniProneAttack()) // + 장전 노리쇠 사격
                ProneLook();


        }
        else if(isRun 
            || playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Stop")) // Run Sight
        {
            if (Input.GetMouseButton(1) && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") || playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Stop")) && !EventSystem.current.IsPointerOverGameObject() && GameManager.itemManager.NowStartType == 0)
            {
                Vector2 mousePos;
                mousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePos.x = mousePos.x - transform.position.x;
                mousePos.y = mousePos.y - transform.position.y;

                

                if(mousePos.x >= 0)
                    playerHead.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg - 90);
                else
                    playerHead.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg + 90);

                if( 120 < playerHead.transform.localEulerAngles.z && playerHead.transform.localEulerAngles.z < 180)
                    playerHead.transform.localRotation = Quaternion.Euler(0, 0, 120);
                else if(180 <= playerHead.transform.localEulerAngles.z && playerHead.transform.localEulerAngles.z < 240)
                    playerHead.transform.localRotation = Quaternion.Euler(0, 0, 240);


            }
            else
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);



        }

        //하단조준

        if (SettingManager.isToggleLow)
        {
            if (Input.GetKeyDown(SettingManager.Key_LowFIre) && !EventSystem.current.IsPointerOverGameObject() && GameManager.itemManager.NowStartType == 0)
            {
                isLow = !isLow;
            }

        }
        else
        {
            if (Input.GetKey(SettingManager.Key_LowFIre) && !EventSystem.current.IsPointerOverGameObject() && GameManager.itemManager.NowStartType == 0)
            {
                isLow = true;
            }
            else
                isLow = false;

        }

        if (selectGun < 3)
        {

            if (isLow)
            {
                if (isStand)
                {
                    if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire())
                    {
                        Vector2 SmousePos;
                        SmousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                        SmousePos.x = SmousePos.x - transform.position.x;
                        SmousePos.y = SmousePos.y - transform.position.y;
                        float mDis = Mathf.Sqrt(SmousePos.x * SmousePos.x + SmousePos.y * SmousePos.y);
                        Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
                        float[] dis = Gun.GetLowDis(nowGun);

                        if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim"))
                        {
                            if(mDis > dis[0])
                            {
                                BodyPlay("Stand_Aim", 0);
                            }
                            else if(mDis > dis[1])
                            {
                                BodyPlay("Stand_Aim", 0.25f);
                            }
                            else
                            {
                                BodyPlay("Stand_Aim", 0.5f);
                            }
                        }
                        else if(isAniStandFire())
                        {
                            if (mDis > dis[0])
                            {
                                BodyPlay("Stand_Fire1", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                            }
                            else if (mDis > dis[1])
                            {
                                BodyPlay("Stand_Fire2", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                            }
                            else
                            {
                                BodyPlay("Stand_Fire3", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                            }

                        }
 
                    }
                    else
                        isLow = false;


                }
                else
                {
                    if (!(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || IsAniProneFIre()))
                    {
                        isLow = false;
                    }


                }



            }
            else if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim"))
            {
                BodyPlay("Stand_Aim", 0);
            }
            else if(isAniStandFire())
            {
                BodyPlay("Stand_Fire1", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }

        }
        else//나이프용 하단
        {
            if(isLow)
            {
                if(isStand && !playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") && !playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Attack"))
                {
                    isLow = false;
                }
                else if (!isStand && !playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") && !isAniProneAttack())
                {
                    isLow = false;
                }
            }


        }

        //장전
        Vector2 LmousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.itemManager.canvas, Input.mousePosition, GameManager.itemManager.mainCamera, out LmousePos);
        LmousePos.x += GameManager.itemManager.canvas.sizeDelta.x / 2;
        LmousePos.y += GameManager.itemManager.canvas.sizeDelta.y / 2;

        if (LmousePos.x <= LeftLine && selectGun < 3)
        {
            if (isStand &&
                (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") ||
                playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") ||
                playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload")))
            {              
                if(LmousePos.y <= RigLine)
                    BodyPlay("Stand_Reload",0);
                else if(LmousePos.y < GearLine)
                    BodyPlay("Stand_Reload",0.25f);
                else
                    BodyPlay("Stand_Reload",0.5f);

            }
            else if(!isStand)
            {
                void PlayProneReload(string name)
                {
                    if (LmousePos.y <= RigLine)
                        BodyPlay(name, 0);
                    else if (LmousePos.y <= (GearLine + RigLine) /2)
                        BodyPlay(name, 0.25f);
                    else if (LmousePos.y < GearLine)
                        BodyPlay(name, 0.5f);
                    else
                        BodyPlay(name, 0.75f);
                }

                if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
                {
                    if(playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)                 
                        PlayProneReload("Prone_PReload");                   
                    else if(playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)                   
                        PlayProneReload("Prone_LReload");                  
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)                 
                        PlayProneReload("Prone_BReload");                   
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)                   
                        PlayProneReload("Prone_RReload");                   
                }
                else if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PReload"))               
                    PlayProneReload("Prone_PReload");                
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LReload"))                
                    PlayProneReload("Prone_LReload");               
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BReload"))                
                    PlayProneReload("Prone_BReload");              
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RReload"))                
                    PlayProneReload("Prone_RReload");
                
            }

        }
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload"))
            BodyPlay("Stand_Aim");
        else if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PReload"))
            BodyPlay("Prone_Aim",0);
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LReload"))        
            BodyPlay("Prone_Aim",0.25f);        
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BReload"))       
            BodyPlay("Prone_Aim",0.5f);       
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RReload"))       
            BodyPlay("Prone_Aim",0.75f);              


        //wasd 처리 -> 스탠드/no스탠드 분기 -> 달리기 상태인지 아닌지 / (달리기상태가 아닐때 쉬프트와 함께 눌렸다면 달리기로 전환)
        if (isStand
            && !playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Prone")
            && !playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Stop")
            && !playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Prone_Stand")
            && !playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Prone"))
        {

            if (Input.GetKey(SettingManager.Key_run))
            {
                Run();
            }
            else
            {
                if (SettingManager.isKeepDownRun)
                {
                    if (isRun)
                        Stop();
                    else
                        Walk();
                }
                else
                {
                    if (isRun)
                        Run();
                    else
                        Walk();

                }


            }

        }
        else if (!isStand &&
            (
            playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") ||
            playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire") ||
            playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move") ||
            isAniProneReload()
            )
            )
        {
            ProneMove();
        }


        if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run"))
        {
            if(audioSource.clip != playerSounds[1] || !audioSource.isPlaying)
            {
                PlayPlayerSound(1, 0);
            }
        }

        if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move"))
        {
            if (audioSource.clip != playerSounds[12] || !audioSource.isPlaying)
            {
                PlayPlayerSound(12, 0);
            }
        }

        //왼클릭,i,1,2,3,spaec,z,R등 각종 기타 애니메이션 처리

        //엎드리기
        if (Input.GetKeyDown(SettingManager.Key_Prone))
        {
            if(isStand)
            {
                if(isRun)
                {
                    ProneSlide();
                }
                else
                {
                    if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload"))
                    {
                        nowSpeed = 0;
                        playerLeg.speed = 1;
                        playerLeg.transform.rotation = transform.rotation;
                        SetZoom(false);

                        BodyPlay("Stand_Prone");
                        playerLeg.Play("Stand_Prone");
                        PlayPlayerSound(13, 3);

                    }
                }


            }
            else
            {
                if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move") || isAniProneReload())
                {
                    nowSpeed = 0;
                    playerLeg.speed = 1;
                    transform.rotation = playerLeg.transform.rotation;
                    playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    SetZoom(false);

                    if (!main1.gameObject.activeSelf)
                        main1.GetComponent<SpriteRenderer>().sprite = standMain1;
                    if (!main2.gameObject.activeSelf)
                        main2.GetComponent<SpriteRenderer>().sprite = standMain2;


                    BodyPlay("Prone_Stand");
                    playerLeg.Play("Prone_Stand");
                    PlayPlayerSound(14, 3);
                }

            }

        }

        //슬라이딩
        if (Input.GetKeyDown(SettingManager.Key_ProneSlide))
        {
            if (isStand 
            && !playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Prone")
            && !playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Stop")
            )
            {
                ProneSlide();
            }
            else if(!isStand 
            && !playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Stand")
            && !playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Rolling") 
            )
            {
                Rolling();
            }
        }

        if(playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Prone_LRolling") && playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Rolling"))
        {
            transform.Translate(-RollingSpeed * Time.deltaTime, 0, 0);
            playerLeg.transform.position = new Vector3(playerBody.transform.position.x, playerBody.transform.position.y, playerLeg.transform.position.z);
        }
        else if(playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Prone_RRolling") && playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Rolling"))
        {
            transform.Translate(RollingSpeed * Time.deltaTime, 0, 0);
            playerLeg.transform.position = new Vector3(playerBody.transform.position.x, playerBody.transform.position.y, playerLeg.transform.position.z);
        }

        //가방
        if(Input.GetKeyDown(SettingManager.Key_Backpack))
        {
            if(isStand)
            {
                if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_BackpackOpen") && playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    BodyPlay("Stand_BackpackClose");
                    PlayPlayerSound(3, 2);
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload"))
                {
                    if (playerGear.GearItems[6] != 0)
                    {
                        BodyPlay("Stand_BackpackOpen");
                        PlayPlayerSound(3, 2);
                        SetZoom(false);
                    }
                }


            }
            else
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BackpackOpen") && playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    BodyPlay("Prone_BackpackClose");
                    PlayPlayerSound(3, 2);
                }
                else if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move") || isAniProneReload())
                {
                    if (playerGear.GearItems[6] != 0)
                    {
                        BodyPlay("Prone_BackpackOpen");
                        playerLeg.Play("Prone_Aim", 0, 0);
                        PlayPlayerSound(3, 2);

                        SetZoom(false);
                        nowSpeed = 0;
                        playerLeg.speed = 1;
                        transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
                        playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                }



            }



        }

        //스왑

        if(selectGun == 0 || selectGun == 1)// 메인 -> 스왑
        {
            if( Input.GetKeyDown(SettingManager.Key_SelectGun[0]) || Input.GetKeyDown(SettingManager.Key_SelectGun[1]) )
            {
                int change = 1 - selectGun;

                if(isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload")))
                {
                    if(playerGear.GearItems[change] != 0)
                    {
                        if (selectGun == 0)
                            BodyPlay("Stand_Main1Swap_Start");
                        else
                            BodyPlay("Stand_Main2Swap_Start");

                        PlayPlayerSound(4, 3);

                        isBB = false;
                        SetZoom(false);

                    }


                }
                else if(!isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move") || isAniProneReload()))
                {
                    if (playerGear.GearItems[change] != 0)
                    {
                        if (selectGun == 0)
                            BodyPlay("Prone_Main1Swap_Start");
                        else
                            BodyPlay("Prone_Main2Swap_Start");

                        PlayPlayerSound(5, 3);

                        isBB = false;
                        playerLeg.Play("Prone_Aim", 0, 0);
                        SetZoom(false);
                        nowSpeed = 0;
                        playerLeg.speed = 1;
                        transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
                        playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }

                }
                        

                




            }
            else if (Input.GetKeyDown(SettingManager.Key_SelectGun[2]))
            {
                if (isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload")))
                {
                    if (playerGear.GearItems[2] != 0)
                    {
                        BodyPlay("Stand_PistolSwap_Start");
                        PlayPlayerSound(6, 3);
                        isBB = false;
                        SetZoom(false);
                    }


                }
                else if (!isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move") || isAniProneReload()))
                {
                    if (playerGear.GearItems[2] != 0)
                    {
                        BodyPlay("Prone_PistolSwap_Start");
                        PlayPlayerSound(7, 3);

                        isBB = false;
                        playerLeg.Play("Prone_Aim", 0, 0);
                        SetZoom(false);
                        nowSpeed = 0;
                        playerLeg.speed = 1;
                        transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
                        playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    }


                }




            }
            else if(Input.GetKeyDown(SettingManager.Key_SelectGun[3]))
            {
                if (isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload")))
                {                 
                        BodyPlay("Stand_KnifeSwap_Start");
                        PlayPlayerSound(6, 3);
                        isBB = false;
                        SetZoom(false);
               
                }
                else if (!isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move") || isAniProneReload()))
                {                   
                        BodyPlay("Prone_KnifeSwap_Start");
                        PlayPlayerSound(7, 3);
                        isBB = false;
                        playerLeg.Play("Prone_Aim", 0, 0);
                        SetZoom(false);
                        nowSpeed = 0;
                        playerLeg.speed = 1;
                        transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
                        playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);

                  
                }
            }




        }
        else if(selectGun == 2) // 권총 -> 스왑
        {
            if (Input.GetKeyDown(SettingManager.Key_SelectGun[0]) || Input.GetKeyDown(SettingManager.Key_SelectGun[1]))
            {
                if (isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload")))
                {
                    if(middleGun != -1 && playerGear.GearItems[middleGun] != 0)
                    {
                        BodyPlay("Stand_PistolSwap_Start");
                        PlayPlayerSound(8, 3);
                        isBB = false;
                        SetZoom(false);
                    }


                }
                else if (!isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move") || isAniProneReload()))
                {
                    if (middleGun != -1 && playerGear.GearItems[middleGun] != 0)
                    {
                        BodyPlay("Prone_PistolSwap_Start");
                        isBB = false;
                        playerLeg.Play("Prone_Aim", 0, 0);
                        PlayPlayerSound(9, 3);
                        SetZoom(false);
                        nowSpeed = 0;
                        playerLeg.speed = 1;
                        transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
                        playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    }

                }


            }
            else if (Input.GetKeyDown(SettingManager.Key_SelectGun[3]))
            {
                if (isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload")))
                {
                    BodyPlay("Stand_PistolKnifeSwap_Start");
                    PlayPlayerSound(6, 3);
                    isBB = false;
                    SetZoom(false);

                }
                else if (!isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move") || isAniProneReload()))
                {
                    BodyPlay("Prone_PistolKnifeSwap_Start");

                    isBB = false;
                    playerLeg.Play("Prone_Aim", 0, 0);
                    PlayPlayerSound(7, 3);

                    SetZoom(false);
                    nowSpeed = 0;
                    playerLeg.speed = 1;
                    transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
                    playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);


                }
            }


        }
        else // 단검 -> 스왑
        {
            if (Input.GetKeyDown(SettingManager.Key_SelectGun[0]) || Input.GetKeyDown(SettingManager.Key_SelectGun[1]))
            {
                if (isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload")))
                {
                    if (middleGun != -1 && playerGear.GearItems[middleGun] != 0)
                    {
                        BodyPlay("Stand_KnifeMainSwap_Start");
                        PlayPlayerSound(8, 3);
                        isBB = false;
                        SetZoom(false);
                    }


                }
                else if (!isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move") || isAniProneReload()))
                {
                    if (middleGun != -1 && playerGear.GearItems[middleGun] != 0)
                    {
                        BodyPlay("Prone_KnifeMainSwap_Start");

                        isBB = false;
                        playerLeg.Play("Prone_Aim", 0, 0);
                        PlayPlayerSound(9, 3);

                        SetZoom(false);
                        nowSpeed = 0;
                        playerLeg.speed = 1;
                        transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
                        playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    }

                }
            }
            else if (Input.GetKeyDown(SettingManager.Key_SelectGun[2]))
            {
                if (isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload")))
                {
                    if (playerGear.GearItems[2] != 0)
                    {
                        BodyPlay("Stand_KnifePistolSwap_Start");
                        PlayPlayerSound(6, 3);
                        isBB = false;
                        SetZoom(false);
                    }


                }
                else if (!isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move") || isAniProneReload()))
                {
                    if (playerGear.GearItems[2] != 0)
                    {
                        BodyPlay("Prone_KnifePistolSwap_Start");

                        isBB = false;
                        playerLeg.Play("Prone_Aim", 0, 0);
                        PlayPlayerSound(7, 3);
                        SetZoom(false);
                        nowSpeed = 0;
                        playerLeg.speed = 1;
                        transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
                        playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    }


                }

            }

        }

        //노리쇠 장전
        if (Input.GetKeyDown(SettingManager.Key_Bolt) && selectGun < 3 && GameManager.itemManager.NowStartType == 0)
        {
            Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
            if (nowGun.gunAnimation != Gun.GunAnimation.DoubleBarrel)
            {

                if (isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload")))
                {
                    if (isBB)
                    {
                        isBB = false;
                        if (!isLow)
                            BodyPlay("Stand_Aim");
                        else
                            AimStandLowAniPlay();

                        PlayGunSound(nowGun.BBSound, 7);
                    }
                    else
                    {
                        BodyPlay("Stand_Bolt");
                        SetZoom(false);
                    }

                }
                else if (!isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || IsAniProneFIre()|| isAniProneReload()))
                {

                    if (isBB)
                    {
                        isBB = false;

                        if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
                        {
                            if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)
                                BodyPlay("Prone_Aim", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)
                                BodyPlay("Prone_Aim", 0.25f);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                                BodyPlay("Prone_Aim", 0.5f);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)
                                BodyPlay("Prone_Aim", 0.75f);
                        }
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PFire"))
                            BodyPlay("Prone_Aim", 0);
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LFire"))
                            BodyPlay("Prone_Aim", 0.25f);
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BFire"))
                            BodyPlay("Prone_Aim", 0.5f);
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RFire"))
                            BodyPlay("Prone_Aim", 0.75f);

                        PlayGunSound(nowGun.BBSound, 7);

                    }
                    else
                    {
                        SetZoom(false);

                        if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
                        {
                            if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)
                                BodyPlay("Prone_PBolt", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)
                                BodyPlay("Prone_LBolt", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                                BodyPlay("Prone_BBolt", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)
                                BodyPlay("Prone_RBolt", 0);
                        }
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PFire"))
                            BodyPlay("Prone_PBolt", 0);
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LFire"))
                            BodyPlay("Prone_LBolt", 0);
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BFire"))
                            BodyPlay("Prone_BBolt", 0);
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RFire"))
                            BodyPlay("Prone_RBolt", 0);
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PReload"))
                            BodyPlay("Prone_PBolt", 0);
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LReload"))
                            BodyPlay("Prone_LBolt", 0);
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BReload"))
                            BodyPlay("Prone_BBolt", 0);
                        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RReload"))
                            BodyPlay("Prone_RBolt", 0);
                    }


                }
            }
        }



        //사격
        if (GameManager.itemManager.NowStartType != 0)
            fireAmount = 0;

        if(fireAmount > 0) // 점사용 업데이트
        {
            if (isStand)
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire())
                {
                    if(fireSecond <= 0)
                    {
                        string gunContents = GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]];
                        if (gunContents[0] != '0')
                        {
                            Fire();
                            if (!isLow)
                                BodyPlay("Stand_Fire1", 0);
                            else
                                FIreStandLowAniPlay();
                            fireAmount--;
                        }
                        else
                        {
                            fireAmount = 0;
                        }
                    }

                }
                else
                    fireAmount = 0;

            }
            else if(!isStand)
            {
                if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || IsAniProneFIre())
                {
                    if(fireSecond <= 0)
                    {
                        string gunContents = GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]];
                        if (gunContents[0] != '0')
                        {
                            Fire();
                            if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
                            {
                                if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)
                                    BodyPlay("Prone_PFire",0);
                                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)
                                    BodyPlay("Prone_LFire", 0);
                                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                                    BodyPlay("Prone_BFire", 0);
                                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)
                                    BodyPlay("Prone_RFire", 0);
                            }
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PFire"))
                                BodyPlay("Prone_PFire", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LFire"))
                                BodyPlay("Prone_LFire", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BFire"))
                                BodyPlay("Prone_BFire", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RFire"))
                                BodyPlay("Prone_RFire", 0);
                            fireAmount--;
                        }
                        else
                            fireAmount = 0;


                    }

                }
                else
                    fireAmount = 0;


            }


        }




        if(Input.GetMouseButton(0) && selectGun < 3 && !EventSystem.current.IsPointerOverGameObject() && GameManager.itemManager.NowStartType == 0)
        {
            Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
            int gunType = Gun.CheckGunType(nowGun);

            if(gunType == 0) //단순 탄창류 총기
            {
                if(isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire()) && fireSecond <= 0)
                {
                    string gunContents = GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]];
                    if(gunContents[0] != '0') //약실에 총알 존재
                    {
                        if(GetSelectGunFireMode() == 0 && Input.GetMouseButtonDown(0))//싱글
                        {
                            Fire();
                            if(!isLow)
                            BodyPlay("Stand_Fire1", 0);
                            else
                            {
                                FIreStandLowAniPlay();
                            }
                        }
                        else if(GetSelectGunFireMode() == 1 && Input.GetMouseButtonDown(0) && fireAmount == 0)//점사
                        {
                            Fire();
                            if (!isLow)
                                BodyPlay("Stand_Fire1", 0);
                            else
                            {
                                FIreStandLowAniPlay();
                            }
                            fireAmount = 2;
                        }
                        else if (GetSelectGunFireMode() == 2)//연사
                        {
                            Fire();
                            if (!isLow)
                                BodyPlay("Stand_Fire1", 0);
                            else
                            {
                                FIreStandLowAniPlay();
                            }
                        }



                    }
                    else if(Input.GetMouseButtonDown(0))//약실에 총알이 없음
                    {
                        if(!isBB)
                        {
                            PlayGunSound(playerSounds[10], 2);
                        }
                        else
                        {
                            isBB = false;
                            if (!isLow)
                                BodyPlay("Stand_Aim");
                            else
                                AimStandLowAniPlay();

                            PlayGunSound(nowGun.BBSound,7);
                        }

                    }


                }
                else if(!isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || IsAniProneFIre()) && fireSecond <= 0)
                {
                    string gunContents = GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]];

                    if (gunContents[0] != '0')
                    {
                        void AniProneFIre()
                        {
                            if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
                            {
                                if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)
                                    BodyPlay("Prone_PFire", 0);
                                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)
                                    BodyPlay("Prone_LFire", 0);
                                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                                    BodyPlay("Prone_BFire", 0);
                                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)
                                    BodyPlay("Prone_RFire", 0);
                            }
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PFire"))
                                BodyPlay("Prone_PFire", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LFire"))
                                BodyPlay("Prone_LFire", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BFire"))
                                BodyPlay("Prone_BFire", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RFire"))
                                BodyPlay("Prone_RFire", 0);

                        }

                        if (GetSelectGunFireMode() == 0 && Input.GetMouseButtonDown(0))//싱글
                        {
                            Fire();
                            AniProneFIre();
                        }
                        else if (GetSelectGunFireMode() == 1 && Input.GetMouseButtonDown(0) && fireAmount == 0)//점사
                        {
                            Fire();
                            AniProneFIre();
                            fireAmount = 2;
                        }
                        else if (GetSelectGunFireMode() == 2)//연사
                        {
                            Fire();
                            AniProneFIre();
                        }



                    }
                    else if(Input.GetMouseButtonDown(0))
                    {
                        if (!isBB)
                        {
                            PlayGunSound(playerSounds[10], 2);
                        }
                        else
                        {
                            isBB = false;

                            if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
                            {
                                if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)
                                    BodyPlay("Prone_Aim", 0);
                                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)
                                    BodyPlay("Prone_Aim", 0.25f);
                                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                                    BodyPlay("Prone_Aim", 0.5f);
                                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)
                                    BodyPlay("Prone_Aim", 0.75f);
                            }
                            else if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PFire"))
                                BodyPlay("Prone_Aim", 0);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LFire"))
                                BodyPlay("Prone_Aim", 0.25f);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BFire"))
                                BodyPlay("Prone_Aim", 0.5f);
                            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RFire"))
                                BodyPlay("Prone_Aim", 0.75f);

                            PlayGunSound(nowGun.BBSound,7);

                        }

                    }


                }

            }
            else if(gunType == 1) // 더블배럴
            {

            }
            else if(gunType == 2) // 펌프샷건 및 반자동/오토 샷건 (사격모드로 구분)
            {

            }
            else if(gunType == 3)// 볼트액션
            {

            }


        }

        //단검 공격
        if(Input.GetMouseButton(0) && selectGun == 3 && !EventSystem.current.IsPointerOverGameObject() && GameManager.itemManager.NowStartType == 0)
        {
            if (isStand && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run")))
            {
                BodyPlay("Stand_Attack");
            }
            else if(!isStand && playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
            {               
                    if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)
                        BodyPlay("Prone_PAttack", 0);
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)
                        BodyPlay("Prone_LAttack", 0);
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                        BodyPlay("Prone_BAttack", 0);
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)
                        BodyPlay("Prone_RAttack", 0);
                


            }
        }

        //탄창버리기
        if(Input.GetKeyDown(SettingManager.Key_DropMagazine) && selectGun < 3)
        {
            Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
            int gunType = Gun.CheckGunType(nowGun);

            if(gunType == 0)
            {
                if(CanGrabItem() || isAniStandFire() || IsAniProneFIre())
                {
                    if( (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_BackpackOpen") && !playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BackpackOpen")) || playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.15)
                    {
                        int[] magazine = Gun.GetMagazineCodePLC(playerGear.GunsPLC[selectGun], true);

                        if (magazine[0] != -1)
                        {
                            magazine[1] = PlusContainerManager.newAllocatePLCnode(magazine[1], GameManager.saveManager.savePLC, GameManager.nonSavePLCManager);

                            GameManager.saveManager.playerGear.nowAttachmentCellGroup.attachmentContainer.RemoveOnlyTextCode(GameManager.saveManager.playerGear.nowAttachmentCellGroup.attachmentContainer.attachmentCells.Length - 1);
                            DropItem.CreateDropItem(magazine[0], new int[1] { magazine[1] } , 1);
                            GameManager.itemManager.AttachmentContainerReRender(playerGear.nowAttachmentCellGroup);

                            PlayGunSound(playerSounds[16], 0);
                        }
                    }
                }
            }
        }

        //파이어 모드 변경
        if(Input.GetKeyDown(SettingManager.key_FIreMode) && selectGun < 3)
        {
            Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);

            void ChangeFIreMode(ref int firemode)
            {          
                    if (firemode == 0)
                    {
                        if (nowGun.bisrt)
                        {
                        firemode = 1;
                        PlayGunSound(playerSounds[17], 0);
                        }
                        else if (nowGun.Auto)
                        {
                        firemode = 2;
                        PlayGunSound(playerSounds[17], 0);
                        }
                    }
                    else if (firemode == 1)
                    {
                        if (nowGun.Auto)
                        {
                        firemode = 2;
                        fireAmount = 0;
                        PlayGunSound(playerSounds[17], 0);
                        }
                        else if (nowGun.single)
                        {
                        firemode = 0;
                        fireAmount = 0;
                        PlayGunSound(playerSounds[17], 0);
                        }
                    }
                    else
                    {
                        if (nowGun.single)
                        {
                        firemode = 0;
                        PlayGunSound(playerSounds[17], 0);
                        }
                        else if (nowGun.bisrt)
                        {
                        firemode = 1;
                        PlayGunSound(playerSounds[17], 0);
                        }
                    }

            }

            if(selectGun == 0)
            {
                ChangeFIreMode(ref main1FireMode);

            }
            else if(selectGun == 1)
            {
                ChangeFIreMode(ref main2FireMode);

            }
            else
            {
                ChangeFIreMode(ref pistolFireMode);

            }


        }



        //상호작용
        if (Input.GetKeyDown(SettingManager.Key_Interaction))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            RaycastHit2D rayhit = Physics2D.Raycast(mousePos, Vector2.zero);
            

            if (rayhit.collider != null && rayhit.collider.GetComponent<Interaction>() != null)
            {
                float dis = Mathf.Sqrt((rayhit.transform.position.x - transform.position.x) * (rayhit.transform.position.x - transform.position.x) + (rayhit.transform.position.y - transform.position.y) * (rayhit.transform.position.y - transform.position.y));
                Interaction intertaction = rayhit.collider.GetComponent<Interaction>();

                if (intertaction.GetType() == typeof(Chest) && dis < 40)
                {
                    if (openChest == null)
                    {
                        ((Chest)intertaction).RenderChest();
                        openChest = (Chest)intertaction;
                    }
                    else if(openChest != (Chest)intertaction)
                    {
                        openChest.DestroyChestCellGroup();
                        ((Chest)intertaction).RenderChest();
                        openChest = (Chest)intertaction;
                    }
                }
                else if(intertaction.GetType() == typeof(ShopDoor) && dis < 40 && playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim"))
                {
                    ((ShopDoor)intertaction).CreateShopUI();
                    isInShop = true;
                    nowSpeed = 0;
                    playerLeg.speed = 0;
                    fireAmount = 0;
                }
                else if(intertaction.GetType() == typeof(HouseCloset) && dis < 40)
                {
                    GameManager.SceneMove("Stage1");
                }
                else if (intertaction.GetType() == typeof(Rift) && dis < 40)
                {
                    GameManager.SceneMove("MainScene");
                }
                else if (intertaction is EventInteraction && dis < ((EventInteraction)intertaction).dis)
                {
                    ((EventInteraction)intertaction).Interact();
                }

                //그외 나머지 상호작용 오브젝트들 타입확인및 거리확인후 처리

            }

        }

        //상자파괴
        if(openChest != null)
        {
            float dis = Mathf.Sqrt((openChest.transform.position.x - transform.position.x) * (openChest.transform.position.x - transform.position.x) + (openChest.transform.position.y - transform.position.y) * (openChest.transform.position.y - transform.position.y));
            if(dis >= 40)
            {
                openChest.DestroyChestCellGroup();
                openChest = null;
            }


        }




        //마우스 리그로 이동
        if (Input.GetKeyDown(SettingManager.Key_MoveMouseToRig) && GameManager.itemManager.NowStartType == 0)
        {
            SetCursorPos(0, Screen.currentResolution.height);
            CursorPos mouPos = new CursorPos();
            GetCursorPos(out mouPos);
            if(!Application.isEditor)
            SetCursorPos(mouPos.x + Screen.width / 9, mouPos.y - Screen.height / 11);
            else
            SetCursorPos(mouPos.x + 100, mouPos.y - 50);
        }


        if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || isAniStandFire() || IsAniProneFIre() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload") || isAniProneReload() || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Bolt") || IsAniProneBolt())
        {
            if (audioSource.clip != playerSounds[0])
                PlayPlayerSound(0, 0);
        }



        //카메라 및 하체 이동


        if (isStand && !playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Prone") && !playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Prone_Stand"))
            playerLeg.transform.position = new Vector3(transform.position.x, transform.position.y, playerLeg.transform.position.z);

        if (!isStand&&!playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Prone") && !playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Prone_Stand"))
            playerLeg.transform.position = new Vector3(transform.position.x, transform.position.y, playerLeg.transform.position.z) - (playerLeg.transform.up * 7);

    }

    void LateUpdate()
    {
        
        if (cameraShake <= 0)
            GameManager.itemManager.mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, GameManager.itemManager.mainCamera.transform.position.z);
        else
        {
            Vector2 shake = new Vector2(Random.Range(-cameraShake * 3, cameraShake * 4), Random.Range(-cameraShake * 3, cameraShake * 4));
            GameManager.itemManager.mainCamera.transform.position = new Vector3(transform.position.x + shake.x, transform.position.y + shake.y, GameManager.itemManager.mainCamera.transform.position.z);
        }
        
    }


    void Walk()
    {
        isRun = false;
        playerHead.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run"))
            BodyPlay("Stand_Aim");


        float[] getMovekey = GetMoveKey();
        
        if(getMovekey == null)
        {
            nowSpeed = 0;
            playerLeg.speed = 0;
        }
        else
        {
            nowSpeed = walkSpeed;
            if (isZoom)
                nowSpeed /= 2;
            playerLeg.speed = nowSpeed/footBunmo;
            playerLeg.transform.rotation = Quaternion.Euler(0, 0, getMovekey[2]);
            transform.Translate(getMovekey[0] * nowSpeed * Time.deltaTime, getMovekey[1] * nowSpeed * Time.deltaTime, 0,Space.World);
        }




    }

    void Run()
    {
        if(isRun)
        {
            float[] getMovekey = GetMoveKey();

            if (getMovekey == null || stamina <= 0)
            {
                Stop();

                return;
            }

            stamina -= RunStamina * Time.deltaTime;

            if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim"))
                BodyPlay("Stand_Run");

            nowSpeed += runAcceleration * Time.deltaTime;


            if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run"))
            {
                if (nowSpeed > runMaximumSpeed)
                    nowSpeed = runMaximumSpeed;
            }
            else if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload"))
            {
                if (nowSpeed > runMaximumSpeed - (runMaximumSpeed - walkSpeed) / 2 + 1)
                    nowSpeed = runMaximumSpeed - (runMaximumSpeed - walkSpeed) / 2 + 1;
            }
            else
            {
                if (nowSpeed > runMaximumSpeed - (runMaximumSpeed - walkSpeed) / 2)
                    nowSpeed = runMaximumSpeed - (runMaximumSpeed - walkSpeed) / 2;
            }


            playerLeg.speed = nowSpeed / footBunmo;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, getMovekey[2]), runRotateSpeed*Mathf.PI/180 * Time.deltaTime);
            playerLeg.transform.rotation = transform.rotation;

            transform.Translate(0, nowSpeed * Time.deltaTime, 0);

        }
        else
        {
            if(stamina > 0)
            {
                float[] getMovekey = GetMoveKey();

                if (getMovekey == null)
                {
                    Walk();
                    return;
                }
                //달리기 시작 확정

                SetZoom(false);

                stamina -= RunStamina * Time.deltaTime;
                isRun = true;
                

                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim"))
                    BodyPlay("Stand_Run");


                

                nowSpeed = walkSpeed + runAcceleration * Time.deltaTime;
                playerLeg.speed = nowSpeed / footBunmo;
                playerLeg.transform.rotation = Quaternion.Euler(0, 0, getMovekey[2]);

                transform.Translate(getMovekey[0] * nowSpeed * Time.deltaTime, getMovekey[1] * nowSpeed * Time.deltaTime, 0, Space.World);
                
                transform.rotation = Quaternion.Euler(0, 0, getMovekey[2]);


            }
            else
            {
                Walk();
            }

        }



    }

    void Stop()
    {
        isRun = false;

        if (nowSpeed > runMaximumSpeed - (runMaximumSpeed - walkSpeed) / 2)
        {
            nowSpeed = 0;
            playerLeg.speed = 1;
            slideSecond = (float)0.4;
            BodyPlay("Stand_Stop");
            playerLeg.Play("Stand_Stop");
            PlayPlayerSound(2, 5f);
        }       
        else
            Walk();
    }

    void FIreStandLowAniPlay()
    {
        Vector2 SmousePos;
        SmousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        SmousePos.x = SmousePos.x - transform.position.x;
        SmousePos.y = SmousePos.y - transform.position.y;
        float mDis = Mathf.Sqrt(SmousePos.x * SmousePos.x + SmousePos.y * SmousePos.y);
        Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
        float[] dis = Gun.GetLowDis(nowGun);


            if (mDis > dis[0])
            {
                BodyPlay("Stand_Fire1");
            }
            else if (mDis > dis[1])
            {
                BodyPlay("Stand_Fire2");
            }
            else
            {
                BodyPlay("Stand_Fire3");
            }

    }

    int GetLow()
    {
        Vector2 SmousePos;
        SmousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        SmousePos.x = SmousePos.x - transform.position.x;
        SmousePos.y = SmousePos.y - transform.position.y;
        float mDis = Mathf.Sqrt(SmousePos.x * SmousePos.x + SmousePos.y * SmousePos.y);
        Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
        float[] dis = Gun.GetLowDis(nowGun);


        if (mDis > dis[0])
        {
            return -3;
        }
        else if (mDis > dis[1])
        {
            return -2;
        }
        else
        {
            return -1;
        }

    }

    void AimStandLowAniPlay()
    {
        Vector2 SmousePos;
        SmousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        SmousePos.x = SmousePos.x - transform.position.x;
        SmousePos.y = SmousePos.y - transform.position.y;
        float mDis = Mathf.Sqrt(SmousePos.x * SmousePos.x + SmousePos.y * SmousePos.y);
        Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
        float[] dis = Gun.GetLowDis(nowGun);


        if (mDis > dis[0])
        {
            BodyPlay("Stand_Aim",0);
        }
        else if (mDis > dis[1])
        {
            BodyPlay("Stand_Aim",0.25f);
        }
        else
        {
            BodyPlay("Stand_Aim",0.5f);
        }

    }

    void Fire()
    {
        Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
        string gunContents = GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]];
        int ammoCode = int.Parse(gunContents.Substring(0, 7));
        if(ammoCode != 0)
        {
            Vector2 origin = new Vector2();
            Vector2 direction = new Vector2();

            if(isStand)
            {
                fireAxis.localRotation = Quaternion.Euler(0, 0, 0);
                if(selectGun < 2)
                    fireStart.localPosition = new Vector3(-mainStandShoulder, 0, 0);
                else
                    fireStart.localPosition = new Vector3(-pistolStandShoulder, 0, 0);
                origin = fireStart.position;
                direction = transform.up;

                GameObject shotFire = GameObject.Instantiate(shotFirePrefab);
                shotFire.GetComponent<ShotFire>().direct = -1;
                if (!isLow)
                    fireStart.Translate(0, Gun.GetBarrelRange(nowGun, -3), 0);
                else
                    fireStart.Translate(0, Gun.GetBarrelRange(nowGun, GetLow()), 0);
                shotFire.transform.position = new Vector3(fireStart.position.x, fireStart.position.y, playerBody.transform.position.z);
                shotFire.transform.rotation = transform.rotation;
                shotFire.transform.SetParent(transform);

                GameObject cartridge = GameObject.Instantiate(cartridgeCasePrefabs);
                cartridge.GetComponent<CartridgeCase>().move = new Vector2(1, 0);
                if (!isLow)
                    fireStart.Translate(0, -Gun.GetBoltRange(nowGun, -3), 0);
                else
                    fireStart.Translate(0, -Gun.GetBoltRange(nowGun, GetLow()), 0);
                cartridge.transform.position = new Vector3(fireStart.position.x, fireStart.position.y, playerBody.transform.position.z + 1);
                cartridge.transform.rotation = transform.rotation;
            }
            else
            {
                int d = GetProneDirection();
                if (d == 0)
                {
                    fireAxis.localRotation = Quaternion.Euler(0, 0, 0);
                    if (selectGun < 2)
                        fireStart.localPosition = new Vector3(-mainProneShoulder, 0, 0);
                    else
                        fireStart.localPosition = new Vector3(-pistolProneShoulder, 0, 0);
                    origin = fireStart.position;
                    direction = transform.up;

                    GameObject shotFire = GameObject.Instantiate(shotFirePrefab);
                    shotFire.GetComponent<ShotFire>().direct = 0;
                    fireStart.Translate(0, Gun.GetBarrelRange(nowGun,0), 0);
                    shotFire.transform.position = new Vector3(fireStart.position.x, fireStart.position.y, playerBody.transform.position.z);
                    shotFire.transform.rotation = transform.rotation;
                    shotFire.transform.SetParent(transform);

                    GameObject cartridge = GameObject.Instantiate(cartridgeCasePrefabs);
                    cartridge.GetComponent<CartridgeCase>().move = new Vector2(1, 0);
                    fireStart.Translate(0, -Gun.GetBoltRange(nowGun, 0), 0);
                    cartridge.transform.position = new Vector3(fireStart.position.x, fireStart.position.y, playerBody.transform.position.z + 1);
                    cartridge.transform.rotation = transform.rotation;
                }
                else if(d == 1)
                {
                    fireAxis.localRotation = Quaternion.Euler(0, 0, 90);
                    if (selectGun < 2)
                        fireStart.localPosition = new Vector3(mainLProneShoulder, 0, 0);
                    else
                        fireStart.localPosition = new Vector3(pistolLProneShoulder, 0, 0);
                    origin = fireStart.position;
                    direction = fireAxis.up;

                    GameObject shotFire = GameObject.Instantiate(shotFirePrefab);
                    shotFire.GetComponent<ShotFire>().direct = 1;
                    fireStart.Translate(0, Gun.GetBarrelRange(nowGun, 1), 0);
                    shotFire.transform.position = new Vector3(fireStart.position.x, fireStart.position.y, playerBody.transform.position.z);
                    shotFire.transform.rotation = fireAxis.rotation;
                    shotFire.transform.SetParent(transform);

                    GameObject cartridge = GameObject.Instantiate(cartridgeCasePrefabs);
                    if(selectGun < 2)
                        cartridge.GetComponent<CartridgeCase>().move = new Vector2(-0.5f, 0);
                    else
                        cartridge.GetComponent<CartridgeCase>().move = new Vector2(0.5f, 0);
                    fireStart.Translate(0, -Gun.GetBoltRange(nowGun, 1), 0);
                    cartridge.transform.position = new Vector3(fireStart.position.x, fireStart.position.y, playerBody.transform.position.z + 1);
                    cartridge.transform.rotation = fireAxis.rotation;
                }
                else if (d == 2)
                {
                    fireAxis.localRotation = Quaternion.Euler(0, 0, 180);
                    if (selectGun < 2)
                        fireStart.localPosition = new Vector3(-mainBProneShoulder, 0, 0);
                    else
                        fireStart.localPosition = new Vector3(-pistolBProneShoulder, 0, 0);
                    origin = fireStart.position;
                    direction = fireAxis.up;

                    GameObject shotFire = GameObject.Instantiate(shotFirePrefab);
                    shotFire.GetComponent<ShotFire>().direct = 2;
                    fireStart.Translate(0, Gun.GetBarrelRange(nowGun, 2), 0);
                    shotFire.transform.position = new Vector3(fireStart.position.x, fireStart.position.y, playerBody.transform.position.z);
                    shotFire.transform.rotation = fireAxis.rotation;
                    shotFire.transform.SetParent(transform);

                    GameObject cartridge = GameObject.Instantiate(cartridgeCasePrefabs);
                    cartridge.GetComponent<CartridgeCase>().move = new Vector2(1, 0);
                    fireStart.Translate(0, -Gun.GetBoltRange(nowGun, 2), 0);
                    cartridge.transform.position = new Vector3(fireStart.position.x, fireStart.position.y, playerBody.transform.position.z + 1);
                    cartridge.transform.rotation = fireAxis.rotation;
                }
                else if (d == 3)
                {
                    fireAxis.localRotation = Quaternion.Euler(0, 0, 90);
                    if (selectGun < 2)
                        fireStart.localPosition = new Vector3(mainLProneShoulder, 0, 0);
                    else
                        fireStart.localPosition = new Vector3(pistolLProneShoulder, 0, 0);
                    origin = fireStart.position;
                    direction = -fireAxis.up;

                    GameObject shotFire = GameObject.Instantiate(shotFirePrefab);
                    shotFire.GetComponent<ShotFire>().direct = 3;
                    fireStart.Translate(0, Gun.GetBarrelRange(nowGun, 3), 0);
                    shotFire.transform.position = new Vector3(fireStart.position.x, fireStart.position.y, playerBody.transform.position.z);
                    shotFire.transform.rotation = Quaternion.Euler(0,0,fireAxis.rotation.eulerAngles.z + 180);
                    shotFire.transform.SetParent(transform);

                    GameObject cartridge = GameObject.Instantiate(cartridgeCasePrefabs);
                    if (selectGun < 2)
                        cartridge.GetComponent<CartridgeCase>().move = new Vector2(0.5f, 0);
                    else
                        cartridge.GetComponent<CartridgeCase>().move = new Vector2(-0.5f, 0);
                    cartridge.GetComponent<CartridgeCase>().isTargetZ = true;
                    cartridge.GetComponent<CartridgeCase>().TargetZ = playerBody.transform.position.z + 1;
                    fireStart.Translate(0, -Gun.GetBoltRange(nowGun, 3), 0);
                    cartridge.transform.position = new Vector3(fireStart.position.x, fireStart.position.y, playerBody.transform.position.z - 1);
                    cartridge.transform.rotation = Quaternion.Euler(0, 0, fireAxis.rotation.eulerAngles.z + 180);
                }


            }
            Ammo ammo = (Ammo)Item.GetItem(ammoCode);

            if (!isLow)
                Attack.AttackWithRayCast(origin.x, origin.y, direction.x, direction.y, false, ammo.damage, ammo.Penetration, gameObject, new int[0], false, 400f);
            else
            {
                Attack.AttackWithRayCast(origin.x, origin.y, direction.x, direction.y, true, ammo.damage, ammo.Penetration, gameObject, new int[0], false, Vector2.Distance(origin, GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition)));
            }

            DoRecoil();
            //여기까지가 실제 레이캐스트 사격 + 반동 + 총구섬광 + 탄피
            PlayGunSound(nowGun.fireSound, nowGun.fireLoudness);

            fireSecond = nowGun.rateOfFire;           


            int[] magazine = Gun.GetMagazineCodePLC(playerGear.GunsPLC[selectGun], true);
            if (magazine[0] == -1)
            {
                if(nowGun.canBB)
                isBB = true;
                gunContents = "0000000" + gunContents.Substring(7, gunContents.Length - 7);
                GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]] = gunContents;
            }
            else
            {
                string magazineContents = GameManager.saveManager.savePLC.plusContainers[magazine[1]];
                if(magazineContents == "")
                {
                    if (nowGun.canBB)
                        isBB = true;
                    gunContents = "0000000" + gunContents.Substring(7, gunContents.Length - 7);
                    GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]] = gunContents;
                }
                else
                {
                    gunContents = "400"  + magazineContents.Substring(magazineContents.Length - 4, 4) + gunContents.Substring(7, gunContents.Length - 7);
                    GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]] = gunContents;
                    magazineContents = magazineContents.Substring(0, magazineContents.Length - 4);
                    GameManager.saveManager.savePLC.plusContainers[magazine[1]] = magazineContents;
                }

            }

            GameManager.itemManager.AttachmentContainerReRender(playerGear.nowAttachmentCellGroup);







        }

    }

    public void DoRecoil()
    {
        Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);

        Vector2 result = new Vector2();

        Vector2 nowRecoil = nowGun.minRecoil * (1 - recoil) + nowGun.maxRecoil * recoil;
        recoil += nowGun.plusRecoil;
        if (recoil > 1)
            recoil = 1;

        Vector2 SmousePos;
        SmousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        SmousePos.x = SmousePos.x - transform.position.x;
        SmousePos.y = SmousePos.y - transform.position.y;

        float angle = Random.Range(-nowRecoil.x, nowRecoil.x);

        result.x = Mathf.Cos(angle * Mathf.Deg2Rad) * SmousePos.x - Mathf.Sin(angle * Mathf.Deg2Rad) * SmousePos.y;
        result.y = Mathf.Sin(angle * Mathf.Deg2Rad) * SmousePos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * SmousePos.y;

        result = result + result.normalized * nowRecoil.y;

        CursorPos mouPos = new CursorPos();
        GetCursorPos(out mouPos);

        Vector2 screenC = new Vector2();

        screenC.x = Screen.width / GameManager.itemManager.canvas.sizeDelta.x;
        screenC.y = Screen.height / GameManager.itemManager.canvas.sizeDelta.y;

        screenC.x = (result - SmousePos).x * screenC.x;
        screenC.y = (result - SmousePos).y * screenC.y;

        #if UNITY_EDITOR
        screenC *= GetGameViewScale();
        #endif

        mouPos.x += (int)screenC.x;
        mouPos.y -= (int)screenC.y;

        SetCursorPos(mouPos.x, mouPos.y);

    }

#if UNITY_EDITOR

    public float GetGameViewScale()
    {
        var assembly = typeof(UnityEditor.EditorWindow).Assembly;
        var type = assembly.GetType("UnityEditor.GameView");
        var gameview = UnityEditor.EditorWindow.GetWindow(type);

        var areaField = type.GetField("m_ZoomArea", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var areaObj = areaField.GetValue(gameview);

        var scaleField = areaObj.GetType().GetField("m_Scale", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        return ((Vector2)scaleField.GetValue(areaObj)).x;
    }

#endif

    public void Bolt(float cool)
    {
        if (BoltSecond > 0)
            return;

        BoltSecond = cool;

        Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
        string gunContents = GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]];
        int ammoCode = int.Parse(gunContents.Substring(0, 7));
        if(ammoCode != 0)
        {
            DropItem.CreateDropItem(ammoCode, new int[0], 1);
        }

        int[] magazine = Gun.GetMagazineCodePLC(playerGear.GunsPLC[selectGun], true);

        if (magazine[0] == -1)
        {
            gunContents = "0000000" + gunContents.Substring(7, gunContents.Length - 7);
            GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]] = gunContents;
        }
        else
        {
            string magazineContents = GameManager.saveManager.savePLC.plusContainers[magazine[1]];
            if (magazineContents == "")
            {
                gunContents = "0000000" + gunContents.Substring(7, gunContents.Length - 7);
                GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]] = gunContents;
            }
            else
            {
                gunContents = "400" + magazineContents.Substring(magazineContents.Length - 4, 4) + gunContents.Substring(7, gunContents.Length - 7);
                GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]] = gunContents;
                magazineContents = magazineContents.Substring(0, magazineContents.Length - 4);
                GameManager.saveManager.savePLC.plusContainers[magazine[1]] = magazineContents;
            }
        }

        PlayGunSound(nowGun.boltSound, 7);

        GameManager.itemManager.AttachmentContainerReRender(playerGear.nowAttachmentCellGroup);

    }

    public void AttackWithKnighf()
    {
        if (knighfSecond > 0)
            return;

        Vector2 colliderPos = new Vector2(0,0);
        float rotate = 0;

        if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Attack"))
        {
            colliderPos = new Vector2(4, 9);
            rotate = 0;
        }
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PAttack"))
        {
            colliderPos = new Vector2(4, 13);
            rotate = 0;
        }
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LAttack"))
        {
            colliderPos = new Vector2(-10, 4);
            rotate = 90;
        }
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BAttack"))
        {
            colliderPos = new Vector2(-4, -6);
            rotate = 0;
        }
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RAttack"))
        {
            colliderPos = new Vector2(9, 4);
            rotate = 90;
        }

        knighfCollider.transform.localPosition = new Vector3(colliderPos.x, colliderPos.y, 0);
        knighfCollider.transform.localRotation = Quaternion.Euler(0, 0, rotate);

        knighfCollider.GetComponent<Rigidbody2D>().position = knighfCollider.transform.position;
        knighfCollider.GetComponent<Rigidbody2D>().rotation = knighfCollider.transform.rotation.eulerAngles.z;

        Attack.AttackWithCastAOE(knighfCollider, isLow, 60, 2, gameObject, new int[0] { }, false);

        knighfSecond = 0.1f;


    }

    void ProneMove()
    {
        float[] getMovekey = GetMoveKey();

        if (getMovekey == null)
        {
            if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move"))
            {
                BodyPlay("Prone_Aim");
                playerLeg.Play("Prone_Aim");
            }
            nowSpeed = 0;
        }
        else
        {
            SetZoom(false);
            nowSpeed = walkSpeed / 3;
            playerLeg.speed = 1;           
            transform.Translate(getMovekey[0] * nowSpeed * Time.deltaTime, getMovekey[1] * nowSpeed * Time.deltaTime, 0, Space.World);
            playerLeg.transform.position = new Vector3(playerBody.transform.position.x, playerBody.transform.position.y, playerLeg.transform.position.z);
            transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
            playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Move"))
            {
                BodyPlay("Prone_Move");
                playerLeg.Play("Prone_Move");
            }

        }



    }
    void ProneSlide()
    {
        if (stamina < 15)
            return;

        if(isRun)
        isRun = false;
        else
        {
            float[] getMovekey = GetMoveKey();
            if (getMovekey != null)
                transform.rotation = Quaternion.Euler(0, 0, getMovekey[2]);

        }
        playerHead.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        SetBackpack(false);

        nowSpeed = 0;
        playerLeg.speed = 1;
        playerLeg.transform.rotation = transform.rotation;
        SetZoom(false);

        playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
        ProneSlideSecond = (float)0.6;
        stamina -= 15;

        BodyPlay("Stand_Prone");
        playerLeg.Play("Stand_Prone");
        PlayPlayerSound(15, 6);
    }

    void Rolling()
    {
        float[] getMovekey = GetMoveKey();

        if (getMovekey == null || stamina < 15)
            return;
       

        float AR = getMovekey[2] - playerLeg.transform.eulerAngles.z;

        if (AR < 0)
            AR = 360 + AR;


        if(15 <= AR &&  AR <= 165)
        {
            SetBackpack(false);
            stamina -= 15;
            BodyPlay("Prone_LRolling");
            playerLeg.Play("Prone_LRolling");
            SetZoom(false);
            nowSpeed = 0;
            transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
            playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
            PlayPlayerSound(11, 4);
        }
        else if(195 <= AR && AR <= 345)
        {
            SetBackpack(false);
            stamina -= 15;
            BodyPlay("Prone_RRolling");
            playerLeg.Play("Prone_RRolling");
            SetZoom(false);
            nowSpeed = 0;
            transform.rotation = Quaternion.Euler(0, 0, playerLeg.transform.eulerAngles.z);
            playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
            PlayPlayerSound(11, 4);
        }


    }

    float[] GetMoveKey()
    {
        if (Input.GetKey(SettingManager.Key_wasd[0]) && Input.GetKey(SettingManager.Key_wasd[1]))
        {
            return new float[3] { -sqrtHalf, sqrtHalf, 45 };
        }
        else if (Input.GetKey(SettingManager.Key_wasd[1]) && Input.GetKey(SettingManager.Key_wasd[2]))
        {
            return new float[3] { -sqrtHalf, -sqrtHalf, 135 };
        }
        else if (Input.GetKey(SettingManager.Key_wasd[2]) && Input.GetKey(SettingManager.Key_wasd[3]))
        {
            return new float[3] { sqrtHalf, -sqrtHalf, 215 };
        }
        else if (Input.GetKey(SettingManager.Key_wasd[3]) && Input.GetKey(SettingManager.Key_wasd[0]))
        {
            return new float[3] { sqrtHalf, sqrtHalf, 315 };
        }
        else if (Input.GetKey(SettingManager.Key_wasd[0]))
        {
            return new float[3] { 0, 1, 0 };
        }
        else if (Input.GetKey(SettingManager.Key_wasd[1]))
        {
            return new float[3] { -1, 0, 90 };
        }
        else if (Input.GetKey(SettingManager.Key_wasd[2]))
        {
            return new float[3] { 0, -1, 180 };
        }
        else if (Input.GetKey(SettingManager.Key_wasd[3]))
        {
            return new float[3] { 1, 0, 270 };
        }
        else
            return null;


    }

    public void SetZoom(bool set)
    {
        if(set)
        {
            if (isZoom)
                return;

            if(isStand)
            {
                if (!isRun && (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") || isAniStandFire()))
                {
                    isZoom = true;
                    Playersight.SetSight(1);
                    GameManager.mousePointer.SetAimZoom();
                }
                else
                    return;


            }
            else
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") || playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Fire"))
                {
                    isZoom = true;
                    Playersight.SetSight(1);
                    GameManager.mousePointer.SetAimZoom();
                }
                else
                    return;
            }


        }
        else
        {
            if (!isZoom)
                return;

                    isZoom = false;
                    Playersight.SetSight(0);
                    GameManager.mousePointer.SetDirectZoom();


        }


    }

    public void SetBackpack(bool set)
    {
        if(set !=  isOpenBackpack)
        {
            isOpenBackpack = set;
            playerGear.PlayerGearReRendering();

            if(GameManager.itemManager.NowStartType != 0)
                GameManager.itemManager.FailMoveItem();

        }

    }

    public void SetGun(int set)
    {
        if(set != selectGun)
        {
            if(selectGun == 0 && set == 1)
            {
                main1.SetActive(true);
                main2.SetActive(false);
            }
            else if(selectGun == 1 && set == 0)
            {
                main1.SetActive(false);
                main2.SetActive(true);
            }


            if (selectGun == 0 || selectGun == 1)
            {
                if (set == 2 || set == 3)
                {
                    middleGun = selectGun;
                    middleGunObject.SetActive(true);
                    middleGunObject.GetComponent<SpriteRenderer>().material.SetTexture("_Gun", ((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[selectGun])).gunTexture);
                }
            }
            else if(selectGun == 2 || selectGun == 3)
            {
                if (set == 0 || set == 1)
                {
                    middleGun = -1;
                    middleGunObject.SetActive(false);
                }
            }
                


            selectGun = set;
            playerGear.PlayerGearReRendering();

            if (GameManager.itemManager.NowStartType != 0)
                GameManager.itemManager.FailMoveItem();

            if (set != 3)
                playerBody.GetComponent<SpriteRenderer>().material.SetTexture("_Gun", ((Gun)Item.GetItem(playerGear.GearItems[set])).gunTexture);
            else
                playerBody.GetComponent<SpriteRenderer>().material.SetTexture("_Gun",basicTextures[2]);

            if (set == 0 || set == 1)
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Main1Swap_Start"))
                {
                    BodyPlay("Stand_Main2Swap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Main2Swap_Start"))
                {
                    BodyPlay("Stand_Main1Swap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Main1Swap_Start"))
                {
                    BodyPlay("Prone_Main2Swap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Main2Swap_Start"))
                {
                    BodyPlay("Prone_Main1Swap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_PistolSwap_Start"))
                {
                    BodyPlay("Stand_PistolSwap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PistolSwap_Start"))
                {
                    BodyPlay("Prone_PistolSwap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_KnifeMainSwap_Start"))
                {
                    BodyPlay("Stand_PistolSwap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_KnifeMainSwap_Start"))
                {
                    BodyPlay("Prone_PistolSwap_End");
                }

            }
            else if(set == 2)
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_PistolSwap_Start"))
                {
                    BodyPlay("Stand_PistolSwap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PistolSwap_Start"))
                {
                    BodyPlay("Prone_PistolSwap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_KnifePistolSwap_Start"))
                {
                    BodyPlay("Stand_PistolSwap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_KnifePistolSwap_Start"))
                {
                    BodyPlay("Prone_PistolSwap_End");
                }


            }
            else if(set == 3)
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_KnifeSwap_Start"))
                {
                    BodyPlay("Stand_KnifeMainSwap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_KnifeSwap_Start"))
                {
                    BodyPlay("Prone_KnifeMainSwap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_PistolKnifeSwap_Start"))
                {
                    BodyPlay("Stand_KnifePistolSwap_End");
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PistolKnifeSwap_Start"))
                {
                    BodyPlay("Prone_KnifePistolSwap_End");
                }


            }

        }


    }

    public int GetSelectGunFireMode()
    {
        if (selectGun == 0)
            return main1FireMode;
        else if (selectGun == 1)
            return main2FireMode;
        else if (selectGun == 2)
            return pistolFireMode;
        else
            return -1;


    }

    public bool CanGrabItem()
    {
        return (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PReload") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BReload") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LReload") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RReload") ||
           (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_BackpackOpen") && playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) ||
           (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BackpackOpen") && playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            );


    }

    public void GrabToNonBBGrab()
    {
        isBB = false;

        if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_BackpackOpen"))
            BodyPlay("Stand_BackpackOpen", 1.5f);
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BackpackOpen"))
            BodyPlay("Prone_BackpackOpen", 1.5f);
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim"))
            BodyPlay("Stand_Aim", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run"))
            BodyPlay("Stand_Run", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload"))
            BodyPlay("Stand_Reload", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
            BodyPlay("Prone_Aim", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PReload"))
            BodyPlay("Prone_PReload", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BReload"))
            BodyPlay("Prone_BReload", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LReload"))
            BodyPlay("Prone_LReload", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RReload"))
            BodyPlay("Prone_RReload", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);

        if (selectGun < 3)
        {
            Gun nowGun = (Gun)Item.GetItem(playerGear.GearItems[selectGun]);
            PlayGunSound(nowGun.BBSound, 7);
        }

    }

    public void AutoNonBB(bool isMagazine)
    {
        if (isMagazine)
        {
            string gunContents = GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]];

            int[] magazine = Gun.GetMagazineCodePLC(playerGear.GunsPLC[selectGun], true);
            if (magazine[0] == -1)
            {
                Debug.Log("MyError: AutoNonBB But noMagazine");
                return;
            }
            else
            {
                string magazineContents = GameManager.saveManager.savePLC.plusContainers[magazine[1]];
                if (magazineContents == "")
                {
                    return;
                }
                else
                {
                    gunContents = "400" + magazineContents.Substring(magazineContents.Length - 4, 4) + gunContents.Substring(7, gunContents.Length - 7);
                    GameManager.saveManager.savePLC.plusContainers[playerGear.GunsPLC[selectGun]] = gunContents;
                    magazineContents = magazineContents.Substring(0, magazineContents.Length - 4);
                    GameManager.saveManager.savePLC.plusContainers[magazine[1]] = magazineContents;
                    GrabToNonBBGrab();
                }

            }



        }
        else
            GrabToNonBBGrab();

    }

    public bool IsAniProneFIre()
    {
        return (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PFire") ||
            playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LFire") ||
            playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BFire") ||
            playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RFire")
            );

    }

    public bool IsAniProneBolt()
    {
        return (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PBolt") ||
            playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LBolt") ||
            playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BBolt") ||
            playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RBolt")
            );

    }

    public bool isAniProneReload()
    {

        return (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PReload") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BReload") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LReload") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RReload"));

    }

    public bool isAniProneAttack()
    {

        return (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PAttack") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BAttack") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LAttack") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RAttack"));

    }

    public bool isAniStandFire()
    {

        return (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Fire1") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Fire2") ||
           playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Fire3")
          );

    }

    public void ReRenderingPlayerPart()
    {
        if(playerGear.GearItems[0] != 0 && selectGun != 0 && middleGun != 0)
        {
            main1.SetActive(true);
        }
        else
            main1.SetActive(false);


        if (playerGear.GearItems[1] != 0 && selectGun != 1 && middleGun != 1)
        {
            main2.SetActive(true);
        }
        else
            main2.SetActive(false);

        if (middleGun != -1)
            middleGunObject.SetActive(true);
        else
            middleGunObject.SetActive(false);

        if (playerGear.GearItems[6] != 0)
            backpack.SetActive(true);
        else
            backpack.SetActive(false);

        SetTextureSelectGun();

        if (isStand)
        {
            if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run"))
                playerBody.GetComponent<PlayerBody>().StandMain12Animate("Run");
            else if(playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_BackpackOpen"))
            {
                playerBody.GetComponent<PlayerBody>().StandMain12Animate("Aim");
                playerBody.GetComponent<PlayerBody>().OnlyBackpackAnimateWithTime("Stand_Open", 1.0f);
            }
            else
                playerBody.GetComponent<PlayerBody>().StandMain12Animate("Aim");
        }
        else
        {
            if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)
                    playerBody.GetComponent<PlayerBody>().Main12Animate("PAim");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)
                    playerBody.GetComponent<PlayerBody>().Main12Animate("LAim");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                    playerBody.GetComponent<PlayerBody>().Main12Animate("BAim");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)
                    playerBody.GetComponent<PlayerBody>().Main12Animate("RAim");
            }
            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PReload"))
                playerBody.GetComponent<PlayerBody>().Main12Animate("PAim");
            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LReload"))
                playerBody.GetComponent<PlayerBody>().Main12Animate("LAim");
            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BReload"))
                playerBody.GetComponent<PlayerBody>().Main12Animate("BAim");
            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RReload"))
                playerBody.GetComponent<PlayerBody>().Main12Animate("RAim");
            else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BackpackOpen"))
            {
                playerBody.GetComponent<PlayerBody>().Main12Animate("PAim");
                playerBody.GetComponent<PlayerBody>().OnlyBackpackAnimateWithTime("Prone_Open", 1.0f);
            }
        }


    }

    public void ForceChangeToKnife()
    {
       if(selectGun < 2)
        {
            if (playerGear.GearItems[1 - selectGun] != 0)
            {
                middleGun = 1 - selectGun;
                middleGunObject.GetComponent<SpriteRenderer>().material.SetTexture("_Gun", ((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1 - selectGun])).gunTexture);

            }

            selectGun = 3;
            playerBody.GetComponent<SpriteRenderer>().material.SetTexture("_Gun", basicTextures[2]);

            isLow = false;
            isBB = false;
            SetZoom(false);

            if (isStand)
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run"))
                    BodyPlay("Stand_Run");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim"))
                    BodyPlay("Stand_Aim");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload"))
                    BodyPlay("Stand_Aim");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_BackpackOpen"))
                {
                    BodyPlay("Stand_BackpackOpen", 1.0f);
                }
            }
            else
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
                {
                    if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)
                        BodyPlay("Prone_Aim", 0);
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)
                        BodyPlay("Prone_Aim", 0.25f);
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                        BodyPlay("Prone_Aim", 0.5f);
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)
                        BodyPlay("Prone_Aim", 0.75f);
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PReload"))
                    BodyPlay("Prone_Aim");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LReload"))
                    BodyPlay("Prone_Aim",0.25f);
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BReload"))
                    BodyPlay("Prone_Aim",0.5f);
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RReload"))
                    BodyPlay("Prone_Aim",0.75f);
                else
                    BodyPlay("Prone_BackpackOpen", 1.0f);
            }



        }
       else if(selectGun == 2)
        {
            selectGun = 3;
            playerBody.GetComponent<SpriteRenderer>().material.SetTexture("_Gun", basicTextures[2]);

            isLow = false;
            isBB = false;
            SetZoom(false);

            if (isStand)
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run"))
                    BodyPlay("Stand_Run");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim"))
                    BodyPlay("Stand_Aim");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Reload"))
                    BodyPlay("Stand_Aim");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_BackpackOpen"))
                    BodyPlay("Stand_BackpackOpen", 1.0f);
            }
            else
            {
                if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
                {
                    if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)
                        BodyPlay("Prone_Aim", 0);
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)
                        BodyPlay("Prone_Aim", 0.25f);
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                        BodyPlay("Prone_Aim", 0.5f);
                    else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)
                        BodyPlay("Prone_Aim", 0.75f);
                }
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PReload"))
                    BodyPlay("Prone_Aim");
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LReload"))
                    BodyPlay("Prone_Aim",0.25f);
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BReload"))
                    BodyPlay("Prone_Aim",0.5f);
                else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RReload"))
                    BodyPlay("Prone_Aim",0.75f);
                else
                    BodyPlay("Prone_BackpackOpen", 1.0f);
            }




        }




    }

    public int GetProneDirection()
    {
        if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
        {
            if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0)
                return 0;
            else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f)
                return 1;
            else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                return 2;
            else if (playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f)
                return 3;
        }
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PFire"))
            return 0;
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LFire"))
            return 1;
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BFire"))
            return 2;
        else if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RFire"))
            return 3;

        return -1;

    }

    public void LookingWithShoulder(float shoulder)
    {

        Vector2 mousePos;
        mousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = mousePos.x - transform.position.x;
        mousePos.y = mousePos.y - transform.position.y;
        float bunmo = Mathf.Sqrt(mousePos.x * mousePos.x + mousePos.y * mousePos.y);

        if (bunmo < Mathf.Abs(shoulder))
            bunmo = Mathf.Abs(shoulder);

        if (mousePos.y <= 0)
            transform.rotation = Quaternion.Euler(0, 0, (-Mathf.Asin(shoulder / bunmo) - Mathf.Acos(mousePos.x / bunmo)) * 180 / Mathf.PI - 90);
        else
            transform.rotation = Quaternion.Euler(0, 0, (-Mathf.Asin(shoulder / bunmo) + Mathf.Acos(mousePos.x / bunmo)) * 180 / Mathf.PI - 90);


    }

    public void LookingWithShoulderH(float shoulder)
    {

        Vector2 mousePos;
        mousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = mousePos.x - transform.position.x;
        mousePos.y = mousePos.y - transform.position.y;
        float bunmo = Mathf.Sqrt(mousePos.x * mousePos.x + mousePos.y * mousePos.y);

        if (bunmo < Mathf.Abs(shoulder))
            bunmo = Mathf.Abs(shoulder);

        if (mousePos.x <= 0)
            transform.rotation = Quaternion.Euler(0, 0, -(-Mathf.Asin(shoulder / bunmo) - Mathf.Acos(mousePos.y / bunmo)) * 180 / Mathf.PI - 90);
        else
            transform.rotation = Quaternion.Euler(0, 0, -(-Mathf.Asin(shoulder / bunmo) + Mathf.Acos(mousePos.y / bunmo)) * 180 / Mathf.PI - 90);


    }

    public void ProneLook()
    {
        if (selectGun == 0 || selectGun == 1)
        {
            LookingWithProneShoulder(mainProneShoulder,mainBProneShoulder,mainLProneShoulder);
        }
        else if (selectGun == 2)
        {
            LookingWithProneShoulder(pistolProneShoulder, pistolBProneShoulder, pistolLProneShoulder);
        }
        else
        {
            LookingWithProneShoulder(mainProneShoulder, mainBProneShoulder, mainLProneShoulder);
        }
    }

    public void LookingWithProneShoulder(float p,float b, float l)
    {
        float nowR;

        Vector2 mousePos;
        mousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = mousePos.x - transform.position.x;
        mousePos.y = mousePos.y - transform.position.y;
        if (mousePos.x >= 0)
            nowR = Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg - 90;
        else
            nowR = Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg + 90;


        if (nowR < 0)
            nowR = 360 + nowR;

        nowR -= playerLeg.transform.eulerAngles.z;

        if (nowR < 0)
            nowR = 360 + nowR;

        if (playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim"))
        {
            if (nowR > 330 || 30 > nowR)//Pront
            {
                LookingWithShoulder(p);
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
                BodyPlay("Prone_Aim", 0);
                playerLeg.Play("Prone_Aim", 0, 0);
            }
            else if (30 <= nowR && nowR <= 130)//Left
            {
                LookingWithShoulderH(l);
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
                BodyPlay("Prone_Aim", 0.25f);
                playerLeg.Play("Prone_Aim", 0, (float)0.25);
            }
            else if (130 < nowR && nowR < 230)//Back
            {
                LookingWithShoulder(b);
                transform.Rotate(new Vector3(0, 0, 180));
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 180);
                BodyPlay("Prone_Aim", 0.5f);
                playerLeg.Play("Prone_Aim", 0, (float)0.5);
            }
            else//Right
            {
                LookingWithShoulderH(-l);
                transform.Rotate(new Vector3(0, 0, 180));
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 270);
                BodyPlay("Prone_Aim", 0.75f);
                playerLeg.Play("Prone_Aim", 0, (float)0.75);
            }

        }
        else if(IsAniProneFIre())
        {
            if (nowR > 330 || 30 > nowR)//Pront
            {
                LookingWithShoulder(p);
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PFire"))
                {
                    BodyPlay("Prone_PFire", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("PAim");
                }
                playerLeg.Play("Prone_Aim", 0, 0);
            }
            else if (30 <= nowR && nowR <= 130)//Left
            {
                LookingWithShoulderH(l);
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LFire"))
                {
                    BodyPlay("Prone_LFire", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("LAim");
                }
                playerLeg.Play("Prone_Aim", 0, (float)0.25);
            }
            else if (130 < nowR && nowR < 230)//Back
            {
                LookingWithShoulder(b);
                transform.Rotate(new Vector3(0, 0, 180));
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 180);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BFire"))
                {
                    BodyPlay("Prone_BFire", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("BAim");
                }
                playerLeg.Play("Prone_Aim", 0, (float)0.5);
            }
            else//Right
            {
                LookingWithShoulderH(-l);
                transform.Rotate(new Vector3(0, 0, 180));
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 270);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RFire"))
                {
                    BodyPlay("Prone_RFire", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("RAim");
                }
                playerLeg.Play("Prone_Aim", 0, (float)0.75);
            }
        }
        else if (IsAniProneBolt())
        {
            if (nowR > 330 || 30 > nowR)//Pront
            {
                LookingWithShoulder(p);
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PBolt"))
                {
                    BodyPlay("Prone_PBolt", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("PAim");
                }
                playerLeg.Play("Prone_Aim", 0, 0);
            }
            else if (30 <= nowR && nowR <= 130)//Left
            {
                LookingWithShoulderH(l);
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LBolt"))
                {
                    BodyPlay("Prone_LBolt", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("LAim");
                }
                playerLeg.Play("Prone_Aim", 0, (float)0.25);
            }
            else if (130 < nowR && nowR < 230)//Back
            {
                LookingWithShoulder(b);
                transform.Rotate(new Vector3(0, 0, 180));
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 180);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BBolt"))
                {
                    BodyPlay("Prone_BBolt", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("BAim");
                }
                playerLeg.Play("Prone_Aim", 0, (float)0.5);
            }
            else//Right
            {
                LookingWithShoulderH(-l);
                transform.Rotate(new Vector3(0, 0, 180));
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 270);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RBolt"))
                {
                    BodyPlay("Prone_RBolt", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("RAim");
                }
                playerLeg.Play("Prone_Aim", 0, (float)0.75);
            }
        }
        else if (isAniProneAttack())
        {
            if (nowR > 330 || 30 > nowR)//Pront
            {
                LookingWithShoulder(p);
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PAttack"))
                {
                    BodyPlay("Prone_PAttack", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("PAim");
                }
                playerLeg.Play("Prone_Aim", 0, 0);
            }
            else if (30 <= nowR && nowR <= 130)//Left
            {
                LookingWithShoulderH(l);
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LAttack"))
                {
                    BodyPlay("Prone_LAttack", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("LAim");
                }
                playerLeg.Play("Prone_Aim", 0, (float)0.25);
            }
            else if (130 < nowR && nowR < 230)//Back
            {
                LookingWithShoulder(b);
                transform.Rotate(new Vector3(0, 0, 180));
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 180);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BAttack"))
                {
                    BodyPlay("Prone_BAttack", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("BAim");
                }
                playerLeg.Play("Prone_Aim", 0, (float)0.5);
            }
            else//Right
            {
                LookingWithShoulderH(-l);
                transform.Rotate(new Vector3(0, 0, 180));
                playerHead.transform.localRotation = Quaternion.Euler(0, 0, 270);
                if (!playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RAttack"))
                {
                    BodyPlay("Prone_RAttack", playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    playerBody.GetComponent<PlayerBody>().Main12Animate("RAim");
                }
                playerLeg.Play("Prone_Aim", 0, (float)0.75);
            }
        }


    }

    public void SetTextureSelectGun()
    {
        if (selectGun != 3)
            playerBody.GetComponent<SpriteRenderer>().material.SetTexture("_Gun", ((Gun)Item.GetItem(playerGear.GearItems[selectGun])).gunTexture);
        else
            playerBody.GetComponent<SpriteRenderer>().material.SetTexture("_Gun", basicTextures[2]);

        if(playerGear.GearItems[0] != 0)
            main1.GetComponent<SpriteRenderer>().material.SetTexture("_Gun", ((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunTexture);

        if (playerGear.GearItems[1] != 0)
            main2.GetComponent<SpriteRenderer>().material.SetTexture("_Gun", ((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunTexture);


        if (middleGun >= 0)
        middleGunObject.GetComponent<SpriteRenderer>().material.SetTexture("_Gun", ((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[middleGun])).gunTexture);
    }

    public void StatusEffect()
    {
        if (blood > 0)
        {
            HP -= blood * Time.deltaTime;
        }

        if (poison > 0)
        {
            HP -= Time.deltaTime;
            poison -= Time.deltaTime;
        }

    }
    public void NoHitDead()
    {
        if (isStand)
        {
            Dead(true, transform.eulerAngles.z);
        }
        else
        {
            if (playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Prone_Aim") && playerLeg.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f)
                Dead(false, GameManager.player.playerLeg.transform.eulerAngles.z);
            else
                Dead(true, GameManager.player.playerLeg.transform.eulerAngles.z);
        }
    }


    public void Dead(bool pront, float angle)
    {
        PlayPlayerSound(18, 7);

        isDead = true;

        isStand = false;       
        playerBody.transform.localPosition = new Vector3(0, -7, 0);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        playerLeg.transform.position = new Vector3(playerBody.transform.position.x, playerBody.transform.position.y,playerLeg.transform.position.z);

        isRun = false;
        playerHead.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        SetBackpack(false);

        nowSpeed = 0;
        playerLeg.speed = 1;
        playerLeg.transform.rotation = transform.rotation;
        SetZoom(false);

        playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);

        if (pront)       
            playerBody.Play("Prone_Dead");
        else
            playerBody.Play("BProne_Dead");
        playerLeg.Play("Prone_Aim", 0, (float)0.5);      
    }

    public void PlayPlayerSound(int index, float loudness)
    {
        audioSource.clip = playerSounds[index];
        audioSource.Play();
        MakeSound(loudness);
    }

    public void PlayGunSound(AudioClip clip, float loudness)
    {
        bodyAudioSource.clip = clip;
        bodyAudioSource.Play();
        MakeSound(loudness);
    }

    public void PlayVocalSound(int index, float loudness)
    {
        vocalAudioSource.clip = vocalSounds[index];
        vocalAudioSource.Play();
        MakeSound(loudness);
    }

    public void PlayHitSound(AudioClip clip, float loudness)
    {
        hitAudioSource.clip = clip;
        hitAudioSource.Play();
        MakeSound(loudness);
    }
    public void MakeSound(float loudness)
    {
        AI.MakeSound(gameObject, 0, transform.position, loudness);
    }


    public void BodyPlay(string animationName)
    {
        if (GameManager.player.selectGun == 0)
        {
            if(isBB && playerBody.HasState(0,Animator.StringToHash(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_" + animationName + "_BB")))
            {
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_" + animationName +"_BB");
            }
            else
             playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_" + animationName);
        }
        else if (GameManager.player.selectGun == 1)
        {
            if (isBB && playerBody.HasState(0, Animator.StringToHash(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_" + animationName + "_BB")))
            {
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_" + animationName + "_BB");
            }
            else
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_" + animationName);
        }
        else if (GameManager.player.selectGun == 2)
        {
            if (isBB && playerBody.HasState(0, Animator.StringToHash(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[2])).gunAnimation.ToString() + "_" + animationName + "_BB")))
            {
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[2])).gunAnimation.ToString() + "_" + animationName + "_BB");
            }
            else
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[2])).gunAnimation.ToString() + "_" + animationName);
        }
        else
        {
            playerBody.Play("Knife" + "_" + animationName);
        }

       playerBody.Update(0f);
    }

    public void BodyPlay(string animationName,float nt)
    {
        if (GameManager.player.selectGun == 0)
        {
            if (isBB && playerBody.HasState(0, Animator.StringToHash(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_" + animationName + "_BB")))
            {
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_" + animationName + "_BB",0,nt);
            }
            else
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_" + animationName, 0, nt);
        }
        else if (GameManager.player.selectGun == 1)
        {
            if (isBB && playerBody.HasState(0, Animator.StringToHash(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_" + animationName + "_BB")))
            {
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_" + animationName + "_BB", 0, nt);
            }
            else
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_" + animationName, 0, nt);
        }
        else if (GameManager.player.selectGun == 2)
        {
            if (isBB && playerBody.HasState(0, Animator.StringToHash(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[2])).gunAnimation.ToString() + "_" + animationName + "_BB")))
            {
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[2])).gunAnimation.ToString() + "_" + animationName + "_BB", 0, nt);
            }
            else
                playerBody.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[2])).gunAnimation.ToString() + "_" + animationName, 0, nt);
        }
        else
        {
            playerBody.Play("Knife" + "_" + animationName, 0, nt);
        }

       playerBody.Update(0f);
    }


}
