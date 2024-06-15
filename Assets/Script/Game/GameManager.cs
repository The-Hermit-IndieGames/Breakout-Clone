using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    //Item: 1=�u�]+  2=�[����  3=�z���u�] 4=�¬} 5=���ʥ��x
    public static int timerLongPaddle = 0;
    public static int timerBurstBall = 0;
    public static int timerBlackHole = 0;

    public static int noBreakTimer = 0;

    public static int totalBalls = 0;                //�u�]��

    public static bool longPaddle = false;
    public static bool burstBall = false;
    public static bool blackHole = false;
}

public class GameManager : MonoBehaviour
{
    //�}��
    private MainManager mainManager;

    //�j��
    [SerializeField] private GameObject brickPrefab;                // �j�����w�m��
    [SerializeField] private GameObject brickUnbreakablePrefab;
    [SerializeField] private Transform brickList;

    //VFX
    [SerializeField] private GameObject vfxStardustScore;

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

    [SerializeField] private Image progressBarImage;
    [SerializeField] private TextMeshProUGUI progressBarMinScore;
    [SerializeField] private TextMeshProUGUI progressBarMiddleScore;
    [SerializeField] private TextMeshProUGUI progressBarMaxScore;

    [SerializeField] private List<GameObject> iconMedals;


    //���T
    public AudioSource soundEffectGetItem;
    [SerializeField] private AudioSource soundEffectGameOver;
    [SerializeField] private AudioSource soundEffectGameCleared;
    [SerializeField] private AudioSource soundEffectGameClearedPlus;


    //�D��
    public GameObject paddle;                   //�ƪO
    public GameObject longPaddle;               //�j�ƪO
    public GameObject blackHole;                //�¬}
    [SerializeField] private GameObject burstPaddlePrefab;

    //�B��
    public static int brickAmount;
    private int minTotalScore = 150;
    private int maxTotalScore = 150;

    void Start()
    {
        //��l��
        Initialization();

        //�ͦ��j��
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

    //��l��
    void Initialization()
    {
        Time.timeScale = 1f;

        //���o���d��
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        GameData.levelName = MainManager.nowLevelData.levelName;

        //��l�t�סA�t�פW���A�C���I���᪺�t�״��ɦ]�l
        GameData.initialSpeed = mainManager.settings.gameSpeedModifier * 15f;
        GameData.maxSpeed = mainManager.settings.gameSpeedModifier * 30f;
        GameData.speedIncreaseFactor = 1.1f;

        GameData.score = 0;                         //�O���O
        GameData.startTime = 0;                     //�O�ɾ�

        GameData.totalBalls = 0;                    //�u�]��

        GameData.burstBall = false;                 //�D��: �z���u�]

        longPaddle.gameObject.SetActive(false);     //�D��: �[���O
        paddle.gameObject.SetActive(true);

        GameData.boundaryX = 21f;                   //������ɭ���

        brickAmount = 0;

        GameData.timerLongPaddle = 0;
        GameData.timerBurstBall = 0;
        GameData.timerBlackHole = 0;

        GameData.noBreakTimer = 0;

        GameData.longPaddle = false;
        GameData.burstBall = false;
        GameData.blackHole = false;

        //��l�ưO���O
        scoreText.text = "Score: " + GameData.score.ToString();

        //��l�ƭp�ɾ�(��ܮɶ�=�ثe�ɶ�-�}�l�ɶ�)
        GameData.startTime = Time.time;

        GameData.gameRunning = true;
        GameData.gameStarted = false;
        GameData.gameOver = false;

        soundEffectGetItem.volume = mainManager.settings.gameSoundEffectF * 1.0f;
        soundEffectGameOver.volume = mainManager.settings.gameSoundEffectF * 1.0f;
        soundEffectGameCleared.volume = mainManager.settings.gameSoundEffectF * 1.0f;
        soundEffectGameClearedPlus.volume = mainManager.settings.gameSoundEffectF * 1.0f;

        Debug.Log("�w��l�����d");
    }


    //���J�ͦ���(�۰ʩI�s->�j���ͦ���)
    void LoadGenerate()
    {
        //�������d���
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


        //�Ұʥͦ���
        if (MainManager.nowLevelData.bricksData != null)
        {
            GenerateBricks();
        }
        else
        {
            Debug.LogWarning("���w�����d�s�����s�b");
        }

    }


    //�j���ͦ���
    void GenerateBricks()
    {
        List<MainManager.BricksData> bricks = new List<MainManager.BricksData>(MainManager.nowLevelData.bricksData);

        foreach (var brickData in bricks)
        {
            Vector3 position = new Vector3(26 - (4 * brickData.xPoint), 24.5f - brickData.yPoint, 0);

            if (brickData.brickType == "Normal")
            {
                GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity, brickList);

                // �]�m�j�����ݩ�
                var brickScript = brick.GetComponent<Brick>();
                if (brickScript != null)
                {
                    brickScript.brickLevel = brickData.normalBricks.brickLevel;
                    brickScript.powerUpType = brickData.normalBricks.powerUpType;
                }
                //�O���p��
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

        // �C���Ȱ��A�N�ɶ��ᵲ
        Time.timeScale = 0f;
    }


    //�~����s
    public void ContinueButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        GameData.gameRunning = true;
        pauseUI.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(true);

        timerText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);

        //�C���~��A�N�ɶ��ѭ�
        Time.timeScale = 1f;
    }


