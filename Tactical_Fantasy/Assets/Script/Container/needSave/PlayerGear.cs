using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PlayerGear
{
    public int[] GearItems = new int[8] {0,0,0,0,0,0,0,0 };
    public int[] GunsPLC = new int[3];
    public int[] chestrigPLC;
    public int backpackPLC;

    public Player player;
    public GameObject Canvas;
    public ItemManager itemManager;
    public PlusContainerManager savePLCManager;

    public GameObject PlayerUI;
    public GameObject BackpackCellGroup;
    public AttachmentCellGroup nowAttachmentCellGroup = null;

    const float leftVMagin = 2;
    const float leftHMagin = 2;
    const float P1 = 22;
    const float P2 = 41;
    const float P3 = 60;
    const float leftUIX = 4;
    const float leftUIY = 325;
    const float ChestRigUIX = -756; // -786
    const float ChestRigUIY = 167; //43 //187


    enum GearIndex
    {
        MainGun1 = 0,
        MainGun2 = 1,
        Pistol = 2,
        Helmet = 3,
        Goggles = 4,
        Uniform = 5,
        Backpack = 6,
        ChestRig = 7
    }

    public readonly int[] GearIndexType = new int[8] { 201, 201, 201, 202, 206, 203, 205, 204 };

    enum GearCellPrefabsIndex
    {
        MainBar = 0,
        Bar = 1,
        MainGun1 = 2,
        MainGun2 = 3,
        Pistol = 4,
        Helmet = 5,
        Goggles = 6,
        Uniform = 7,
        BackpackBar = 8,
        Backpack = 9,
        ChestrigBar = 10,
        Chestrig =11
    }


    public void RenderingPlayerGear()
    {

        GameObject PlayerUI = GameObject.Instantiate(itemManager.cellPrefabs[0]);
        this.PlayerUI = PlayerUI;
        GameObject.Destroy(PlayerUI.GetComponent<Image>());
        PlayerUI.transform.SetParent(Canvas.transform,false);
        PlayerUI.transform.localPosition = new Vector3(-Canvas.GetComponent<RectTransform>().sizeDelta.x/2, -Canvas.GetComponent<RectTransform>().sizeDelta.y/2, -1);

        void CreateLeftGearCell(float xPos,float yPos, int index)
        {
            xPos += leftUIX;
            yPos += leftUIY;

            GameObject bar;
            if (index == 0 || index == 1)           
                bar = GameObject.Instantiate(itemManager.gearCellPrefabs[0]);            
            else            
                bar = GameObject.Instantiate(itemManager.gearCellPrefabs[1]);
            
            bar.transform.SetParent(PlayerUI.transform, false);
            bar.transform.localPosition = new Vector3(xPos -2, yPos, -1);

            GameObject Cell = GameObject.Instantiate(itemManager.gearCellPrefabs[index + 2]);
            Cell.transform.SetParent(PlayerUI.transform, false);
            Cell.transform.localPosition = bar.transform.localPosition + new Vector3(bar.GetComponent<RectTransform>().sizeDelta.x + leftVMagin, 0, 0);

            if(GearItems[index] == 0)
            {
                Cell.AddComponent<PlayerGearCell>().index = index;
            }
            else
            {
                Cell.GetComponent<Image>().sprite = itemManager.gearEmptyCellSprites[index];

                if (player.selectGun != index || 3 <= index)
                {
                    GameObject ParentMask = GameObject.Instantiate(itemManager.cellPrefabs[0]);
                    ParentMask.transform.SetParent(Cell.transform, false);
                    ParentMask.AddComponent<Mask>().showMaskGraphic = false;
                    ParentMask.transform.localPosition = new Vector3(Cell.GetComponent<RectTransform>().sizeDelta.x / 2, Cell.GetComponent<RectTransform>().sizeDelta.y / 2, -1);
                    Sprite itemSprite = Item.GetItem(GearItems[index]).itemSprite;
                    ParentMask.GetComponent<RectTransform>().sizeDelta = new Vector2(itemSprite.bounds.size.x -4, itemSprite.bounds.size.y -4);

                    GameObject ItemImageObject = GameObject.Instantiate(itemManager.cellPrefabs[0]);
                    ItemImageObject.GetComponent<Image>().sprite = itemSprite;
                    ItemImageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(itemSprite.bounds.size.x, itemSprite.bounds.size.y);
                    ItemImageObject.transform.SetParent(ParentMask.transform, false);
                    ItemImageObject.transform.localPosition = new Vector3(0,0,0);
                }
                else
                {
                    AttachmentContainer GunContainer = new AttachmentContainer(GunsPLC[index], true, GearItems[index], ((Gun)Item.GetItem(GearItems[index])).attachmentCells);                   

                    nowAttachmentCellGroup = AttachmentCellGroup.RenderingCellGroup(Cell, GunContainer, new Vector2(Cell.GetComponent<RectTransform>().sizeDelta.x / 2 - Item.GetItem(GearItems[index]).itemSprite.bounds.size.x/2, Cell.GetComponent<RectTransform>().sizeDelta.y / 2 + Item.GetItem(GearItems[index]).itemSprite.bounds.size.y/2), true, itemManager.cellPrefabs).GetComponent<AttachmentCellGroup>();
                    nowAttachmentCellGroup.GetComponent<Image>().material = GameManager.itemManager.NoLineItemMaterial;
                }

                GameObject barItemImageObject =  GameObject.Instantiate(itemManager.gearCellPrefabs[0]);
                barItemImageObject.transform.SetParent(bar.transform, false);
                barItemImageObject.GetComponent<Image>().sprite = Item.GetItem(GearItems[index]).itemSprite;
                barItemImageObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                barItemImageObject.GetComponent<RectTransform>().sizeDelta = bar.GetComponent<RectTransform>().sizeDelta;
                barItemImageObject.transform.localPosition = new Vector3(0, 0, -1);

                ItemImage barItemImage = barItemImageObject.AddComponent<ItemImage>();
                barItemImage.cellType = 3;
                barItemImage.index = index;
                barItemImage.itemCode = GearItems[index];
                barItemImage.isVertical = true;
                barItemImage.amount = 1;
                if (index == 0 || index == 1 || index == 2)
                    barItemImage.PLCcode = new int[1] { GunsPLC[index] };

            }

        }
        if (player.selectGun == 0 || player.selectGun == 3)
        {
            CreateLeftGearCell(leftVMagin, leftHMagin, 0);
            CreateLeftGearCell(leftVMagin, 2 * leftHMagin + P3, 1);
            CreateLeftGearCell(leftVMagin, 3 * leftHMagin + 2 * P3, 2);
        }
        else if(player.selectGun == 1)
        {
            CreateLeftGearCell(leftVMagin, leftHMagin, 1);
            CreateLeftGearCell(leftVMagin, 2 * leftHMagin + P3, 0);
            CreateLeftGearCell(leftVMagin, 3 * leftHMagin + 2 * P3, 2);
        }
        else if(player.selectGun == 2)
        {
            CreateLeftGearCell(leftVMagin, leftHMagin, 2);
            CreateLeftGearCell(leftVMagin, 2 * leftHMagin + P2, 0);
            CreateLeftGearCell(leftVMagin, 3 * leftHMagin + P2+ P3, 1);
        }

        CreateLeftGearCell(leftVMagin, 4 * leftHMagin + 2 * P3 + P2, 3);
        CreateLeftGearCell(3 * leftVMagin + P1 + P2, 4 * leftHMagin + 2 * P3 + P2, 4);
        CreateLeftGearCell(5 * leftVMagin + 2*P1 + 2*P2, 4 * leftHMagin + 2 * P3 + P2, 5);

        if (player.selectGun == 3)
            nowAttachmentCellGroup = null;

        //backpackUI

        GameObject backpackBar;

        backpackBar = GameObject.Instantiate(itemManager.gearCellPrefabs[8]);
        backpackBar.transform.SetParent(PlayerUI.transform, false);
        backpackBar.transform.localPosition = new Vector3(Canvas.GetComponent<RectTransform>().sizeDelta.x - leftVMagin - 20, Canvas.GetComponent<RectTransform>().sizeDelta.y - leftHMagin - 3, -1);

        GameObject backpackCell = GameObject.Instantiate(itemManager.gearCellPrefabs[9]);
        backpackCell.transform.SetParent(PlayerUI.transform, false);
        backpackCell.transform.localPosition = backpackBar.transform.localPosition + new Vector3(0, -P1 - leftHMagin, 0);

        if(GearItems[6] == 0)
        {
            backpackCell.AddComponent<PlayerGearCell>().index = 6;
            BackpackCellGroup = null;
        }
        else
        {
            backpackCell.GetComponent<Image>().sprite = itemManager.gearEmptyCellSprites[6];

            GameObject ParentMask = GameObject.Instantiate(itemManager.cellPrefabs[0]);
            ParentMask.transform.SetParent(backpackCell.transform, false);
            ParentMask.AddComponent<Mask>().showMaskGraphic = false;
            ParentMask.transform.localPosition = new Vector3(-backpackCell.GetComponent<RectTransform>().sizeDelta.x / 2, -backpackCell.GetComponent<RectTransform>().sizeDelta.y / 2, -1);
            Sprite itemSprite = Item.GetItem(GearItems[6]).itemSprite;
            ParentMask.GetComponent<RectTransform>().sizeDelta = new Vector2(itemSprite.bounds.size.x - 4, itemSprite.bounds.size.y - 4);

            GameObject ItemImageObject = GameObject.Instantiate(itemManager.cellPrefabs[0]);
            ItemImageObject.GetComponent<Image>().sprite = itemSprite;
            ItemImageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(itemSprite.bounds.size.x, itemSprite.bounds.size.y);
            ItemImageObject.transform.SetParent(ParentMask.transform, false);
            ItemImageObject.transform.localPosition = new Vector3(0, 0, 0);

          

            if (player.isOpenBackpack)
            {
                Vector2 CellgroupPos = (Vector2)ParentMask.transform.localPosition + new Vector2(-ItemImageObject.GetComponent<Image>().sprite.bounds.size.x / 2 + 1, ItemImageObject.GetComponent<Image>().sprite.bounds.size.y / 2 - 1);
                BackpackCellGroup = CellGroup.RenderingBackpackContainer(backpackCell, GearItems[6], backpackPLC, true, CellgroupPos);
                BackpackCellGroup.GetComponent<CellGroup>().isGearBackpack = true;
            }
            else
            {
                BackpackCellGroup = null;

                GameObject barItemImageObject = GameObject.Instantiate(itemManager.gearCellPrefabs[8]);
                barItemImageObject.transform.SetParent(backpackBar.transform, false);
                barItemImageObject.GetComponent<Image>().sprite = Item.GetItem(GearItems[6]).itemSprite;
                barItemImageObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                barItemImageObject.GetComponent<RectTransform>().sizeDelta = backpackBar.GetComponent<RectTransform>().sizeDelta;
                barItemImageObject.transform.localPosition = new Vector3(0, 0, -1);

                ItemImage barItemImage = barItemImageObject.AddComponent<ItemImage>();
                barItemImage.cellType = 3;
                barItemImage.index = 6;
                barItemImage.itemCode = GearItems[6];
                barItemImage.isVertical = true;
                barItemImage.amount = 1;
                barItemImage.PLCcode = new int[1] { backpackPLC };


            }


        }



        //chestRig

        GameObject chestRigCell = GameObject.Instantiate(itemManager.gearCellPrefabs[11]);
        chestRigCell.transform.SetParent(PlayerUI.transform, false);
        chestRigCell.transform.localPosition = new Vector3(Canvas.GetComponent<RectTransform>().sizeDelta.x - leftVMagin, leftHMagin, -1);
        chestRigCell.transform.localPosition += new Vector3(ChestRigUIX, ChestRigUIY, 0);

        GameObject chestRIgBar = GameObject.Instantiate(itemManager.gearCellPrefabs[10]);
        chestRIgBar.transform.SetParent(PlayerUI.transform, false);
        //chestRIgBar.transform.localPosition = chestRigCell.transform.localPosition + new Vector3(0, chestRigCell.GetComponent<Image>().sprite.bounds.size.y + leftHMagin, 0);
        chestRIgBar.transform.localPosition = chestRigCell.transform.localPosition + new Vector3(-chestRigCell.GetComponent<Image>().sprite.bounds.size.x -chestRIgBar.GetComponent<Image>().sprite.bounds.size.y - leftVMagin, 0, 0);
        chestRIgBar.transform.localRotation = Quaternion.Euler(0, 0, -90);

        if (GearItems[7] == 0)
        {
            chestRigCell.AddComponent<PlayerGearCell>().index = 7;
        }
        else
        {
            chestRigCell.GetComponent<Image>().sprite = itemManager.gearEmptyCellSprites[7];

            GameObject barItemImageObject = GameObject.Instantiate(itemManager.gearCellPrefabs[10]);
            barItemImageObject.transform.SetParent(chestRIgBar.transform, false);
            barItemImageObject.GetComponent<Image>().sprite = Item.GetItem(GearItems[7]).itemSprite;
            barItemImageObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            barItemImageObject.GetComponent<RectTransform>().sizeDelta = chestRIgBar.GetComponent<RectTransform>().sizeDelta;
            barItemImageObject.transform.localPosition = new Vector3(0, 0, -1);

            ItemImage barItemImage = barItemImageObject.AddComponent<ItemImage>();
            barItemImage.cellType = 3;
            barItemImage.index = 7;
            barItemImage.itemCode = GearItems[7];
            barItemImage.isVertical = true;
            barItemImage.amount = 1;
            barItemImage.PLCcode = chestrigPLC;


            CellGroup.RenderingChestRigContainer(chestRigCell, GearItems[7], chestrigPLC, true, new Vector2(-chestRigCell.GetComponent<RectTransform>().sizeDelta.x / 2, chestRigCell.GetComponent<RectTransform>().sizeDelta.y / 2));


        }



    }

    public void PlayerGearReRendering()
    {
        CellGroup[] childs = null;
        bool saveChild = (BackpackCellGroup != null && player.isOpenBackpack);

        if (saveChild)
        {
            childs = BackpackCellGroup.GetComponentsInChildren<CellGroup>();
            foreach (CellGroup child in childs)
            {
                if (child.transform.parent == BackpackCellGroup.transform)
                    child.transform.SetParent(Canvas.transform);
            }
        }

        GameObject.Destroy(PlayerUI);
        RenderingPlayerGear();

        if (saveChild)
        {
            foreach (CellGroup child in childs)
            {
                if (child.transform.parent == Canvas.transform)
                    child.transform.SetParent(BackpackCellGroup.transform);
            }

        }

    }


    public bool insertItem(int itemCode, int[] PLCcode,int index)
    {
        if (GearItems[index] == 0 && itemCode / 10000 == GearIndexType[index])
        {

            if(index == 0 || index == 1)
            {
                if (((Gun)Item.GetItem(itemCode)).gunAnimation == Gun.GunAnimation.Pistol)
                    return false;

            }
            else if(index == 2)
            {
                if (((Gun)Item.GetItem(itemCode)).gunAnimation != Gun.GunAnimation.Pistol)
                    return false;
            }


            GearItems[index] = itemCode;

            if (index == 0 || index == 1 || index == 2)
                GunsPLC[index] = PLCcode[0];
            else if (index == 6)
                backpackPLC = PLCcode[0];
            else if (index == 7)
                chestrigPLC = PLCcode;


            return true;
           
        }
        else
            return false;




    }

    public bool CaninsertItem(int itemCode, int index)
    {
        if (GearItems[index] == 0 && itemCode / 10000 == GearIndexType[index])
        {

            if (index == 0 || index == 1)
            {
                if (((Gun)Item.GetItem(itemCode)).gunAnimation == Gun.GunAnimation.Pistol)
                    return false;

            }
            else if (index == 2)
            {
                if (((Gun)Item.GetItem(itemCode)).gunAnimation != Gun.GunAnimation.Pistol)
                    return false;
            }


            return true;

        }
        else
            return false;
    }



    public bool RemoveOnlyTextCode(int index)
    {
        if (GearItems[index] == 0)
            return false;

        GearItems[index] = 0;


        if (index == 0 || index == 1 || index == 2)
            GunsPLC[index] = -1;
        else if (index == 6)
            backpackPLC = -1;
        else if (index == 7)
            chestrigPLC = new int[0];

        return true;


    }

    public bool MoveItemToContainerCell(int index, bool isVertical, Container toContaiener, int toX, int toY)
    {
        if (index < 0 || index > 7)
            return false;

        if (GearItems[index] == 0)
            return false;


        if(2< index && index < 6) // non PLC
        {
            if(toContaiener.InsertItem(GearItems[index],new int[0],1,isVertical,toX,toY))
            {
                RemoveOnlyTextCode(index);
                return true;
            }
            else
                return false;

        }
        else //PLC
        {
            int[] PLCcode;

            if (index < 3)
                PLCcode = new int[1] { GunsPLC[index] };
            else if (index == 6)
                PLCcode = new int[1] { backpackPLC };
            else
                PLCcode = chestrigPLC;

            if (toContaiener.isNeedSave)
            {
                if (toContaiener.InsertItem(GearItems[index], PLCcode, 1, isVertical, toX, toY))
                {
                    RemoveOnlyTextCode(index);
                    return true;
                }
                else
                    return false;


            }
            else
            {
                if (toContaiener.CanInsertItem(GearItems[index], isVertical, toX, toY))
                {
                    for (int i = 0; i < PLCcode.Length; i++)
                        PLCcode[i] = PlusContainerManager.newAllocatePLCnode(PLCcode[i], savePLCManager, toContaiener.GetThisPLCManager());

                    if(toContaiener.InsertItem(GearItems[index], PLCcode, 1, isVertical, toX, toY))
                    {
                        RemoveOnlyTextCode(index);
                        return true;
                    }
                    else
                    {
                        Debug.Log("MyError: Serious Error!!!!!!!!!!!! Move PLC code But Fail Move Item!!!!!!!!!");
                        return false;
                    }


                }
                else
                    return false;

                



            }


            
        }


    }

    public void Save(int slot)
    {
        if (!Directory.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString()))
        {
            Directory.CreateDirectory(Application.dataPath + "/save" + "/slot" + slot.ToString());
        }

        StreamWriter gear = File.CreateText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/gear.tps");
        StreamWriter gearPLC = File.CreateText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/gearPLC.tps");

        for(int i = 0;i<8;i++)
        {
            gear.Write(GearItems[i].ToString() + ",");
        }

        for(int i = 0;i<3;i++)
        {
            if(GearItems[i] != 0)
                gearPLC.Write(GunsPLC[i].ToString() + ",");
            else
                gearPLC.Write("n,");
        }

        if(GearItems[6] != 0)
            gearPLC.Write(backpackPLC.ToString() + ",");
        else
            gearPLC.Write("n,");

        if(GearItems[7] != 0)
        {
            for(int i = 0;i< chestrigPLC.Length;i++)
            gearPLC.Write(chestrigPLC[i].ToString() + ",");
        }
        else
            gearPLC.Write("n,");

        gear.Close();
        gearPLC.Close();
    }

    public void Load(int slot)
    {
        if (File.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/gear.tps") && File.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/gearPLC.tps"))
        {
            string gear = File.ReadAllText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/gear.tps");
            string gearPLC = File.ReadAllText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/gearPLC.tps");

            string[] gears = gear.Split(',');

            for(int i = 0; i< 8;i++)
            {
                GearItems[i] =  int.Parse(gears[i]);
            }

            string[] gearPLCs = gearPLC.Split(',');

            for (int i = 0; i < 3; i++)
            {
                if (gearPLCs[i] != "n")
                    GunsPLC[i] = int.Parse(gearPLCs[i]);
            }

            if(gearPLCs[3] != "n")
                backpackPLC = int.Parse(gearPLCs[3]);

            if(gearPLCs[4] != "n")
            {
                chestrigPLC = new int[gearPLCs.Length - 5];

                for (int i = 0; i < chestrigPLC.Length; i++)
                    chestrigPLC[i] = int.Parse(gearPLCs[i + 4]);
            }
                

        }
        else
            Debug.LogError("no SaveFile");
    }


}
