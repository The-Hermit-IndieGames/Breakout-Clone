using System;
using UnityEngine;

public class ExtraBall : MonoBehaviour
{
    private int collisionTimes = 1;
    private Rigidbody rb;

    [SerializeField] private GameObject vfxExplode;

    [SerializeField] private AudioSource soundEffectCollision;
    [SerializeField] private AudioSource soundEffectBurstBall;

    [SerializeField] private Transform blackHole;
    public float forceStrength = 7.5f; // 作用力的強度


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        soundEffectCollision.volume = MainManager.settingFile.gameSoundEffectF * 1.0f;
        soundEffectBurstBall.volume = MainManager.settingFile.gameSoundEffectF * 0.25f;
    }

    private void Update()
    {
        if (GameData.gameOver == true)
        {
            rb.linearVelocity = Vector3.zero; // 設置速度為零
        }

        if (GameData.blackHole == true)
        {
            BlackHole();
        }
    }

    //碰撞檢測
    private void OnCollisionExit(Collision other)
    {
        soundEffectCollision.Play();

        var velocity = rb.linearVelocity;

        //碰撞後加速
        velocity *= GameData.speedIncreaseFactor;

        //檢查是否接近完全垂直，因為這會導致卡住，削弱一點垂直力
        if (Vector3.Dot(velocity.normalized, Vector3.up) > 0.998f)
        {
            velocity.y *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("向量修正" + velocity.normalized);
            if (Math.Abs(velocity.x) <= 0.01f)
            {
                float speed = velocity.y;
                velocity = new Vector3(0.1f * collisionTimes, 1.0f, 0f).normalized * speed;
                Debug.Log("垂直卡死 修正向量" + velocity.normalized);
                collisionTimes *= -1;
            }
        }

        //檢查是否接近完全水平，因為這會導致卡住，削弱一點水平力
        if (Vector3.Dot(velocity.normalized, Vector3.right) > 0.998f)
        {
            velocity.x *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("向量修正" + velocity.normalized);
            if (Math.Abs(velocity.y) <= 0.01f)
            {
                float speed = velocity.x;
                velocity = new Vector3(1.0f, 0.1f * collisionTimes, 0f).normalized * speed;
                Debug.Log("水平卡死 修正向量" + velocity.normalized);
                collisionTimes *= -1;
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
            }
        }

        rb.linearVelocity = velocity;
    }

    void BurstBall()
    {
        // 在半徑為4的範圍內檢查其他物件
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f);

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

    void BlackHole()
    {
        if (blackHole != null && rb != null)
        {
            //計算方向向量
            Vector3 direction = (blackHole.position - transform.position).normalized;
            direction.z = 0;

            //施加力
            rb.AddForce(direction * forceStrength, ForceMode.Force);
        }
        else if (blackHole == null)
        {
            blackHole = GameObject.Find("Black Hole").transform;
        }
    }
}
