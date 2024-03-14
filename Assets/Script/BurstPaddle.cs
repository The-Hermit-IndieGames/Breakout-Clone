using UnityEngine;

public class BurstPaddle : MonoBehaviour
{
    private float moveSpeed = 4f;

    void Start()
    {
        
    }


    void Update()
    {
        //����
        if (GameData.gameRunning && GameData.gameStarted)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        }

        //�y�СA�R��
        if (transform.position.y > 22.0f)
        {
            Destroy(gameObject);
        }
    }
}
