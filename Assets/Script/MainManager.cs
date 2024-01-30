using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;

public class GameSetting
{
    //音樂 ON/OFF + 滑動條
    public static bool gameMusic;
    public static float gameMusicF;

    //音效 ON/OFF + 滑動條
    public static bool gameSoundEffect;
    public static float gameSoundEffectF;

    //VFX效果
    public static float effectsVFX;

    //VFX背景
    public static float backgroundVFX;

    //速度倍率
    public static float gameSpeedModifier;

}

public class MainManager : MonoBehaviour
{
    //序列化==========================================================================================================================

    //序列化定義(設定)
    [Serializable]
    public class SettingRoot
    {
        public bool gameMusic;
        public float gameMusicF;
        public bool gameSoundEffect;
        public float gameSoundEffectF;

        public float effectsVFX;
        public float backgroundVFX;

        public float gameSpeedModifier;
    }

    //序列化定義(通關資訊)
    [Serializable]
    public class PlayerDataRoot
    {
        public int clearLevelAmount;
        public List<ClearLevel> clearLevel;
    }

    [Serializable]
    public class ClearLevel
    {
        public string level;
        public ClearData clearData;
    }

    [Serializable]
    public class ClearData
    {
        public bool clear;
        public int time;
        public int score;
        public float speed;
    }

    //序列化定義(預設關卡)
    [Serializable]
    public class Root
    {
        public List<LevelConfig> levelConfig;
    }

    //menuX menuY menuStyle 未使用
    [Serializable]
    public class LevelConfig
    {
        public string levelName;
        public string gameType;
        public float menuX;
        public float menuY;
        public int menuStyle;
        public List<BricksData> bricksData;
    }

    [Serializable]
    public class BricksData
    {
        public int xPoint;
        public int yPoint;
        public int pointValue;
        public int brickLevel;
        public int powerUpType;
        public int brickType;
    }

    //變數==========================================================================================================================

    //檔案
    [SerializeField] private TextAsset jsonDefaultLevels;
    [SerializeField] private TextAsset jsonPlayerData;
    [SerializeField] private TextAsset jsonSetting;

    //實例
    public SettingRoot settings;                                //設定    
    public Root defaultLevelsRoot;                              //預設關卡
    [SerializeField] private PlayerDataRoot playerData;         //通關資訊

    //腳本

    //預覽
    [SerializeField] private GameObject previewBrickPrefab;                 // 磚塊的預置體
    [SerializeField] private GameObject[] powerUpPrefabs;                   // 4種不同的預製件，分別代表type為1 - 4的預製件
    [SerializeField] private Transform previewBrickList;
    private int brickAmount;

    //運行
    public int nowLevel = 0;

    void Start()
    {
        //檔案
        jsonDefaultLevels = Resources.Load<TextAsset>("Data/DefaultLevels");

        LoadDefaultLevels();
        LoadSettingsFromFile();
        LoadPlayerDataFromFile();
    }


    void Update()
    {

    }

    //資料操作==========================================================================================================================

    //資料操作-設定參數----------------------------------------------

    //初始化設定參數(並匯出)
    public void InitializationSettings()
    {
        Debug.LogWarning("初始化設定檔...");
        settings.gameMusic = true;
        settings.gameMusicF = 1.0f;
        settings.gameSoundEffect = true;
        settings.gameSoundEffectF = 1.0f;

        settings.effectsVFX = 1.0f;
        settings.backgroundVFX = 1.0f;

        settings.gameSpeedModifier = 1.0f;

        SaveSettingsToJson();
        Debug.Log("設定檔已初始化");
        Debug.Log("重新讀取設定檔...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");
        string json = File.ReadAllText(path);
        jsonSetting = new TextAsset(json);

        if (jsonSetting != null)
        {
            settings = JsonUtility.FromJson<SettingRoot>(jsonSetting.text);

            if (settings != null)
            {
                //音樂 ON/OFF + 滑動條
                GameSetting.gameMusic = settings.gameMusic;
                GameSetting.gameMusicF = settings.gameMusicF;

                //音效 ON/OFF + 滑動條
                GameSetting.gameSoundEffect = settings.gameSoundEffect;
                GameSetting.gameSoundEffectF = settings.gameSoundEffectF;

                //VFX效果
                GameSetting.effectsVFX = settings.effectsVFX;

                //VFX背景
                GameSetting.backgroundVFX = settings.backgroundVFX;

                //速度倍率
                GameSetting.gameSpeedModifier = settings.gameSpeedModifier;
                Debug.Log("[初始化]已成功載入設定參數");
            }
            else
            {
                Debug.LogWarning("[初始化]無法載入設定參數");
            }
        }
        else
        {
            Debug.LogWarning("[初始化]未找到設定參數檔");
        }
    }


