using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCheck : MonoBehaviour
{

    void Update()
    {
        if (GameManager.PauseAmount > 0)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(GameManager.nonSavePLCManager.availability);
            Debug.Log(GameManager.saveManager.savePLC.availability);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            GameManager.saveManager.Save(0);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SaveManager.CreateNewSlot(0);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GameManager.player.PlayPlayerSound(0,0);
        }
    }
}
