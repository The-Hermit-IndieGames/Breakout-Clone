using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAdManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string adUnitId;

    void Start()
    {
        adUnitId = "Interstitial" + AdsInitializerUnity.runningOS;
        LoadAd();
    }

    // 加載廣告的方法
    public void LoadAd()
    {
        Advertisement.Load(adUnitId, this);
    }

    // 顯示廣告的方法
    public void ShowAd()
    {
        Advertisement.Show(adUnitId, this);
    }

    // IUnityAdsLoadListener 方法實現
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        //廣告載入成功
        Debug.Log("Ad loaded successfully.");
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        //載入廣告單元時發生錯誤
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    // IUnityAdsShowListener 方法實現
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        //顯示廣告單元 {adUnitId} 時發生錯誤
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        //廣告單元 {adUnitId} 開始展示
        Debug.Log($"Ad Unit {adUnitId} started showing.");
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        //廣告單元 {adUnitId} 已被點擊
        Debug.Log($"Ad Unit {adUnitId} was clicked.");
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        //廣告單元 {adUnitId} 已完成展示，狀態為 {showCompletionState}
        Debug.Log($"Ad Unit {adUnitId} completed showing with state {showCompletionState}.");
        // 當廣告顯示完成後，可以選擇再次加載新的廣告
        LoadAd();
    }
}
