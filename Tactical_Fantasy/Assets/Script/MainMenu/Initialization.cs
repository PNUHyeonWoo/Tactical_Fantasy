using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    GameObject gameManager;
    SaveManager saveManager;
    public void NewGameInitializate()
    {
        saveManager = gameManager.transform.Find("SaveManager").GetComponent<SaveManager>();
        


    }
}
