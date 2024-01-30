using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectLevelButton : MonoBehaviour
{
    //文字
    public TextMeshProUGUI levelNameText;

    //腳本
    private MenuManager menuManager;
    private MainManager mainManager;

    //關卡號-生成時指定
    public int buttonLevel;



    void Start()
    {
        //調用腳本
        menuManager = GameObject.Find("Canvas").GetComponent<MenuManager>();
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        levelNameText.text = mainManager.defaultLevelsRoot.levelConfig[buttonLevel].levelName;
    }


    void Update()
    {

    }


    //選擇關卡號-預覽關卡
    public void LevelNumberButton()
    {
        //傳遞關卡號
        mainManager.nowLevel = buttonLevel;

        //呼叫MenuManager 預覽關卡
        menuManager.PreviewLevel(buttonLevel);
    }
}
