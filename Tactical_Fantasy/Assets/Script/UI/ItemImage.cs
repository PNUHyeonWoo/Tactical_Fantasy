using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemImage : MonoBehaviour ,IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int cellType;
    public int index;
    public int x;
    public int y;
    public int itemCode;
    public Container thisContainer;
    public AttachmentContainer attachmentContainer;
    public bool isVertical;
    public int amount;
    public int[] PLCcode;
    public bool cantSelectThisItem = false;
    public ItemManager itemManager;

    public const float selectAlpha = (float)200/(float)255;


    void Start()
    {
        itemManager = GameManager.itemManager;
    }


     void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (!GameManager.player.CanGrabItem())
            return;

        if (eventData.button == 0 && GameManager.PauseAmount == 0 && !cantSelectThisItem)
        {
            itemManager.NowStartType = cellType;
            itemManager.NowStartObject = this;

            Color originalColor = gameObject.GetComponent<Image>().color;
            originalColor = new Color(selectAlpha, selectAlpha, selectAlpha, 1);
            if(cellType != 3)
            gameObject.GetComponent<Image>().color = originalColor;

            gameObject.GetComponent<Image>().raycastTarget = false;

            itemManager.NowDestinationType = (int)ItemManager.TypeOfDestination.None;
            itemManager.NowDestinationObject = null;

        }

        if((int)eventData.button == 1 && GameManager.PauseAmount == 0)
        {

            if (cellType == 1)
            {
                if(itemCode / 10000 == 205 || itemCode / 10000 == 204 || itemCode / 10000 == 201)
                    RenderingItemMenu(new string[3] { "Inspect", "Open" ,"Discard" });
                else if(itemCode / 10000 == 301)
                    RenderingItemMenu(new string[3] { "Inspect", "Unload", "Discard" });
                else if(itemCode / 1000000 == 5)
                    RenderingItemMenu(new string[3] { "Inspect", "Use", "Discard" });
                else
                    RenderingItemMenu(new string[2] { "Inspect", "Discard" });


            }
            else if(cellType == 2)
            {
                if(itemCode/10000 == 301 && ( (int)((Magazine)Item.GetItem(itemCode)).kindOfMagazine == 11 || (int)((Magazine)Item.GetItem(itemCode)).kindOfMagazine == 12 ) )
                    RenderingItemMenu(new string[2] {"Inspect", "Unload"});
                else
                    RenderingItemMenu(new string[2] { "Inspect", "Discard"});
            }
            else
            {
                RenderingItemMenu(new string[2] { "Inspect", "Discard" });
            }

        }
        

    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (cellType == 3)
            return;

        itemManager.NowDestinationType = cellType * 2;
        itemManager.NowDestinationObject = this;
       
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (cellType == 3)
            return;

        itemManager.NowDestinationType = (int)ItemManager.TypeOfDestination.None;
        itemManager.NowDestinationObject = null;
    }


    


    void RenderingItemMenu(string[] buttons)
    {
        GameManager.cameraManager.PlayUISound(6);

        GameObject itemmenu = GameObject.Instantiate(itemManager.itemMenuPrefabs[0]);
        GameObject.Destroy(itemmenu.GetComponent<Image>());
        itemmenu.transform.SetParent(itemManager.canvas);
        Destroy(itemmenu.transform.Find("Button").gameObject);
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(itemManager.canvas, Input.mousePosition, itemManager.mainCamera, out mousePos);
        itemmenu.transform.localPosition= new Vector3(mousePos.x, mousePos.y + 1, 0);
        GameObject destroyButton = GameObject.Instantiate(itemManager.itemMenuPrefabs[1]);
        destroyButton.transform.SetParent(itemManager.canvas);
        destroyButton.transform.localPosition = new Vector3(0, 0, 0);
        itemmenu.transform.SetParent(destroyButton.transform);

        if(GameManager.player.itemMenu == null)
        GameManager.player.itemMenu = destroyButton;
        else
        {
            Destroy(GameManager.player.itemMenu);
            GameManager.player.itemMenu = destroyButton;
        }


        for (int i = 0; i < 3; i++)
        {
            GameObject nowLine = GameObject.Instantiate(itemManager.cellPrefabs[2]);
            nowLine.transform.SetParent(itemmenu.transform);
            nowLine.transform.localPosition = new Vector3(-CellGroup.cellSize + 1 + i * (CellGroup.cellSize - 1), -(float)1.5);              
        }

        GameObject leftConer = GameObject.Instantiate(itemManager.cellPrefabs[1]);
        leftConer.transform.SetParent(itemmenu.transform);
        leftConer.transform.localPosition = new Vector3(-CellGroup.cellSize + 1 - CellGroup.cellSize / 2 + (float)0.5, -(float)1.5 - buttons.Length * itemmenu.GetComponent<RectTransform>().sizeDelta.y);
        leftConer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));

        GameObject rightConer = GameObject.Instantiate(itemManager.cellPrefabs[1]);
        rightConer.transform.SetParent(itemmenu.transform);
        rightConer.transform.localPosition = new Vector3(-CellGroup.cellSize + 1 + 2 * (CellGroup.cellSize - 1) + CellGroup.cellSize / 2 - (float)0.5, -(float)1.5 - buttons.Length * itemmenu.GetComponent<RectTransform>().sizeDelta.y);
        rightConer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));


        for (int i = 0; i < 3; i++)
        {
            GameObject nowLine = GameObject.Instantiate(itemManager.cellPrefabs[2]);
            nowLine.transform.SetParent(itemmenu.transform);
            nowLine.transform.localPosition = new Vector3(-CellGroup.cellSize + 1 + i * (CellGroup.cellSize - 1), -(float)1.5 - buttons.Length*itemmenu.GetComponent<RectTransform>().sizeDelta.y);
            nowLine.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        

        for (int i = 0; i < buttons.Length;i++)
        {
            GameObject nowMenu = GameObject.Instantiate(itemManager.itemMenuPrefabs[0]);
            nowMenu.transform.SetParent(itemmenu.transform);
            nowMenu.transform.localPosition = new Vector3(0,-1 -i * itemmenu.GetComponent<RectTransform>().sizeDelta.y);

            Button button = nowMenu.GetComponentInChildren<Button>();

            if (buttons[i] == "Use")
                button.onClick.AddListener(delegate { Use(); Destroy(destroyButton); GameManager.player.itemMenu = null; });
            else if(buttons[i] == "Inspect")
                button.onClick.AddListener(delegate { Inspect(); Destroy(destroyButton); GameManager.player.itemMenu = null; });
            else if (buttons[i] == "Open")
                button.onClick.AddListener(delegate { Open(); Destroy(destroyButton); GameManager.player.itemMenu = null; });
            else if (buttons[i] == "Unload")
                button.onClick.AddListener(delegate { Unload(); Destroy(destroyButton); GameManager.player.itemMenu = null; });
            else if (buttons[i] == "Discard")
                button.onClick.AddListener(delegate { Discard(); Destroy(destroyButton); GameManager.player.itemMenu = null; });

            button.GetComponentInChildren<Text>().text = buttons[i];


        }





    }

    public void Use()
    {
        if (cellType == 1)
        {
            Consumable consumable = (Consumable)Item.GetItem(itemCode);
            consumable.useEffect.Invoke();
            if (consumable.isDisposable && ConsumableEvent.result)
            {
                if (amount <= 1)
                    thisContainer.RemoveItem(x, y);
                else
                    thisContainer.RemoveItem(1,x,y);
            }
            if (ConsumableEvent.result)
                GameManager.cameraManager.PlayConsumableSound(consumable.useSoundIndex);

            itemManager.ContainerReRender(this);
        }
    }

    public void Inspect()
    {
        GameObject inspectUI = GameObject.Instantiate(itemManager.itemMenuPrefabs[2]);
        inspectUI.transform.SetParent(itemManager.canvas);
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(itemManager.canvas, Input.mousePosition, itemManager.mainCamera, out mousePos);

        Vector3 SpriteSize = inspectUI.GetComponent<Image>().sprite.bounds.size;



        if (mousePos.x - SpriteSize.x / 2 < - itemManager.canvas.sizeDelta.x/2)
            mousePos.x = -itemManager.canvas.sizeDelta.x / 2 + SpriteSize.x / 2;
        if (mousePos.x + SpriteSize.x / 2 > itemManager.canvas.sizeDelta.x/2)
            mousePos.x = itemManager.canvas.sizeDelta.x/2 - SpriteSize.x / 2;
        if (mousePos.y - SpriteSize.y < -itemManager.canvas.sizeDelta.y / 2)
            mousePos.y = -itemManager.canvas.sizeDelta.y / 2 + SpriteSize.y;


        inspectUI.transform.localPosition = new Vector3(mousePos.x, mousePos.y, 0);

        inspectUI.transform.Find("Name").GetComponent<Text>().text = GameManager.stringManager.GetItemString(itemCode).name;
        inspectUI.transform.Find("Inspect").GetComponent<Text>().text = GameManager.stringManager.GetItemString(itemCode).inspect;

        GameManager.cameraManager.PlayUISound(6);

    }

    public void Open()
    {
        if (itemCode / 10000 == 205)
        {
            GameManager.cameraManager.PlayUISound(3);
            CellGroup.RenderingBackpackContainer(this);
        }
        else if (itemCode / 10000 == 204)
        {
            GameManager.cameraManager.PlayUISound(4);
            CellGroup.RenderingChestRigContainer(this);
        }
        else if (itemCode / 10000 == 201)
        {
            GameManager.cameraManager.PlayUISound(5);
            AttachmentCellGroup.RenderingCellGroup(this);
        }
    }

    public void Unload()
    {
        if(cellType == 1)
        {
            PlusContainerManager thisPLCManager = thisContainer.GetThisPLCManager();
            string contents = thisPLCManager.plusContainers[PLCcode[0]];

            if (contents.Length == 0)
                return;

            string ammoCode;
            int ammoAmount = 1;

            ammoCode = contents.Substring(contents.Length - 4, 4);
            contents = contents.Substring(0, contents.Length - 4);

            while(contents.Length > 0)
            {
                if (ammoCode == contents.Substring(contents.Length - 4, 4))
                {
                    ammoAmount++;
                    contents = contents.Substring(0, contents.Length - 4);
                }
                else
                    break;

            }

            int iammoCode = 4000000 + int.Parse(ammoCode);

            bool[,] boolmap = thisContainer.GetBoolMap(thisContainer.GetContents());
            bool successInput = false;

            for (int y = 0; y < thisContainer.ySize; y++)
            {
                for (int x = 0; x < thisContainer.xSize; x++)
                {
                    if (!boolmap[y, x])
                    {
                        if (thisContainer.InsertItem(iammoCode,new int[0],ammoAmount,true,x,y))
                        {
                            successInput = true;
                            thisPLCManager.plusContainers[PLCcode[0]] = contents;
                            itemManager.ContainerReRender(this);
                            GameManager.cameraManager.PlayUISound(7);
                            break;
                        }
                        else if (thisContainer.InsertItem(iammoCode, new int[0], ammoAmount, false, x, y))
                        {
                            successInput = true;
                            thisPLCManager.plusContainers[PLCcode[0]] = contents;
                            itemManager.ContainerReRender(this);
                            GameManager.cameraManager.PlayUISound(7);
                            break;
                        }


                    }


                }
                if (successInput)
                {
                    break;
                }

            }

           



        }
        else if(cellType == 2)
        {
            PlusContainerManager thisPLCManager = attachmentContainer.GetThisPLCManager();
            string contents = thisPLCManager.plusContainers[PLCcode[0]];

            if (contents.Length == 0)
                return;

            string ammoCode;
            int ammoAmount = 1;

            ammoCode = contents.Substring(contents.Length - 4, 4);
            contents = contents.Substring(0, contents.Length - 4);

            while (contents.Length > 0)
            {
                if (ammoCode == contents.Substring(contents.Length - 4, 4))
                {
                    ammoAmount++;
                    contents = contents.Substring(0, contents.Length - 4);
                }
                else
                    break;

            }

            int iammoCode = 4000000 + int.Parse(ammoCode);

            if(transform.parent.GetComponent<AttachmentCellGroup>().isGearGun)
            {
                if(itemManager.playerGear.GearItems[7] != 0)
                {
                    ChestRig chestRig = (ChestRig)Item.GetItem(itemManager.playerGear.GearItems[7]);
                    Container[] chestRogContainers = new Container[chestRig.rigCells.Length];
                    for (int i = 0; i < chestRig.rigCells.Length; i++)
                    {
                        chestRogContainers[i] = new Container(itemManager.playerGear.chestrigPLC[i], true, chestRig.rigCells[i].xsize, chestRig.rigCells[i].ysize);
                    }
                    bool successInput = false;


                    for (int i = 0; i < chestRogContainers.Length; i++)
                    {

                        for (int y = 0; y < chestRogContainers[i].ySize; y++)
                        {
                          for (int x = 0; x < chestRogContainers[i].xSize; x++)
                          {
                            
                                if (chestRogContainers[i].InsertItem(iammoCode, new int[0], ammoAmount, true, x, y))
                                {
                                    successInput = true;
                                    thisPLCManager.plusContainers[PLCcode[0]] = contents;
                                    itemManager.playerGear.PlayerGearReRendering();
                                    GameManager.cameraManager.PlayUISound(7);
                                    return;
                                }
                                else if (chestRogContainers[i].InsertItem(iammoCode, new int[0], ammoAmount, false, x, y))
                                {
                                    successInput = true;
                                    thisPLCManager.plusContainers[PLCcode[0]] = contents;
                                    itemManager.playerGear.PlayerGearReRendering();
                                    GameManager.cameraManager.PlayUISound(7);
                                    return;
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

                }


                if(itemManager.playerGear.GearItems[6] != 0 && itemManager.playerGear.player.isOpenBackpack)
                {
                    Backpack backpack = (Backpack)Item.GetItem(itemManager.playerGear.GearItems[6]);
                    Container parentContainer = new Container(itemManager.playerGear.backpackPLC, true, backpack.xSpace, backpack.ySpace);

                    bool[,] boolmap = parentContainer.GetBoolMap(parentContainer.GetContents());
                    bool successInput = false;

                    for (int y = 0; y < parentContainer.ySize; y++)
                    {
                        for (int x = 0; x < parentContainer.xSize; x++)
                        {
                            if (!boolmap[y, x])
                            {
                                if (parentContainer.InsertItem(iammoCode, new int[0], ammoAmount, true, x, y))
                                {
                                    successInput = true;
                                    thisPLCManager.plusContainers[PLCcode[0]] = contents;
                                    itemManager.playerGear.PlayerGearReRendering();
                                    GameManager.cameraManager.PlayUISound(7);
                                    return;
                                }
                                else if (parentContainer.InsertItem(iammoCode, new int[0], ammoAmount, false, x, y))
                                {
                                    successInput = true;
                                    thisPLCManager.plusContainers[PLCcode[0]] = contents;
                                    itemManager.playerGear.PlayerGearReRendering();
                                    GameManager.cameraManager.PlayUISound(7);
                                    return;
                                }


                            }


                        }
                        if (successInput)
                        {
                            break;
                        }

                    }


                }

                //총알 버리기처리




            }
            else
            {
                Container parentContainer = transform.parent.parent.GetComponent<CellGroup>().container;

                bool[,] boolmap = parentContainer.GetBoolMap(parentContainer.GetContents());
                bool successInput = false;

                for (int y = 0; y < parentContainer.ySize; y++)
                {
                    for (int x = 0; x < parentContainer.xSize; x++)
                    {
                        if (!boolmap[y, x])
                        {
                            if (parentContainer.InsertItem(iammoCode, new int[0], ammoAmount, true, x, y))
                            {
                                successInput = true;
                                thisPLCManager.plusContainers[PLCcode[0]] = contents;
                                itemManager.ContainerReRender(transform.parent.parent.GetComponent<CellGroup>());
                                GameManager.cameraManager.PlayUISound(7);
                                break;
                            }
                            else if (parentContainer.InsertItem(iammoCode, new int[0], ammoAmount, false, x, y))
                            {
                                successInput = true;
                                thisPLCManager.plusContainers[PLCcode[0]] = contents;
                                itemManager.ContainerReRender(transform.parent.parent.GetComponent<CellGroup>());
                                GameManager.cameraManager.PlayUISound(7);
                                break;
                            }


                        }


                    }
                    if (successInput)
                    {
                        break;
                    }

                }




            }



        }

    }

    public void Discard()
    {
        GameManager.cameraManager.PlayUISound(1);

        if(cellType == 1)
        {
            if (!Item.CheckPLC(itemCode))
            {
                thisContainer.RemoveItem(x, y);
                DropItem.CreateDropItem(itemCode, PLCcode, amount);
                itemManager.ContainerReRender(this);
            }
            else
            {

                if (thisContainer.isNeedSave)
                {
                    for (int i = 0; i < PLCcode.Length; i++)
                        PLCcode[i] = PlusContainerManager.newAllocatePLCnode(PLCcode[i], thisContainer.GetThisPLCManager(), GameManager.nonSavePLCManager);

                    thisContainer.RemoveOnlyTextCode(x, y);
                    DropItem.CreateDropItem(itemCode, PLCcode, amount);
                    itemManager.ContainerReRender(this);


                }
                else
                {
                    thisContainer.RemoveOnlyTextCode(x, y);
                    DropItem.CreateDropItem(itemCode, PLCcode, amount);
                    itemManager.ContainerReRender(this);
                }

             }

        }
        else if(cellType == 2)
        {
            if (!Item.CheckPLC(itemCode))
            {
                attachmentContainer.RemoveItem(index);
                DropItem.CreateDropItem(itemCode, PLCcode, amount);
                itemManager.AttachmentContainerReRender(this);
            }
            else
            {

                if (attachmentContainer.isNeedSave)
                {
                    for (int i = 0; i < PLCcode.Length; i++)
                        PLCcode[i] = PlusContainerManager.newAllocatePLCnode(PLCcode[i], attachmentContainer.GetThisPLCManager(), GameManager.nonSavePLCManager);

                    attachmentContainer.RemoveOnlyTextCode(index);
                    DropItem.CreateDropItem(itemCode, PLCcode, amount);
                    itemManager.AttachmentContainerReRender(this);
                }
                else
                {
                    attachmentContainer.RemoveOnlyTextCode(index);
                    DropItem.CreateDropItem(itemCode, PLCcode, amount);
                    itemManager.AttachmentContainerReRender(this);
                }

            }




        }
        else
        {
            if(2 < index && index < 6)
            {
                GameManager.itemManager.playerGear.RemoveOnlyTextCode(index);
                DropItem.CreateDropItem(itemCode, PLCcode, amount);
                GameManager.itemManager.playerGear.PlayerGearReRendering();
            }
            else
            {
                for (int i = 0; i < PLCcode.Length; i++)
                    PLCcode[i] = PlusContainerManager.newAllocatePLCnode(PLCcode[i], GameManager.saveManager.savePLC, GameManager.nonSavePLCManager);

                GameManager.itemManager.playerGear.RemoveOnlyTextCode(index);
                DropItem.CreateDropItem(itemCode, PLCcode, amount);

                if(index < 3 || index == 6)
                {
                    if (index < 3 && GameManager.player.selectGun == index)
                        GameManager.player.ForceChangeToKnife();

                    if ((GameManager.player.selectGun == 2 || GameManager.player.selectGun == 3) && index < 2)
                    {
                        if (GameManager.player.middleGun == index)
                        {
                            if (GameManager.itemManager.playerGear.GearItems[1 - index] != 0)
                            {
                                GameManager.player.middleGun = 1 - index;
                            }
                            else
                                GameManager.player.middleGun = -1;
                        }


                    }

                    if (index < 3 || index == 6)
                        GameManager.player.ReRenderingPlayerPart();

                    if (index == 0)
                        GameManager.player.main1FireMode = 0;
                    else if (index == 1)
                        GameManager.player.main2FireMode = 0;
                    else if (index == 2)
                        GameManager.player.pistolFireMode = 0;

                }

                GameManager.itemManager.playerGear.PlayerGearReRendering();

            }



        }

    }






}
