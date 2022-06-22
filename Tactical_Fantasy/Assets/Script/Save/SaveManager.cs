using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public PlusContainerManager savePLC = new PlusContainerManager();
    public PlayerGear playerGear = new PlayerGear();
    public sbyte[] questFlag = new sbyte[2000];
    public sbyte[] eventFlag = new sbyte[2000];
    public sbyte[] chestFlag = new sbyte[2000];
    public int money = 0;

    public void StartManager()
    {        
        playerGear.player = GameManager.player;
        playerGear.Canvas = GameObject.Find("Canvas");
        playerGear.itemManager = GameManager.itemManager;
        playerGear.savePLCManager = savePLC;

        Load(SettingManager.slot);
    }

    public void ReStartManager()
    {
        playerGear.player = GameManager.player;
        playerGear.Canvas = GameObject.Find("Canvas");
    }


    public void Save(int slot)
    {
        savePLC.Save(slot);
        playerGear.Save(slot);

        if (!Directory.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString()))
        {
            Directory.CreateDirectory(Application.dataPath + "/save" + "/slot" + slot.ToString());
        }

        FileStream questWriter = File.Create(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/quest.tps");
        FileStream eventWriter = File.Create(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/event.tps");
        FileStream chestWriter = File.Create(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/chest.tps");
        FileStream etcWriter = File.Create(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/etc.tps");

        for (int i = 0; i<2000;i++)
        {
            questWriter.WriteByte((byte)questFlag[i]);
            eventWriter.WriteByte((byte)eventFlag[i]);
            chestWriter.WriteByte((byte)chestFlag[i]);
        }

        byte[] moneyByte = System.BitConverter.GetBytes(money);

        for (int i = 0; i < 4; i++)
            etcWriter.WriteByte(moneyByte[i]);

        questWriter.Close();
        eventWriter.Close();
        chestWriter.Close();
        etcWriter.Close();
    }
    public void Load(int slot)
    {
        savePLC.Load(slot);
        playerGear.Load(slot);

        if (File.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/quest.tps") && File.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/event.tps") && File.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/chest.tps") && File.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/etc.tps"))
        {
            byte[] questReader = File.ReadAllBytes(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/quest.tps");
            byte[] eventReader = File.ReadAllBytes(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/event.tps");
            byte[] chestReader = File.ReadAllBytes(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/chest.tps");
            byte[] etcReader = File.ReadAllBytes(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/etc.tps");

            for (int i = 0; i < 2000; i++)
            {
                questFlag[i] = (sbyte)questReader[i];
                eventFlag[i] = (sbyte)eventReader[i];
                chestFlag[i] = (sbyte)chestReader[i];
            }

             money = System.BitConverter.ToInt32(etcReader, 0);

        }
        else
            Debug.LogError("no SaveFile");


    }

    public static void CreateNewSlot(int slot)
    {
        if (!Directory.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString()))
        {
            Directory.CreateDirectory(Application.dataPath + "/save" + "/slot" + slot.ToString());
        }

        StreamWriter writer = File.CreateText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/PLC.tps");
        StreamWriter ava = File.CreateText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/ava.tps");

        writer.Write("0,,1,,2,,3,,4,,5,,6,,7,,8,,9,,");
        ava.Write("1111111111");
       
        writer.Close();
        ava.Close();

        StreamWriter gear = File.CreateText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/gear.tps");
        StreamWriter gearPLC = File.CreateText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/gearPLC.tps");

        gear.Write("0,0,0,0,0,0,0,0,");
        gearPLC.Write("n,n,n,n,n,");

        gear.Close();
        gearPLC.Close();

        FileStream questWriter = File.Create(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/quest.tps");
        FileStream eventWriter = File.Create(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/event.tps");
        FileStream chestWriter = File.Create(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/chest.tps");
        FileStream etcWriter = File.Create(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/etc.tps");

        for (int i = 0; i < 2000; i++)
        {
            questWriter.WriteByte(0);
            eventWriter.WriteByte(0);
            chestWriter.WriteByte(0);
        }

        byte[] moneyByte = System.BitConverter.GetBytes((int)0);

        for (int i = 0; i < 4; i++)
            etcWriter.WriteByte(moneyByte[i]);


        questWriter.Close();
        eventWriter.Close();
        chestWriter.Close();
        etcWriter.Close();

    }

    public BitArray GetQuestBitArray(int index)
    {
        return new BitArray(new byte[1] { (byte)questFlag[index] });
    }

    public void SetQuestBitArray(int index , BitArray setArray)
    {
        setArray.CopyTo(questFlag, index);
    }

    public bool GetIsQuestClear(int index)
    {
        return GetQuestBitArray(index)[7];
    }

    //bitarray.copy(sbyte배열,0)으로 비트어레이 sbyte에 집어넣기 가능

}
