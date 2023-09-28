using UnityEngine;

public class Item : MonoBehaviour
{
    public int type;                    // �D��s��
    public bool inBrick = true;         // ����b�j����?

    private float moveSpeed = 4f;       // ����V�U���ʪ��t��
    private Vector3 originalScale;      // ��l���Y���
    private CircleCollider2D circleCollider;

    private void Start()
    {
        originalScale = transform.localScale;

        // ��� Circle Collider 2D �ե󪺤ޥ�
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (!inBrick)
        {
            if (circleCollider != null)
            {
                // �ҥ� Circle Collider 2D
                circleCollider.enabled = true;

                circleCollider = null;
            }

            // �p�G���󤣦b�j�����A�W�[���M�e����l��3��
            transform.localScale = new Vector3(originalScale.x * 3, originalScale.y * 3, 1);

            // �V�U����
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

            // �p�GY�y�Фp��minY�A�R������
            if (transform.position.y < -5f)
            {
                Destroy(gameObject);
            }
        }
    }
}
