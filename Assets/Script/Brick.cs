using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public int pointValue;      //破壞分數(生成時寫入)
    public int brickLevel;      //磚塊等級(生成時寫入)
    public int powerUpType;     //道具類別(生成時寫入)
    public GameObject[] powerUpPrefabs; // 三種不同的預製件，預設大小為3，分別代表type為1、2、3的預製件

    private int brickHP;        //磚塊生命(變動)

    private GameObject spawnedPowerUp;
    private Renderer brickRenderer;
    private GameManager gameManager;
    private Transform bricksList;

    void Start()
    {
        //取得渲染
        brickRenderer = GetComponent<Renderer>();

        //調用GameManager腳本
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        bricksList = GameObject.Find("BrickList").GetComponent<Transform>();

        //設定HP 更新顏色
        brickHP = brickLevel;
        UpdateBrickColor();

        //生成道具(鎖定)
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
            default:
                Debug.LogWarning("未知的Item類型: " + powerUpType);
                break;
        }
    }


    //碰撞處理
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            BrickCollision();
        }
    }

    public void BrickCollision()
    {
        brickHP -= 1;
        UpdateBrickColor();

        //破碎粒子

        if (brickHP == 0)
        {
            //計算分數
            gameManager.UpdateScore(pointValue);

            // 當Ball物件碰撞時，刪除自身(稍微延遲)
            Destroy(gameObject, 0.05f);

            if (powerUpType != 0)
            {
                //釋放道具
                var itemScript = spawnedPowerUp.GetComponent<Item>();
                itemScript.inBrick = false;
            }

            //計算磚塊數
            gameManager.brickAmount -= 1;
            if (gameManager.brickAmount <= 0)
            {
                gameManager.GameCleared();
            }
        }

        if (brickHP < 0)
        {
            Destroy(gameObject, 0.02f);
        }
    }


    //色彩更新器
    private void UpdateBrickColor()
    {
        Color brickColor = Color.white;

        switch (brickHP)
        {
            case 0:
                brickColor = new Color(0.01f, 0.2f, 0.5f, 0.001f);   // 近透明
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
