using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container
{
    public int PLCcode;
    public bool isNeedSave;
    public int xSize;
    public int ySize;

    public struct ItemData
        {
        public bool isVertical;
        public int itemCode;
        public int amount;
        public int[] PLCcode;
        public int PLCamount;

        public ItemData(bool isVertical ,int itemCode ,int amount,int[] PLCcode,int PLCamount)
        {
            this.isVertical = isVertical;
            this.itemCode = itemCode;
            this.amount = amount;
            this.PLCcode = PLCcode;
            this.PLCamount = PLCamount;
        }
        }

    public Container()
    {

    }
    public Container(int PLCcode,bool isNeedSave, int xSize, int ySize)
    {
        this.PLCcode = PLCcode;
        this.isNeedSave = isNeedSave;
        this.xSize = xSize;
        this.ySize = ySize;     
    }

    public Container(bool isNeedSave,int xSize,int ySize,string contents)
    {
        this.isNeedSave = isNeedSave;
        this.xSize = xSize;
        this.ySize = ySize;

        PlusContainerManager thisPLCmanager;

        if (isNeedSave)
        {
            thisPLCmanager =  GameManager.saveManager.savePLC;
        }
        else
        {
            thisPLCmanager =  GameManager.nonSavePLCManager;
        }

        this.PLCcode = thisPLCmanager.CreatePlusContainer(contents);
    }



    public void RemoveThisPLC()
    {
        PlusContainerManager thisPLCmanager = GetThisPLCManager();


        thisPLCmanager.RemovePlusContainer(PLCcode);       

    }

    public string GetContents()
    {
        PlusContainerManager thisPLCmanager = GetThisPLCManager();


        return thisPLCmanager.plusContainers[PLCcode];
    }


    public bool InsertItem(int item,int[] PLCcode,int amount, bool isVertical,int xinsert, int yinsert)
    {
        if (xinsert < 0 || yinsert < 0)
            return false;

        string contents;
        PlusContainerManager thisPLCmanager = GetThisPLCManager();

        contents = thisPLCmanager.plusContainers[this.PLCcode];
        
        Item realitem = Item.GetItem(item);

        if (isVertical)
        {
            if (xinsert + realitem.xsize - 1 > xSize - 1 ||
               yinsert + realitem.ysize - 1 > ySize - 1)
                return false;
        }
        else
        {
            if (xinsert + realitem.ysize - 1 > xSize - 1 ||
               yinsert + realitem.xsize - 1 > ySize - 1)
                return false;
        }

        bool[,] boolmap = GetBoolMap(contents);


        if (isVertical)
        {
            for (int y = yinsert; y < yinsert + realitem.ysize; y++)
            {
                for (int x = xinsert; x < xinsert + realitem.xsize; x++)
                {
                    if (boolmap[y, x] == true)
                        return false;
                }
            }
        }
        else
        {
            for (int y = yinsert; y < yinsert + realitem.xsize; y++)
            {
                for (int x = xinsert; x < xinsert + realitem.ysize; x++)
                {
                    if (boolmap[y, x] == true)
                        return false;
                }
            }
        }

        char isV;
        if (isVertical)
            isV = 'v';
        else
            isV = 'h';

        if (Item.CheckPLC(item))
        {
            if (PLCcode.Length == 1)
            {
                contents += "x" + xinsert.ToString() + "y" + yinsert.ToString() + isV + item.ToString() + "a" + amount.ToString() + "p" + PLCcode[0].ToString();
                thisPLCmanager.plusContainers[this.PLCcode]= contents;
                return true;
            }
            else
            {
                contents += "x" + xinsert.ToString() + "y" + yinsert.ToString() + isV + item.ToString() + "a" + amount.ToString();
                foreach(int i in PLCcode)
                {
                    contents += "p" + i.ToString();
                }
                thisPLCmanager.plusContainers[this.PLCcode] = contents;
                return true;

            }
        }
        else
        {
            contents += "x" + xinsert.ToString() + "y" + yinsert.ToString() + isV + item.ToString() + "a" + amount.ToString();
            thisPLCmanager.plusContainers[this.PLCcode] = contents;
            return true;
        }

    }

    public bool InsertItem(int item, int[] PLCcode, int amount)
    {

        bool[,] boolmap = GetBoolMap(GetContents());
        bool successInput = false;

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                if (!boolmap[y, x])
                {
                    if (InsertItem(item,PLCcode,amount,true,x,y))
                    {
                        successInput = true;
                        break;
                    }
                    else if (InsertItem(item, PLCcode, amount, false, x, y))
                    {
                        successInput = true;
                        break;
                    }


                }


            }
            if (successInput)
            {
                break;
            }

        }

        return successInput;

    }

    public bool RemoveItem(int x,int y)
    {
        string contents = GetContents();

        int i = contents.IndexOf("x" + x.ToString() + "y" + y.ToString() + "v");
        if (i == -1)
            i = contents.IndexOf("x" + x.ToString() + "y" + y.ToString() + "h");

        if (i == -1)
            return false;

        PlusContainerManager thisPLCmanager = GetThisPLCManager();
 
        int c = i+4;

        while(contents[c] != 'v' && contents[c] != 'h')
        {
            c++;
        }

        c++;

        string itemcodestr = "";

        for(int j = 0;j<7;j++)
        {
            itemcodestr += contents[c];
            c++;
        }

        int itemcode = int.Parse(itemcodestr);

        if (!Item.CheckPLC(itemcode))
        {
            contents = contents.Remove(i, 1);


            while (i < contents.Length)
            {
                if (contents[i] != 'x')
                {
                    contents = contents.Remove(i, 1);
                }
                else
                {
                    thisPLCmanager.plusContainers[this.PLCcode] = contents;
                    return true;
                }
            }
            thisPLCmanager.plusContainers[this.PLCcode] = contents;
            return true;
        }
        else
        {
            c+= 2;

            while(contents[c] != 'p' )
            {
                c++;
            }

            while (contents[c] == 'p')
            {
                c++;
                string nowPLCcode = "";

                while (contents[c] != 'p' && contents[c] != 'x')
                {
                    nowPLCcode += contents[c];
                    c++;
                    if (c >= contents.Length)
                        break;
                }

                      
                RemovePLCnode(int.Parse(nowPLCcode),thisPLCmanager);


                if (c >= contents.Length)
                    break;

            }

            contents = contents.Remove(i, 1);

            while (i < contents.Length)
            {
                if (contents[i] != 'x')
                {
                    contents = contents.Remove(i, 1);
                }
                else
                {
                    thisPLCmanager.plusContainers[this.PLCcode] = contents;
                    return true;
                }
            }
            thisPLCmanager.plusContainers[this.PLCcode] = contents;
            return true;
        }


        void RemovePLCnode(int PLCcode,PlusContainerManager PLCmanager)
        {

            string contents = PLCmanager.plusContainers[PLCcode];
            for(int i = 0; i < contents.Length;i++)
            {
                if (contents[i] == 'p')
                {
                    i++;
                    string nowPLC= "";
                    while(i < contents.Length && contents[i] != 'p' && contents[i] != 'x')
                    {
                       nowPLC += contents[i];
                        i++;                   
                    }
                    RemovePLCnode(int.Parse(nowPLC),PLCmanager);

                    if(i >= contents.Length)
                    {
                        continue;
                    }

                    if (contents[i] == 'p')
                        i--;

                }


            }

            PLCmanager.RemovePlusContainer(PLCcode);


        }
    

    }

    public bool RemoveItem(int amount, int x, int y)
    {
        string contents = GetContents();

        int i = contents.IndexOf("x" + x.ToString() + "y" + y.ToString() + "v");
        if (i == -1)
            i = contents.IndexOf("x" + x.ToString() + "y" + y.ToString() + "h");

        if (i == -1)
            return false;

        PlusContainerManager thisPLCmanager = GetThisPLCManager();
 

        i += 12;


            while (contents[i] != 'a')
            {
            i++;
            }

        string orignalAmount = "";
        i++;

        while (i < contents.Length)
        {
            if (contents[i] != 'x' && contents[i] != 'p')
            {
                orignalAmount += contents[i];
                contents = contents.Remove(i,1);

            }
            else
            {
                int newAmount1 = int.Parse(orignalAmount) - amount;
                if (newAmount1 < 1)
                {
                    newAmount1 = 1;
                    Debug.Log("My Error: remove item result is lower than 0");
                }
                contents = contents.Insert(i, newAmount1.ToString());
                thisPLCmanager.plusContainers[this.PLCcode] = contents;
                return true;
            }
        }
        int newAmount = int.Parse(orignalAmount) - amount;
        if (newAmount < 1)
        {
            newAmount = 1;
            Debug.Log("My Error: remove item result is lower than 0");
        }
        contents = contents.Insert(i, newAmount.ToString());
        thisPLCmanager.plusContainers[this.PLCcode] = contents;
        return true;
      


    }

    public bool RemoveOnlyTextCode(int x,int y)
    {
        PlusContainerManager thisPLCmanager = GetThisPLCManager();
  

        string contents = GetContents();

        int i = contents.IndexOf("x" + x.ToString() + "y" + y.ToString() + "v");
        if (i == -1)
            i = contents.IndexOf("x" + x.ToString() + "y" + y.ToString() + "h");

        if (i == -1)
            return false;

        contents = contents.Remove(i, 1);


        while (i < contents.Length)
        {
            if (contents[i] != 'x')
            {
                contents = contents.Remove(i, 1);
            }
            else
            {
                thisPLCmanager.plusContainers[this.PLCcode] = contents;
                return true;
            }
        }
        thisPLCmanager.plusContainers[this.PLCcode] = contents;
        return true;
    }

    public bool MoveItemToSelf(int myX, int myY, bool isVertical, int toX, int toY)
    {
        string contents = GetContents();
        string itemString = GetItemString(myX, myY);
        if(itemString == "")
        {
            Debug.Log("Myerror : no item to move");
            return false;
        }

        ItemData itemData = GetItemDataFromString(itemString);


        int i = contents.IndexOf("x" + myX.ToString() + "y" + myY.ToString() + "v");
        if(i == -1)
            i = contents.IndexOf("x" + myX.ToString() + "y" + myY.ToString() + "h");

        if (i == -1)
        {
            Debug.Log("Myerror : no item to move");
            return false;
        }

        contents = contents.Remove(i, 1);


        while (i < contents.Length)
        {
            if (contents[i] != 'x')
            {
                contents = contents.Remove(i, 1);
            }
            else
            {
                break;
            }
        }
        if (CanInsertItem(contents, itemData.itemCode, isVertical, toX, toY))
        {
            RemoveOnlyTextCode(myX, myY);
            if (InsertItem(itemData.itemCode, itemData.PLCcode, itemData.amount, isVertical, toX, toY))
                return true;
            else
            {
                Debug.Log("MyError: Seriose Error!!!!!!!!!!!!!!! only Remove no insert!!!!!!!!!!");
                return false;
            }
        }
        else
            return false;



    }


    public bool MoveItemToContainerCell(int myX,int myY,bool isVertical, Container toContaiener, int toX , int toY)
    {
        string contents = GetContents();

        string itemString = GetItemString(myX, myY);

        if (itemString == "")
        {
            Debug.Log("Myerror: no item move");
            return false;
        }

        if (!itemString.Contains("p")) //non PLC Item Move
        {
           ItemData itemdata = GetItemDataFromString(itemString);

            itemdata.isVertical = isVertical;


           if(toContaiener.InsertItem(itemdata.itemCode, new int[0], itemdata.amount, itemdata.isVertical, toX, toY))
                {
                    RemoveItem(myX, myY);
                    return true;
                }
           else
                    return false;

        }
        else  //PLC Item Move
        {
            ItemData itemdata = GetItemDataFromString(itemString);

            itemdata.isVertical = isVertical;

            if (GetThisPLCManager() == toContaiener.GetThisPLCManager())
            {
                if (toContaiener.InsertItem(itemdata.itemCode, itemdata.PLCcode, itemdata.amount, itemdata.isVertical, toX, toY))
                {
                    RemoveOnlyTextCode(myX, myY);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if(toContaiener.CanInsertItem(itemdata.itemCode, itemdata.isVertical, toX, toY))
                {
                    PlusContainerManager thisPLCmanager = GetThisPLCManager();
                    PlusContainerManager otherPLCmanager = toContaiener.GetThisPLCManager();

  

                    for (int i = 0; i < itemdata.PLCcode.Length; i++)
                        itemdata.PLCcode[i] = PlusContainerManager.newAllocatePLCnode(itemdata.PLCcode[i], thisPLCmanager, otherPLCmanager);


                    if (toContaiener.InsertItem(itemdata.itemCode, itemdata.PLCcode, itemdata.amount, itemdata.isVertical, toX, toY))
                    {
                        RemoveOnlyTextCode(myX, myY);
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

    public bool MoveItemToAttachemntCell(int myX, int myY, AttachmentContainer toContaiener, int index)
    {
       string itemString = GetItemString(myX, myY);

        if (itemString == "")
        {
            Debug.Log("Myerror: no item move");
            return false;
        }

        ItemData itemdata = GetItemDataFromString(itemString);

      

        if (toContaiener.IsRemainIndexCell(index) && (int)(toContaiener.attachmentCells[index].kindOfAttachment) == itemdata.itemCode / 10000)
        {
            if(itemdata.itemCode / 10000 != 301)
            {
                if(itemdata.itemCode / 10000 == 400)
                {
                    if (toContaiener.InsertItem(itemdata.itemCode, -1, index))
                    {
                        if(itemdata.amount == 1)
                        {
                            RemoveItem(myX, myY);
                            return true;
                        }
                        else
                        {
                            RemoveItem(1, myX, myY);
                            return true;
                        }

                    }
                    else
                        return false;

                }


                if (toContaiener.InsertItem(itemdata.itemCode, -1, index))
                {
                    RemoveItem(myX, myY);
                    return true;
                }
                else
                    return false;

            }
            else //Magazine
            {
                if(isNeedSave == toContaiener.isNeedSave)
                {
                    if (toContaiener.InsertItem(itemdata.itemCode, itemdata.PLCcode[0], index))
                    {
                        RemoveOnlyTextCode(myX, myY);
                        return true;
                    }
                    else
                        return false;

                }
                else
                {
                    if (toContaiener.CanInsertItem(itemdata.itemCode, index))
                    {
                        PlusContainerManager thisPLCmanager = GetThisPLCManager();
                        PlusContainerManager otherPLCmanager = toContaiener.GetThisPLCManager();

                        itemdata.PLCcode[0] = PlusContainerManager.newAllocatePLCnode(itemdata.PLCcode[0], thisPLCmanager, otherPLCmanager);

                        if (toContaiener.InsertItem(itemdata.itemCode, itemdata.PLCcode[0], index))
                        {
                            RemoveOnlyTextCode(myX, myY);
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
        {
            return false;
        }


    }

    public bool MoveItemToPlayerGear(int myX, int myY,PlayerGear playerGear,int index)
    {
        string itemString = GetItemString(myX, myY);

        if (itemString == "")
        {
            Debug.Log("Myerror: no item move");
            return false;
        }

        ItemData itemdata = GetItemDataFromString(itemString);

        if(playerGear.CaninsertItem(itemdata.itemCode,index))
        {
            if (Item.CheckPLC(itemdata.itemCode))
            {
                if(isNeedSave)
                {
                    if (playerGear.insertItem(itemdata.itemCode, itemdata.PLCcode, index))
                    {
                        RemoveOnlyTextCode(myX, myY);
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    for (int i = 0; i < itemdata.PLCcode.Length; i++)
                        itemdata.PLCcode[i] = PlusContainerManager.newAllocatePLCnode(itemdata.PLCcode[i], GetThisPLCManager(), playerGear.savePLCManager);

                    if (playerGear.insertItem(itemdata.itemCode, itemdata.PLCcode, index))
                    {
                        RemoveOnlyTextCode(myX, myY);
                        return true;
                    }
                    else
                    {
                        Debug.Log("MyError: Serious Error!!!!!!!!!!! only reAloocatePLC No Move!!!!!!!!!");
                        return false;
                    }


                }



            }
            else
            {
                if (playerGear.insertItem(itemdata.itemCode, new int[0], index))
                {
                    RemoveItem(myX, myY);
                    return true;
                }
                else
                    return false;
            }




        }
        else
            return false;




    }


    public string GetItemString(int x, int y)
    {
        string contents = GetContents();
        string result = "";

        int i = contents.IndexOf("x" + x.ToString() + "y" + y.ToString() + "v");
        if(i == -1)
             i = contents.IndexOf("x" + x.ToString() + "y" + y.ToString() + "h");

        if (i == -1)
            return "";

        result += "x";

        i++;

        while(i < contents.Length && contents[i] != 'x')
        {
            result += contents[i];
            i++;
        }

        return result;
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

    public ItemData GetItemDataFromString(string itemString)
    {
        bool isVertical;
        string itemCode = "";
        string amount = "";
        int[] PLCcode;
        int PLCamount = 0;

        for (int j = 0; j < itemString.Length; j++)
        {
            if (itemString[j] == 'p')
                PLCamount++;
        }

        PLCcode = new int[PLCamount];

        int i = 4;

        while (itemString[i] != 'v' && itemString[i] != 'h')
            i++;

        isVertical = itemString[i] == 'v';

        i++;

        for (int j = 0; j < 7; j++)
        {
            itemCode += itemString[i];
            i++;
        }

        i++;

        while (i < itemString.Length && itemString[i] != 'p')
        {
            amount += itemString[i];
            i++;
        }

        i++;

        int PLCindex = 0;

        while (i < itemString.Length)
        {
         
           string nowPLC = "";
           
            while(i < itemString.Length && itemString[i] != 'p')
            {
                nowPLC += itemString[i];
                i++;
            }

            PLCcode[PLCindex] = int.Parse(nowPLC);
            PLCindex++;
            i++;

        }


        return new ItemData(isVertical, int.Parse(itemCode), int.Parse(amount), PLCcode, PLCamount);
        //³Ö±â


    }

    public bool CanInsertItem(int item, bool isVertical, int xinsert, int yinsert)
    {
        if (xinsert < 0 || yinsert < 0)
            return false;


        string contents = GetContents();
        Item realitem = Item.GetItem(item);
        if (isVertical)
        {
            if (xinsert + realitem.xsize - 1 > xSize - 1 ||
               yinsert + realitem.ysize - 1 > ySize - 1)
                return false;
        }
        else
        {
            if (xinsert + realitem.ysize - 1 > xSize - 1 ||
               yinsert + realitem.xsize - 1 > ySize - 1)
                return false;
        }

        bool[,] boolmap = GetBoolMap(contents);


        if (isVertical)
        {
            for (int y = yinsert; y < yinsert + realitem.ysize; y++)
            {
                for (int x = xinsert; x < xinsert + realitem.xsize; x++)
                {
                    if (boolmap[y, x] == true)
                        return false;
                }
            }
        }
        else
        {
            for (int y = yinsert; y < yinsert + realitem.xsize; y++)
            {
                for (int x = xinsert; x < xinsert + realitem.ysize; x++)
                {
                    if (boolmap[y, x] == true)
                        return false;
                }
            }
        }

        return true;

    }

    public bool CanInsertItem(string contents ,int item, bool isVertical, int xinsert, int yinsert)
    {
        if (xinsert < 0 || yinsert < 0)
            return false;
       
        Item realitem = Item.GetItem(item);
        if (isVertical)
        {
            if (xinsert + realitem.xsize - 1 > xSize - 1 ||
               yinsert + realitem.ysize - 1 > ySize - 1)
                return false;
        }
        else
        {
            if (xinsert + realitem.ysize - 1 > xSize - 1 ||
               yinsert + realitem.xsize - 1 > ySize - 1)
                return false;
        }

        bool[,] boolmap = GetBoolMap(contents);


        if (isVertical)
        {
            for (int y = yinsert; y < yinsert + realitem.ysize; y++)
            {
                for (int x = xinsert; x < xinsert + realitem.xsize; x++)
                {
                    if (boolmap[y, x] == true)
                        return false;
                }
            }
        }
        else
        {
            for (int y = yinsert; y < yinsert + realitem.xsize; y++)
            {
                for (int x = xinsert; x < xinsert + realitem.ysize; x++)
                {
                    if (boolmap[y, x] == true)
                        return false;
                }
            }
        }

        return true;

    }

    public bool CanInsertItem(int item)
    {

        bool[,] boolmap = GetBoolMap(GetContents());
        bool successInput = false;

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                if (!boolmap[y, x])
                {
                    if (CanInsertItem(item, true, x, y))
                    {
                        successInput = true;
                        break;
                    }
                    else if (CanInsertItem(item, false, x, y))
                    {
                        successInput = true;
                        break;
                    }


                }


            }
            if (successInput)
            {
                break;
            }

        }

        return successInput;

    }


    public bool[,] GetBoolMap(string contents)
    {
        bool[,] boolMap = new bool[ySize,xSize];
        for(int i = 0; i < contents.Length;i++)
        {
            if(contents[i] == 'x')
            {
                int[] xy = GetXY(contents, i);
                for (int y = xy[1]; y < xy[1] + xy[3]; y++) 
                {
                    for (int x = xy[0]; x < xy[0] + xy[2]; x++)
                    {
                        boolMap[y, x] = true;
                    }

                }


            }

        }
        return boolMap;

    }

    private int[] GetXY(string contents, int index)
    {
        int j = index+1;
        string xPos = "";
        string yPos = "";
        string code= "";
        bool isVertical;

        while (contents[j] != 'y')
        {
            xPos += contents[j];
            j++;
        }
        j++;
        while (contents[j] != 'v' && contents[j] != 'h')
        {
            yPos += contents[j];
            j++;
        }

        isVertical = contents[j] == 'v';

        j++;
        for (int u = 0; u < 7; u++)
        {
            code += contents[j];
            j++;
        }
        Item item = Item.GetItem(int.Parse(code));

        if(isVertical)
        return new int[]{ int.Parse(xPos), int.Parse(yPos), item.xsize, item.ysize };
        else
        return new int[] { int.Parse(xPos), int.Parse(yPos), item.ysize, item.xsize };
    }

    public void LogBoolMap()
    {


       bool[,] boolMap = GetBoolMap(GetContents());

        string a = "";

        for (int i = 0; i < ySize;i++)
        {
            for(int j = 0;j<xSize;j++)
            {
                char c;
                if (boolMap[i, j])
                    c = '¡á';
                else
                    c = '¡à';
                
                   a += c;


            }
            a += "\n";

        }

        Debug.Log(a);


    }
}
