using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellGroup : MonoBehaviour
{
    public const float cellSize = 20;
    public const float xConerSize = -1;
    public const float yConerSize = -1;
    public Container container;
    public float xLocalPos;
    public float yLocalPos;
    public bool Exit = false;
    public bool isGearBackpack = false;
    public bool isGearChestRig = false;
    public Chest chest = null;
    public Shop shop = null;
    public GameObject spriteObject = null;
    public enum CellImage
    {
        cell = 0,
        coner = 1,
        Line = 2
    }
    

    public static GameObject RenderContainer(GameObject parent, Container container, Vector2 LPos,bool isCreateExit ,GameObject[] cellPrefabs , Font amountFont)
    {
        GameObject cellGroupObject = CreateCellGroup(parent, container, LPos, isCreateExit,cellPrefabs);

        string contents = container.GetContents();

        for (int i = 0; i<contents.Length;i++)
        {
           

            if(contents[i] == 'x')
            {
                string x = "";
                string y = "";
                string item = "";
                bool isVertical;
                string amount = "";
                string PLCcode = "";

                i++;
                while(contents[i] != 'y')
                {
                    x += contents[i];
                    i++;
                }
                i++;

                while(contents[i] != 'v' && contents[i] != 'h')
                {
                    y += contents[i];
                    i++;
                }
                isVertical = contents[i] == 'v';

                i++;

                for(int j=0;j<7;j++)
                {
                    item += contents[i];
                    i++;
                }

                i++;

                while(i < contents.Length && contents[i] != 'x' && contents[i] != 'p')
                {
                    amount += contents[i];
                    i++;
                }

                if(i < contents.Length && contents[i] == 'p')
                {
                    while (i < contents.Length && contents[i] != 'x')
                    {
                        PLCcode += contents[i];
                        i++;
                    }

                }
                i--;


                //plc및 어마운트 처리


                

                Sprite itemsprite = Item.GetItem(int.Parse(item)).itemSprite;
                //자기 반사이즈만큼 x+ y-

                GameObject itemImage = Instantiate(cellPrefabs[0]);

                if (int.Parse(amount) > 1)
                {
                    GameObject amountTextObject = Instantiate(cellPrefabs[4]);
                    amountTextObject.transform.SetParent(itemImage.transform);
                    Text amountText = amountTextObject.GetComponent<Text>();
                    amountText.raycastTarget = false;
                    amountText.text = amount;
                    amountText.alignment = TextAnchor.LowerRight;
                    amountText.font = amountFont;
                    amountText.color = new Color(0, 0, 0);

                    if (isVertical)
                    {
                        float aX = itemsprite.bounds.size.x / 2 - amountTextObject.GetComponent<RectTransform>().sizeDelta.x / 2 - 1.5f;
                        float aY = -(itemsprite.bounds.size.y / 2 - amountTextObject.GetComponent<RectTransform>().sizeDelta.y / 2) + 0.5f;

                        amountTextObject.transform.localPosition = new Vector3(aX, aY, -1);
                    }
                    else
                    {
                        float aX = itemsprite.bounds.size.x / 2 - amountTextObject.GetComponent<RectTransform>().sizeDelta.x / 2 -0.5f;
                        float aY = itemsprite.bounds.size.y / 2 - amountTextObject.GetComponent<RectTransform>().sizeDelta.y / 2 - 1.5f;

                        amountTextObject.transform.localPosition = new Vector3(aX, aY, -1);
                        amountTextObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    }
                }

                ItemImage itemImageScript = itemImage.AddComponent<ItemImage>();

                itemImageScript.cellType = 1;
                itemImageScript.x = int.Parse(x);
                itemImageScript.y = int.Parse(y);
                itemImageScript.isVertical = isVertical;
                itemImageScript.itemCode = int.Parse(item);
                itemImageScript.amount = int.Parse(amount);
                itemImageScript.thisContainer = container;

                int PLCamount = 0;
                for(int j = 0; j < PLCcode.Length; j++)
                {
                    if(PLCcode[j] == 'p')
                    {
                        PLCamount++;
                    }
                }
                itemImageScript.PLCcode = new int[PLCamount];

                int PLCindex = 0;
                for(int j = 1; j < PLCcode.Length; j++)
                {
                    string nowPLC = "";
                    while(j < PLCcode.Length && PLCcode[j] != 'p')
                    {
                        nowPLC += PLCcode[j];
                        j++;
                    }
                    itemImageScript.PLCcode[PLCindex] = int.Parse(nowPLC);
                    PLCindex++;
                }


                itemImage.transform.SetParent(cellGroupObject.transform);
                itemImage.GetComponent<Image>().sprite = itemsprite;
                Vector2 xy = GetItemPosition(LPos, int.Parse(x), int.Parse(y));
                if(isVertical)
                {
                    xy.x += itemsprite.bounds.size.x / 2;
                    xy.y -= itemsprite.bounds.size.y / 2;
                }
                else
                {
                    itemImage.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                    xy.x += itemsprite.bounds.size.y / 2;
                    xy.y -= itemsprite.bounds.size.x / 2;
                }
                itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(itemsprite.bounds.size.x, itemsprite.bounds.size.y);
                itemImage.transform.localPosition = new Vector3(xy.x, xy.y, -2);

            }



        }

        return cellGroupObject;
    }


    public static GameObject CreateCellGroup(GameObject parent, Container container, Vector2 LPos, bool isCreateExit ,GameObject[] cellPrefabs)
    {
        int xSize = container.xSize;
        int ySize = container.ySize;

        //Create CellGroup
        GameObject cellGroupObject = GameObject.Instantiate(cellPrefabs[0]);
        Destroy(cellGroupObject.GetComponent<Image>());
        cellGroupObject.transform.SetParent(parent.transform);
        cellGroupObject.transform.localPosition = new Vector3(0, 0, 0);
        CellGroup cellGroup  = cellGroupObject.AddComponent<CellGroup>();
        cellGroup.container = container;
        cellGroup.xLocalPos = LPos.x;
        cellGroup.yLocalPos = LPos.y;
        cellGroup.Exit = isCreateExit;
        
   

        void CreateConerLine(int prefabs,Vector2 PlusPos,float angle)
        {
                GameObject nowObject = Instantiate(cellPrefabs[prefabs]);
                nowObject.transform.SetParent(cellGroupObject.transform);
                nowObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                nowObject.transform.localPosition = new Vector3(LPos.x + PlusPos.x, LPos.y + PlusPos.y, -1);
        }

        CreateConerLine(1, new Vector2(xConerSize/2, -yConerSize/2), 0);
        CreateConerLine(1, new Vector2(xConerSize/2 + (cellSize-1)*xSize + 1 +xConerSize, -yConerSize / 2), -90);
        CreateConerLine(1, new Vector2(xConerSize/2 + (cellSize-1)*xSize+ 1 +xConerSize, -yConerSize / 2 - (cellSize-1)*ySize - 1 - yConerSize), -180);
        CreateConerLine(1, new Vector2(xConerSize/2, -yConerSize/2 - (cellSize-1) * ySize - 1 - yConerSize), -270);

        if(isCreateExit)
        {
            GameObject Exit = Instantiate(cellPrefabs[3]);
            Exit.transform.SetParent(cellGroupObject.transform);
            Sprite ExitSprite = Exit.GetComponent<Image>().sprite;
            Exit.transform.localPosition = new Vector3(LPos.x + xConerSize / 2 + (cellSize - 1) * xSize + (float)2.5 + xConerSize - 6, LPos.y + -yConerSize / 2 + (float)1.5 + 5, -1);

            Exit.GetComponent<Button>().onClick.AddListener
                (
                delegate
                {
                    if(GameManager.PauseAmount == 0)
                    Destroy(cellGroupObject);
                }
                );

        }
        
        for(int i = 0; i <xSize;i++)
        {
            CreateConerLine(2, new Vector2(cellSize / 2 + xConerSize + i * (cellSize-1), -yConerSize / 2), 0);
            CreateConerLine(2, new Vector2(cellSize / 2 + xConerSize + i * (cellSize-1), -yConerSize / 2 - ySize * (cellSize-1) - 1 - yConerSize), -180);
        }
        for(int i = 0; i<ySize;i++)
        {
            CreateConerLine(2, new Vector2(xConerSize / 2 + xSize * (cellSize-1) + 1 + xConerSize, -cellSize / 2 - yConerSize - i * (cellSize-1)), -90);
            CreateConerLine(2, new Vector2(xConerSize / 2, -cellSize / 2 - yConerSize - i * (cellSize-1)), -270);
        }


        void CreateCell(int x, int y)
        {
            GameObject nowCellObject = Instantiate(cellPrefabs[0]);
            nowCellObject.transform.SetParent(cellGroupObject.transform);
            nowCellObject.AddComponent<ItemCell>().cellGroup = cellGroupObject;
            SetCellPosition(nowCellObject, LPos, x, y);
        }


        for (int ix = 0; ix < xSize; ix++)
        {
            for (int iy = 0; iy < ySize; iy++)
            {
                CreateCell(ix, iy);
            }
        }





        return cellGroupObject;


    }

    public static void RenderingBackpackContainer(ItemImage itemimage) // InContainer Use
    {
        ItemManager itemManager = GameManager.itemManager;
        Backpack backpackItem = (Backpack)Item.GetItem(itemimage.itemCode);
        Container backpackContainer = new Container(itemimage.PLCcode[0], itemimage.thisContainer.isNeedSave, backpackItem.xSpace, backpackItem.ySpace);
        GameObject cellGroup = RenderContainer(itemimage.transform.parent.gameObject, backpackContainer, new Vector2(itemimage.transform.localPosition.x-backpackItem.itemSprite.bounds.size.x/2 +1, itemimage.transform.localPosition.y + backpackItem.itemSprite.bounds.size.y/2 - 1), true,itemManager.cellPrefabs, itemManager.amountFont);

        GameObject SpriteObject = GameObject.Instantiate(itemManager.cellPrefabs[0]);
        SpriteObject.transform.SetParent(cellGroup.transform);
        SpriteObject.GetComponent<Image>().sprite = Item.GetItem(itemimage.itemCode).itemSprite;
        SpriteObject.GetComponent<RectTransform>().sizeDelta = new Vector2(SpriteObject.GetComponent<Image>().sprite.bounds.size.x, SpriteObject.GetComponent<Image>().sprite.bounds.size.y);
        SpriteObject.transform.localPosition = new Vector3(cellGroup.GetComponent<CellGroup>().xLocalPos + SpriteObject.GetComponent<Image>().sprite.bounds.size.x/2 -1, cellGroup.GetComponent<CellGroup>().yLocalPos - SpriteObject.GetComponent<Image>().sprite.bounds.size.y/2 + 1, 0);
        if (itemimage.isVertical == false)
            SpriteObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
        SpriteObject.transform.SetAsFirstSibling();
        cellGroup.GetComponent<CellGroup>().spriteObject = SpriteObject;

        if (!itemimage.isVertical)
        {
            Vector2 move = new Vector2(0, 0);

            Vector2 basePos = GameManager.itemManager.mainCamera.WorldToScreenPoint(itemimage.transform.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.itemManager.canvas, basePos , GameManager.itemManager.mainCamera,out basePos);

            if (basePos.x - backpackItem.itemSprite.bounds.size.x/2 - 1 < -GameManager.itemManager.canvas.sizeDelta.x / 2)
            {
                move.x = -GameManager.itemManager.canvas.sizeDelta.x / 2 + backpackItem.itemSprite.bounds.size.x / 2 + 1;
                move.x -= basePos.x;
            }
            else if(basePos.x + backpackItem.itemSprite.bounds.size.x / 2 + 1 > GameManager.itemManager.canvas.sizeDelta.x / 2)
            {
                move.x = GameManager.itemManager.canvas.sizeDelta.x / 2 - backpackItem.itemSprite.bounds.size.x / 2 - 1;
                move.x -= basePos.x;
            }

            if(basePos.y - backpackItem.itemSprite.bounds.size.y /2 - 1 < -GameManager.itemManager.canvas.sizeDelta.y / 2)
            {
                move.y = -GameManager.itemManager.canvas.sizeDelta.y / 2 + backpackItem.itemSprite.bounds.size.y / 2 + 1;
                move.y -= basePos.y;
            }
            else if(basePos.y + backpackItem.itemSprite.bounds.size.y / 2 + 11 > GameManager.itemManager.canvas.sizeDelta.y / 2)
            {
                move.y = GameManager.itemManager.canvas.sizeDelta.y / 2 - backpackItem.itemSprite.bounds.size.y / 2 - 11;
                move.y -= basePos.y;
            }

            cellGroup.transform.localPosition += new Vector3(move.x,move.y, 0);
            cellGroup.GetComponent<CellGroup>().xLocalPos += move.x;
            cellGroup.GetComponent<CellGroup>().yLocalPos += move.y;
            SpriteObject.transform.localPosition -= new Vector3(move.x, move.y, 0);

        }


    }

    public static GameObject RenderingBackpackContainer(GameObject parent,int itemCode,int PLCcode,bool isNeedSave ,Vector2 Pos)// playerGear Use
    {
        ItemManager itemManager = GameManager.itemManager;
        Backpack backpackItem = (Backpack)Item.GetItem(itemCode);
        Container backpackContainer = new Container(PLCcode,isNeedSave, backpackItem.xSpace, backpackItem.ySpace);
        return RenderContainer(parent, backpackContainer, Pos, false, itemManager.cellPrefabs, itemManager.amountFont);
    }

    public static void RenderingChestRigContainer(ItemImage itemimage) // InContainer Use
    {
        ItemManager itemManager = GameManager.itemManager;
        ChestRig chestRigItem = (ChestRig)Item.GetItem(itemimage.itemCode);

        GameObject containerGroupObject = GameObject.Instantiate(itemManager.cellPrefabs[0]);
        GameObject.Destroy(containerGroupObject.GetComponent<Image>());
        containerGroupObject.transform.SetParent(itemimage.transform.parent.transform);
        containerGroupObject.transform.localPosition = itemimage.transform.localPosition;
        containerGroupObject.AddComponent<CellGroup>();

        void CreateGroupConerLine(int prefabs, Vector2 PlusPos, float angle)
        {
            GameObject nowObject = Instantiate(itemManager.cellPrefabs[prefabs]);
            nowObject.transform.SetParent(containerGroupObject.transform);
            nowObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            nowObject.transform.localPosition = new Vector3(PlusPos.x, PlusPos.y, -1);
        }

        
            CreateGroupConerLine(1, new Vector2(-chestRigItem.itemSprite.bounds.size.x / 2 - CellGroup.xConerSize / 2, chestRigItem.itemSprite.bounds.size.y / 2 + CellGroup.yConerSize / 2), 0);
            CreateGroupConerLine(1, new Vector2(chestRigItem.itemSprite.bounds.size.x / 2 + CellGroup.xConerSize / 2, chestRigItem.itemSprite.bounds.size.y / 2 + CellGroup.yConerSize / 2), -90);
            CreateGroupConerLine(1, new Vector2(chestRigItem.itemSprite.bounds.size.x / 2 + CellGroup.xConerSize / 2, -chestRigItem.itemSprite.bounds.size.y / 2 - CellGroup.yConerSize / 2), -180);
            CreateGroupConerLine(1, new Vector2(-chestRigItem.itemSprite.bounds.size.x / 2 - CellGroup.xConerSize / 2, -chestRigItem.itemSprite.bounds.size.y / 2 - CellGroup.yConerSize / 2), -270);

            for (int j = 0; j < chestRigItem.xsize; j++)
            {
                CreateGroupConerLine(2, new Vector2(-(((float)chestRigItem.xsize - 1) / 2 * CellGroup.cellSize) + (((float)chestRigItem.xsize - 1) / 2) + (j * CellGroup.cellSize) - j, chestRigItem.itemSprite.bounds.size.y / 2 + CellGroup.yConerSize / 2), 0);
                CreateGroupConerLine(2, new Vector2(-(((float)chestRigItem.xsize - 1) / 2 * CellGroup.cellSize) + (((float)chestRigItem.xsize - 1) / 2) + (j * CellGroup.cellSize) - j, -chestRigItem.itemSprite.bounds.size.y / 2 - CellGroup.yConerSize / 2), -180);
            }

            for (int j = 0; j < chestRigItem.ysize; j++)
            {
                CreateGroupConerLine(2, new Vector2(chestRigItem.itemSprite.bounds.size.x / 2 + CellGroup.xConerSize / 2, (((float)chestRigItem.ysize - 1) / 2 * CellGroup.cellSize) - (((float)chestRigItem.ysize - 1) / 2) - (j * CellGroup.cellSize) + j), -90);
                CreateGroupConerLine(2, new Vector2(-chestRigItem.itemSprite.bounds.size.x / 2 - CellGroup.xConerSize / 2, (((float)chestRigItem.ysize - 1) / 2 * CellGroup.cellSize) - (((float)chestRigItem.ysize - 1) / 2) - (j * CellGroup.cellSize) + j), -270);
            }

        

        GameObject SpriteObject = GameObject.Instantiate(itemManager.cellPrefabs[0]);
        SpriteObject.transform.SetParent(containerGroupObject.transform);
        SpriteObject.transform.localPosition = new Vector3(0, 0, 0);
        SpriteObject.GetComponent<Image>().sprite = Item.GetItem(itemimage.itemCode).itemSprite;
        SpriteObject.GetComponent<RectTransform>().sizeDelta = new Vector2(SpriteObject.GetComponent<Image>().sprite.bounds.size.x, SpriteObject.GetComponent<Image>().sprite.bounds.size.y);
        if (itemimage.isVertical == false)
        {
            SpriteObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
            SpriteObject.transform.SetAsFirstSibling();

            GameObject childSpriteObject = GameObject.Instantiate(itemManager.cellPrefabs[0]);
            childSpriteObject.transform.SetParent(containerGroupObject.transform);
            childSpriteObject.transform.localPosition = new Vector3(0, 0, 0);
            childSpriteObject.GetComponent<Image>().sprite = Item.GetItem(itemimage.itemCode).itemSprite;
            childSpriteObject.GetComponent<RectTransform>().sizeDelta = new Vector2(childSpriteObject.GetComponent<Image>().sprite.bounds.size.x, childSpriteObject.GetComponent<Image>().sprite.bounds.size.y);

            Vector2 move = new Vector2(0, 0);

            Vector2 basePos = GameManager.itemManager.mainCamera.WorldToScreenPoint(itemimage.transform.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.itemManager.canvas, basePos, GameManager.itemManager.mainCamera, out basePos);

            if (basePos.x - chestRigItem.itemSprite.bounds.size.x / 2 - 1 < -GameManager.itemManager.canvas.sizeDelta.x / 2)
            {
                move.x = -GameManager.itemManager.canvas.sizeDelta.x / 2 + chestRigItem.itemSprite.bounds.size.x / 2 + 1;
                move.x -= basePos.x;
            }
            else if (basePos.x + chestRigItem.itemSprite.bounds.size.x / 2 + 1 > GameManager.itemManager.canvas.sizeDelta.x / 2)
            {
                move.x = GameManager.itemManager.canvas.sizeDelta.x / 2 - chestRigItem.itemSprite.bounds.size.x / 2 - 1;
                move.x -= basePos.x;
            }

            if (basePos.y - chestRigItem.itemSprite.bounds.size.y / 2 - 1 < -GameManager.itemManager.canvas.sizeDelta.y / 2)
            {
                move.y = -GameManager.itemManager.canvas.sizeDelta.y / 2 + chestRigItem.itemSprite.bounds.size.y / 2 + 1;
                move.y -= basePos.y;
            }
            else if (basePos.y + chestRigItem.itemSprite.bounds.size.y / 2 + 11 > GameManager.itemManager.canvas.sizeDelta.y / 2)
            {
                move.y = GameManager.itemManager.canvas.sizeDelta.y / 2 - chestRigItem.itemSprite.bounds.size.y / 2 - 11;
                move.y -= basePos.y;
            }

            containerGroupObject.transform.localPosition += new Vector3(move.x, move.y, 0);
            SpriteObject.transform.localPosition -= new Vector3(move.x, move.y, 0);

        }

        for (int i = 0; i < chestRigItem.rigCells.Length;i++)
        {
            Container RigCellContainer = new Container(itemimage.PLCcode[i], itemimage.thisContainer.isNeedSave, chestRigItem.rigCells[i].xsize, chestRigItem.rigCells[i].ysize);
            Vector2 ContainerPos = chestRigItem.rigCells[i].postion + new Vector2(-(chestRigItem.rigCells[i].xsize * 19 + 1) / (float)2 + 1, (chestRigItem.rigCells[i].ysize * 19 + 1) / (float)2 - 1);
            RenderContainer(containerGroupObject, RigCellContainer, ContainerPos, false, itemManager.cellPrefabs, itemManager.amountFont);
        }

        GameObject Exit = Instantiate(itemManager.cellPrefabs[3]);
        Exit.transform.SetParent(containerGroupObject.transform);
        Exit.transform.localPosition = new Vector3(SpriteObject.GetComponent<Image>().sprite.bounds.size.x/2 - 5 , SpriteObject.GetComponent<Image>().sprite.bounds.size.y /2  +  6);

        Exit.GetComponent<Button>().onClick.AddListener
            (
            delegate
            {
                if (GameManager.PauseAmount == 0)
                    Destroy(containerGroupObject);
            }
            );



    }


    public static void RenderingChestRigContainer(GameObject parent, int itemCode, int[] PLCcode, bool isNeedSave, Vector2 Pos) // playerGear Use
    {
        ItemManager itemManager = GameManager.itemManager;
        ChestRig chestRigItem = (ChestRig)Item.GetItem(itemCode);

        GameObject containerGroupObject = GameObject.Instantiate(itemManager.cellPrefabs[0]);
        containerGroupObject.GetComponent<Image>().sprite = Item.GetItem(itemCode).itemSprite;
        containerGroupObject.GetComponent<Image>().material = itemManager.NoLineItemMaterial;
        containerGroupObject.GetComponent<RectTransform>().sizeDelta = new Vector2(containerGroupObject.GetComponent<Image>().sprite.bounds.size.x, containerGroupObject.GetComponent<Image>().sprite.bounds.size.y);
        containerGroupObject.transform.SetParent(parent.transform);
        containerGroupObject.transform.localPosition = Pos;
        containerGroupObject.AddComponent<CellGroup>();

        for (int i = 0; i < chestRigItem.rigCells.Length; i++)
        {
            Container RigCellContainer = new Container(PLCcode[i], isNeedSave, chestRigItem.rigCells[i].xsize, chestRigItem.rigCells[i].ysize);
            Vector2 ContainerPos = chestRigItem.rigCells[i].postion + new Vector2(-(chestRigItem.rigCells[i].xsize * 19 + 1) / (float)2 + 1, (chestRigItem.rigCells[i].ysize * 19 + 1) / (float)2 - 1);
            RenderContainer(containerGroupObject, RigCellContainer, ContainerPos, false, itemManager.cellPrefabs, itemManager.amountFont).GetComponent<CellGroup>().isGearChestRig = true;
        }

    }



    private static void SetCellPosition(GameObject cellObject,Vector2 LPos ,int x, int y)
    {
        ItemCell nowCell = cellObject.GetComponent<ItemCell>();
        nowCell.cellX = x;
        nowCell.cellY = y;
        LPos.x += ( (float)x +  (float)0.5) * cellSize + xConerSize -x;
        LPos.y -= ( (float)y +  (float)0.5 ) * cellSize + yConerSize - y;
        cellObject.transform.localPosition = new Vector3(LPos.x,LPos.y,-1);
        cellObject.name = "x" + x.ToString() + "y" + y.ToString();

    }

    public static Vector2 GetItemPosition(Vector2 LPos, int x, int y)
    {
        LPos.x += x  * (cellSize-1) + xConerSize;
        LPos.y -= y  * (cellSize-1) + yConerSize;

        return LPos;
    }
}
