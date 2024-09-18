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

    //���T
    public AudioSource uiSoundEffectUiTrue;
    public AudioSource uiSoundEffectUiFalse;
    public AudioSource uiSoundEffectUiPage;


    void Start()
    {
        //�եθ}��
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        //�q�\ �]�w�ƥ�
        musicSlider.onValueChanged.AddListener(HandleMusicChange);
        soundEffectSlider.onValueChanged.AddListener(HandleSoundEffectChange);
        effectsVFXSlider.onValueChanged.AddListener(HandleEffectsVFXChange);
        backgroundVFXSlider.onValueChanged.AddListener(HandleBackgroundVFXChange);
        speedModifierSlider.onValueChanged.AddListener(HandleSpeedModifierChange);

        //UI�ƥέ��T
        uiSoundEffectUiTrue.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        uiSoundEffectUiFalse.volume = MainManager.settingFile.gameSoundEffectF * 2.0f;
        uiSoundEffectUiPage.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;

        StartCoroutine(StartAfterAllObjectsLoaded());
    }

    IEnumerator StartAfterAllObjectsLoaded()
    {
        // ����1��A�Ҧ�����[����������檺�N�X
        yield return new WaitForSeconds(1);

        AdsPlatformIntegration.AdBanner_Show();
        AdsPlatformIntegration.AdInterstitial_Show();
    }


    void Update()
    {

    }

    //���D�e��==========================================================================================================================

    //���D�e��-�}�l
    public void TitlePlayButton()
    {
        mainManager.soundEffectUiTrue.Play();
    }

    //���D�e��-�s�@
    public void TitleMakingButton()
    {
        mainManager.soundEffectUiTrue.Play();
        // ���J LevelMaking
        SceneManager.LoadScene("LevelMaking");
    }

    //���D�e��-����ʤ��e
    public void TitleExperimentalButton()
    {
        mainManager.soundEffectUiTrue.Play();
    }

    //���D�e��-�]�w
    public void TitleSettingButton()
    {
        mainManager.soundEffectUiTrue.Play();
        SetInitialSliderValue();
    }

    //���D�e��-�����C��
    public void QuitGame()
    {
        mainManager.soundEffectUiTrue.Play();
        // �b�o�̩�m�����C�����޿�
        if (true)
        {
            Debug.Log("���}�C��");

            // Unity �w���M�ץX���{�����A���i�H�ϥ� Application.Quit() �ӵ������ε{��
            Application.Quit();
        }
    }


    //���d�P��==========================================================================================================================

    [SerializeField] private string inspectorStarMap = "���d�P��";

    [SerializeField] private GameObject canvas;             //�D�e��
    [SerializeField] private GameObject levelsStarMap;      //�P�ϭ���
    [SerializeField] private GameObject backgroundCanvas;   //�I������
    [SerializeField] private TextMeshProUGUI starMapNameText;

    [SerializeField] private GameObject levelsStarButton;           //�������s(�w�s��)    
    [SerializeField] private Transform buttonsList;                 //�����e��(���Шt)



    //�i�}�P��
    public void OpenLevelsStarMap()
    {
        mainManager.soundEffectUiTrue.Play();
        backgroundCanvas.gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);
        levelsStarMap.SetActive(true);

        LoadJsonToMap();
    }

    //�����P��
    public void QuitLevelsStarMap()
    {
        mainManager.soundEffectUiTrue.Play();
        backgroundCanvas.gameObject.SetActive(true);
        canvas.gameObject.SetActive(true);
        levelsStarMap.SetActive(false);
        previewCanvas.SetActive(false);
    }


    private List<GameObject> buttonInMap = new List<GameObject>();                          //�a�Ϥ������s�C��

    //�P��-�ѪR JSON ��UI
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

    //�w���e��
    [SerializeField] private string inspectorPreview = "�w���e��";

    [SerializeField] private Transform bricksList;                  //�j���C��(���Шt)
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

    //�P��-�w�����d
    public void PreviewTheLevel()
    {
        mainManager.soundEffectUiTrue.Play();
        previewCanvas.SetActive(true);
        MainManager.FindLevelConfigById();

        PreviewGenerateBricks();

        previewNameText.text = MainManager.nowLevelData.levelName;
        previewBrickAmountText.text = brickAmount.ToString();

        //�N�ɶ��榡�Ƭ����G��
        int minutes = Mathf.FloorToInt(MainManager.nowClearLevel.clearData.time / 60);
        int seconds = Mathf.FloorToInt(MainManager.nowClearLevel.clearData.time % 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        string timerAndSpeedString = (timerString + "   <size=24>x " + MainManager.nowClearLevel.clearData.speed + " Speed Modifier</size>");
        int score = MainManager.nowClearLevel.clearData.score;

        previewScoreText.text = score.ToString();
        previewTimerAndSpeedText.text = timerAndSpeedString;

        //��l�D��
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

    //�w��-�j���ͦ���
    private void PreviewGenerateBricks()
    {
        List<MainManager.BricksData> bricks = new List<MainManager.BricksData>(MainManager.nowLevelData.bricksData);

        foreach (var brickData in bricks)
        {
            Vector3 position = new Vector3(16 - (4 * brickData.xPoint), 16.5f - brickData.yPoint, -6);

            if (brickData.brickType == "Normal")
            {
                GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity, bricksList);

                // �]�m�j�����ݩ�
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

    //�P��-�w�����d��^
    public void PreviewBack()
    {
        mainManager.soundEffectUiTrue.Play();
        BricksDelete();
    }

    //�j���M����
    public void BricksDelete()
    {
        // �M�� Transform �����C�Ӥl����
        for (int i = 0; i < bricksList.childCount; i++)
        {
            //����l���� Transform
            Transform childTransform = bricksList.GetChild(i);

            //����l���� GameObject
            GameObject childGameObject = childTransform.gameObject;

            Destroy(childGameObject);
        }
        brickAmount = 0;
    }

    //�w���e��-�i�J���d
    public void EnterTheLevel()
    {
        mainManager.soundEffectUiTrue.Play();
        // ���J GameScene
        SceneManager.LoadScene("GameScene");
    }


    //�]�w����==========================================================================================================================

    [SerializeField] private string inspectorSettings = "�]�w����";

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


    //�]�w-��l��
    public void SetInitialSliderValue()
    {
        //�N Slider ���ȳ]�w�� GameSetting ������
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
                //���U�����s interactable = false
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


    //�]�w�Ʀr���-����
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

    //�]�w�Ʀr���-����
    void HandleSoundEffectChange(float volume)
    {
        float roundedNumber = volume / 100f;
        MainManager.settingFile.gameSoundEffectF = roundedNumber;
        soundEffectSliderValue.text = (MainManager.settingFile.gameSoundEffectF * 100).ToString() + "%";
        MainManager.SaveSettingFile();
        mainManager.UpdateAudio();

        //UI�ƥέ��T
        uiSoundEffectUiTrue.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        uiSoundEffectUiFalse.volume = MainManager.settingFile.gameSoundEffectF * 2.0f;
        uiSoundEffectUiPage.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
    }

    public void SwitchSoundEffectChange()
    {
        MainManager.settingFile.gameSoundEffect = soundEffectToggle.isOn;
        MainManager.SaveSettingFile();
    }

    //�]�w�Ʀr���-VFX�S��
    void HandleEffectsVFXChange(float volume)
    {
        MainManager.settingFile.effectsVFX = ((int)volume);
        effectsVFXSliderValue.text = MainManager.settingFile.effectsVFX.ToString();
        MainManager.SaveSettingFile();
    }

    //�]�w�Ʀr���-VFX�I��
    void HandleBackgroundVFXChange(float volume)
    {
        MainManager.settingFile.backgroundVFX = ((int)volume);
        backgroundVFXSliderValue.text = MainManager.settingFile.backgroundVFX.ToString();
        MainManager.SaveSettingFile();
    }

    //�]�w�Ʀr���-�t��
    void HandleSpeedModifierChange(float volume)
    {
        float roundedNumber = volume / 100f;
        MainManager.settingFile.gameSpeedModifier = roundedNumber;
        speedModifierSliderValue.text = MainManager.settingFile.gameSpeedModifier.ToString();
        MainManager.SaveSettingFile();
    }

    //���BGM
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
