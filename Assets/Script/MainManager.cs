using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


public class MainManager : MonoBehaviour
{
    //序列化==========================================================================================================================

    //序列化定義(設定)
    [Serializable]
    public class SettingRoot
    {
        public int bgmId;
        public bool gameMusic;
        public float gameMusicF;
        public bool gameSoundEffect;
        public float gameSoundEffectF;

        public int effectsVFX;
        public int backgroundVFX;

        public float gameSpeedModifier;
    }

    //序列化定義(通關資訊)
    [Serializable]
    public class ClearDataRoot
    {
        public string name;
        public string version;
        public List<ClearLevel> clearLevel;
    }

    [Serializable]
    public class ClearLevel
    {
        public string levelID;
        public bool clear;
        public ClearData clearData;
    }

    [Serializable]
    public class ClearData
    {
        public int medalLevel;
        public int score;
        public int time;
        public float speed;
    }

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
        public bool hidden;
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

    //靜態==========================================================================================================================

    //檔案
    [SerializeField] private TextAsset jsonDefaultLevels;
    public static Root[] levelConfigFiles;
    [SerializeField] private Root[] levelConfigFilesOnGUI;          //DeBug - 用於GUI預覽

    public static ClearDataRoot[] clearDataFiles;
    [SerializeField] private ClearDataRoot[] clearDataFilesOnGUI;   //DeBug - 用於GUI預覽

    public static SettingRoot settingFile;
    [SerializeField] private SettingRoot settingFileOnGUI;          //DeBug - 用於GUI預覽

    //檔案ID (用於星圖生成)
    public static int nowFileId;
    public static ClearDataRoot nowClearDataFile;

    //關卡ID (用於關卡生成)
    public static string nowLevelId;
    public static LevelConfig nowLevelData;
    public static ClearLevel nowClearLevel;

    //讀檔用(關卡)
    public void LoadLevelConfigFiles()
    {
        Debug.Log("讀取關卡檔案...");
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "CustomLevels");

        if (!Directory.Exists(directoryPath))
        {
            Debug.LogWarning($"Directory not found: {directoryPath}");
            Directory.CreateDirectory(directoryPath);
        }

        string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");

        // i = 0  =>  預設關卡
        levelConfigFiles = new Root[jsonFiles.Length + 1];

        try
        {
            Root levelConfig = JsonUtility.FromJson<Root>(jsonDefaultLevels.text);
            levelConfigFiles[0] = levelConfig;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse JSON file at jsonDefaultLevels : {e.Message}");
        }

        for (int i = 1; i <= jsonFiles.Length; i++)
        {
            string jsonContent = File.ReadAllText(jsonFiles[i - 1]);

            try
            {
                Root levelConfig = JsonUtility.FromJson<Root>(jsonContent);
                levelConfigFiles[i] = levelConfig;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse JSON file at {jsonFiles[i - 1]}: {e.Message}");
            }
        }

        Debug.Log($"已載入 {levelConfigFiles.Length} 項關卡檔案");

