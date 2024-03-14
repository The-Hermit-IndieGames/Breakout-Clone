using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static MainManager;

public class MakingManager : MonoBehaviour
{
    //序列化定義-關卡資料
    [Serializable]
    public class Root
    {
        //根目錄: 目錄名稱、製作者、版本號、描述、關卡列表(LevelConfig)
        public string name;
        public string maker;
        public string version;
        public string description;
        public List<LevelConfig> levelConfig;
    }

    [Serializable]
    public class LevelConfig
    {
        //單一關卡資料: 關卡名稱、前置關卡名稱(NEW)、遊戲模式(Normal、?、?)、選關按鈕座標(x、y)、選關按鈕風格(?)、磚塊列表(BricksData)
        public string levelName;
        public string preLevelName;
        public string gameType;
        public float menuX;
        public float menuY;
        public int menuStyle;
        public List<BricksData> bricksData;
    }

    [Serializable]
    public class BricksData
    {
        //單一磚塊資料: 磚塊座標(x、y)、磚塊類型(Normal、Unbreakable)、磚塊類型特有資料
        public int xPoint;
        public int yPoint;
        public string brickType;
        public NormalBricks normalBricks;

        //刪
        public int pointValue;
        public int brickLevel;
        public int powerUpType;
    }

    [Serializable]
    public class NormalBricks
    {
        //普通磚塊資料: 級別、道具
        public int brickLevel;
        public int powerUpType;
    }

    private MainManager mainManager;

    //檔案
    [SerializeField] private TextAsset jsonDefaultLevels;

    //參數
    public TMP_InputField inputLevelsName;                                  //輸入格-關卡名
    public string levelsName = "Null";                                      //關卡名
    public string exportFileName;                                           //保存的JSON文件名
    public GameObject brickPrefab;                                          //磚塊的預置體
    public TMP_InputField inputBrickLevel;                                  //輸入格-磚塊等級
    public TMP_InputField inputPointValue;                                  //輸入格-破壞分數
    public TextMeshProUGUI textBrickLevel;                                  //數值-磚塊等級
    public TextMeshProUGUI textPointValue;                                  //數值-破壞分數

    //運行
    private Vector3 brickPosition;                                                          //磚塊座標
    [SerializeField] private List<GameObject> bricksInScene = new List<GameObject>();       //場景中的磚塊列表
    [SerializeField] private Transform bricksList;                                          //磚塊列表(坐標系)
    private BrickMake nowBrick;

    //icon
    [SerializeField] private GameObject[] powerUpIcons;

    //DefaultLevels JSON配置文件
    [SerializeField] private Root defaultLevelsData;

    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        jsonDefaultLevels = Resources.Load<TextAsset>("Data/DefaultLevels");

