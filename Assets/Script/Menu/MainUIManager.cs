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

    //�P�ϭ�������:
    [SerializeField] private GameObject selectLevelButtonPrefab;        //�������s(�w�s��)    
    [SerializeField] private Transform selectLevelMenu;                 //�����e��(���Шt)

    [SerializeField] private GameObject previewCanvas;                  //�w���e��


    //�w��-UI
    public TextMeshProUGUI previewNameText;
    public TextMeshProUGUI previewBrickAmountText;
    public TextMeshProUGUI previewScoreText;
    public TextMeshProUGUI previewTimerText;
    public TextMeshProUGUI previewSpeedText;
    public GameObject previewClearTF;


    //�]�w����
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
        mainManager.FindGameObject();

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

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject levelsStarMap;

    [SerializeField] private GameObject levelsStarButton;
    [SerializeField] private Transform buttonsList;


    [SerializeField] private TextMeshProUGUI starMapNameText;



    //�i�}�P��
    public void OpenLevelsStarMap()
    {
        canvas.gameObject.SetActive(false) ;
        levelsStarMap.SetActive(true);

        LoadJsonToMap();
    }

    //�����P��
    public void QuitLevelsStarMap()
    {
        canvas.gameObject.SetActive(true);
        levelsStarMap.SetActive(false);
    }


    private List<GameObject> buttonInMap = new List<GameObject>();                          //�a�Ϥ������s�C��

    //�P��-�ѪR JSON ��UI
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


    //�P��-�w�����d


    //�P��-�w�����d��^


    //�w���e��-�i�J���d



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
