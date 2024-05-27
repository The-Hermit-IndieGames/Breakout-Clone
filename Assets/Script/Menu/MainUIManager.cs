using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class MainUIManager : MonoBehaviour
{
    private MainManager mainManager;

    //星圖頁面相關:
    [SerializeField] private GameObject selectLevelButtonPrefab;        //選關按鈕(預製件)    
    [SerializeField] private Transform selectLevelMenu;                 //選關畫面(坐標系)

    [SerializeField] private GameObject previewCanvas;                  //預覽畫面


    //預覽-UI
    public TextMeshProUGUI previewNameText;
    public TextMeshProUGUI previewBrickAmountText;
    public TextMeshProUGUI previewScoreText;
    public TextMeshProUGUI previewTimerText;
    public TextMeshProUGUI previewSpeedText;
    public GameObject previewClearTF;


    //設定頁面
    public Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicSliderValue;
    public Slider soundEffectSlider;
    [SerializeField] private TextMeshProUGUI soundEffectSliderValue;
    public Slider effectsVFXSlider;
    [SerializeField] private TextMeshProUGUI effectsVFXSliderValue;
    public Slider backgroundVFXSlider;
    [SerializeField] private TextMeshProUGUI backgroundVFXSliderValue;
    public Slider speedModifierSlider;
    [SerializeField] private TextMeshProUGUI speedModifierSliderValue;



    void Start()
    {
        //調用腳本
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        mainManager.FindGameObject();

        //訂閱 設定事件
        musicSlider.onValueChanged.AddListener(HandleMusicChange);
        soundEffectSlider.onValueChanged.AddListener(HandleSoundEffectChange);
        effectsVFXSlider.onValueChanged.AddListener(HandleEffectsVFXChange);
        backgroundVFXSlider.onValueChanged.AddListener(HandleBackgroundVFXChange);
        speedModifierSlider.onValueChanged.AddListener(HandleSpeedModifierChange);
    }


    void Update()
    {

    }

    //標題畫面==========================================================================================================================

    //標題畫面-開始
    public void TitlePlayButton()
    {

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
        
    }

    //標題畫面-設定
    public void TitleSettingButton()
    {

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

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject levelsStarMap;

    [SerializeField] private GameObject levelsStarButton;
    [SerializeField] private Transform buttonsList;


    [SerializeField] private TextMeshProUGUI starMapNameText;



    //展開星圖
    public void OpenLevelsStarMap()
    {
        canvas.gameObject.SetActive(false) ;
        levelsStarMap.SetActive(true);

        LoadJsonToMap();
    }

    //關閉星圖
    public void QuitLevelsStarMap()
    {
        canvas.gameObject.SetActive(true);
        levelsStarMap.SetActive(false);
    }


    private List<GameObject> buttonInMap = new List<GameObject>();                          //地圖中的按鈕列表

    //星圖-解析 JSON 至UI
    private void LoadJsonToMap()
    {
        var rootLevelsData = MainManager.levelConfigFiles[MainManager.nowFileId];

        if (rootLevelsData != null)
        {
            starMapNameText.text = rootLevelsData.name + "    Ver." + rootLevelsData.version;

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
                    buttonScript.levelId = levelConfig.levelID;
                    buttonScript.levelName = levelConfig.levelName;
                }
            }

            //nowButton = buttonInMap[0];
            //nowLevelsData = rootLevelsData.levelConfig[0];
        }
    }


    //星圖-預覽關卡


    //星圖-預覽關卡返回


    //預覽畫面-進入關卡



    //設定介面==========================================================================================================================


    //設定-初始化
    void SetInitialSliderValue()
    {
        //將 Slider 的值設定為 GameSetting 中的值
        musicSlider.value = mainManager.settings.gameMusicF;
        musicSliderValue.text = mainManager.settings.gameMusicF.ToString("F3");
        soundEffectSlider.value = mainManager.settings.gameSoundEffectF;
        soundEffectSliderValue.text = mainManager.settings.gameSoundEffectF.ToString("F3");
        effectsVFXSlider.value = mainManager.settings.effectsVFX;
        effectsVFXSliderValue.text = mainManager.settings.effectsVFX.ToString("F0");
        backgroundVFXSlider.value = mainManager.settings.backgroundVFX;
        backgroundVFXSliderValue.text = mainManager.settings.backgroundVFX.ToString("F0");
        speedModifierSlider.value = mainManager.settings.gameSpeedModifier;
        speedModifierSliderValue.text = mainManager.settings.gameSpeedModifier.ToString("F2");
    }


    //設定數字顯示-音樂
    void HandleMusicChange(float volume)
    {
        mainManager.settings.gameMusicF = volume;
        mainManager.settings.gameMusicF = volume;
        musicSliderValue.text = mainManager.settings.gameMusicF.ToString("F3");
    }

    //設定數字顯示-音效
    void HandleSoundEffectChange(float volume)
    {
        mainManager.settings.gameSoundEffectF = volume;
        mainManager.settings.gameSoundEffectF = volume;
        soundEffectSliderValue.text = mainManager.settings.gameSoundEffectF.ToString("F3");
    }

    //設定數字顯示-VFX特效
    void HandleEffectsVFXChange(float volume)
    {
        mainManager.settings.effectsVFX = ((int)volume);
        effectsVFXSliderValue.text = mainManager.settings.effectsVFX.ToString();
    }

    //設定數字顯示-VFX背景
    void HandleBackgroundVFXChange(float volume)
    {
        mainManager.settings.backgroundVFX = ((int)volume);
        backgroundVFXSliderValue.text = mainManager.settings.backgroundVFX.ToString();
    }

    //設定數字顯示-速度
    void HandleSpeedModifierChange(float volume)
    {
        mainManager.settings.gameSpeedModifier = volume;
        mainManager.settings.gameSpeedModifier = volume;
        speedModifierSliderValue.text = mainManager.settings.gameSpeedModifier.ToString("F2");
    }
}
