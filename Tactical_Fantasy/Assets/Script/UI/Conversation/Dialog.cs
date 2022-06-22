using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    public int code;
    public int index;
    
    public void NextPage()
    {

    }

    public void QuitDialog()
    {
        GameManager.player.dialog = null;
        Destroy(gameObject);
    }
}
