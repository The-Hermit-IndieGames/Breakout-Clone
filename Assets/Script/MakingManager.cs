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
    }

    [Serializable]
    public class LevelConfig
    {
        //��@���d���: ���d�W�١B�e�m���dID�B�C���Ҧ�(Normal�B?�B?)�B�������s�y��(x�By)�B�������s����B�j���C��(BricksData)
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

    private MainManager mainManager;

    //�ɮ�
    [SerializeField] private TextAsset jsonDefaultLevels;

    //�Ѽ�
    public TMP_InputField inputLevelsName;                                  //��J��-���d�W
    public string levelsName = "Null";                                      //���d�W
    public string exportFileName;                                           //�O�s��JSON���W
    public GameObject brickPrefab;                                          //�j�����w�m��
    public TMP_InputField inputBrickLevel;                                  //��J��-�j������
    public TMP_InputField inputPointValue;                                  //��J��-�}�a����
    public TextMeshProUGUI textBrickLevel;                                  //�ƭ�-�j������
    public TextMeshProUGUI textPointValue;                                  //�ƭ�-�}�a����

    //�B��
    private List<GameObject> bricksInScene = new List<GameObject>();                        //���������j���C��
    [SerializeField] private Transform bricksList;                                          //�j���C��(���Шt)

    private BrickMake nowBrick;

    //icon
    [SerializeField] private GameObject[] powerUpIcons;

    //�ª� �ݲM�z!!!  DefaultLevels JSON�t�m���
    [SerializeField] private Root defaultLevelsData;

    //JSON�t�m���
    [SerializeField] private Root rootLevelsData;
    [SerializeField] private LevelConfig nowLevelsData;
    [SerializeField] private List<BricksData> bricksDataList;

    //��J����
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

                    //��J��
                    textBrickLevel.text = brick.brickLevel.ToString();
                    inputBrickLevel.text = "";
                    inputPointValue.text = "";

                    ItemButtonUpdate();
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
                ItemButtonUpdate();
            }
        }
    }

    //�D����s-�ק�ϥ�
    void ItemButtonUpdate()
    {
        //�ק�ϥ�
        if (nowBrick.powerUpType > 5 || nowBrick.powerUpType < 0)
        {
            Debug.LogWarning("������Item����: " + nowBrick.powerUpType);
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


    ////�ץX JSON ���s(�w�]���d�M��!!)
    //public void ExportDefaultLevels()
    //{
    //    if (defaultLevelsData.levelConfig.Count == 0)
    //    {
    //        Debug.Log("��Ҭ��� �I�sLoadDefaultLevels");
    //        LoadDefaultLevels();
    //    }


    //    // �Ыؤ@�ӷs��LevelConfig��H
    //    LevelConfig levelConfig = new LevelConfig();

    //    levelConfig.levelName = "Level." + levelsName;
    //    levelConfig.gameType = "Time";
    //    levelConfig.menuX = -1.0f;
    //    levelConfig.menuY = -1.0f;
    //    levelConfig.menuStyle = -1;
    //    levelConfig.bricksData = new List<BricksData>();

    //    // �������������j���ƾ�
    //    foreach (GameObject brick in bricksInScene)
    //    {
    //        // ����j���}��
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

    //    //��� levelName �˴��O�_�X�{����
    //    for (int i = 0; i < defaultLevelsData.levelConfig.Count; i++)
    //    {
    //        if (defaultLevelsData.levelConfig[i].levelName == ("Level." + levelsName))
    //        {
    //            //�X�{���� --> ���N
    //            defaultLevelsData.levelConfig[i] = levelConfig;
    //            break;
    //        }
    //        else if (i == (defaultLevelsData.levelConfig.Count - 1))
    //        {
    //            //�L���� --> �K�[LevelConfig��Root
    //            defaultLevelsData.levelConfig.Add(levelConfig);
    //        }
    //    }

    //    if (defaultLevelsData.levelConfig.Count == 0)
    //    {
    //        //�Ŷ��X�� --> �K�[LevelConfig��Root
    //        defaultLevelsData.levelConfig.Add(levelConfig);
    //    }

    //    // �ഫ��JSON�r�Ŧ�
    //    string defaultLevelsDataJson = JsonUtility.ToJson(defaultLevelsData, true);

    //    // �T�O�ؿ��s�b
    //    string directoryPath = Application.dataPath + "/Resources/Data/";
    //    Directory.CreateDirectory(directoryPath);

    //    // �g�J JSON ���ɮ�
    //    string filePath = directoryPath + "DefaultLevels.json";
    //    File.WriteAllText(filePath, defaultLevelsDataJson);

    //    Debug.Log("���d�t�m�w�O�s�� " + filePath);

    //    // ���s���J�귽
    //    Resources.UnloadAsset(Resources.Load("Data/DefaultLevels"));

    //    // ���J�s���귽
    //    TextAsset newJsonFile = Resources.Load<TextAsset>("Data/example");
    //}


    ////�פJ JSON ���s(�w�]���d�M��!!)
    //public void LoadDefaultLevels()
    //{
    //    if (jsonDefaultLevels != null)
    //    {
    //        // �ഫ JSON �r�ꬰ������ Root ���
    //        defaultLevelsData = JsonUtility.FromJson<Root>(jsonDefaultLevels.text);

    //        if (defaultLevelsData != null)
    //        {
    //            Debug.Log("�w���\���J�]�w");
    //        }
    //        else
    //        {
    //            Debug.LogWarning("�L�k���J JSON �t�m���A���s�إ�");
    //            // �Ыؤ@�ӷs��Root��H
    //            defaultLevelsData = new Root();
    //            defaultLevelsData.levelConfig = new List<LevelConfig>();
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("����� JSON �t�m���A���s�إ�");
    //        // �Ыؤ@�ӷs��Root��H
    //        defaultLevelsData = new Root();
    //        defaultLevelsData.levelConfig = new List<LevelConfig>();
    //    }
    //}



    //�s��UI ==============================================================================================================================


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
        rootLevelsData.name = fileName;
        rootLevelsData.maker = makerName;
        rootLevelsData.version = version;
        rootLevelsData.description = description;
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
        string filePath = Path.Combine(directoryPath, fileName + ".json");
        File.WriteAllText(filePath, rootLevelsDataJson);

        Debug.Log("���d�t�m�w�O�s�� " + filePath);
    }


    //�ɮװt�m�O��-�j�M JSON ���s
    public void SearchLevelsJson()
    {
        if (jsonDefaultLevels != null)
        {
            // �ഫ JSON �r�ꬰ������ Root ���
            defaultLevelsData = JsonUtility.FromJson<Root>(jsonDefaultLevels.text);

            if (defaultLevelsData != null)
            {
                Debug.Log("�w���\���J�]�w");
            }
            else
            {
                Debug.LogWarning("�L�k���J JSON �t�m���A���s�إ�");
                // �Ыؤ@�ӷs��Root��H
                defaultLevelsData = new Root();
                defaultLevelsData.levelConfig = new List<LevelConfig>();
            }
        }
        else
        {
            Debug.LogWarning("����� JSON �t�m���A���s�إ�");
            // �Ыؤ@�ӷs��Root��H
            defaultLevelsData = new Root();
            defaultLevelsData.levelConfig = new List<LevelConfig>();
        }
    }


    //�ɮװt�m�O��-��J��
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



    //���d�t�m�O��-�s�W-----------------------------------------------------------------------------------------------------
    public void MapAddLevel()
    {

    }

    //���d�t�m�O��-Ū��
    public void MapLoadLevel()
    {

    }

    //���d�t�m�O��-�O�s
    public void MapSaveLevel()
    {

    }

    //���d�t�m�O��-�R��
    public void MapDeleteLevel()
    {

    }


    //���d�t�m�O��-��J���d�W��
    public void MapLevelNameInput()
    {
        nowLevelName = inputNowLevelName.text;
    }


    //���d�t�m�O��-���s�˦�
    public void MapButtonType()
    {

    }


    //���d�t�m�O��-�e�m���d-��J-------------------------------------------------------------------------------------------
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
        ShowWarningCanva("BricksRestart", "OOOOO");
    }
    public void BricksRestartNext()
    {
        BricksDelete();
        BricksGenerateNew();
    }

    //�j���t�m�O��-���J�j��
    public void BricksLoad()
    {
        BricksDelete();
        BricksGenerate();
    }

    //�j���t�m�O��-�x�s�j��
    public void BricksSave()
    {
        BricksAnalyze();
    }

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

    //�s���ާ@ ==============================================================================================================================

    //�j���ͦ���
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

            foreach (var brickData in bricksDataList)
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
                    Debug.LogWarning("�������j������:" + brickScript.brickType + "  ( " + brickScript.xPoint + " , " + brickScript.yPoint + " )");
                    brickData.brickType = "Null";
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
}