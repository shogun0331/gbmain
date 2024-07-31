#if GB_SOCIAL
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi;
using System;
using UnityEngine;

namespace GB
{
    /// <summary>
    /// Gpgs V11.01 : https://github.com/playgameservices/play-games-plugin-for-unity
    /// </summary>
    /// 
    public class GooglePlayGameService : MonoBehaviour
    {
        static GooglePlayGameService _instance;
        public static GooglePlayGameService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<GooglePlayGameService>();
                if (_instance == null)
                    _instance = new GameObject(typeof(GooglePlayGameService).Name).AddComponent<GooglePlayGameService>();
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

        bool _isInit;

        public static void Signin(Action<bool> success, bool isManually)
        {
            if (!Instance._isInit) PlayGamesPlatform.Activate();

            Instance._isInit = true;

            PlayGamesPlatform.Instance.Authenticate((result) =>
            {
                if (result != GooglePlayGames.BasicApi.SignInStatus.Success)
                {
                    if (isManually)
                    {
                        PlayGamesPlatform.Instance.ManuallyAuthenticate((result2) =>
                        {
                            success?.Invoke(result2 == GooglePlayGames.BasicApi.SignInStatus.Success);
                        });
                    }
                    else
                    {
                        success?.Invoke(result == GooglePlayGames.BasicApi.SignInStatus.Success);
                    }

                }
            });

        }

        public static void PostStepAchievement(string achievementID, int step, Action<bool> result = null)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(
             achievementID, step, (bool success) =>
             {
                 // handle success or failure
                 result?.Invoke(success);
             });
        }

        public static void PostToUnlockAchievement(string achievementID, Action<bool> result = null)
        {
            if (!Social.localUser.authenticated)
            {
                result.Invoke(false);
                return;
            }


            if (PlayerPrefs.GetInt(achievementID, 0) == 1)
            {
                result?.Invoke(true);
                return;
            }

            Instance.UnlockAchievement(achievementID, result);

        }

        void UnlockAchievement(string achievementID, Action<bool> result = null)
        {
            Social.ReportProgress(achievementID, 100.0f, (bool success) =>
            {
                if (success) PlayerPrefs.SetInt(achievementID, 1);
                // handle success or failure
                result?.Invoke(success);
            });
        }

        public static void PostToLeaderboard(string leaderboardID, long score, Action<bool> result = null)
        {
            Social.ReportScore(score, leaderboardID, (bool success) =>
            {
                // handle success or failure
                result?.Invoke(success);
            });
        }

        public static void GetServerAuth(Action<string> authCode)
        {
            PlayGamesPlatform.Instance.RequestServerSideAccess(
        /* forceRefreshToken= */ false,
            code =>
            {
                authCode?.Invoke(code);
                // send code to server
            });
        }

        public static void ShowAchievementsUI(bool isAutoSignin = true)
        {
            if (isAutoSignin == false)
            {
                if (!Social.localUser.authenticated)
                    return;

                Social.ShowAchievementsUI();

            }
            else
            {
                if (!Social.localUser.authenticated)
                {
                    Signin((result) =>
                    {
                        if (result) Social.ShowAchievementsUI();
                    }, true);
                }
                else
                {
                    Social.ShowAchievementsUI();
                }
            }
        }

        public static void ShowLeaderboardUI(bool isAutoSignin = true)
        {
            if (isAutoSignin == false)
            {
                if (!Social.localUser.authenticated)
                    return;

                Social.ShowLeaderboardUI();

            }
            else
            {
                if (!Social.localUser.authenticated)
                {
                    Signin((result) =>
                    {
                        if (result) Social.ShowLeaderboardUI();
                    }, true);
                }
                else
                {
                    Social.ShowLeaderboardUI();
                }
            }
        }

        public static void CloudSave(string fileName, string data, Action<bool> result = null)
        {
            if (!Social.localUser.authenticated) return;

            Instance._ResultSave = result;
            Instance.SaveGame(fileName, data);
        }

        public static void CloudLoad(string fileName, Action<bool, string> result)
        {
            if (!Social.localUser.authenticated)
            {
                result?.Invoke(false, string.Empty);
                return;
            }

            Instance._ResultLoad = result;
            Instance.LoadGame(fileName);
        }

        void SaveGame(string fileName, string data)
        {
            _fileName = fileName;
            _data = data;
            _saving = true;
            OpenSavedGame(_fileName);
        }

        void LoadGame(string fileName)
        {
            _fileName = fileName;
            _saving = false;

            OpenSavedGame(_fileName);
        }


        string _fileName;
        string _data;
        bool _saving;

        Action<bool, string> _ResultLoad;
        Action<bool> _ResultSave;

        void OpenSavedGame(string filename)
        {

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, SavedGameOpened);
        }


        private void SavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                if (_saving)
                {

                    //TimeSpan playedTime = PlayerInfo.Instance._totalPlayingTime;
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(_data);


                    SavedGameMetadataUpdate.Builder builder = new
                    SavedGameMetadataUpdate.Builder()
                    //.WithUpdatedPlayedTime(playedTime)
                    .WithUpdatedDescription("Saved Game at " + DateTime.Now);
                    SavedGameMetadataUpdate updatedMetadata = builder.Build();
                    ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, updatedMetadata, data, SavedGameWritten);

                }
                else
                {

                    ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, SavedGameLoaded);

                }
            }
            else
            {
                Debug.LogWarning("Error opening game: " + status);
            }
        }

        private void SavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            _ResultSave?.Invoke(status == SavedGameRequestStatus.Success);

            if (status == SavedGameRequestStatus.Success)
                Debug.Log("Game " + game.Description + " written");
            else
                Debug.LogWarning("Error saving game: " + status);

        }

        private void SavedGameLoaded(SavedGameRequestStatus status, byte[] bytes)
        {
            if (status == SavedGameRequestStatus.Success)
            {

                Debug.Log("SaveGameLoaded, success=" + status);
                string data = System.Text.Encoding.UTF8.GetString(bytes);
                _ResultLoad?.Invoke(true, data);
            }
            else
            {
                Debug.LogWarning("Error reading game: " + status);
                _ResultLoad?.Invoke(false, string.Empty);

            }
        }



    }

}
#endif