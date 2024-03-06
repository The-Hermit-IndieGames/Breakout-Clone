using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class BallControl : MonoBehaviour
{
    private MainManager mainManager;

    [SerializeField] private GameObject vfxExplode;
    private Vector3 paddleToBallVector;
    private Rigidbody rb;

    private int startingStage = 0;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private AudioSource soundEffectCollision;
    [SerializeField] private AudioSource soundEffectBurstBall;


    private void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        rb = GetComponent<Rigidbody>();
        paddleToBallVector = transform.position - FindObjectOfType<PaddleControl>().transform.position;
        lineRenderer.gameObject.SetActive(false);

        soundEffectCollision.volume = mainManager.settings.gameSoundEffectF * 1.0f;
        soundEffectBurstBall.volume = mainManager.settings.gameSoundEffectF * 0.25f;
    }

    private void Update()
    {
        //[遊戲開始前檢測]
        if (!GameData.gameStarted && startingStage <= 1)
        {
            LockBallToPaddle();
            LaunchOnMouseClick();
        }

        if (GameData.gameOver)
        {
            rb.velocity = Vector3.zero; // 設置速度為零
        }
    }


    //遊戲開始前鎖定位置
    private void LockBallToPaddle()
    {
        Vector3 paddlePos = FindObjectOfType<PaddleControl>().transform.position;
        transform.position = paddlePos + paddleToBallVector;
    }


    //點擊滑鼠時開始遊戲
    private void LaunchOnMouseClick()
    {
        if (startingStage == 0)
        {
            //選擇發射位置、鎖定
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameData.gameRunning = false;
                startingStage = 1;
                lineRenderer.gameObject.SetActive(true);
            }
        }
        else if (startingStage == 1)
        {
            //選擇發射角度、發射

            //讀取滑鼠在螢幕上的位置 轉換為世界座標
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));

            //更新預覽軌跡
            UpdatePreview(transform.position, worldPosition);

            //發射! 開始遊戲
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                lineRenderer.gameObject.SetActive(false);
                GameData.gameStarted = true;
                GameData.gameRunning = true;
                GameData.startTime = Time.time;
                Vector3 vector3 = worldPosition - transform.position;
                rb.velocity = vector3.normalized * GameData.initialSpeed;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                //取消
                GameData.gameRunning = true;
                startingStage = 0;
                lineRenderer.gameObject.SetActive(false);
            }
        }
    }


    //更新預覽軌跡
    public void UpdatePreview(Vector3 startPosition, Vector3 endPosition)
    {
        // 設置Line Renderer的起始和終止點
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
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
