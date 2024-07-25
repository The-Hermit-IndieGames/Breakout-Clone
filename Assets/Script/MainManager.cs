using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


public class MainManager : MonoBehaviour
{
    //�ǦC��==========================================================================================================================

    //�ǦC�Ʃw�q(�]�w)
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

    //�ǦC�Ʃw�q(�q����T)
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

    //�ǦC�Ʃw�q-���d���
    [Serializable]
    public class Root
    {
        //�ڥؿ�: �ؿ��W�١B�s�@�̡B�������B�y�z�B���d�C��(LevelConfig)
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
        //��@���d���: ���d�W�١B�e�m���dID�B�C���Ҧ�(Normal�B?�B?)�B�������s�y��(x�By)�B�������s����B�j���C��(BricksData)
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
        //��@�j�����: �j���y��(x�By)�B�j������(Null�BNormal�BUnbreakable)�B�j�������S�����
        public int xPoint;
        public int yPoint;
        public string brickType;
        public NormalBricks normalBricks;
    }

    [Serializable]
    public class NormalBricks
    {
        //���q�j�����: �ŧO�B�D��
        public int brickLevel;
        public int powerUpType;
    }

<<<<<<< Updated upstream
=======
    [Serializable]
    public class InitialItem
    {
        //��l�D��
        public bool addBall;
        public bool longPaddle;
        public bool burstBall;
        public bool blackHole;
        public bool burstPaddle;
    }

>>>>>>> Stashed changes
    //�R�A==========================================================================================================================

    //�ɮ�
    [SerializeField] private TextAsset jsonDefaultLevels;
    public static Root[] levelConfigFiles;
    [SerializeField] private Root[] levelConfigFilesOnGUI;          //DeBug - �Ω�GUI�w��
<<<<<<< Updated upstream

    public static ClearDataRoot[] clearDataFiles;
    [SerializeField] private ClearDataRoot[] clearDataFilesOnGUI;   //DeBug - �Ω�GUI�w��

    public static SettingRoot settingFile;
    [SerializeField] private SettingRoot settingFileOnGUI;          //DeBug - �Ω�GUI�w��

    //�ɮ�ID (�Ω�P�ϥͦ�)
    public static int nowFileId;
    public static ClearDataRoot nowClearDataFile;

    //���dID (�Ω����d�ͦ�)
    public static string nowLevelId;
    public static LevelConfig nowLevelData;
    public static ClearLevel nowClearLevel;

    //Ū�ɥ�(���d)
    public void LoadLevelConfigFiles()
    {
        Debug.Log("Ū�����d�ɮ�...");
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "CustomLevels");

        if (!Directory.Exists(directoryPath))
        {
            Debug.LogWarning($"Directory not found: {directoryPath}");
            Directory.CreateDirectory(directoryPath);
        }

        string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");

        // i = 0  =>  �w�]���d
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

        Debug.Log($"�w���J {levelConfigFiles.Length} �����d�ɮ�");

