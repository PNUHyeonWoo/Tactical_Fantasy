using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantUI : MonoBehaviour
{
    public Shop shop;
    public MerchantButton merchantButton;
    public GameObject shopUI;
    public GameObject QuestUI;
    public GameObject ShopCell;
    public GameObject QuestPage;
    public GameObject QuestTitleButton;
    public Slider AmountSlider;
    public Text SliderText;
    public Text PriceText;
    public Text NameText;
    public ShopItem selectItem = null;

    public const int ShopCellXSize = 28;
    public const int ShopCellYSize = 22;

    public void Start()
    {
        RenderShop();
    }

    public void Exit()
    {
        if (GameManager.PauseAmount > 0)
            return;

        if (GameManager.itemManager.NowStartType != 0)
        {
            GameManager.itemManager.FailMoveItem();
        }

        if (GameManager.player.itemMenu != null)
        {
            Destroy(GameManager.player.itemMenu);
            GameManager.player.itemMenu = null;
        }

        for(int i = 0; i < shop.merchantButtons.Length;i++)
        {
            shop.merchantButtons[i].Start();
        }

        Destroy(this.gameObject);
    }

    public void ShopButtonClick()
    {
        if (GameManager.PauseAmount > 0)
            return;


        if (!shopUI.activeSelf && QuestUI.activeSelf)
        {
            shopUI.SetActive(true);
            RenderShop();
            QuestUI.SetActive(false);
            Destroy(QuestUI.GetComponentInChildren<QuestPage>().gameObject);
        }

    }

    public void QuestButtonClick()
    {
        if (GameManager.PauseAmount > 0)
            return;

        if (shopUI.activeSelf && !QuestUI.activeSelf)
        {
            shopUI.SetActive(false);
            foreach(Transform child in ShopCell.transform.GetComponentInChildren<Transform>())
            {
                Destroy(child.gameObject);
            }
            selectItem = null;
            AmountSlider.maxValue = 1;
            AmountSlider.value = 1;
            SliderText.text = "0";
            PriceText.text = "0G";
            NameText.text = "";
            QuestUI.SetActive(true);
            RenderQuest(-1);
        }
    }

    public void RenderShop()
    {
        int intimacy = 0;
        StringManager.QuestString[] quests = GameManager.stringManager.questStrings[merchantButton.index];

        for (int i = 0; i < quests.Length; i++)
        {
            if (GameManager.saveManager.GetIsQuestClear(GetQuestIndex(i)))
                intimacy += quests[i].intimacy;
        }

        int level = 0;
        for(int i = 0; i<merchantButton.intimacys.Length; i++)
        {
            if (intimacy >= merchantButton.intimacys[i])
                level++;
        }

        bool[,] boolMap = new bool[ShopCellYSize,ShopCellXSize];
        int itemIndex = 0;

        for (int j = 0; j < level+1; j++)
        {
            
            while (itemIndex < merchantButton.ShopItems.Length && merchantButton.ShopItems[itemIndex] != -1)
            {               
                Item nowItem = Item.GetItem(merchantButton.ShopItems[itemIndex]);

                bool InsertItem = false;

                for(int y = 0;y < ShopCellYSize;y++)
                {
                    for(int x = 0; x< ShopCellXSize;x++)
                    {
                        if(!boolMap[y,x])
                        {
                            bool canInsert =true;
                            for(int ckY = y;ckY < y + nowItem.ysize;ckY++)
                            {
                                for(int ckX = x; ckX < x + nowItem.xsize; ckX++)
                                {
                                    if(!(ckY < ShopCellYSize && ckX < ShopCellXSize && !boolMap[ckY, ckX]))
                                        canInsert = false;
                                }
                            }

                            if(canInsert)
                            {
                                RenderShopItem(merchantButton.ShopItems[itemIndex], nowItem, x, y);
                                for (int iY = y; iY < y + nowItem.ysize; iY++)
                                {
                                    for (int iX = x; iX < x + nowItem.xsize; iX++)
                                    {
                                        boolMap[iY, iX] = true;
                                    }
                                }


                                InsertItem = true;
                                break;
                            }



                        }
                    }
                    if (InsertItem)
                        break;
                }
                itemIndex++;
            }
            itemIndex++;
        }


    }

    public void RenderShopItem(int itemCode,Item item,int x, int y)
    {
        GameObject shopitem = GameObject.Instantiate(shop.ShopItemPrefab);
        shopitem.transform.SetParent(ShopCell.transform);
        Vector2 pos = new Vector2(-ShopCell.GetComponent<RectTransform>().sizeDelta.x/2, ShopCell.GetComponent<RectTransform>().sizeDelta.y/2);
        pos = CellGroup.GetItemPosition(pos, x, y) + new Vector2(2,-2);
        shopitem.transform.localPosition = new Vector3(pos.x, pos.y, 0);

        shopitem.GetComponent<Image>().sprite = item.itemSprite;
        shopitem.GetComponent<RectTransform>().sizeDelta = new Vector2(item.itemSprite.bounds.size.x, item.itemSprite.bounds.size.y);

        shopitem.GetComponent<ShopItem>().merchantUI = this;
        shopitem.GetComponent<ShopItem>().itemCode = itemCode;
        shopitem.GetComponent<ShopItem>().item = item;
    }

    public void PurchaseItem()
    {
        if (GameManager.PauseAmount > 0)
            return;

        if (selectItem != null)
        {
            if((int)(selectItem.item.value * AmountSlider.value) <= GameManager.saveManager.money)
            {
                Container chestContainer = shop.chestCellGroup.GetComponent<CellGroup>().container;
                if (chestContainer.CanInsertItem(selectItem.itemCode))
                {
                    GameManager.saveManager.money -= (int)(selectItem.item.value * AmountSlider.value);
                    shop.money.text = GameManager.saveManager.money.ToString();

                    if(!Item.CheckPLC(selectItem.itemCode))
                    {
                        chestContainer.InsertItem(selectItem.itemCode, new int[1], (int)AmountSlider.value);
                    }
                    else
                    {
                        int itemType = selectItem.itemCode / 10000;

                        if(itemType == 205 || itemType == 301)
                        {
                            chestContainer.InsertItem(selectItem.itemCode,new int[1] { GameManager.saveManager.savePLC.CreatePlusContainer("") }, 1);
                        }
                        else if(itemType == 201)
                        {
                            int attachment = ((Gun)(selectItem.item)).attachmentCells.Length;
                            string str = "";

                            if(((Gun)(selectItem.item)).gunAnimation == Gun.GunAnimation.Boltaction || ((Gun)(selectItem.item)).gunAnimation == Gun.GunAnimation.PumpShotgun || ((Gun)(selectItem.item)).gunAnimation == Gun.GunAnimation.DoubleBarrel)
                            {
                                for (int i = 0; i < attachment - 1; i++)
                                {
                                    str += "0000000";
                                }

                                str += ((Gun)(selectItem.item)).constMagazineCode.ToString();

                                str += "p" +GameManager.saveManager.savePLC.CreatePlusContainer("").ToString();

                            }
                            else
                            {
                                for (int i = 0; i < attachment; i++)
                                {
                                    str += "0000000";
                                }
                            }

                            chestContainer.InsertItem(selectItem.itemCode, new int[1] { GameManager.saveManager.savePLC.CreatePlusContainer(str) }, 1);
                        }
                        else if(itemType == 204)
                        {
                            int rigcell = ((ChestRig)(selectItem.item)).rigCells.Length;
                            int[] PLC = new int[rigcell];

                            for(int i = 0; i < rigcell; i++)
                            {
                                PLC[i] = GameManager.saveManager.savePLC.CreatePlusContainer("");
                            }

                            chestContainer.InsertItem(selectItem.itemCode,PLC, 1);

                        }




                    }
                    GameManager.itemManager.ContainerReRender(shop.chestCellGroup.GetComponent<CellGroup>());


                }




            }
        }

    }

    public void SelectItem(ShopItem shopitem)
    {
        if (GameManager.PauseAmount > 0)
            return;

        selectItem = shopitem;
        AmountSlider.maxValue = shopitem.item.overlapLimit;
        if (AmountSlider.maxValue == 0)
            AmountSlider.maxValue = 1;
        AmountSlider.value = 1;
        SliderText.text = AmountSlider.value.ToString();
        PriceText.text = (selectItem.item.value * AmountSlider.value).ToString() + "G";
        NameText.text = GameManager.stringManager.GetItemString(shopitem.itemCode).name;
    }

    public void SliderChange()
    {
        SliderText.text = AmountSlider.value.ToString();
        if (selectItem != null)
            PriceText.text = (selectItem.item.value * AmountSlider.value).ToString() + "G";
    }

    public void InspectButtonClick()
    {
        if (GameManager.PauseAmount > 0)
            return;

        if (selectItem != null)
        {
            GameObject inspectUI = GameObject.Instantiate(GameManager.itemManager.itemMenuPrefabs[2]);
            inspectUI.transform.SetParent(GameManager.itemManager.canvas);
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.itemManager.canvas, Input.mousePosition,GameManager.itemManager.mainCamera, out mousePos);

            Vector3 SpriteSize = inspectUI.GetComponent<Image>().sprite.bounds.size;



            if (mousePos.x - SpriteSize.x / 2 < -GameManager.itemManager.canvas.sizeDelta.x / 2)
                mousePos.x = -GameManager.itemManager.canvas.sizeDelta.x / 2 + SpriteSize.x / 2;
            if (mousePos.x + SpriteSize.x / 2 > GameManager.itemManager.canvas.sizeDelta.x / 2)
                mousePos.x = GameManager.itemManager.canvas.sizeDelta.x / 2 - SpriteSize.x / 2;
            if (mousePos.y - SpriteSize.y < -GameManager.itemManager.canvas.sizeDelta.y / 2)
                mousePos.y = -GameManager.itemManager.canvas.sizeDelta.y / 2 + SpriteSize.y;


            inspectUI.transform.localPosition = new Vector3(mousePos.x, mousePos.y, 0);

            inspectUI.transform.Find("Name").GetComponent<Text>().text = GameManager.stringManager.GetItemString(selectItem.itemCode).name;
            inspectUI.transform.Find("Inspect").GetComponent<Text>().text = GameManager.stringManager.GetItemString(selectItem.itemCode).inspect;
        }

    }


    public void RenderQuest(int selectIndex)
    {
        GameObject questPage = GameObject.Instantiate(QuestPage);
        questPage.transform.SetParent(QuestUI.transform);
        questPage.transform.localPosition = Vector3.zero;
        QuestPage page = questPage.GetComponent<QuestPage>();

        int intimacy = 0;
        StringManager.QuestString[] quests = GameManager.stringManager.questStrings[merchantButton.index];

        for (int i = 0; i < quests.Length;i++)
        {
            if (GameManager.saveManager.GetIsQuestClear(GetQuestIndex(i)))
                intimacy += quests[i].intimacy;
        }

        int level = 0;
        for (int i = 0; i < merchantButton.intimacys.Length; i++)
        {
            if (intimacy >= merchantButton.intimacys[i])
                level++;
        }

        if(level == merchantButton.intimacys.Length)//MAX Level
        {
            page.levelText.text = "LV.MAX";
            page.maxText.text = "MAX";
            page.nowText.text = "MAX";
            page.intimacyInBar.sizeDelta = page.intimacyInBar.transform.parent.GetComponent<RectTransform>().sizeDelta - new Vector2(6,6);
        }
        else
        {
            page.levelText.text = "LV." + level.ToString();
            page.maxText.text = merchantButton.intimacys[level].ToString();

            int nowintimacy = intimacy;
            for(int i = 0; i < level; i++)
            {
                nowintimacy -= merchantButton.intimacys[i];
            }
            page.nowText.text = nowintimacy.ToString();

            page.intimacyInBar.sizeDelta = new Vector2(page.intimacyInBar.transform.parent.GetComponent<RectTransform>().sizeDelta.x * nowintimacy / merchantButton.intimacys[level], page.intimacyInBar.transform.parent.GetComponent<RectTransform>().sizeDelta.y -6);
        }

        float totalButtonYpos = 0;

        for (int i = 0; i < quests.Length; i++)
        {
            if (!GameManager.saveManager.GetIsQuestClear(GetQuestIndex(i)) && (quests[i].needQuest == -1 || GameManager.saveManager.GetIsQuestClear(quests[i].needQuest)) && quests[i].needIntimacy <= intimacy)
            {
                GameObject titleButton = GameObject.Instantiate(QuestTitleButton);
                titleButton.transform.SetParent(page.buttonParent);
                titleButton.transform.localPosition = new Vector3(0, totalButtonYpos, 0);
                totalButtonYpos -= titleButton.GetComponent<RectTransform>().sizeDelta.y;

                titleButton.transform.Find("Text").GetComponent<Text>().text = quests[i].title;
                QuestTitleButton buttonscr = titleButton.GetComponent<QuestTitleButton>();
                buttonscr.merchantUI = this;
                buttonscr.questPage = questPage;
                buttonscr.index = i;
            }
                
        }

        if(selectIndex != -1)
        {
            page.titleText.text = quests[selectIndex].title;
            page.contentsText.text = quests[selectIndex].contents;
            page.moneyText.text = quests[selectIndex].Money.ToString();
            page.intimacyText.text = quests[selectIndex].intimacy.ToString();

            BitArray questBit = GameManager.saveManager.GetQuestBitArray(GetQuestIndex(selectIndex));

            for (int i = 0; i < quests[selectIndex].requestItems.Length;i++)
            {
                if (quests[selectIndex].requestAmounts[i] == 1)
                    page.requestTexts[i].text = GameManager.stringManager.GetItemString(quests[selectIndex].requestItems[i]).name;
                else
                    page.requestTexts[i].text = GameManager.stringManager.GetItemString(quests[selectIndex].requestItems[i]).name + "(" + quests[selectIndex].requestAmounts[i].ToString() + ")";

                              
                if (questBit[i])
                    page.requestChecks[i].sprite = page.checkBox;               

            }

            QuestCell questCell = page.questCell.AddComponent<QuestCell>();

            questCell.shop = shop;
            questCell.merchantUI = this;
            questCell.questPage = questPage;
            questCell.localIndex = selectIndex;

        }

    }

    public void SubmitItem(int questIndex ,ItemImage itemImage , GameObject questPage)
    {
        StringManager.QuestString quest = GameManager.stringManager.questStrings[merchantButton.index][questIndex];

        for(int i = 0; i < quest.requestItems.Length;i++)
        {
            if(itemImage.itemCode == quest.requestItems[i] && itemImage.amount >= quest.requestAmounts[i] && !GameManager.saveManager.GetQuestBitArray(GetQuestIndex(questIndex))[i])
            {
                if (itemImage.amount == quest.requestAmounts[i])
                    itemImage.thisContainer.RemoveItem(itemImage.x, itemImage.y);
                else
                    itemImage.thisContainer.RemoveItem(quest.requestAmounts[i], itemImage.x, itemImage.y);

                BitArray bitArray = GameManager.saveManager.GetQuestBitArray(GetQuestIndex(questIndex));
                bitArray[i] = true;

                bool isClearQuest = true;

                for(int j = 0; j < quest.requestItems.Length; j++)
                {
                    if(!bitArray[j])
                    {
                        isClearQuest = false;
                        break;
                    }

                }

                if (isClearQuest)
                {
                    bitArray[7] = true;
                    GameManager.saveManager.money += quest.Money;
                    shop.money.text = GameManager.saveManager.money.ToString();
                }


                GameManager.saveManager.SetQuestBitArray(GetQuestIndex(questIndex), bitArray);

                if (isClearQuest)
                    RenderQuest(-1);
                else
                    RenderQuest(questIndex);

                Destroy(questPage);


                GameManager.itemManager.ContainerReRender(itemImage.transform.parent.GetComponent<CellGroup>());
                break;
            }



        }


    }

    public int GetQuestIndex(int index)
    {
        return merchantButton.index * 50 + index;
    }

}
