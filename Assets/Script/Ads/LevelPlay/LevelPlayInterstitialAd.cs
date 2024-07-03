using UnityEngine;

public class LevelPlayInterstitialAd : MonoBehaviour
{
    void Start()
    {
        // 加載插頁式廣告
        IronSource.Agent.loadInterstitial();
    }

    public void ShowInterstitial()
    {
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            Debug.Log("插頁式廣告尚未準備好");
        }
    }
}