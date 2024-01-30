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
    //���� ON/OFF + �ưʱ�
    public static bool gameMusic;
    public static float gameMusicF;

    //���� ON/OFF + �ưʱ�
    public static bool gameSoundEffect;
    public static float gameSoundEffectF;

    //VFX�ĪG
    public static float effectsVFX;

    //VFX�I��
    public static float backgroundVFX;

    //�t�׭��v
    public static float gameSpeedModifier;

}

public class MainManager : MonoBehaviour
{
    //�ǦC��==========================================================================================================================

    //�ǦC�Ʃw�q(�]�w)
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

    //�ǦC�Ʃw�q(�q����T)
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

    //�ǦC�Ʃw�q(�w�]���d)
    [Serializable]
    public class Root
    {
        public List<LevelConfig> levelConfig;
    }

    //menuX menuY menuStyle ���ϥ�
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

    //�ܼ�==========================================================================================================================

    //�ɮ�
    [SerializeField] private TextAsset jsonDefaultLevels;
    [SerializeField] private TextAsset jsonPlayerData;
    [SerializeField] private TextAsset jsonSetting;

    //���
    public SettingRoot settings;                                //�]�w    
    public Root defaultLevelsRoot;                              //�w�]���d
    [SerializeField] private PlayerDataRoot playerData;         //�q����T

    //�}��

    //�w��
    [SerializeField] private GameObject previewBrickPrefab;                 // �j�����w�m��
    [SerializeField] private GameObject[] powerUpPrefabs;                   // 4�ؤ��P���w�s��A���O�N��type��1 - 4���w�s��
    [SerializeField] private Transform previewBrickList;
    private int brickAmount;

    //�B��
    public int nowLevel = 0;

    void Start()
    {
        //�ɮ�
        jsonDefaultLevels = Resources.Load<TextAsset>("Data/DefaultLevels");

        LoadDefaultLevels();
        LoadSettingsFromFile();
        LoadPlayerDataFromFile();
    }


    void Update()
    {

    }

    //��ƾާ@==========================================================================================================================

    //��ƾާ@-�]�w�Ѽ�----------------------------------------------

    //��l�Ƴ]�w�Ѽ�(�öץX)
    public void InitializationSettings()
    {
        Debug.LogWarning("��l�Ƴ]�w��...");
        settings.gameMusic = true;
        settings.gameMusicF = 1.0f;
        settings.gameSoundEffect = true;
        settings.gameSoundEffectF = 1.0f;

        settings.effectsVFX = 1.0f;
        settings.backgroundVFX = 1.0f;

        settings.gameSpeedModifier = 1.0f;

        SaveSettingsToJson();
        Debug.Log("�]�w�ɤw��l��");
        Debug.Log("���sŪ���]�w��...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");
        string json = File.ReadAllText(path);
        jsonSetting = new TextAsset(json);

        if (jsonSetting != null)
        {
            settings = JsonUtility.FromJson<SettingRoot>(jsonSetting.text);

            if (settings != null)
            {
                //���� ON/OFF + �ưʱ�
                GameSetting.gameMusic = settings.gameMusic;
                GameSetting.gameMusicF = settings.gameMusicF;

                //���� ON/OFF + �ưʱ�
                GameSetting.gameSoundEffect = settings.gameSoundEffect;
                GameSetting.gameSoundEffectF = settings.gameSoundEffectF;

                //VFX�ĪG
                GameSetting.effectsVFX = settings.effectsVFX;

                //VFX�I��
                GameSetting.backgroundVFX = settings.backgroundVFX;

                //�t�׭��v
                GameSetting.gameSpeedModifier = settings.gameSpeedModifier;
                Debug.Log("[��l��]�w���\���J�]�w�Ѽ�");
            }
            else
            {
                Debug.LogWarning("[��l��]�L�k���J�]�w�Ѽ�");
            }
        }
        else
        {
            Debug.LogWarning("[��l��]�����]�w�Ѽ���");
        }
    }


    //Ū���]�w�Ѽ�
    public void LoadSettingsFromFile()
    {
        Debug.Log("Ū���]�w��...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");

        try
        {
            string json = File.ReadAllText(path);
            jsonSetting = new TextAsset(json);
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarning("���`: �����ؿ�");

            Debug.Log("�إߥؿ� " + "PlayerData");
            string folderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            Directory.CreateDirectory(folderPath);

            Debug.Log("�إߤ�� " + "SettingFile.json");
            InitializationSettings();
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("���`: �������");

            Debug.Log("�إߤ�� " + "SettingFile.json");
            InitializationSettings();
        }
        finally
        {
            SetGameSetting();
        }
    }


