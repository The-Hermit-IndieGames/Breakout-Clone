using UnityEngine;

public class BurstPaddle : MonoBehaviour
{
    private float moveSpeed = 4f;

    void Start()
    {
        
    }


    void Update()
    {
        //移動
        if (GameData.gameRunning && GameData.gameStarted)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        }

        //座標，刪除
        if (transform.position.y > 25.0f)
        {
            Destroy(gameObject);
        }
    }
}
