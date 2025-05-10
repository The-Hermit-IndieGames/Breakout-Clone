using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsPlatformIntegration : MonoBehaviour
{
    [SerializeField] private bool testModeOnUi = true;
    public static bool testMode;

    [SerializeField] private AdsInitializerUnity adsInitializerUnity;
    [SerializeField] private BannerAdManager bannerAdManager;
    [SerializeField] private InterstitialAdManager interstitialAdManager;
    [SerializeField] private RewardedAdManager rewardedAdManager;

    [SerializeField] private LevelPlayInitialization levelPlayInitialization;
    [SerializeField] private LevelPlayBannerAd levelPlayBannerAd;
    [SerializeField] private LevelPlayInterstitialAd levelPlayInterstitialAd;
    [SerializeField] private LevelPlayRewardedAd levelPlayRewardedAd;

    public static bool aReward = false;


    void Awake()
    {
        if (!MainManager.AdON)
        {
            return;
        }

        testMode = testModeOnUi;

#if UNITY_ANDROID
        Debug.Log("[Platform Integration] Running on Android");
        adsInitializerUnity.InitializeAds();
#elif UNITY_IOS
        Debug.Log("[Platform Integration] Running on iOS");
        adsInitializerUnity.InitializeAds();
#elif UNITY_STANDALONE_WIN
        Debug.Log("[Platform Integration] Running on Windows");
        levelPlayInitialization.InitializeAds();
#elif UNITY_STANDALONE_OSX
        Debug.Log("[Platform Integration] Running on macOS");
        levelPlayInitialization.InitializeAds();
#elif UNITY_WEBGL
        Debug.Log("[Platform Integration] Running on WebGL");
        levelPlayInitialization.InitializeAds();
#else
        Debug.Log("[Platform Integration] Running on an unknown platform");
        levelPlayInitialization.InitializeAds();
#endif

    }

    //總集成: 顯示與隱藏
    public static void AdBanner_Show()
    {
        if (!MainManager.AdON)
        {
            return;
        }

        var adsPlatformIntegration = GameObject.Find("MainManager").GetComponent<AdsPlatformIntegration>();

#if UNITY_ANDROID
        adsPlatformIntegration.Unity_AdBanner_Show();
#elif UNITY_IOS
        adsPlatformIntegration.Unity_AdBanner_Show();
#elif UNITY_STANDALONE_WIN
        adsPlatformIntegration.LevelPlay_AdBanner_Show();
#elif UNITY_STANDALONE_OSX
        adsPlatformIntegration.LevelPlay_AdBanner_Show();
#elif UNITY_WEBGL
        adsPlatformIntegration.LevelPlay_AdBanner_Show();
#else
        adsPlatformIntegration.LevelPlay_AdBanner_Show();
#endif

    }

    public static void AdBanner_Hide()
    {
        if (!MainManager.AdON)
        {
            return;
        }

        var adsPlatformIntegration = GameObject.Find("MainManager").GetComponent<AdsPlatformIntegration>();

#if UNITY_ANDROID
        adsPlatformIntegration.Unity_AdBanner_Hide();
#elif UNITY_IOS
        adsPlatformIntegration.Unity_AdBanner_Hide();
#elif UNITY_STANDALONE_WIN
        adsPlatformIntegration.LevelPlay_AdBanner_Hide();
#elif UNITY_STANDALONE_OSX
        adsPlatformIntegration.LevelPlay_AdBanner_Hide();
#elif UNITY_WEBGL
        adsPlatformIntegration.LevelPlay_AdBanner_Hide();
#else
        adsPlatformIntegration.LevelPlay_AdBanner_Hide();
#endif

    }

    public static void AdInterstitial_Show()
    {
        if (!MainManager.AdON)
        {
            return;
        }

        var adsPlatformIntegration = GameObject.Find("MainManager").GetComponent<AdsPlatformIntegration>();

#if UNITY_ANDROID
        adsPlatformIntegration.Unity_AdInterstitial_Show();
#elif UNITY_IOS
        adsPlatformIntegration.Unity_AdInterstitial_Show();
#elif UNITY_STANDALONE_WIN
        adsPlatformIntegration.LevelPlay_AdInterstitial_Show();
#elif UNITY_STANDALONE_OSX
        adsPlatformIntegration.LevelPlay_AdInterstitial_Show();
#elif UNITY_WEBGL
        adsPlatformIntegration.LevelPlay_AdInterstitial_Show();
#else
        adsPlatformIntegration.LevelPlay_AdInterstitial_Show();
#endif

    }

    public static void AdRewarded_Show()
    {
        if (!MainManager.AdON)
        {
            return;
        }

        var adsPlatformIntegration = GameObject.Find("MainManager").GetComponent<AdsPlatformIntegration>();

#if UNITY_ANDROID
        adsPlatformIntegration.Unity_AdRewarded_Show();
#elif UNITY_IOS
        adsPlatformIntegration.Unity_AdRewarded_Show();
#elif UNITY_STANDALONE_WIN
        adsPlatformIntegration.LevelPlay_AdRewarded_Show();
#elif UNITY_STANDALONE_OSX
        adsPlatformIntegration.LevelPlay_AdRewarded_Show();
#elif UNITY_WEBGL
        adsPlatformIntegration.LevelPlay_AdRewarded_Show();
#else
        adsPlatformIntegration.LevelPlay_AdRewarded_Show();
#endif

    }

    // Unity集成: 顯示與隱藏
    private void Unity_AdBanner_Show()
    {
        bannerAdManager.ShowBannerAd();
    }


    private void Unity_AdBanner_Hide()
    {
        bannerAdManager.HideBannerAd();
    }

    private void Unity_AdInterstitial_Show()
    {
        interstitialAdManager.ShowAd();
    }

    private void Unity_AdRewarded_Show()
    {
        rewardedAdManager.ShowRewardedAd();
    }



    // LevelPlay集成: 顯示與隱藏
    private void LevelPlay_AdBanner_Show()
    {
        levelPlayBannerAd.ShowBanner();
    }


    private void LevelPlay_AdInterstitial_Show()
    {
        levelPlayInterstitialAd.ShowInterstitial();
    }


    private void LevelPlay_AdBanner_Hide()
    {
        levelPlayBannerAd.HideBanner();
    }

    private void LevelPlay_AdRewarded_Show()
    {
        levelPlayRewardedAd.ShowRewardedVideo();
    }
}
