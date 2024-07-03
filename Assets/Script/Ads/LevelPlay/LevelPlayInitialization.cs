using UnityEngine;

public class LevelPlayInitialization : MonoBehaviour
{
    [SerializeField] private string appKey = "1eeea6b8d";

    public void InitializeAds()
    {
        IronSource.Agent.init(appKey);
        IronSource.Agent.validateIntegration();
        IronSource.Agent.setAdaptersDebug(AdsPlatformIntegration.testMode);

        // ���U�ƥ��ť��
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
    }

    void SdkInitializationCompletedEvent()
    {
        Debug.Log("IronSource SDK ��l�Ƨ���");
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
