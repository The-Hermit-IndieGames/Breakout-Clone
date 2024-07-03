using UnityEngine;
using UnityEngine.Advertisements;


public class AdsInitializerUnity : MonoBehaviour, IUnityAdsInitializationListener
{
    //預設廣告單元ID: Interstitial_Android、Interstitial_iOS、Rewarded_Android、Rewarded_iOS、Banner_Android、Banner_iOS
    [SerializeField] private string androidGameId;
    [SerializeField] private string iOSGameId;

    public static string gameId;
    public static string runningOS;


    void Awake()
    {

    }

    public void InitializeAds()
    {
#if UNITY_ANDROID
        gameId = androidGameId;
        runningOS = "_Android";
#elif UNITY_IOS
        gameId = iOSGameId;
        runningOS = "_iOS";
#else
        Debug.Log("Running on an unknown platform");
        gameId = androidGameId;
        runningOS = "_Android";
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, AdsPlatformIntegration.testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity 廣告初始化完成");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity 廣告初始化失敗： {error.ToString()} - {message}");
    }
}