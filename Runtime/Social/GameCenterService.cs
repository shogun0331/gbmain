#if GB_SOCIAL
using System;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif


namespace GB
{
    public class GameCenterService : MonoBehaviour
    {

        static GameCenterService _instance;
        public static GameCenterService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<GameCenterService>();
                if (_instance == null)
                    _instance = new GameObject(typeof(GameCenterService).Name).AddComponent<GameCenterService>();
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

        public static void Signin(Action<bool> result)
        {
            if (Social.localUser.authenticated)
            {
                result.Invoke(true);
                return;
            }

            if (!Social.localUser.authenticated && Instance._isInit)
            {
                result.Invoke(false);
                return;
            }

            Social.localUser.Authenticate((success) => {result.Invoke(success);});
            Instance._isInit = true;

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


        public static void PostStepAchievement(string achievementID, float step, Action<bool> result = null)
        {

            Social.ReportProgress(achievementID, step, (bool success) =>
            {
                // handle success or failure
                result?.Invoke(success);
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
                    });
                }
                else
                {
                    Social.ShowAchievementsUI();
                }
            }
        }
        public static void PostToLeaderboard(string leaderboardID, long score, Action<bool> result = null)
        {
            Social.ReportScore(score, leaderboardID, (bool success) =>
            {
                // handle success or failure
                result?.Invoke(success);
            });
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
                    });
                }
                else
                {
                    Social.ShowLeaderboardUI();
                }
            }
        }
        Action<bool, string> _ResultLoad;
        Action<bool> _ResultSave;

        public static void CloudSave(string fileName, string data, Action<bool> result = null)
        {
            if (!Social.localUser.authenticated) return;

            Instance._ResultSave = result;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            Instance.send((int)GAMESERVICES_MESSAGE.SAVE_SAVEDGAME, bytes);
        }

        public static void CloudLoad(string fileName, Action<bool, string> result)
        {
            if (!Social.localUser.authenticated)
            {
                result?.Invoke(false, string.Empty);
                return;
            }

            Instance._ResultLoad = result;
#if UNITY_IOS
            _send(GAMESERVICES_MESSAGE.LOAD_SAVEDGAME);
#elif UNITY_EDITOR
            Instance._ResultLoad?.Invoke(false, string.Empty);
#endif

        }


        void send(int type, byte[] data)
        {

#if UNITY_IOS
            _sendWithByteArray(type, data, data.Length);
#endif
            Instance._ResultSave?.Invoke(true);
        }


        public void receiveString(int type, string data)
        {

            switch (type)
            {
                //Load
                case (int)GAMESERVICES_MESSAGE.LOAD_SAVEDGAME:

                    if (string.Empty.Equals(data))
                    {
                        Instance._ResultLoad?.Invoke(true, string.Empty);
                        return;
                    }

                    Instance._ResultLoad?.Invoke(true, data);


                    break;
            }
        }


#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _send(int type);
    
    [DllImport("__Internal")]
    private static extern void _sendWithInt(int type, int data);
    
    [DllImport("__Internal")]
    private static extern void _sendWithString(int type, string data);
    
    [DllImport("__Internal")]
    private static extern void _sendWithStringInt(int type, string strData, int intData);
   
    [DllImport("__Internal")]
    private static extern void _sendWithByteArray(int type, byte[] data, int length);
    
    [DllImport("__Internal")]
    private static extern void _sendWithIntByteArray(int type, int data, byte[] byteArray, int length);
   
#endif

    }
    public enum GAMESERVICES_MESSAGE
    {
        SYNCHRO = 0,
        SIGNIN,
        PING,
        MULTIPLAY,
        READY_FOR_START,
        DATA,
        ENEMY_JOIN,

        CONNECT_CHECK,
        CONNECT_SUCCESS,

        AI_MATCHING,
        ENEMY_LEFT,
        ENEMY_MISSING,
        LEFT_ROOM,

        SAVE_SAVEDGAME,
        LOAD_SAVEDGAME,

        SEND_MYINFO,

        END_OF_ENUM
    };

}
#endif