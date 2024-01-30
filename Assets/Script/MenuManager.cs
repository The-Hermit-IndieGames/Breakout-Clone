using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class MenuManager : MonoBehaviour
{
    private MainManager mainManager;

    [SerializeField] private GameObject titleScreen;              //標題畫面
    [SerializeField] private GameObject levelMenu;                //選擇關卡
    [SerializeField] private GameObject other;                    //其他?
    [SerializeField] private GameObject settingsMenu;             //設定選項
    [SerializeField] private GameObject previewCanvas;            //預覽畫面

    //PLAY頁面相關:
    [SerializeField] private GameObject selectLevelButtonPrefab;        //選關按鈕(預製件)    
    [SerializeField] private Transform selectLevelMenu;                 //選關畫面(坐標系)
    [SerializeField] private int selectedLevel;
    [SerializeField] private int nowPage = 1;
    [SerializeField] private bool playButtonGenerate = false;
    [SerializeField] private GameObject pageCanvas;
    [SerializeField] private GameObject pageBackground;

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


    //音源
    public AudioSource audioSourceMusic;
    public AudioSource audioSoundEffect;


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


    //主畫面(通用)==========================================================================================================================

    //通用-返回標題畫面
    public void ReturnTitleButton()
    {
        titleScreen.gameObject.SetActive(true);
        levelMenu.gameObject.SetActive(false);
        other.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(false);
    }

    //主畫面-結束遊戲
    public void QuitGame()
    {
        // 在這裡放置結束遊戲的邏輯
        if (true)
        {
            Debug.Log("離開遊戲");

            // Unity 預覽和匯出的程式中，都可以使用 Application.Quit() 來結束應用程序
            Application.Quit();
        }
    }


    //標題畫面==========================================================================================================================

    //標題畫面-開始
    public void TitlePlayButton()
    {
        titleScreen.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(true);

        if (playButtonGenerate == false)
        {
            ButtonsGenerate();
        }
    }

    //標題畫面-製作
    public void TitleMakingButton()
    {
        // 載入 LevelMaking
        SceneManager.LoadScene("LevelMaking");
    }

    //標題畫面-自訂
    public void TitleCustomizeButton()
    {
        titleScreen.gameObject.SetActive(false);
        other.gameObject.SetActive(true);
    }

    //標題畫面-設定
    public void TitleSettingButton()
    {
        titleScreen.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(true);

        SetInitialSliderValue();
    }


    //關卡畫面==========================================================================================================================

    //關卡-生成按鈕
    private void ButtonsGenerate()
    {
        //編寫一個方法，用於生成10個按鈕，按鈕預製件為 SelectLevelButton ，生成時指定物件附加腳本 SelectLevelButton 的 selectedLevel 值為1~10，
        //並將 SelectLevelButton 設為 SelectLevelMenu 的子物件，最後設定座標為X=(-500 + ( (selectedLevel % 5) * 250) ) 、Y=(150)
        int iPage;
        Vector2 startPosition = new Vector2(-500f, 150f);

        for (int i = 1; i < mainManager.defaultLevelsRoot.levelConfig.Count; i++)
        {
            // 在指定位置生成按鈕
            GameObject button = Instantiate(selectLevelButtonPrefab, Vector3.zero, Quaternion.identity, selectLevelMenu);
            button.GetComponent<SelectLevelButton>().buttonLevel = i;
            iPage = (i - 1) / 10;

            // 設置按鈕座標
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
            if ((((i - 1) % 10) / 5) >= 1)      //上下排判斷
            {
                buttonRectTransform.anchoredPosition = startPosition + new Vector2(((i - 1) % 5 * 250f + iPage * 2000), -300f);
            }
            else
            {
                buttonRectTransform.anchoredPosition = startPosition + new Vector2(((i - 1) % 5 * 250f + iPage * 2000), 0f);
            }
        }

        playButtonGenerate = true;
    }

    //關卡-翻頁+
    public void PageAdd()
    {
        float allPage = (mainManager.defaultLevelsRoot.levelConfig.Count - 1) / 10.0f;
        if (nowPage < allPage)
        {
            nowPage++;
            pageBackground.transform.localPosition = new Vector3(pageBackground.transform.localPosition.x - 200, pageBackground.transform.localPosition.y);
            pageCanvas.transform.localPosition = new Vector3(pageCanvas.transform.localPosition.x - 2000, pageCanvas.transform.localPosition.y);
        }
    }
    //關卡-翻頁-
    public void PageDown()
    {
        if (nowPage >= 2)
        {
            nowPage--;
            pageBackground.transform.localPosition = new Vector3(pageBackground.transform.localPosition.x + 200, pageBackground.transform.localPosition.y);
            pageCanvas.transform.localPosition = new Vector3(pageCanvas.transform.localPosition.x + 2000, pageCanvas.transform.localPosition.y);
        }
    }

    //關卡-預覽關卡
    public void PreviewLevel(int levelid)
    {
        selectedLevel = levelid;
        //顯示預覽畫面
        previewCanvas.gameObject.SetActive(true);

        //模擬生成關卡
        mainManager.PreviewGenerate();
    }

    //關卡-預覽關卡返回
    public void PreviewLevelBack()
    {
        selectedLevel = 0;
        //關閉預覽畫面
        previewCanvas.gameObject.SetActive(false);
        mainManager.PreviewClearChildren();
    }

    //預覽畫面-進入關卡
    public void EnterLevelButton()
    {
        mainManager.EnterGameScene();
    }


    //設定介面==========================================================================================================================

    //設定-返回標題畫面
    public void SettingReturnTitleButton()
    {
        titleScreen.gameObject.SetActive(true);
        settingsMenu.gameObject.SetActive(false);
        mainManager.SaveSettingsToJson();
    }

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


    //設定-音樂
    void HandleMusicChange(float volume)
    {
        GameSetting.gameMusicF = volume;
        mainManager.settings.gameMusicF = volume;
        musicSliderValue.text = GameSetting.gameMusicF.ToString("F3");
    }

    //設定-音效
    void HandleSoundEffectChange(float volume)
    {
        GameSetting.gameSoundEffectF = volume;
        mainManager.settings.gameSoundEffectF = volume;
        soundEffectSliderValue.text = GameSetting.gameSoundEffectF.ToString("F3");
    }

    //設定-VFX特效
    void HandleEffectsVFXChange(float volume)
    {
        GameSetting.effectsVFX = volume;
        mainManager.settings.effectsVFX = volume;
        effectsVFXSliderValue.text = GameSetting.effectsVFX.ToString("F0");
    }

    //設定-VFX背景
    void HandleBackgroundVFXChange(float volume)
    {
        GameSetting.backgroundVFX = volume;
        mainManager.settings.backgroundVFX = volume;
        backgroundVFXSliderValue.text = GameSetting.backgroundVFX.ToString("F0");
    }

    //設定-速度
    void HandleSpeedModifierChange(float volume)
    {
        GameSetting.gameSpeedModifier = volume;
        mainManager.settings.gameSpeedModifier = volume;
        speedModifierSliderValue.text = GameSetting.gameSpeedModifier.ToString("F2");
    }
}
