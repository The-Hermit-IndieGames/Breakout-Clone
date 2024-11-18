using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

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

    //Item: 1=彈珠+  2=加長版  3=爆炸彈珠 4=黑洞 5=移動平台
    public static int timerLongPaddle = 0;
    public static int timerBurstBall = 0;
    public static int timerBlackHole = 0;

    public static int noBreakTimer = 0;

    public static int totalBalls = 0;                //彈珠數

    public static bool longPaddle = false;
    public static bool burstBall = false;
    public static bool blackHole = false;
}

public class GameManager : MonoBehaviour
{
    //腳本
    private MainManager mainManager;

    //磚塊
    [SerializeField] private GameObject brickPrefab;                // 磚塊的預置體
    [SerializeField] private GameObject brickUnbreakablePrefab;
    [SerializeField] private Transform brickList;

    //VFX
    [SerializeField] private GameObject vfxStardustScore;

    //UI
    [SerializeField] private GameObject pauseButton;                // 暫停按鍵

    [SerializeField] private TextMeshProUGUI scoreText;                               // 主記分板
    [SerializeField] private TextMeshProUGUI timerText;                               // 主計時器
    [SerializeField] private TextMeshProUGUI infoText;                                // 提示

    [SerializeField] private GameObject pauseUI;                    // 暫停畫面UI
    [SerializeField] private TextMeshProUGUI pauseTextName;
    [SerializeField] private TextMeshProUGUI pauseTextScore;
    [SerializeField] private TextMeshProUGUI pauseTextTimer;
    [SerializeField] private TextMeshProUGUI pauseTextSpeed;

    [SerializeField] private GameObject gameOverUI;                 // 遊戲結束畫面UI
    [SerializeField] private GameObject adsButton_A;
    [SerializeField] private GameObject adsButton_B;
    [SerializeField] private GameObject AdsReward;
    [SerializeField] private TextMeshProUGUI gameOverTextName;
    [SerializeField] private TextMeshProUGUI gameOverTextScore;
    [SerializeField] private TextMeshProUGUI gameOverTextTimer;
    [SerializeField] private TextMeshProUGUI gameOverTextSpeed;

    [SerializeField] private GameObject gameClearedUI;              // 遊戲過關畫面UI
    [SerializeField] private TextMeshProUGUI gameClearedTextName;
    [SerializeField] private TextMeshProUGUI gameClearedTextScore;
    [SerializeField] private TextMeshProUGUI gameClearedTextTimer;
    [SerializeField] private TextMeshProUGUI gameClearedTextSpeed;

    [SerializeField] private Image progressBarImage;
    [SerializeField] private TextMeshProUGUI progressBarMinScore;
    [SerializeField] private TextMeshProUGUI progressBarMiddleScore;
    [SerializeField] private TextMeshProUGUI progressBarMaxScore;

    [SerializeField] private List<GameObject> iconMedals;


    //音訊
    public AudioSource soundEffectGetItem;
    [SerializeField] private AudioSource soundEffectGameOver;
    [SerializeField] private AudioSource soundEffectGameCleared;
    [SerializeField] private AudioSource soundEffectGameClearedPlus;

    [SerializeField] private AudioSource soundEffectBlackHole;


    //道具
    public GameObject paddle;                   //滑板
    public GameObject longPaddle;               //大滑板
    public GameObject blackHole;                //黑洞
    [SerializeField] private GameObject burstPaddlePrefab;
    [SerializeField] private List<GameObject> initialItem;

    //運行
    public static int brickAmount;
    private int minTotalScore = 150;
    private int maxTotalScore = 150;

