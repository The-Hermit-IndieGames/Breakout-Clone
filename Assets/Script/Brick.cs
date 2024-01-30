using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public int pointValue;      //�}�a����(�ͦ��ɼg�J)
    public int brickLevel;      //�j������(�ͦ��ɼg�J)
    public int powerUpType;     //�D�����O(�ͦ��ɼg�J)
    public GameObject[] powerUpPrefabs; // �T�ؤ��P���w�s��A�w�]�j�p��3�A���O�N��type��1�B2�B3���w�s��

    private int brickHP;        //�j���ͩR(�ܰ�)

    private GameObject spawnedPowerUp;
    private Renderer brickRenderer;
    private GameManager gameManager;
    private Transform bricksList;

    void Start()
    {
        //���o��V
        brickRenderer = GetComponent<Renderer>();

        //�ե�GameManager�}��
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        bricksList = GameObject.Find("BrickList").GetComponent<Transform>();

        //�]�wHP ��s�C��
        brickHP = brickLevel;
        UpdateBrickColor();

        //�ͦ��D��(��w)
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
                Debug.LogWarning("������Item����: " + powerUpType);
                break;
        }
    }


    //�I���B�z
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

        //�}�H�ɤl

        if (brickHP == 0)
        {
            //�p�����
            gameManager.UpdateScore(pointValue);

            // ��Ball����I���ɡA�R���ۨ�(�y�L����)
            Destroy(gameObject, 0.05f);

            if (powerUpType != 0)
            {
                //����D��
                var itemScript = spawnedPowerUp.GetComponent<Item>();
                itemScript.inBrick = false;
            }

            //�p��j����
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


    //��m��s��
    private void UpdateBrickColor()
    {
        Color brickColor = Color.white;

        switch (brickHP)
        {
            case 0:
                brickColor = new Color(0.01f, 0.2f, 0.5f, 0.001f);   // ��z��
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