    //讀取設定參數
    public void LoadSettingsFromFile()
    {
        Debug.Log("讀取設定檔...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");

        try
        {
            string json = File.ReadAllText(path);
            jsonSetting = new TextAsset(json);
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarning("異常: 未找到目錄");

            Debug.Log("建立目錄 " + "PlayerData");
            string folderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            Directory.CreateDirectory(folderPath);

            Debug.Log("建立文件 " + "SettingFile.json");
            InitializationSettings();
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("異常: 未找到文件");

            Debug.Log("建立文件 " + "SettingFile.json");
            InitializationSettings();
        }
        finally
        {
            SetGameSetting();
        }
    }


    //按照實例指定參數
    public void SetGameSetting()
    {
        settings = JsonUtility.FromJson<SettingRoot>(jsonSetting.text);

        if (settings != null)
        {
            //音樂 ON/OFF + 滑動條
            GameSetting.gameMusic = settings.gameMusic;
            GameSetting.gameMusicF = settings.gameMusicF;

            //音效 ON/OFF + 滑動條
            GameSetting.gameSoundEffect = settings.gameSoundEffect;
            GameSetting.gameSoundEffectF = settings.gameSoundEffectF;

            //VFX效果
            GameSetting.effectsVFX = settings.effectsVFX;

            //VFX背景
            GameSetting.backgroundVFX = settings.backgroundVFX;

            //速度倍率
            GameSetting.gameSpeedModifier = settings.gameSpeedModifier;
            Debug.Log("已解析設定參數");
        }
        else
        {
            Debug.LogWarning("無法解析設定參數");
        }
    }


    //匯出設定參數 JSON
    public void SaveSettingsToJson()
    {
        Debug.Log("保存設定參數...");
        // 將SettingRoot轉換為JSON字符串
        string settingsjson = JsonUtility.ToJson(settings, true);

        // 檔案路徑
        string exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");

        // 將 JSON 字串寫入檔案
        File.WriteAllText(exportFileName, settingsjson);

        Debug.Log("設定參數已保存到 " + exportFileName);
    }


    //資料操作-通關資訊----------------------------------------------

    //初始化通關資訊(並匯出)
    public void InitializationPlayerData()
    {
        Debug.LogWarning("初始化通關資訊...");
        playerData = new PlayerDataRoot();

        playerData.clearLevelAmount = 0;
        playerData.clearLevel = new List<ClearLevel>();

        //設置Level.0 Level.1基礎檔案
        for (int i = 0; i < 2; i++)
        {
            ClearLevel clearLevel = new ClearLevel();
            clearLevel.level = defaultLevelsRoot.levelConfig[i].levelName;

            ClearData clearData = new ClearData();
            clearData.clear = false;
            clearData.score = -1;
            clearData.time = -1;
            clearData.speed = -1;

            clearLevel.clearData = clearData;

            playerData.clearLevel.Add(clearLevel);
        }

        SavePlayerDataToJson();
        Debug.Log("通關資訊已初始化");
        Debug.Log("重新讀取通關資訊...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");
        string json = File.ReadAllText(path);
        jsonPlayerData = new TextAsset(json);

        if (jsonPlayerData != null)
        {
            playerData = JsonUtility.FromJson<PlayerDataRoot>(jsonPlayerData.text);

            if (playerData != null)
            {
                Debug.Log("[初始化]已成功載入通關資訊");
            }
            else
            {
                Debug.LogWarning("[初始化]無法載入通關資訊");
            }
        }
        else
        {
            Debug.LogWarning("[初始化]未找到通關資訊檔");
        }
    }


    //讀取通關資訊
    public void LoadPlayerDataFromFile()
    {
        Debug.Log("讀取通關資訊...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");

        try
        {
            string json = File.ReadAllText(path);
            jsonSetting = new TextAsset(json);
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarning("異常: 未找到目錄");

            Debug.Log("建立目錄 " + "PlayerData");
            string folderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            Directory.CreateDirectory(folderPath);

            Debug.Log("建立文件 " + "ClearLevelData.json");
            InitializationPlayerData();
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("異常: 未找到文件");

            Debug.Log("建立文件 " + "ClearLevelData.json");
            InitializationPlayerData();
        }
        finally
        {
            SetPlayerData();
        }
    }


