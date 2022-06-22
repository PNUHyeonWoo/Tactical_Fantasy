using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Shop : MonoBehaviour
{
    public int selectChest = 1;
    public GameObject chestCellGroup = null;
    public GameObject chestParent;
    public Text money;
    public object destinationCell = null;

    public GameObject merchantPrefab;
    public GameObject SellBoxPrefab;
    public GameObject ShopItemPrefab;

    public MerchantButton[] merchantButtons;
   
    public void Start()
    {
        SelectChest(0);
        money.text = GameManager.saveManager.money.ToString();
    }



    public void SelectChest(int i)
    {
        if (GameManager.PauseAmount > 0)
            return;

        if (selectChest == i)
            return;

        selectChest = i;

        if (chestCellGroup != null)
            Destroy(chestCellGroup);

        if (GameManager.itemManager.NowStartType != 0)
        {
            GameManager.itemManager.FailMoveItem();
        }

        if (GameManager.player.itemMenu != null)
        {
            Destroy(GameManager.player.itemMenu);
            GameManager.player.itemMenu = null;
        }

        chestCellGroup = CellGroup.RenderContainer(chestParent, new Container(i, true, 17, 14), new Vector2(-161, 132.5f), false, GameManager.itemManager.cellPrefabs, GameManager.itemManager.amountFont);
        chestCellGroup.GetComponent<CellGroup>().shop = this;
    }

    public void RenderMerchant(MerchantButton merchant)
    {
        GameObject merchantObject = GameObject.Instantiate(merchantPrefab);
        merchantObject.transform.SetParent(merchant.transform.parent);
        merchantObject.transform.localPosition = Vector3.zero;
        merchantObject.GetComponent<MerchantUI>().shop = this;
        merchantObject.GetComponent<MerchantUI>().merchantButton = merchant;
    }

    public void CreateSellBox(ItemImage itemimage)
    {
        GameManager.GamePause();
        Item item = Item.GetItem(itemimage.itemCode);

        GameObject sellBox = GameObject.Instantiate(SellBoxPrefab);
        sellBox.transform.SetParent(GameManager.itemManager.canvas);
        sellBox.transform.localPosition = new Vector3(0, 0, 0);

        Slider slider = sellBox.transform.Find("Slider").GetComponent<Slider>();
        Text text = sellBox.transform.Find("Text").GetComponent<Text>();
        Text priceText = sellBox.transform.Find("PriceText").GetComponent<Text>();
        slider.minValue = 1;
        slider.maxValue = itemimage.amount;
        text.text = GameManager.stringManager.merchantSystem[2].Replace("<n>", GameManager.stringManager.GetItemString(itemimage.itemCode).name).Replace("<a>",slider.value.ToString());
        priceText.text = (slider.value * item.value/2).ToString() + "G";

        slider.onValueChanged.AddListener((f) => {
            text.text = GameManager.stringManager.merchantSystem[2].Replace("<n>", GameManager.stringManager.GetItemString(itemimage.itemCode).name).Replace("<a>", f.ToString());
            priceText.text = (f * item.value/2).ToString() + "G";
        });

        sellBox.transform.Find("Sell").GetComponent<Button>().onClick.AddListener(
            delegate
            {
                if (slider.value >= itemimage.amount)
                {
                    itemimage.thisContainer.RemoveItem(itemimage.x, itemimage.y);
                }
                else if(slider.value < itemimage.amount)
                {
                    itemimage.thisContainer.RemoveItem((int)slider.value,itemimage.x, itemimage.y);
                }

                GameManager.itemManager.ContainerReRender(itemimage.transform.parent.GetComponent<CellGroup>());

                GameManager.saveManager.money += (int)slider.value * item.value/2;
                money.text = GameManager.saveManager.money.ToString();


                Destroy(sellBox);
                GameManager.GamePlay();
            }
            );

        sellBox.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(
            delegate
            {
                Destroy(sellBox);
                GameManager.GamePlay();
            }
            );

    }


    public void Exit()
    {
        if (GameManager.PauseAmount > 0)
            return;

        GameManager.player.isInShop = false;
        GameManager.player.shop = null;

        if (GameManager.itemManager.NowStartType != 0)
        {
            GameManager.itemManager.FailMoveItem();
        }

        if (GameManager.player.itemMenu != null)
        {
            Destroy(GameManager.player.itemMenu);
            GameManager.player.itemMenu = null;
        }

        Destroy(this.gameObject);
    }
}
