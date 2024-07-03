using UnityEngine;
using TMPro;

public class LevelStarButton : MonoBehaviour
{
    public string levelId;
    public string levelName;
    public TextMeshProUGUI levelNameTMP;

    public bool usable;     //前置關卡完成 (WIP)
    public bool clear;
    public int medalLevel;

    [SerializeField] private Renderer[] buttonRenderer;
    [SerializeField] private GameObject[] medal;
    [SerializeField] private BasicTransformControl basicTransformControl;

    private void Start()
    {
        levelNameTMP.text = levelName;
        SetRenderer();
    }

    private void OnMouseDown()
    {
        if (usable)
        {
            MainManager.nowLevelId = levelId;
            MainManager.CalibrationInfoByLevelId();

            var mainUIManager = GameObject.Find("MainUIManager").GetComponent<MainUIManager>();
            mainUIManager.BricksDelete();
            mainUIManager.PreviewTheLevel();
        }
        else
        {

        }
    }


    private void SetRenderer()
    {
        Color buttonColor;
        if (usable == false)
        {
            //不可選-灰、停止
            buttonColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            basicTransformControl.isRotation = false;
        }
        else
        {
            if (clear)
            {
                //過關-淡藍
                buttonColor = new Color(0.5f, 0.5f, 0.9f, 0.4f);
                switch (medalLevel)
                {
                    case 1:
                        medal[0].gameObject.SetActive(true);
                        break;
                    case 2:
                        medal[1].gameObject.SetActive(true);
                        break;
                    case 3:
                        medal[2].gameObject.SetActive(true);
                        break;
                    default:
                        Debug.LogWarning("未知的 medalLevel 類型: " + medalLevel);
                        break;
                }
            }
            else
            {
                //未過關-黃
                buttonColor = new Color(0.6f, 0.8f, 0.4f, 0.4f);
            }
        }

        for (int i = 0; i < buttonRenderer.Length; i++)
        {
            buttonRenderer[i].material.color = buttonColor;
        }

    }
}
