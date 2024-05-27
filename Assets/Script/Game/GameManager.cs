using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Transform brickList;

    //VFX
    [SerializeField] private GameObject vfxStardustScore;

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


    //音訊
    public AudioSource soundEffectGetItem;
    [SerializeField] private AudioSource soundEffectGameOver;
    [SerializeField] private AudioSource soundEffectGameCleared;
    [SerializeField] private AudioSource soundEffectGameClearedPlus;


    //道具
    public GameObject paddle;                   //滑板
    public GameObject longPaddle;               //大滑板
    public GameObject blackHole;                //黑洞
    [SerializeField] private GameObject burstPaddlePrefab;

    //寫入參數
    public int selectedLevel;                   //指定要生成的關卡編號 (主控台指定)

    //運行
    [SerializeField] private Transform bricksList;                                          //磚塊列表(坐標系)
    MainManager.LevelConfig targetLevelConfig;              //關卡資料
    public int brickAmount;

    void Start()
    {
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
        selectedLevel = mainManager.nowLevel;
        GameData.levelName = mainManager.defaultLevelsRoot.levelConfig[mainManager.nowLevel].levelName;

        //初始速度，速度上限，每次碰撞後的速度提升因子
        GameData.initialSpeed = mainManager.settings.gameSpeedModifier * 15f;
        GameData.maxSpeed = mainManager.settings.gameSpeedModifier * 30f;
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

        soundEffectGetItem.volume = mainManager.settings.gameSoundEffectF * 1.0f;
        soundEffectGameOver.volume = mainManager.settings.gameSoundEffectF * 1.0f;
        soundEffectGameCleared.volume = mainManager.settings.gameSoundEffectF * 1.0f;
        soundEffectGameClearedPlus.volume = mainManager.settings.gameSoundEffectF * 1.0f;

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
                brickScript.pointValue = brickData.normalBricks.brickLevel * 10;
                brickScript.brickLevel = brickData.normalBricks.brickLevel;
                brickScript.powerUpType = brickData.normalBricks.powerUpType;
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
        mainManager.soundEffectUiTrue.Play();
        GameData.gameRunning = false;
        pauseUI.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);

        pauseTextName.text = GameData.levelName;
        pauseTextTimer.text = GameData.timerString;
        pauseTextSpeed.text = mainManager.settings.gameSpeedModifier.ToString();
        pauseTextScore.text = GameData.score.ToString();

        // 遊戲暫停，將時間凍結
        Time.timeScale = 0f;
    }


    //繼續按鈕
    public void ContinueButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        GameData.gameRunning = true;
        pauseUI.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(true);

        timerText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);

        //遊戲繼續，將時間解凍
        Time.timeScale = 1f;
    }


    //重新開始按鈕
    public void RestartButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //退出按鈕
    public void BackButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
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
        gameOverTextSpeed.text = mainManager.settings.gameSpeedModifier.ToString();
        gameOverTextScore.text = GameData.score.ToString();

        GameData.gameRunning = false;
        GameData.gameOver = true;

        //時間凍結
        //Time.timeScale = 0f;

        //
        soundEffectGameOver.Play();
    }


    //重新開始按鈕
    public void NextButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        if ((mainManager.nowLevel + 1) < mainManager.defaultLevelsRoot.levelConfig.Count)
        {
            mainManager.nowLevel += 1;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //遊戲過關
    public void GameCleared()
    {
        GameData.gameRunning = false;
        GameData.gameOver = true;

        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball"); // 找到所有標籤為"Ball"的物件
        foreach (GameObject ball in balls)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero; // 設置速度為零
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
                UpdateScore(200);

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
                    bursts[0].count = (short)mainManager.settings.effectsVFX * 0.5f; //粒子數量
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
                UpdateScore(60);

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
                    bursts[0].count = (short)mainManager.settings.effectsVFX * 0.5f; //粒子數量
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
        PauseButton.gameObject.SetActive(false);

        gameClearedTextName.text = GameData.levelName;
        gameClearedTextTimer.text = GameData.timerString;
        gameClearedTextSpeed.text = mainManager.settings.gameSpeedModifier.ToString();
        gameClearedTextScore.text = GameData.score.ToString();

        soundEffectGameCleared.Play();

        mainManager.GameClearedSave();
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


            Debug.Log("道具: 延長/爆炸/黑洞 剩餘 " + GameData.timerLongPaddle + " / " + GameData.timerBurstBall + " / " + GameData.timerBlackHole + " 秒\n" + "防卡關道具發放 累積 " + GameData.noBreakTimer + "/30 秒");

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
                //由PowerUPPaddle觸發
                break;
            case 2:
                UpdateScore(100);
                ItemLongPaddle();
                GameData.timerLongPaddle = 30;
                break;
            case 3:
                UpdateScore(100);
                ItemBurstBall();
                GameData.timerBurstBall = 20;
                break;
            case 4:
                UpdateScore(200);
                ItemBlackHole();
                GameData.timerBlackHole = 10;
                break;
            case 5:
                UpdateScore(20);
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
