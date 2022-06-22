using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager
{
    static public int slot = 0;
    static public string language = "Korean";

    static public bool isToggleZoom = true;
    static public bool isToggleLow = true;
    static public bool isKeepDownRun = true;
    static public bool assultRifleDefaultAuto = false;
    static public bool submachineGunDefaultAuto = false;
    static public bool pistolDefaultAuto = true;

    static public KeyCode[] Key_wasd = new KeyCode[4] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    static public KeyCode Key_run = KeyCode.LeftShift;
    static public KeyCode Key_Prone = KeyCode.Z;
    static public KeyCode Key_ProneSlide = KeyCode.Space;
    static public KeyCode Key_Backpack = KeyCode.I;
    static public KeyCode[] Key_SelectGun= new KeyCode[4] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
    static public KeyCode Key_MoveMouseToRig = KeyCode.Tab;
    static public KeyCode Key_Bolt = KeyCode.R;
    static public KeyCode Key_LowFIre = KeyCode.LeftControl;
    static public KeyCode Key_Interaction = KeyCode.E;
    static public KeyCode Key_DropMagazine = KeyCode.T;
    static public KeyCode key_FIreMode = KeyCode.B;

}
