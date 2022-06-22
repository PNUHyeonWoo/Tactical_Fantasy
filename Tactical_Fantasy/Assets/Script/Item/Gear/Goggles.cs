using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Goggles : Gear
{
    public enum GoggleCategory
    {
        GasMask = 1,
        NightVision = 2
    }
    public GoggleCategory goggleCategory;
    public int performance;

}
