using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interaction
{
    public Container thisContainer = null;
    public GameObject cellGroupObject = null;
    public bool isRemoveIfNoItem = false;
    public int xSize = 5;
    public int ySize = 4;

    public enum ChestType
    {
        None = 0,
        Random = 1,
        OneTime = 2
    }

    public ChestType chestType = ChestType.None;
    public int chestIndex;
    public int[] Items;
    public float[] itemPercents;

    public Sprite close;
    public Sprite open;

    public AudioClip closeSound;
    public AudioClip openSound;
    void Start()
    {
        if(chestType == ChestType.Random)
        {
            thisContainer = new Container(false, xSize, ySize, "");

            for(int i = 0; i < Items.Length; i++)
            {
                if(-0.01f < itemPercents[i] % 1.0f && itemPercents[i] % 1.0f < 0.01f)
                {
                    if((int)itemPercents[i] > 0)
                    thisContainer.InsertItem(Items[i], new int[] { }, (int)itemPercents[i]);
                }
                else
                {
                    int count = (int)Mathf.Ceil(itemPercents[i]);
                    float percent = itemPercents[i] % 1.0f;                  
                    int resultCount = 0;
                    for(int j = 0; j < count; j++)
                    {
                        float random = Random.Range(0f, 1f);
                        if (random <= percent)
                            resultCount++;
                    }

                    if(resultCount > 0)
                        thisContainer.InsertItem(Items[i], new int[] { }, resultCount);


                }
            }


        }
        else if(chestType == ChestType.OneTime)
        {
            thisContainer = new Container(false, xSize, ySize, "");

            if(GameManager.saveManager.chestFlag[chestIndex] == 0)
            {
                thisContainer.InsertItem(Items[0], new int[] { }, 1);
            }


        }

    }


    public void RenderChest()
    {
        if(!thisContainer.isNeedSave)
            cellGroupObject = CellGroup.RenderContainer(GameManager.itemManager.canvas.gameObject, thisContainer, new Vector2(GameManager.itemManager.canvas.sizeDelta.x/2 - 202, -GameManager.itemManager.canvas.sizeDelta.y / 2 + 274), false, GameManager.itemManager.cellPrefabs, GameManager.itemManager.amountFont);
        else
            cellGroupObject = CellGroup.RenderContainer(GameManager.itemManager.canvas.gameObject, thisContainer, new Vector2(GameManager.itemManager.canvas.sizeDelta.x / 2 - 326, -GameManager.itemManager.canvas.sizeDelta.y / 2 + 274), false, GameManager.itemManager.cellPrefabs, GameManager.itemManager.amountFont);

        cellGroupObject.GetComponent<CellGroup>().chest = this;
        GetComponent<SpriteRenderer>().sprite = open;
        GameManager.cameraManager.PlayOutterSound(openSound);

        if (GameManager.player.itemMenu != null)
        {
            Destroy(GameManager.player.itemMenu);
            GameManager.player.itemMenu = null;
        }
       
    }

    public void DestroyChestCellGroup()
    {
        if(cellGroupObject != null)
        {
            if(GameManager.itemManager.NowStartType != 0)
            {
                GameManager.itemManager.FailMoveItem();
            }
            Destroy(cellGroupObject);
            cellGroupObject = null;
            GetComponent<SpriteRenderer>().sprite = close;
            GameManager.cameraManager.PlayOutterSound(closeSound);

            if (chestType == ChestType.OneTime)
            {
                if (!thisContainer.GetContents().Contains(Items[0].ToString() + "a"))
                    GameManager.saveManager.chestFlag[chestIndex] = 1;
            }


            if(isRemoveIfNoItem)
            {
                if(thisContainer.GetContents() == "")
                {
                    thisContainer.RemoveThisPLC();
                    Destroy(this.gameObject);
                }


            }


            if (GameManager.player.itemMenu != null)
            {
                Destroy(GameManager.player.itemMenu);
                GameManager.player.itemMenu = null;
            }

        }
    }

}
