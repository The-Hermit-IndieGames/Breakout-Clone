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

    [SerializeField] private GameObject titleScreen;              //���D�e��
    [SerializeField] private GameObject levelMenu;                //������d
    [SerializeField] private GameObject other;                    //��L?
    [SerializeField] private GameObject settingsMenu;             //�]�w�ﶵ
    [SerializeField] private GameObject previewCanvas;            //�w���e��

    //PLAY��������:
    [SerializeField] private GameObject selectLevelButtonPrefab;        //�������s(�w�s��)    
    [SerializeField] private Transform selectLevelMenu;                 //�����e��(���Шt)
    [SerializeField] private int selectedLevel;
    [SerializeField] private int nowPage = 1;
    [SerializeField] private bool playButtonGenerate = false;
    [SerializeField] private GameObject pageCanvas;
    [SerializeField] private GameObject pageBackground;

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


    //����
    public AudioSource audioSourceMusic;
    public AudioSource audioSoundEffect;


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


    //�D�e��(�q��)==========================================================================================================================

    //�q��-��^���D�e��
    public void ReturnTitleButton()
    {
        mainManager.soundEffectUiTrue.Play();
        titleScreen.gameObject.SetActive(true);
        levelMenu.gameObject.SetActive(false);
        other.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(false);
    }

    //�D�e��-�����C��
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


    //���D�e��==========================================================================================================================

    //���D�e��-�}�l
    public void TitlePlayButton()
    {
        mainManager.soundEffectUiTrue.Play();
        titleScreen.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(true);

        if (playButtonGenerate == false)
        {
            ButtonsGenerate();
        }
    }

    //���D�e��-�s�@
    public void TitleMakingButton()
    {
        mainManager.soundEffectUiTrue.Play();
        // ���J LevelMaking
        SceneManager.LoadScene("LevelMaking");
    }

    //���D�e��-�ۭq
    public void TitleCustomizeButton()
    {
        mainManager.soundEffectUiTrue.Play();
        titleScreen.gameObject.SetActive(false);
        other.gameObject.SetActive(true);
    }

    //���D�e��-�]�w
    public void TitleSettingButton()
    {
        mainManager.soundEffectUiTrue.Play();
        titleScreen.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(true);

        SetInitialSliderValue();
    }


    //���d�e��==========================================================================================================================

    //���d-�ͦ����s
    private void ButtonsGenerate()
    {
        //�s�g�@�Ӥ�k�A�Ω�ͦ�10�ӫ��s�A���s�w�s�� SelectLevelButton �A�ͦ��ɫ��w������[�}�� SelectLevelButton �� selectedLevel �Ȭ�1~10�A
        //�ñN SelectLevelButton �]�� SelectLevelMenu ���l����A�̫�]�w�y�Ь�X=(-500 + ( (selectedLevel % 5) * 250) ) �BY=(150)
        int iPage;
        Vector2 startPosition = new Vector2(-500f, 150f);

        for (int i = 1; i < mainManager.defaultLevelsRoot.levelConfig.Count; i++)
        {
            // �b���w��m�ͦ����s
            GameObject button = Instantiate(selectLevelButtonPrefab, Vector3.zero, Quaternion.identity, selectLevelMenu);
            button.GetComponent<SelectLevelButton>().buttonLevel = i;
            iPage = (i - 1) / 10;

            // �]�m���s�y��
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
            if ((((i - 1) % 10) / 5) >= 1)      //�W�U�ƧP�_
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

    //���d-½��+
    public void PageAdd()
    {
        mainManager.soundEffectUiPage.Play();
        float allPage = (mainManager.defaultLevelsRoot.levelConfig.Count - 1) / 10.0f;
        if (nowPage < allPage)
        {
            nowPage++;
            pageBackground.transform.localPosition = new Vector3(pageBackground.transform.localPosition.x - 200, pageBackground.transform.localPosition.y);
            pageCanvas.transform.localPosition = new Vector3(pageCanvas.transform.localPosition.x - 2000, pageCanvas.transform.localPosition.y);
        }
    }
    //���d-½��-
    public void PageDown()
    {
        mainManager.soundEffectUiPage.Play();
        if (nowPage >= 2)
        {
            nowPage--;
            pageBackground.transform.localPosition = new Vector3(pageBackground.transform.localPosition.x + 200, pageBackground.transform.localPosition.y);
            pageCanvas.transform.localPosition = new Vector3(pageCanvas.transform.localPosition.x + 2000, pageCanvas.transform.localPosition.y);
        }
    }

    //���d-�w�����d
    public void PreviewLevel(int levelid)
    {
        mainManager.soundEffectUiTrue.Play();
        selectedLevel = levelid;
        //��ܹw���e��
        previewCanvas.gameObject.SetActive(true);

        //�����ͦ����d
        mainManager.PreviewGenerate();
    }

    //���d-�w�����d��^
    public void PreviewLevelBack()
    {
        mainManager.soundEffectUiTrue.Play();
        selectedLevel = 0;
        //�����w���e��
        previewCanvas.gameObject.SetActive(false);
        mainManager.PreviewClearChildren();
    }

    //�w���e��-�i�J���d
    public void EnterLevelButton()
    {
        mainManager.soundEffectUiTrue.Play();
        mainManager.EnterGameScene();
    }


    //�]�w����==========================================================================================================================

    //�]�w-��^���D�e��
    public void SettingReturnTitleButton()
    {
        mainManager.soundEffectUiTrue.Play();
        titleScreen.gameObject.SetActive(true);
        settingsMenu.gameObject.SetActive(false);
        mainManager.SaveSettingsToJson();
        mainManager.UpdateAudio();
    }

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
