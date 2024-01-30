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
        //[�C���}�l�e�˴�]
        if (!GameData.gameStarted)
        {
            LockBallToPaddle();
            LaunchOnMouseClick();
        }
    }


    //�C���}�l�e��w��m
    private void LockBallToPaddle()
    {
        Vector3 paddlePos = FindObjectOfType<PaddleControl>().transform.position;
        transform.position = paddlePos + paddleToBallVector;
    }


    //�I���ƹ��ɶ}�l�C��
    private void LaunchOnMouseClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameData.gameStarted = true;
            GameData.startTime = Time.time;
            rb.velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1f, 0f).normalized * GameData.initialSpeed;
        }
    }


    //�I���˴�
    private void OnCollisionExit(Collision other)
    {
        var velocity = rb.velocity;

        //�I����[�t
        velocity *= GameData.speedIncreaseFactor;

        //�ˬd�O�_���񧹥������A�]���o�|�ɭP�d��A�d�z�@�I�����O
        if (Vector3.Dot(velocity.normalized, Vector3.up) > 0.998f)
        {
            //Debug.Log("�ץ� ����");
            velocity.y *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("�V�q�ץ�");
            if (velocity.x < 0.0005f)
            {
                Debug.Log("�d�� ���ئV�q");
                float speed = velocity.y;
                velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f).normalized * speed;
            }
        }

        //�ˬd�O�_���񧹥������A�]���o�|�ɭP�d��A�d�z�@�I�����O
        if (Vector3.Dot(velocity.normalized, Vector3.right) > 0.998f)
        {
            //Debug.Log("�ץ� ����");
            velocity.x *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("�V�q�ץ�");
            if (velocity.y < 0.0005f)
            {
                Debug.Log("�d�� ���ئV�q");
                float speed = velocity.x;
                velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f).normalized * speed;
            }
        }

        //�̤j�t��
        if (velocity.magnitude > GameData.maxSpeed)
        {
            velocity = velocity.normalized * GameData.maxSpeed;
        }

        //�D��:�z��
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
        // �b�b�|��4���d���ˬd��L����
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
