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
    public float forceStrength = 7.5f; // �@�ΤO���j��


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
            rb.velocity = Vector3.zero; // �]�m�t�׬��s
        }

        if (GameData.blackHole == true)
        {
            BlackHole();
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
            Debug.Log("�V�q�ץ�" + velocity.normalized);
            if (Math.Abs(velocity.x) <= 0.01f)
            {
                float speed = velocity.y;
                velocity = new Vector3(0.1f * collisionTimes, 1.0f, 0f).normalized * speed;
                Debug.Log("�����d�� �ץ��V�q" + velocity.normalized);
                collisionTimes *= -1;
            }
        }

        //�ˬd�O�_���񧹥������A�]���o�|�ɭP�d��A�d�z�@�I�����O
        if (Vector3.Dot(velocity.normalized, Vector3.right) > 0.998f)
        {
            velocity.x *= 0.5f;
            velocity *= 2.0f;
            Debug.Log("�V�q�ץ�" + velocity.normalized);
            if (Math.Abs(velocity.y) <= 0.01f)
            {
                float speed = velocity.x;
                velocity = new Vector3(1.0f, 0.1f * collisionTimes, 0f).normalized * speed;
                Debug.Log("�����d�� �ץ��V�q" + velocity.normalized);
                collisionTimes *= -1;
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
            }
        }

        rb.velocity = velocity;
    }

    void BurstBall()
    {
        // �b�b�|��4���d���ˬd��L����
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
            //�p���V�V�q
            Vector3 direction = (blackHole.position - transform.position).normalized;
            direction.z = 0;

            //�I�[�O
            rb.AddForce(direction * forceStrength, ForceMode.Force);
        }
        else if (blackHole == null)
        {
            blackHole = GameObject.Find("Black Hole").transform;
        }
    }
}
