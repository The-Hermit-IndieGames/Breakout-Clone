using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PowerUPPaddle : MonoBehaviour
{
    public GameObject ballPrefabs;  // ExtraBall�w�s��

    private GameManager gameManager;

    private void Start()
    {
        //�ե�GameManager�}��
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // ��2D Sprite�P��L�I���鱵Ĳ�ɽեΦ���k
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �ˬd�I��������O�_�a��Item����
        if (collision.gameObject.CompareTag("Item"))
        {
            // �q�I���������Item�}��
            Item item = collision.gameObject.GetComponent<Item>();

            if (item != null)
            {
                // ���Item��type��
                int itemType = item.type;

                // �ھ�type�Ȱ��������PowerUP
                switch (itemType)
                {
                    case 1:
                        PowerUP_1();
                        break;
                    case 2:
                        PowerUP_2();
                        break;
                    case 3:
                        PowerUP_3();
                        break;
                    default:
                        Debug.LogWarning("������Item����: " + itemType);
                        break;
                }

                // �P��Item����
                Destroy(collision.gameObject);
            }
        }
    }


    // �D��ĪG
    private void PowerUP_1()
    {
        // �b�ۨ���m�V�W���� (0, 0.5, 0) ����m
        Vector3 spawnPosition = transform.position + new Vector3(0f, 1.5f, 0f);

        // �Ыعw�s��
        GameObject ball = Instantiate(ballPrefabs, spawnPosition, Quaternion.identity);
        GameData.totalBalls += 1;

        // ���o Rigidbody �ե�
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        
        // �]�w�t�סA�H�V�W�o�g
        rb.velocity = Vector3.up * GameData.initialSpeed;
    }

    private void PowerUP_2()
    {
        gameManager.ItemLongPaddle();
    }

    private void PowerUP_3()
    {
        gameManager.ItemBurstBall();
    }
}
