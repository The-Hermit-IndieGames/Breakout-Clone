using UnityEngine;

public class LevelPlayInterstitialAd : MonoBehaviour
{
    void Start()
    {
        // �[���������s�i
        IronSource.Agent.loadInterstitial();
    }

    public void ShowInterstitial()
    {
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            Debug.Log("�������s�i�|���ǳƦn");
        }
    }
}