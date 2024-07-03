using UnityEngine;
using UnityEngine.Advertisements;


public class AdsInitializerUnity : MonoBehaviour, IUnityAdsInitializationListener
{
    //�w�]�s�i�椸ID: Interstitial_Android�BInterstitial_iOS�BRewarded_Android�BRewarded_iOS�BBanner_Android�BBanner_iOS
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
        Debug.Log("Unity �s�i��l�Ƨ���");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity �s�i��l�ƥ��ѡG {error.ToString()} - {message}");
    }
}