    void Start()
    {
        AdsPlatformIntegration.AdBanner_Hide();

        //初始化
        Initialization();

        //生成磚塊
        LoadGenerate();

        StartCoroutine(ItemsTimer());
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

        //取得關卡號
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        GameData.levelName = MainManager.nowLevelData.levelName;

        //初始速度，速度上限，每次碰撞後的速度提升因子
        GameData.initialSpeed = MainManager.settingFile.gameSpeedModifier * 15f;
        GameData.maxSpeed = MainManager.settingFile.gameSpeedModifier * 30f;
        GameData.speedIncreaseFactor = 1.1f;

        GameData.score = 0;                         //記分板
        GameData.startTime = 0;                     //記時器

        GameData.totalBalls = 0;                    //彈珠數

        GameData.burstBall = false;                 //道具: 爆炸彈珠

        longPaddle.gameObject.SetActive(false);     //道具: 加長板
        paddle.gameObject.SetActive(true);

        GameData.boundaryX = 21f;                   //移動邊界限制

        brickAmount = 0;

        GameData.timerLongPaddle = 0;
        GameData.timerBurstBall = 0;
        GameData.timerBlackHole = 0;

        GameData.noBreakTimer = 0;

        GameData.longPaddle = false;
        GameData.burstBall = false;
        GameData.blackHole = false;

        //初始化記分板
        scoreText.text = "Score: " + GameData.score.ToString();

        //初始化計時器(顯示時間=目前時間-開始時間)
        GameData.startTime = Time.time;

        GameData.gameRunning = true;
        GameData.gameStarted = false;
        GameData.gameOver = false;

        soundEffectGetItem.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        soundEffectGameOver.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        soundEffectGameCleared.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        soundEffectGameClearedPlus.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        soundEffectBlackHole.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;

        Debug.Log("已初始化關卡");
    }


    //載入生成器(自動呼叫->磚塊生成器)
    void LoadGenerate()
    {
        //提取關卡資料
        if (MainManager.nowLevelData != null)
        {
            GameData.levelName = MainManager.nowLevelData.levelName;

            if (MainManager.nowLevelData.gameType == "Time" || MainManager.nowLevelData.gameType == "Score")
            {
                GameData.gameType = MainManager.nowLevelData.gameType;
            }
            else
            {
                GameData.gameType = "Time";
            }
        }


        //啟動生成器
        if (MainManager.nowLevelData.bricksData != null)
        {
            GenerateBricks();
        }
        else
        {
            Debug.LogWarning("指定的關卡編號不存在");
        }

    }


    //磚塊生成器
    void GenerateBricks()
    {
        //初始道具
        Debug.LogWarning("初始道具");
        if (MainManager.nowLevelData.initialItem.addBall == true)
        { maxTotalScore += 200; initialItem[0].SetActive(true); }
        if (MainManager.nowLevelData.initialItem.longPaddle == true)
        { maxTotalScore += 200; initialItem[1].SetActive(true); }
        if (MainManager.nowLevelData.initialItem.burstBall == true)
        { maxTotalScore += 200; initialItem[2].SetActive(true); }
        if (MainManager.nowLevelData.initialItem.blackHole == true)
        { maxTotalScore += 200; initialItem[3].SetActive(true); }
        if (MainManager.nowLevelData.initialItem.burstPaddle == true)
        { maxTotalScore += 200; initialItem[4].SetActive(true); }

        List<MainManager.BricksData> bricks = new List<MainManager.BricksData>(MainManager.nowLevelData.bricksData);

        //生成磚塊
        foreach (var brickData in bricks)
        {
            Vector3 position = new Vector3(26 - (4 * brickData.xPoint), 24.5f - brickData.yPoint, 0);

            if (brickData.brickType == "Normal")
            {
                GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity, brickList);

                // 設置磚塊的屬性
                var brickScript = brick.GetComponent<Brick>();
                if (brickScript != null)
                {
                    brickScript.brickLevel = brickData.normalBricks.brickLevel;
                    brickScript.powerUpType = brickData.normalBricks.powerUpType;
                }
                //記分計算
                brickAmount += 1;
                minTotalScore += brickData.normalBricks.brickLevel * 20;
                maxTotalScore += brickData.normalBricks.brickLevel * 20;
                if (brickData.normalBricks.powerUpType >= 1)
                {
                    maxTotalScore += 200;
                }
            }
            else if (brickData.brickType == "Unbreakable")
            {
                GameObject brick = Instantiate(brickUnbreakablePrefab, position, Quaternion.identity, brickList);
            }
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


    //遊戲開始
    public void GameStarted()
    {
        GameData.gameStarted = true;
        GameData.gameRunning = true;
        GameData.startTime = Time.time;

        pauseButton.gameObject.SetActive(true);
    }

    //暫停按鈕
    public void PauseButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        GameData.gameRunning = false;
        pauseUI.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);

