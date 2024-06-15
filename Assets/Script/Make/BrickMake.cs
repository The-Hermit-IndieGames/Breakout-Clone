using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrickMake : MonoBehaviour
{
    public int brickType;       //�j�����O

    public int brickLevel;      //�j������
    public int powerUpType;     //�D�����O

    public int xPoint;
    public int yPoint;


    public GameObject[] powerUpPrefabs;         //���P�D�㪺�w�s��

    private Renderer brickRenderer;
    private GameObject spawnedPowerUp;
    private Transform bricksList;

    void Start()
    {
        //���o��V
        brickRenderer = GetComponent<Renderer>();

        bricksList = GameObject.Find("BrickMakeList").GetComponent<Transform>();

        //��s�C��
        UpdateBrickColor();
        PowerUpType();
    }


    //��s powerUpType
    public void UpdateItem()
    {
        //��s powerUpType
        powerUpType += 1;
        if (powerUpType >= 6)
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
    public void UpdateBrickColor()
    {
        Color brickColor = Color.white;

        if (brickType == 0)
        {
            // �]�m Metallic �M Smoothness �ݩ�
            brickRenderer.material.SetFloat("_Metallic", 0.2f);
            brickRenderer.material.SetFloat("_Glossiness", 1.0f);
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
        }
        else if (brickType == 1)
        {
            // �]�m Metallic �M Smoothness �ݩ�
            brickRenderer.material.SetFloat("_Metallic", 0.6f);
            brickRenderer.material.SetFloat("_Glossiness", 0.6f);
            brickColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);   // �Ǧ�
        }

        brickRenderer.material.color = brickColor;
    }

}
