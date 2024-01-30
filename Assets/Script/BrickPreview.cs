using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPreview : MonoBehaviour
{
    public int pointValue;      //�}�a����
    public int brickLevel;      //�j������
    public int powerUpType;     //�D�����O
    public int brickType = 0;   //�j�����O

    private Renderer brickRenderer;

    void Start()
    {
        //���o��V
        brickRenderer = GetComponent<Renderer>();

        UpdateBrickColor();
    }


    //��m��s��
    private void UpdateBrickColor()
    {
        Color brickColor = Color.white;

        switch (brickLevel)
        {
            case 0:
                brickColor = new Color(0.1f, 0.1f, 0.1f, 0.01f);   // ��z��
                break;
            case 1:
                brickColor = new Color(0.5f, 0.5f, 0.9f, 0.2f);    // �b�z���H��
                break;
            case 2:
                brickColor = new Color(0.5f, 0.9f, 0.5f, 0.3f); // �b�z���H��
                break;
            case 3:
                brickColor = new Color(0.8f, 0.8f, 0.4f, 0.45f); // �b�z����
                break;
            case 4:
                brickColor = new Color(0.9f, 0.5f, 0.1f, 0.6f); // �b�z����
                break;
            case 5:
                brickColor = new Color(0.8f, 0.2f, 0.1f, 0.8f); // �b�z����
                break;
            default:
                brickColor = new Color(0.3f, 0f, 0.3f, 1.0f);   // ����
                break;
        }

        brickRenderer.material.color = brickColor;
    }

}