        //�Ӷ��ȥΩ�}�o�w��
        levelConfigFilesOnGUI = levelConfigFiles;
    }

    //Ū�ɥ�(�L�����) [�ݥ[�K]
    public void LoadClearDataFiles()
    {
        Debug.Log("Ū���L�����...");
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData");

        if (!Directory.Exists(directoryPath))
        {
            Debug.LogWarning($"Directory not found: {directoryPath}");
            Directory.CreateDirectory(directoryPath);
        }


        string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");

        // i = 0  =>  �w�]���d
        clearDataFiles = new ClearDataRoot[jsonFiles.Length + 1];

        try
        {
            // �w�]���d�s��� PlayerData/ClearLevelData.json
            string defaultDirectoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");
            string defaultClearJson = File.ReadAllText(defaultDirectoryPath);
            ClearDataRoot defaultClearDataRoot = JsonUtility.FromJson<ClearDataRoot>(defaultClearJson);
            clearDataFiles[0] = defaultClearDataRoot;
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarning("���`: �����ؿ�");

            Debug.Log("�إߥؿ� " + "PlayerData");
            string folderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            Directory.CreateDirectory(folderPath);
            CreateClearDataFile("ClearLevelData");

            // ���sŪ��
            string defaultDirectoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");
            string defaultClearJson = File.ReadAllText(defaultDirectoryPath);
            ClearDataRoot defaultClearDataRoot = JsonUtility.FromJson<ClearDataRoot>(defaultClearJson);
            clearDataFiles[0] = defaultClearDataRoot;
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("���`: �������");
            CreateClearDataFile("ClearLevelData");

            // ���sŪ��
            string defaultDirectoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");
            string defaultClearJson = File.ReadAllText(defaultDirectoryPath);
            ClearDataRoot defaultClearDataRoot = JsonUtility.FromJson<ClearDataRoot>(defaultClearJson);
            clearDataFiles[0] = defaultClearDataRoot;
        }
        finally
        {
            Debug.Log("�w�ѪR�w�]���d����");
        }

        for (int i = 1; i <= jsonFiles.Length; i++)
        {
            string jsonContent = File.ReadAllText(jsonFiles[i - 1]);

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

        Debug.Log($"�w���J {clearDataFiles.Length} ���L������ɮ�");

        //�Ӷ��ȥΩ�}�o�w��
        clearDataFilesOnGUI = clearDataFiles;
    }

    //Ū�ɥ�(�]�w��)
    public void LoadSettingFile()
    {
        Debug.Log("Ū���]�w��...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");

        string jsonSetting = null;
        try
        {
            jsonSetting = File.ReadAllText(path);
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarning("���`: �����ؿ�");

            Debug.Log("�إߥؿ� " + "PlayerData");
            string folderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            Directory.CreateDirectory(folderPath);

            Debug.Log("�إߤ�� " + "SettingFile.json");
            InitializationSettings();
            jsonSetting = File.ReadAllText(path);
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("���`: �������");

            Debug.Log("�إߤ�� " + "SettingFile.json");
            InitializationSettings();
            jsonSetting = File.ReadAllText(path);
        }
        finally
        {
            settingFile = JsonUtility.FromJson<SettingRoot>(jsonSetting);
            Debug.Log("�w�ѪR�]�w�Ѽ�");
        }

        //�Ӷ��ȥΩ�}�o�w��
        settingFileOnGUI = settingFile;
    }

    //��l�Ƴ]�w�Ѽ�(�öץX)
    public void InitializationSettings()
    {
        Debug.LogWarning("��l�Ƴ]�w��...");
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
        Debug.Log("�]�w�ɤw��l��");
    }

    //�O�s�]�w�Ѽ�
    public static void SaveSettingFile()
    {
        string settingsjson = JsonUtility.ToJson(settingFile, true);
        string exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");
        File.WriteAllText(exportFileName, settingsjson);
    }

    //�إ� ClearData �ɮ� [�ݥ[�K]
    private void CreateClearDataFile(string fileName)
    {
        Debug.Log("�إߤ�� " + "ClearLevelData.json");
        var newClearDataRoot = new ClearDataRoot();

        // �ɮ׸��|
        string exportFileName;
        if (fileName == "ClearLevelData")
        {
            //�w�]���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");
            newClearDataRoot.name = levelConfigFiles[0].name;
        }
        else
        {
            //��L���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData", "ClearLevelData_" + fileName + ".json");
            newClearDataRoot.name = fileName;
        }

        // �NRoot�ഫ��JSON�r�Ŧ�
        string newClearDataJson = JsonUtility.ToJson(newClearDataRoot, true);

        // �N JSON �r��g�J�ɮ�
        File.WriteAllText(exportFileName, newClearDataJson);
    }

    //���Ҧ� ClearData �� LevelConfig
    private void CompareAllClearData()
    {
        //���"�s�b"
        for (int i = 1; i < levelConfigFiles.Length; i++)
        {
            bool haveClearData = false;
            for (int j = 1; i < clearDataFiles.Length; j++)
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

        //���"����"
        for (int i = 0; i < levelConfigFiles.Length; i++)
        {
            for (int j = 0; i < clearDataFiles.Length; j++)
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

    //����@ ClearData ���� [�ݥ[�K]
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

        // �ഫ��JSON�r�Ŧ�
        string clearDataFilesJson = JsonUtility.ToJson(clearDataFiles[clearID], true);

        // �T�O�ؿ��s�b
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData");
        Directory.CreateDirectory(directoryPath);

        // �ɮ׸��|
        string exportFileName;
        if (clearID == 0)
        {
            //�w�]���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");
        }
        else
        {
            //��L���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData", "ClearLevelData_" + clearDataFiles[clearID].name + ".json");
        }

        // �g�J JSON ���ɮ�
        File.WriteAllText(exportFileName, clearDataFilesJson);

        Debug.Log("�q����T�����w��s�ëO�s�� " + exportFileName);
    }

    //�H nowLevelId �M�����d
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

    //�H nowFileId �M��L���ɮ�
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
            Debug.LogWarning("�䤣����ɮ�!");
        }
    }

    //�H nowLevelId �M��L�����
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

    //�H nowLevelData �� nowClearLevel �ˬd�O�_�����e�m���d
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

    //�H nowLevelId �O�s��e���d [�ݥ[�K]
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


        // �ɮ׸��|
        string exportFileName;
        if (nowFileId == 0)
        {
            //�w�]���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.json");
        }
        else
        {
            //��L���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData", "ClearLevelData_" + nowClearDataFile.name + ".json");
        }

        // �NRoot�ഫ��JSON�r�Ŧ�
        string newClearDataJson = JsonUtility.ToJson(nowClearDataFile, true);

        // �N JSON �r��g�J�ɮ�
        File.WriteAllText(exportFileName, newClearDataJson);
    }

    //�H nowLevelId �M��U�@��
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
                Debug.LogWarning("�L�k���o NextID " + nextID);
            }
            else if (nextID > MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig.Count)
            {
                nextLevelId = nowLevelId;
                Debug.LogWarning("�W�X���d�d�� " + nextID);
            }
            else if (nextID == MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig.Count)
            {
                nextLevelId = nowLevelId;
                Debug.Log("�w�g�O�̫�@��");
            }
            else if (nextID <= MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig.Count)
            {
                nextLevelId = MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig[nextID].levelID;
                Debug.Log("�U�@����: " + nextID + " / " + nextLevelId);
            }
        }
        else
        {
            nextLevelId = nowLevelData.nextLevelID;
        }

        nowLevelId = nextLevelId;
        CalibrationInfoByLevelId();
    }

    //�H nowLevelId �շǸ��
    public static void CalibrationInfoByLevelId()
    {
        FindLevelConfigById();
        FindClearLevelById();
    }


    //�ܼ�==========================================================================================================================
