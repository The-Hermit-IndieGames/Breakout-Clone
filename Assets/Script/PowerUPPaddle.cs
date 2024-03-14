using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PowerUPPaddle : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefabs;  // ExtraBall預製件

    private GameManager gameManager;

    private void Start()
    {
        //調用GameManager腳本
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // 當2D Sprite與其他碰撞體接觸時調用此方法
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 檢查碰撞的物件是否帶有Item標籤
        if (collision.gameObject.CompareTag("Item"))
        {
            gameManager.soundEffectGetItem.Play();

            // 從碰撞物件中獲取Item腳本
            Item item = collision.gameObject.GetComponent<Item>();

            if (item != null)
            {
                // 獲取Item的type值
                int itemType = item.type;

                // 根據type值執行相應的PowerUP
                if (itemType == 1) { addBall(); }
                gameManager.ItemPowerUP(itemType);

                //粒子效果
                item.GetItem();

                // 銷毀Item物件
                Destroy(collision.gameObject);
            }
        }
    }


    //道具效果
    void addBall()
    {
        // 在自身位置向上偏移 (0, 0.5, 0) 的位置
        Vector3 spawnPosition = transform.position + new Vector3(0f, 1.5f, 0f);

        // 創建預製件
        GameObject ball = Instantiate(ballPrefabs, spawnPosition, Quaternion.identity);
        GameData.totalBalls += 1;

        // 取得 Rigidbody 組件
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        
        // 設定速度，以向上發射
        rb.velocity = Vector3.up * GameData.initialSpeed;
    }

}
