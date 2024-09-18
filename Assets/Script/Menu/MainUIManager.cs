using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;

public class MainUIManager : MonoBehaviour
{
    private MainManager mainManager;

    //音訊
    public AudioSource uiSoundEffectUiTrue;
    public AudioSource uiSoundEffectUiFalse;
    public AudioSource uiSoundEffectUiPage;


    void Start()
    {
        //調用腳本
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        //訂閱 設定事件
        musicSlider.onValueChanged.AddListener(HandleMusicChange);
        soundEffectSlider.onValueChanged.AddListener(HandleSoundEffectChange);
        effectsVFXSlider.onValueChanged.AddListener(HandleEffectsVFXChange);
        backgroundVFXSlider.onValueChanged.AddListener(HandleBackgroundVFXChange);
        speedModifierSlider.onValueChanged.AddListener(HandleSpeedModifierChange);

        //UI備用音訊
        uiSoundEffectUiTrue.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        uiSoundEffectUiFalse.volume = MainManager.settingFile.gameSoundEffectF * 2.0f;
        uiSoundEffectUiPage.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;

        StartCoroutine(StartAfterAllObjectsLoaded());
    }

    IEnumerator StartAfterAllObjectsLoaded()
    {
        // 等待1秒，所有物件加載完成後執行的代碼
        yield return new WaitForSeconds(1);

        AdsPlatformIntegration.AdBanner_Show();
        AdsPlatformIntegration.AdInterstitial_Show();
    }


    void Update()
    {

    }

    //標題畫面==========================================================================================================================

    //標題畫面-開始
    public void TitlePlayButton()
    {
        mainManager.soundEffectUiTrue.Play();
    }

    //標題畫面-製作
    public void TitleMakingButton()
    {
        mainManager.soundEffectUiTrue.Play();
        // 載入 LevelMaking
        SceneManager.LoadScene("LevelMaking");
    }

    //標題畫面-實驗性內容
    public void TitleExperimentalButton()
    {
        mainManager.soundEffectUiTrue.Play();
    }

    //標題畫面-設定
    public void TitleSettingButton()
    {
        mainManager.soundEffectUiTrue.Play();
        SetInitialSliderValue();
    }

    //標題畫面-結束遊戲
    public void QuitGame()
    {
        mainManager.soundEffectUiTrue.Play();
        // 在這裡放置結束遊戲的邏輯
        if (true)
        {
            Debug.Log("離開遊戲");

            // Unity 預覽和匯出的程式中，都可以使用 Application.Quit() 來結束應用程序
            Application.Quit();
        }
    }


    //關卡星圖==========================================================================================================================

    [SerializeField] private string inspectorStarMap = "關卡星圖";

    [SerializeField] private GameObject canvas;             //主畫布
    [SerializeField] private GameObject levelsStarMap;      //星圖頁面
    [SerializeField] private GameObject backgroundCanvas;   //背景頁面
    [SerializeField] private TextMeshProUGUI starMapNameText;

    [SerializeField] private GameObject levelsStarButton;           //選關按鈕(預製件)    
    [SerializeField] private Transform buttonsList;                 //選關畫面(坐標系)



    //展開星圖
    public void OpenLevelsStarMap()
    {
        mainManager.soundEffectUiTrue.Play();
        backgroundCanvas.gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);
        levelsStarMap.SetActive(true);

