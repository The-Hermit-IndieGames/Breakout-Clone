using UnityEngine;

public class Item : MonoBehaviour
{
    public int type;                    // 道具編號
    public bool inBrick = true;         // 物件在磚塊內?

    private float moveSpeed = 4f;       // 物件向下移動的速度
    private Vector3 originalScale;      // 原始的縮放值
    private CircleCollider2D circleCollider;

    private void Start()
    {
        originalScale = transform.localScale;

        // 獲取 Circle Collider 2D 組件的引用
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (!inBrick)
        {
            if (circleCollider != null)
            {
                // 啟用 Circle Collider 2D
                circleCollider.enabled = true;

                circleCollider = null;
            }

            // 如果物件不在磚塊內，增加長和寬為原始的3倍
            transform.localScale = new Vector3(originalScale.x * 3, originalScale.y * 3, 1);

            // 向下移動
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

            // 如果Y座標小於minY，刪除物件
            if (transform.position.y < -5f)
            {
                Destroy(gameObject);
            }
        }
    }
}
