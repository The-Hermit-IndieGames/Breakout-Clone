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

    // �[���s�i����k
    public void LoadAd()
    {
        Advertisement.Load(adUnitId, this);
    }

    // ��ܼs�i����k
    public void ShowAd()
    {
        Advertisement.Show(adUnitId, this);
    }

    // IUnityAdsLoadListener ��k��{
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        //�s�i���J���\
        Debug.Log("Ad loaded successfully.");
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        //���J�s�i�椸�ɵo�Ϳ��~
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    // IUnityAdsShowListener ��k��{
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        //��ܼs�i�椸 {adUnitId} �ɵo�Ϳ��~
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        //�s�i�椸 {adUnitId} �}�l�i��
        Debug.Log($"Ad Unit {adUnitId} started showing.");
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        //�s�i�椸 {adUnitId} �w�Q�I��
        Debug.Log($"Ad Unit {adUnitId} was clicked.");
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        //�s�i�椸 {adUnitId} �w�����i�ܡA���A�� {showCompletionState}
        Debug.Log($"Ad Unit {adUnitId} completed showing with state {showCompletionState}.");
        // ��s�i��ܧ�����A�i�H��ܦA���[���s���s�i
        LoadAd();
    }
}
