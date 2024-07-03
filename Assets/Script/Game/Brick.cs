using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class Brick : MonoBehaviour
{
    public int pointValue;      //�}�a����
    public int brickLevel;      //�j������(�ͦ��ɼg�J)
    public int powerUpType;     //�D�����O(�ͦ��ɼg�J)

    [SerializeField] private GameObject[] powerUpPrefabs; // �T�ؤ��P���w�s��A�w�]�j�p��3�A���O�N��type��1�B2�B3���w�s��
    [SerializeField] private GameObject vfxStardust;

    private int brickHP;        //�j���ͩR(�ܰ�)

    private GameObject spawnedPowerUp;
    private Renderer brickRenderer;
    private Transform bricksList;

    private Color brickColor = Color.white;


    [SerializeField] private AudioSource soundEffectCollision;
    [SerializeField] private AudioSource soundEffectDestroy;


    void Start()
    {
        //���o��V
        brickRenderer = GetComponent<Renderer>();

        bricksList = GameObject.Find("BrickList").GetComponent<Transform>();

        //�]�w���� HP ��s�C��
        brickHP = brickLevel;
        pointValue = brickLevel * 20;
        UpdateBrickColor();


        //����
        soundEffectCollision.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        soundEffectDestroy.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;

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
            case 5:
                spawnedPowerUp = Instantiate(powerUpPrefabs[4], transform.position, Quaternion.identity, bricksList);
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
            GameData.noBreakTimer = 0;
        }
    }

    //�ɤl�ĪG
    void VFXcontrol()
    {
        GameObject vfx = Instantiate(vfxStardust, transform.position, Quaternion.identity, bricksList);
        var particleSystem = vfx.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            //�]�m���C��
            var mainModule = particleSystem.main;
            var newColor = new Color(brickColor.r, brickColor.g, brickColor.b, 1f);
            mainModule.startColor = newColor;

            //�]�m�ɤl�o�g�ƶq
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
            bursts[0].time = 0.0f; // �q�B��}�l�ɥߧY�o�g
            bursts[0].count = (short)MainManager.settingFile.effectsVFX; //�ɤl�ƶq
            particleSystem.emission.SetBursts(bursts);

            //�]�m�l���� Force Over Lifetime ��
            Transform subToScore = vfx.transform.Find("SubToScore");
            if (subToScore != null)
            {
                var subParticleSystem = subToScore.GetComponent<ParticleSystem>();
                var forceModule = subParticleSystem.forceOverLifetime;
                forceModule.x = (-21 - transform.position.x);
                forceModule.y = (25 - transform.position.y);
            }
            else
            {
                Debug.LogError("�����W�� 'subToScore' ���l����I");
            }
        }
    }

    public void BrickCollision()
    {
        VFXcontrol();
        brickHP -= 1;
        UpdateBrickColor();

        if (brickHP > 0)
        {
            soundEffectCollision.Play();
        }
        else if (brickHP == 0)
        {
            soundEffectDestroy.Play();

            //�p�����
            var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            GameManager.brickAmount -= 1;
            if (GameManager.brickAmount <= 0)
            {
                gameManager.GameCleared();
            }
        }
        else if (brickHP < 0)
        {
            Destroy(gameObject, 0.02f);
        }
    }


    //��m��s��
    private void UpdateBrickColor()
    {
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
