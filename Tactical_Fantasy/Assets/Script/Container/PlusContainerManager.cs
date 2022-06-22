using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlusContainerManager 
{
    public string availability;
    public Dictionary<int,string> plusContainers; 

    public PlusContainerManager()
    {
        availability = "0";
        plusContainers = new Dictionary<int, string>();
    }

    public int CreatePlusContainer(string contents)
    {
        for (int i = 0; i < availability.Length; i++)
        {
            if (availability[i] == '1')
                continue;
            plusContainers.Add(i, contents);
            char[] avachar = availability.ToCharArray();
            avachar[i] = '1';
            availability = new string(avachar);
            return i;
        }
        plusContainers.Add(availability.Length, contents);
        availability += "1";
        return availability.Length - 1; 
    }  
    

    public void RemovePlusContainer(int index)
    {
        plusContainers.Remove(index);
        char[] avachar = availability.ToCharArray();
        avachar[index] = '0';
        availability = new string(avachar);
    }


    public void Save(int slot)
    {       

        if (!Directory.Exists(Application.dataPath +"/save" +"/slot" + slot.ToString()))
        {
            Directory.CreateDirectory(Application.dataPath + "/save" +"/slot" + slot.ToString());
        }

        StreamWriter writer = File.CreateText(Application.dataPath + "/save" +"/slot" +slot.ToString()+ "/PLC.tps");
        StreamWriter ava = File.CreateText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/ava.tps");

        for (int i = 0; i < availability.Length; i++)
        {
            if (availability[i] == '0')
                continue;

            writer.Write(i.ToString() + "," + plusContainers[i] + ",");
        }
        ava.Write(availability);


        writer.Close();
        ava.Close();
    }

    public void Load(int slot)
    {   

        if (File.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/PLC.tps") && File.Exists(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/ava.tps"))
        {
            string plc = File.ReadAllText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/PLC.tps");
            string ava = File.ReadAllText(Application.dataPath + "/save" + "/slot" + slot.ToString() + "/ava.tps");

            string[] plcs = plc.Split(',');

            for (int i = 0; i < plcs.Length - 1; i = i + 2 )
            {
                plusContainers.Add(int.Parse(plcs[i]), plcs[i + 1]);
            }
            availability = ava;
        }
        else
            Debug.LogError("No SaveFile");      
        
    }

    public static int newAllocatePLCnode(int PLCcode, PlusContainerManager thisPLCmanager, PlusContainerManager otherPLCmanager)
    {
        string contents = thisPLCmanager.plusContainers[PLCcode];
        for (int i = 0; i < contents.Length; i++)
        {
            if (contents[i] == 'p')
            {
                i++;
                string nowPLC = "";
                while (i < contents.Length && contents[i] != 'p' && contents[i] != 'x')
                {
                    nowPLC += contents[i];
                    i++;
                }
                int newPLC = newAllocatePLCnode(int.Parse(nowPLC), thisPLCmanager, otherPLCmanager);

                i--;
                while (contents[i] != 'p')
                {
                    contents = contents.Remove(i, 1);
                    i--;
                }
                i++;

                contents = contents.Insert(i, newPLC.ToString());


            }

        }
        thisPLCmanager.RemovePlusContainer(PLCcode);
        return otherPLCmanager.CreatePlusContainer(contents);


    }

}
