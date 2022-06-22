using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Conversation : MonoBehaviour
{
     public delegate void DialogEvent();
     public static void CreateDialog(int code)
     {


     }

     public static void CreateDialog(int code, DialogEvent quitDialog)
     {
       

     }

     public static void CreateDialog(int code , DialogEvent quitDialog, DialogEvent[] answerEvents)
     {

 
     }

}
