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


    private MainManager mainManager;

    //�w�m��
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private GameObject brickUnbreakablePrefab;

    //JSON�t�m���
    [SerializeField] private Root rootLevelsData;
    [SerializeField] private LevelConfig nowLevelsData;
    [SerializeField] private List<BricksData> bricksDataList;

    [SerializeField] private GameObject warningCanva;
    [SerializeField] private TextMeshProUGUI warningTextEN;
    [SerializeField] private TextMeshProUGUI warningTextCH;

    //�B��
    private List<GameObject> buttonInMap = new List<GameObject>();                          //�a�Ϥ������s�C��
    private List<GameObject> bricksInScene = new List<GameObject>();                        //���������j���C��

    [SerializeField] private Transform buttonsMap;                                          //���s�C��(���Шt)
    [SerializeField] private Transform bricksList;                                          //�j���C��(���Шt)

    private BrickMake nowBrick;


    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        StartCoroutine(StartAfterAllObjectsLoaded());
    }

    IEnumerator StartAfterAllObjectsLoaded()
    {
        // ����1��A�Ҧ�����[����������檺�N�X
        yield return new WaitForSeconds(1);

        AdsPlatformIntegration.AdBanner_Hide();
    }


    void Update()
    {
        // �ˬd�ƹ������I���ƥ�
        if (Input.GetMouseButtonDown(0)) // ����
        {
            HandleLeftClick();
        }
        else if (Input.GetMouseButtonDown(1)) // �k��
        {
            HandleRightClick();
        }
    }

    //����
    void HandleLeftClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // �ϥήg�u�˴��ƹ��I������m
        if (Physics.Raycast(ray, out hit))
        {
            // �ˬd�O�_�I����F Brick ����
            BrickMake brick = hit.collider.GetComponent<BrickMake>();
            if (brick != null)
            {
                //���wnowBrick�Ω������
                nowBrick = brick;

                if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
                {
                    //�ק�������
                    brick.brickType += 1;
                    if (brick.brickType >= 2)
                    {
                        brick.brickType = 0;
                    }
                    brick.UpdateBrickColor();
                }
                else
                {
                    //�ק������šB��s brickLevel
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

    //�k��
    void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // �ϥήg�u�˴��ƹ��I������m
        if (Physics.Raycast(ray, out hit))
        {
            // �ˬd�O�_�I����F Brick ����
            BrickMake brick = hit.collider.GetComponent<BrickMake>();
            if (brick != null)
            {
                //���wnowBrick�Ω������
                nowBrick = brick;

                brick.UpdateItem();     //��s powerUpType
            }
        }
    }



    //�s��UI ==============================================================================================================================

    //ĵ�i�e��--------------------------------------------------------------------------------------------------------------

    //�w�q�@�өe�U�����A�Ω��ܨS���ѼƩM�S����^�Ȫ���k
    delegate void Action();

    //�x�s����ʧ@���e�U�ܼ�
    private Action nextAction;

    //���UI
    void ShowWarningCanva(string textEN, string textZH)
    {
        warningCanva.SetActive(true);
        warningTextEN.text = textEN;
        warningTextCH.text = textZH;
    }

    // UI��ܦ^�� YES/NO
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



    //�D�O��-�h�X���s ---------------------------------------------------------------------------------------------------------
    public void BackButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        // ���J MenuScene
        SceneManager.LoadScene("MenuScene");
    }



    //�ɮװt�m�O��-�s�W JSON ���s ---------------------------------------------------------------------------------------------
    public void NewLevelsJson()
    {
        rootLevelsData = new Root();
    }


    //�ɮװt�m�O��-��s JSON ���
    public void UpdateLevelsJson()
    {
        //��ƶ��h-�ɮ׸�T
        OnInputFileName();
        OnInputMakerName();
        OnInputVersion();
        OnInputDescription();
    }

    //���d�t�m���O�s(rootLevelsData.levelConfig << nowLevelsData)
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

    //���d�t�m�����J(rootLevelsData.levelConfig >> nowLevelsData)
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


    //�ɮװt�m�O��-�ץX JSON ���s
    public void ExportLevelsJson()
    {
        UpdateLevelsJson();

        // �ഫ��JSON�r�Ŧ�
        string rootLevelsDataJson = JsonUtility.ToJson(rootLevelsData, true);

        // �T�O�ؿ��s�b
        string directoryPath = Path.Combine(Application.persistentDataPath, "PlayerData", "CustomLevels");
        Directory.CreateDirectory(directoryPath);

        // �g�J JSON ���ɮ�
        string filePath = Path.Combine(directoryPath, rootLevelsData.name + ".json");
        File.WriteAllText(filePath, rootLevelsDataJson);

        Debug.Log("���d�t�m�w�O�s�� " + filePath);
    }


    //�ɮװt�m�O��-�j�M JSON ���s
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


    //�ɮװt�m�O��-�ѪR JSON ��UI
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

            Debug.Log("�ɮ�: " + rootLevelsData.name + ".json ���J����!");
        }
    }



    //��J����
    public TMP_InputField inputFileName;
    public TMP_InputField inputMakerName;
    public TMP_InputField inputVersion;
    public TMP_InputField inputDescription;

    public TMP_InputField inputSearchFileName;
    private string searchFileName;


    //�ɮװt�m�O��-��J��
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



    //���d�t�m�O��----------------------------------------------------------------------------------------------------------

    public GameObject nowButton;

    public TMP_InputField inputNowLevelName;
    public TMP_InputField inputPrerequisitesLevel;
    private string prerequisitesLevel;

    //Map-�s�W���d
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
        // �ϥΦr��榡�ƱN��� ID �ഫ���a���T�w��ƪ� ID �r��
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

    //��s�e����T
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

    //���d�t�m�O��-Ū��
    public void MapLoadLevel(string id)
    {
        SaveLevelToRoot();
        LoadRootToLevel(id);

        //��s�e����T
        UpDataMapLevel();
    }

    //���d�t�m�O��-�ƻsID
    public void CopyId()
    {
        GUIUtility.systemCopyBuffer = nowLevelsData.levelID;
        Debug.Log("Copy ID : " + nowLevelsData.levelID);
    }

    //���d�t�m�O��-�K�WID (�U�@��)
    public void PasteId()
    {
        nowLevelsData.nextLevelID = GUIUtility.systemCopyBuffer;
        nextIdText.text = nowLevelsData.nextLevelID;

        SaveLevelToRoot();
    }

    //���d�t�m�O��-�R�����d
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

    //���d�t�m�O��-��J���d�W��
    public void MapLevelNameInput()
    {
        nowLevelsData.levelName = inputNowLevelName.text;

        SaveLevelToRoot();
        UpDataMapLevel();
    }


    //���d�t�m�O��-���s�˦�
    public void MapButtonType()
    {

    }

    //���d�t�m�O��-��J�y��
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


    //���d�t�m�O��-�a��-��s�y��
    public void UpDataPosition(string id, Vector2 position)
    {
        MapLoadLevel(id);
        nowLevelsData.menuX = position.x;
        nowLevelsData.menuY = position.y;

        inputCoordinateX.text = nowLevelsData.menuX.ToString();
        inputCoordinateY.text = nowLevelsData.menuY.ToString();

        SaveLevelToRoot();
    }


    //���d�t�m�O��-�e�m���d-��J---------------------------
    public void PrerequisitesInput()
    {

    }

    //���d�t�m�O��-�e�m���d-�[�J
    public void PrerequisitesAdd()
    {

    }

    //���d�t�m�O��-�e�m���d-���
    public void PrerequisitesDropdown()
    {

    }

    //���d�t�m�O��-�e�m���d-�R��
    public void PrerequisitesDelete()
    {

    }





    //�j���t�m�O��-���s�j��-------------------------------------------------------------------------------------------
    public void BricksRestart()
    {
        nextAction = BricksRestartNext;
        ShowWarningCanva("Restart Bricks ?", "�T�w�n���m�e�������j����?");
    }

    public void BricksRestartNext()
    {
        BricksDelete();
        BricksGenerateNew();

        nextAction = null;
    }

    //�j���t�m�O��-���J�j��
    public void BricksLoadByTemp()
    {
        tempBricksText.text = ("Temp\n " + bricksDataList.Count + "\nBricks");

        BricksDelete();
        BricksGenerate(bricksDataList);
    }

    //�j���t�m�O��-�x�s�j��
    public void BricksSaveToTemp()
    {
        BricksAnalyze();
        tempBricksText.text = ("Temp\n " + bricksDataList.Count + "\nBricks");
        Debug.Log("BricksSave: �ѪR�j���ëO�s�� bricksDataList");
    }



    //�j���t�m�O��-���J�j��
    public void TempLoadByLevel()
    {
        //�קK�o�Ͷǧ}�I�s
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

    //�j���t�m�O��-�x�s�j��
    public void TempSaveToLevel()
    {
        //�קK�o�Ͷǧ}�I�s
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


    //�j���ͦ���(Ū��)
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

                    // �]�m�j�����ݩ�
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

                // �]�m�j�����ݩ�
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
                            Debug.LogWarning("�������j������:" + brickData.brickType + "  ( " + brickData.xPoint + " , " + brickData.yPoint + " )");
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

                // �]�m�j�����ݩ�
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


    //�j���ͦ���(�s)
    void BricksGenerateNew()
    {
        for (int x = 1; x <= 12; x++)
        {
            for (int y = 1; y <= 16; y++)
            {
                Vector3 brickPosition = new Vector3(26 - (4 * x), 24.5f - y, 0);
                GameObject brick = Instantiate(brickPrefab, brickPosition, Quaternion.identity, bricksList);
                bricksInScene.Add(brick);

                // �]�m�j�����ݩ�
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


    //�j���ѪR��
    void BricksAnalyze()
    {
        bricksDataList.Clear();

        // �������������j���ƾ�
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
                    Debug.LogWarning("�������j������:" + brickScript.brickType + "  ( " + brickScript.xPoint + " , " + brickScript.yPoint + " )");
                    break;
            }

        }
    }


    //�j���M����
    void BricksDelete()
    {
        // �M�� Transform �����C�Ӥl����
        for (int i = 0; i < bricksList.childCount; i++)
        {
            //����l���� Transform
            Transform childTransform = bricksList.GetChild(i);

            //����l���� GameObject
            GameObject childGameObject = childTransform.gameObject;

            Destroy(childGameObject);
            bricksInScene.Clear();
        }
    }



    //��l�D��--------------------------------------------------------------------------------------------------------


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