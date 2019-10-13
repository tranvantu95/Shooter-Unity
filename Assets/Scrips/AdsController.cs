using UnityEngine;
using UnityEngine.Advertisements;

public class AdsController : MonoBehaviour
{
    [SerializeField]
    string iosGameId;
    [SerializeField]
    string androidGameId;
    [SerializeField]
    bool enableTestMode;

    void Start()
    {
        string gameId = null;

        #if UNITY_IOS // If build platform is set to iOS...
            gameId = iosGameId;
        #elif UNITY_ANDROID // Else if build platform is set to Android...
            gameId = androidGameId;
        #endif

        if (string.IsNullOrEmpty(gameId))
        { // Make sure the Game ID is set.
            Debug.LogError("Failed to initialize Unity Ads. Game ID is null or empty.");
        }
        else if (!Advertisement.isSupported)
        {
            Debug.LogWarning("Unable to initialize Unity Ads. Platform not supported.");
        }
        else if (Advertisement.isInitialized)
        {
            Debug.Log("Unity Ads is already initialized.");
        }
        else
        {
            Debug.Log(string.Format("Initialize Unity Ads using Game ID {0} with Test Mode {1}.",
                gameId, enableTestMode ? "enabled" : "disabled"));
            Advertisement.Initialize(gameId, enableTestMode);
        }
    }

    [ContextMenu("Show Ads")]
    public void ShowAds()
    {
        if (!Advertisement.isInitialized || !Advertisement.IsReady()) return;

        Advertisement.Show();
    }

    public bool isShowing()
    {
        return Advertisement.isShowing;
    }

    #region Videos Ads
    public string zoneId;
    public int rewardQty = 250;

    [ContextMenu("Show Videos Ads")]
    public void ShowVideosAds()
    {
        if (!Advertisement.isInitialized || !Advertisement.IsReady(zoneId)) return;

        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show(zoneId, options);
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Video completed. User rewarded " + rewardQty + " credits.");
                break;
            case ShowResult.Skipped:
                Debug.LogWarning("Video was skipped.");
                break;
            case ShowResult.Failed:
                Debug.LogError("Video failed to show.");
                break;
        }
    }
    #endregion
}