=======

    public static ClearDataRoot[] clearDataFiles;
    [SerializeField] private ClearDataRoot[] clearDataFilesOnGUI;   //DeBug - �Ω�GUI�w��

    public static SettingRoot settingFile;
    [SerializeField] private SettingRoot settingFileOnGUI;          //DeBug - �Ω�GUI�w��

    //�ɮ�ID (�Ω�P�ϥͦ�)
    public static int nowFileId;
    public static ClearDataRoot nowClearDataFile;

    //���dID (�Ω����d�ͦ�)
    public static string nowLevelId;
    public static LevelConfig nowLevelData;
    public static ClearLevel nowClearLevel;

    //Ū�ɥ�(���d)
    public void LoadLevelConfigFiles()
    {
        Debug.Log("Ū�����d�ɮ�...");
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "CustomLevels");

        if (!Directory.Exists(directoryPath))
        {
            Debug.LogWarning($"Directory not found: {directoryPath}");
            Directory.CreateDirectory(directoryPath);
        }

        string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");

        // i = 0  =>  �w�]���d
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

        Debug.Log($"�w���J {levelConfigFiles.Length} �����d�ɮ�");

        //�Ӷ��ȥΩ�}�o�w��
        levelConfigFilesOnGUI = levelConfigFiles;
    }

    // Ū�ɥ�(�L�����) [�ݥ[�K]
    public void LoadClearDataFiles()
    {
        Debug.Log("Ū���L�����...");
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData");

        if (!Directory.Exists(directoryPath))
        {
            Debug.LogWarning($"Directory not found: {directoryPath}");
            Directory.CreateDirectory(directoryPath);
        }

        string[] jsonFiles = Directory.GetFiles(directoryPath, "*.ejson");

        // i = 0  =>  �w�]���d
        clearDataFiles = new ClearDataRoot[jsonFiles.Length + 1];

        try
        {
            // �w�]���d�s��� PlayerData/ClearLevelData.json
            string defaultDirectoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
            string encryptedDefaultClearJson = File.ReadAllText(defaultDirectoryPath);
            string defaultClearJson = EncryptionUtility.DecryptString(encryptedDefaultClearJson);
            ClearDataRoot defaultClearDataRoot = JsonUtility.FromJson<ClearDataRoot>(defaultClearJson);
            clearDataFiles[0] = defaultClearDataRoot;
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarning("���`: �����ؿ�");

            Debug.Log("�إߥؿ� " + "PlayerData");
            string folderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            Directory.CreateDirectory(folderPath);
            CreateClearDataFile("ClearLevelData");

            // ���sŪ��
            string defaultDirectoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
            string encryptedDefaultClearJson = File.ReadAllText(defaultDirectoryPath);
            string defaultClearJson = EncryptionUtility.DecryptString(encryptedDefaultClearJson);
            ClearDataRoot defaultClearDataRoot = JsonUtility.FromJson<ClearDataRoot>(defaultClearJson);
            clearDataFiles[0] = defaultClearDataRoot;
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("���`: �������");
            CreateClearDataFile("ClearLevelData");

            // ���sŪ��
            string defaultDirectoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
            string encryptedDefaultClearJson = File.ReadAllText(defaultDirectoryPath);
            string defaultClearJson = EncryptionUtility.DecryptString(encryptedDefaultClearJson);
            ClearDataRoot defaultClearDataRoot = JsonUtility.FromJson<ClearDataRoot>(defaultClearJson);
            clearDataFiles[0] = defaultClearDataRoot;
        }
        finally
        {
            Debug.Log("�w�ѪR�w�]���d����");
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

        Debug.Log($"�w���J {clearDataFiles.Length} ���L������ɮ�");

        // �Ӷ��ȥΩ�}�o�w��
        clearDataFilesOnGUI = clearDataFiles;
    }

    //Ū�ɥ�(�]�w��)
    public void LoadSettingFile()
    {
        Debug.Log("Ū���]�w��...");
        string path = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");

        string jsonSetting = null;
        try
        {
            jsonSetting = File.ReadAllText(path);
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarning("���`: �����ؿ�");

            Debug.Log("�إߥؿ� " + "PlayerData");
            string folderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            Directory.CreateDirectory(folderPath);

            Debug.Log("�إߤ�� " + "SettingFile.json");
            InitializationSettings();
            jsonSetting = File.ReadAllText(path);
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("���`: �������");

            Debug.Log("�إߤ�� " + "SettingFile.json");
            InitializationSettings();
            jsonSetting = File.ReadAllText(path);
        }
        finally
        {
            settingFile = JsonUtility.FromJson<SettingRoot>(jsonSetting);
            Debug.Log("�w�ѪR�]�w�Ѽ�");
        }

        //�Ӷ��ȥΩ�}�o�w��
        settingFileOnGUI = settingFile;
    }

    //��l�Ƴ]�w�Ѽ�(�öץX)
    public void InitializationSettings()
    {
        Debug.LogWarning("��l�Ƴ]�w��...");
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
        Debug.Log("�]�w�ɤw��l��");
    }

    //�O�s�]�w�Ѽ�
    public static void SaveSettingFile()
    {
        string settingsjson = JsonUtility.ToJson(settingFile, true);
        string exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "SettingFile.json");
        File.WriteAllText(exportFileName, settingsjson);
    }

    // �إ� ClearData �ɮ� [�ݥ[�K]
    private void CreateClearDataFile(string fileName)
    {
        Debug.Log("�إߤ�� " + "ClearLevelData.ejson");
        var newClearDataRoot = new ClearDataRoot();

        // �ɮ׸��|
        string exportFileName;
        if (fileName == "ClearLevelData")
        {
            // �w�]���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
            newClearDataRoot.name = levelConfigFiles[0].name;
        }
        else
        {
            // ��L���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData", "ClearLevelData_" + fileName + ".ejson");
            newClearDataRoot.name = fileName;
        }

        // �NRoot�ഫ��JSON�r�Ŧ�
        string newClearDataJson = JsonUtility.ToJson(newClearDataRoot, true);

        // �N JSON �r��[�K�üg�J�ɮ�
        string encryptedClearDataJson = EncryptionUtility.EncryptString(newClearDataJson);
        File.WriteAllText(exportFileName, encryptedClearDataJson);
    }

    //���Ҧ� ClearData �� LevelConfig
    private void CompareAllClearData()
    {
        //���"�s�b"
        for (int i = 1; i < levelConfigFiles.Length; i++)
        {
            bool haveClearData = false;
            for (int j = 1; i < clearDataFiles.Length; j++)
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

        //���"����"
        for (int i = 0; i < levelConfigFiles.Length; i++)
        {
            for (int j = 0; i < clearDataFiles.Length; j++)
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

    // ����@ ClearData ���� [�ݥ[�K]
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

        // �ഫ��JSON�r�Ŧ�
        string clearDataFilesJson = JsonUtility.ToJson(clearDataFiles[clearID], true);

        // �T�O�ؿ��s�b
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData");
        Directory.CreateDirectory(directoryPath);

        // �ɮ׸��|
        string exportFileName;
        if (clearID == 0)
        {
            // �w�]���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
        }
        else
        {
            // ��L���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData", "ClearLevelData_" + clearDataFiles[clearID].name + ".ejson");
        }

        // �[�K�üg�J JSON ���ɮ�
        string encryptedClearDataFilesJson = EncryptionUtility.EncryptString(clearDataFilesJson);
        File.WriteAllText(exportFileName, encryptedClearDataFilesJson);

        Debug.Log("�q����T�����w��s�ëO�s�� " + exportFileName);
    }

    //�H nowLevelId �M�����d
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

    //�H nowFileId �M��L���ɮ�
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
            Debug.LogWarning("�䤣����ɮ�!");
        }
    }

    //�H nowLevelId �M��L�����
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

    //�H nowLevelData �� nowClearLevel �ˬd�O�_�����e�m���d
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

    // �H nowLevelId �O�s��e���d [�ݥ[�K]
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

        // �ɮ׸��|
        string exportFileName;
        if (nowFileId == 0)
        {
            // �w�]���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearLevelData.ejson");
        }
        else
        {
            // ��L���d
            exportFileName = Path.Combine(Application.persistentDataPath, "PlayerData", "ClearData", "ClearLevelData_" + nowClearDataFile.name + ".ejson");
        }

        // �NRoot�ഫ��JSON�r�Ŧ�
        string newClearDataJson = JsonUtility.ToJson(nowClearDataFile, true);

        // �[�K�üg�J JSON �r����ɮ�
        string encryptedClearDataJson = EncryptionUtility.EncryptString(newClearDataJson);
        File.WriteAllText(exportFileName, encryptedClearDataJson);
    }

    //�H nowLevelId �M��U�@��
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
                Debug.LogWarning("�L�k���o NextID " + nextID);
            }
            else if (nextID > MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig.Count)
            {
                nextLevelId = nowLevelId;
                Debug.LogWarning("�W�X���d�d�� " + nextID);
            }
            else if (nextID == MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig.Count)
            {
                nextLevelId = nowLevelId;
                Debug.Log("�w�g�O�̫�@��");
            }
            else if (nextID <= MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig.Count)
            {
                nextLevelId = MainManager.levelConfigFiles[MainManager.nowFileId].levelConfig[nextID].levelID;
                Debug.Log("�U�@����: " + nextID + " / " + nextLevelId);
            }
        }
        else
        {
            nextLevelId = nowLevelData.nextLevelID;
        }

        nowLevelId = nextLevelId;
        CalibrationInfoByLevelId();
    }

    //�H nowLevelId �շǸ��
    public static void CalibrationInfoByLevelId()
    {
        FindLevelConfigById();
        FindClearLevelById();
    }


