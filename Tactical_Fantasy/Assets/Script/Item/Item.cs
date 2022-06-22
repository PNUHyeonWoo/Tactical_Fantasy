using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string name;
    public int ItemCode = 0;
    public int xsize = 1;
    public int ysize = 1;
    public Sprite itemSprite;
    public int overlapLimit = 1;
    public int value;
    public const int MainIndex = 1000000;
    public const int MiddleIndex = 10000;

    public enum MainCatecory
    {
        Loot = 1,
        Gear = 2,
        Attachment = 3,
        Ammo = 4,
        Consumable = 5
    }

    public static Item GetItem(int code)
    {
        int mCode = code % MainIndex;

        switch (code / MainIndex)
        {
            case 1: //Loot

               if (mCode / MiddleIndex == 1) //NorLoot
                {
                    return GameManager.itemData.transform.Find("LootData").GetComponent<DataOfLoot>().norLoots[mCode % MiddleIndex];

                }
                else                  //ImpLoot
                {
                    return GameManager.itemData.transform.Find("LootData").GetComponent<DataOfLoot>().impLoots[mCode % MiddleIndex];
                }
                

            case 2: //Gear
                switch (mCode / MiddleIndex)
                {
                    
                    case 1: //Gun
                    return GameManager.itemData.transform.Find("GearData").GetComponent<DataOfGear>().guns[mCode % MiddleIndex];
                    case 2: //Helmet
                    return GameManager.itemData.transform.Find("GearData").GetComponent<DataOfGear>().helmets[mCode % MiddleIndex];
                    case 3: //Uniform
                    return GameManager.itemData.transform.Find("GearData").GetComponent<DataOfGear>().uniforms[mCode % MiddleIndex];
                    case 4: //ChestRig
                    return GameManager.itemData.transform.Find("GearData").GetComponent<DataOfGear>().chestRigs[mCode % MiddleIndex];
                    case 5: //Backpack
                    return GameManager.itemData.transform.Find("GearData").GetComponent<DataOfGear>().backpacks[mCode % MiddleIndex];
                    case 6: //goggles
                    return GameManager.itemData.transform.Find("GearData").GetComponent<DataOfGear>().goggles[mCode % MiddleIndex];
                }
                break;

            case 3: //Attachment
                switch (mCode / MiddleIndex)
                {
                    case 1: //Magazine
                        return GameManager.itemData.transform.Find("AttachmentData").GetComponent<DataOfAttachment>().magazines[mCode % MiddleIndex];
                    case 2: //Handle
                        return GameManager.itemData.transform.Find("AttachmentData").GetComponent<DataOfAttachment>().handles[mCode % MiddleIndex];
                    case 3: //Flash
                        return GameManager.itemData.transform.Find("AttachmentData").GetComponent<DataOfAttachment>().flashes[mCode % MiddleIndex];
                    case 4: //Scope
                        return GameManager.itemData.transform.Find("AttachmentData").GetComponent<DataOfAttachment>().scopes[mCode % MiddleIndex];
                    case 5: //Reverse
                        return GameManager.itemData.transform.Find("AttachmentData").GetComponent<DataOfAttachment>().reverses[mCode % MiddleIndex];
                    case 6: //Spray
                        return GameManager.itemData.transform.Find("AttachmentData").GetComponent<DataOfAttachment>().sprays[mCode % MiddleIndex];                       
                }
                break;

            case 4: //Ammo
                return GameManager.itemData.transform.Find("AmmoData").GetComponent<DataOfAmmo>().ammos[mCode % MiddleIndex];              

            case 5: //Consumable
                return GameManager.itemData.transform.Find("ConsumableData").GetComponent<DataOfConsumable>().consumables[mCode % MiddleIndex];

        }
        return new Item();
    }

    public static bool CheckPLC(int code)
    {
        int mCode = code % MainIndex;

        if (code / MainIndex == 2)
        {
            if (mCode / MiddleIndex == 1 ||               
               mCode / MiddleIndex == 4 ||
               mCode / MiddleIndex == 5)
                return true;
        }

        if (code / MainIndex == 3 && mCode / MiddleIndex == 1)
            return true;

        return false;

    }

}
