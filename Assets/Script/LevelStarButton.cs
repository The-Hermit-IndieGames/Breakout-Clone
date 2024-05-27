using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelStarButton : MonoBehaviour
{
    public string levelId;
    public string levelName;
    public TextMeshProUGUI levelNameTMP;


    private void Start()
    {
        levelNameTMP.text = levelName;
    }

    private void OnMouseDown()
    {
        MainManager.nowLevelId = levelId;
        Debug.Log("nowLevelId = " + MainManager.nowLevelId);
    }
}
