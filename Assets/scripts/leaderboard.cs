using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class leaderboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        SignIn();
    }
    // Update is called once per frame
    void SignIn()
    {
        Social.localUser.Authenticate(success => { });
    }

    public static void AddScoreToLeaderboard(string leaderboardID, long score)
    {
        Social.ReportScore(score, leaderboardID, success => { });
    }

    public void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }
}
