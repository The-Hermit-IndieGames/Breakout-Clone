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
        Debug.Log("��T�s�i�w���");
    }

    public void HideBanner()
    {
        IronSource.Agent.hideBanner();
        Debug.Log("��T�s�i�w����");
    }
}
