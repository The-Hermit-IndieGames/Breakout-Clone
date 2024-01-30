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

    public static string gameType = "Time";             //計分類型: Time / Score

    public static bool gameRunning = true;
    public static bool gameStarted = false;
    public static bool gameOver = false;

    public static float initialSpeed;                   //初始速度
    public static float maxSpeed;                       //速度上限
    public static float speedIncreaseFactor;            //每次碰撞後的速度提升因子

    public static float boundaryX = 21f;                // 移動邊界限制

    public static int score = 0;                        //記分板
    public static float startTime = 0;                  //記時器
    public static string timerString;
    public static float saveTime;

    public static int totalBalls = 0;                   //彈珠數

    public static bool burstBall = false;               //爆炸彈珠
}

public class GameManager : MonoBehaviour
{
    //腳本
    private MainManager mainManager;

    //磚塊
    [SerializeField] private GameObject brickPrefab;                // 磚塊的預置體
    [SerializeField] private Transform brickList;

    //UI
    [SerializeField] private GameObject PauseButton;                // 暫停按鍵

    [SerializeField] private TextMeshProUGUI scoreText;                               // 主記分板
    [SerializeField] private TextMeshProUGUI timerText;                               // 主計時器

    [SerializeField] private GameObject pauseUI;                    // 暫停畫面UI
    [SerializeField] private TextMeshProUGUI pauseTextName;
    [SerializeField] private TextMeshProUGUI pauseTextScore;
    [SerializeField] private TextMeshProUGUI pauseTextTimer;
    [SerializeField] private TextMeshProUGUI pauseTextSpeed;

    [SerializeField] private GameObject gameOverUI;                 // 遊戲結束畫面UI
    [SerializeField] private TextMeshProUGUI gameOverTextName;
    [SerializeField] private TextMeshProUGUI gameOverTextScore;
    [SerializeField] private TextMeshProUGUI gameOverTextTimer;
    [SerializeField] private TextMeshProUGUI gameOverTextSpeed;

    [SerializeField] private GameObject gameClearedUI;              // 遊戲過關畫面UI
    [SerializeField] private TextMeshProUGUI gameClearedTextName;
    [SerializeField] private TextMeshProUGUI gameClearedTextScore;
    [SerializeField] private TextMeshProUGUI gameClearedTextTimer;
    [SerializeField] private TextMeshProUGUI gameClearedTextSpeed;


    public GameObject paddle;                   // 滑板
    public GameObject longPaddle;               // 大滑板

    //寫入參數
    public int selectedLevel;                   // 指定要生成的關卡編號 (主控台指定)

    //運行
    [SerializeField] private Transform bricksList;                                          //磚塊列表(坐標系)
    MainManager.LevelConfig targetLevelConfig;              //關卡資料
    public int brickAmount;
    private Coroutine nowItem2;
    private Coroutine nowItem3;


    void Start()
    {
        //初始化
        Initialization();

        //生成磚塊
        LoadGenerate();
    }


    void Update()
    {
        if (GameData.gameStarted)
        {
            UpdateTimer();
        }
    }

    //初始化
    void Initialization()
    {
        Time.timeScale = 1f;

        GameData.gameRunning = true;
        GameData.gameStarted = false;
        GameData.gameOver = false;

        //取得關卡號
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        selectedLevel = mainManager.nowLevel;
        GameData.levelName = mainManager.defaultLevelsRoot.levelConfig[mainManager.nowLevel].levelName;

        //初始速度，速度上限，每次碰撞後的速度提升因子
        GameData.initialSpeed = GameSetting.gameSpeedModifier * 15f;
        GameData.maxSpeed = GameSetting.gameSpeedModifier * 30f;
        GameData.speedIncreaseFactor = 1.1f;

        GameData.score = 0;                         //記分板
        GameData.startTime = 0;                     //記時器

        GameData.totalBalls = 0;                    //彈珠數

        GameData.burstBall = false;                 //道具: 爆炸彈珠

        longPaddle.gameObject.SetActive(false);     //道具: 加長板
        paddle.gameObject.SetActive(true);

        GameData.boundaryX = 21f;                   //移動邊界限制

        brickAmount = 0;

        //初始化記分板
        scoreText.text = "Score: " + GameData.score.ToString();

        //初始化計時器(顯示時間=目前時間-開始時間)
        GameData.startTime = Time.time;

        Debug.Log("已初始化關卡");
    }


    //載入生成器(自動呼叫->磚塊生成器)
    void LoadGenerate()
    {
        if (mainManager.defaultLevelsRoot != null)
        {
            // 轉換 JSON 字串為對應的 Root 實例
            var root = mainManager.defaultLevelsRoot;

            //提取關卡資料(核對關卡編號)

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


            //啟動生成器
            if (targetLevelConfig != null)
            {
                GenerateBricks(targetLevelConfig.bricksData);
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


    //磚塊生成器
    void GenerateBricks(List<MainManager.BricksData> bricks)
    {
        foreach (var brickData in bricks)
        {
            Vector3 position = new Vector3(26 - (4 * brickData.xPoint), 24.5f - brickData.yPoint, 0);
            GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity, bricksList);

            // 設置磚塊的屬性
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


    //計時器
    public void UpdateTimer()
    {
        //計算過去的時間
        GameData.saveTime = Time.time - GameData.startTime;

        //將總秒數轉換為分鐘和秒
        int minutes = Mathf.FloorToInt(GameData.saveTime / 60);
        int seconds = Mathf.FloorToInt(GameData.saveTime % 60);

        //將時間格式化為分：秒
        GameData.timerString = string.Format("{0:00}:{1:00}", minutes, seconds);

        //將時間顯示在介面文本上
        timerText.text = GameData.timerString;
    }


    //更新分數
    public void UpdateScore(int amount)
    {
        GameData.score += amount;
        scoreText.text = "Score: " + GameData.score.ToString();
    }


    //暫停按鈕
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

        // 遊戲暫停，將時間凍結
        Time.timeScale = 0f;
    }


    //繼續按鈕
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
                Debug.LogWarning("未知的Type類型: " + GameData.gameType);
                break;
        }

        // 遊戲繼續，將時間解凍
        Time.timeScale = 1f;
    }


    //重新開始按鈕
    public void RestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //退出按鈕
    public void BackButtonClick()
    {
        // 載入 MenuScene
        SceneManager.LoadScene("MenuScene");
    }


    //遊戲結束
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

        //時間凍結
        Time.timeScale = 0f;
    }


    //重新開始按鈕
    public void NextButtonClick()
    {
        if ((mainManager.nowLevel + 1) < mainManager.defaultLevelsRoot.levelConfig.Count)
        {
            mainManager.nowLevel += 1;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //遊戲過關
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


    //道具2
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


    //道具3
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