    //按照實例指定通關資訊
    public void SetPlayerData()
    {
        if (jsonPlayerData != null)
        {
            playerData = JsonUtility.FromJson<PlayerDataRoot>(jsonPlayerData.text);

            if (playerData != null)
            {
                Debug.Log("已解析通關資訊");
            }
            else
            {
                Debug.LogWarning("無法解析通關資訊");
            }
        }
    }


    //匯出通關資訊 JSON
    public void SavePlayerDataToJson()
    {
        // 將SettingRoot轉換為JSON字符串
        string playerDatajson = JsonUtility.ToJson(playerData, true);

        // 檔案路徑
        string exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");

        // 將 JSON 字串寫入檔案
        File.WriteAllText(exportFileName, playerDatajson);

        Debug.Log("通關資訊已保存到 " + exportFileName);
    }


    //資料操作-預設關卡----------------------------------------------

    //讀取預設關卡資訊
    public void LoadDefaultLevels()
    {
        if (jsonDefaultLevels != null)
        {
            // 使用 JsonUtility 解析 JSON
            defaultLevelsRoot = JsonUtility.FromJson<Root>(jsonDefaultLevels.text);

            if (defaultLevelsRoot != null)
            {
                Debug.Log("已成功載入預設關卡");
            }
            else
            {
                Debug.LogWarning("無法載入預設關卡");
            }
        }
        else
        {
            Debug.LogWarning("未找到預設關卡檔");
        }
    }

    //預覽關卡==========================================================================================================================

    //預覽-指定容器
    public void FindGameObject()
    {
        //指定容器
        previewBrickList = GameObject.Find("GamePreviewList").GetComponent<Transform>();
    }

    //預覽-解析部分
    public void PreviewGenerate()
    {
        LevelConfig targetLevelConfig;

        if (defaultLevelsRoot != null)
        {
            targetLevelConfig = defaultLevelsRoot.levelConfig[nowLevel];

            //啟動生成器
            if (targetLevelConfig != null)
            {
                PreviewUI(targetLevelConfig);
                FindGameObject();
                PreviewGenerateBricks(targetLevelConfig.bricksData);
            }
            else
            {
                Debug.LogWarning("指定的關卡編號不存在");
            }
        }
        else
        {
            Debug.LogError("未提供 JSON 配置文件");
        }
    }

    //預覽-U.I.部分
    void PreviewUI(LevelConfig targetLevelConfig)
    {
        MenuManager menuManager = GameObject.Find("Canvas").GetComponent<MenuManager>();
        menuManager.previewNameText.text = targetLevelConfig.levelName;
        menuManager.previewBrickAmountText.text = targetLevelConfig.bricksData.Count.ToString();

        if (nowLevel < playerData.clearLevel.Count)
        {
            menuManager.previewScoreText.text = playerData.clearLevel[nowLevel].clearData.score.ToString();
            menuManager.previewTimerText.text = playerData.clearLevel[nowLevel].clearData.time.ToString();
            menuManager.previewSpeedText.text = playerData.clearLevel[nowLevel].clearData.speed.ToString();
            menuManager.previewClearTF.gameObject.SetActive(playerData.clearLevel[nowLevel].clearData.clear);
        }
        else
        {
            menuManager.previewScoreText.text = "Null";
            menuManager.previewTimerText.text = "Null";
            menuManager.previewSpeedText.text = "Null";
            menuManager.previewClearTF.gameObject.SetActive(false);
        }
    }

    //預覽-生成部分
    void PreviewGenerateBricks(List<BricksData> bricks)
    {
        brickAmount = 0;

        foreach (var brickData in bricks)
        {
            Vector3 position = new Vector3(16.25f - (2.5f * brickData.xPoint), 6.5f - (0.5f * brickData.yPoint), -1);
            GameObject brick = Instantiate(previewBrickPrefab, position, Quaternion.identity, previewBrickList);

            // 設置磚塊的屬性
            var brickScript = brick.GetComponent<BrickPreview>();
            if (brickScript != null)
            {
                brickScript.pointValue = brickData.pointValue;
                brickScript.brickLevel = brickData.brickLevel;
                brickScript.powerUpType = brickData.powerUpType;
                PreviewGeneratePowerUp(position, brickData.powerUpType);
            }

            brickAmount += 1;
        }
    }

