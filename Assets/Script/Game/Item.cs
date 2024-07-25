using UnityEngine;

public class Item : MonoBehaviour
{
    public int type;                    // �D��s��
    public bool inBrick = true;         // ����b�j����?

    private float moveSpeed = 4f;       // ����V�U���ʪ��t��
    private Vector3 originalScale;      // ��l���Y���
    private CircleCollider2D circleCollider;

    [SerializeField] private GameObject vfxStardustScore;

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
            if (GameData.gameRunning == true && GameData.gameStarted == true)
            {                
                transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            }            

            // �p�GY�y�Фp��minY�A�R������
            if (transform.position.y < -5f)
            {
                Destroy(gameObject);
            }
        }
    }

    //���o�D���
    public void GetItem()
    {
        GameObject vfx = Instantiate(vfxStardustScore, transform.position, Quaternion.identity);
        var particleSystem = vfx.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            //�]�m���C��
            var mainModule = particleSystem.main;
            var newColor = new Color(1f, 1f, 1f, 1f);
            mainModule.startColor = newColor;

            //�]�m�ɤl�o�g�ƶq
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
            bursts[0].time = 0.0f; // �q�B��}�l�ɥߧY�o�g
            bursts[0].count = (short)MainManager.settingFile.effectsVFX * 0.5f; //�ɤl�ƶq
            particleSystem.emission.SetBursts(bursts);

            //�]�m�l���� Force Over Lifetime ��
            Transform subToScore = vfx.transform.Find("SubToScore");
            if (subToScore != null)
            {
                var subParticleSystem = subToScore.GetComponent<ParticleSystem>();
                var forceModule = subParticleSystem.forceOverLifetime;
                forceModule.x = (-21 - transform.position.x);
                forceModule.y = (25 - transform.position.y);
            }
            else
            {
                Debug.LogError("�����W�� 'subToScore' ���l����I");
            }
        }
    }
}
