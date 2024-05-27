using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectLevelButton : MonoBehaviour
{
    //��r
    public TextMeshProUGUI levelNameText;

    //�}��
    private MenuManager menuManager;
    private MainManager mainManager;

    //���d��-�ͦ��ɫ��w
    public int buttonLevel;



    void Start()
    {
        //�եθ}��
        menuManager = GameObject.Find("Canvas").GetComponent<MenuManager>();
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        levelNameText.text = mainManager.defaultLevelsRoot.levelConfig[buttonLevel].levelName;
    }


    void Update()
    {

    }


    //������d��-�w�����d
    public void LevelNumberButton()
    {
        //�ǻ����d��
        mainManager.nowLevel = buttonLevel;

        //�I�sMenuManager �w�����d
        menuManager.PreviewLevel(buttonLevel);
    }
}
