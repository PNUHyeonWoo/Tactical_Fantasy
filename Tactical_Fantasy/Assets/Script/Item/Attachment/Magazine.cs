using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Magazine : Attachment
{
    public enum KindOfMagazine
    {
        FiveAR = 1,
        SevenAR = 2,
        NinePistol = 3,
        Shotgun = 4,
        Carbin = 5,
        Makaorv = 6,
        SevenSN = 7,
        FourPDW = 8,
        NineUzi = 9,
        NineMP5 = 10,
        ConstShotgun = 11,
        ConstSevenSN = 12
    }
    public KindOfMagazine kindOfMagazine;
    public int magazineSize;
    public float magazineSpeed;
    public Sprite MagazineSprite;
    public Color MagazineColor;

    public static bool CheckAmmo(KindOfMagazine magazine,Ammo.KindOfAmmo ammo)
    {
        if((int)magazine == 4 || (int)magazine == 11)
        {
            //¼¦°Ç
            return (int)ammo == 4;

        }
        else if((int)magazine == 3 || (int)magazine == 9 || (int)magazine == 10)
        {
            //9
            return (int)ammo == 3;
        }
        else if((int)magazine == 7 || (int)magazine == 12)
        {
            //7sn
            return (int)ammo == 7;

        }
        else
        {
            return (int)magazine == (int)ammo;

        }



    }
}