        //該項僅用於開發預覽
        levelConfigFilesOnGUI = levelConfigFiles;
    }

    // 讀檔用(過關資料) [需加密]
    public void LoadClearDataFiles()
    {
        Debug.Log("讀取過關資料...");
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData");

        if (!Directory.Exists(directoryPath))
        {
            Debug.LogWarning($"Directory not found: {directoryPath}");
            Directory.CreateDirectory(directoryPath);
        }

        string[] jsonFiles = Directory.GetFiles(directoryPath, "*.ejson");

        // i = 0  =>  預設關卡
        clearDataFiles = new ClearDataRoot[jsonFiles.Length + 1];

        try
        {
            // 預設關卡存放於 PlayerData/ClearLevelData.json
            string defaultDirectoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
            string encryptedDefaultClearJson = File.ReadAllText(defaultDirectoryPath);
            string defaultClearJson = EncryptionUtility.DecryptString(encryptedDefaultClearJson);
            ClearDataRoot defaultClearDataRoot = JsonUtility.FromJson<ClearDataRoot>(defaultClearJson);
            clearDataFiles[0] = defaultClearDataRoot;
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarning("異常: 未找到目錄");

            Debug.Log("建立目錄 " + "PlayerData");
            string folderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            Directory.CreateDirectory(folderPath);
            CreateClearDataFile("ClearLevelData");

            // 重新讀取
            string defaultDirectoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
            string encryptedDefaultClearJson = File.ReadAllText(defaultDirectoryPath);
            string defaultClearJson = EncryptionUtility.DecryptString(encryptedDefaultClearJson);
            ClearDataRoot defaultClearDataRoot = JsonUtility.FromJson<ClearDataRoot>(defaultClearJson);
            clearDataFiles[0] = defaultClearDataRoot;
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("異常: 未找到文件");
            CreateClearDataFile("ClearLevelData");

            // 重新讀取
            string defaultDirectoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
            string encryptedDefaultClearJson = File.ReadAllText(defaultDirectoryPath);
            string defaultClearJson = EncryptionUtility.DecryptString(encryptedDefaultClearJson);
            ClearDataRoot defaultClearDataRoot = JsonUtility.FromJson<ClearDataRoot>(defaultClearJson);
            clearDataFiles[0] = defaultClearDataRoot;
        }
        finally
        {
            Debug.Log("已解析預設關卡紀錄");
        }

        for (int i = 1; i <= jsonFiles.Length; i++)
        {
            string encryptedJsonContent = File.ReadAllText(jsonFiles[i - 1]);
            string jsonContent = EncryptionUtility.DecryptString(encryptedJsonContent);

            try
            {
                ClearDataRoot clearDataRoot = JsonUtility.FromJson<ClearDataRoot>(jsonContent);
                clearDataFiles[i] = clearDataRoot;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse JSON file at {jsonFiles[i - 1]}: {e.Message}");
            }
        }

        Debug.Log($"已載入 {clearDataFiles.Length} 項過關資料檔案");

        // 該項僅用於開發預覽
        clearDataFilesOnGUI = clearDataFiles;
    }

    //讀檔用(設定檔)
    public void LoadSettingFile()
    {
        Debug.Log("讀取設定檔...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");

        string jsonSetting = null;
        try
        {
            jsonSetting = File.ReadAllText(path);
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarning("異常: 未找到目錄");

            Debug.Log("建立目錄 " + "PlayerData");
            string folderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            Directory.CreateDirectory(folderPath);

            Debug.Log("建立文件 " + "SettingFile.json");
            InitializationSettings();
            jsonSetting = File.ReadAllText(path);
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("異常: 未找到文件");

            Debug.Log("建立文件 " + "SettingFile.json");
            InitializationSettings();
            jsonSetting = File.ReadAllText(path);
        }
        finally
        {
            settingFile = JsonUtility.FromJson<SettingRoot>(jsonSetting);
            Debug.Log("已解析設定參數");
        }

        //該項僅用於開發預覽
        settingFileOnGUI = settingFile;

        //更新音量
        UpdateAudio();
    }

    //初始化設定參數(並匯出)
    public void InitializationSettings()
    {
        Debug.LogWarning("初始化設定檔...");
        settingFile = new SettingRoot();
        settingFile.bgmId = 0;
        settingFile.gameMusic = true;
        settingFile.gameMusicF = 1.0f;
        settingFile.gameSoundEffect = true;
        settingFile.gameSoundEffectF = 1.0f;

        settingFile.effectsVFX = 20;
        settingFile.backgroundVFX = 500;

        settingFile.gameSpeedModifier = 1.0f;

        SaveSettingFile();
        Debug.Log("設定檔已初始化");
    }

    //保存設定參數
    public static void SaveSettingFile()
    {
        string settingsjson = JsonUtility.ToJson(settingFile, true);
        string exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");
        File.WriteAllText(exportFileName, settingsjson);
    }

    // 建立 ClearData 檔案 [需加密]
    private void CreateClearDataFile(string fileName)
    {
        Debug.Log("建立文件 " + "ClearLevelData.ejson");
        var newClearDataRoot = new ClearDataRoot();

        // 檔案路徑
        string exportFileName;
        if (fileName == "ClearLevelData")
        {
            // 預設關卡
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
            newClearDataRoot.name = levelConfigFiles[0].name;
        }
        else
        {
            // 其他關卡
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData", "ClearLevelData_" + fileName + ".ejson");
            newClearDataRoot.name = fileName;
        }

        // 將Root轉換為JSON字符串
        string newClearDataJson = JsonUtility.ToJson(newClearDataRoot, true);

        // 將 JSON 字串加密並寫入檔案
        string encryptedClearDataJson = EncryptionUtility.EncryptString(newClearDataJson);
        File.WriteAllText(exportFileName, encryptedClearDataJson);
    }

    //比對所有 ClearData 及 LevelConfig
    private void CompareAllClearData()
    {
        //比對"存在"
        for (int i = 1; i < levelConfigFiles.Length; i++)
        {
            bool haveClearData = false;
            for (int j = 1; j < clearDataFiles.Length; j++)
            {
                if (levelConfigFiles[i].name == clearDataFiles[j].name)
                {
                    haveClearData = true;
                    break;
                }
            }

            if (!haveClearData)
            {
                CreateClearDataFile(levelConfigFiles[i].name);
            }
        }

        //比對"版本"
        for (int i = 0; i < levelConfigFiles.Length; i++)
        {
            for (int j = 0; j < clearDataFiles.Length; j++)
            {
                if (levelConfigFiles[i].name == clearDataFiles[j].name)
                {
                    if (levelConfigFiles[i].version != clearDataFiles[j].version)
                    {
                        UpdateVersionClearData(i, j);
                    }

                    break;
                }
            }
        }
    }

    // 比對單一 ClearData 版本 [需加密]
    private void UpdateVersionClearData(int levelID, int clearID)
    {
        clearDataFiles[clearID].version = levelConfigFiles[levelID].version;
        var levelConfig = levelConfigFiles[levelID].levelConfig;
        var clearLevel = clearDataFiles[clearID].clearLevel;

        foreach (var level in levelConfig)
        {
            bool haveData = false;
            foreach (var data in clearLevel)
            {
                if (level.levelID == data.levelID)
                {
                    haveData = true;
                }
            }

            if (!haveData)
            {
                var newClearLevel = new ClearLevel();
                newClearLevel.levelID = level.levelID;
                newClearLevel.clear = false;

                var newClearData = new ClearData();
                newClearData.medalLevel = 0;
                newClearLevel.clearData = newClearData;

                clearDataFiles[clearID].clearLevel.Add(newClearLevel);
            }
        }

        // 轉換為JSON字符串
        string clearDataFilesJson = JsonUtility.ToJson(clearDataFiles[clearID], true);

        // 確保目錄存在
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData");
        Directory.CreateDirectory(directoryPath);

        // 檔案路徑
        string exportFileName;
        if (clearID == 0)
        {
            // 預設關卡
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
        }
        else
        {
            // 其他關卡
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData", "ClearLevelData_" + clearDataFiles[clearID].name + ".ejson");
        }

        // 加密並寫入 JSON 到檔案
        string encryptedClearDataFilesJson = EncryptionUtility.EncryptString(clearDataFilesJson);
        File.WriteAllText(exportFileName, encryptedClearDataFilesJson);

        Debug.Log("通關資訊版本已更新並保存到 " + exportFileName);
    }

    //以 nowLevelId 尋找關卡
    public static void FindLevelConfigById()
    {
        foreach (var levelConfig in MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig)
        {
            if (levelConfig.levelID == nowLevelId)
            {
                MainManager.nowLevelData = levelConfig;
            }
        }
    }

    //以 nowFileId 尋找過關檔案
    public static void FindClearDataFileById()
    {
        bool check = false;
        foreach (var clearDataFile in MainManager.clearDataFiles)
        {
            if (clearDataFile.name == MainManager.levelConfigFiles[MainManager.nowFileId].name)
            {
                MainManager.nowClearDataFile = clearDataFile;
                check = true;
                break;
            }
        }

        if (check == false)
        {
            Debug.LogWarning("找不到該檔案!");
        }
    }

    //以 nowLevelId 尋找過關資料
    public static void FindClearLevelById()
    {
        foreach (var clearLevel in MainManager.nowClearDataFile.clearLevel)
        {
            if (clearLevel.levelID == nowLevelId)
            {
                MainManager.nowClearLevel = clearLevel;
            }
        }
    }

    //以 nowLevelData 及 nowClearLevel 檢查是否完成前置關卡
    public static bool CheckPreconditionById()
    {
        int preNumber = 0;
        for (int i = 0; i < nowLevelData.preLevelID.Length; i++)
        {
            foreach (var clearLevel in nowClearDataFile.clearLevel)
            {
                if (nowLevelData.preLevelID[i] == clearLevel.levelID && clearLevel.clear == true)
                {
                    preNumber++;
                }
            }
        }

        if (preNumber == nowLevelData.preLevelID.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 以 nowLevelId 保存當前關卡 [需加密]
    public static void SaveCurrentLevelById(int medalLevel, int score, int time, float speed)
    {
        nowClearLevel.clear = true;
        if (score >= nowClearLevel.clearData.score)
        {
            var newClearData = new ClearData();
            newClearData.medalLevel = medalLevel;
            newClearData.score = score;
            if (time <= nowClearLevel.clearData.time && nowClearLevel.clearData.time != 0)
            {
                newClearData.time = time;
                newClearData.speed = speed;
            }
            else if (nowClearLevel.clearData.time == 0)
            {
                newClearData.time = time;
                newClearData.speed = speed;
            }
            nowClearLevel.clearData = newClearData;
        }

        // 檔案路徑
        string exportFileName;
        if (nowFileId == 0)
        {
            // 預設關卡
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
        }
        else
        {
            // 其他關卡
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData", "ClearLevelData_" + nowClearDataFile.name + ".ejson");
        }

        // 將Root轉換為JSON字符串
        string newClearDataJson = JsonUtility.ToJson(nowClearDataFile, true);

        // 加密並寫入 JSON 字串到檔案
        string encryptedClearDataJson = EncryptionUtility.EncryptString(newClearDataJson);
        File.WriteAllText(exportFileName, encryptedClearDataJson);
    }

    //以 nowLevelId 尋找下一關
    public static void FindNextLevelById()
    {
        string nextLevelId = null;
        if (nowLevelData.nextLevelID == null)
        {
            int nextID = -1;
            for (int i = 0; i < MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig.Count; i++)
            {
                if (MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig[i].levelID == nowLevelId)
                {
                    nextID = i + 1;
                    break;
                }
            }

            if (nextID == -1)
            {
                nextLevelId = nowLevelId;
                Debug.LogWarning("無法取得 NextID " + nextID);
            }
            else if (nextID > MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig.Count)
            {
                nextLevelId = nowLevelId;
                Debug.LogWarning("超出關卡範圍 " + nextID);
            }
            else if (nextID == MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig.Count)
            {
                nextLevelId = nowLevelId;
                Debug.Log("已經是最後一關");
            }
            else if (nextID <= MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig.Count)
            {
                nextLevelId = MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig[nextID].levelID;
                Debug.Log("下一關為: " + nextID + " / " + nextLevelId);
            }
        }
        else
        {
            nextLevelId = nowLevelData.nextLevelID;
        }

        nowLevelId = nextLevelId;
        CalibrationInfoByLevelId();
    }

    //以 nowLevelId 校準資料
    public static void CalibrationInfoByLevelId()
    {
        FindLevelConfigById();
        FindClearLevelById();
    }



    //音訊
    public AudioSource soundEffectUiTrue;
    public AudioSource soundEffectUiFalse;
    public AudioSource soundEffectUiPage;

    public AudioSource[] bgmMusic;

    //運行
    public int nowLevel = 0;


    //轉換場景
    void Awake()
    {
        //轉換場景時保留物件
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroy");

        if (objs.Length > 1)
        {
            // 已經存在相同物件的實例，銷毀新的實例
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        //檔案
        jsonDefaultLevels = Resources.Load<TextAsset>("Data/DefaultLevels");

        //新版
        LoadLevelConfigFiles();
        LoadClearDataFiles();
        LoadSettingFile();
        CompareAllClearData();


        UpdateAudio();

        Debug.Log("MainManager 初始化完成");
    }

    void Start()
    {
        StartCoroutine(StartAfterAllObjectsLoaded());
    }

    IEnumerator StartAfterAllObjectsLoaded()
    {
        // 等待1秒，所有物件加載完成後執行的代碼
        yield return new WaitForSeconds(1);

        bgmMusic[MainManager.settingFile.bgmId].gameObject.SetActive(true);
    }

    void Update()
    {

    }

    //更新音量大小
    public void UpdateAudio()
    {
        soundEffectUiTrue.volume = settingFile.gameSoundEffectF * 1.0f;
        soundEffectUiFalse.volume = settingFile.gameSoundEffectF * 2.0f;
        soundEffectUiPage.volume = settingFile.gameSoundEffectF * 1.0f;

        for (int i = 0; i < bgmMusic.Length; i++)
        {
            bgmMusic[i].volume = settingFile.gameMusicF * 1.0f;
        }
    }


    //場景轉換==========================================================================================================================
    //接續(SelectLevelButton)-進入關卡
    public void EnterGameScene()
    {
        // 載入 GameScene
        SceneManager.LoadScene("GameScene");
    }
}