        pauseTextName.text = GameData.levelName;
        pauseTextTimer.text = GameData.timerString;
        pauseTextSpeed.text = MainManager.settingFile.gameSpeedModifier.ToString();
        pauseTextScore.text = GameData.score.ToString();

        // 遊戲暫停，將時間凍結
        Time.timeScale = 0f;

        AdsPlatformIntegration.AdBanner_Show();
    }


    //繼續按鈕
    public void ContinueButtonClick()
    {
        AdsPlatformIntegration.AdBanner_Hide();

        mainManager.soundEffectUiTrue.Play();
        GameData.gameRunning = true;
        pauseUI.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);

        timerText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);

        //遊戲繼續，將時間解凍
        Time.timeScale = 1f;
    }


    //重新開始按鈕
    public void RestartButtonClick()
    {
        AdsPlatformIntegration.AdBanner_Hide();

        mainManager.soundEffectUiTrue.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //退出按鈕
    public void BackButtonClick()
    {
        AdsPlatformIntegration.AdBanner_Hide();

        mainManager.soundEffectUiTrue.Play();
        Time.timeScale = 1f;

        // 載入 MenuScene
        SceneManager.LoadScene("MenuScene");
    }


    //遊戲失敗
    public void GameOver()
    {
        gameOverUI.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);

        gameOverTextName.text = GameData.levelName;
        gameOverTextTimer.text = GameData.timerString;
        gameOverTextSpeed.text = MainManager.settingFile.gameSpeedModifier.ToString();
        gameOverTextScore.text = GameData.score.ToString();

        GameData.gameRunning = false;
        GameData.gameOver = true;

        soundEffectGameOver.Play();

        AdsPlatformIntegration.AdBanner_Show();
    }


    //廣告按鈕-撥放
    public void AdButtonClick_A()
    {
        AdsPlatformIntegration.AdRewarded_Show();
        adsButton_A.gameObject.SetActive(false);
        adsButton_B.gameObject.SetActive(true);

        mainManager.soundEffectUiTrue.Play();
    }


    //廣告按鈕-繼續
    public void AdButtonClick_B()
    {
        AdsPlatformIntegration.AdBanner_Hide();
        adsButton_B.gameObject.SetActive(false);

        if (AdsPlatformIntegration.aReward)
        {
            AdsPlatformIntegration.aReward = false;
            Debug.Log("廣告獎勵-第二條命");

            GameData.gameRunning = true;
            GameData.gameOver = false;
            gameOverUI.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);

            timerText.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);

            AdsReward.gameObject.SetActive(true);
            UpdateScore(-400);

            Time.timeScale = 1f;

            mainManager.soundEffectUiTrue.Play();
        }
        else
        {
            adsButton_A.gameObject.SetActive(true);
            adsButton_B.gameObject.SetActive(false);

            mainManager.soundEffectUiFalse.Play();
        }
    }


    //下一關按鈕
    public void NextButtonClick()
    {
        AdsPlatformIntegration.AdBanner_Hide();

        mainManager.soundEffectUiTrue.Play();
        MainManager.FindNextLevelById();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //遊戲過關
    public void GameCleared()
    {
        GameData.gameRunning = false;
        GameData.gameOver = true;
        pauseButton.gameObject.SetActive(false);

        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball"); // 找到所有標籤為"Ball"的物件
        foreach (GameObject ball in balls)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; // 設置速度為零
            }
        }

        Debug.Log("開始結算...");
        StartCoroutine(DestroyBalls());
    }

    //結算-消除彈珠
    IEnumerator DestroyBalls()
    {
        while (true)
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball"); // 找到所有標籤為"Ball"的物件

            if (balls.Length > 0)
            {
                // 隨機選擇一個球進行消除
                int randomIndex = Random.Range(0, balls.Length);
                GameObject ballToDestroy = balls[randomIndex];

                // 增加分數
                UpdateScore(150);

                //音訊
                soundEffectGetItem.Play();

                //VFX
                GameObject vfx = Instantiate(vfxStardustScore, ballToDestroy.transform.position, Quaternion.identity);
                var particleSystem = vfx.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    //設置粒子發射數量
                    ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
                    bursts[0].time = 0.0f; // 從運行開始時立即發射
                    bursts[0].count = (short)MainManager.settingFile.effectsVFX * 0.5f; //粒子數量
                    particleSystem.emission.SetBursts(bursts);

                    //設置子物件的 Force Over Lifetime 值
                    Transform subToScore = vfx.transform.Find("SubToScore");
                    if (subToScore != null)
                    {
                        var subParticleSystem = subToScore.GetComponent<ParticleSystem>();
                        var forceModule = subParticleSystem.forceOverLifetime;
                        forceModule.x = (-21 - transform.position.x);
                        forceModule.y = (25 - transform.position.y);
                    }
                    else
                    {
                        Debug.LogError("未找到名為 'subToScore' 的子物件！");
                    }
                }

                // 消除物件
                Destroy(ballToDestroy);

                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                StartCoroutine(DestroyItems());
                break; // 如果場景中沒有符合條件的物件，則結束
            }
        }
    }

    //結算-消除道具
    IEnumerator DestroyItems()
    {
        while (true)
        {
            GameObject[] items = GameObject.FindGameObjectsWithTag("Item"); // 找到所有標籤為"Ball"的物件

            if (items.Length > 0)
            {
                // 隨機選擇一個球進行消除
                int randomIndex = Random.Range(0, items.Length);
                GameObject itemToDestroy = items[randomIndex];

                // 增加分數
                UpdateScore(200);

                //音訊
                soundEffectGetItem.Play();

                //VFX
                GameObject vfx = Instantiate(vfxStardustScore, itemToDestroy.transform.position, Quaternion.identity);
                var particleSystem = vfx.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    //設置粒子發射數量
                    ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
                    bursts[0].time = 0.0f; // 從運行開始時立即發射
                    bursts[0].count = (short)MainManager.settingFile.effectsVFX * 0.5f; //粒子數量
                    particleSystem.emission.SetBursts(bursts);

                    //設置子物件的 Force Over Lifetime 值
                    Transform subToScore = vfx.transform.Find("SubToScore");
                    if (subToScore != null)
                    {
                        var subParticleSystem = subToScore.GetComponent<ParticleSystem>();
                        var forceModule = subParticleSystem.forceOverLifetime;
                        forceModule.x = (-21 - transform.position.x);
                        forceModule.y = (25 - transform.position.y);
                    }
                    else
                    {
                        Debug.LogError("未找到名為 'subToScore' 的子物件！");
                    }
                }

                // 消除物件
                Destroy(itemToDestroy);

                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                GameClearedEnd();
                break; // 如果場景中沒有符合條件的物件，則結束
            }
        }
    }

    //結算
    void GameClearedEnd()
    {
        gameClearedUI.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);

        gameClearedTextName.text = GameData.levelName;
        gameClearedTextTimer.text = GameData.timerString;
        gameClearedTextSpeed.text = MainManager.settingFile.gameSpeedModifier.ToString();
        gameClearedTextScore.text = GameData.score.ToString();

        int middleScore = (int)(minTotalScore + (maxTotalScore - minTotalScore) * 0.6f);
        float scoreRatio = (float)(GameData.score - minTotalScore) / (float)(maxTotalScore - minTotalScore);
        if (minTotalScore == maxTotalScore) { scoreRatio = 1; }
        progressBarMinScore.text = minTotalScore.ToString();
        progressBarMiddleScore.text = middleScore.ToString();
        progressBarMaxScore.text = maxTotalScore.ToString();
        progressBarImage.fillAmount = scoreRatio;


        int nowMedalLevel = 0;
        if (GameData.score >= minTotalScore)
        {
            nowMedalLevel = 1;
            iconMedals[0].gameObject.SetActive(true);

            if (GameData.score >= middleScore)
            {
                nowMedalLevel = 2;
                iconMedals[0].gameObject.SetActive(false);
                iconMedals[1].gameObject.SetActive(true);

                if (GameData.score >= maxTotalScore)
                {
                    nowMedalLevel = 3;
                    iconMedals[0].gameObject.SetActive(false);
                    iconMedals[1].gameObject.SetActive(false);
                    iconMedals[2].gameObject.SetActive(true);
                }
            }
        }

        MainManager.SaveCurrentLevelById(nowMedalLevel, GameData.score, (int)GameData.saveTime, MainManager.settingFile.gameSpeedModifier);

        soundEffectGameCleared.Play();
        Time.timeScale = 0f;

        AdsPlatformIntegration.AdBanner_Show();
    }

    //道具======================================================================================================================

    //道具計時器
    IEnumerator ItemsTimer()
    {
        while (true)
        {
            if (GameData.gameRunning && !GameData.gameOver && GameData.gameStarted)
            {
                if (GameData.timerLongPaddle >= 1)
                {
                    GameData.timerLongPaddle--;
                    if (GameData.timerLongPaddle == 0)
                    {
                        ItemLongPaddleOFF();
                    }
                }

                if (GameData.timerBurstBall >= 1)
                {
                    GameData.timerBurstBall--;
                    if (GameData.timerBurstBall == 0)
                    {
                        ItemBurstBallOFF();
                    }
                }

                if (GameData.timerBlackHole >= 1)
                {
                    GameData.timerBlackHole--;
                    if (GameData.timerBlackHole == 0)
                    {
                        ItemBlackHoleOFF();
                    }
                }

                //防卡關機制
                GameData.noBreakTimer++;
                if (GameData.noBreakTimer >= 30)
                {
                    BurstPaddle();
                    GameData.noBreakTimer = 0;
                }
            }

            infoText.text = ("道具: 延長/爆炸/黑洞 剩餘 " + GameData.timerLongPaddle + " / " + GameData.timerBurstBall + " / " + GameData.timerBlackHole + " 秒\n" + "防卡關道具發放 累積 " + GameData.noBreakTimer + "/30 秒");

            yield return new WaitForSeconds(1.0f);
        }
    }

    //道具觸發器
    public void ItemPowerUP(int type)
    {
        //Item: 1=彈珠+  2=加長版  3=爆炸彈珠 4=黑洞 5=移動平台
        switch (type)
        {
            case 1:
                UpdateScore(50);
                //由PowerUPPaddle來源直接觸發
                break;
            case 2:
                UpdateScore(200);
                ItemLongPaddle();
                GameData.timerLongPaddle = 30;
                break;
            case 3:
                UpdateScore(200);
                ItemBurstBall();
                GameData.timerBurstBall = 20;
                break;
            case 4:
                UpdateScore(200);
                ItemBlackHole();
                GameData.timerBlackHole = 10;
                break;
            case 5:
                UpdateScore(200);
                BurstPaddle();
                break;
            default:
                Debug.LogWarning("未知的Item類型: " + type);
                break;
        }
    }


    //道具2 加長版
    void ItemLongPaddle()
    {
        paddle.gameObject.SetActive(false);
        longPaddle.gameObject.SetActive(true);
        GameData.boundaryX = 18f;
    }

    void ItemLongPaddleOFF()
    {
        longPaddle.gameObject.SetActive(false);
        paddle.gameObject.SetActive(true);
        GameData.boundaryX = 21f;
    }


    //道具3 爆炸彈珠
    void ItemBurstBall()
    {
        GameData.burstBall = true;
    }

    void ItemBurstBallOFF()
    {
        GameData.burstBall = false;
    }


    //道具4 黑洞
    void ItemBlackHole()
    {
        GameData.blackHole = true;
        blackHole.gameObject.SetActive(true);
    }

    void ItemBlackHoleOFF()
    {
        GameData.blackHole = false;
        blackHole.gameObject.SetActive(false);
    }


    //道具5 移動平台
    void BurstPaddle()
    {
        Vector3 spawnPosition = new Vector3(0f, -1f, 0f);
        GameObject burstPaddle = Instantiate(burstPaddlePrefab, spawnPosition, Quaternion.identity);
    }
}
