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
        //[�C���}�l�e�˴�]
        if (!GameData.gameStarted && startingStage <= 1)
        {
            LockBallToPaddle();
            LaunchOnMouseClick();
        }

        if (GameData.gameOver)
        {
            rb.velocity = Vector3.zero; // �]�m�t�׬��s
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
        if (startingStage == 0)
        {
            //��ܵo�g��m�B��w
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameData.gameRunning = false;
                startingStage = 1;
                lineRenderer.gameObject.SetActive(true);
            }
        }
        else if (startingStage == 1)
        {
            //��ܵo�g���סB�o�g

            //Ū���ƹ��b�ù��W����m �ഫ���@�ɮy��
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));

            //��s�w���y��
            UpdatePreview(transform.position, worldPosition);

            //�o�g! �}�l�C��
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
                //����
                GameData.gameRunning = true;
                startingStage = 0;
                lineRenderer.gameObject.SetActive(false);
            }
        }
    }


    //��s�w���y��
    public void UpdatePreview(Vector3 startPosition, Vector3 endPosition)
    {
        // �]�mLine Renderer���_�l�M�פ��I
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
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
