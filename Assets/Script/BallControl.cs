using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BallControl : MonoBehaviour
{   
    private Vector3 paddleToBallVector;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        paddleToBallVector = transform.position - FindObjectOfType<PaddleControl>().transform.position;
    }

    private void Update()
    {
        //[遊戲開始前檢測]
        if (!GameData.gameStarted)
        {
            LockBallToPaddle();
            LaunchOnMouseClick();
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameData.gameStarted = true;
            GameData.startTime = Time.time;
            rb.velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1f, 0f).normalized * GameData.initialSpeed;
        }
    }


    //碰撞檢測
    private void OnCollisionExit(Collision other)
    {
        var velocity = rb.velocity;

        //碰撞後加速
        velocity *= GameData.speedIncreaseFactor;

        //檢查是否接近完全垂直，因為這會導致卡住，削弱一點垂直力
        if (Vector3.Dot(velocity.normalized, Vector3.up) > 0.998f)
        {
            //Debug.Log("修正 垂直");
            velocity.y *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("向量修正");
            if (velocity.x < 0.0005f)
            {
                Debug.Log("卡死 重建向量");
                float speed = velocity.y;
                velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f).normalized * speed;
            }
        }

        //檢查是否接近完全水平，因為這會導致卡住，削弱一點水平力
        if (Vector3.Dot(velocity.normalized, Vector3.right) > 0.998f)
        {
            //Debug.Log("修正 水平");
            velocity.x *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("向量修正");
            if (velocity.y < 0.0005f)
            {
                Debug.Log("卡死 重建向量");
                float speed = velocity.x;
                velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f).normalized * speed;
            }
        }

        //最大速度
        if (velocity.magnitude > GameData.maxSpeed)
        {
            velocity = velocity.normalized * GameData.maxSpeed;
        }

        //道具:爆炸
        if(GameData.burstBall==true)
        {
            if (other.gameObject.CompareTag("Brick"))
            {
                BurstBall();
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
