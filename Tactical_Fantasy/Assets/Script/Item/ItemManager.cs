using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemManager : MonoBehaviour
{
    public Font amountFont;
    public GameObject mouseItemPrefabs;
    public GameObject[] cellPrefabs;
    public GameObject[] gearCellPrefabs;
    public Sprite[] gearEmptyCellSprites;
    public Material NoLineItemMaterial;
    public GameObject[] itemMenuPrefabs;
    public GameObject moveAmountBox;
    public GameObject dropItemPrefabs;
    public GameObject[] sightObject;
    public GameObject wallDefaultBloodPrefab;
    public float CanvasHalfX = 480;
    public float CanvasHalfY = 270;
    public RectTransform canvas;
    public Camera mainCamera;
    public PlayerGear playerGear;

    public enum TypeOfStart
    {
        None = 0,
        Container = 1,
        AttachmentCell = 2,
        PlayerGear = 3
    }
    public int NowStartType = 0;
    public object NowStartObject;
    public GameObject mouseItem;
    public bool mouseItemIsVertical;

    public enum TypeOfDestination
    {
        None = 0,
        ContainerCell = 1,
        ContainerItem = 2,
        AttachmentCell = 3,
        AttachmentItem= 4,
        PlayerGearCell = 5,
    }
    public int NowDestinationType = 0;
    public object NowDestinationObject;

    public void StartManager()
    {
        mouseItem = null;
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerGear = GameManager.saveManager.playerGear;
    }

    public void ReStartManager()
    {
        mouseItem = null;
        NowStartType = 0;
        NowStartObject = null;
        NowDestinationType = 0;
        NowDestinationObject = null;
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Start()
    {
        
    }


    void Update()
    {
        if (NowStartType == 0)
            return;

        if(!GameManager.player.CanGrabItem() || GameManager.player.itemMenu != null)
        {
            FailMoveItem();
            return;
        }

        IfExistStartupdate();

        if (NowStartType ==1) 
        {
            
            if (Input.GetMouseButtonUp(0))
            {
                if (NowStartObject == NowDestinationObject) //Error defense
                {
                    FailMoveItem();
                    return;
                }

                ItemImage startItemImage = ((ItemImage)NowStartObject);

                switch (NowDestinationType)
                {
                 case 0:
                        if(!GameManager.player.isInShop)
                        FailMoveItem();
                        else
                        {
                            if(GameManager.player.shop.destinationCell != null)
                            {
                                if(GameManager.player.shop.destinationCell.GetType() == typeof(ShopCell))//상점 판매
                                {
                                    FailMoveItem();
                                    GameManager.player.shop.CreateSellBox(startItemImage);

                                }
                                else if(GameManager.player.shop.destinationCell.GetType() == typeof(QuestCell))//퀘스트 제출
                                {
                                    FailMoveItem();
                                    QuestCell questCell = ((QuestCell)(GameManager.player.shop.destinationCell));
                                    questCell.merchantUI.SubmitItem(questCell.localIndex, startItemImage , questCell.questPage);
                                }
                                else
                                    FailMoveItem();

                            }
                            else
                                FailMoveItem();
                        }
                        break;
                 case 1: //start컨테이너 -> Destination컨테이너 셀
                        if (!Input.GetKey(KeyCode.LeftControl) || ((ItemImage)NowStartObject).amount == 1)
                        {                            
                            ItemCell destinationItemCell = ((ItemCell)NowDestinationObject);
                            int[] XY = GetInputXY(destinationItemCell.cellX, destinationItemCell.cellY, startItemImage.itemCode, mouseItemIsVertical);

                            if (((MonoBehaviour)NowStartObject).transform.parent == ((MonoBehaviour)NowDestinationObject).transform.parent)
                            {
                                if (startItemImage.thisContainer.MoveItemToSelf(startItemImage.x, startItemImage.y, mouseItemIsVertical, XY[0], XY[1]))
                                    SuccessMoveItem(true);
                                else
                                    FailMoveItem();
                            }
                            else
                            {                               
                                if (startItemImage.thisContainer.MoveItemToContainerCell(startItemImage.x, startItemImage.y, mouseItemIsVertical, destinationItemCell.transform.parent.GetComponent<CellGroup>().container, XY[0], XY[1]))
                                    SuccessMoveItem(true);
                                else
                                    FailMoveItem();
                            }                          
                        }
                        else
                        {
                            ItemCell destinationItemCell = ((ItemCell)NowDestinationObject);
                            int[] XY = GetInputXY(destinationItemCell.cellX, destinationItemCell.cellY, startItemImage.itemCode, mouseItemIsVertical);
                            if (destinationItemCell.transform.parent.GetComponent<CellGroup>().container.CanInsertItem(startItemImage.itemCode,mouseItemIsVertical,XY[0],XY[1]))
                            {


                                GameObject AmountUI = CreateSliderUI(0, startItemImage.amount);
                                Slider slider = AmountUI.transform.Find("Slider").GetComponent<Slider>();

                                AmountUI.transform.Find("Divide").GetComponent<Button>().onClick.AddListener
                                    (
                                     delegate
                                     {
                                         NowStartObject = startItemImage;
                                         NowStartType = 1;
                                         NowDestinationObject = destinationItemCell;
                                         NowDestinationType = 1;

                                         if(slider.value == 0)
                                         {                                      
                                                                           
                                         }
                                         else if(slider.value == slider.maxValue)
                                         {
                                             if (startItemImage.thisContainer.MoveItemToContainerCell(startItemImage.x, startItemImage.y, mouseItemIsVertical, destinationItemCell.transform.parent.GetComponent<CellGroup>().container, XY[0], XY[1]))
                                                 SuccessMoveItem(false);
                                             else
                                                 Debug.Log("MyError: insert Check Error");
                                                                                   
                                         }
                                         else
                                         {
                                             if (destinationItemCell.transform.parent.GetComponent<CellGroup>().container.InsertItem(startItemImage.itemCode, new int[1], (int)slider.value, mouseItemIsVertical, XY[0], XY[1]))
                                             {
                                                 if (!startItemImage.thisContainer.RemoveItem((int)slider.value, startItemImage.x, startItemImage.y))
                                                     Debug.Log("MyError: Serious Error!!!!!!!!! only insert no remove (insert Check Error)");
                                             }
                                             else
                                                 Debug.Log("MyError: insert Check Error");


                                             SuccessMoveItem(false);

                                         }


                                         NowStartObject = null;
                                         NowStartType = 0;
                                         NowDestinationObject = null;
                                         NowDestinationType = 0;
                                         Destroy(AmountUI);
                                         GameManager.GamePlay();


                                     }
                                    );

                                

                               
                            }
                            else
                                FailMoveItem();



                        }
                        break;
                 case 2://start컨테이너 -> Destination컨테이너 아이템
                        if (!Input.GetKey(KeyCode.LeftControl) || ((ItemImage)NowStartObject).amount == 1 || ((ItemImage)NowDestinationObject).itemCode / 10000 == 201)
                        {
                            ItemImage destinationItemImage = ((ItemImage)NowDestinationObject);

                            if(!Item.CheckPLC(destinationItemImage.itemCode)) //아이템 겹치기
                            {
                                if (startItemImage.itemCode == destinationItemImage.itemCode && Item.GetItem(startItemImage.itemCode).overlapLimit > 1)
                                {
                                  int remainAmount = Item.GetItem(startItemImage.itemCode).overlapLimit - (startItemImage.amount + destinationItemImage.amount);

                                    if(remainAmount >= 0)
                                    {
                                        startItemImage.thisContainer.RemoveItem(startItemImage.x, startItemImage.y);
                                        destinationItemImage.thisContainer.RemoveItem(-startItemImage.amount, destinationItemImage.x, destinationItemImage.y);
                                        SuccessMoveItem(true);
                                    }
                                    else 
                                    {                                        
                                        int moveAmount = startItemImage.amount + remainAmount;
                                        if (moveAmount >= 1)
                                        {
                                            startItemImage.thisContainer.RemoveItem(moveAmount,startItemImage.x, startItemImage.y);
                                            destinationItemImage.thisContainer.RemoveItem(-moveAmount, destinationItemImage.x, destinationItemImage.y);
                                            SuccessMoveItem(true);
                                       }
                                        else
                                            FailMoveItem();

                                    }

                                }
                                else
                                    FailMoveItem();


                            }
                            else
                            {
                                if(destinationItemImage.itemCode / 10000 == 301) // 탄창삽입
                                {
                                    Magazine destinationMagazine = ((Magazine)Item.GetItem(destinationItemImage.itemCode));
                                    if (startItemImage.itemCode / 10000 == 400 && Magazine.CheckAmmo(destinationMagazine.kindOfMagazine, ((Ammo)(Item.GetItem(startItemImage.itemCode))).kindOfAmmo))
                                    {
                                        PlusContainerManager destinationPLCManager = destinationItemImage.thisContainer.GetThisPLCManager();
                                        string magazineContents = destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]];
                                        int remainAmount = destinationMagazine.magazineSize - (startItemImage.amount + magazineContents.Length / 4);

                                        if(remainAmount >= 0)
                                        {
                                            string AmmoStr = startItemImage.itemCode.ToString();
                                            AmmoStr = AmmoStr.Substring(3, 4);
                                            for(int i = 0;i<startItemImage.amount;i++)
                                            {
                                                magazineContents += AmmoStr;
                                            }
                                            destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]] = magazineContents;                                         
                                            startItemImage.thisContainer.RemoveItem(startItemImage.x, startItemImage.y);
                                            SuccessMoveItem(true);                  
                                        }
                                        else
                                        {
                                            int moveAmount = startItemImage.amount + remainAmount;
                                            if (moveAmount >= 1)
                                            {
                                                startItemImage.thisContainer.RemoveItem(moveAmount, startItemImage.x, startItemImage.y);
                                                string AmmoStr = startItemImage.itemCode.ToString();
                                                AmmoStr = AmmoStr.Substring(3, 4);
                                                for (int i = 0; i < moveAmount; i++)
                                                {
                                                    magazineContents += AmmoStr;
                                                }
                                                destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]] = magazineContents;
                                                SuccessMoveItem(true);
                                            }
                                            else
                                                FailMoveItem();


                                        }


                                    }
                                    else
                                        FailMoveItem();

                                }
                                else if(destinationItemImage.itemCode / 10000 == 204 || destinationItemImage.itemCode / 10000 == 205)
                                {
                                    if(destinationItemImage.itemCode / 10000 == 204) //chestrig
                                    {
                                        ChestRig chestRig = (ChestRig)Item.GetItem(destinationItemImage.itemCode);
                                        Container[] chestRogContainers = new Container[chestRig.rigCells.Length];
                                        for(int i = 0; i < chestRig.rigCells.Length; i++)
                                        {
                                            chestRogContainers[i] = new Container(destinationItemImage.PLCcode[i], destinationItemImage.thisContainer.isNeedSave, chestRig.rigCells[i].xsize, chestRig.rigCells[i].ysize);
                                        }

                                        bool successInput = false;

                                        for (int i = 0; i < chestRogContainers.Length; i++)
                                        {
                                            

                                            for (int y = 0; y < chestRogContainers[i].ySize; y++)
                                            {
                                                for (int x = 0; x < chestRogContainers[i].xSize; x++)
                                                {
                                                
                                                        if (startItemImage.thisContainer.MoveItemToContainerCell(startItemImage.x, startItemImage.y, true, chestRogContainers[i], x, y))
                                                        {
                                                            successInput = true;
                                                            SuccessMoveItem(true);
                                                            break;
                                                        }
                                                        else if (startItemImage.thisContainer.MoveItemToContainerCell(startItemImage.x, startItemImage.y, false, chestRogContainers[i], x, y))
                                                        {
                                                            successInput = true;
                                                            SuccessMoveItem(true);
                                                            break;
                                                        }


                                                }
                                                if (successInput)
                                                {
                                                    break;
                                                }

                                            }
                                            if (successInput)
                                            {
                                                break;
                                            }

                                        }

                                        if(!successInput)
                                            FailMoveItem();

                                    }
                                    else //backpack
                                    {
                                        Backpack backpack = (Backpack)Item.GetItem(destinationItemImage.itemCode);
                                        Container backpackContainer = new Container(destinationItemImage.PLCcode[0], destinationItemImage.thisContainer.isNeedSave, backpack.xSpace, backpack.ySpace);
                                        bool[,] boolmap = backpackContainer.GetBoolMap(backpackContainer.GetContents());
                                        bool successInput = false;

                                        for(int y = 0;y < backpack.ySpace;y++)
                                        {
                                            for(int x = 0; x < backpack.xSpace;x++)
                                            {
                                                if(!boolmap[y,x])
                                                {
                                                    if (startItemImage.thisContainer.MoveItemToContainerCell(startItemImage.x, startItemImage.y, true, backpackContainer, x, y))
                                                    {
                                                        successInput = true;
                                                        SuccessMoveItem(true);
                                                        break;
                                                    }
                                                    else if(startItemImage.thisContainer.MoveItemToContainerCell(startItemImage.x, startItemImage.y, false, backpackContainer, x, y))
                                                    {
                                                        successInput = true;
                                                        SuccessMoveItem(true);
                                                        break;
                                                    }


                                                }


                                            }
                                            if(successInput)
                                            {
                                                break;
                                            }

                                        }

                                        if (!successInput)
                                            FailMoveItem();


                                    }



                                }
                                else //gun
                                {                                    
                                    Gun gun = (Gun)Item.GetItem(destinationItemImage.itemCode);
                                    if(gun.gunAnimation == Gun.GunAnimation.Boltaction || gun.gunAnimation == Gun.GunAnimation.PumpShotgun || gun.gunAnimation == Gun.GunAnimation.DoubleBarrel)
                                    {
                                        if(startItemImage.itemCode / 10000 != 400)
                                        {
                                            AttachmentContainer GunContainer = new AttachmentContainer(destinationItemImage.PLCcode[0], destinationItemImage.thisContainer.isNeedSave, destinationItemImage.itemCode, gun.attachmentCells);

                                            bool succesInput = false;
                                            for (int i = 0; i < gun.attachmentCells.Length; i++)
                                            {
                                                if (startItemImage.thisContainer.MoveItemToAttachemntCell(startItemImage.x, startItemImage.y, GunContainer, i))
                                                {
                                                    succesInput = true;
                                                    SuccessMoveItem(true);
                                                    break;

                                                }
                                            }

                                            if (!succesInput)
                                                FailMoveItem();
                                        }
                                        else
                                        {
                                            if (Magazine.CheckAmmo(gun.attachmentCells[gun.attachmentCells.Length - 1].kindOfMagazine, ((Ammo)Item.GetItem(startItemImage.itemCode)).kindOfAmmo))
                                            {
                                                PlusContainerManager destinationPLCManager = destinationItemImage.thisContainer.GetThisPLCManager();
                                                string GunContents = destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]];
                                                string magazineCode = "";
                                                string magazinePLC = "";
                                                
                                                    int i = 0;
                                                while(GunContents[i] != 'p')
                                                {
                                                    i++;
                                                }
                                                i -= 7;

                                                for (int j = 0; j < 7; j++)
                                                {
                                                    magazineCode += GunContents[i];
                                                    i++;
                                                }


                                                i++;
                                                while(i < GunContents.Length)
                                                {
                                                    magazinePLC += GunContents[i];
                                                    i++;
                                                }
                                                string magazineContents = destinationPLCManager.plusContainers[int.Parse(magazinePLC)];
                                                int magazineSize = ((Magazine)Item.GetItem(int.Parse(magazineCode))).magazineSize;

                                                if(magazineContents.Length/4 < magazineSize)
                                                {
                                                    string AmmoStr = startItemImage.itemCode.ToString();
                                                    AmmoStr = AmmoStr.Substring(3, 4);
                                                    magazineContents += AmmoStr;
                                                    destinationPLCManager.plusContainers[int.Parse(magazinePLC)] = magazineContents;
                                                    
                                                    if(startItemImage.amount == 1)
                                                    {
                                                        startItemImage.thisContainer.RemoveItem(startItemImage.x, startItemImage.y);
                                                        SuccessMoveItem(true);
                                                    }
                                                    else
                                                    {
                                                        startItemImage.thisContainer.RemoveItem(1,startItemImage.x, startItemImage.y);
                                                        SuccessMoveItem(true);
                                                    }

                                                }
                                                else
                                                {
                                                    //약실넣기
                                                    AttachmentContainer GunContainer = new AttachmentContainer(destinationItemImage.PLCcode[0], destinationItemImage.thisContainer.isNeedSave, destinationItemImage.itemCode, gun.attachmentCells);
                                                    if (startItemImage.thisContainer.MoveItemToAttachemntCell(startItemImage.x, startItemImage.y, GunContainer, 0))
                                                    {
                                                        SuccessMoveItem(true);
                                                    }
                                                    else
                                                    {
                                                        FailMoveItem();
                                                    }
                                                }
                                            }
                                            else
                                                FailMoveItem();
                                        }



                                    }
                                    else // nomal Attachment
                                    {
                                        AttachmentContainer GunContainer = new AttachmentContainer(destinationItemImage.PLCcode[0], destinationItemImage.thisContainer.isNeedSave, destinationItemImage.itemCode, gun.attachmentCells);

                                        bool succesInput = false;
                                        for(int i = 0; i < gun.attachmentCells.Length; i++)
                                        {
                                            if (startItemImage.thisContainer.MoveItemToAttachemntCell(startItemImage.x, startItemImage.y, GunContainer, i))
                                            {
                                                succesInput = true;
                                                SuccessMoveItem(true);
                                                break;

                                            }

                                        }

                                        if (!succesInput)
                                            FailMoveItem();



                                    }

                                }

                            }



                        }
                        else //Amount Slide Move
                        {
                            ItemImage destinationItemImage = ((ItemImage)NowDestinationObject);


                            if (!Item.CheckPLC(destinationItemImage.itemCode)) // nomal item
                            {
                                if (startItemImage.itemCode == destinationItemImage.itemCode && Item.GetItem(startItemImage.itemCode).overlapLimit > 1 && destinationItemImage.amount < Item.GetItem(destinationItemImage.itemCode).overlapLimit)
                                {
                                    int remainAmount = Item.GetItem(startItemImage.itemCode).overlapLimit - (startItemImage.amount + destinationItemImage.amount);
                                    int maxMoveAmount;

                                    if (remainAmount >= 0)
                                        maxMoveAmount = startItemImage.amount;
                                    else
                                        maxMoveAmount = startItemImage.amount + remainAmount;

                                    GameObject AmountUI = CreateSliderUI(0, maxMoveAmount);
                                    Slider slider = AmountUI.transform.Find("Slider").GetComponent<Slider>();

                                    AmountUI.transform.Find("Divide").GetComponent<Button>().onClick.AddListener
                                        (
                                        delegate
                                        {
                                            NowStartObject = startItemImage;
                                            NowStartType = 1;
                                            NowDestinationObject = destinationItemImage;
                                            NowDestinationType = 2;

                                            if(slider.value == 0)
                                            {

                                            }
                                            else if(slider.value >= startItemImage.amount)
                                            {
                                                startItemImage.thisContainer.RemoveItem(startItemImage.x, startItemImage.y);
                                                destinationItemImage.thisContainer.RemoveItem(-startItemImage.amount, destinationItemImage.x, destinationItemImage.y);
                                                SuccessMoveItem(false);
                                            }
                                            else
                                            {
                                                startItemImage.thisContainer.RemoveItem((int)slider.value,startItemImage.x, startItemImage.y);
                                                destinationItemImage.thisContainer.RemoveItem(-(int)slider.value, destinationItemImage.x, destinationItemImage.y);
                                                SuccessMoveItem(false);
                                            }

                                            NowStartObject = null;
                                            NowStartType = 0;
                                            NowDestinationObject = null;
                                            NowDestinationType = 0;
                                            Destroy(AmountUI);
                                            GameManager.GamePlay();

                                        }
                                        );




          
                                }
                                else
                                    FailMoveItem();


                            }
                            else
                            {
                                if(destinationItemImage.itemCode / 10000 == 301) // Magazine
                                {
                                    Magazine destinationMagazine = ((Magazine)Item.GetItem(destinationItemImage.itemCode));
                                    PlusContainerManager destinationPLCManager = destinationItemImage.thisContainer.GetThisPLCManager();
                                    string magazineContents = destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]];

                                    if (startItemImage.itemCode / 10000 == 400 && Magazine.CheckAmmo(destinationMagazine.kindOfMagazine, ((Ammo)(Item.GetItem(startItemImage.itemCode))).kindOfAmmo) && magazineContents.Length/4 < destinationMagazine.magazineSize)
                                    {
                                        
                                        int remainAmount = destinationMagazine.magazineSize - (startItemImage.amount + magazineContents.Length / 4);
                                       
                                        int maxMoveAmount;

                                        if (remainAmount >= 0)
                                            maxMoveAmount = startItemImage.amount;
                                        else
                                            maxMoveAmount = startItemImage.amount + remainAmount;

                                        GameObject AmountUI = CreateSliderUI(0, maxMoveAmount);
                                        Slider slider = AmountUI.transform.Find("Slider").GetComponent<Slider>();

                                        AmountUI.transform.Find("Divide").GetComponent<Button>().onClick.AddListener
                                            (
                                            delegate
                                            {
                                                NowStartObject = startItemImage;
                                                NowStartType = 1;
                                                NowDestinationObject = destinationItemImage;
                                                NowDestinationType = 2;

                                                if (slider.value == 0)
                                                {

                                                }
                                                else if (slider.value >= startItemImage.amount)
                                                {
                                                    startItemImage.thisContainer.RemoveItem(startItemImage.x, startItemImage.y);

                                                    string AmmoStr = startItemImage.itemCode.ToString();
                                                    AmmoStr = AmmoStr.Substring(3, 4);
                                                    for (int i = 0; i < startItemImage.amount; i++)
                                                    {
                                                        magazineContents += AmmoStr;
                                                    }
                                                    destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]] = magazineContents;

                                                    SuccessMoveItem(false);
                                                }
                                                else
                                                {
                                                    startItemImage.thisContainer.RemoveItem((int)slider.value, startItemImage.x, startItemImage.y);

                                                    string AmmoStr = startItemImage.itemCode.ToString();
                                                    AmmoStr = AmmoStr.Substring(3, 4);
                                                    for (int i = 0; i < slider.value; i++)
                                                    {
                                                        magazineContents += AmmoStr;
                                                    }
                                                    destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]] = magazineContents;

                                                    SuccessMoveItem(false);
                                                }

                                                NowStartObject = null;
                                                NowStartType = 0;
                                                NowDestinationObject = null;
                                                NowDestinationType = 0;
                                                Destroy(AmountUI);
                                                GameManager.GamePlay();

                                            }
                                            );                                

                                    }
                                    else
                                        FailMoveItem();






                                }
                                else
                                {
                                    if (destinationItemImage.itemCode / 10000 == 204) //chestrig
                                    {
                                        ChestRig chestRig = (ChestRig)Item.GetItem(destinationItemImage.itemCode);
                                        Container[] chestRogContainers = new Container[chestRig.rigCells.Length];
                                        for (int i = 0; i < chestRig.rigCells.Length; i++)
                                        {
                                            chestRogContainers[i] = new Container(destinationItemImage.PLCcode[i], destinationItemImage.thisContainer.isNeedSave, chestRig.rigCells[i].xsize, chestRig.rigCells[i].ysize);
                                        }

                                        bool successInput = false;

                                        for (int i = 0; i < chestRogContainers.Length; i++)
                                        {


                                            for (int y = 0; y < chestRogContainers[i].ySize; y++)
                                            {
                                                for (int x = 0; x < chestRogContainers[i].xSize; x++)
                                                {

                                                    if (chestRogContainers[i].CanInsertItem(startItemImage.itemCode,true,x,y))
                                                    {
                                                        GameObject AmountUI = CreateSliderUI(0, startItemImage.amount);
                                                        Slider slider = AmountUI.transform.Find("Slider").GetComponent<Slider>();
                                                        AmountUI.transform.Find("Divide").GetComponent<Button>().onClick.AddListener
                                                            (
                                                            delegate
                                                            {
                                                                NowStartObject = startItemImage;
                                                                NowStartType = 1;
                                                                NowDestinationObject = destinationItemImage;
                                                                NowDestinationType = 2;

                                                                if (slider.value == 0)
                                                                {

                                                                }
                                                                else if (slider.value >= startItemImage.amount)
                                                                {
                                                                    startItemImage.thisContainer.RemoveItem(startItemImage.x, startItemImage.y);
                                                                    chestRogContainers[i].InsertItem(startItemImage.itemCode, new int[0], startItemImage.amount, true, x, y);
                                                                    SuccessMoveItem(false);
                                                                }
                                                                else
                                                                {
                                                                    startItemImage.thisContainer.RemoveItem((int)slider.value, startItemImage.x, startItemImage.y);
                                                                    chestRogContainers[i].InsertItem(startItemImage.itemCode, new int[0], (int)slider.value, true, x, y);
                                                                    SuccessMoveItem(false);
                                                                }

                                                                NowStartObject = null;
                                                                NowStartType = 0;
                                                                NowDestinationObject = null;
                                                                NowDestinationType = 0;
                                                                Destroy(AmountUI);
                                                                GameManager.GamePlay();



                                                            }
                                                            );



                                                        successInput = true;
                                                        
                                                        break;
                                                    }
                                                    else if (chestRogContainers[i].CanInsertItem(startItemImage.itemCode, false, x, y))
                                                    {
                                                        GameObject AmountUI = CreateSliderUI(0, startItemImage.amount);
                                                        Slider slider = AmountUI.transform.Find("Slider").GetComponent<Slider>();
                                                        AmountUI.transform.Find("Divide").GetComponent<Button>().onClick.AddListener
                                                            (
                                                            delegate
                                                            {
                                                                NowStartObject = startItemImage;
                                                                NowStartType = 1;
                                                                NowDestinationObject = destinationItemImage;
                                                                NowDestinationType = 2;

                                                                if (slider.value == 0)
                                                                {

                                                                }
                                                                else if (slider.value >= startItemImage.amount)
                                                                {
                                                                    startItemImage.thisContainer.RemoveItem(startItemImage.x, startItemImage.y);
                                                                    chestRogContainers[i].InsertItem(startItemImage.itemCode, new int[0], startItemImage.amount, false, x, y);
                                                                    SuccessMoveItem(false);
                                                                }
                                                                else
                                                                {
                                                                    startItemImage.thisContainer.RemoveItem((int)slider.value, startItemImage.x, startItemImage.y);
                                                                    chestRogContainers[i].InsertItem(startItemImage.itemCode, new int[0], (int)slider.value, false, x, y);
                                                                    SuccessMoveItem(false);
                                                                }

                                                                NowStartObject = null;
                                                                NowStartType = 0;
                                                                NowDestinationObject = null;
                                                                NowDestinationType = 0;
                                                                Destroy(AmountUI);
                                                                GameManager.GamePlay();

                                                            }
                                                            );

                                                        successInput = true;
                                                       
                                                        break;
                                                    }


                                                }
                                                if (successInput)
                                                {
                                                    break;
                                                }

                                            }
                                            if (successInput)
                                            {
                                                break;
                                            }

                                        }

                                        if (!successInput)
                                            FailMoveItem();




                                    }
                                    else  //backpack
                                    {
                                        Backpack backpack = (Backpack)Item.GetItem(destinationItemImage.itemCode);
                                        Container backpackContainer = new Container(destinationItemImage.PLCcode[0], destinationItemImage.thisContainer.isNeedSave, backpack.xSpace, backpack.ySpace);
                                        bool[,] boolmap = backpackContainer.GetBoolMap(backpackContainer.GetContents());
                                        bool successInput = false;

                                        for (int y = 0; y < backpack.ySpace; y++)
                                        {
                                            for (int x = 0; x < backpack.xSpace; x++)
                                            {
                                                if (!boolmap[y, x])
                                                {
                                                    if (backpackContainer.CanInsertItem(startItemImage.itemCode,true,x,y))
                                                    {
                                                        GameObject AmountUI = CreateSliderUI(0, startItemImage.amount);
                                                        Slider slider = AmountUI.transform.Find("Slider").GetComponent<Slider>();
                                                        AmountUI.transform.Find("Divide").GetComponent<Button>().onClick.AddListener
                                                            (
                                                            delegate
                                                            {
                                                                NowStartObject = startItemImage;
                                                                NowStartType = 1;
                                                                NowDestinationObject = destinationItemImage;
                                                                NowDestinationType = 2;

                                                                if (slider.value == 0)
                                                                {

                                                                }
                                                                else if (slider.value >= startItemImage.amount)
                                                                {
                                                                    startItemImage.thisContainer.RemoveItem(startItemImage.x, startItemImage.y);
                                                                    backpackContainer.InsertItem(startItemImage.itemCode, new int[0], startItemImage.amount, true, x, y);
                                                                    SuccessMoveItem(false);
                                                                }
                                                                else
                                                                {
                                                                    startItemImage.thisContainer.RemoveItem((int)slider.value, startItemImage.x, startItemImage.y);
                                                                    backpackContainer.InsertItem(startItemImage.itemCode, new int[0], (int)slider.value, true, x, y);
                                                                    SuccessMoveItem(false);
                                                                }

                                                                NowStartObject = null;
                                                                NowStartType = 0;
                                                                NowDestinationObject = null;
                                                                NowDestinationType = 0;
                                                                Destroy(AmountUI);
                                                                GameManager.GamePlay();



                                                            }
                                                            );


                                                        successInput = true;
                                                        
                                                        break;
                                                    }
                                                    else if (backpackContainer.CanInsertItem(startItemImage.itemCode, false, x, y))
                                                    {
                                                        GameObject AmountUI = CreateSliderUI(0, startItemImage.amount);
                                                        Slider slider = AmountUI.transform.Find("Slider").GetComponent<Slider>();
                                                        AmountUI.transform.Find("Divide").GetComponent<Button>().onClick.AddListener
                                                            (
                                                            delegate
                                                            {
                                                                NowStartObject = startItemImage;
                                                                NowStartType = 1;
                                                                NowDestinationObject = destinationItemImage;
                                                                NowDestinationType = 2;

                                                                if (slider.value == 0)
                                                                {

                                                                }
                                                                else if (slider.value >= startItemImage.amount)
                                                                {
                                                                    startItemImage.thisContainer.RemoveItem(startItemImage.x, startItemImage.y);
                                                                    backpackContainer.InsertItem(startItemImage.itemCode, new int[0], startItemImage.amount, false, x, y);
                                                                    SuccessMoveItem(false);
                                                                }
                                                                else
                                                                {
                                                                    startItemImage.thisContainer.RemoveItem((int)slider.value, startItemImage.x, startItemImage.y);
                                                                    backpackContainer.InsertItem(startItemImage.itemCode, new int[0], (int)slider.value, false, x, y);
                                                                    SuccessMoveItem(false);
                                                                }

                                                                NowStartObject = null;
                                                                NowStartType = 0;
                                                                NowDestinationObject = null;
                                                                NowDestinationType = 0;
                                                                Destroy(AmountUI);
                                                                GameManager.GamePlay();



                                                            }
                                                            );





                                                        successInput = true;
                                                        
                                                        break;
                                                    }


                                                }


                                            }
                                            if (successInput)
                                            {
                                                break;
                                            }

                                        }

                                        if (!successInput)
                                            FailMoveItem();

                                    }

                                }


                            }




                        }
                            break;
                 case 3:  //start컨테이너 -> Destination부착물 셀
                        AttachmentItemCell destinationAttachmentCell = (AttachmentItemCell)NowDestinationObject;
                        AttachmentContainer attachmentContainer = destinationAttachmentCell.transform.parent.GetComponent<AttachmentCellGroup>().attachmentContainer;

                        if (startItemImage.thisContainer.MoveItemToAttachemntCell(startItemImage.x, startItemImage.y, attachmentContainer, destinationAttachmentCell.index))
                        {
                            if(destinationAttachmentCell.transform.parent.GetComponent<AttachmentCellGroup>().isGearGun && GameManager.player.isBB)
                            {
                                if (startItemImage.itemCode / 1000000 == 4)
                                    GameManager.player.AutoNonBB(false);
                                else if(startItemImage.itemCode / 10000 == 301)
                                    GameManager.player.AutoNonBB(true);
                            }


                            SuccessMoveItem(true);
                        }
                        else
                            FailMoveItem();
                       

                        break;
                 case 4: //start컨테이너 -> Destination부착물 아이템 (샷건같은 고정탄창에 총알 넣은경우만 반응)

                        if (startItemImage.itemCode / 10000 == 400)
                        {
                            ItemImage destinationItemImage = (ItemImage)NowDestinationObject;

                            if (destinationItemImage.itemCode / 10000 == 301 && ((int)((Magazine)Item.GetItem(destinationItemImage.itemCode)).kindOfMagazine == 11 || (int)((Magazine)Item.GetItem(destinationItemImage.itemCode)).kindOfMagazine == 12))
                            {
                                if (Magazine.CheckAmmo(((Magazine)Item.GetItem(destinationItemImage.itemCode)).kindOfMagazine, ((Ammo)Item.GetItem(startItemImage.itemCode)).kindOfAmmo))
                                {
                                    PlusContainerManager destinationPLCManager = destinationItemImage.attachmentContainer.GetThisPLCManager();
                                    string contents = destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]];

                                    if (contents.Length / 4 < ((Magazine)Item.GetItem(destinationItemImage.itemCode)).magazineSize)
                                    {
                                        string AmmoStr = startItemImage.itemCode.ToString();
                                        AmmoStr = AmmoStr.Substring(3, 4);
                                        contents += AmmoStr;
                                        
                                        destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]] = contents;
                                        if(startItemImage.amount == 1)
                                        startItemImage.thisContainer.RemoveItem(startItemImage.x, startItemImage.y);
                                        else
                                        startItemImage.thisContainer.RemoveItem(1,startItemImage.x, startItemImage.y);


                                        SuccessMoveItem(true);


                                    }
                                    else
                                        FailMoveItem();
                                }
                                else
                                    FailMoveItem();
                            }
                            else
                                FailMoveItem();
                        }
                        else
                            FailMoveItem();
                        


                        break;

                 case 5:  //start컨테이너 -> Destination기어 셀
                        PlayerGearCell playerGearCell = (PlayerGearCell)NowDestinationObject;

                        if (startItemImage.thisContainer.MoveItemToPlayerGear(startItemImage.x, startItemImage.y, playerGear, playerGearCell.index))
                        {
                          
                            if(playerGearCell.index < 3)
                            {
                                if ((GameManager.player.selectGun == 2 || GameManager.player.selectGun == 3) && GameManager.player.middleGun == -1 && playerGearCell.index < 2)
                                    GameManager.player.middleGun = playerGearCell.index;

                                GameManager.player.ReRenderingPlayerPart();
                            }

                            if(playerGearCell.index == 6)
                                GameManager.player.ReRenderingPlayerPart();

                            if(playerGearCell.index == 0)
                            {
                                GameManager.player.main1FireMode = Gun.GetDefaultFireMode((Gun)Item.GetItem(startItemImage.itemCode));
                            }
                            else if(playerGearCell.index == 1)
                            {
                                GameManager.player.main2FireMode = Gun.GetDefaultFireMode((Gun)Item.GetItem(startItemImage.itemCode));
                            }
                            else if(playerGearCell.index == 2)
                            {
                                GameManager.player.pistolFireMode = Gun.GetDefaultFireMode((Gun)Item.GetItem(startItemImage.itemCode));
                            }

                            SuccessMoveItem(true);

                        }
                        else
                            FailMoveItem();


                       break;


                    default:
                        FailMoveItem();
                        break; 

                }
                    



            }


        }
        else if (NowStartType == 2)
        {
            
          
            if (Input.GetMouseButtonUp(0))
            {
                if (NowStartObject == NowDestinationObject)
                {
                    FailMoveItem();
                    return;
                }

                ItemImage startItemImage = ((ItemImage)NowStartObject);


                switch (NowDestinationType)
                {
                    case 0:
                        FailMoveItem();
                        break;
                    case 1:  //start부착물 -> Destination컨테이너 셀
                        ItemCell destinationItemCell = ((ItemCell)NowDestinationObject);
                        int[] XY = GetInputXY(destinationItemCell.cellX, destinationItemCell.cellY, startItemImage.itemCode, mouseItemIsVertical);

                        if (startItemImage.attachmentContainer.MoveItemToContainerCell(startItemImage.index, mouseItemIsVertical, destinationItemCell.transform.parent.GetComponent<CellGroup>().container, XY[0], XY[1]))
                            SuccessMoveItem(true);
                        else
                            FailMoveItem();

                        break;
                    case 2:  //start부착물 -> Destination컨테이너 아이템
                        ItemImage destinationItemImage = ((ItemImage)NowDestinationObject);

                        if (!Item.CheckPLC(destinationItemImage.itemCode))
                        {
                            if (startItemImage.itemCode == destinationItemImage.itemCode && Item.GetItem(startItemImage.itemCode).overlapLimit > 1)
                            {
                                if (Item.GetItem(startItemImage.itemCode).overlapLimit > destinationItemImage.amount)
                                {
                                    startItemImage.attachmentContainer.RemoveItem(startItemImage.index);
                                    destinationItemImage.thisContainer.RemoveItem(-1, destinationItemImage.x, destinationItemImage.y);
                                    SuccessMoveItem(true);
                                }
                                else
                                    FailMoveItem();
                            }
                            else
                                FailMoveItem();

                        }
                        else
                        {
                            if (destinationItemImage.itemCode / 10000 == 301) // 탄창삽입
                            {
                                Magazine destinationMagazine = ((Magazine)Item.GetItem(destinationItemImage.itemCode));
                                if (startItemImage.itemCode / 10000 == 400 && Magazine.CheckAmmo(destinationMagazine.kindOfMagazine, ((Ammo)(Item.GetItem(startItemImage.itemCode))).kindOfAmmo))
                                {
                                    PlusContainerManager destinationPLCManager = destinationItemImage.thisContainer.GetThisPLCManager();
                                    string magazineContents = destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]];

                                    if (destinationMagazine.magazineSize > magazineContents.Length / 4)
                                    {
                                        string AmmoStr = startItemImage.itemCode.ToString();
                                        AmmoStr = AmmoStr.Substring(3, 4);
                                        magazineContents += AmmoStr;
                                        destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]] = magazineContents;
                                        startItemImage.attachmentContainer.RemoveItem(startItemImage.index);
                                        SuccessMoveItem(true);
                                    }
                                    else
                                        FailMoveItem();

                                }
                                else
                                    FailMoveItem();



                            }
                            else if(destinationItemImage.itemCode / 10000 == 204) // chestrig
                            {
                                ChestRig chestRig = (ChestRig)Item.GetItem(destinationItemImage.itemCode);
                                Container[] chestRogContainers = new Container[chestRig.rigCells.Length];
                                for (int i = 0; i < chestRig.rigCells.Length; i++)
                                {
                                    chestRogContainers[i] = new Container(destinationItemImage.PLCcode[i], destinationItemImage.thisContainer.isNeedSave, chestRig.rigCells[i].xsize, chestRig.rigCells[i].ysize);
                                }

                                bool successInput = false;

                                for (int i = 0; i < chestRogContainers.Length; i++)
                                {


                                    for (int y = 0; y < chestRogContainers[i].ySize; y++)
                                    {
                                        for (int x = 0; x < chestRogContainers[i].xSize; x++)
                                        {

                                            if (startItemImage.attachmentContainer.MoveItemToContainerCell(startItemImage.index,true,chestRogContainers[i],x,y))
                                            {
                                                successInput = true;
                                                SuccessMoveItem(true);
                                                break;
                                            }
                                            else if (startItemImage.attachmentContainer.MoveItemToContainerCell(startItemImage.index, false, chestRogContainers[i], x, y))
                                            {
                                                successInput = true;
                                                SuccessMoveItem(true);
                                                break;
                                            }


                                        }

                                        if (successInput)
                                        {
                                            break;
                                        }

                                    }

                                    if (successInput)
                                    {
                                        break;
                                    }

                                }

                                if (!successInput)
                                    FailMoveItem();





                            }
                            else if(destinationItemImage.itemCode / 10000 == 205) // backpack
                            {
                                Backpack backpack = (Backpack)Item.GetItem(destinationItemImage.itemCode);
                                Container backpackContainer = new Container(destinationItemImage.PLCcode[0], destinationItemImage.thisContainer.isNeedSave, backpack.xSpace, backpack.ySpace);
                                bool[,] boolmap = backpackContainer.GetBoolMap(backpackContainer.GetContents());
                                bool successInput = false;

                                for (int y = 0; y < backpack.ySpace; y++)
                                {
                                    for (int x = 0; x < backpack.xSpace; x++)
                                    {
                                        if (!boolmap[y, x])
                                        {
                                            if (startItemImage.attachmentContainer.MoveItemToContainerCell(startItemImage.index,true,backpackContainer,x,y))
                                            {
                                                successInput = true;
                                                SuccessMoveItem(true);
                                                break;
                                            }
                                            else if (startItemImage.attachmentContainer.MoveItemToContainerCell(startItemImage.index, false, backpackContainer, x, y))
                                            {
                                                successInput = true;
                                                SuccessMoveItem(true);
                                                break;
                                            }


                                        }


                                    }
                                    if (successInput)
                                    {
                                        break;
                                    }

                                }

                                if (!successInput)
                                    FailMoveItem();


                            }
                            else //gun
                            {
                                Gun gun = (Gun)Item.GetItem(destinationItemImage.itemCode);
                                if (gun.gunAnimation == Gun.GunAnimation.Boltaction || gun.gunAnimation == Gun.GunAnimation.PumpShotgun || gun.gunAnimation == Gun.GunAnimation.DoubleBarrel)
                                {
                                    if (startItemImage.itemCode / 10000 != 400)
                                    {
                                        AttachmentContainer GunContainer = new AttachmentContainer(destinationItemImage.PLCcode[0], destinationItemImage.thisContainer.isNeedSave, destinationItemImage.itemCode, gun.attachmentCells);

                                        bool succesInput = false;
                                        for (int i = 0; i < gun.attachmentCells.Length; i++)
                                        {
                                            if (startItemImage.attachmentContainer.MoveItemToAttachemntContainer(startItemImage.index,GunContainer,i))
                                            {
                                                succesInput = true;
                                                SuccessMoveItem(true);
                                                break;

                                            }
                                        }

                                        if (!succesInput)
                                            FailMoveItem();
                                    }
                                    else
                                    {
                                        if (Magazine.CheckAmmo(gun.attachmentCells[gun.attachmentCells.Length - 1].kindOfMagazine, ((Ammo)Item.GetItem(startItemImage.itemCode)).kindOfAmmo))
                                        {
                                            PlusContainerManager destinationPLCManager = destinationItemImage.thisContainer.GetThisPLCManager();
                                            string GunContents = destinationPLCManager.plusContainers[destinationItemImage.PLCcode[0]];
                                            string magazineCode = "";
                                            string magazinePLC = "";

                                            int i = 0;
                                            while (GunContents[i] != 'p')
                                            {
                                                i++;
                                            }
                                            i -= 7;

                                            for (int j = 0; j < 7; j++)
                                            {
                                                magazineCode += GunContents[i];
                                                i++;
                                            }


                                            i++;
                                            while (i < GunContents.Length)
                                            {
                                                magazinePLC += GunContents[i];
                                                i++;
                                            }
                                            string magazineContents = destinationPLCManager.plusContainers[int.Parse(magazinePLC)];
                                            int magazineSize = ((Magazine)Item.GetItem(int.Parse(magazineCode))).magazineSize;

                                            if (magazineContents.Length / 4 < magazineSize)
                                            {
                                                string AmmoStr = startItemImage.itemCode.ToString();
                                                AmmoStr = AmmoStr.Substring(3, 4);
                                                magazineContents += AmmoStr;
                                                destinationPLCManager.plusContainers[int.Parse(magazinePLC)] = magazineContents;

                                                startItemImage.attachmentContainer.RemoveItem(startItemImage.index);
                                                SuccessMoveItem(true);

                                            }
                                            else
                                            {
                                                //약실넣기
                                                AttachmentContainer GunContainer = new AttachmentContainer(destinationItemImage.PLCcode[0], destinationItemImage.thisContainer.isNeedSave, destinationItemImage.itemCode, gun.attachmentCells);
                                                if (startItemImage.attachmentContainer.MoveItemToAttachemntContainer(startItemImage.index,GunContainer,0))
                                                {
                                                    SuccessMoveItem(true);
                                                }
                                                else
                                                {
                                                    FailMoveItem();
                                                }
                                            }
                                        }
                                        else
                                            FailMoveItem();
                                    }



                                }
                                else // nomal Attachment
                                {
                                    AttachmentContainer GunContainer = new AttachmentContainer(destinationItemImage.PLCcode[0], destinationItemImage.thisContainer.isNeedSave, destinationItemImage.itemCode, gun.attachmentCells);

                                    bool succesInput = false;
                                    for (int i = 0; i < gun.attachmentCells.Length; i++)
                                    {
                                        if (startItemImage.attachmentContainer.MoveItemToAttachemntContainer(startItemImage.index,GunContainer,i))
                                        {
                                            succesInput = true;
                                            SuccessMoveItem(true);
                                            break;

                                        }

                                    }

                                    if (!succesInput)
                                        FailMoveItem();



                                }



                            }

                        }



                         break;
                    case 3: //start부착물 -> Destination부착물 셀
                        AttachmentItemCell destinationAttachmentCell = (AttachmentItemCell)NowDestinationObject;
                        AttachmentContainer attachmentContainer = destinationAttachmentCell.transform.parent.GetComponent<AttachmentCellGroup>().attachmentContainer;

                        if (startItemImage.attachmentContainer.MoveItemToAttachemntContainer(startItemImage.index, attachmentContainer, destinationAttachmentCell.index))
                        {
                            if (destinationAttachmentCell.transform.parent.GetComponent<AttachmentCellGroup>().isGearGun && GameManager.player.isBB)
                            {
                                if (startItemImage.itemCode / 1000000 == 4)
                                    GameManager.player.AutoNonBB(false);
                                else if (startItemImage.itemCode / 10000 == 301)
                                    GameManager.player.AutoNonBB(true);
                            }


                            SuccessMoveItem(true);
                        }
                        else
                            FailMoveItem();


                        break;
                    case 4:  //start부착물 -> Destination부착물 아이템

                        if (startItemImage.itemCode / 10000 == 400)
                        {
                            ItemImage destinationItemImage2 = (ItemImage)NowDestinationObject;

                            if (destinationItemImage2.itemCode / 10000 == 301 && ((int)((Magazine)Item.GetItem(destinationItemImage2.itemCode)).kindOfMagazine == 11 || (int)((Magazine)Item.GetItem(destinationItemImage2.itemCode)).kindOfMagazine == 12))
                            {
                                if (Magazine.CheckAmmo(((Magazine)Item.GetItem(destinationItemImage2.itemCode)).kindOfMagazine, ((Ammo)Item.GetItem(startItemImage.itemCode)).kindOfAmmo))
                                {
                                    PlusContainerManager destinationPLCManager = destinationItemImage2.attachmentContainer.GetThisPLCManager();
                                    string contents = destinationPLCManager.plusContainers[destinationItemImage2.PLCcode[0]];

                                    if (contents.Length / 4 < ((Magazine)Item.GetItem(destinationItemImage2.itemCode)).magazineSize)
                                    {
                                        string AmmoStr = startItemImage.itemCode.ToString();
                                        AmmoStr = AmmoStr.Substring(3, 4);
                                        contents += AmmoStr;

                                        destinationPLCManager.plusContainers[destinationItemImage2.PLCcode[0]] = contents;

                                        startItemImage.attachmentContainer.RemoveItem(startItemImage.index);

                                        SuccessMoveItem(true);


                                    }
                                    else
                                        FailMoveItem();
                                }
                                else
                                    FailMoveItem();
                            }
                            else
                                FailMoveItem();
                        }
                        else
                            FailMoveItem();

                        break;
                    case 5: //start부착물 -> Destination기어 셀
                        FailMoveItem();
                        break;

                    default:
                        FailMoveItem();
                        break;

                }



            }


        }
        else if (NowStartType == 3)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (NowStartObject == NowDestinationObject)
                {
                    FailMoveItem();
                    return;
                }
                ItemImage startItemImage = ((ItemImage)NowStartObject);

                switch (NowDestinationType)
                {
                    case 0:
                        FailMoveItem();
                        break;
                    case 1: //start기어 -> Destination컨테이너 셀
                        ItemCell destinationItemCell = ((ItemCell)NowDestinationObject);
                        int[] XY = GetInputXY(destinationItemCell.cellX, destinationItemCell.cellY, startItemImage.itemCode, mouseItemIsVertical);

                        if (playerGear.MoveItemToContainerCell(startItemImage.index, mouseItemIsVertical, destinationItemCell.transform.parent.GetComponent<CellGroup>().container, XY[0], XY[1]))
                        {
                            if (startItemImage.index < 3 && GameManager.player.selectGun == startItemImage.index)
                                GameManager.player.ForceChangeToKnife();

                            if((GameManager.player.selectGun == 2 || GameManager.player.selectGun == 3) && startItemImage.index < 2)
                            {
                                if(GameManager.player.middleGun == startItemImage.index)
                                {
                                    if (playerGear.GearItems[1 - startItemImage.index] != 0)
                                    {
                                        GameManager.player.middleGun = 1 - startItemImage.index;
                                    }
                                    else
                                        GameManager.player.middleGun = -1;
                                }


                            }

                            if (startItemImage.index < 3 || startItemImage.index == 6)
                                GameManager.player.ReRenderingPlayerPart();

                            if (startItemImage.index == 0)
                                GameManager.player.main1FireMode = 0;
                            else if(startItemImage.index == 1)
                                GameManager.player.main2FireMode = 0;
                            else if(startItemImage.index == 2)
                                GameManager.player.pistolFireMode = 0;

                            if (destinationItemCell.transform.parent.GetComponent<CellGroup>().isGearChestRig || destinationItemCell.transform.parent.GetComponent<CellGroup>().isGearBackpack)
                            {
                                GameManager.cameraManager.PlayUISound(1);
                                playerGear.PlayerGearReRendering();
                                FinishMove();
                            }
                            else
                                SuccessMoveItem(true);

                        }
                        else
                            FailMoveItem();

                        break;

                    case 2:  //start기어 -> Destination컨테이너 아이템
                        ItemImage destinationItemImage = ((ItemImage)NowDestinationObject);

                        if (destinationItemImage.itemCode / 10000 == 204) // chestrig
                        {
                            ChestRig chestRig = (ChestRig)Item.GetItem(destinationItemImage.itemCode);
                            Container[] chestRogContainers = new Container[chestRig.rigCells.Length];
                            for (int i = 0; i < chestRig.rigCells.Length; i++)
                            {
                                chestRogContainers[i] = new Container(destinationItemImage.PLCcode[i], destinationItemImage.thisContainer.isNeedSave, chestRig.rigCells[i].xsize, chestRig.rigCells[i].ysize);
                            }

                            bool successInput = false;

                            for (int i = 0; i < chestRogContainers.Length; i++)
                            {


                                for (int y = 0; y < chestRogContainers[i].ySize; y++)
                                {
                                    for (int x = 0; x < chestRogContainers[i].xSize; x++)
                                    {

                                        if (playerGear.MoveItemToContainerCell(startItemImage.index, true, chestRogContainers[i], x, y))
                                        {
                                            if (startItemImage.index < 3 && GameManager.player.selectGun == startItemImage.index)
                                                GameManager.player.ForceChangeToKnife();

                                            if ((GameManager.player.selectGun == 2 || GameManager.player.selectGun == 3) && startItemImage.index < 2)
                                            {
                                                if (GameManager.player.middleGun == startItemImage.index)
                                                {
                                                    if (playerGear.GearItems[1 - startItemImage.index] != 0)
                                                    {
                                                        GameManager.player.middleGun = 1 - startItemImage.index;
                                                    }
                                                    else
                                                        GameManager.player.middleGun = -1;
                                                }


                                            }

                                            if (startItemImage.index < 3 || startItemImage.index == 6)
                                                GameManager.player.ReRenderingPlayerPart();

                                            if (startItemImage.index == 0)
                                                GameManager.player.main1FireMode = 0;
                                            else if (startItemImage.index == 1)
                                                GameManager.player.main2FireMode = 0;
                                            else if (startItemImage.index == 2)
                                                GameManager.player.pistolFireMode = 0;

                                            successInput = true;

                                            if (destinationItemImage.transform.parent.GetComponent<CellGroup>().isGearBackpack || destinationItemImage.transform.parent.GetComponent<CellGroup>().isGearChestRig)
                                            {
                                                GameManager.cameraManager.PlayUISound(1);
                                                playerGear.PlayerGearReRendering();
                                                FinishMove();
                                            }
                                            else
                                                SuccessMoveItem(true);

                                            break;
                                        }
                                        else if (playerGear.MoveItemToContainerCell(startItemImage.index, false, chestRogContainers[i], x, y))
                                        {
                                            if (startItemImage.index < 3 && GameManager.player.selectGun == startItemImage.index)
                                                GameManager.player.ForceChangeToKnife();

                                            if ((GameManager.player.selectGun == 2 || GameManager.player.selectGun == 3) && startItemImage.index < 2)
                                            {
                                                if (GameManager.player.middleGun == startItemImage.index)
                                                {
                                                    if (playerGear.GearItems[1 - startItemImage.index] != 0)
                                                    {
                                                        GameManager.player.middleGun = 1 - startItemImage.index;
                                                    }
                                                    else
                                                        GameManager.player.middleGun = -1;
                                                }


                                            }

                                            if (startItemImage.index < 3 || startItemImage.index == 6)
                                                GameManager.player.ReRenderingPlayerPart();

                                            if (startItemImage.index == 0)
                                                GameManager.player.main1FireMode = 0;
                                            else if (startItemImage.index == 1)
                                                GameManager.player.main2FireMode = 0;
                                            else if (startItemImage.index == 2)
                                                GameManager.player.pistolFireMode = 0;

                                            successInput = true;

                                            if (destinationItemImage.transform.parent.GetComponent<CellGroup>().isGearBackpack || destinationItemImage.transform.parent.GetComponent<CellGroup>().isGearChestRig)
                                            {
                                                GameManager.cameraManager.PlayUISound(1);
                                                playerGear.PlayerGearReRendering();
                                                FinishMove();
                                            }
                                            else
                                                SuccessMoveItem(true);

                                            break;
                                        }


                                    }

                                    if (successInput)
                                    {
                                        break;
                                    }

                                }

                                if (successInput)
                                {
                                    break;
                                }

                            }

                            if (!successInput)
                                FailMoveItem();






                        }
                        else if (destinationItemImage.itemCode / 10000 == 205)  //backpack
                        {
                            Backpack backpack = (Backpack)Item.GetItem(destinationItemImage.itemCode);
                            Container backpackContainer = new Container(destinationItemImage.PLCcode[0], destinationItemImage.thisContainer.isNeedSave, backpack.xSpace, backpack.ySpace);
                            bool[,] boolmap = backpackContainer.GetBoolMap(backpackContainer.GetContents());
                            bool successInput = false;

                            for (int y = 0; y < backpack.ySpace; y++)
                            {
                                for (int x = 0; x < backpack.xSpace; x++)
                                {
                                    if (!boolmap[y, x])
                                    {
                                        if (playerGear.MoveItemToContainerCell(startItemImage.index, true, backpackContainer, x, y))
                                        {
                                            if (startItemImage.index < 3 && GameManager.player.selectGun == startItemImage.index)
                                                GameManager.player.ForceChangeToKnife();

                                            if ((GameManager.player.selectGun == 2 || GameManager.player.selectGun == 3) && startItemImage.index < 2)
                                            {
                                                if (GameManager.player.middleGun == startItemImage.index)
                                                {
                                                    if (playerGear.GearItems[1 - startItemImage.index] != 0)
                                                    {
                                                        GameManager.player.middleGun = 1 - startItemImage.index;
                                                    }
                                                    else
                                                        GameManager.player.middleGun = -1;
                                                }


                                            }

                                            if (startItemImage.index < 3 || startItemImage.index == 6)
                                                GameManager.player.ReRenderingPlayerPart();

                                            if (startItemImage.index == 0)
                                                GameManager.player.main1FireMode = 0;
                                            else if (startItemImage.index == 1)
                                                GameManager.player.main2FireMode = 0;
                                            else if (startItemImage.index == 2)
                                                GameManager.player.pistolFireMode = 0;

                                            successInput = true;

                                            if (destinationItemImage.transform.parent.GetComponent<CellGroup>().isGearBackpack || destinationItemImage.transform.parent.GetComponent<CellGroup>().isGearChestRig)
                                            {
                                                GameManager.cameraManager.PlayUISound(1);
                                                playerGear.PlayerGearReRendering();
                                                FinishMove();
                                            }
                                            else
                                                SuccessMoveItem(true);

                                            break;
                                        }
                                        else if (playerGear.MoveItemToContainerCell(startItemImage.index, false, backpackContainer, x, y))
                                        {
                                            if (startItemImage.index < 3 && GameManager.player.selectGun == startItemImage.index)
                                                GameManager.player.ForceChangeToKnife();

                                            if ((GameManager.player.selectGun == 2 || GameManager.player.selectGun == 3) && startItemImage.index < 2)
                                            {
                                                if (GameManager.player.middleGun == startItemImage.index)
                                                {
                                                    if (playerGear.GearItems[1 - startItemImage.index] != 0)
                                                    {
                                                        GameManager.player.middleGun = 1 - startItemImage.index;
                                                    }
                                                    else
                                                        GameManager.player.middleGun = -1;
                                                }


                                            }

                                            if (startItemImage.index < 3 || startItemImage.index == 6)
                                                GameManager.player.ReRenderingPlayerPart();

                                            if (startItemImage.index == 0)
                                                GameManager.player.main1FireMode = 0;
                                            else if (startItemImage.index == 1)
                                                GameManager.player.main2FireMode = 0;
                                            else if (startItemImage.index == 2)
                                                GameManager.player.pistolFireMode = 0;

                                            successInput = true;

                                            if (destinationItemImage.transform.parent.GetComponent<CellGroup>().isGearBackpack || destinationItemImage.transform.parent.GetComponent<CellGroup>().isGearChestRig)
                                            {
                                                GameManager.cameraManager.PlayUISound(1);
                                                playerGear.PlayerGearReRendering();
                                                FinishMove();
                                            }
                                            else
                                                SuccessMoveItem(true);

                                            break;
                                        }


                                    }


                                }
                                if (successInput)
                                {
                                    break;
                                }

                            }

                            if (!successInput)
                                FailMoveItem();



                        }
                        else
                            FailMoveItem();

                        break;

                    case 3: //start기어 -> Destination부착물 셀
                        FailMoveItem();
                        break;

                    case 4: //start기어 -> Destination부착물 아이템
                        FailMoveItem();
                        break;
                    case 5: //기어 -> 기어 (메인총) -> (메인총)
                        PlayerGearCell destinationPlayerGearCell = (PlayerGearCell)NowDestinationObject;

                        if (startItemImage.index == 0 && destinationPlayerGearCell.index == 1)
                        {
                            if (playerGear.insertItem(playerGear.GearItems[0], new int[1] { playerGear.GunsPLC[0] }, 1))
                            {
                                playerGear.RemoveOnlyTextCode(0);

                                if (GameManager.player.selectGun == startItemImage.index)
                                    GameManager.player.ForceChangeToKnife();
                                else if (GameManager.player.selectGun == 2 || GameManager.player.selectGun == 3)
                                    GameManager.player.middleGun = destinationPlayerGearCell.index;

                                GameManager.player.ReRenderingPlayerPart();

                                GameManager.player.main1FireMode = 0;

                                GameManager.player.main2FireMode = Gun.GetDefaultFireMode((Gun)Item.GetItem(startItemImage.itemCode));

                                GameManager.cameraManager.PlayUISound(2);
                                playerGear.PlayerGearReRendering();
                                FinishMove();
                            }
                            else
                                FailMoveItem();


                        }
                        else if (startItemImage.index == 1 && destinationPlayerGearCell.index == 0)
                        {
                            if (playerGear.insertItem(playerGear.GearItems[1], new int[1] { playerGear.GunsPLC[1] }, 0))
                            {
                                playerGear.RemoveOnlyTextCode(1);

                                if (GameManager.player.selectGun == startItemImage.index)
                                    GameManager.player.ForceChangeToKnife();
                                else if (GameManager.player.selectGun == 2 || GameManager.player.selectGun == 3)
                                    GameManager.player.middleGun = destinationPlayerGearCell.index;

                                GameManager.player.ReRenderingPlayerPart();

                                GameManager.player.main2FireMode = 0;

                                GameManager.player.main1FireMode = Gun.GetDefaultFireMode((Gun)Item.GetItem(startItemImage.itemCode));

                                GameManager.cameraManager.PlayUISound(2);
                                playerGear.PlayerGearReRendering();
                                FinishMove();
                            }
                            else
                                FailMoveItem();

                        }
                        else
                            FailMoveItem();
                        
                        break;

                    default:
                        FailMoveItem();
                        break;

                }



            }


        }

    }

    private void IfExistStartupdate()
    {


        if (mouseItem == null)
        {
            mouseItem = GameObject.Instantiate(mouseItemPrefabs);
            mouseItem.transform.SetParent(canvas.transform);
            Sprite itemSprite = ((MonoBehaviour)NowStartObject).gameObject.GetComponent<Image>().sprite;
            mouseItem.GetComponent<Image>().sprite = itemSprite;
            mouseItem.GetComponent<RectTransform>().sizeDelta = new Vector2(itemSprite.bounds.size.x, itemSprite.bounds.size.y);
            if (((ItemImage)NowStartObject).isVertical)
            {
                mouseItemIsVertical = true;
            }
            else
            {
                mouseItem.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                mouseItemIsVertical = false;
            }

            Vector2 mousePos;
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseItem.transform.position = new Vector3(mousePos.x, mousePos.y, -100);
        }
        else
        {
            Vector2 mousePos;
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseItem.transform.position = new Vector3(mousePos.x, mousePos.y, -100);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            mouseItemIsVertical = !mouseItemIsVertical;


            if (mouseItemIsVertical)          
                mouseItem.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));         
            else
                mouseItem.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));


        }



    }

    private void SuccessMoveItem(bool needFinish)
    {
        if (((MonoBehaviour)NowStartObject).transform.parent == ((MonoBehaviour)NowDestinationObject).transform.parent)
        {
            switch (NowStartType)
            {
                case 1:
                    ContainerReRender(NowStartObject);
                    break;
                case 2:
                    AttachmentContainerReRender(NowStartObject);
                    break;
                case 3:
                    playerGear.PlayerGearReRendering();
                    break;

            }
        }
        else
        {
            switch (NowStartType)
            {

                case 1:
                    ContainerReRender(NowStartObject);
                    break;
                case 2:
                    AttachmentContainerReRender(NowStartObject);
                    break;
                case 3:
                    playerGear.PlayerGearReRendering();
                    break ;

            }


            switch (NowDestinationType)
            {
                case 1:
                    ContainerReRender(NowDestinationObject);
                    break;
                case 2:
                    ContainerReRender(NowDestinationObject);
                    break;
                case 3:
                    AttachmentContainerReRender(NowDestinationObject);
                    break;
                case 4:
                    AttachmentContainerReRender(NowDestinationObject);
                    break;
                case 5:
                    playerGear.PlayerGearReRendering();
                    break;
            }
        }

        if(NowDestinationType == 1)
        {
            GameManager.cameraManager.PlayUISound(1);
        }
        else if(NowDestinationType == 2)
        {
            int middle = ((ItemImage)NowDestinationObject).itemCode / 10000;

            if(middle == 201 || middle == 301)
            {
                GameManager.cameraManager.PlayUISound(2);
            }
            else
            {
                GameManager.cameraManager.PlayUISound(1);
            }

        }
        else if (NowDestinationType == 3)
        {
            GameManager.cameraManager.PlayUISound(2);
        }
        else if (NowDestinationType == 4)
        {
            GameManager.cameraManager.PlayUISound(2);
        }
        else if(NowDestinationType == 5)
        {
            if(((PlayerGearCell)NowDestinationObject).index < 3)
            {
                GameManager.cameraManager.PlayUISound(2);
            }
            else
                GameManager.cameraManager.PlayUISound(1);


        }



        if (needFinish)
        FinishMove();


    }


    public void ContainerReRender(object containerObject)
    {
        Container startContainer;
        Transform startParent;
        Transform cellParent;
        Vector2 startPos;

        if (containerObject.GetType() == typeof(ItemImage))
        {
            ItemImage itemimage;
            itemimage = ((ItemImage)containerObject);
            startContainer = itemimage.thisContainer;
            cellParent = itemimage.transform.parent;
        }
        else
        {
            ItemCell itemCell;
            itemCell = ((ItemCell)containerObject);
            cellParent = itemCell.transform.parent;
            startContainer = cellParent.GetComponent<CellGroup>().container;       
        }
        
        startPos = new Vector2(cellParent.GetComponent<CellGroup>().xLocalPos, cellParent.GetComponent<CellGroup>().yLocalPos);
        startParent = cellParent.transform.parent;
        CellGroup[] childs = cellParent.GetComponentsInChildren<CellGroup>();
        foreach(CellGroup child in childs)
        {
          if(child.transform.parent == cellParent.transform)
            child.transform.SetParent(startParent.transform);
        }
        GameObject newCellGroup = CellGroup.RenderContainer(startParent.gameObject, startContainer, startPos, cellParent.GetComponent<CellGroup>().Exit, cellPrefabs, amountFont);
        foreach (CellGroup child in childs)
        {
            if (child.transform.parent == startParent.transform)
                child.transform.SetParent(newCellGroup.transform);
        }

        if(cellParent.GetComponent<CellGroup>().isGearBackpack)
        {
            newCellGroup.GetComponent<CellGroup>().isGearBackpack = true;
            playerGear.BackpackCellGroup = newCellGroup;
        }

        if (cellParent.GetComponent<CellGroup>().isGearChestRig)
            newCellGroup.GetComponent<CellGroup>().isGearChestRig = true;

        if (cellParent.GetComponent<CellGroup>().chest != null)
        {
            cellParent.GetComponent<CellGroup>().chest.cellGroupObject = newCellGroup;
            newCellGroup.GetComponent<CellGroup>().chest = cellParent.GetComponent<CellGroup>().chest;
        }

        if(cellParent.GetComponent<CellGroup>().shop != null)
        {
            cellParent.GetComponent<CellGroup>().shop.chestCellGroup = newCellGroup;
            newCellGroup.GetComponent<CellGroup>().shop = cellParent.GetComponent<CellGroup>().shop;
        }

        if(cellParent.GetComponent<CellGroup>().spriteObject != null)
        {
            cellParent.GetComponent<CellGroup>().spriteObject.transform.SetParent(newCellGroup.transform);
            cellParent.GetComponent<CellGroup>().spriteObject.transform.SetAsFirstSibling();
            newCellGroup.GetComponent<CellGroup>().spriteObject = cellParent.GetComponent<CellGroup>().spriteObject;
        }


        GameObject.Destroy(cellParent.gameObject);    
    }

    public void ContainerReRender(CellGroup cellgroup)
    {
        Container startContainer;
        Transform startParent;
        Transform cellParent;
        Vector2 startPos;


        startContainer = cellgroup.container;
        cellParent = cellgroup.transform;
 

        startPos = new Vector2(cellParent.GetComponent<CellGroup>().xLocalPos, cellParent.GetComponent<CellGroup>().yLocalPos);
        startParent = cellParent.transform.parent;
        CellGroup[] childs = cellParent.GetComponentsInChildren<CellGroup>();
        foreach (CellGroup child in childs)
        {
            if (child.transform.parent == cellParent.transform)
                child.transform.SetParent(startParent.transform);
        }
        GameObject newCellGroup = CellGroup.RenderContainer(startParent.gameObject, startContainer, startPos, cellParent.GetComponent<CellGroup>().Exit, cellPrefabs, amountFont);
        foreach (CellGroup child in childs)
        {
            if (child.transform.parent == startParent.transform)
                child.transform.SetParent(newCellGroup.transform);
        }

        if (cellParent.GetComponent<CellGroup>().isGearBackpack)
        {
            newCellGroup.GetComponent<CellGroup>().isGearBackpack = true;
            playerGear.BackpackCellGroup = newCellGroup;
        }

        if (cellParent.GetComponent<CellGroup>().isGearChestRig)
            newCellGroup.GetComponent<CellGroup>().isGearChestRig = true;

        if (cellParent.GetComponent<CellGroup>().chest != null)
        {
            cellParent.GetComponent<CellGroup>().chest.cellGroupObject = newCellGroup;
            newCellGroup.GetComponent<CellGroup>().chest = cellParent.GetComponent<CellGroup>().chest;
        }

        if (cellParent.GetComponent<CellGroup>().shop != null)
        {
            cellParent.GetComponent<CellGroup>().shop.chestCellGroup = newCellGroup;
            newCellGroup.GetComponent<CellGroup>().shop = cellParent.GetComponent<CellGroup>().shop;
        }

        if (cellParent.GetComponent<CellGroup>().spriteObject != null)
        {
            cellParent.GetComponent<CellGroup>().spriteObject.transform.SetParent(newCellGroup.transform);
            cellParent.GetComponent<CellGroup>().spriteObject.transform.SetAsFirstSibling();
            newCellGroup.GetComponent<CellGroup>().spriteObject = cellParent.GetComponent<CellGroup>().spriteObject;
        }

        GameObject.Destroy(cellParent.gameObject);
    }

    public void AttachmentContainerReRender(object containerObject)
    {
        AttachmentContainer startContainer;
        Transform startParent;
        Transform cellParent;
        Vector2 startPos;

        if (containerObject.GetType() == typeof(ItemImage))
        {
            ItemImage itemimage;
            itemimage = ((ItemImage)containerObject);
            startContainer = itemimage.attachmentContainer;
            cellParent = itemimage.transform.parent;
        }
        else
        {
            AttachmentItemCell itemCell;
            itemCell = ((AttachmentItemCell)containerObject);
            cellParent = itemCell.transform.parent;
            startContainer = cellParent.GetComponent<AttachmentCellGroup>().attachmentContainer;
        }

        startPos = new Vector2(cellParent.GetComponent<AttachmentCellGroup>().xLocalPos, cellParent.GetComponent<AttachmentCellGroup>().yLocalPos);
        startParent = cellParent.transform.parent;
        bool isGearGun = cellParent.GetComponent<AttachmentCellGroup>().isGearGun;
        GameObject cellgroup = AttachmentCellGroup.RenderingCellGroup(startParent.gameObject, startContainer, startPos,isGearGun, cellPrefabs);
        if (!isGearGun)
        {
            cellgroup.AddComponent<CellGroup>();

            GameObject Exit = Instantiate(cellPrefabs[3]);
            Exit.transform.SetParent(cellgroup.transform);
            Exit.transform.localPosition = new Vector3(Item.GetItem(startContainer.itemCode).itemSprite.bounds.size.x / 2 - 5, Item.GetItem(startContainer.itemCode).itemSprite.bounds.size.y / 2 + 6);

            Exit.GetComponent<Button>().onClick.AddListener
                (
                delegate
                {
                    Destroy(cellgroup);
                }
                );


        }
        else
        {
            playerGear.nowAttachmentCellGroup = cellgroup.GetComponent<AttachmentCellGroup>();
            cellgroup.GetComponent<Image>().material = NoLineItemMaterial;
        }

        if(cellParent.GetComponent<AttachmentCellGroup>().spriteObject != null)
        {
            cellParent.GetComponent<AttachmentCellGroup>().spriteObject.transform.SetParent(cellgroup.transform);
            cellParent.GetComponent<AttachmentCellGroup>().spriteObject.transform.SetAsFirstSibling();
            cellgroup.GetComponent<AttachmentCellGroup>().spriteObject = cellParent.GetComponent<AttachmentCellGroup>().spriteObject;
        }


        GameObject.Destroy(cellParent.gameObject);
    }

    public void AttachmentContainerReRender(AttachmentCellGroup attachcellgroup)
    {
        AttachmentContainer startContainer = attachcellgroup.attachmentContainer;
        Transform startParent;
        Transform cellParent = attachcellgroup.transform;
        Vector2 startPos;

        startPos = new Vector2(cellParent.GetComponent<AttachmentCellGroup>().xLocalPos, cellParent.GetComponent<AttachmentCellGroup>().yLocalPos);
        startParent = cellParent.transform.parent;
        bool isGearGun = cellParent.GetComponent<AttachmentCellGroup>().isGearGun;
        
        GameObject cellgroup = AttachmentCellGroup.RenderingCellGroup(startParent.gameObject, startContainer, startPos, isGearGun, cellPrefabs);
        if (!isGearGun)
        {
            cellgroup.AddComponent<CellGroup>();

            GameObject Exit = Instantiate(cellPrefabs[3]);
            Exit.transform.SetParent(cellgroup.transform);
            Exit.transform.localPosition = new Vector3(Item.GetItem(startContainer.itemCode).itemSprite.bounds.size.x / 2 - 5, Item.GetItem(startContainer.itemCode).itemSprite.bounds.size.y / 2 + 6);

            Exit.GetComponent<Button>().onClick.AddListener
                (
                delegate
                {
                    Destroy(cellgroup);
                }
                );


        }
        else
        {
            playerGear.nowAttachmentCellGroup = cellgroup.GetComponent<AttachmentCellGroup>();
            cellgroup.GetComponent<Image>().material = NoLineItemMaterial;
        }

        if (cellParent.GetComponent<AttachmentCellGroup>().spriteObject != null)
        {
            cellParent.GetComponent<AttachmentCellGroup>().spriteObject.transform.SetParent(cellgroup.transform);
            cellParent.GetComponent<AttachmentCellGroup>().spriteObject.transform.SetAsFirstSibling();
            cellgroup.GetComponent<AttachmentCellGroup>().spriteObject = cellParent.GetComponent<AttachmentCellGroup>().spriteObject;
        }

        GameObject.Destroy(cellParent.gameObject);

    }


    public void FailMoveItem()
    {
        Color color = ((MonoBehaviour)NowStartObject).GetComponent<Image>().color;
        color = new Color(1, 1, 1, 1);
        if(NowStartType != 3)
        ((MonoBehaviour)NowStartObject).GetComponent<Image>().color = color;

        ((MonoBehaviour)NowStartObject).GetComponent<Image>().raycastTarget = true;

        FinishMove();
    }

    private void FinishMove()
    {
        NowStartType = 0;
        NowStartObject = null;
        if(mouseItem != null)
        GameObject.Destroy(mouseItem);
        mouseItem = null;
    }

    private int[] GetInputXY(int x,int y,int itemcode , bool isVertical)
    {
        int[] result = new int[2];
        Item item = Item.GetItem(itemcode);
        int xSize;
        int ySize;
        if(isVertical)
        {
            xSize = item.xsize;
            ySize = item.ysize;
        }
        else
        {
            xSize = item.ysize;
            ySize = item.xsize;
        }


        if(xSize%2 == 1)      
            result[0] = x - (xSize / 2);
        else
        {
            Vector2 mousePos;
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (((MonoBehaviour)NowDestinationObject).gameObject.GetComponent<RectTransform>().position.x > mousePos.x)
              result[0] = x - (xSize / 2);
            else
              result[0] = x - ((xSize / 2) -1);
        }

        if (ySize % 2 == 1)
            result[1] = y - (ySize / 2);
        else
        {
            Vector2 mousePos;
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (((MonoBehaviour)NowDestinationObject).gameObject.GetComponent<RectTransform>().position.y <= mousePos.y)
                result[1] = y - (ySize / 2);
            else
                result[1] = y - ((ySize / 2) - 1);
        }



        return result;
        

    }
    public GameObject CreateSliderUI(int minValue,int maxValue)
    {
        GameManager.GamePause();

        FailMoveItem();

        GameObject AmountUI = GameObject.Instantiate(moveAmountBox);
        AmountUI.transform.SetParent(canvas.transform);
        AmountUI.transform.localPosition = new Vector3(0, 0, -10);

        Slider slider = AmountUI.transform.Find("Slider").GetComponent<Slider>();
        Text text = AmountUI.transform.Find("Text").GetComponent<Text>();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.onValueChanged.AddListener((f) => { text.text = f.ToString(); });

        return AmountUI;
    }
}