        LoadJsonToMap();
    }

    //關閉星圖
    public void QuitLevelsStarMap()
    {
        mainManager.soundEffectUiTrue.Play();
        backgroundCanvas.gameObject.SetActive(true);
        canvas.gameObject.SetActive(true);
        levelsStarMap.SetActive(false);
        previewCanvas.SetActive(false);
    }


    private List<GameObject> buttonInMap = new List<GameObject>();                          //地圖中的按鈕列表

    //星圖-解析 JSON 至UI
    private void LoadJsonToMap()
    {
        var rootLevelsData = MainManager.levelConfigFiles[MainManager.nowFileId];
        MainManager.FindClearDataFileById();

        if (rootLevelsData != null)
        {
            var mapNameText = rootLevelsData.name + "    <size=36>Ver." + rootLevelsData.version + "</size>";
            starMapNameText.text = mapNameText;

            foreach (var button in buttonInMap)
            {
                Destroy(button.gameObject);
            }
            buttonInMap.Clear();


            var mapLevelConfig = rootLevelsData.levelConfig;
            foreach (var levelConfig in mapLevelConfig)
            {
                GameObject button = Instantiate(levelsStarButton, Vector3.zero, Quaternion.identity, buttonsList);
                buttonInMap.Add(button);

                Vector3 buttonPosition = new Vector3(levelConfig.menuX, levelConfig.menuY, 0f);
                button.transform.localPosition = buttonPosition * 0.1f;

                var buttonScript = button.GetComponent<LevelStarButton>();
                if (buttonScript != null)
                {
                    MainManager.nowLevelId = levelConfig.levelID;
                    MainManager.FindLevelConfigById();
                    MainManager.FindClearLevelById();

                    buttonScript.levelId = levelConfig.levelID;
                    buttonScript.levelName = levelConfig.levelName;

                    buttonScript.usable = MainManager.CheckPreconditionById();
                    buttonScript.clear = MainManager.nowClearLevel.clear;
                    buttonScript.medalLevel = MainManager.nowClearLevel.clearData.medalLevel;
                }
            }
        }
    }

    //預覽畫面
    [SerializeField] private string inspectorPreview = "預覽畫面";

    [SerializeField] private Transform bricksList;                  //磚塊列表(坐標系)
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private GameObject brickUnbreakablePrefab;
    private int brickAmount;

    [SerializeField] private GameObject previewCanvas;

    [SerializeField] private TextMeshProUGUI previewNameText;
    [SerializeField] private TextMeshProUGUI previewBrickAmountText;

    [SerializeField] private TextMeshProUGUI previewScoreText;
    [SerializeField] private TextMeshProUGUI previewTimerAndSpeedText;


    [SerializeField] private List<GameObject> previewClearMedal;
    [SerializeField] private List<GameObject> previewInitialItems;

    //星圖-預覽關卡
    public void PreviewTheLevel()
    {
        mainManager.soundEffectUiTrue.Play();
        previewCanvas.SetActive(true);
        MainManager.FindLevelConfigById();

        PreviewGenerateBricks();

        previewNameText.text = MainManager.nowLevelData.levelName;
        previewBrickAmountText.text = brickAmount.ToString();

        //將時間格式化為分：秒
        int minutes = Mathf.FloorToInt(MainManager.nowClearLevel.clearData.time / 60);
        int seconds = Mathf.FloorToInt(MainManager.nowClearLevel.clearData.time % 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        string timerAndSpeedString = (timerString + "   <size=24>x " + MainManager.nowClearLevel.clearData.speed + " Speed Modifier</size>");
        int score = MainManager.nowClearLevel.clearData.score;

        previewScoreText.text = score.ToString();
        previewTimerAndSpeedText.text = timerAndSpeedString;

        //初始道具
        if (MainManager.nowLevelData.initialItem.addBall == true)       { previewInitialItems[0].SetActive(true); }
        else { previewInitialItems[0].SetActive(false); }
        if (MainManager.nowLevelData.initialItem.longPaddle == true)    { previewInitialItems[1].SetActive(true); }
        else { previewInitialItems[1].SetActive(false); }
        if (MainManager.nowLevelData.initialItem.burstBall == true)     { previewInitialItems[2].SetActive(true); }
        else { previewInitialItems[2].SetActive(false); }
        if (MainManager.nowLevelData.initialItem.blackHole == true)     { previewInitialItems[3].SetActive(true); }
        else { previewInitialItems[3].SetActive(false); }
        if (MainManager.nowLevelData.initialItem.burstPaddle == true)   { previewInitialItems[4].SetActive(true); }
        else { previewInitialItems[4].SetActive(false); }

    }

    //預覽-磚塊生成器
    private void PreviewGenerateBricks()
    {
        List<MainManager.BricksData> bricks = new List<MainManager.BricksData>(MainManager.nowLevelData.bricksData);

        foreach (var brickData in bricks)
        {
            Vector3 position = new Vector3(16 - (4 * brickData.xPoint), 16.5f - brickData.yPoint, -6);

            if (brickData.brickType == "Normal")
            {
                GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity, bricksList);

                // 設置磚塊的屬性
                var brickScript = brick.GetComponent<Brick>();
                if (brickScript != null)
                {
                    brickScript.brickLevel = brickData.normalBricks.brickLevel;
                    brickScript.powerUpType = brickData.normalBricks.powerUpType;
                }
                brickAmount += 1;
            }
            else if (brickData.brickType == "Unbreakable")
            {
                GameObject brick = Instantiate(brickUnbreakablePrefab, position, Quaternion.identity, bricksList);
            }
        }
    }

    //星圖-預覽關卡返回
    public void PreviewBack()
    {
        mainManager.soundEffectUiTrue.Play();
        BricksDelete();
    }

    //磚塊清除器
    public void BricksDelete()
    {
        // 遍歷 Transform 中的每個子物件
        for (int i = 0; i < bricksList.childCount; i++)
        {
            //獲取子物件的 Transform
            Transform childTransform = bricksList.GetChild(i);

            //獲取子物件的 GameObject
            GameObject childGameObject = childTransform.gameObject;

            Destroy(childGameObject);
        }
        brickAmount = 0;
    }

    //預覽畫面-進入關卡
    public void EnterTheLevel()
    {
        mainManager.soundEffectUiTrue.Play();
        // 載入 GameScene
        SceneManager.LoadScene("GameScene");
    }


    //設定介面==========================================================================================================================

    [SerializeField] private string inspectorSettings = "設定頁面";

    public Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicSliderValue;
    [SerializeField] private Toggle musicToggle;
    public Slider soundEffectSlider;
    [SerializeField] private TextMeshProUGUI soundEffectSliderValue;
    [SerializeField] private Toggle soundEffectToggle;

    public Slider effectsVFXSlider;
    [SerializeField] private TextMeshProUGUI effectsVFXSliderValue;
    public Slider backgroundVFXSlider;
    [SerializeField] private TextMeshProUGUI backgroundVFXSliderValue;

    public Slider speedModifierSlider;
    [SerializeField] private TextMeshProUGUI speedModifierSliderValue;

    [SerializeField] private Button[] bgnButton;


    //設定-初始化
    public void SetInitialSliderValue()
    {
        //將 Slider 的值設定為 GameSetting 中的值
        musicSlider.value = (MainManager.settingFile.gameMusicF * 100);
        musicSliderValue.text = (MainManager.settingFile.gameMusicF * 100).ToString("F0") + "%";
        musicToggle.isOn = MainManager.settingFile.gameMusic;
        soundEffectSlider.value = (MainManager.settingFile.gameSoundEffectF * 100);
        soundEffectSliderValue.text = (MainManager.settingFile.gameSoundEffectF * 100).ToString("F0") + "%";
        soundEffectToggle.isOn = MainManager.settingFile.gameSoundEffect;

        for (int i = 0; i < bgnButton.Length; i++)
        {
            if (i == MainManager.settingFile.bgmId)
            {
                //按下的按鈕 interactable = false
                bgnButton[i].interactable = false;
            }
            else
            {
                bgnButton[i].interactable = true;
            }
        }

        effectsVFXSlider.value = MainManager.settingFile.effectsVFX;
        effectsVFXSliderValue.text = MainManager.settingFile.effectsVFX.ToString("F0");
        backgroundVFXSlider.value = MainManager.settingFile.backgroundVFX;
        backgroundVFXSliderValue.text = MainManager.settingFile.backgroundVFX.ToString("F0");

        speedModifierSlider.value = (MainManager.settingFile.gameSpeedModifier * 100);
        speedModifierSliderValue.text = MainManager.settingFile.gameSpeedModifier.ToString("F2");
    }


    //設定數字顯示-音樂
    void HandleMusicChange(float volume)
    {
        float roundedNumber = volume / 100f;
        MainManager.settingFile.gameMusicF = roundedNumber;
        musicSliderValue.text = (MainManager.settingFile.gameMusicF * 100).ToString() + "%";
        MainManager.SaveSettingFile();
        mainManager.UpdateAudio();
    }

    public void SwitchMusicChange()
    {
        MainManager.settingFile.gameMusic = musicToggle.isOn;
        MainManager.SaveSettingFile();
    }

    //設定數字顯示-音效
    void HandleSoundEffectChange(float volume)
    {
        float roundedNumber = volume / 100f;
        MainManager.settingFile.gameSoundEffectF = roundedNumber;
        soundEffectSliderValue.text = (MainManager.settingFile.gameSoundEffectF * 100).ToString() + "%";
        MainManager.SaveSettingFile();
        mainManager.UpdateAudio();

        //UI備用音訊
        uiSoundEffectUiTrue.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        uiSoundEffectUiFalse.volume = MainManager.settingFile.gameSoundEffectF * 2.0f;
        uiSoundEffectUiPage.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
    }

    public void SwitchSoundEffectChange()
    {
        MainManager.settingFile.gameSoundEffect = soundEffectToggle.isOn;
        MainManager.SaveSettingFile();
    }

    //設定數字顯示-VFX特效
    void HandleEffectsVFXChange(float volume)
    {
        MainManager.settingFile.effectsVFX = ((int)volume);
        effectsVFXSliderValue.text = MainManager.settingFile.effectsVFX.ToString();
        MainManager.SaveSettingFile();
    }

    //設定數字顯示-VFX背景
    void HandleBackgroundVFXChange(float volume)
    {
        MainManager.settingFile.backgroundVFX = ((int)volume);
        backgroundVFXSliderValue.text = MainManager.settingFile.backgroundVFX.ToString();
        MainManager.SaveSettingFile();
    }

    //設定數字顯示-速度
    void HandleSpeedModifierChange(float volume)
    {
        float roundedNumber = volume / 100f;
        MainManager.settingFile.gameSpeedModifier = roundedNumber;
        speedModifierSliderValue.text = MainManager.settingFile.gameSpeedModifier.ToString();
        MainManager.SaveSettingFile();
    }

    //選擇BGM
    public void BgmButtons(int id)
    {
        for (int i = 0; i < mainManager.bgmMusic.Length; i++)
        {
            mainManager.bgmMusic[i].gameObject.SetActive(false);
        }
        mainManager.bgmMusic[id].gameObject.SetActive(true);
        MainManager.settingFile.bgmId = id;
        MainManager.SaveSettingFile();

        for (int i = 0; i < bgnButton.Length; i++)
        {
            if (i == id)
            {
                bgnButton[i].interactable = false;
            }
            else
            {
                bgnButton[i].interactable = true;
            }
        }
    }
}
