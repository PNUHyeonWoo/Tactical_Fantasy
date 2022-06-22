using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gear : Item
{
    public float discomfort;
    public class Camouflage
    {
        public int woodland;
        public int stone;
        public int desert;
        public int winter;
        public int dark;
    }
    public Camouflage camouflage;

    public enum MiddleCategory
    {
        Gun = 1,
        Hemet = 2,
        Uniform = 3,
        ChestRig = 4,
        Backpack = 5,
        Goggles = 6
    }

}
