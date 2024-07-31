#if GB_SOCIAL
using System;
using UnityEngine;

namespace GB
{
    public class SocialManager : MonoBehaviour
    {
        static SocialManager _instance;
        public static SocialManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<SocialManager>();
                if (_instance == null)
                    _instance = new GameObject(typeof(SocialManager).Name).AddComponent<SocialManager>();
                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }

        public static string UserID => Social.localUser.id;
        public static string UserName => Social.localUser.userName;
        public static bool IsSignIn => Social.localUser.authenticated;


        public static void SignIn(Action<bool> result, bool isManually = false)
        {

#if UNITY_ANDROID
            GooglePlayGameService.Signin(result, isManually);
#else
        GameCenterService.Signin(result);
#endif

        }
        public static void PostToUnlockAchievement(string achievementID, Action<bool> result = null)
        {

#if UNITY_ANDROID
            GooglePlayGameService.PostToUnlockAchievement(achievementID, result);
#else
        GameCenterService.PostToUnlockAchievement(achievementID, result);
#endif

        }
        public static void PostToStepAchievement(string achievementID, int step, Action<bool> result = null)
        {

#if UNITY_ANDROID
            GooglePlayGameService.PostStepAchievement(achievementID, step, result);
#else
        GameCenterService.PostStepAchievement(achievementID, step, result);
#endif
        }

        public static void PostToLeaderboard(string leaderboardID, long score, Action<bool> result = null)
        {

#if UNITY_ANDROID
            GooglePlayGameService.PostToLeaderboard(leaderboardID, score, result);
#else
        GameCenterService.PostToLeaderboard(leaderboardID, score, result);
#endif

        }
        public static void ShowAchievementsUI(bool isAutoSignin = true)
        {


#if UNITY_ANDROID
            GooglePlayGameService.ShowAchievementsUI(isAutoSignin);
#else
        GameCenterService.ShowAchievementsUI(isAutoSignin);
#endif

        }
        public static void ShowLeaderboardUI(bool isAutoSignin = true)
        {
#if UNITY_ANDROID
            GooglePlayGameService.ShowLeaderboardUI(isAutoSignin);
#else
        GameCenterService.ShowLeaderboardUI(isAutoSignin);
#endif
        }


        public static void CloudSave(string fileName, string data, Action<bool> result = null)
        {

#if UNITY_ANDROID
            GooglePlayGameService.CloudSave(fileName, data, result);
#else
        GameCenterService.CloudSave(fileName,data, result);
#endif

        }


        public static void CloudLoad(string fileName, Action<bool, string> result)
        {
#if UNITY_ANDROID
            GooglePlayGameService.CloudLoad(fileName, result);
#else
        GameCenterService.CloudLoad(fileName,  result);
#endif
        }



    }
}
#endif