using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttachmentCellGroup : MonoBehaviour
{
    public AttachmentContainer attachmentContainer;
    public float xLocalPos;
    public float yLocalPos;
    public bool isGearGun = false;
    public GameObject spriteObject = null;



    public static GameObject RenderingCellGroup(GameObject parent, AttachmentContainer attachmentContainer, Vector2 LPos,bool isGearGun ,GameObject[] cellPrefabs)
    {
        GameObject cellGroupObject = GameObject.Instantiate(cellPrefabs[0]);
        cellGroupObject.GetComponent<Image>().sprite = Item.GetItem(attachmentContainer.itemCode).itemSprite;
        cellGroupObject.GetComponent<RectTransform>().sizeDelta = new Vector2(cellGroupObject.GetComponent<Image>().sprite.bounds.size.x, cellGroupObject.GetComponent<Image>().sprite.bounds.size.y);
        cellGroupObject.transform.SetParent(parent.transform);
        cellGroupObject.transform.localPosition = new Vector3(LPos.x + cellGroupObject.GetComponent<RectTransform>().sizeDelta.x/2, LPos.y-cellGroupObject.GetComponent<RectTransform>().sizeDelta.y / 2, -1);
        AttachmentCellGroup cellGroup = cellGroupObject.AddComponent<AttachmentCellGroup>();
        cellGroup.attachmentContainer = attachmentContainer;
        cellGroup.xLocalPos = LPos.x;
        cellGroup.yLocalPos = LPos.y;
        cellGroup.isGearGun = isGearGun;

        void CreateGroupConerLine(int prefabs, Vector2 PlusPos, float angle)
        {
            GameObject nowObject = Instantiate(cellPrefabs[prefabs]);
            nowObject.transform.SetParent(cellGroupObject.transform);
            nowObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            nowObject.transform.localPosition = new Vector3(PlusPos.x, PlusPos.y, -1);
        }

        if (!isGearGun)
        {
            CreateGroupConerLine(1, new Vector2(-cellGroupObject.GetComponent<RectTransform>().sizeDelta.x / 2 - CellGroup.xConerSize / 2, cellGroupObject.GetComponent<RectTransform>().sizeDelta.y / 2 + CellGroup.yConerSize / 2), 0);
            CreateGroupConerLine(1, new Vector2(cellGroupObject.GetComponent<RectTransform>().sizeDelta.x / 2 + CellGroup.xConerSize / 2, cellGroupObject.GetComponent<RectTransform>().sizeDelta.y / 2 + CellGroup.yConerSize / 2), -90);
            CreateGroupConerLine(1, new Vector2(cellGroupObject.GetComponent<RectTransform>().sizeDelta.x / 2 + CellGroup.xConerSize / 2, -cellGroupObject.GetComponent<RectTransform>().sizeDelta.y / 2 - CellGroup.yConerSize / 2), -180);
            CreateGroupConerLine(1, new Vector2(-cellGroupObject.GetComponent<RectTransform>().sizeDelta.x / 2 - CellGroup.xConerSize / 2, -cellGroupObject.GetComponent<RectTransform>().sizeDelta.y / 2 - CellGroup.yConerSize / 2), -270);

            for (int j = 0; j < Item.GetItem(attachmentContainer.itemCode).xsize; j++)
            {
                CreateGroupConerLine(2, new Vector2(-(((float)Item.GetItem(attachmentContainer.itemCode).xsize - 1) / 2 * CellGroup.cellSize) +(((float)Item.GetItem(attachmentContainer.itemCode).xsize - 1)/2) + (j * CellGroup.cellSize) - j, cellGroupObject.GetComponent<RectTransform>().sizeDelta.y / 2 + CellGroup.yConerSize / 2), 0);
                CreateGroupConerLine(2, new Vector2(-(((float)Item.GetItem(attachmentContainer.itemCode).xsize - 1) / 2 * CellGroup.cellSize) + (((float)Item.GetItem(attachmentContainer.itemCode).xsize - 1) / 2) + (j * CellGroup.cellSize) - j, -cellGroupObject.GetComponent<RectTransform>().sizeDelta.y / 2 - CellGroup.yConerSize / 2), -180);
            }

            for (int j = 0; j < Item.GetItem(attachmentContainer.itemCode).ysize; j++)
            {
                CreateGroupConerLine(2, new Vector2(cellGroupObject.GetComponent<RectTransform>().sizeDelta.x / 2 + CellGroup.xConerSize / 2, (((float)Item.GetItem(attachmentContainer.itemCode).ysize - 1) / 2 * CellGroup.cellSize)- (((float)Item.GetItem(attachmentContainer.itemCode).ysize - 1)/2) - (j * CellGroup.cellSize) + j), -90);
                CreateGroupConerLine(2, new Vector2(-cellGroupObject.GetComponent<RectTransform>().sizeDelta.x / 2 - CellGroup.xConerSize / 2, (((float)Item.GetItem(attachmentContainer.itemCode).ysize - 1) / 2 * CellGroup.cellSize) - (((float)Item.GetItem(attachmentContainer.itemCode).ysize - 1) / 2) - (j * CellGroup.cellSize) + j), -270);
            }

            GameObject childImage = GameObject.Instantiate(cellPrefabs[0]);
            childImage.GetComponent<Image>().sprite = Item.GetItem(attachmentContainer.itemCode).itemSprite;
            childImage.GetComponent<RectTransform>().sizeDelta = new Vector2(cellGroupObject.GetComponent<Image>().sprite.bounds.size.x, cellGroupObject.GetComponent<Image>().sprite.bounds.size.y);
            childImage.transform.SetParent(cellGroupObject.transform);
            childImage.transform.localPosition = new Vector3(0, 0, -1);
        }


        string contents = attachmentContainer.GetContents();

       

        for (int i= 0;i< attachmentContainer.attachmentCells.Length; i++)
        {
           
            if (int.Parse(contents[i * 7].ToString()) == 0)
            {
                AttachmentCell nowAC = attachmentContainer.attachmentCells[i];
                GameObject nowCellObject = Instantiate(cellPrefabs[0]);
                nowCellObject.transform.SetParent(cellGroupObject.transform);
                nowCellObject.AddComponent<AttachmentItemCell>().index = i;
                nowCellObject.transform.localPosition = new Vector3(nowAC.cellPosition.x + CellGroup.cellSize / 2, nowAC.cellPosition.y - CellGroup.cellSize / 2, -1);

                void CreateConerLine(int prefabs, Vector2 PlusPos, float angle)
                {
                    GameObject nowObject = Instantiate(cellPrefabs[prefabs]);
                    nowObject.transform.SetParent(cellGroupObject.transform);
                    nowObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    nowObject.transform.localPosition = new Vector3(nowCellObject.transform.localPosition.x + PlusPos.x, nowCellObject.transform.localPosition.y + PlusPos.y, -1);
                }

                CreateConerLine(1, new Vector2(-CellGroup.cellSize / 2 - CellGroup.xConerSize / 2, CellGroup.cellSize / 2 + CellGroup.yConerSize / 2), 0);
                CreateConerLine(1, new Vector2(CellGroup.cellSize / 2 + CellGroup.xConerSize / 2, CellGroup.cellSize / 2 + CellGroup.yConerSize / 2), -90);
                CreateConerLine(1, new Vector2(CellGroup.cellSize / 2 + CellGroup.xConerSize / 2, -CellGroup.cellSize / 2 - CellGroup.yConerSize / 2), -180);
                CreateConerLine(1, new Vector2(-CellGroup.cellSize / 2 - CellGroup.xConerSize / 2, -CellGroup.cellSize / 2 - CellGroup.yConerSize / 2), -270);
                CreateConerLine(2, new Vector2(0, CellGroup.cellSize / 2 + CellGroup.yConerSize / 2), 0);
                CreateConerLine(2, new Vector2(CellGroup.cellSize / 2 + CellGroup.xConerSize / 2, 0), -90);
                CreateConerLine(2, new Vector2(0, -CellGroup.cellSize / 2 - CellGroup.yConerSize / 2), -180);
                CreateConerLine(2, new Vector2(-CellGroup.cellSize / 2 - CellGroup.xConerSize / 2, 0), -270);

                nowCellObject.transform.SetAsLastSibling();
            }
            else
            {
                string itemCodeStr = "";
                for(int j= 0;j < 7; j++)
                {
                    itemCodeStr += contents[(i * 7) + j];
                }
                int itemCode = int.Parse(itemCodeStr);

                AttachmentCell nowAC = attachmentContainer.attachmentCells[i];
                GameObject nowItemObject = Instantiate(cellPrefabs[0]);
                nowItemObject.transform.SetParent(cellGroupObject.transform);
                nowItemObject.GetComponent<Image>().sprite = Item.GetItem(itemCode).itemSprite;
                nowItemObject.GetComponent<RectTransform>().sizeDelta = new Vector2(nowItemObject.GetComponent<Image>().sprite.bounds.size.x, nowItemObject.GetComponent<Image>().sprite.bounds.size.y);

                ItemImage itemImage = nowItemObject.AddComponent<ItemImage>();
                itemImage.cellType = 2;
                itemImage.index = i;
                itemImage.attachmentContainer = attachmentContainer;
                itemImage.itemCode = itemCode;
                itemImage.amount = 1;
                itemImage.isVertical = true;

                if (itemCode / 10000 == 301)
                {
                    int j = (attachmentContainer.attachmentCells.Length * 7) + 1;
                    string PLCcodeStr = "";
                    while (j < contents.Length)
                    {
                        PLCcodeStr += contents[j];
                        j++;
                    }
                    itemImage.PLCcode = new int[1] { int.Parse(PLCcodeStr) };

                    int magazineType = (int)nowAC.kindOfMagazine;
                    if (magazineType == 11 || magazineType == 12)
                    {
                        itemImage.cantSelectThisItem = true;
                    }

                }

                nowItemObject.transform.localPosition= new Vector3(nowAC.cellPosition.x + nowItemObject.GetComponent<RectTransform>().sizeDelta.x / 2, nowAC.cellPosition.y - nowItemObject.GetComponent<RectTransform>().sizeDelta.y / 2, -1);

                void CreateConerLine(int prefabs, Vector2 PlusPos, float angle)
                {
                    GameObject nowObject = Instantiate(cellPrefabs[prefabs]);
                    nowObject.transform.SetParent(cellGroupObject.transform);
                    nowObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    nowObject.transform.localPosition = new Vector3(nowItemObject.transform.localPosition.x + PlusPos.x, nowItemObject.transform.localPosition.y + PlusPos.y, -1);
                }

                CreateConerLine(1, new Vector2(-nowItemObject.GetComponent<RectTransform>().sizeDelta.x / 2 - CellGroup.xConerSize/2, nowItemObject.GetComponent<RectTransform>().sizeDelta.y / 2 + CellGroup.yConerSize / 2), 0);
                CreateConerLine(1, new Vector2(nowItemObject.GetComponent<RectTransform>().sizeDelta.x / 2 + CellGroup.xConerSize / 2, nowItemObject.GetComponent<RectTransform>().sizeDelta.y / 2 + CellGroup.yConerSize / 2), -90);
                CreateConerLine(1, new Vector2(nowItemObject.GetComponent<RectTransform>().sizeDelta.x / 2 + CellGroup.xConerSize / 2, -nowItemObject.GetComponent<RectTransform>().sizeDelta.y / 2 - CellGroup.yConerSize / 2), -180);
                CreateConerLine(1, new Vector2(-nowItemObject.GetComponent<RectTransform>().sizeDelta.x / 2 - CellGroup.xConerSize / 2, -nowItemObject.GetComponent<RectTransform>().sizeDelta.y / 2 - CellGroup.yConerSize / 2), -270);

                for(int j = 0; j < Item.GetItem(itemCode).xsize;j++)
                {
                    CreateConerLine(2, new Vector2(-(((float)Item.GetItem(itemCode).xsize-1)/ 2 * CellGroup.cellSize) + (j * CellGroup.cellSize) +(((float)Item.GetItem(itemCode).xsize - 1)/2) - j, nowItemObject.GetComponent<RectTransform>().sizeDelta.y / 2 + CellGroup.yConerSize / 2), 0);
                    CreateConerLine(2, new Vector2(-(((float)Item.GetItem(itemCode).xsize - 1) / 2 * CellGroup.cellSize) + (j * CellGroup.cellSize) + (((float)Item.GetItem(itemCode).xsize - 1) / 2) - j, -nowItemObject.GetComponent<RectTransform>().sizeDelta.y / 2 - CellGroup.yConerSize / 2), -180);
                }

                for (int j = 0; j < Item.GetItem(itemCode).ysize; j++)
                {
                    CreateConerLine(2, new Vector2(nowItemObject.GetComponent<RectTransform>().sizeDelta.x / 2 + CellGroup.xConerSize / 2, (((float)Item.GetItem(itemCode).ysize - 1) / 2 * CellGroup.cellSize) - (j * CellGroup.cellSize) - (((float)Item.GetItem(itemCode).ysize - 1) / 2) + j), -90);
                    CreateConerLine(2, new Vector2(-nowItemObject.GetComponent<RectTransform>().sizeDelta.x / 2 - CellGroup.xConerSize / 2, (((float)Item.GetItem(itemCode).ysize - 1) / 2 * CellGroup.cellSize) - (j * CellGroup.cellSize) - (((float)Item.GetItem(itemCode).ysize - 1) / 2) + j), -270);
                }

                nowItemObject.transform.SetAsLastSibling();

            }

        }

        return cellGroupObject;



    }

    public static void RenderingCellGroup(ItemImage itemimage)
    {
        ItemManager itemManager = GameManager.itemManager;
        Gun gun = (Gun)Item.GetItem(itemimage.itemCode);
        AttachmentContainer gunContainer = new AttachmentContainer(itemimage.PLCcode[0], itemimage.thisContainer.isNeedSave, itemimage.itemCode, gun.attachmentCells);
        GameObject cellGroup = AttachmentCellGroup.RenderingCellGroup(itemimage.transform.parent.gameObject, gunContainer, new Vector2(itemimage.transform.localPosition.x - gun.itemSprite.bounds.size.x/2, itemimage.transform.localPosition.y + gun.itemSprite.bounds.size.y / 2), false,itemManager.cellPrefabs);
        cellGroup.AddComponent<CellGroup>();


        GameObject Exit = Instantiate(itemManager.cellPrefabs[3]);
        Exit.transform.SetParent(cellGroup.transform);
        Exit.transform.localPosition = new Vector3(gun.itemSprite.bounds.size.x / 2 - 5, gun.itemSprite.bounds.size.y / 2 + 6);

        Exit.GetComponent<Button>().onClick.AddListener
            (
            delegate
            {
                if (GameManager.PauseAmount == 0)
                    Destroy(cellGroup);
            }
            );

        if(itemimage.isVertical == false)
        {
            GameObject SpriteObject = GameObject.Instantiate(itemManager.cellPrefabs[0]);
            SpriteObject.transform.SetParent(cellGroup.transform);
            SpriteObject.transform.localPosition = new Vector3(0, 0, 0);
            SpriteObject.GetComponent<Image>().sprite = gun.itemSprite;
            SpriteObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gun.itemSprite.bounds.size.x, gun.itemSprite.bounds.size.y);
            SpriteObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
            SpriteObject.transform.SetAsFirstSibling();
            cellGroup.GetComponent<AttachmentCellGroup>().spriteObject = SpriteObject;

            Vector2 move = new Vector2(0, 0);

            Vector2 basePos = GameManager.itemManager.mainCamera.WorldToScreenPoint(itemimage.transform.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.itemManager.canvas, basePos, GameManager.itemManager.mainCamera, out basePos);

            if (basePos.x - gun.itemSprite.bounds.size.x / 2 - 1 < -GameManager.itemManager.canvas.sizeDelta.x / 2)
            {
                move.x = -GameManager.itemManager.canvas.sizeDelta.x / 2 + gun.itemSprite.bounds.size.x / 2 + 1;
                move.x -= basePos.x;
            }
            else if (basePos.x + gun.itemSprite.bounds.size.x / 2 + 1 > GameManager.itemManager.canvas.sizeDelta.x / 2)
            {
                move.x = GameManager.itemManager.canvas.sizeDelta.x / 2 - gun.itemSprite.bounds.size.x / 2 - 1;
                move.x -= basePos.x;
            }

            if (basePos.y - gun.itemSprite.bounds.size.y / 2 - 1 < -GameManager.itemManager.canvas.sizeDelta.y / 2)
            {
                move.y = -GameManager.itemManager.canvas.sizeDelta.y / 2 + gun.itemSprite.bounds.size.y / 2 + 1;
                move.y -= basePos.y;
            }
            else if (basePos.y + gun.itemSprite.bounds.size.y / 2 + 11 > GameManager.itemManager.canvas.sizeDelta.y / 2)
            {
                move.y = GameManager.itemManager.canvas.sizeDelta.y / 2 - gun.itemSprite.bounds.size.y / 2 - 11;
                move.y -= basePos.y;
            }

            cellGroup.transform.localPosition += new Vector3(move.x, move.y, 0);
            cellGroup.GetComponent<AttachmentCellGroup>().xLocalPos += move.x;
            cellGroup.GetComponent<AttachmentCellGroup>().yLocalPos += move.y;
            SpriteObject.transform.localPosition -= new Vector3(move.x, move.y, 0);

        }




    }

}
