using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MousePointer : MonoBehaviour
{
    public bool isDirectZoom = true;
    public bool isUpUI = false;

    public Image childImage;

    enum MouseSprite
    {
        DirectZoom = 0,
        AimZoom = 1,
        Hand = 2,
        Catch= 3

    }

    public Sprite[] mouseSprites;
    void Update()
    {
        if (GameManager.itemManager.NowStartType != 0)
        {
            SetCatchCursor();
        }
        else
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetHandCursor();
            }
            else
            {
                if (GameManager.player.isZoom)
                    SetAimZoom();
                else
                    SetDirectZoom();
            }
        }

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.itemManager.canvas, Input.mousePosition, GameManager.itemManager.mainCamera, out mousePos);
        transform.localPosition = new Vector3(mousePos.x, mousePos.y, transform.localPosition.z);
        if(isUpUI)
        transform.SetAsLastSibling();
        else
        transform.SetAsFirstSibling();

        if (isDirectZoom)
        {
            mousePos = GameManager.itemManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.x = mousePos.x - GameManager.player.transform.position.x;
            mousePos.y = mousePos.y - GameManager.player.transform.position.y;

            if (mousePos.x >= 0)
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg - 90);
            else
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg + 90);

            float dis = Vector2.Distance(mousePos, Vector2.zero);

            GetComponent<RectTransform>().sizeDelta = new Vector2(dis / 2, 256);

        }

    }

    public void SetDirectZoom()
    {
        isDirectZoom = true;
        isUpUI = false;

        childImage.sprite = mouseSprites[0];

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.itemManager.canvas, Input.mousePosition, GameManager.itemManager.mainCamera, out mousePos);

        if (mousePos.x >= 0)
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg - 90);
        else
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg + 90);

        float dis = Vector2.Distance(mousePos, Vector2.zero);
        GetComponent<RectTransform>().sizeDelta = new Vector2(dis / 2, 256);

    }

    public void SetAimZoom()
    {
        isDirectZoom = false;
        isUpUI = false;

        childImage.sprite = mouseSprites[1];
        GetComponent<RectTransform>().sizeDelta = new Vector2(256, 256);
        transform.rotation = Quaternion.Euler(0, 0, 0);

    }

    public void SetHandCursor()
    {
        isDirectZoom = false;
        isUpUI = true;
        childImage.sprite = mouseSprites[2];
        GetComponent<RectTransform>().sizeDelta = new Vector2(256, 256);
        transform.rotation = Quaternion.Euler(0, 0, 0);

    }

    public void SetCatchCursor()
    {
        isDirectZoom = false;
        isUpUI = true;
        childImage.sprite = mouseSprites[3];
        GetComponent<RectTransform>().sizeDelta = new Vector2(256, 256);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
