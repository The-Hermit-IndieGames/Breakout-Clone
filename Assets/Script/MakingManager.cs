using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

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
        //單一關卡資料: 關卡名稱、前置關卡ID、遊戲模式(Normal、?、?)、選關按鈕座標(x、y)、選關按鈕風格、磚塊列表(BricksData)
        public string levelName;
        public int[] preLevelID;
        public string gameType;
        public float menuX;
        public float menuY;
        public int menuStyle;
        public List<BricksData> bricksData;
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
    private List<GameObject> bricksInScene = new List<GameObject>();                        //場景中的磚塊列表
    [SerializeField] private Transform bricksList;                                          //磚塊列表(坐標系)

    private BrickMake nowBrick;

    //icon
    [SerializeField] private GameObject[] powerUpIcons;

    //舊版 需清理!!!  DefaultLevels JSON配置文件
    [SerializeField] private Root defaultLevelsData;

    //JSON配置文件
    [SerializeField] private Root rootLevelsData;
    [SerializeField] private LevelConfig nowLevelsData;
    [SerializeField] private List<BricksData> bricksDataList;

    //輸入項目
    public TMP_InputField inputFileName;
    private string fileName;
    public TMP_InputField inputMakerName;
    private string makerName;
    public TMP_InputField inputVersion;
    private string version;
    public TMP_InputField inputDescription;
    private string description;

    public TMP_InputField inputSearchFileName;
    private string searchFileName;

    public TMP_InputField inputNowLevelName;
    private string nowLevelName;
    public TMP_InputField inputPrerequisitesLevel;
    private string prerequisitesLevel;

    [SerializeField] private GameObject warningCanva;
    [SerializeField] private TextMeshProUGUI warningTextEN;
    [SerializeField] private TextMeshProUGUI warningTextCH;


    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        jsonDefaultLevels = Resources.Load<TextAsset>("Data/DefaultLevels");
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

                    //輸入條
                    textBrickLevel.text = brick.brickLevel.ToString();
                    inputBrickLevel.text = "";
                    inputPointValue.text = "";

                    ItemButtonUpdate();
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
                ItemButtonUpdate();
            }
        }
    }

    //道具按鈕-修改圖示
    void ItemButtonUpdate()
    {
        //修改圖示
        if (nowBrick.powerUpType > 5 || nowBrick.powerUpType < 0)
        {
            Debug.LogWarning("未知的Item類型: " + nowBrick.powerUpType);
        }
        else
        {
            powerUpIcons[0].gameObject.SetActive(false);
            powerUpIcons[1].gameObject.SetActive(false);
            powerUpIcons[2].gameObject.SetActive(false);
            powerUpIcons[3].gameObject.SetActive(false);
            powerUpIcons[4].gameObject.SetActive(false);
            powerUpIcons[5].gameObject.SetActive(false);

            powerUpIcons[nowBrick.powerUpType].gameObject.SetActive(true);
        }
    }


    ////匯出 JSON 按鈕(預設關卡專用!!)
    //public void ExportDefaultLevels()
    //{
    //    if (defaultLevelsData.levelConfig.Count == 0)
    //    {
    //        Debug.Log("實例為空 呼叫LoadDefaultLevels");
    //        LoadDefaultLevels();
    //    }


    //    // 創建一個新的LevelConfig對象
    //    LevelConfig levelConfig = new LevelConfig();

    //    levelConfig.levelName = "Level." + levelsName;
    //    levelConfig.gameType = "Time";
    //    levelConfig.menuX = -1.0f;
    //    levelConfig.menuY = -1.0f;
    //    levelConfig.menuStyle = -1;
    //    levelConfig.bricksData = new List<BricksData>();

    //    // 收集場景中的磚塊數據
    //    foreach (GameObject brick in bricksInScene)
    //    {
    //        // 獲取磚塊腳本
    //        var brickScript = brick.GetComponent<BrickMake>();
    //        if (brickScript.brickLevel != 0)
    //        {
    //            BricksData brickData = new BricksData();
    //            brickData.xPoint = Mathf.RoundToInt((26 - brick.transform.position.x) / 4);
    //            brickData.yPoint = Mathf.RoundToInt(24.5f - brick.transform.position.y);
    //            brickData.normalBricks.brickLevel = brickScript.brickLevel;
    //            brickData.normalBricks.powerUpType = brickScript.powerUpType;

    //            levelConfig.bricksData.Add(brickData);
    //        }
    //    }

    //    //比較 levelName 檢測是否出現重複
    //    for (int i = 0; i < defaultLevelsData.levelConfig.Count; i++)
    //    {
    //        if (defaultLevelsData.levelConfig[i].levelName == ("Level." + levelsName))
    //        {
    //            //出現重複 --> 取代
    //            defaultLevelsData.levelConfig[i] = levelConfig;
    //            break;
    //        }
    //        else if (i == (defaultLevelsData.levelConfig.Count - 1))
    //        {
    //            //無重複 --> 添加LevelConfig到Root
    //            defaultLevelsData.levelConfig.Add(levelConfig);
    //        }
    //    }

    //    if (defaultLevelsData.levelConfig.Count == 0)
    //    {
    //        //空集合時 --> 添加LevelConfig到Root
    //        defaultLevelsData.levelConfig.Add(levelConfig);
    //    }

    //    // 轉換為JSON字符串
    //    string defaultLevelsDataJson = JsonUtility.ToJson(defaultLevelsData, true);

    //    // 確保目錄存在
    //    string directoryPath = Application.dataPath + "/Resources/Data/";
    //    Directory.CreateDirectory(directoryPath);

    //    // 寫入 JSON 到檔案
    //    string filePath = directoryPath + "DefaultLevels.json";
    //    File.WriteAllText(filePath, defaultLevelsDataJson);

    //    Debug.Log("關卡配置已保存到 " + filePath);

    //    // 重新載入資源
    //    Resources.UnloadAsset(Resources.Load("Data/DefaultLevels"));

    //    // 載入新的資源
    //    TextAsset newJsonFile = Resources.Load<TextAsset>("Data/example");
    //}


    ////匯入 JSON 按鈕(預設關卡專用!!)
    //public void LoadDefaultLevels()
    //{
    //    if (jsonDefaultLevels != null)
    //    {
    //        // 轉換 JSON 字串為對應的 Root 實例
    //        defaultLevelsData = JsonUtility.FromJson<Root>(jsonDefaultLevels.text);

    //        if (defaultLevelsData != null)
    //        {
    //            Debug.Log("已成功載入設定");
    //        }
    //        else
    //        {
    //            Debug.LogWarning("無法載入 JSON 配置文件，重新建立");
    //            // 創建一個新的Root對象
    //            defaultLevelsData = new Root();
    //            defaultLevelsData.levelConfig = new List<LevelConfig>();
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("未找到 JSON 配置文件，重新建立");
    //        // 創建一個新的Root對象
    //        defaultLevelsData = new Root();
    //        defaultLevelsData.levelConfig = new List<LevelConfig>();
    //    }
    //}



    //新版UI ==============================================================================================================================


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
        rootLevelsData.name = fileName;
        rootLevelsData.maker = makerName;
        rootLevelsData.version = version;
        rootLevelsData.description = description;
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
        string filePath = Path.Combine(directoryPath, fileName + ".json");
        File.WriteAllText(filePath, rootLevelsDataJson);

        Debug.Log("關卡配置已保存到 " + filePath);
    }


    //檔案配置板塊-搜尋 JSON 按鈕
    public void SearchLevelsJson()
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


    //檔案配置板塊-輸入條
    public void OnInputFileName()
    {
        fileName = inputFileName.text;
    }

    public void OnInputMakerName()
    {
        makerName = inputMakerName.text;
    }

    public void OnInputVersion()
    {
        version = inputVersion.text;
    }

    public void OnInputDescription()
    {
        description = inputDescription.text;
    }

    public void OnInputSearchFileName()
    {
        searchFileName = inputSearchFileName.text;
    }



    //關卡配置板塊-新增-----------------------------------------------------------------------------------------------------
    public void MapAddLevel()
    {

    }

    //關卡配置板塊-讀取
    public void MapLoadLevel()
    {

    }

    //關卡配置板塊-保存
    public void MapSaveLevel()
    {

    }

    //關卡配置板塊-刪除
    public void MapDeleteLevel()
    {

    }


    //關卡配置板塊-輸入關卡名稱
    public void MapLevelNameInput()
    {
        nowLevelName = inputNowLevelName.text;
    }


    //關卡配置板塊-按鈕樣式
    public void MapButtonType()
    {

    }


    //關卡配置板塊-前置關卡-輸入-------------------------------------------------------------------------------------------
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
        ShowWarningCanva("BricksRestart", "OOOOO");
    }
    public void BricksRestartNext()
    {
        BricksDelete();
        BricksGenerateNew();
    }

    //磚塊配置板塊-載入磚塊
    public void BricksLoad()
    {
        BricksDelete();
        BricksGenerate();
    }

    //磚塊配置板塊-儲存磚塊
    public void BricksSave()
    {
        BricksAnalyze();
    }

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

    //新版操作 ==============================================================================================================================

    //磚塊生成器
    void BricksGenerate()
    {
        if (bricksDataList.Count == 0)
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

            foreach (var brickData in bricksDataList)
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
                    else
                    {
                        brickData.brickType = "Null";
                    }
                    break;
                case 1:
                    brickData.brickType = "Unbreakable";

                    bricksDataList.Add(brickData);
                    break;
                default:
                    Debug.LogWarning("未知的磚塊類型:" + brickScript.brickType + "  ( " + brickScript.xPoint + " , " + brickScript.yPoint + " )");
                    brickData.brickType = "Null";
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
}