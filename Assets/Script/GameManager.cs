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

    public static float initialSpeed = 15f;             // ��l�t��
    public static float maxSpeed = 30f;                 // �t�פW��
    public static float speedIncreaseFactor = 1.1f;     // �C���I���᪺�t�״��ɦ]�l

    public static float boundaryX = 21f;                // ������ɭ���

    public static int score = 0;                        //�O���O
    public static int totalBalls = 0;                   //�u�]��

    public static bool burstBall = false;               //�z���u�]
}

public class GameManager : MonoBehaviour
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


    //�w�]�Ѽ�
    public TextAsset levelData;                 // JSON�t�m���
    public GameObject brickPrefab;              // �j�����w�m��
    public GameObject PauseButton;              // �Ȱ�����
    public TextMeshProUGUI scoreText;           // �O���O
    public TextMeshProUGUI pauseCanvas;         // �Ȱ��e��
    public TextMeshProUGUI gameOverCanvas;      // �C�������e��
    public TextMeshProUGUI gameClearedCanvas;   // �C���L���e��

    public GameObject paddle;                   // �ƪO
    public GameObject longPaddle;               // �j�ƪO

    //�g�J�Ѽ�
    public string selectedLevel;               // ���w�n�ͦ������d�s��

    //�B��
    LevelConfig targetLevelConfig;              //���d���
    public int brickAmount;
    private Coroutine nowItem2;
    private Coroutine nowItem3;


    void Start()
    {
        //�����D���
        //SceneManager.UnloadSceneAsync("MenuScene");

        //��l��
        Time.timeScale = 1f;
        GameData.gameRunning = true;
        GameData.gameStarted = false;
        GameData.gameOver = false;
        GameData.score = 0;

        longPaddle.gameObject.SetActive(false);
        paddle.gameObject.SetActive(true);
        GameData.boundaryX = 21f;

        brickAmount = 0;

        //��l�ưO���O
        scoreText.text = "Score: " + GameData.score.ToString();

        //�ͦ��j��
        if (levelData != null)
        {
            //Ū��JSON�ɮ�
            var root = JsonUtility.FromJson<Root>(levelData.text);

            //�������d���
            for (int i = 0; i < root.levelConfig.Count; i++)
            {
                if (root.levelConfig[i].level == selectedLevel)
                {
                    targetLevelConfig = root.levelConfig[i];
                    break;
                }
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


    void Update()
    {

    }

    //�j���ͦ���
    void GenerateBricks(List<BricksData> bricks)
    {
        foreach (var brickData in bricks)
        {
            Vector3 position = new Vector3(26 - (4 * brickData.xPoint), 24.5f - brickData.yPoint, 0);
            GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity);

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
        pauseCanvas.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);
        pauseCanvas.text = "Score: " + GameData.score.ToString();

        // �C���Ȱ��A�N�ɶ��ᵲ
        Time.timeScale = 0f;
    }


    //�~����s
    public void ContinueButtonClick()
    {
        GameData.gameRunning = true;
        pauseCanvas.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        PauseButton.gameObject.SetActive(true);

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
        gameOverCanvas.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);
        gameOverCanvas.text = "Score: " + GameData.score.ToString();
        GameData.gameRunning = false;
        GameData.gameOver = true;
    }


    //�C���L��
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
