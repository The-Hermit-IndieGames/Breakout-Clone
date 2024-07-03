using UnityEngine;

public class LevelPlayRewardedAd : MonoBehaviour
{
    void Start()
    {
        
    }

    public void ShowRewardedVideo()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.LogWarning("獎勵影片不可用");
        }
    }
}
