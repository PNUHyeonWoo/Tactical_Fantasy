using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DataOfGear : MonoBehaviour
{
    public List<Gun> guns = new List<Gun>();
    public List<Helmet> helmets = new List<Helmet>();
    public List<Uniform> uniforms = new List<Uniform>();
    public List<ChestRig> chestRigs = new List<ChestRig>();
    public List<Backpack> backpacks = new List<Backpack>();
    public List<Goggles> goggles = new List<Goggles>();
    void Start()
    {
        
    }


}
