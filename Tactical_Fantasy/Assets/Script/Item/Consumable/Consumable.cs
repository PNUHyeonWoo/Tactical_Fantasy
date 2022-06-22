using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class Consumable : Item
{
    public bool isDisposable = true;
    public int useSoundIndex;
    public UnityEvent useEffect;
}
