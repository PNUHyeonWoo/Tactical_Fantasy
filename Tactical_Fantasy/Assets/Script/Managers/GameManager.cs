using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager = null;
    public static int PauseAmount = 0;

    public static SaveManager saveManager;
    public static PlusContainerManager nonSavePLCManager;
    public static ItemManager itemManager;
    public static StringManager stringManager;
    public static Player player;
    public static MousePointer mousePointer;
    public static GameObject itemData;
    public static CameraManager cameraManager;
    public Material blackSightMaterial;

    static float hp;
    static int[] firemode = new int[3] { 0, 0, 0 };
    static int selectgun = 0;
    static int middlegun = -1;

    static float blood;
    static float poison;

    private void Awake()
    {
      if(gameManager == null)
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);
            GameManagerStart();
        }
        else
        {
            if(gameManager != this)
            {
                gameManager.GameManagerReStart();
                Destroy(gameObject);
            }
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void GameManagerStart()
    {
        RectTransform canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        canvas.sizeDelta = new Vector2(960, 540);
        canvas.localScale = new Vector3(1, 1, 1);
        saveManager = gameObject.transform.Find("SaveManager").GetComponent<SaveManager>();
        nonSavePLCManager = gameObject.transform.Find("ItemManager/NonSaveItems").GetComponent<NonSaveItems>().nonSavePLC;
        itemManager = gameObject.transform.Find("ItemManager").GetComponent<ItemManager>();
        stringManager = gameObject.transform.Find("StringManager").GetComponent<StringManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        mousePointer = GameObject.Find("MousePointer").GetComponent<MousePointer>();
        itemData = GameObject.Find("ItemManager/ItemData").gameObject;      

        saveManager.StartManager();
        itemManager.StartManager();
        stringManager.StartManager();
        player.StartPlayer();
        cameraManager = itemManager.mainCamera.GetComponent<CameraManager>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        saveManager.playerGear.RenderingPlayerGear();

        if (saveManager.eventFlag[0] == 0)
            InitializeGame();
    }

    public void GameManagerReStart()
    {
        RectTransform canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        canvas.sizeDelta = new Vector2(960, 540);
        canvas.localScale = new Vector3(1, 1, 1);

        nonSavePLCManager.availability = "0";
        nonSavePLCManager.plusContainers.Clear();
        player = GameObject.Find("Player").GetComponent<Player>();
        mousePointer = GameObject.Find("MousePointer").GetComponent<MousePointer>();

        saveManager.ReStartManager();
        itemManager.ReStartManager();
        stringManager.RestartManager();
        player.StartPlayer();
        player.HP = hp;
        player.selectGun = selectgun;
        player.middleGun = middlegun;
        player.main1FireMode = firemode[0];
        player.main2FireMode = firemode[1];
        player.pistolFireMode = firemode[2];
        cameraManager = itemManager.mainCamera.GetComponent<CameraManager>();

        player.blood = blood;
        player.poison = poison;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        saveManager.playerGear.RenderingPlayerGear();
    }

    public void InitializeGame()
    {
        saveManager.eventFlag[0] = 1;
    }

    public static void GamePause()
    {
        PauseAmount++;
        Time.timeScale = 0;
    }

    public static void GamePlay()
    {
        PauseAmount--;
        if(PauseAmount <= 0)
        Time.timeScale = 1;
    }

    public static void SceneMove(string name)
    {
        hp = player.HP;
        selectgun = player.selectGun;
        middlegun = player.middleGun;
        firemode[0] = player.main1FireMode;
        firemode[1] = player.main2FireMode;
        firemode[2] = player.pistolFireMode;

        blood = player.blood;
        poison = player.poison;

        SceneManager.LoadScene(name);

    }

    public static void MoveToMainMenu(string name)
    {
        GameObject destroyObj = gameManager.gameObject;
        gameManager = null;
        PauseAmount = 0;

        saveManager = null;
        nonSavePLCManager = null;
        itemManager = null;
        stringManager = null;
        player = null;
        mousePointer = null;
        itemData = null;
        cameraManager = null;

        Destroy(destroyObj);
        SceneManager.LoadScene(name);
    }


}
