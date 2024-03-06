using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExtraBall : MonoBehaviour
{
    private MainManager mainManager;

    [SerializeField] private GameObject vfxExplode;
    private Rigidbody rb;

    [SerializeField] private AudioSource soundEffectCollision;
    [SerializeField] private AudioSource soundEffectBurstBall;

    private void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        rb = GetComponent<Rigidbody>();

        soundEffectCollision.volume = mainManager.settings.gameSoundEffectF * 1.0f;
        soundEffectBurstBall.volume = mainManager.settings.gameSoundEffectF * 0.25f;
    }

    private void Update()
    {
        if (GameData.gameOver)
        {
            rb.velocity = Vector3.zero; // 設置速度為零
        }
    }

    //碰撞檢測
    private void OnCollisionExit(Collision other)
    {
        soundEffectCollision.Play();

        var velocity = rb.velocity;

        //碰撞後加速
        velocity *= GameData.speedIncreaseFactor;

        //檢查是否接近完全垂直，因為這會導致卡住，削弱一點垂直力
        if (Vector3.Dot(velocity.normalized, Vector3.up) > 0.998f)
        {
            velocity.y *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("向量修正");
            if (velocity.x == 0f)
            {
                Debug.Log("垂直卡死 修正向量");
                float speed = velocity.y;
                velocity = new Vector3(0.1f, 1.0f, 0f).normalized * speed;
            }
        }

        //檢查是否接近完全水平，因為這會導致卡住，削弱一點水平力
        if (Vector3.Dot(velocity.normalized, Vector3.right) > 0.998f)
        {
            velocity.x *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("向量修正");
            if (velocity.y == 0f)
            {
                Debug.Log("水平卡死 修正向量");
                float speed = velocity.x;
                velocity = new Vector3(1.0f, 0.11f, 0f).normalized * speed;
            }
        }

        //最大速度
        if (velocity.magnitude > GameData.maxSpeed)
        {
            velocity = velocity.normalized * GameData.maxSpeed;
        }

        //道具:爆炸
        if (GameData.burstBall == true)
        {
            if (other.gameObject.CompareTag("Brick"))
            {
                BurstBall();

                soundEffectBurstBall.Play();

                //設置粒子
                GameObject vfx = Instantiate(vfxExplode, transform.position, Quaternion.identity);
                var particleSystem = vfx.GetComponent<ParticleSystem>();
                ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
                short burstsCount = (short)(mainManager.settings.effectsVFX * 1.0);
                bursts[0].time = 0.0f; // 從運行開始時立即發射
                bursts[0].count = burstsCount; //粒子數量
                particleSystem.emission.SetBursts(bursts);
            }
        }

        rb.velocity = velocity;
    }

    void BurstBall()
    {
        // 在半徑為4的範圍內檢查其他物件
        Collider[] colliders = Physics.OverlapSphere(transform.position, 4f);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Brick"))
            {
                Brick brick = col.GetComponent<Brick>();
                if (brick != null)
                {
                    brick.BrickCollision();
                }
            }
        }
    }
}