    //���s�}�l���s
    public void RestartButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //�h�X���s
    public void BackButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        Time.timeScale = 1f;

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
        gameOverTextSpeed.text = mainManager.settings.gameSpeedModifier.ToString();
        gameOverTextScore.text = GameData.score.ToString();

        GameData.gameRunning = false;
        GameData.gameOver = true;

        soundEffectGameOver.Play();
    }


    //�U�@�����s
    public void NextButtonClick()
    {
        mainManager.soundEffectUiTrue.Play();
        //if ((mainManager.nowLevel + 1) < mainManager.defaultLevelsRoot.levelConfig.Count)
        //{
        //    mainManager.nowLevel += 1;
        //}
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //�C���L��
    public void GameCleared()
    {
        GameData.gameRunning = false;
        GameData.gameOver = true;
        PauseButton.gameObject.SetActive(false);

        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball"); // ���Ҧ����Ҭ�"Ball"������
        foreach (GameObject ball in balls)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero; // �]�m�t�׬��s
            }
        }

        Debug.Log("�}�l����...");
        StartCoroutine(DestroyBalls());
    }

    //����-�����u�]
    IEnumerator DestroyBalls()
    {
        while (true)
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball"); // ���Ҧ����Ҭ�"Ball"������

            if (balls.Length > 0)
            {
                // �H����ܤ@�Ӳy�i�����
                int randomIndex = Random.Range(0, balls.Length);
                GameObject ballToDestroy = balls[randomIndex];

                // �W�[����
                UpdateScore(150);

                //���T
                soundEffectGetItem.Play();

                //VFX
                GameObject vfx = Instantiate(vfxStardustScore, ballToDestroy.transform.position, Quaternion.identity);
                var particleSystem = vfx.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    //�]�m�ɤl�o�g�ƶq
                    ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
                    bursts[0].time = 0.0f; // �q�B��}�l�ɥߧY�o�g
                    bursts[0].count = (short)mainManager.settings.effectsVFX * 0.5f; //�ɤl�ƶq
                    particleSystem.emission.SetBursts(bursts);

                    //�]�m�l���� Force Over Lifetime ��
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
                        Debug.LogError("�����W�� 'subToScore' ���l����I");
                    }
                }

                // ��������
                Destroy(ballToDestroy);

                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                StartCoroutine(DestroyItems());
                break; // �p�G�������S���ŦX���󪺪���A�h����
            }
        }
    }

    //����-�����D��
    IEnumerator DestroyItems()
    {
        while (true)
        {
            GameObject[] items = GameObject.FindGameObjectsWithTag("Item"); // ���Ҧ����Ҭ�"Ball"������

            if (items.Length > 0)
            {
                // �H����ܤ@�Ӳy�i�����
                int randomIndex = Random.Range(0, items.Length);
                GameObject itemToDestroy = items[randomIndex];

                // �W�[����
                UpdateScore(200);

                //���T
                soundEffectGetItem.Play();

                //VFX
                GameObject vfx = Instantiate(vfxStardustScore, itemToDestroy.transform.position, Quaternion.identity);
                var particleSystem = vfx.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    //�]�m�ɤl�o�g�ƶq
                    ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
                    bursts[0].time = 0.0f; // �q�B��}�l�ɥߧY�o�g
                    bursts[0].count = (short)mainManager.settings.effectsVFX * 0.5f; //�ɤl�ƶq
                    particleSystem.emission.SetBursts(bursts);

                    //�]�m�l���� Force Over Lifetime ��
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
                        Debug.LogError("�����W�� 'subToScore' ���l����I");
                    }
                }

                // ��������
                Destroy(itemToDestroy);

                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                GameClearedEnd();
                break; // �p�G�������S���ŦX���󪺪���A�h����
            }
        }
    }

    //����
    void GameClearedEnd()
    {
        gameClearedUI.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);

        gameClearedTextName.text = GameData.levelName;
        gameClearedTextTimer.text = GameData.timerString;
        gameClearedTextSpeed.text = mainManager.settings.gameSpeedModifier.ToString();
        gameClearedTextScore.text = GameData.score.ToString();

        int middleScore = (int)(minTotalScore + (maxTotalScore - minTotalScore) * 0.6f);
        float scoreRatio = (float)(GameData.score - minTotalScore) / (float)(maxTotalScore - minTotalScore);
        progressBarMinScore.text = minTotalScore.ToString();
        progressBarMiddleScore.text = middleScore.ToString();
        progressBarMaxScore.text = maxTotalScore.ToString();
        progressBarImage.fillAmount = scoreRatio;

        Debug.Log("scoreRatio = " + scoreRatio);

        if (GameData.score >= minTotalScore)
        {
            iconMedals[0].gameObject.SetActive(true);

            if (GameData.score >= middleScore)
            {
                iconMedals[0].gameObject.SetActive(false);
                iconMedals[1].gameObject.SetActive(true);

                if (GameData.score >= maxTotalScore)
                {
                    iconMedals[0].gameObject.SetActive(false);
                    iconMedals[1].gameObject.SetActive(false);
                    iconMedals[2].gameObject.SetActive(true);
                }
            }
        }

        soundEffectGameCleared.Play();

    }

    //�D��======================================================================================================================

    //�D��p�ɾ�
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

                //���d������
                GameData.noBreakTimer++;
                if (GameData.noBreakTimer >= 30)
                {
                    BurstPaddle();
                    GameData.noBreakTimer = 0;
                }
            }


            Debug.Log("�D��: ����/�z��/�¬} �Ѿl " + GameData.timerLongPaddle + " / " + GameData.timerBurstBall + " / " + GameData.timerBlackHole + " ��\n" + "���d���D��o�� �ֿn " + GameData.noBreakTimer + "/30 ��");

            yield return new WaitForSeconds(1.0f);
        }
    }

    //�D��Ĳ�o��
    public void ItemPowerUP(int type)
    {
        //Item: 1=�u�]+  2=�[����  3=�z���u�] 4=�¬} 5=���ʥ��x
        switch (type)
        {
            case 1:
                UpdateScore(50);
                //��PowerUPPaddle�ӷ�����Ĳ�o
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
                Debug.LogWarning("������Item����: " + type);
                break;
        }
    }


    //�D��2 �[����
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


    //�D��3 �z���u�]
    void ItemBurstBall()
    {
        GameData.burstBall = true;
    }

    void ItemBurstBallOFF()
    {
        GameData.burstBall = false;
    }


    //�D��4 �¬}
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


    //�D��5 ���ʥ��x
    void BurstPaddle()
    {
        Vector3 spawnPosition = new Vector3(0f, -1f, 0f);
        GameObject burstPaddle = Instantiate(burstPaddlePrefab, spawnPosition, Quaternion.identity);
    }
}
