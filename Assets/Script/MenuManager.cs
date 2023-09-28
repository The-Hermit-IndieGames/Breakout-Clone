using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public GameObject titleScreen;         // ���D�e��
    public GameObject levelMenu;           // ������d
    public GameObject other;               // ��L?
    public GameObject settingsMenu;        // �]�w�ﶵ

    void Start()
    {
        //������L����
        //SceneManager.UnloadSceneAsync("GameScene");
        //SceneManager.UnloadSceneAsync("LevelMaking");
    }

    
    void Update()
    {
        
    }


    //���D�e��-�}�l
    public void TitlePlayButton()
    {
        titleScreen.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(true);
    }

    //���D�e��-�s�@
    public void TitleMakingButton()
    {
        // ���J LevelMaking
        SceneManager.LoadScene("LevelMaking");
    }

    //���D�e��-??
    public void TitleOtherButton()
    {
        titleScreen.gameObject.SetActive(false);
        other.gameObject.SetActive(true);
    }

    //���D�e��-�]�w
    public void TitleSettingButton()
    {
        titleScreen.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(true);
    }

    //�q��-��^���D�e��
    public void ReturnTitleButton()
    {
        titleScreen.gameObject.SetActive(true);
        levelMenu.gameObject.SetActive(false);
        other.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(false);
    }

    //���d-������d
    public void SelectedLevelButton()
    {
        // ���J GameScene
        SceneManager.LoadScene("GameScene");
    }



}
