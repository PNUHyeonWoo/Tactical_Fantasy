using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Ammo : Item
{
    public enum KindOfAmmo
    {
        FiveAR = 1,
        SevenAR = 2,
        NinePistol = 3,
        Shotgun = 4,
        Carbin = 5,
        Makaorv = 6,
        SevenSN = 7,
        FourPDW = 8
    }
    public KindOfAmmo kindOfAmmo;
    public float damage;
    public int Penetration;

    public int numWarhead;
    public float spread;
}
