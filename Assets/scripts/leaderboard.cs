using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class leaderboard : MonoBehaviour
{  

    const string leaderboardID = "CgkIsci-0IAPEAIQAA";
    
    public Text DebugText;

    // Start is called before the first frame update
    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        SignIn();

        //PlayGamesPlatform.Activate();
        //SignIn();
    }
    // Update is called once per frame
    void SignIn()
    {
        Social.localUser.Authenticate((bool success) => 
        { 
            if (success) DebugText.text = "Logged in";
            else DebugText.text = "Failed";
        });
    }

    public static void AddScoreToLeaderboard(string leaderboardID, long score)
    {
        Social.ReportScore(score, leaderboardID, success => { });
    }

    public void ShowLeaderboard()
    {
        Debug.Log("show leaderboard");
        Social.ShowLeaderboardUI();
    }
}