>>>>>>> Stashed changes


    //���T
    public AudioSource soundEffectUiTrue;
    public AudioSource soundEffectUiFalse;
    public AudioSource soundEffectUiPage;

    public AudioSource[] bgmMusic;

    //�B��
    public int nowLevel = 0;


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

        //�ɮ�
        jsonDefaultLevels = Resources.Load<TextAsset>("Data/DefaultLevels");

        //�s��
        LoadLevelConfigFiles();
        LoadClearDataFiles();
        LoadSettingFile();
        CompareAllClearData();


        UpdateAudio();

        Debug.Log("MainManager ��l�Ƨ���");
    }

    void Start()
    {
        StartCoroutine(StartAfterAllObjectsLoaded());
    }

    IEnumerator StartAfterAllObjectsLoaded()
    {
        // ����1��A�Ҧ�����[����������檺�N�X
        yield return new WaitForSeconds(1);

        bgmMusic[MainManager.settingFile.bgmId].gameObject.SetActive(true);
    }

    void Update()
    {

    }

    //��s���q�j�p
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


    //�����ഫ==========================================================================================================================

    //����(SelectLevelButton)-�i�J���d
    public void EnterGameScene()
    {
        // ���J GameScene
        SceneManager.LoadScene("GameScene");
    }
}
