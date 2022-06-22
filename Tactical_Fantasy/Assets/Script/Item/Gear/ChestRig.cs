using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ChestRig : Gear
{
    public float bodyDefence;
    public RigCell[] rigCells;
    [System.Serializable]
    public class RigCell
    {
        public int xsize;
        public int ysize;
        public Vector2 postion;
    }
}
