using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DebugChest : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Start()
    {
        Chest chest = GetComponent<Chest>();

        chest.thisContainer = new Container(false, 10, 14, "");

        //รั
        int gun1PLC = GameManager.nonSavePLCManager.CreatePlusContainer("00000000000000");
        int gun2PLC = GameManager.nonSavePLCManager.CreatePlusContainer("00000000000000");
        int pistol1PLC = GameManager.nonSavePLCManager.CreatePlusContainer("00000000000000");
        int pistol2PLC = GameManager.nonSavePLCManager.CreatePlusContainer("00000000000000");
        int backpack1PLC = GameManager.nonSavePLCManager.CreatePlusContainer("");
        int[] chestrig1PLC = new int[4];
        for(int i =0; i < 4; i++)
        {
            chestrig1PLC[i] = GameManager.nonSavePLCManager.CreatePlusContainer("");
        }
        int magazine1PLC = GameManager.nonSavePLCManager.CreatePlusContainer("");
        int magazine2PLC = GameManager.nonSavePLCManager.CreatePlusContainer("");
        int magazine3PLC = GameManager.nonSavePLCManager.CreatePlusContainer("");
        int magazine4PLC = GameManager.nonSavePLCManager.CreatePlusContainer("");
        int magazine5PLC = GameManager.nonSavePLCManager.CreatePlusContainer("");
        int magazine6PLC = GameManager.nonSavePLCManager.CreatePlusContainer("");

        int pMagazine1PLC = GameManager.nonSavePLCManager.CreatePlusContainer("");
        int pMagazine2PLC = GameManager.nonSavePLCManager.CreatePlusContainer("");
        int pMagazine3PLC = GameManager.nonSavePLCManager.CreatePlusContainer("");


        chest.thisContainer.InsertItem(2010000,new int[1] {gun1PLC},1);
        chest.thisContainer.InsertItem(2010003, new int[1] { gun2PLC }, 1);
        chest.thisContainer.InsertItem(2010001, new int[1] { pistol1PLC }, 1);
        chest.thisContainer.InsertItem(2010001, new int[1] { pistol2PLC }, 1);
        chest.thisContainer.InsertItem(2050000, new int[1] { backpack1PLC }, 1);
        chest.thisContainer.InsertItem(2040000, chestrig1PLC, 1);
        chest.thisContainer.InsertItem(3010000, new int[1] { magazine1PLC }, 1);
        chest.thisContainer.InsertItem(3010000, new int[1] { magazine2PLC }, 1);
        chest.thisContainer.InsertItem(3010000, new int[1] { magazine3PLC }, 1);
        chest.thisContainer.InsertItem(3010000, new int[1] { magazine4PLC }, 1);
        chest.thisContainer.InsertItem(3010004, new int[1] { magazine5PLC }, 1);
        chest.thisContainer.InsertItem(3010004, new int[1] { magazine6PLC }, 1);
        chest.thisContainer.InsertItem(3010001, new int[1] { pMagazine1PLC }, 1);
        chest.thisContainer.InsertItem(3010001, new int[1] { pMagazine2PLC }, 1);
        chest.thisContainer.InsertItem(3010001, new int[1] { pMagazine3PLC }, 1);


        chest.thisContainer.InsertItem(4000000, new int[] { }, 60);
        chest.thisContainer.InsertItem(4000000, new int[] { }, 60);
        chest.thisContainer.InsertItem(4000000, new int[] { }, 60);
        chest.thisContainer.InsertItem(4000000, new int[] { }, 60);
        chest.thisContainer.InsertItem(4000001, new int[] { }, 60);
        chest.thisContainer.InsertItem(4000001, new int[] { }, 60);
        chest.thisContainer.InsertItem(4000001, new int[] { }, 60);
        chest.thisContainer.InsertItem(4000002, new int[] { }, 30);
        chest.thisContainer.InsertItem(4000003, new int[] { }, 30);
        chest.thisContainer.InsertItem(4000004, new int[] { }, 60);

        chest.thisContainer.InsertItem(5000000, new int[] { }, 1);
        chest.thisContainer.InsertItem(5000001, new int[] { }, 1);
        chest.thisContainer.InsertItem(5000001, new int[] { }, 1);
        chest.thisContainer.InsertItem(5000002, new int[] { }, 1);
        chest.thisContainer.InsertItem(5000002, new int[] { }, 1);
        chest.thisContainer.InsertItem(1010000, new int[] { }, 16);
        chest.thisContainer.InsertItem(1010001, new int[] { }, 16);
        //CheckSbyte(new bool[8] { false, false, false, false, false, false, false, true });


    }

    // Update is called once per frame
    void Update()
    {       
        /*
        if(Input.GetKeyDown(KeyCode.H))
        {        
            Attack.HitResult[] hits = Attack.AttackWithCastAOE(GameObject.Find("testColider").GetComponent<Collider2D>(), false, 100, 4, gameObject, new int[3] {0,1,2}, false);
            
            Vector2 SmousePos;
            SmousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            SmousePos.x = SmousePos.x - GameObject.Find("testColider").transform.position.x; 
            SmousePos.y = SmousePos.y - GameObject.Find("testColider").transform.position.y;

            Attack.AttackWithRayCast(GameObject.Find("testColider").transform.position.x, GameObject.Find("testColider").transform.position.y, SmousePos.x, SmousePos.y, false, 90, 4, GameObject.Find("testColider").gameObject, new int[1] { 0 }, false, 300);

            
            foreach(Attack.HitResult hit in hits)
            {
                Debug.Log(hit.hitObject);
                Debug.Log(hit.isDie);
                Debug.Log(hit.damage);
                Debug.Log(hit.TargetID);
            }
            

            //GameObject.Find("testPoint").transform.position = new Vector3(hits[0].point.x, hits[0].point.y, -50);

        }
         */
    }

    void CheckSbyte(bool[] bs)
    {
        BitArray asd = new BitArray(bs);
        sbyte[] s = new sbyte[1];
        asd.CopyTo(s, 0);
        Debug.Log(s[0]);
    }
}
