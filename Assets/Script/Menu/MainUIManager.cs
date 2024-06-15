using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{
    private MainManager mainManager;


    //�]�w����
    [SerializeField] private string inspectorSettings = "�]�w����";
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
        //�եθ}��
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        //�q�\ �]�w�ƥ�
        musicSlider.onValueChanged.AddListener(HandleMusicChange);
        soundEffectSlider.onValueChanged.AddListener(HandleSoundEffectChange);
        effectsVFXSlider.onValueChanged.AddListener(HandleEffectsVFXChange);
        backgroundVFXSlider.onValueChanged.AddListener(HandleBackgroundVFXChange);
        speedModifierSlider.onValueChanged.AddListener(HandleSpeedModifierChange);
    }


    void Update()
    {

    }

    //���D�e��==========================================================================================================================

    //���D�e��-�}�l
    public void TitlePlayButton()
    {

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

    }

    //���D�e��-�]�w
    public void TitleSettingButton()
    {

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
    [SerializeField] private TextMeshProUGUI starMapNameText;

    [SerializeField] private GameObject levelsStarButton;           //�������s(�w�s��)    
    [SerializeField] private Transform buttonsList;                 //�����e��(���Шt)



    //�i�}�P��
    public void OpenLevelsStarMap()
    {
        canvas.gameObject.SetActive(false);
        levelsStarMap.SetActive(true);

        LoadJsonToMap();
    }

    //�����P��
    public void QuitLevelsStarMap()
    {
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

    [SerializeField] private GameObject previewClearTF;

    //�P��-�w�����d
    public void PreviewTheLevel()
    {
        previewCanvas.SetActive(true);
        MainManager.FindLevelConfigById();

        PreviewGenerateBricks();

        previewNameText.text = MainManager.nowLevelData.levelName;
        previewBrickAmountText.text = brickAmount.ToString();

        previewScoreText.text = "8888";
        previewTimerAndSpeedText.text = ("[00:00]" + " (x" + "[1.00]" + ")");
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
            }
            else if (brickData.brickType == "Unbreakable")
            {
                GameObject brick = Instantiate(brickUnbreakablePrefab, position, Quaternion.identity, bricksList);
            }

            brickAmount += 1;
        }
    }

    //�P��-�w�����d��^
    public void PreviewBack()
    {
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


    //�]�w-��l��
    void SetInitialSliderValue()
    {
        //�N Slider ���ȳ]�w�� GameSetting ������
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


    //�]�w�Ʀr���-����
    void HandleMusicChange(float volume)
    {
        mainManager.settings.gameMusicF = volume;
        mainManager.settings.gameMusicF = volume;
        musicSliderValue.text = mainManager.settings.gameMusicF.ToString("F3");
    }

    //�]�w�Ʀr���-����
    void HandleSoundEffectChange(float volume)
    {
        mainManager.settings.gameSoundEffectF = volume;
        mainManager.settings.gameSoundEffectF = volume;
        soundEffectSliderValue.text = mainManager.settings.gameSoundEffectF.ToString("F3");
    }

    //�]�w�Ʀr���-VFX�S��
    void HandleEffectsVFXChange(float volume)
    {
        mainManager.settings.effectsVFX = ((int)volume);
        effectsVFXSliderValue.text = mainManager.settings.effectsVFX.ToString();
    }

    //�]�w�Ʀr���-VFX�I��
    void HandleBackgroundVFXChange(float volume)
    {
        mainManager.settings.backgroundVFX = ((int)volume);
        backgroundVFXSliderValue.text = mainManager.settings.backgroundVFX.ToString();
    }

    //�]�w�Ʀr���-�t��
    void HandleSpeedModifierChange(float volume)
    {
        mainManager.settings.gameSpeedModifier = volume;
        mainManager.settings.gameSpeedModifier = volume;
        speedModifierSliderValue.text = mainManager.settings.gameSpeedModifier.ToString("F2");
    }
}
