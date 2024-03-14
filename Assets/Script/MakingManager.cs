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
        //��@���d���: ���d�W�١B�e�m���d�W��(NEW)�B�C���Ҧ�(Normal�B?�B?)�B�������s�y��(x�By)�B�������s����(?)�B�j���C��(BricksData)
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
        //��@�j�����: �j���y��(x�By)�B�j������(Normal�BUnbreakable)�B�j�������S�����
        public int xPoint;
        public int yPoint;
        public string brickType;
        public NormalBricks normalBricks;

        //�R
        public int pointValue;
        public int brickLevel;
        public int powerUpType;
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
    private Vector3 brickPosition;                                                          //�j���y��
    [SerializeField] private List<GameObject> bricksInScene = new List<GameObject>();       //���������j���C��
    [SerializeField] private Transform bricksList;                                          //�j���C��(���Шt)
    private BrickMake nowBrick;

    //icon
    [SerializeField] private GameObject[] powerUpIcons;

    //DefaultLevels JSON�t�m���
    [SerializeField] private Root defaultLevelsData;

    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        jsonDefaultLevels = Resources.Load<TextAsset>("Data/DefaultLevels");

        GenerateBricks();
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
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
                {
                    // �b�o��Ĳ�o�A�Q�n���檺�ާ@
                    Debug.Log("Shift + ����Q���U�I");

                    // �|�ҡG�i�H�b�o�̩I�s��L��k�ΰ����L�N�X
                }
                else
                {
                    //���wnowBrick�Ω������
                    nowBrick = brick;

                    //��s brickLevel
                    brick.brickLevel += 1;
                    if (brick.brickLevel >= 6)
                    {
                        brick.brickLevel = 0;
                    }
                    brick.UpdateLevel();

                    //��J��
                    textBrickLevel.text = brick.brickLevel.ToString();
                    textPointValue.text = brick.pointValue.ToString();
                    inputBrickLevel.text = "";
                    inputPointValue.text = "";

                    ItemButtonUpdate();
                }               
            }
        }
    }

    //��J��(����)
    public void OnInputBrickLevel()
    {
        //Ū�� TMP Input Field �����奻
        string inputText = inputBrickLevel.text;
        int.TryParse(inputText, out int inputNumber);

        nowBrick.brickLevel = inputNumber;
        inputPointValue.text = "";
        nowBrick.UpdateLevel();
        textBrickLevel.text = nowBrick.brickLevel.ToString();
        textPointValue.text = nowBrick.pointValue.ToString();
    }

    //��J��(����)
    public void OnInputBrickScore()
    {
        //Ū�� TMP Input Field �����奻
        string inputText = inputPointValue.text;
        int.TryParse(inputText, out int inputNumber);

        nowBrick.pointValue = inputNumber;
        textPointValue.text = nowBrick.pointValue.ToString();
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

    //�D����s
    public void OnItemButton()
    {
        nowBrick.UpdateItem();     //��s powerUpType

        ItemButtonUpdate();
    }

    //�D����s-�ק�ϥ�
    void ItemButtonUpdate()
    {
        //�ק�ϥ�
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
                Debug.LogWarning("������Item����: " + nowBrick.powerUpType);
                break;
        }
    }


    //�j���ͦ���
    void GenerateBricks()
    {
        for (int x = 1; x <= 12; x++)
        {
            for (int y = 1; y <= 16; y++)
            {
                brickPosition = new Vector3(26 - (4 * x), 24.5f - y, 0);
                GameObject brick = Instantiate(brickPrefab, brickPosition, Quaternion.identity, bricksList);
                bricksInScene.Add(brick);

                // �]�m�j�����ݩ�
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


    //���s�}�l���s
    public void RestartButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //��J��(���d�W)
    public void OnInputLevelsName()
    {
        //Ū�� TMP Input Field �����奻
        levelsName = inputLevelsName.text;
    }


    //�ץX JSON ���s
    public void ExportLevelConfigToJson()
    {
        mainManager.soundEffectUiTrue.Play();
        // �Ыؤ@�ӷs��Root��H
        Root root = new Root();
        root.levelConfig = new List<LevelConfig>();

        // �Ыؤ@�ӷs��LevelConfig��H
        LevelConfig levelConfig = new LevelConfig();

        levelConfig.levelName = "Custom_" + levelsName;
        levelConfig.gameType = "Time";
        levelConfig.menuX = -1.0f;
        levelConfig.menuY = -1.0f;
        levelConfig.menuStyle = -1;
        levelConfig.bricksData = new List<BricksData>();

        // �������������j���ƾ�
        foreach (GameObject brick in bricksInScene)
        {
            // ����j���}��
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

        // �K�[LevelConfig��Root
        root.levelConfig.Add(levelConfig);

        // �ഫ��JSON�r�Ŧ�
        string json = JsonUtility.ToJson(root, true);

        // �g�JJSON���
        string fileName = "Resources/PlayerData/CustomLevels/" + levelsName + ".json";

        exportFileName = Path.Combine(Application.dataPath, fileName);

        File.WriteAllText(exportFileName, json);

        Debug.Log("���d�t�m�w�O�s�� " + exportFileName);
    }


    //�ץX JSON ���s(�w�]���d�M��!!)
    public void ExportDefaultLevels()
    {
        if (defaultLevelsData.levelConfig.Count == 0)
        {
            Debug.Log("��Ҭ��� �I�sLoadDefaultLevels");
            LoadDefaultLevels();
        }


        // �Ыؤ@�ӷs��LevelConfig��H
        LevelConfig levelConfig = new LevelConfig();

        levelConfig.levelName = "Level." + levelsName;
        levelConfig.gameType = "Time";
        levelConfig.menuX = -1.0f;
        levelConfig.menuY = -1.0f;
        levelConfig.menuStyle = -1;
        levelConfig.bricksData = new List<BricksData>();

        // �������������j���ƾ�
        foreach (GameObject brick in bricksInScene)
        {
            // ����j���}��
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

        //��� levelName �˴��O�_�X�{����
        for (int i = 0; i < defaultLevelsData.levelConfig.Count; i++)
        {
            if (defaultLevelsData.levelConfig[i].levelName == ("Level." + levelsName))
            {
                //�X�{���� --> ���N
                defaultLevelsData.levelConfig[i] = levelConfig;
                break;
            }
            else if (i == (defaultLevelsData.levelConfig.Count - 1))
            {
                //�L���� --> �K�[LevelConfig��Root
                defaultLevelsData.levelConfig.Add(levelConfig);
            }
        }

        if (defaultLevelsData.levelConfig.Count == 0)
        {
            //�Ŷ��X�� --> �K�[LevelConfig��Root
            defaultLevelsData.levelConfig.Add(levelConfig);
        }

        // �ഫ��JSON�r�Ŧ�
        string defaultLevelsDataJson = JsonUtility.ToJson(defaultLevelsData, true);

        // �T�O�ؿ��s�b
        string directoryPath = Application.dataPath + "/Resources/Data/";
        Directory.CreateDirectory(directoryPath);

        // �g�J JSON ���ɮ�
        string filePath = directoryPath + "DefaultLevels.json";
        File.WriteAllText(filePath, defaultLevelsDataJson);

        Debug.Log("���d�t�m�w�O�s�� " + filePath);

        // ���s���J�귽
        Resources.UnloadAsset(Resources.Load("Data/DefaultLevels"));

        // ���J�s���귽
        TextAsset newJsonFile = Resources.Load<TextAsset>("Data/example");
    }


    //�פJ JSON ���s(�w�]���d�M��!!)
    public void LoadDefaultLevels()
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


    //�h�X���s
    public void BackButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        // ���J MenuScene
        SceneManager.LoadScene("MenuScene");
    }
}