        GenerateBricks();
    }


    void Update()
    {
        // 檢查滑鼠按鍵點擊事件
        if (Input.GetMouseButtonDown(0)) // 左鍵
        {
            HandleLeftClick();
        }
        else if (Input.GetMouseButtonDown(1)) // 右鍵
        {
            HandleRightClick();
        }
    }

    //左鍵
    void HandleLeftClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 使用射線檢測滑鼠點擊的位置
        if (Physics.Raycast(ray, out hit))
        {
            // 檢查是否點擊到了 Brick 物件
            BrickMake brick = hit.collider.GetComponent<BrickMake>();
            if (brick != null)
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
                {
                    // 在這裡觸發你想要執行的操作
                    Debug.Log("Shift + 左鍵被按下！");

                    // 舉例：可以在這裡呼叫其他方法或執行其他代碼
                }
                else
                {
                    //指定nowBrick用於後續更改
                    nowBrick = brick;

                    //更新 brickLevel
                    brick.brickLevel += 1;
                    if (brick.brickLevel >= 6)
                    {
                        brick.brickLevel = 0;
                    }
                    brick.UpdateLevel();

                    //輸入條
                    textBrickLevel.text = brick.brickLevel.ToString();
                    textPointValue.text = brick.pointValue.ToString();
                    inputBrickLevel.text = "";
                    inputPointValue.text = "";

                    ItemButtonUpdate();
                }               
            }
        }
    }

    //輸入條(等級)
    public void OnInputBrickLevel()
    {
        //讀取 TMP Input Field 中的文本
        string inputText = inputBrickLevel.text;
        int.TryParse(inputText, out int inputNumber);

        nowBrick.brickLevel = inputNumber;
        inputPointValue.text = "";
        nowBrick.UpdateLevel();
        textBrickLevel.text = nowBrick.brickLevel.ToString();
        textPointValue.text = nowBrick.pointValue.ToString();
    }

    //輸入條(分數)
    public void OnInputBrickScore()
    {
        //讀取 TMP Input Field 中的文本
        string inputText = inputPointValue.text;
        int.TryParse(inputText, out int inputNumber);

        nowBrick.pointValue = inputNumber;
        textPointValue.text = nowBrick.pointValue.ToString();
    }

    //右鍵
    void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 使用射線檢測滑鼠點擊的位置
        if (Physics.Raycast(ray, out hit))
        {
            // 檢查是否點擊到了 Brick 物件
            BrickMake brick = hit.collider.GetComponent<BrickMake>();
            if (brick != null)
            {
                //指定nowBrick用於後續更改
                nowBrick = brick;

                brick.UpdateItem();     //更新 powerUpType
                ItemButtonUpdate();
            }
        }
    }

    //道具按鈕
    public void OnItemButton()
    {
        nowBrick.UpdateItem();     //更新 powerUpType

        ItemButtonUpdate();
    }

    //道具按鈕-修改圖示
    void ItemButtonUpdate()
    {
        //修改圖示
        switch (nowBrick.powerUpType)
        {
            case 0:
                powerUpIcons[0].gameObject.SetActive(true);
                powerUpIcons[1].gameObject.SetActive(false);
                powerUpIcons[2].gameObject.SetActive(false);
                powerUpIcons[3].gameObject.SetActive(false);
                powerUpIcons[4].gameObject.SetActive(false);
                break;
            case 1:
                powerUpIcons[0].gameObject.SetActive(false);
                powerUpIcons[1].gameObject.SetActive(true);
                powerUpIcons[2].gameObject.SetActive(false);
                powerUpIcons[3].gameObject.SetActive(false);
                powerUpIcons[4].gameObject.SetActive(false);
                break;
            case 2:
                powerUpIcons[0].gameObject.SetActive(false);
                powerUpIcons[1].gameObject.SetActive(false);
                powerUpIcons[2].gameObject.SetActive(true);
                powerUpIcons[3].gameObject.SetActive(false);
                powerUpIcons[4].gameObject.SetActive(false);
                break;
            case 3:
                powerUpIcons[0].gameObject.SetActive(false);
                powerUpIcons[1].gameObject.SetActive(false);
                powerUpIcons[2].gameObject.SetActive(false);
                powerUpIcons[3].gameObject.SetActive(true);
                powerUpIcons[4].gameObject.SetActive(false);
                break; ;
            case 4:
                powerUpIcons[0].gameObject.SetActive(false);
                powerUpIcons[1].gameObject.SetActive(false);
                powerUpIcons[2].gameObject.SetActive(false);
                powerUpIcons[3].gameObject.SetActive(false);
                powerUpIcons[4].gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("未知的Item類型: " + nowBrick.powerUpType);
                break;
        }
    }


    //磚塊生成器
    void GenerateBricks()
    {
        for (int x = 1; x <= 12; x++)
        {
            for (int y = 1; y <= 16; y++)
            {
                brickPosition = new Vector3(26 - (4 * x), 24.5f - y, 0);
                GameObject brick = Instantiate(brickPrefab, brickPosition, Quaternion.identity, bricksList);
                bricksInScene.Add(brick);

                // 設置磚塊的屬性
                var brickScript = brick.GetComponent<Brick>();
                if (brickScript != null)
                {
                    brickScript.pointValue = 0;
                    brickScript.brickLevel = 0;
                    brickScript.powerUpType = 0;
                }
            }
        }
    }


    //重新開始按鈕
    public void RestartButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //輸入條(關卡名)
    public void OnInputLevelsName()
    {
        //讀取 TMP Input Field 中的文本
        levelsName = inputLevelsName.text;
    }


    //匯出 JSON 按鈕
    public void ExportLevelConfigToJson()
    {
        mainManager.soundEffectUiTrue.Play();
        // 創建一個新的Root對象
        Root root = new Root();
        root.levelConfig = new List<LevelConfig>();

        // 創建一個新的LevelConfig對象
        LevelConfig levelConfig = new LevelConfig();

        levelConfig.levelName = "Custom_" + levelsName;
        levelConfig.gameType = "Time";
        levelConfig.menuX = -1.0f;
        levelConfig.menuY = -1.0f;
        levelConfig.menuStyle = -1;
        levelConfig.bricksData = new List<BricksData>();

        // 收集場景中的磚塊數據
        foreach (GameObject brick in bricksInScene)
        {
            // 獲取磚塊腳本
            var brickScript = brick.GetComponent<BrickMake>();
            if (brickScript.brickLevel != 0)
            {
                BricksData brickData = new BricksData();
                brickData.xPoint = Mathf.RoundToInt((26 - brick.transform.position.x) / 4);
                brickData.yPoint = Mathf.RoundToInt(24.5f - brick.transform.position.y);
                brickData.pointValue = brickScript.pointValue;
                brickData.brickLevel = brickScript.brickLevel;
                brickData.powerUpType = brickScript.powerUpType;
                //brickData.brickType = brickScript.brickType;

                levelConfig.bricksData.Add(brickData);
            }
        }

        // 添加LevelConfig到Root
        root.levelConfig.Add(levelConfig);

        // 轉換為JSON字符串
        string json = JsonUtility.ToJson(root, true);

        // 寫入JSON文件
        string fileName = "Resources/PlayerData/CustomLevels/" + levelsName + ".json";

        exportFileName = Path.Combine(Application.dataPath, fileName);

        File.WriteAllText(exportFileName, json);

        Debug.Log("關卡配置已保存到 " + exportFileName);
    }


    //匯出 JSON 按鈕(預設關卡專用!!)
    public void ExportDefaultLevels()
    {
        if (defaultLevelsData.levelConfig.Count == 0)
        {
            Debug.Log("實例為空 呼叫LoadDefaultLevels");
            LoadDefaultLevels();
        }


        // 創建一個新的LevelConfig對象
        LevelConfig levelConfig = new LevelConfig();

        levelConfig.levelName = "Level." + levelsName;
        levelConfig.gameType = "Time";
        levelConfig.menuX = -1.0f;
        levelConfig.menuY = -1.0f;
        levelConfig.menuStyle = -1;
        levelConfig.bricksData = new List<BricksData>();

        // 收集場景中的磚塊數據
        foreach (GameObject brick in bricksInScene)
        {
            // 獲取磚塊腳本
            var brickScript = brick.GetComponent<BrickMake>();
            if (brickScript.brickLevel != 0)
            {
                BricksData brickData = new BricksData();
                brickData.xPoint = Mathf.RoundToInt((26 - brick.transform.position.x) / 4);
                brickData.yPoint = Mathf.RoundToInt(24.5f - brick.transform.position.y);
                brickData.pointValue = brickScript.pointValue;
                brickData.brickLevel = brickScript.brickLevel;
                brickData.powerUpType = brickScript.powerUpType;

                levelConfig.bricksData.Add(brickData);
            }
        }

        //比較 levelName 檢測是否出現重複
        for (int i = 0; i < defaultLevelsData.levelConfig.Count; i++)
        {
            if (defaultLevelsData.levelConfig[i].levelName == ("Level." + levelsName))
            {
                //出現重複 --> 取代
                defaultLevelsData.levelConfig[i] = levelConfig;
                break;
            }
            else if (i == (defaultLevelsData.levelConfig.Count - 1))
            {
                //無重複 --> 添加LevelConfig到Root
                defaultLevelsData.levelConfig.Add(levelConfig);
            }
        }

        if (defaultLevelsData.levelConfig.Count == 0)
        {
            //空集合時 --> 添加LevelConfig到Root
            defaultLevelsData.levelConfig.Add(levelConfig);
        }

        // 轉換為JSON字符串
        string defaultLevelsDataJson = JsonUtility.ToJson(defaultLevelsData, true);

        // 確保目錄存在
        string directoryPath = Application.dataPath + "/Resources/Data/";
        Directory.CreateDirectory(directoryPath);

        // 寫入 JSON 到檔案
        string filePath = directoryPath + "DefaultLevels.json";
        File.WriteAllText(filePath, defaultLevelsDataJson);

        Debug.Log("關卡配置已保存到 " + filePath);

        // 重新載入資源
        Resources.UnloadAsset(Resources.Load("Data/DefaultLevels"));

        // 載入新的資源
        TextAsset newJsonFile = Resources.Load<TextAsset>("Data/example");
    }


    //匯入 JSON 按鈕(預設關卡專用!!)
    public void LoadDefaultLevels()
    {
        if (jsonDefaultLevels != null)
        {
            // 轉換 JSON 字串為對應的 Root 實例
            defaultLevelsData = JsonUtility.FromJson<Root>(jsonDefaultLevels.text);

            if (defaultLevelsData != null)
            {
                Debug.Log("已成功載入設定");
            }
            else
            {
                Debug.LogWarning("無法載入 JSON 配置文件，重新建立");
                // 創建一個新的Root對象
                defaultLevelsData = new Root();
                defaultLevelsData.levelConfig = new List<LevelConfig>();
            }
        }
        else
        {
            Debug.LogWarning("未找到 JSON 配置文件，重新建立");
            // 創建一個新的Root對象
            defaultLevelsData = new Root();
            defaultLevelsData.levelConfig = new List<LevelConfig>();
        }
    }


    //退出按鈕
    public void BackButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        // 載入 MenuScene
        SceneManager.LoadScene("MenuScene");
    }
}
