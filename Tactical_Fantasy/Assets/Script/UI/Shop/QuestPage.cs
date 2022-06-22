using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestPage : MonoBehaviour
{
    public Text levelText;
    public Text nowText;
    public Text maxText;
    public RectTransform intimacyInBar;

    public Transform buttonParent;

    public Text titleText;
    public Text contentsText;
    public Text moneyText;
    public Text intimacyText;

    public Text[] requestTexts;
    public Image[] requestChecks;
    public Sprite checkBox;

    public GameObject questCell;
}
