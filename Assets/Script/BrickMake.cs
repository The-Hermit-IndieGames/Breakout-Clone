using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrickMake : MonoBehaviour
{
    public int pointValue;      //�}�a����
    public int brickLevel;      //�j������
    public int powerUpType;     //�D�����O
    public int brickType = 0;   //�j�����O


    private Renderer brickRenderer;
    public GameObject[] powerUpPrefabs;         // �T�ؤ��P���w�s��A�w�]�j�p��3�A���O�N��type��1�B2�B3���w�s��
    private GameObject spawnedPowerUp;
    private Transform bricksList;

    void Start()
    {
        //���o��V
        brickRenderer = GetComponent<Renderer>();

        bricksList = GameObject.Find("BrickMakeList").GetComponent<Transform>();

        //��s�C��
        UpdateBrickColor();
        
    }


    //��s brickLevel
    public void UpdateLevel()
    {
        UpdateBrickColor();

        //�۰ʫ��w����
        pointValue=(brickLevel * 20);
    }


    //��s powerUpType
    public void UpdateItem()
    {
        //��s powerUpType
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
                Debug.LogWarning("������Item����: " + powerUpType);
                break;
        }
    }


    //��m��s��
    private void UpdateBrickColor()
    {
        Color brickColor = Color.white;

        switch (brickLevel)
        {
            case 0:
                brickColor = new Color(0.1f, 0.1f, 0.1f, 0.001f);   // ��z��
                break;
            case 1:
                brickColor = new Color(0.5f, 0.5f, 0.9f, 0.05f);    // �b�z���H��
                break;
            case 2:
                brickColor = new Color(0.5f, 0.9f, 0.5f, 0.1f); // �b�z���H��
                break;
            case 3:
                brickColor = new Color(0.8f, 0.8f, 0.4f, 0.2f); // �b�z����
                break;
            case 4:
                brickColor = new Color(0.9f, 0.5f, 0.1f, 0.4f); // �b�z����
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
