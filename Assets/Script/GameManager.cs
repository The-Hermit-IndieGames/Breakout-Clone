using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData
{
    public static string levelName;

    public static string gameType = "Time";             //�p������: Time / Score

    public static bool gameRunning = true;
    public static bool gameStarted = false;
    public static bool gameOver = false;

    public static float initialSpeed;                   //��l�t��
    public static float maxSpeed;                       //�t�פW��
    public static float speedIncreaseFactor;            //�C���I���᪺�t�״��ɦ]�l

    public static float boundaryX = 21f;                // ������ɭ���

    public static int score = 0;                        //�O���O
    public static float startTime = 0;                  //�O�ɾ�
    public static string timerString;
    public static float saveTime;

    public static int totalBalls = 0;                   //�u�]��

    public static bool burstBall = false;               //�z���u�]
}

public class GameManager : MonoBehaviour
{
    //�}��
    private MainManager mainManager;

    //�j��
    [SerializeField] private GameObject brickPrefab;                // �j�����w�m��
    [SerializeField] private Transform brickList;

    //UI
    [SerializeField] private GameObject PauseButton;                // �Ȱ�����

    [SerializeField] private TextMeshProUGUI scoreText;                               // �D�O���O
    [SerializeField] private TextMeshProUGUI timerText;                               // �D�p�ɾ�

    [SerializeField] private GameObject pauseUI;                    // �Ȱ��e��UI
    [SerializeField] private TextMeshProUGUI pauseTextName;
    [SerializeField] private TextMeshProUGUI pauseTextScore;
    [SerializeField] private TextMeshProUGUI pauseTextTimer;
    [SerializeField] private TextMeshProUGUI pauseTextSpeed;

    [SerializeField] private GameObject gameOverUI;                 // �C�������e��UI
    [SerializeField] private TextMeshProUGUI gameOverTextName;
    [SerializeField] private TextMeshProUGUI gameOverTextScore;
    [SerializeField] private TextMeshProUGUI gameOverTextTimer;
    [SerializeField] private TextMeshProUGUI gameOverTextSpeed;

    [SerializeField] private GameObject gameClearedUI;              // �C���L���e��UI
    [SerializeField] private TextMeshProUGUI gameClearedTextName;
    [SerializeField] private TextMeshProUGUI gameClearedTextScore;
    [SerializeField] private TextMeshProUGUI gameClearedTextTimer;
    [SerializeField] private TextMeshProUGUI gameClearedTextSpeed;


    public GameObject paddle;                   // �ƪO
    public GameObject longPaddle;               // �j�ƪO

    //�g�J�Ѽ�
    public int selectedLevel;                   // ���w�n�ͦ������d�s�� (�D���x���w)

    //�B��
    [SerializeField] private Transform bricksList;                                          //�j���C��(���Шt)
    MainManager.LevelConfig targetLevelConfig;              //���d���
    public int brickAmount;
    private Coroutine nowItem2;
    private Coroutine nowItem3;


    void Start()
    {
        //��l��
        Initialization();

        //�ͦ��j��
        LoadGenerate();
    }


    void Update()
    {
        if (GameData.gameStarted)
        {
            UpdateTimer();
        }
    }

    //��l��
    void Initialization()
    {
        Time.timeScale = 1f;

        GameData.gameRunning = true;
        GameData.gameStarted = false;
        GameData.gameOver = false;

        //���o���d��
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        selectedLevel = mainManager.nowLevel;
        GameData.levelName = mainManager.defaultLevelsRoot.levelConfig[mainManager.nowLevel].levelName;

        //��l�t�סA�t�פW���A�C���I���᪺�t�״��ɦ]�l
        GameData.initialSpeed = GameSetting.gameSpeedModifier * 15f;
        GameData.maxSpeed = GameSetting.gameSpeedModifier * 30f;
        GameData.speedIncreaseFactor = 1.1f;

        GameData.score = 0;                         //�O���O
        GameData.startTime = 0;                     //�O�ɾ�

        GameData.totalBalls = 0;                    //�u�]��

        GameData.burstBall = false;                 //�D��: �z���u�]

        longPaddle.gameObject.SetActive(false);     //�D��: �[���O
        paddle.gameObject.SetActive(true);

        GameData.boundaryX = 21f;                   //������ɭ���

        brickAmount = 0;

        //��l�ưO���O
        scoreText.text = "Score: " + GameData.score.ToString();

        //��l�ƭp�ɾ�(��ܮɶ�=�ثe�ɶ�-�}�l�ɶ�)
        GameData.startTime = Time.time;

        Debug.Log("�w��l�����d");
    }