    //���ӹ�ҫ��w�Ѽ�
    public void SetGameSetting()
    {
        settings = JsonUtility.FromJson<SettingRoot>(jsonSetting.text);

        if (settings != null)
        {
            //���� ON/OFF + �ưʱ�
            GameSetting.gameMusic = settings.gameMusic;
            GameSetting.gameMusicF = settings.gameMusicF;

            //���� ON/OFF + �ưʱ�
            GameSetting.gameSoundEffect = settings.gameSoundEffect;
            GameSetting.gameSoundEffectF = settings.gameSoundEffectF;

            //VFX�ĪG
            GameSetting.effectsVFX = settings.effectsVFX;

            //VFX�I��
            GameSetting.backgroundVFX = settings.backgroundVFX;

            //�t�׭��v
            GameSetting.gameSpeedModifier = settings.gameSpeedModifier;
            Debug.Log("�w�ѪR�]�w�Ѽ�");
        }
        else
        {
            Debug.LogWarning("�L�k�ѪR�]�w�Ѽ�");
        }
    }


    //�ץX�]�w�Ѽ� JSON
    public void SaveSettingsToJson()
    {
        Debug.Log("�O�s�]�w�Ѽ�...");
        // �NSettingRoot�ഫ��JSON�r�Ŧ�
        string settingsjson = JsonUtility.ToJson(settings, true);

        // �ɮ׸��|
        string exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");

        // �N JSON �r��g�J�ɮ�
        File.WriteAllText(exportFileName, settingsjson);

        Debug.Log("�]�w�ѼƤw�O�s�� " + exportFileName);
    }


    //��ƾާ@-�q����T----------------------------------------------

    //��l�Ƴq����T(�öץX)
    public void InitializationPlayerData()
    {
        Debug.LogWarning("��l�Ƴq����T...");
        playerData = new PlayerDataRoot();

        playerData.clearLevelAmount = 0;
        playerData.clearLevel = new List<ClearLevel>();

        //�]�mLevel.0 Level.1��¦�ɮ�
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
        Debug.Log("�q����T�w��l��");
        Debug.Log("���sŪ���q����T...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");
        string json = File.ReadAllText(path);
        jsonPlayerData = new TextAsset(json);

        if (jsonPlayerData != null)
        {
            playerData = JsonUtility.FromJson<PlayerDataRoot>(jsonPlayerData.text);

            if (playerData != null)
            {
                Debug.Log("[��l��]�w���\���J�q����T");
            }
            else
            {
                Debug.LogWarning("[��l��]�L�k���J�q����T");
            }
        }
        else
        {
            Debug.LogWarning("[��l��]�����q����T��");
        }
    }


    //Ū���q����T
    public void LoadPlayerDataFromFile()
    {
        Debug.Log("Ū���q����T...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");

        try
        {
            string json = File.ReadAllText(path);
            jsonSetting = new TextAsset(json);
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarning("���`: �����ؿ�");

            Debug.Log("�إߥؿ� " + "PlayerData");
            string folderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            Directory.CreateDirectory(folderPath);

            Debug.Log("�إߤ�� " + "ClearLevelData.json");
            InitializationPlayerData();
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("���`: �������");

            Debug.Log("�إߤ�� " + "ClearLevelData.json");
            InitializationPlayerData();
        }
        finally
        {
            SetPlayerData();
        }
    }


    //���ӹ�ҫ��w�q����T
    public void SetPlayerData()
    {
        if (jsonPlayerData != null)
        {
            playerData = JsonUtility.FromJson<PlayerDataRoot>(jsonPlayerData.text);

            if (playerData != null)
            {
                Debug.Log("�w�ѪR�q����T");
            }
            else
            {
                Debug.LogWarning("�L�k�ѪR�q����T");
            }
        }
    }


    //�ץX�q����T JSON
    public void SavePlayerDataToJson()
    {
        // �NSettingRoot�ഫ��JSON�r�Ŧ�
        string playerDatajson = JsonUtility.ToJson(playerData, true);

        // �ɮ׸��|
        string exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");

        // �N JSON �r��g�J�ɮ�
        File.WriteAllText(exportFileName, playerDatajson);

        Debug.Log("�q����T�w�O�s�� " + exportFileName);
    }


