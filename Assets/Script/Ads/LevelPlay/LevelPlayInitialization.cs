using UnityEngine;

public class LevelPlayInitialization : MonoBehaviour
{
    [SerializeField] private string appKey = "1eeea6b8d";

    public void InitializeAds()
    {
        IronSource.Agent.init(appKey);
        IronSource.Agent.validateIntegration();
        IronSource.Agent.setAdaptersDebug(AdsPlatformIntegration.testMode);

        // 註冊事件監聽器
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
    }

    void SdkInitializationCompletedEvent()
    {
        Debug.Log("IronSource SDK 初始化完成");
    }

    public static bool IsAndroid()
    {
        return Application.platform == RuntimePlatform.Android;
    }

    public static bool IsIOS()
    {
        return Application.platform == RuntimePlatform.IPhonePlayer;
    }

    public static bool IsWindows()
    {
        return Application.platform == RuntimePlatform.WindowsPlayer;
    }
}