    //���J�ͦ���(�۰ʩI�s->�j���ͦ���)
    void LoadGenerate()
    {
        if (mainManager.defaultLevelsRoot != null)
        {
            // �ഫ JSON �r�ꬰ������ Root ���
            var root = mainManager.defaultLevelsRoot;

            //�������d���(�ֹ����d�s��)

            if (root.levelConfig[selectedLevel] != null)
            {
                if (root.levelConfig[selectedLevel].levelName != null)
                {
                    GameData.levelName = root.levelConfig[selectedLevel].levelName;
                }
                else
                {
                    GameData.levelName = selectedLevel.ToString();
                }

                if (root.levelConfig[selectedLevel].gameType == "Time" || root.levelConfig[selectedLevel].gameType == "Score")
                {
                    GameData.gameType = root.levelConfig[selectedLevel].gameType;
                }
                else
                {
                    GameData.gameType = "Time";
                }

                targetLevelConfig = root.levelConfig[selectedLevel];
            }


            //�Ұʥͦ���
            if (targetLevelConfig != null)
            {
                GenerateBricks(targetLevelConfig.bricksData);
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


    //�j���ͦ���
    void GenerateBricks(List<MainManager.BricksData> bricks)
    {
        foreach (var brickData in bricks)
        {
            Vector3 position = new Vector3(26 - (4 * brickData.xPoint), 24.5f - brickData.yPoint, 0);
            GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity, bricksList);

            // �]�m�j�����ݩ�
            var brickScript = brick.GetComponent<Brick>();
            if (brickScript != null)
            {
                brickScript.pointValue = brickData.pointValue;
                brickScript.brickLevel = brickData.brickLevel;
                brickScript.powerUpType = brickData.powerUpType;
            }

            brickAmount += 1;
        }
    }


    //�p�ɾ�
    public void UpdateTimer()
    {
        //�p��L�h���ɶ�
        GameData.saveTime = Time.time - GameData.startTime;

        //�N�`����ഫ�������M��
        int minutes = Mathf.FloorToInt(GameData.saveTime / 60);
        int seconds = Mathf.FloorToInt(GameData.saveTime % 60);

        //�N�ɶ��榡�Ƭ����G��
        GameData.timerString = string.Format("{0:00}:{1:00}", minutes, seconds);

        //�N�ɶ���ܦb�����奻�W
        timerText.text = GameData.timerString;
    }


    //��s����
    public void UpdateScore(int amount)
    {
        GameData.score += amount;
        scoreText.text = "Score: " + GameData.score.ToString();
    }


    //�Ȱ����s
    public void PauseButtonClick()
    {
        GameData.gameRunning = false;
        pauseUI.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);

        pauseTextName.text = GameData.levelName;
        pauseTextTimer.text = GameData.timerString;
        pauseTextSpeed.text = GameSetting.gameSpeedModifier.ToString();
        pauseTextScore.text = "Score: " + GameData.score.ToString();

        // �C���Ȱ��A�N�ɶ��ᵲ
        Time.timeScale = 0f;
    }


    //�~����s
    public void ContinueButtonClick()
    {
        GameData.gameRunning = true;
        pauseUI.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(true);

        switch (GameData.gameType)
        {
            case "Time":
                timerText.gameObject.SetActive(true);
                break;
            case "Score":
                scoreText.gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("������Type����: " + GameData.gameType);
                break;
        }

        // �C���~��A�N�ɶ��ѭ�
        Time.timeScale = 1f;
    }


    //���s�}�l���s
    public void RestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //�h�X���s
    public void BackButtonClick()
    {
        // ���J MenuScene
        SceneManager.LoadScene("MenuScene");
    }


    //�C������
    public void GameOver()
    {
        gameOverUI.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);

        gameOverTextName.text = GameData.levelName;
        gameOverTextTimer.text = GameData.timerString;
        gameOverTextSpeed.text = GameSetting.gameSpeedModifier.ToString();
        gameOverTextScore.text = "Score: " + GameData.score.ToString();

        GameData.gameRunning = false;
        GameData.gameOver = true;

        //�ɶ��ᵲ
        Time.timeScale = 0f;
    }


    //���s�}�l���s
    public void NextButtonClick()
    {
        if ((mainManager.nowLevel + 1) < mainManager.defaultLevelsRoot.levelConfig.Count)
        {
            mainManager.nowLevel += 1;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //�C���L��
    public void GameCleared()
    {
        gameClearedUI.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);

        gameClearedTextName.text = GameData.levelName;
        gameClearedTextTimer.text = GameData.timerString;
        gameClearedTextSpeed.text = GameSetting.gameSpeedModifier.ToString();
        gameClearedTextScore.text = "Score: " + GameData.score.ToString();

        GameData.gameRunning = false;
        GameData.gameOver = true;
        Time.timeScale = 0f;
        mainManager.GameClearedSave();
    }


    //�D��2
    public void ItemLongPaddle()
    {
        paddle.gameObject.SetActive(false);
        longPaddle.gameObject.SetActive(true);
        GameData.boundaryX = 18f;
        if (nowItem2 != null)
        {
            StopCoroutine(nowItem2);
            nowItem2 = null;
        }
        nowItem2 = StartCoroutine(Item2());
    }

    IEnumerator Item2()
    {
        yield return new WaitForSeconds(27f);

        Debug.Log("ItemLongPaddle 3s");
        yield return new WaitForSeconds(1f);

        Debug.Log("ItemLongPaddle 2s");
        yield return new WaitForSeconds(1f);

        Debug.Log("ItemLongPaddle 1s");
        yield return new WaitForSeconds(1f);

        longPaddle.gameObject.SetActive(false);
        paddle.gameObject.SetActive(true);
        GameData.boundaryX = 21f;
    }


    //�D��3
    public void ItemBurstBall()
    {
        GameData.burstBall = true;
        if (nowItem3 != null)
        {
            StopCoroutine(nowItem3);
            nowItem3 = null;
        }
        nowItem3 = StartCoroutine(Item3());
    }

    IEnumerator Item3()
    {
        yield return new WaitForSeconds(7f);

        Debug.Log("ItemBurstBall 3s");
        yield return new WaitForSeconds(1f);

        Debug.Log("ItemBurstBall 2s");
        yield return new WaitForSeconds(1f);

        Debug.Log("ItemBurstBall 1s");
        yield return new WaitForSeconds(1f);

        GameData.burstBall = false;
    }
}
