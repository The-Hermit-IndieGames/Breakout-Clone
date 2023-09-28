using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class GameData
{
    public static bool gameRunning = true;
    public static bool gameStarted = false;
    public static bool gameOver = false;

    public static float initialSpeed = 15f;             // 初始速度
    public static float maxSpeed = 30f;                 // 速度上限
    public static float speedIncreaseFactor = 1.1f;     // 每次碰撞後的速度提升因子

    public static float boundaryX = 21f;                // 移動邊界限制

    public static int score = 0;                        //記分板
    public static int totalBalls = 0;                   //彈珠數

    public static bool burstBall = false;               //爆炸彈珠
}

public class GameManager : MonoBehaviour
{
    //序列化定義
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


    //預設參數
    public TextAsset levelData;                 // JSON配置文件
    public GameObject brickPrefab;              // 磚塊的預置體
    public GameObject PauseButton;              // 暫停按鍵
    public TextMeshProUGUI scoreText;           // 記分板
    public TextMeshProUGUI pauseCanvas;         // 暫停畫面
    public TextMeshProUGUI gameOverCanvas;      // 遊戲結束畫面
    public TextMeshProUGUI gameClearedCanvas;   // 遊戲過關畫面

    public GameObject paddle;                   // 滑板
    public GameObject longPaddle;               // 大滑板

    //寫入參數
    public string selectedLevel;               // 指定要生成的關卡編號

    //運行
    LevelConfig targetLevelConfig;              //關卡資料
    public int brickAmount;
    private Coroutine nowItem2;
    private Coroutine nowItem3;


    void Start()
    {
        //卸載主選單
        //SceneManager.UnloadSceneAsync("MenuScene");

        //初始化
        Time.timeScale = 1f;
        GameData.gameRunning = true;
        GameData.gameStarted = false;
        GameData.gameOver = false;
        GameData.score = 0;

        longPaddle.gameObject.SetActive(false);
        paddle.gameObject.SetActive(true);
        GameData.boundaryX = 21f;

        brickAmount = 0;

        //初始化記分板
        scoreText.text = "Score: " + GameData.score.ToString();

        //生成磚塊
        if (levelData != null)
        {
            //讀取JSON檔案
            var root = JsonUtility.FromJson<Root>(levelData.text);

            //提取關卡資料
            for (int i = 0; i < root.levelConfig.Count; i++)
            {
                if (root.levelConfig[i].level == selectedLevel)
                {
                    targetLevelConfig = root.levelConfig[i];
                    break;
                }
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


    void Update()
    {

    }

    //磚塊生成器
    void GenerateBricks(List<BricksData> bricks)
    {
        foreach (var brickData in bricks)
        {
            Vector3 position = new Vector3(26 - (4 * brickData.xPoint), 24.5f - brickData.yPoint, 0);
            GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity);

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
        pauseCanvas.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);
        pauseCanvas.text = "Score: " + GameData.score.ToString();

        // 遊戲暫停，將時間凍結
        Time.timeScale = 0f;
    }


    //繼續按鈕
    public void ContinueButtonClick()
    {
        GameData.gameRunning = true;
        pauseCanvas.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        PauseButton.gameObject.SetActive(true);

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
        gameOverCanvas.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);
        gameOverCanvas.text = "Score: " + GameData.score.ToString();
        GameData.gameRunning = false;
        GameData.gameOver = true;
    }


    //遊戲過關
    public void GameCleared()
    {
        gameClearedCanvas.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);
        gameClearedCanvas.text = "Score: " + GameData.score.ToString();
        GameData.gameRunning = false;
        GameData.gameOver = true;
        Time.timeScale = 0f;
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