    //��ƾާ@-�w�]���d----------------------------------------------

    //Ū���w�]���d��T
    public void LoadDefaultLevels()
    {
        if (jsonDefaultLevels != null)
        {
            // �ϥ� JsonUtility �ѪR JSON
            defaultLevelsRoot = JsonUtility.FromJson<Root>(jsonDefaultLevels.text);

            if (defaultLevelsRoot != null)
            {
                Debug.Log("�w���\���J�w�]���d");
            }
            else
            {
                Debug.LogWarning("�L�k���J�w�]���d");
            }
        }
        else
        {
            Debug.LogWarning("�����w�]���d��");
        }
    }

    //�w�����d==========================================================================================================================

    //�w��-���w�e��
    public void FindGameObject()
    {
        //���w�e��
        previewBrickList = GameObject.Find("GamePreviewList").GetComponent<Transform>();
    }

    //�w��-�ѪR����
    public void PreviewGenerate()
    {
        LevelConfig targetLevelConfig;

        if (defaultLevelsRoot != null)
        {
            targetLevelConfig = defaultLevelsRoot.levelConfig[nowLevel];

            //�Ұʥͦ���
            if (targetLevelConfig != null)
            {
                PreviewUI(targetLevelConfig);
                FindGameObject();
                PreviewGenerateBricks(targetLevelConfig.bricksData);
            }
            else
            {
                Debug.LogWarning("���w�����d�s�����s�b");
            }
        }
        else
        {
            Debug.LogError("������ JSON �t�m���");
        }
    }

    //�w��-U.I.����
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

    //�w��-�ͦ�����
    void PreviewGenerateBricks(List<BricksData> bricks)
    {
        brickAmount = 0;

        foreach (var brickData in bricks)
        {
            Vector3 position = new Vector3(16.25f - (2.5f * brickData.xPoint), 6.5f - (0.5f * brickData.yPoint), -1);
            GameObject brick = Instantiate(previewBrickPrefab, position, Quaternion.identity, previewBrickList);

            // �]�m�j�����ݩ�
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

    //�w��-�ͦ�����-�D��ͦ�
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
                Debug.LogWarning("������Item����: " + powerUpType);
                break;
        }
    }

    //�w��-�P���Ҧ��l����
    public void PreviewClearChildren()
    {
        // �M���þP���Ҧ��l����
        foreach (Transform child in previewBrickList)
        {
            Destroy(child.gameObject);
        }

        // �M�Ťl����C��
        previewBrickList.DetachChildren();
    }

    //�����ഫ==========================================================================================================================

    //����(SelectLevelButton)-�i�J���d
    public void EnterGameScene()
    {
        // ���J GameScene
        SceneManager.LoadScene("GameScene");
    }


    //�ഫ����
    void Awake()
    {
        //�ഫ�����ɫO�d����
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroy");

        if (objs.Length > 1)
        {
            // �w�g�s�b�ۦP���󪺹�ҡA�P���s�����
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    //�D����==========================================================================================================================

    //�L���O�s
    public void GameClearedSave()
    {
        //�w�s�b�q����T
        if (playerData.clearLevel[nowLevel].clearData.clear == true)
        {
            //���ƾ��u
            if (GameData.score > playerData.clearLevel[nowLevel].clearData.score)
            {
                playerData.clearLevel[nowLevel].clearData.score = GameData.score;
            }
            //�p�ɾ��u
            if (GameData.saveTime > playerData.clearLevel[nowLevel].clearData.time)
            {
                playerData.clearLevel[nowLevel].clearData.time = (int)GameData.saveTime;
                playerData.clearLevel[nowLevel].clearData.speed = settings.gameSpeedModifier;
            }
        }
        //���q��
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
        //�L�q����T
        else
        {
            if (defaultLevelsRoot.levelConfig[nowLevel].levelName != "Level.DeBug")
            {
                playerData.clearLevelAmount += 1;
            }

            //��l��
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

        //�O�sJson
        SavePlayerDataToJson();
    }

    //�s�@ N+1��
    void NewClearLevelSave()
    {
        bool TorF = true;

        //���d���ƧP�_��
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
            //��l��
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

    //�]�w����==========================================================================================================================


    //��L==========================================================================================================================
}
