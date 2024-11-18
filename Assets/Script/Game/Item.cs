using UnityEngine;

public class Item : MonoBehaviour
{
    public int type;                    // 道具編號
    public bool inBrick = true;         // 物件在磚塊內?

    private float moveSpeed = 4f;       // 物件向下移動的速度
    private Vector3 originalScale;      // 原始的縮放值
    private CircleCollider2D circleCollider;

    [SerializeField] private GameObject vfxStardustScore;

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
            if (GameData.gameRunning == true && GameData.gameStarted == true)
            {                
                transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            }            

            // 如果Y座標小於minY，刪除物件
            if (transform.position.y < -5f)
            {
                Destroy(gameObject);
            }
        }
    }

    //取得道具時
    public void GetItem()
    {
        GameObject vfx = Instantiate(vfxStardustScore, transform.position, Quaternion.identity);
        var particleSystem = vfx.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            //設置為顏色
            var mainModule = particleSystem.main;
            var newColor = new Color(1f, 1f, 1f, 1f);
            mainModule.startColor = newColor;

            //設置粒子發射數量
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
            bursts[0].time = 0.0f; // 從運行開始時立即發射
            bursts[0].count = (short)MainManager.settingFile.effectsVFX * 0.5f; //粒子數量
            particleSystem.emission.SetBursts(bursts);

            //設置子物件的 Force Over Lifetime 值
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
                Debug.LogError("未找到名為 'subToScore' 的子物件！");
            }
        }
    }
}
