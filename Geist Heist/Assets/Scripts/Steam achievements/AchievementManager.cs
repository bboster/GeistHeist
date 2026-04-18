using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class AchievementManager : Singleton<AchievementManager>
{
    public static AchievementManager instance;
    private uint appID = 4476090;//giest heists appID
    private bool connectedToSteam = false;
    private int totalNumberOfAchievements = 30;

    public enum eAchievements {OpenGame, Credits, BeatTutorial, BeatLevel1, BeatLevel2, BeatLevel3, BeatLevel4, BeatLevel5, InteractWithLore, RollOut, FinishGame, OpenDoor, OpenPeachDoor, CollectFirstHat, TryOnHat, TryOnLobster, TryOnSleepy, TryOnJester, CollecttwelveHats, CollectAllHats, PosessVase20, throwCan, ThrowCan20, Drive, Bonk5, Drive20, getcaught, CanGuard, CarGuard3 }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            Steamworks.SteamClient.Init(appID);
            connectedToSteam = true;
        }
        catch (System.Exception exception)
        {
            connectedToSteam = false;
        }

        UnlockAchievement(eAchievements.OpenGame);
    }

    // Update is called once per frame
    void Update()
    {
        if(connectedToSteam)
        {
            Steamworks.SteamClient.RunCallbacks();
        }
    }

    public void dissconnectFromSteam()
    {
        if(connectedToSteam)
        {
            Steamworks.SteamClient.Shutdown();
        }
    }

    public void UnlockAchievement(eAchievements Unlock)
    {
        if (connectedToSteam)
        {
            var achivement = new Steamworks.Data.Achievement("Achievement_" + (int)Unlock);
            achivement.Trigger();
        }
    }

    public void UpdateProgress(eAchievements Unlock, int currentProg, int maxProg)
    {
        Steamworks.SteamUserStats.IndicateAchievementProgress("Achievement_ " + (int)Unlock, currentProg, maxProg);
    }

    //this is strictly for testing so we can reset our achievements in case we need to test something
    private void ClearAchievements()
    {
        for(int i = 0; i < totalNumberOfAchievements; i++)
        {
            var achivement = new Steamworks.Data.Achievement("Achievement_" + i);
            achivement.Clear();
        }
    }
    
}
