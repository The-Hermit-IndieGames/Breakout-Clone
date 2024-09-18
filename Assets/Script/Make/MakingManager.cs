using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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
        public int idCounter;
    }

    [Serializable]
    public class LevelConfig
    {
        //單一關卡資料: 關卡名稱、前置關卡ID、遊戲模式(Normal、?、?)、選關按鈕座標(x、y)、選關按鈕風格、磚塊列表(BricksData)
        public string levelID;
        public string levelName;
        public string[] preLevelID;
        public string nextLevelID;
        public string gameType;
        public float menuX;
        public float menuY;
        public int menuStyle;
        public List<BricksData> bricksData;
        public InitialItem initialItem;
    }

    [Serializable]
    public class BricksData
    {
        //單一磚塊資料: 磚塊座標(x、y)、磚塊類型(Null、Normal、Unbreakable)、磚塊類型特有資料
        public int xPoint;
        public int yPoint;
        public string brickType;
        public NormalBricks normalBricks;
    }

    [Serializable]
    public class NormalBricks
    {
        //普通磚塊資料: 級別、道具
        public int brickLevel;
        public int powerUpType;
    }

    [Serializable]
    public class InitialItem
    {
        //初始道具
        public bool addBall;
        public bool longPaddle;
        public bool burstBall;
        public bool blackHole;
        public bool burstPaddle;
    }


    private MainManager mainManager;

    //預置體
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private GameObject brickUnbreakablePrefab;

    //JSON配置文件
    [SerializeField] private Root rootLevelsData;
    [SerializeField] private LevelConfig nowLevelsData;
    [SerializeField] private List<BricksData> bricksDataList;

    [SerializeField] private GameObject warningCanva;
    [SerializeField] private TextMeshProUGUI warningTextEN;
    [SerializeField] private TextMeshProUGUI warningTextCH;

    //運行
    private List<GameObject> buttonInMap = new List<GameObject>();                          //地圖中的按鈕列表
    private List<GameObject> bricksInScene = new List<GameObject>();                        //場景中的磚塊列表

    [SerializeField] private Transform buttonsMap;                                          //按鈕列表(坐標系)
    [SerializeField] private Transform bricksList;                                          //磚塊列表(坐標系)

    private BrickMake nowBrick;


    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        StartCoroutine(StartAfterAllObjectsLoaded());
    }

    IEnumerator StartAfterAllObjectsLoaded()
    {
        // 等待1秒，所有物件加載完成後執行的代碼
        yield return new WaitForSeconds(1);

        AdsPlatformIntegration.AdBanner_Hide();
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
                //指定nowBrick用於後續更改
                nowBrick = brick;

                if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
                {
                    //修改方塊類型
                    brick.brickType += 1;
                    if (brick.brickType >= 2)
                    {
                        brick.brickType = 0;
                    }
                    brick.UpdateBrickColor();
                }
                else
                {
                    //修改方塊等級、更新 brickLevel
                    brick.brickLevel += 1;
                    if (brick.brickLevel >= 6)
                    {
                        brick.brickLevel = 0;
                    }
                    brick.UpdateBrickColor();
                }
            }
        }
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
            }
        }
    }



    //新版UI ==============================================================================================================================

    //警告畫面--------------------------------------------------------------------------------------------------------------

    //定義一個委託類型，用於表示沒有參數和沒有返回值的方法
    delegate void Action();

    //儲存後續動作的委託變數
    private Action nextAction;

    //顯示UI
    void ShowWarningCanva(string textEN, string textZH)
    {
        warningCanva.SetActive(true);
        warningTextEN.text = textEN;
        warningTextCH.text = textZH;
    }

    // UI選擇回傳 YES/NO
    public void WarningCanvaYes()
    {
        warningCanva.SetActive(false);
        if (nextAction != null)
        {
            nextAction();
            nextAction = null;
        }
    }

    public void WarningCanvaNo()
    {
        warningCanva.SetActive(false);
        nextAction = null;
    }



    //主板塊-退出按鈕 ---------------------------------------------------------------------------------------------------------
    public void BackButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        // 載入 MenuScene
        SceneManager.LoadScene("MenuScene");
    }



    //檔案配置板塊-新增 JSON 按鈕 ---------------------------------------------------------------------------------------------
    public void NewLevelsJson()
    {
        rootLevelsData = new Root();
    }


    //檔案配置板塊-更新 JSON 資料
    public void UpdateLevelsJson()
    {
        //資料階層-檔案資訊
        OnInputFileName();
        OnInputMakerName();
        OnInputVersion();
        OnInputDescription();
    }

    //關卡配置比對保存(rootLevelsData.levelConfig << nowLevelsData)
    public void SaveLevelToRoot()
    {
        if (nowLevelsData.levelID != null)
        {
            bool haveData = false;

            foreach (var levelConfig in rootLevelsData.levelConfig)
            {
                if (nowLevelsData.levelID == levelConfig.levelID)
                {
                    haveData = true;
                    levelConfig.levelName = nowLevelsData.levelName;
                    levelConfig.preLevelID = nowLevelsData.preLevelID;
                    levelConfig.gameType = nowLevelsData.gameType;
                    levelConfig.menuX = nowLevelsData.menuX;
                    levelConfig.menuY = nowLevelsData.menuY;
                    levelConfig.menuStyle = nowLevelsData.menuStyle;
                }
            }

            if (haveData == false)
            {
                rootLevelsData.levelConfig.Add(nowLevelsData);
            }
        }
    }

    //關卡配置比對載入(rootLevelsData.levelConfig >> nowLevelsData)
    public void LoadRootToLevel(string id)
    {
        foreach (var levelConfig in rootLevelsData.levelConfig)
        {
            if (levelConfig.levelID == id)
            {
                nowLevelsData = levelConfig;
            }
        }
    }


    //檔案配置板塊-匯出 JSON 按鈕
    public void ExportLevelsJson()
    {
        UpdateLevelsJson();

        // 轉換為JSON字符串
        string rootLevelsDataJson = JsonUtility.ToJson(rootLevelsData, true);

        // 確保目錄存在
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "CustomLevels");
        Directory.CreateDirectory(directoryPath);

        // 寫入 JSON 到檔案
        string filePath = Path.Combine(directoryPath, rootLevelsData.name + ".json");
        File.WriteAllText(filePath, rootLevelsDataJson);

        Debug.Log("關卡配置已保存到 " + filePath);
    }


    //檔案配置板塊-搜尋 JSON 按鈕
    public void SearchLevelsJson()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerData", "CustomLevels", searchFileName + ".json");

        try
        {
            if (File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);
                rootLevelsData = JsonUtility.FromJson<Root>(jsonContent);

                LoadJsonToMap();
                Debug.Log("Successfully loaded JSON file: " + jsonContent);
            }
            else
            {
                Debug.LogWarning("File not found: " + filePath);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Error loading JSON file: " + e.Message);
        }
    }


    //檔案配置板塊-解析 JSON 至UI
    private void LoadJsonToMap()
    {
        if (rootLevelsData != null)
        {
            inputFileName.text = rootLevelsData.name;
            inputMakerName.text = rootLevelsData.maker;
            inputVersion.text = rootLevelsData.version;
            inputDescription.text = rootLevelsData.description;

            foreach (var button in buttonInMap)
            {
                Destroy(button.gameObject);
            }
            buttonInMap.Clear();

            foreach (var levelConfig in rootLevelsData.levelConfig)
            {
                GameObject button = Instantiate(buttonPrefab, Vector2.zero, Quaternion.identity, buttonsMap);
                buttonInMap.Add(button);

                Vector2 buttonPosition = new Vector2(levelConfig.menuX, levelConfig.menuY);
                button.transform.localPosition = buttonPosition;

                var buttonScript = button.GetComponent<MapLevelButton>();
                if (buttonScript != null)
                {
                    buttonScript.levelID = levelConfig.levelID;
                }
            }

            nowButton = buttonInMap[0];
            nowLevelsData = rootLevelsData.levelConfig[0];

            Debug.Log("檔案: " + rootLevelsData.name + ".json 載入完成!");
        }
    }



    //輸入項目
    public TMP_InputField inputFileName;
    public TMP_InputField inputMakerName;
    public TMP_InputField inputVersion;
    public TMP_InputField inputDescription;

    public TMP_InputField inputSearchFileName;
    private string searchFileName;


    //檔案配置板塊-輸入條
    public void OnInputFileName()
    {
        rootLevelsData.name = inputFileName.text;
    }

    public void OnInputMakerName()
    {
        rootLevelsData.maker = inputMakerName.text;
    }

    public void OnInputVersion()
    {
        rootLevelsData.version = inputVersion.text;
    }

    public void OnInputDescription()
    {
        rootLevelsData.description = inputDescription.text;
    }

    public void OnInputSearchFileName()
    {
        searchFileName = inputSearchFileName.text;
    }



    //關卡配置板塊----------------------------------------------------------------------------------------------------------

    public GameObject nowButton;

    public TMP_InputField inputNowLevelName;
    public TMP_InputField inputPrerequisitesLevel;
    private string prerequisitesLevel;

    //Map-新增關卡
    public void MapAddLevel()
    {
        Vector2 buttonPosition = new Vector2(0, 10);
        GameObject button = Instantiate(buttonPrefab, buttonPosition, Quaternion.identity, buttonsMap);
        buttonInMap.Add(button);
        nowButton = button;

        nowLevelsData = new LevelConfig();
        nowLevelsData.levelID = GenerateID(rootLevelsData.idCounter);
        rootLevelsData.idCounter++;

        nowLevelsData.levelName = "Null";
        nowLevelsData.gameType = "Normal";
        nowLevelsData.menuStyle = 0;

        var buttonScript = button.GetComponent<MapLevelButton>();
        if (buttonScript != null)
        {
            buttonScript.GetMakingManager();
            buttonScript.levelID = nowLevelsData.levelID;
        }

        LoadRootToLevel(nowLevelsData.levelID);
        UpDataMapLevel();
    }

    private string GenerateID(int id)
    {
        // 使用字串格式化將整數 ID 轉換為帶有固定位數的 ID 字串
        return $"ID_{id:D4}";
    }

    public string GetLevelName(string id)
    {
        string name = "Null";

        foreach (var levelConfig in rootLevelsData.levelConfig)
        {
            if (id == levelConfig.levelID)
            {
                name = levelConfig.levelName;
            }
        }
        return name;
    }

    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private TextMeshProUGUI nextIdText;
    [SerializeField] private TMP_InputField inputCoordinateX;
    [SerializeField] private TMP_InputField inputCoordinateY;
    [SerializeField] private TextMeshProUGUI tempBricksText;

    //更新畫面資訊
    public void UpDataMapLevel()
    {
        idText.text = nowLevelsData.levelID;
        inputNowLevelName.text = nowLevelsData.levelName;
        nextIdText.text = nowLevelsData.nextLevelID;
        inputCoordinateX.text = nowLevelsData.menuX.ToString();
        inputCoordinateY.text = nowLevelsData.menuY.ToString();

        tempBricksText.text = ("Temp\n " + bricksDataList.Count + "\nBricks");
        initialItemToggle[1].isOn = nowLevelsData.initialItem.addBall;
        initialItemToggle[2].isOn = nowLevelsData.initialItem.longPaddle;
        initialItemToggle[3].isOn = nowLevelsData.initialItem.burstBall;
        initialItemToggle[4].isOn = nowLevelsData.initialItem.blackHole;
        initialItemToggle[5].isOn = nowLevelsData.initialItem.burstPaddle;

        int bricksCount;
        if (nowLevelsData.bricksData == null)
        {
            bricksCount = 0;
        }
        else
        {
            var bricks = nowLevelsData.bricksData;
            bricksCount = bricks.Count;
        }

        var buttonScript = nowButton.GetComponent<MapLevelButton>();
        if (buttonScript != null)
        {
            buttonScript.UpdateButtonName();
        }
    }

    //關卡配置板塊-讀取
    public void MapLoadLevel(string id)
    {
        SaveLevelToRoot();
        LoadRootToLevel(id);

        //更新畫面資訊
        UpDataMapLevel();
    }

    //關卡配置板塊-複製ID
    public void CopyId()
    {
        GUIUtility.systemCopyBuffer = nowLevelsData.levelID;
        Debug.Log("Copy ID : " + nowLevelsData.levelID);
    }

    //關卡配置板塊-貼上ID (下一關)
    public void PasteId()
    {
        nowLevelsData.nextLevelID = GUIUtility.systemCopyBuffer;
        nextIdText.text = nowLevelsData.nextLevelID;

        SaveLevelToRoot();
    }

    //關卡配置板塊-刪除關卡
    public void MapDeleteLevel()
    {
        nextAction = MapDeleteLevelNext;
        ShowWarningCanva("Delete The Level ?", "OOOOO");
    }

    public void MapDeleteLevelNext()
    {
        string id = nowLevelsData.levelID;
        Destroy(nowButton);
        nowButton = buttonInMap[0];
        nowLevelsData = rootLevelsData.levelConfig[0];
        rootLevelsData.levelConfig.RemoveAll(config => config.levelID == id);

        nextAction = null;
    }

    //關卡配置板塊-輸入關卡名稱
    public void MapLevelNameInput()
    {
        nowLevelsData.levelName = inputNowLevelName.text;

        SaveLevelToRoot();
        UpDataMapLevel();
    }


    //關卡配置板塊-按鈕樣式
    public void MapButtonType()
    {

    }

    //關卡配置板塊-輸入座標
    public void CoordinateXInput()
    {
        float value;
        float.TryParse(inputCoordinateX.text, out value);
        nowLevelsData.menuX = value;

        var script = nowButton.GetComponent<MapLevelButton>();
        script.UpDataCoordinate(new Vector2(nowLevelsData.menuX, nowLevelsData.menuY));

        SaveLevelToRoot();
    }

    public void CoordinateYInput()
    {
        float value;
        float.TryParse(inputCoordinateY.text, out value);
        nowLevelsData.menuY = value;

        var script = nowButton.GetComponent<MapLevelButton>();
        script.UpDataCoordinate(new Vector2(nowLevelsData.menuX, nowLevelsData.menuY));

        SaveLevelToRoot();
    }


    //關卡配置板塊-地圖-更新座標
    public void UpDataPosition(string id, Vector2 position)
    {
        MapLoadLevel(id);
        nowLevelsData.menuX = position.x;
        nowLevelsData.menuY = position.y;

        inputCoordinateX.text = nowLevelsData.menuX.ToString();
        inputCoordinateY.text = nowLevelsData.menuY.ToString();

        SaveLevelToRoot();
    }


    //關卡配置板塊-前置關卡-輸入---------------------------
    public void PrerequisitesInput()
    {

    }

    //關卡配置板塊-前置關卡-加入
    public void PrerequisitesAdd()
    {

    }

    //關卡配置板塊-前置關卡-選擇
    public void PrerequisitesDropdown()
    {

    }

    //關卡配置板塊-前置關卡-刪除
    public void PrerequisitesDelete()
    {

    }





    //磚塊配置板塊-重製磚塊-------------------------------------------------------------------------------------------
    public void BricksRestart()
    {
        nextAction = BricksRestartNext;
        ShowWarningCanva("Restart Bricks ?", "確定要重置畫面中的磚塊嗎?");
    }

    public void BricksRestartNext()
    {
        BricksDelete();
        BricksGenerateNew();

        nextAction = null;
    }

    //磚塊配置板塊-載入磚塊
    public void BricksLoadByTemp()
    {
        tempBricksText.text = ("Temp\n " + bricksDataList.Count + "\nBricks");

        BricksDelete();
        BricksGenerate(bricksDataList);
    }

    //磚塊配置板塊-儲存磚塊
    public void BricksSaveToTemp()
    {
        BricksAnalyze();
        tempBricksText.text = ("Temp\n " + bricksDataList.Count + "\nBricks");
        Debug.Log("BricksSave: 解析磚塊並保存至 bricksDataList");
    }



    //磚塊配置板塊-載入磚塊
    public void TempLoadByLevel()
    {
        //避免發生傳址呼叫
        if (nowLevelsData.bricksData == null)
        {
            bricksDataList = new List<BricksData>();
        }
        else
        {
            bricksDataList = new List<BricksData>(nowLevelsData.bricksData);
        }

        tempBricksText.text = ("Temp\n " + bricksDataList.Count + "\nBricks");
    }

    //磚塊配置板塊-儲存磚塊
    public void TempSaveToLevel()
    {
        //避免發生傳址呼叫
        nowLevelsData.bricksData = new List<BricksData>(bricksDataList);

        foreach (var levelConfig in rootLevelsData.levelConfig)
        {
            if (nowLevelsData.levelID == levelConfig.levelID)
            {
                levelConfig.bricksData = nowLevelsData.bricksData;
            }
        }

        tempBricksText.text = ("Temp\n " + bricksDataList.Count + "\nBricks");
    }


    //磚塊生成器(讀取)
    void BricksGenerate(List<BricksData> bricksDatas)
    {
        if (bricksDatas == null)
        {
            for (int x = 1; x <= 12; x++)
            {
                for (int y = 1; y <= 16; y++)
                {
                    Vector3 brickPosition = new Vector3(26 - (4 * x), 24.5f - y, 0);
                    GameObject brick = Instantiate(brickPrefab, brickPosition, Quaternion.identity, bricksList);
                    bricksInScene.Add(brick);

                    // 設置磚塊的屬性
                    var brickScript = brick.GetComponent<BrickMake>();
                    if (brickScript != null)
                    {
                        brickScript.xPoint = x;
                        brickScript.yPoint = y;
                        brickScript.brickType = 0;
                        brickScript.brickLevel = 0;
                        brickScript.powerUpType = 0;
                    }
                }
            }
        }
        else
        {
            List<int> intList = new List<int>();
            for (int x = 1; x <= 12; x++)
            {
                for (int y = 1; y <= 16; y++)
                {
                    intList.Add(x * 100 + y);
                }
            }

            foreach (var brickData in bricksDatas)
            {
                Vector3 brickPosition = new Vector3(26 - (4 * brickData.xPoint), 24.5f - brickData.yPoint, 0);
                GameObject brick = Instantiate(brickPrefab, brickPosition, Quaternion.identity, bricksList);
                bricksInScene.Add(brick);

                // 設置磚塊的屬性
                var brickScript = brick.GetComponent<BrickMake>();
                if (brickScript != null)
                {
                    brickScript.xPoint = brickData.xPoint;
                    brickScript.yPoint = brickData.yPoint;

                    switch (brickData.brickType)
                    {
                        case "Normal":
                            brickScript.brickType = 0;
                            brickScript.brickLevel = brickData.normalBricks.brickLevel;
                            brickScript.powerUpType = brickData.normalBricks.powerUpType;

                            intList.RemoveAll(item => item == (brickData.xPoint * 100 + brickData.yPoint));
                            break;

                        case "Unbreakable":
                            brickScript.brickType = 1;

                            intList.RemoveAll(item => item == (brickData.xPoint * 100 + brickData.yPoint));
                            break;

                        default:
                            Debug.LogWarning("未知的磚塊類型:" + brickData.brickType + "  ( " + brickData.xPoint + " , " + brickData.yPoint + " )");
                            break;
                    }
                }
            }

            foreach (var noData in intList)
            {
                int x = noData / 100;
                int y = noData % 100;

                Vector3 brickPosition = new Vector3(26 - (4 * x), 24.5f - y, 0);
                GameObject brick = Instantiate(brickPrefab, brickPosition, Quaternion.identity, bricksList);
                bricksInScene.Add(brick);

                // 設置磚塊的屬性
                var brickScript = brick.GetComponent<BrickMake>();
                if (brickScript != null)
                {
                    brickScript.xPoint = x;
                    brickScript.yPoint = y;
                    brickScript.brickType = 0;
                    brickScript.brickLevel = 0;
                    brickScript.powerUpType = 0;
                }
            }
        }

    }


    //磚塊生成器(新)
    void BricksGenerateNew()
    {
        for (int x = 1; x <= 12; x++)
        {
            for (int y = 1; y <= 16; y++)
            {
                Vector3 brickPosition = new Vector3(26 - (4 * x), 24.5f - y, 0);
                GameObject brick = Instantiate(brickPrefab, brickPosition, Quaternion.identity, bricksList);
                bricksInScene.Add(brick);

                // 設置磚塊的屬性
                var brickScript = brick.GetComponent<BrickMake>();
                if (brickScript != null)
                {
                    brickScript.xPoint = x;
                    brickScript.yPoint = y;
                    brickScript.brickType = 0;
                    brickScript.brickLevel = 0;
                    brickScript.powerUpType = 0;
                }
            }
        }
    }


    //磚塊解析器
    void BricksAnalyze()
    {
        bricksDataList.Clear();

        // 收集場景中的磚塊數據
        foreach (GameObject brick in bricksInScene)
        {
            var brickScript = brick.GetComponent<BrickMake>();
            BricksData brickData = new BricksData();

            brickData.xPoint = brickScript.xPoint;
            brickData.yPoint = brickScript.yPoint;

            switch (brickScript.brickType)
            {
                case 0:
                    if (brickScript.brickLevel >= 1)
                    {
                        brickData.brickType = "Normal";

                        NormalBricks normalBricks = new NormalBricks();
                        normalBricks.brickLevel = brickScript.brickLevel;
                        normalBricks.powerUpType = brickScript.powerUpType;
                        brickData.normalBricks = normalBricks;

                        bricksDataList.Add(brickData);
                    }
                    break;
                case 1:
                    brickData.brickType = "Unbreakable";

                    bricksDataList.Add(brickData);
                    break;
                default:
                    Debug.LogWarning("未知的磚塊類型:" + brickScript.brickType + "  ( " + brickScript.xPoint + " , " + brickScript.yPoint + " )");
                    break;
            }

        }
    }


    //磚塊清除器
    void BricksDelete()
    {
        // 遍歷 Transform 中的每個子物件
        for (int i = 0; i < bricksList.childCount; i++)
        {
            //獲取子物件的 Transform
            Transform childTransform = bricksList.GetChild(i);

            //獲取子物件的 GameObject
            GameObject childGameObject = childTransform.gameObject;

            Destroy(childGameObject);
            bricksInScene.Clear();
        }
    }



    //初始道具--------------------------------------------------------------------------------------------------------


    [SerializeField] private List<Toggle> initialItemToggle;


    public void SetInitialItem_1()
    {
        var newInitialItem = nowLevelsData.initialItem;
        newInitialItem.addBall = initialItemToggle[1].isOn;

        SaveLevelToRoot();
    }

    public void SetInitialItem_2()
    {
        var newInitialItem = nowLevelsData.initialItem;
        newInitialItem.longPaddle = initialItemToggle[2].isOn;

        SaveLevelToRoot();
    }

    public void SetInitialItem_3()
    {
        var newInitialItem = nowLevelsData.initialItem;
        newInitialItem.burstBall = initialItemToggle[3].isOn;

        SaveLevelToRoot();
    }

    public void SetInitialItem_4()
    {
        var newInitialItem = nowLevelsData.initialItem;
        newInitialItem.blackHole = initialItemToggle[4].isOn;

        SaveLevelToRoot();
    }

    public void SetInitialItem_5()
    {
        var newInitialItem = nowLevelsData.initialItem;
        newInitialItem.burstPaddle = initialItemToggle[5].isOn;

        SaveLevelToRoot();
    }
}