using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPreview : MonoBehaviour
{
    public int pointValue;      //破壞分數
    public int brickLevel;      //磚塊等級
    public int powerUpType;     //道具類別
    public int brickType = 0;   //磚塊類別

    private Renderer brickRenderer;

    void Start()
    {
        //取得渲染
        brickRenderer = GetComponent<Renderer>();

        UpdateBrickColor();
    }


    //色彩更新器
    private void UpdateBrickColor()
    {
        Color brickColor = Color.white;

        switch (brickLevel)
        {
            case 0:
                brickColor = new Color(0.1f, 0.1f, 0.1f, 0.01f);   // 近透明
                break;
            case 1:
                brickColor = new Color(0.5f, 0.5f, 0.9f, 0.2f);    // 半透明淡藍
                break;
            case 2:
                brickColor = new Color(0.5f, 0.9f, 0.5f, 0.3f); // 半透明淡綠
                break;
            case 3:
                brickColor = new Color(0.8f, 0.8f, 0.4f, 0.45f); // 半透明黃
                break;
            case 4:
                brickColor = new Color(0.9f, 0.5f, 0.1f, 0.6f); // 半透明橙
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
