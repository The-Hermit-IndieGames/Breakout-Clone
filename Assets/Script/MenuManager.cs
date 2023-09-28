using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public GameObject titleScreen;         // 標題畫面
    public GameObject levelMenu;           // 選擇關卡
    public GameObject other;               // 其他?
    public GameObject settingsMenu;        // 設定選項

    void Start()
    {
        //卸載其他場景
        //SceneManager.UnloadSceneAsync("GameScene");
        //SceneManager.UnloadSceneAsync("LevelMaking");
    }

    
    void Update()
    {
        
    }


    //標題畫面-開始
    public void TitlePlayButton()
    {
        titleScreen.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(true);
    }

    //標題畫面-製作
    public void TitleMakingButton()
    {
        // 載入 LevelMaking
        SceneManager.LoadScene("LevelMaking");
    }

    //標題畫面-??
    public void TitleOtherButton()
    {
        titleScreen.gameObject.SetActive(false);
        other.gameObject.SetActive(true);
    }

    //標題畫面-設定
    public void TitleSettingButton()
    {
        titleScreen.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(true);
    }

    //通用-返回標題畫面
    public void ReturnTitleButton()
    {
        titleScreen.gameObject.SetActive(true);
        levelMenu.gameObject.SetActive(false);
        other.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(false);
    }

    //關卡-選擇關卡
    public void SelectedLevelButton()
    {
        // 載入 GameScene
        SceneManager.LoadScene("GameScene");
    }



}
