using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        // 在場景開始時，獲取初始的Ball物件總數
        GameData.totalBalls = GameObject.FindGameObjectsWithTag("Ball").Length;

        //調用GameManager腳本
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    //碰撞檢測
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // 當Ball物件碰撞時，刪除該物件
            Destroy(collision.gameObject);

            // 更新Ball物件的總數
            GameData.totalBalls--;

            // 如果Ball物件總數為0，結束遊戲
            if (GameData.totalBalls == 0)
            {                
                gameManager.GameOver();
            }
        }
    }
}
