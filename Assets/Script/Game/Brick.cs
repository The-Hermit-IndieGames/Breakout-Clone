using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class Brick : MonoBehaviour
{
    public int pointValue;      //破壞分數
    public int brickLevel;      //磚塊等級(生成時寫入)
    public int powerUpType;     //道具類別(生成時寫入)

    [SerializeField] private GameObject[] powerUpPrefabs; // 三種不同的預製件，預設大小為3，分別代表type為1、2、3的預製件
    [SerializeField] private GameObject vfxStardust;

    private int brickHP;        //磚塊生命(變動)

    private GameObject spawnedPowerUp;
    private Renderer brickRenderer;
    private Transform bricksList;

    private Color brickColor = Color.white;


    [SerializeField] private AudioSource soundEffectCollision;
    [SerializeField] private AudioSource soundEffectDestroy;


    void Start()
    {
        //取得渲染
        brickRenderer = GetComponent<Renderer>();

        bricksList = GameObject.Find("BrickList").GetComponent<Transform>();

        //設定分數 HP 更新顏色
        brickHP = brickLevel;
        pointValue = brickLevel * 20;
        UpdateBrickColor();


        //音效
        soundEffectCollision.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        soundEffectDestroy.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;

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
            case 5:
                spawnedPowerUp = Instantiate(powerUpPrefabs[4], transform.position, Quaternion.identity, bricksList);
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
            GameData.noBreakTimer = 0;
        }
    }

    //粒子效果
    void VFXcontrol()
    {
        GameObject vfx = Instantiate(vfxStardust, transform.position, Quaternion.identity, bricksList);
        var particleSystem = vfx.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            //設置為顏色
            var mainModule = particleSystem.main;
            var newColor = new Color(brickColor.r, brickColor.g, brickColor.b, 1f);
            mainModule.startColor = newColor;

            //設置粒子發射數量
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
            bursts[0].time = 0.0f; // 從運行開始時立即發射
            bursts[0].count = (short)MainManager.settingFile.effectsVFX; //粒子數量
            particleSystem.emission.SetBursts(bursts);

            //設置子物件的 Force Over Lifetime 值
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
                Debug.LogError("未找到名為 'subToScore' 的子物件！");
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

            //計算分數
            var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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


    //色彩更新器
    private void UpdateBrickColor()
    {
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
