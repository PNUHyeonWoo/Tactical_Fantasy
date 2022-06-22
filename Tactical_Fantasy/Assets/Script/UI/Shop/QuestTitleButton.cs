using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTitleButton : MonoBehaviour
{
    public MerchantUI merchantUI;
    public GameObject questPage;
    public int index;

    public void ButtonClick()
    {
        if (GameManager.PauseAmount > 0)
            return;

        merchantUI.RenderQuest(index);
        Destroy(questPage);
    }

}
