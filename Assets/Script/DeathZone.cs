using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        // �b�����}�l�ɡA�����l��Ball�����`��
        GameData.totalBalls = GameObject.FindGameObjectsWithTag("Ball").Length;

        //�ե�GameManager�}��
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    //�I���˴�
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // ��Ball����I���ɡA�R���Ӫ���
            Destroy(collision.gameObject);

            // ��sBall�����`��
            GameData.totalBalls--;

            // �p�GBall�����`�Ƭ�0�A�����C��
            if (GameData.totalBalls == 0)
            {                
                gameManager.GameOver();
            }
        }
    }
}
