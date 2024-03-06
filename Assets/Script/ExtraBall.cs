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
            rb.velocity = Vector3.zero; // �]�m�t�׬��s
        }
    }

    //�I���˴�
    private void OnCollisionExit(Collision other)
    {
        soundEffectCollision.Play();

        var velocity = rb.velocity;

        //�I����[�t
        velocity *= GameData.speedIncreaseFactor;

        //�ˬd�O�_���񧹥������A�]���o�|�ɭP�d��A�d�z�@�I�����O
        if (Vector3.Dot(velocity.normalized, Vector3.up) > 0.998f)
        {
            velocity.y *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("�V�q�ץ�");
            if (velocity.x == 0f)
            {
                Debug.Log("�����d�� �ץ��V�q");
                float speed = velocity.y;
                velocity = new Vector3(0.1f, 1.0f, 0f).normalized * speed;
            }
        }

        //�ˬd�O�_���񧹥������A�]���o�|�ɭP�d��A�d�z�@�I�����O
        if (Vector3.Dot(velocity.normalized, Vector3.right) > 0.998f)
        {
            velocity.x *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("�V�q�ץ�");
            if (velocity.y == 0f)
            {
                Debug.Log("�����d�� �ץ��V�q");
                float speed = velocity.x;
                velocity = new Vector3(1.0f, 0.11f, 0f).normalized * speed;
            }
        }

        //�̤j�t��
        if (velocity.magnitude > GameData.maxSpeed)
        {
            velocity = velocity.normalized * GameData.maxSpeed;
        }

        //�D��:�z��
        if (GameData.burstBall == true)
        {
            if (other.gameObject.CompareTag("Brick"))
            {
                BurstBall();

                soundEffectBurstBall.Play();

                //�]�m�ɤl
                GameObject vfx = Instantiate(vfxExplode, transform.position, Quaternion.identity);
                var particleSystem = vfx.GetComponent<ParticleSystem>();
                ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
                short burstsCount = (short)(mainManager.settings.effectsVFX * 1.0);
                bursts[0].time = 0.0f; // �q�B��}�l�ɥߧY�o�g
                bursts[0].count = burstsCount; //�ɤl�ƶq
                particleSystem.emission.SetBursts(bursts);
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
