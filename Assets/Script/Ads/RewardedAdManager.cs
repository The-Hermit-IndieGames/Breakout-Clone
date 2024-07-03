using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAdManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string adUnitId;

    void Start()
    {
        adUnitId = "Rewarded" + AdsInitializerUnity.runningOS;
        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        Advertisement.Load(adUnitId, this);
    }

    public void ShowRewardedAd()
    {
        Advertisement.Show(adUnitId, this);
    }

    // IUnityAdsLoadListener 實現
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Rewarded ad loaded: " + adUnitId);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading ad unit {adUnitId}: {error.ToString()} - {message}");
    }

    // IUnityAdsShowListener 實現
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing ad unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log($"Ad unit {adUnitId} started showing.");
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        Debug.Log($"Ad unit {adUnitId} was clicked.");
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"Ad unit {adUnitId} completed showing with state {showCompletionState}.");
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            AdsPlatformIntegration.aReward = true;
            Debug.Log("獎勵已發放");
        }
    }
}