    //預覽-生成部分-道具生成
    void PreviewGeneratePowerUp(Vector3 position, int powerUpType)
    {
        GameObject spawnedPowerUp;

        switch (powerUpType)
        {
            case 0:
                break;
            case 1:
                spawnedPowerUp = Instantiate(powerUpPrefabs[0], position, Quaternion.identity, previewBrickList);
                break;
            case 2:
                spawnedPowerUp = Instantiate(powerUpPrefabs[1], position, Quaternion.identity, previewBrickList);
                break;
            case 3:
                spawnedPowerUp = Instantiate(powerUpPrefabs[2], position, Quaternion.identity, previewBrickList);
                break;
            case 4:
                spawnedPowerUp = Instantiate(powerUpPrefabs[3], position, Quaternion.identity, previewBrickList);
                break;
            default:
                Debug.LogWarning("未知的Item類型: " + powerUpType);
                break;
        }
    }

    //預覽-銷毀所有子物件
    public void PreviewClearChildren()
    {
        // 遍歷並銷毀所有子物件
        foreach (Transform child in previewBrickList)
        {
            Destroy(child.gameObject);
        }

        // 清空子物件列表
        previewBrickList.DetachChildren();
    }

    //場景轉換==========================================================================================================================

    //接續(SelectLevelButton)-進入關卡
    public void EnterGameScene()
    {
        // 載入 GameScene
        SceneManager.LoadScene("GameScene");
    }


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
    }

    //主場景==========================================================================================================================

    //過關保存
    public void GameClearedSave()
    {
        //已存在通關資訊
        if (playerData.clearLevel[nowLevel].clearData.clear == true)
        {
            //分數擇優
            if (GameData.score > playerData.clearLevel[nowLevel].clearData.score)
            {
                playerData.clearLevel[nowLevel].clearData.score = GameData.score;
            }
            //計時擇優
            if (GameData.saveTime > playerData.clearLevel[nowLevel].clearData.time)
            {
                playerData.clearLevel[nowLevel].clearData.time = (int)GameData.saveTime;
                playerData.clearLevel[nowLevel].clearData.speed = settings.gameSpeedModifier;
            }
        }
        //未通關
        else if (playerData.clearLevel[nowLevel].clearData.clear == false)
        {
            if (defaultLevelsRoot.levelConfig[nowLevel].levelName != "Level.DeBug")
            {
                playerData.clearLevelAmount += 1;
            }

            playerData.clearLevel[nowLevel].clearData.clear = true;
            playerData.clearLevel[nowLevel].clearData.score = GameData.score;
            playerData.clearLevel[nowLevel].clearData.time = (int)GameData.saveTime;
            playerData.clearLevel[nowLevel].clearData.speed = settings.gameSpeedModifier;

            NewClearLevelSave();
        }
        //無通關資訊
        else
        {
            if (defaultLevelsRoot.levelConfig[nowLevel].levelName != "Level.DeBug")
            {
                playerData.clearLevelAmount += 1;
            }

            //初始化
            ClearLevel clearLevel = new ClearLevel();
            clearLevel.level = defaultLevelsRoot.levelConfig[nowLevel].levelName;

            ClearData clearData = new ClearData();
            clearData.clear = true;
            clearData.score = GameData.score;
            clearData.time = (int)GameData.saveTime;
            clearData.speed = settings.gameSpeedModifier;

            clearLevel.clearData = clearData;

            playerData.clearLevel.Add(clearLevel);

            NewClearLevelSave();
        }

        //保存Json
        SavePlayerDataToJson();
    }

    //製作 N+1關
    void NewClearLevelSave()
    {
        bool TorF = true;

        //關卡重複判斷式
        for (int i = 0; i < playerData.clearLevel.Count; i++)
        {
            if (defaultLevelsRoot.levelConfig[i].levelName == defaultLevelsRoot.levelConfig[(nowLevel + 1)].levelName)
            {
                TorF = false;
                break;
            }
        }

        if (TorF)
        {
            //初始化
            ClearLevel newClearLevel = new ClearLevel();
            newClearLevel.level = defaultLevelsRoot.levelConfig[(nowLevel + 1)].levelName;

            ClearData newClearData = new ClearData();
            newClearData.clear = false;
            newClearData.score = -1;
            newClearData.time = -1;
            newClearData.speed = -1;

            newClearLevel.clearData = newClearData;

            playerData.clearLevel.Add(newClearLevel);
        }
    }

    //設定介面==========================================================================================================================


    //其他==========================================================================================================================
}
