using UnityEngine;

public class LevelPlayBannerAd : MonoBehaviour
{
    void Start()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    public void ShowBanner()
    {
        IronSource.Agent.displayBanner();
        Debug.Log("橫幅廣告已顯示");
    }

    public void HideBanner()
    {
        IronSource.Agent.hideBanner();
        Debug.Log("橫幅廣告已隱藏");
    }
}
