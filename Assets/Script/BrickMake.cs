using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrickMake : MonoBehaviour
{
    public int pointValue;      //破壞分數
    public int brickLevel;      //磚塊等級
    public int powerUpType;     //道具類別
    public int brickType = 0;   //磚塊類別


    private Renderer brickRenderer;
    public GameObject[] powerUpPrefabs;         // 三種不同的預製件，預設大小為3，分別代表type為1、2、3的預製件
    private GameObject spawnedPowerUp;
    private Transform bricksList;

    void Start()
    {
        //取得渲染
        brickRenderer = GetComponent<Renderer>();

        bricksList = GameObject.Find("BrickMakeList").GetComponent<Transform>();

        //更新顏色
        UpdateBrickColor();
        
    }


    //更新 brickLevel
    public void UpdateLevel()
    {
        UpdateBrickColor();

        //自動指定分數
        pointValue=(brickLevel * 20);
    }


    //更新 powerUpType
    public void UpdateItem()
    {
        //更新 powerUpType
        powerUpType += 1;
        if (powerUpType >= 5)
        {
            powerUpType = 0;
        }
        PowerUpType();
    }


    public void PowerUpType()
    {
        Destroy(spawnedPowerUp);
        switch (powerUpType)
        {
            case 0:
                break;
            case 1:
                spawnedPowerUp = Instantiate(powerUpPrefabs[0], transform.position, Quaternion.identity, bricksList);
                break;
            case 2:
                spawnedPowerUp = Instantiate(powerUpPrefabs[1], transform.position, Quaternion.identity, bricksList);
                break;
            case 3:
                spawnedPowerUp = Instantiate(powerUpPrefabs[2], transform.position, Quaternion.identity, bricksList);
                break;
            case 4:
                spawnedPowerUp = Instantiate(powerUpPrefabs[3], transform.position, Quaternion.identity, bricksList);
                break;
            case 5:
                spawnedPowerUp = Instantiate(powerUpPrefabs[4], transform.position, Quaternion.identity, bricksList);
                break;
            default:
                Debug.LogWarning("未知的Item類型: " + powerUpType);
                break;
        }
    }


    //色彩更新器
    private void UpdateBrickColor()
    {
        Color brickColor = Color.white;

        switch (brickLevel)
        {
            case 0:
                brickColor = new Color(0.1f, 0.1f, 0.1f, 0.001f);   // 近透明
                break;
            case 1:
                brickColor = new Color(0.5f, 0.5f, 0.9f, 0.05f);    // 半透明淡藍
                break;
            case 2:
                brickColor = new Color(0.5f, 0.9f, 0.5f, 0.1f); // 半透明淡綠
                break;
            case 3:
                brickColor = new Color(0.8f, 0.8f, 0.4f, 0.2f); // 半透明黃
                break;
            case 4:
                brickColor = new Color(0.9f, 0.5f, 0.1f, 0.4f); // 半透明橙
                break;
            case 5:
                brickColor = new Color(0.8f, 0.2f, 0.1f, 0.8f); // 半透明紅
                break;
            default:
                brickColor = new Color(0.3f, 0f, 0.3f, 1.0f);   // 紫色
                break;
        }

        brickRenderer.material.color = brickColor;
    }

}
