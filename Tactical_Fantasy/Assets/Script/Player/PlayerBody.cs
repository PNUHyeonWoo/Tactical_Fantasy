using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    public Vector3 PlayerPositon;
    public Animator main1;
    public Animator main2;
    public Animator backpack;
    public Animator middleGun;
    public Animator magazine;

    public void StartPlayerBody()
    {
        main1 = transform.Find("Main1").GetComponent<Animator>();
        main2 = transform.Find("Main2").GetComponent<Animator>();
        backpack = transform.Find("Backpack").GetComponent<Animator>();
        middleGun = transform.Find("MiddleGun").GetComponent<Animator>();
        magazine = transform.Find("Magazine").GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Prone") && GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.05 && GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95)
        {        
            GameManager.player.playerBody.transform.localPosition = new Vector3(0, GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime * -7, 0);
            GameManager.player.playerLeg.transform.position = new Vector3(GameManager.player.playerBody.transform.position.x, GameManager.player.playerBody.transform.position.y, GameManager.player.playerLeg.transform.position.z);
            if (GameManager.player.ProneSlideSecond <= 0)            
                GameManager.player.GetComponent<Rigidbody2D>().velocity = transform.up * 7;
            
        }
        else if(GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Stand") && GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.05 && GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95)
        {           
            GameManager.player.playerBody.transform.localPosition = new Vector3(0, GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime * 7 - 7, 0);
            GameManager.player.playerLeg.transform.position = new Vector3(GameManager.player.playerBody.transform.position.x, GameManager.player.playerBody.transform.position.y, GameManager.player.playerLeg.transform.position.z);
            if (GameManager.player.ProneSlideSecond <= 0)
                GameManager.player.GetComponent<Rigidbody2D>().velocity = transform.up * -7;
        }

    }

    public void SetIsStand(int i)
    {
        if (i == 0)
        {
            GameManager.player.isStand = false;
            GameManager.player.playerBody.transform.localPosition = new Vector3(0, -7, 0);
            GameManager.player.playerLeg.transform.position = new Vector3(GameManager.player.playerBody.transform.position.x, GameManager.player.playerBody.transform.position.y, GameManager.player.playerLeg.transform.position.z);
            GameManager.player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
        else
        {
            GameManager.player.isStand = true;
            GameManager.player.playerBody.transform.localPosition = new Vector3(0, 0, 0);
            GameManager.player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }

    public void SaveNowPoisition()
    {
        PlayerPositon = GameManager.player.transform.position;
    }


    public void StandMain12Animate(string name) // +backpackAnimate
    {
        if (main1.gameObject.activeSelf)
        {
            main1.Play("Main1_" + name);
        }

        if (main2.gameObject.activeSelf)
        {
            main2.Play("Main2_" + name);
        }

        if(backpack.gameObject.activeSelf)
        {
            backpack.Play("Backpack_" + (GameManager.itemManager.playerGear.GearItems[6] % 10000).ToString() + "_" + name);
        }

        if(middleGun.gameObject.activeSelf)
        {
            if (GameManager.player.middleGun == 0)
                middleGun.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_MiddleGun_" + name);
            else if(GameManager.player.middleGun == 1)
                middleGun.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_MiddleGun_" + name);
        }
        

    }


    public void Main12Animate(string name)  // +backpackAnimate
    {
        if(main1.gameObject.activeSelf)
        {
            main1.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_Main1_" + name);
        }

        if(main2.gameObject.activeSelf)
        {
            main2.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_Main2_" + name);
        }

        if (backpack.gameObject.activeSelf)
        {
            backpack.Play("Backpack_" + (GameManager.itemManager.playerGear.GearItems[6] % 10000).ToString() + "_" + name);
        }

        if(middleGun.gameObject.activeSelf)
        {
            if (GameManager.player.middleGun == 0)
                middleGun.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_MiddleGun_" + name);
            else if (GameManager.player.middleGun == 1)
                middleGun.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_MiddleGun_" + name);
        }


        if(name == "LAim"  || name == "RAim")
        {
            if(GameManager.player.selectGun < 2 && Gun.CheckVisualMagzine((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[GameManager.player.selectGun])))
            {
                string contents = GameManager.saveManager.savePLC.plusContainers[GameManager.itemManager.playerGear.GunsPLC[GameManager.player.selectGun]];
                if(contents.Contains("p"))
                {
                    magazine.gameObject.SetActive(true);

                    if (GameManager.player.IsAniProneFIre())
                    {
                        if (name == "LAim")
                        {
                            magazine.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[GameManager.player.selectGun])).gunAnimation.ToString() + "_Magazine_LFire");
                        }
                        else
                        {
                            magazine.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[GameManager.player.selectGun])).gunAnimation.ToString() + "_Magazine_RFire");
                        }

                    }
                    else if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("Prone_Rolling"))
                    {
                        if(name == "LAim")
                        {
                            magazine.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[GameManager.player.selectGun])).gunAnimation.ToString() + "_Magazine_LRolling");
                        }
                        else
                        {
                            magazine.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[GameManager.player.selectGun])).gunAnimation.ToString() + "_Magazine_RRolling");
                        }

                    }
                    else
                        magazine.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[GameManager.player.selectGun])).gunAnimation.ToString() + "_Magazine_" + name);


                }
                else
                    magazine.gameObject.SetActive(false);
            }
           else
                magazine.gameObject.SetActive(false);
        }
        else
            magazine.gameObject.SetActive(false);


    }

    public void OnlyMain1Animate(string name)
    {
        if (main1.gameObject.activeSelf)
        {
            main1.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[0])).gunAnimation.ToString() + "_Main1_" + name);
        }

    }

    public void OnlyMain2Animate(string name)
    {
        if (main2.gameObject.activeSelf)
        {
            main2.Play(((Gun)Item.GetItem(GameManager.itemManager.playerGear.GearItems[1])).gunAnimation.ToString() + "_Main2_" + name);
        }

    }

    public void OnlyBackpackAnimate(string name)
    {
        if (backpack.gameObject.activeSelf)
        {
            backpack.Play("Backpack_" + (GameManager.itemManager.playerGear.GearItems[6] % 10000).ToString() + "_" + name);
        }
    }

    public void OnlyBackpackAnimateWithTime(string name, float f)
    {
        if (backpack.gameObject.activeSelf)
        {
            backpack.Play("Backpack_" + (GameManager.itemManager.playerGear.GearItems[6] % 10000).ToString() + "_" + name,0,f);
        }

    }

    public void SetBackpack(int i)
    {
        if (i == 0)
            GameManager.player.SetBackpack(false);
        else
            GameManager.player.SetBackpack(true);
    }

    public void SetGun(int i)
    {
        if(0 <= i && i <= 3)
        {
            if ((GameManager.player.selectGun == 2 || GameManager.player.selectGun == 3) && (i == 0 || i == 1))
            GameManager.player.SetGun(GameManager.player.middleGun);
            else
            GameManager.player.SetGun(i);

            if (GameManager.player.isStand)
                StandMain12Animate("Aim");
            else
                Main12Animate("PAim");

        }


    }

    public void Bolt(float cool)
    {
        GameManager.player.Bolt(cool);
    }

    public void AttackWithKnighf()
    {
        GameManager.player.AttackWithKnighf();
    }

}
