using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class StringManager : MonoBehaviour
{
    public struct ItemString
    {
        public string name;
        public string inspect;

        public ItemString(string name, string inspect)
        {
            this.name = name;
            this.inspect = inspect;
        }

    }

    public struct QuestString
    {
        public string title;
        public string contents;
        public int needQuest;
        public int needIntimacy;
        public bool isUnsealQuest;
        public int Money;
        public int intimacy;
        public int[] requestItems;
        public int[] requestAmounts;
    }

    [System.Serializable]
    public struct DialogString
    {
        public Color color;
        public string[] dialogContents;
        public bool isAnswer;
        public string[] answers;
    }

    public ItemString[][][] itemStrings;

    public QuestString[][] questStrings;

    public DialogString[] dialogStrings;

    public string[] merchantSystem;





    public void StartManager()
    {
        itemStrings = new ItemString[5][][];

        itemStrings[0] = new ItemString[2][];
        itemStrings[0][0] = GetStringList("Item/Loot/NorLoot.csv");
        itemStrings[0][1] = GetStringList("Item/Loot/ImpLoot.csv");

        itemStrings[1] = new ItemString[6][];
        itemStrings[1][0] = GetStringList("Item/Gear/Gun.csv");
        itemStrings[1][1] = GetStringList("Item/Gear/Helmet.csv");
        itemStrings[1][2] = GetStringList("Item/Gear/Uniform.csv");
        itemStrings[1][3] = GetStringList("Item/Gear/ChestRig.csv");
        itemStrings[1][4] = GetStringList("Item/Gear/Backpack.csv");
        itemStrings[1][5] = GetStringList("Item/Gear/Goggles.csv");

        itemStrings[2] = new ItemString[6][];
        itemStrings[2][0] = GetStringList("Item/Attachment/Magazine.csv");
        itemStrings[2][1] = GetStringList("Item/Attachment/Handle.csv");
        itemStrings[2][2] = GetStringList("Item/Attachment/Flash.csv");
        itemStrings[2][3] = GetStringList("Item/Attachment/Scope.csv");
        itemStrings[2][4] = GetStringList("Item/Attachment/Reverse.csv");
        itemStrings[2][5] = GetStringList("Item/Attachment/Spray.csv");

        itemStrings[3] = new ItemString[1][];
        itemStrings[3][0] = GetStringList("Item/Ammo/Ammo.csv");

        itemStrings[4] = new ItemString[1][];
        itemStrings[4][0] = GetStringList("Item/Consumable/Consumable.csv");

        questStrings = GetQuestList("Merchant/Quest");

        dialogStrings = GetDialogList();

        merchantSystem = GetSystemList("Merchant/System.csv");
    }

    public void RestartManager()
    {
        dialogStrings = GetDialogList();
    }

    public static ItemString[] GetStringList(string route)
    {
        ItemString[] result;       

        string[] lawString = File.ReadAllText(Application.dataPath + "/CSV/" + SettingManager.language+"/" + route).Split('\n');

        result = new ItemString[lawString.Length - 2];


        for (int i = 1; i <  lawString.Length - 1;i++)
        {
            string[] nowString = lawString[i].Split(',');
            result[i - 1] = new ItemString(nowString[1], nowString[2]);
        }


        return result;

    }

    public static QuestString[][] GetQuestList(string route)//quest폴더까지
    {
        QuestString[][] result = new QuestString[20][];

        for (int i = 0; i < 20; i++)
        {
            string[] lawString = File.ReadAllText(Application.dataPath + "/CSV/" + SettingManager.language + "/" + route + "/Merchant" + i.ToString() + ".csv").Split('\n');

            result[i] = new QuestString[lawString.Length - 2];

            for(int j = 1; j < lawString.Length - 1; j++)
            {
                string[] nowString = lawString[j].Split(',');
                QuestString nowQuestString = new QuestString();
                nowQuestString.title = nowString[1];
                nowQuestString.contents = nowString[2];
                nowQuestString.needQuest = int.Parse(nowString[3]);
                nowQuestString.needIntimacy = int.Parse(nowString[4]);
                nowQuestString.isUnsealQuest = (nowString[5] == "1");
                nowQuestString.Money = int.Parse(nowString[6]);
                nowQuestString.intimacy = int.Parse(nowString[7]);

                int allRequestAmount = 7;
                for(int m = 8; m < 22; m = m + 2)
                {
                    if (nowString[m] == "")
                    {
                        allRequestAmount = (m - 8) / 2;
                        break;
                    }
                }

                nowQuestString.requestItems = new int[allRequestAmount];
                nowQuestString.requestAmounts = new int[nowQuestString.requestItems.Length];
                for(int k = 0; k < nowQuestString.requestItems.Length; k++)
                {
                    nowQuestString.requestItems[k] = int.Parse(nowString[8 + (2 * k)]);
                    nowQuestString.requestAmounts[k] = int.Parse(nowString[9 + (2 * k)]);
                }
                result[i][j - 1] = nowQuestString;
            }
        }

        return result;
    }
    
    public static string[] GetSystemList(string route)
    {
        string[] result;

        string[] lawString = File.ReadAllText(Application.dataPath + "/CSV/" + SettingManager.language + "/" + route).Split('\n');

        result = new string[lawString.Length - 2];

        for (int i = 1; i < lawString.Length - 1; i++)
        {
            string[] nowString = lawString[i].Split(',');
            result[i - 1] = nowString[2];
        }

        return result;
    }

    public static DialogString[] GetDialogList()
    {
        string[] lawString = File.ReadAllText(Application.dataPath + "/CSV/" + SettingManager.language + "/Conversation/" + SceneManager.GetActiveScene().name +".csv").Split('\n');

        DialogString[] result = new DialogString[lawString.Length - 2];

        for(int i = 1; i < lawString.Length - 1; i++)
        {
            string[] nowString = lawString[i].Split(',');

            ColorUtility.TryParseHtmlString(nowString[1], out result[i - 1].color);
            result[i - 1].isAnswer = nowString[2] != "0";

            if (result[i - 1].isAnswer)
            {
                int answerAmount = int.Parse(nowString[2]);
                result[i - 1].answers = new string[answerAmount];
            }

            List<string> stringlist = new List<string>();


            for(int j = 3; j < nowString.Length; j++)
            {
                if (nowString[j] == "" || nowString[j] == "\r")
                    break;

                if(nowString[j] != "/a")
                {
                    stringlist.Add(nowString[j]);
                }
                else
                {
                    int index = 0;
                    for(int k = j + 1; k < j + 1 + result[i - 1].answers.Length; k++)
                    {
                        result[i - 1].answers[index] = nowString[k];
                        index++;
                    }
                    break;
                }

            }

            result[i - 1].dialogContents = stringlist.ToArray();


        }

        return result;

    }
    
    public ItemString GetItemString(int itemCode)
    {
        if(itemCode/ 1000000 == 4)
            return itemStrings[itemCode / 1000000 - 1][0][itemCode % 10000];

        if (itemCode / 1000000 == 5)
            return itemStrings[itemCode / 1000000 - 1][0][itemCode % 10000];

        return itemStrings[itemCode / 1000000 - 1][(itemCode % 1000000) / 10000 - 1][itemCode % 10000];
    }

}
