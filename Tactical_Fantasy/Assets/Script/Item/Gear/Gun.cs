using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun : Gear
{
  
    public enum GunAnimation
    {
        K2Ak = 1,
        Ar15 = 2,
        PumpShotgun = 3,
        DoubleBarrel = 4,
        Boltaction = 5,
        K7 = 6,
        Uzi = 7,
        Mp7 = 8,
        Pistol = 9 ,
        M1Carbin = 10    
    }
    public GunAnimation gunAnimation;
    public int constMagazineCode;
    public Texture gunTexture;
    public AttachmentCell[] attachmentCells;
    public bool canBB;
    public float rateOfFire;
    public Vector2 maxRecoil;
    public Vector2 minRecoil;
    public float plusRecoil;
    public float recoverRecoil;
    public AudioClip fireSound;
    public float fireLoudness;
    public AudioClip boltSound;
    public AudioClip BBSound;
    public bool single;
    public bool bisrt;
    public bool Auto;

    public static bool CheckVisualMagzine(Gun gun)
    {
        return (gun.gunAnimation != GunAnimation.PumpShotgun && gun.gunAnimation != GunAnimation.DoubleBarrel && gun.gunAnimation != GunAnimation.Boltaction && gun.gunAnimation != GunAnimation.Pistol);
   
    }

    public static int CheckGunType(Gun gun)
    {
        if (gun.gunAnimation == GunAnimation.K2Ak || gun.gunAnimation == GunAnimation.Ar15 || gun.gunAnimation == GunAnimation.K7 || gun.gunAnimation == GunAnimation.Uzi || gun.gunAnimation == GunAnimation.Mp7 || gun.gunAnimation == GunAnimation.Pistol || gun.gunAnimation == GunAnimation.M1Carbin)
            return 0;
        else if (gun.gunAnimation == GunAnimation.DoubleBarrel)
            return 1;
        else if (gun.gunAnimation == GunAnimation.PumpShotgun)
            return 2;
        else if (gun.gunAnimation == GunAnimation.Boltaction)
            return 3;

        return -1;

    }

    public static int[] GetMagazineCodePLC(int gunPLC , bool isSave)
    {
        string contents;    

        if (isSave)
            contents = GameManager.saveManager.savePLC.plusContainers[gunPLC];
        else
            contents = GameManager.nonSavePLCManager.plusContainers[gunPLC];

        if (!contents.Contains("p"))
            return new int[] { -1,-1 };

        int i = contents.Length - 1;

        while (contents[i] != 'p' && 0 < i)
        {
            i--;
        }

        if (i <= 0)
            return new int[] {-1,-1 };

        int pi = i;

        i -= 7;

        string itemCode = "";

        for (int j = 0; j < 7; j++)
        {
            itemCode += contents[i];
            i++;
        }

        string PLCCode = "";

        pi++;
        while(contents.Length > pi)
        {
            PLCCode += contents[pi];
            pi++;
        }

        return new int[] { int.Parse(itemCode), int.Parse(PLCCode) };
        


    }

    public static float[] GetLowDis(Gun gun)
    {
        if(gun.gunAnimation == GunAnimation.K2Ak)
        {
            return new float[2] { 150, 50 };

        }
        else if(gun.gunAnimation == GunAnimation.Pistol)
        {
            return new float[2] { 150, 50 };
        }


        return new float[2] {-1,-1 };

    }

    public static int GetDefaultFireMode(Gun gun)
    {
        if(gun.gunAnimation == GunAnimation.K2Ak || gun.gunAnimation == GunAnimation.Ar15)
        {
            if(SettingManager.assultRifleDefaultAuto)
            {
                if (gun.Auto)
                    return 2;
                else if (gun.bisrt)
                    return 1;
                else
                    return 0;
            }
            else
            {
                if (gun.single)
                    return 0;
                else if (gun.bisrt)
                    return 1;
                else
                    return 2;
            }

        }
        else if(gun.gunAnimation == GunAnimation.K7 || gun.gunAnimation == GunAnimation.Uzi || gun.gunAnimation == GunAnimation.Mp7)
        {
            if (SettingManager.submachineGunDefaultAuto)
            {
                if (gun.Auto)
                    return 2;
                else if (gun.bisrt)
                    return 1;
                else
                    return 0;
            }
            else
            {
                if (gun.single)
                    return 0;
                else if (gun.bisrt)
                    return 1;
                else
                    return 2;
            }

        }
        else if(gun.gunAnimation == GunAnimation.Pistol)
        {
            if (SettingManager.pistolDefaultAuto)
            {
                if (gun.Auto)
                    return 2;
                else if (gun.bisrt)
                    return 1;
                else
                    return 0;
            }
            else
            {
                if (gun.single)
                    return 0;
                else if (gun.bisrt)
                    return 1;
                else
                    return 2;
            }


        }
        else
        {
            return 0;
        }

    }

    public static float GetBarrelRange(Gun gun,int direct)
    {
        if (gun.gunAnimation == GunAnimation.K2Ak)
        {
            if (direct == -3)
                return 17;
            else if (direct == -2)
                return 12;
            else if (direct == -1)
                return 8;
            else if (direct == 0)
                return 24;
            else if (direct == 1)
                return 20;
            else if (direct == 2)
                return 17;
            else if (direct == 3)
                return -20;

        }
        else if (gun.gunAnimation == GunAnimation.Pistol)
        {
            if (direct == -3)
                return 14;
            else if (direct == -2)
                return 10;
            else if (direct == -1)
                return 8;
            else if (direct == 0)
                return 20;
            else if (direct == 1)
                return 16;
            else if (direct == 2)
                return 12;
            else if (direct == 3)
                return -16;
        }


        return 0;
    }

    public static float GetBoltRange(Gun gun, int direct)
    {
        if (gun.gunAnimation == GunAnimation.K2Ak)
        {
            if (direct == -3)
                return 10.5f;
            else if (direct == -2)
                return 6.5f;
            else if (direct == -1)
                return 3.5f;
            else if (direct == 0)
                return 10.5f;
            else if (direct == 1)
                return 11.5f;
            else if (direct == 2)
                return 10.5f;
            else if (direct == 3)
                return -11.5f;

        }
        else if (gun.gunAnimation == GunAnimation.Pistol)
        {
            if (direct == -3)
                return 5.5f;
            else if (direct == -2)
                return 2.5f;
            else if (direct == -1)
                return 2.5f;
            else if (direct == 0)
                return 5.5f;
            else if (direct == 1)
                return 5.5f;
            else if (direct == 2)
                return 5.5f;
            else if (direct == 3)
                return -5.5f;
        }


        return 0;
    }

}
