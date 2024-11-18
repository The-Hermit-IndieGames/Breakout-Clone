using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAdManager : MonoBehaviour
{
    [SerializeField] private string adUnitId;

    void Start()
    {
        adUnitId = "Banner" + AdsInitializerUnity.runningOS;
    }

    public void ShowBannerAd()
    {
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(adUnitId);
        Debug.Log("橫幅廣告已顯示");
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
        Debug.Log("橫幅廣告已隱藏");
    }
}
