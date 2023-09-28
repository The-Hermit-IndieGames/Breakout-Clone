using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MakingManager : MonoBehaviour
{
    //�ǦC�Ʃw�q
    [Serializable]
    public class BricksData
    {
        public int xPoint;
        public int yPoint;
        public int pointValue;
        public int brickLevel;
        public int powerUpType;
    }

    [Serializable]
    public class LevelConfig
    {
        public string level;
        public List<BricksData> bricksData;
    }

    [Serializable]
    public class Root
    {
        public List<LevelConfig> levelConfig;
    }

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
    private Vector3 brickPosition;                                      //�j���y��
    private List<GameObject> bricksInScene = new List<GameObject>();    //���������j���C��
    private BrickMake nowBrick;


    void Start()
    {
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
                brick.UpdateItem();     //��s powerUpType
            }
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
                GameObject brick = Instantiate(brickPrefab, brickPosition, Quaternion.identity);
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
        // �Ыؤ@�ӷs��Root��H
        Root root = new Root();
        root.levelConfig = new List<LevelConfig>();

        // �Ыؤ@�ӷs��LevelConfig��H
        LevelConfig levelConfig = new LevelConfig();
        levelConfig.bricksData = new List<BricksData>();
        levelConfig.level = levelsName;

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

        // �K�[LevelConfig��Root
        root.levelConfig.Add(levelConfig);

        // �ഫ��JSON�r�Ŧ�
        string json = JsonUtility.ToJson(root, true);

        // �g�JJSON���
        exportFileName = levelsName + ".json";
        File.WriteAllText(exportFileName, json);

        Debug.Log("���d�t�m�w�O�s�� " + exportFileName);
    }


    //�h�X���s
    public void BackButtonClick()
    {
        // ���J MenuScene
        SceneManager.LoadScene("MenuScene");
    }
}
