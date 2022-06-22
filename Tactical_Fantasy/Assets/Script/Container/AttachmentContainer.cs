using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentContainer
{
    public int PLCcode;
    public bool isNeedSave;
    public int itemCode;
    public AttachmentCell[] attachmentCells;

    

    public AttachmentContainer()
    {

    }

    public AttachmentContainer(int PLCcode, bool isNeedSave,int itemCode,AttachmentCell[] attachmentCells)
    {
      this.PLCcode = PLCcode;
      this.isNeedSave= isNeedSave;
      this.itemCode = itemCode;
      this.attachmentCells = attachmentCells;
    }

    public string GetContents()
    {
        PlusContainerManager thisPLCmanager = GetThisPLCManager();



        return thisPLCmanager.plusContainers[PLCcode];
    }

    public bool InsertItem(int item, int PLCcode, int index)
    {
        if (index < 0 || index >= attachmentCells.Length)
            return false;

        string contents = GetContents();

        PlusContainerManager thisPLCmanager = GetThisPLCManager();




        if (int.Parse(contents[index * 7].ToString()) == 0)
        {
            if (item / 10000 == (int)attachmentCells[index].kindOfAttachment)
            {
                if((int)attachmentCells[index].kindOfAttachment != 301) //no magazine
                {
                    if((int)attachmentCells[index].kindOfAttachment == 400) // if ammo
                    {
                        if (!Magazine.CheckAmmo(attachmentCells[attachmentCells.Length - 1].kindOfMagazine, ((Ammo)Item.GetItem(item)).kindOfAmmo))
                            return false;
                    }


                    string itemStr = item.ToString();
                    char[] contentsChars = contents.ToCharArray();
                    for (int i = 0; i < 7; i++)
                    {
                        contentsChars[(index * 7) + i] = itemStr[i];
                    }
                        thisPLCmanager.plusContainers[this.PLCcode] = new string(contentsChars);
                    return true;

                }
                else //magazine
                {
                    if (attachmentCells[index].kindOfMagazine == ((Magazine)(Item.GetItem(item))).kindOfMagazine)
                    {
                        string itemStr = item.ToString();
                        char[] contentsChars = contents.Substring(0,(7 * attachmentCells.Length)).ToCharArray();
                        for (int i = 0; i < 7; i++)
                        {
                            contentsChars[(index * 7) + i] = itemStr[i];
                        }
                        contents = new string(contentsChars);
                        contents += 'p';

                        string PLCcodeStr = PLCcode.ToString();
                        

                        foreach(char c in PLCcodeStr)
                        {
                            contents += c;
                        }
                        thisPLCmanager.plusContainers[this.PLCcode] = contents;

                        return true;

                    }
                    else
                       return false;


                }




            }
            else
                return false;



        }
        else 
            return false;

    }


    public bool RemoveItem(int index)
    {
        if (index < 0 || index >= attachmentCells.Length)
            return false;

        string contents = GetContents();

        if (int.Parse(contents[index * 7].ToString()) == 0)
            return false;

        PlusContainerManager thisPLCmanager = GetThisPLCManager();

 



        if ((int)attachmentCells[index].kindOfAttachment != 301)
        {

            char[] contentsChars = contents.ToCharArray();
            for (int i = 0; i < 7; i++)
            {
                contentsChars[(index * 7) + i] = '0';
            }

            thisPLCmanager.plusContainers[this.PLCcode] = new string(contentsChars);
            return true;
        }
        else
        {
            int i = (attachmentCells.Length * 7) + 1;
            string PLCcodeStr = "";
            while(i < contents.Length)
            {
                PLCcodeStr += contents[i];
                i++;
            }
            thisPLCmanager.RemovePlusContainer(int.Parse(PLCcodeStr));
            char[] contentsChars = contents.Substring(0, (7 * attachmentCells.Length)).ToCharArray();

            for (int j = 0; j < 7; j++)
            {
                contentsChars[(index * 7) + j] = '0';
            }
            thisPLCmanager.plusContainers[this.PLCcode] = new string(contentsChars);
            return true;

        }


    }

    public bool RemoveOnlyTextCode(int index)
    {
        if (index < 0 || index >= attachmentCells.Length)
            return false;

        string contents = GetContents();

        if (int.Parse(contents[index * 7].ToString()) == 0)
            return false;

        PlusContainerManager thisPLCmanager = GetThisPLCManager();




        if ((int)attachmentCells[index].kindOfAttachment != 301)
        {

            char[] contentsChars = contents.ToCharArray();
            for (int i = 0; i < 7; i++)
            {
                contentsChars[(index * 7) + i] = '0';
            }

            thisPLCmanager.plusContainers[this.PLCcode] = new string(contentsChars);
            return true;
        }
        else
        {
            char[] contentsChars = contents.Substring(0, (7 * attachmentCells.Length)).ToCharArray();

            for (int j = 0; j < 7; j++)
            {
                contentsChars[(index * 7) + j] = '0';
            }
            thisPLCmanager.plusContainers[this.PLCcode] = new string(contentsChars);
            return true;

        }


    }

    public bool MoveItemToContainerCell(int index, bool isVertical, Container toContaiener, int toX, int toY)
    {
        if (index < 0 || index >= attachmentCells.Length)
            return false;

        string contents = GetContents();


        if (int.Parse(contents[index * 7].ToString()) == 0)
            return false;

        int[] itemdata = GetItemData(index);

        if (itemdata[0] == -1)
            return false;

        if(itemdata.Length == 1) //no Magazine
        {
            if (toContaiener.InsertItem(itemdata[0], new int[0], 1, isVertical, toX, toY))
            {
                RemoveItem(index);
                return true;
            }
            else
                return false;

        }
        else // Magazine
        {
            if(GetThisPLCManager() == toContaiener.GetThisPLCManager())
            {
                if (toContaiener.InsertItem(itemdata[0], new int[1] { itemdata[1] }, 1, isVertical, toX, toY))
                {
                    RemoveOnlyTextCode(index);
                    return true;
                }
                else
                    return false;

            }
            else
            {
                if (toContaiener.CanInsertItem(itemdata[0], isVertical, toX, toY))
                {
                    int newPLC = PlusContainerManager.newAllocatePLCnode(itemdata[1], GetThisPLCManager(), toContaiener.GetThisPLCManager());

                    if (toContaiener.InsertItem(itemdata[0], new int[1] { newPLC }, 1, isVertical, toX, toY))
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

    public bool MoveItemToAttachemntContainer(int myIndex, AttachmentContainer toContaiener, int toIndex)
    {
        if (myIndex < 0 || myIndex >= attachmentCells.Length)
            return false;

        if (toIndex < 0 || toIndex >= toContaiener.attachmentCells.Length)
            return false;


        string contents = GetContents();


        if (int.Parse(contents[myIndex * 7].ToString()) == 0)
            return false;

        int[] itemdata = GetItemData(myIndex);

        if (itemdata[0] == -1)
            return false;

        if (toContaiener.IsRemainIndexCell(toIndex) && (int)(toContaiener.attachmentCells[toIndex].kindOfAttachment) == itemdata[0] / 10000)
        {
            if(itemdata[0]/10000 != 301)
            {

                if (toContaiener.InsertItem(itemdata[0], -1, toIndex))
                {
                    RemoveItem(myIndex);
                    return true;
                }
                else
                    return false;

            }
            else
            {
                if(isNeedSave == toContaiener.isNeedSave)
                {
                    if (toContaiener.InsertItem(itemdata[0], itemdata[1], toIndex))
                    {
                        RemoveOnlyTextCode(myIndex);
                        return true;
                    }
                    else
                        return false;


                }
                else
                {
                    if (toContaiener.CanInsertItem(itemdata[0], toIndex))
                    {
                        int newPLC = PlusContainerManager.newAllocatePLCnode(itemdata[1], GetThisPLCManager(), toContaiener.GetThisPLCManager());

                        if(toContaiener.InsertItem(itemdata[0],newPLC,toIndex))
                        {
                            RemoveOnlyTextCode(myIndex);
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
        else
            return false;




    }


    public bool CanInsertItem(int item, int index)
    {
        if (index < 0 || index >= attachmentCells.Length)
            return false;

        string contents = GetContents();




        if (int.Parse(contents[index * 7].ToString()) == 0)
        {
            if (item / 10000 == (int)attachmentCells[index].kindOfAttachment)
            {
                if ((int)attachmentCells[index].kindOfAttachment != 301) //no magazine
                {
                    if ((int)attachmentCells[index].kindOfAttachment == 400) // if ammo
                    {
                        if (!Magazine.CheckAmmo(attachmentCells[attachmentCells.Length - 1].kindOfMagazine, ((Ammo)Item.GetItem(item)).kindOfAmmo))
                            return false;
                    }

                    return true;
                }
                else //magazine
                {
                    if (attachmentCells[index].kindOfMagazine == ((Magazine)(Item.GetItem(item))).kindOfMagazine)
                    {
                        return true;

                    }
                    else
                        return false;
                }
            }
            else
                return false;
        }
        else
            return false;

    }

    public PlusContainerManager GetThisPLCManager()
    {

        if (isNeedSave)
        {
            return GameManager.saveManager.savePLC;
        }
        else
        {
            return GameManager.nonSavePLCManager;
        }

    }

    public bool IsRemainIndexCell(int index)
    {
        if (index < 0 || index >= attachmentCells.Length)
            return false;

        return GetContents()[index * 7] == '0';
    }

    public int[] GetItemData(int index)
    {
        if (index < 0 || index >= attachmentCells.Length)
            return new int[1] {-1};

        string contents = GetContents();


        if (int.Parse(contents[index * 7].ToString()) == 0)
            return new int[1] { -1 };

        string itemCode = "";

        int startIndex = index * 7;

        for (int i = startIndex; i < startIndex + 7; i++)
            itemCode += contents[i];


        if (int.Parse(itemCode) / 10000 == 301)
        {
            int i = (attachmentCells.Length * 7) + 1;
            string PLCcodeStr = "";
            while (i < contents.Length)
            {
                PLCcodeStr += contents[i];
                i++;
            }

            return new int[2] { int.Parse(itemCode), int.Parse(PLCcodeStr) };

        }
        else
            return new int[1] { int.Parse(itemCode) };

        




    }

}
