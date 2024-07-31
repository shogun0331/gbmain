
#if GB_PLAYFAB
using System.Collections.Generic;
using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;
namespace GB
{
    public class PlayFabManager : AutoSingleton<PlayFabManager>
    {

        [SerializeField] GetPlayerCombinedInfoRequestParams _InfoRequestParams;

        [SerializeField] bool _isLogin;
        public bool IsLogin { get { return _isLogin; } }

        [SerializeField] string _playFabId;
        public string PlayFabId { get { return _playFabId; } }

        [SerializeField] string _sessionTicket;
        public string SessionTicket { get { return _sessionTicket; } }

        Dictionary<string, string> _titleData;
        public IReadOnlyDictionary<string, string> TitleData { get { return _titleData; } }

        Dictionary<string, string> _userData;
        public IReadOnlyDictionary<string, string> UserData { get { return _userData; } }

        Dictionary<string, string> _userReadOnlyData;
        public IReadOnlyDictionary<string, string> UserReadOnlyData { get { return _userReadOnlyData; } }

        Dictionary<string, int> _userVirtualCurrency;
        public IReadOnlyDictionary<string, int> UserVirtualCurrency { get { return _userVirtualCurrency; } }

        List<ItemInstance> _userInventory;
        public IReadOnlyCollection<ItemInstance> UserInventory { get { return _userInventory; } }

        EntityKey _entityKey;
        public EntityKey EntityKey { get { return _entityKey; } }

        DateTime _lastLoginDateTime;
        public DateTime LastLoginDateTime { get { return _lastLoginDateTime; } }

        PlayerProfileModel _playerProfile;
        public PlayerProfileModel PlayerProfile { get { return _playerProfile; } }

        bool _isInit;

        Dictionary<string, List<PlayerLeaderboardEntry>> _leaderboards;
        public IReadOnlyDictionary<string, List<PlayerLeaderboardEntry>> Leaderboards { get { return _leaderboards; } }
        Dictionary<string, PlayerLeaderboardEntry> _myLeaderboardRanks;
        public IReadOnlyDictionary<string, PlayerLeaderboardEntry> MyLeaderboardRanks { get { return _myLeaderboardRanks; } }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }


        private void Init()
        {
            if (_isInit == true) return;

            if (_InfoRequestParams == null)
                _InfoRequestParams = new GetPlayerCombinedInfoRequestParams();

            _InfoRequestParams.GetCharacterInventories = false;
            _InfoRequestParams.GetCharacterList = false;
            _InfoRequestParams.GetPlayerProfile = true;
            _InfoRequestParams.GetPlayerStatistics = true;
            _InfoRequestParams.GetTitleData = true;
            _InfoRequestParams.GetUserData = true;
            _InfoRequestParams.GetUserAccountInfo = true;
            _InfoRequestParams.GetUserInventory = true;
            _InfoRequestParams.GetUserReadOnlyData = true;
            _InfoRequestParams.GetUserVirtualCurrency = true;

            _isInit = true;
        }
        public static void LoginWithCustomID(bool createAccount, Action<LoginResult> success, Action<PlayFabError> error)
        {
            I.Init();

            Net.Log("LoginWithCustomIDRequest", Net.NetType.Send);

            var request = new LoginWithCustomIDRequest
            {
                CustomId = PlayFabSettings.DeviceUniqueIdentifier,
                CreateAccount = createAccount,
                InfoRequestParameters = I._InfoRequestParams
            };

            PlayFabClientAPI.LoginWithCustomID(request,
                (LoginResult suc) =>
                {
                    I._isLogin = true;
                    SuccessSignin(suc);
                    success?.Invoke(suc);

                    Net.Log("LoginWithCustomIDRequest - Success", Net.NetType.Recive);
                },
                (err) =>
                {
                    error?.Invoke(err);
                    Net.Log("LoginWithCustomIDRequest - Error", Net.NetType.Recive_Err);
                });
        }
        public static void LoginWithCustomID(string custumID, bool createAccount, Action<LoginResult> success, Action<PlayFabError> error)
        {
            I.Init();

            Net.Log("LoginWithCustomIDRequest", Net.NetType.Send);

            var request = new LoginWithCustomIDRequest
            {
                CustomId = custumID,
                CreateAccount = createAccount,
                InfoRequestParameters = I._InfoRequestParams
            };

            PlayFabClientAPI.LoginWithCustomID(request,
                (LoginResult suc) =>
                {
                    I._isLogin = true;
                    SuccessSignin(suc);
                    success?.Invoke(suc);

                    Net.Log("LoginWithCustomIDRequest - Success", Net.NetType.Recive);
                },
                (err) =>
                {
                    error?.Invoke(err);
                    Net.Log("LoginWithCustomIDRequest - Error", Net.NetType.Recive_Err);
                });
        }
        public static void UpdateUserData(Dictionary<string, string> data, Action success, Action<PlayFabError> error)
        {
            if (I.IsLogin == false) return;

            Net.Log("UpdateUserData Request", Net.NetType.Send);

            var request = new UpdateUserDataRequest { Data = data };

            PlayFabClientAPI.UpdateUserData(request,
            (UpdateUserDataResult result) =>
            {
                foreach (var v in data) I._userData[v.Key] = v.Value;
                success?.Invoke();
                Net.Log("UpdateUserData Request - Success", Net.NetType.Recive);
            },
            (PlayFabError err) =>
            {
                error?.Invoke(err);
                Net.Log("UpdateUserData Request - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
            });
        }
        public static void GetUserData(Action success, Action<PlayFabError> error)
        {
            if (I.IsLogin == false) return;

            Net.Log("GetUserDataRequest", Net.NetType.Send);

            var request = new GetUserDataRequest{PlayFabId = I.PlayFabId};

            PlayFabClientAPI.GetUserData(request,
                (result) =>
                {
                    I._userData = ConverterUserDataRecord(result.Data);
                    Net.Log("GetUserDataRequest  - Success", Net.NetType.Recive);
                    success?.Invoke();
                },
                (err) =>
                {
                    Net.Log("GetUserDataRequest  - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                    error?.Invoke(err);
                });

        }
        public static void GetLeaderboard(string staticsName, int maxCount, Action success, Action<PlayFabError> error)
        {
            if (I.IsLogin == false) return;

            Net.Log("GetLeaderboardRequest", Net.NetType.Send);

            var profileConstraints = new PlayerProfileViewConstraints
            {
                ShowAvatarUrl = true,
                ShowBannedUntil = false,
                ShowCampaignAttributions = false,
                ShowContactEmailAddresses = false,
                ShowCreated = false,
                ShowDisplayName = true,
                ShowExperimentVariants = false,
                ShowLastLogin = false,
                ShowLinkedAccounts = false,
                ShowLocations = false,
                ShowMemberships = false,
                ShowOrigination = false,
                ShowPushNotificationRegistrations = false,
                ShowStatistics = true,
                ShowTags = false,
                ShowTotalValueToDateInUsd = false,
                ShowValuesToDate = false
            };

            var request = new GetLeaderboardRequest
            {

                MaxResultsCount = maxCount,
                StatisticName = staticsName,
                StartPosition = 0,
                ProfileConstraints = profileConstraints
            };

            PlayFabClientAPI.GetLeaderboard(request,
                (result) =>
                {
                    I._leaderboards[staticsName] = result.Leaderboard;
                    success?.Invoke();
                    Net.Log("GetLeaderboardRequest - Success", Net.NetType.Recive);

                },
                (err) =>
                {
                    error?.Invoke(err);
                    Net.Log("GetLeaderboardRequest - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                });
        }
        public static void GetLeaderboardMyRank(string staticsName, Action success, Action<PlayFabError> error)
        {
            if (I.IsLogin == false) return;
            Net.Log("GetLeaderboardAroundPlayerRequest", Net.NetType.Send);
            var profileConstraints = new PlayerProfileViewConstraints
            {
                ShowAvatarUrl = true,
                ShowBannedUntil = false,
                ShowCampaignAttributions = false,
                ShowContactEmailAddresses = false,
                ShowCreated = false,
                ShowDisplayName = true,
                ShowExperimentVariants = false,
                ShowLastLogin = false,
                ShowLinkedAccounts = false,
                ShowLocations = false,
                ShowMemberships = false,
                ShowOrigination = false,
                ShowPushNotificationRegistrations = false,
                ShowStatistics = true,
                ShowTags = false,
                ShowTotalValueToDateInUsd = false,
                ShowValuesToDate = false
            };

            var request = new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = staticsName,
                MaxResultsCount = 1,
                ProfileConstraints = profileConstraints,
                PlayFabId = I.PlayFabId,
            };
            PlayFabClientAPI.GetLeaderboardAroundPlayer(request,
                (result) =>
                {
                    if (result.Leaderboard != null && result.Leaderboard.Count > 0)
                        I._myLeaderboardRanks[staticsName] = result.Leaderboard[0];

                    success?.Invoke();

                    Net.Log("GetLeaderboardAroundPlayerRequest - Success", Net.NetType.Recive);
                },
                (err) =>
                {
                    error?.Invoke(err);
                    Net.Log("GetLeaderboardAroundPlayerRequest - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                });
        }
        public static void UpdateLeaderboard(List<StatisticUpdate> statisticUpdates, Action success, Action<PlayFabError> error)
        {
            if (I.IsLogin == false) return;

            Net.Log("UpdatePlayerStatisticsRequest", Net.NetType.Send);

            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = statisticUpdates,

            };
            PlayFabClientAPI.UpdatePlayerStatistics(request,
                (result) =>
                {
                    Net.Log("UpdatePlayerStatistics - Success", Net.NetType.Recive);
                    success?.Invoke();

                },
                (err) =>
                {
                    Net.Log("UpdatePlayerStatistics - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                    error?.Invoke(err);
                });
        }
        public static void UpdateLeaderboard(string staticsName, int score, Action success, Action<PlayFabError> error)
        {
            if (I.IsLogin == false) return;

            Net.Log("UpdatePlayerStatisticsRequest", Net.NetType.Send);

            List<StatisticUpdate> statisticUpdates = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = staticsName,
                    Value = score
                }
            };

            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = statisticUpdates,

            };
            PlayFabClientAPI.UpdatePlayerStatistics(request,
                (result) =>
                {
                    Net.Log("UpdatePlayerStatistics - Success", Net.NetType.Recive);
                    success?.Invoke();

                },
                (err) =>
                {
                    Net.Log("UpdatePlayerStatistics - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                    error?.Invoke(err);
                });

        }
        public static void AddVirtualCurrency(string currenyID, int amount, Action<ModifyUserVirtualCurrencyResult> success, Action<PlayFabError> error)
        {

            Net.Log("AddUserVirtualCurrencyRequest", Net.NetType.Send);
            var request = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = currenyID,
                Amount = amount
            };

            PlayFabClientAPI.AddUserVirtualCurrency(request,
                (result) =>
                {
                    I._userVirtualCurrency[result.VirtualCurrency] = result.Balance;
                    success?.Invoke(result);
                    Net.Log("AddUserVirtualCurrencyRequest - Success", Net.NetType.Recive);
                },
                (err) =>
                {
                    error?.Invoke(err);
                    Net.Log("AddUserVirtualCurrencyRequest - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                });
        }
        public static void SubtractVirtualCurrency(string currenyID, int amount, Action<ModifyUserVirtualCurrencyResult> success, Action<PlayFabError> error)
        {

            Net.Log("SubtractUserVirtualCurrencyRequest", Net.NetType.Send);
            var request = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = currenyID,
                Amount = amount
            };

            PlayFabClientAPI.SubtractUserVirtualCurrency(request,
                (result) =>
                {
                    I._userVirtualCurrency[result.VirtualCurrency] = result.Balance;
                    success?.Invoke(result);
                    Net.Log("SubtractUserVirtualCurrencyRequest - Success", Net.NetType.Recive);
                },
                (err) =>
                {
                    error?.Invoke(err);
                    Net.Log("SubtractUserVirtualCurrencyRequest - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                });
        }
        public static void GetTime(Action success, Action<PlayFabError> error)
        {
            Net.Log("GetTimeRequest", Net.NetType.Send);
            PlayFabClientAPI.GetTime(new GetTimeRequest(),
                (result) =>
                {
                    I.ResultServerTimeSuccess(result.Time);
                    Net.Log("GetTimeRequest - Success", Net.NetType.Recive);
                },
                (err) =>
                {
                    error?.Invoke(err);
                    Net.Log("GetTimeRequest - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                });
        }
        public static void ExecuteCloudScript(string funcName,object parameter, Action<ExecuteCloudScriptResult> success, Action<PlayFabError> error)
        {

            Net.Log("ExecuteCloudScriptRequest", Net.NetType.Send);
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = funcName,
                FunctionParameter = parameter,
                GeneratePlayStreamEvent = false
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (result) =>
                {
                    Net.Log("ExecuteCloudScriptRequest - Success", Net.NetType.Recive);
                    success?.Invoke(result);
                },
                (err) =>
                {
                    error?.Invoke(err);
                    Net.Log("ExecuteCloudScriptRequest - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                });
        }
        public static void GetUserInventory(Action success, Action<PlayFabError> error)
        {
            Net.Log("GetUserInventoryRequest", Net.NetType.Send);
            var request = new GetUserInventoryRequest();

            PlayFabClientAPI.GetUserInventory(request,
                (result) =>
                {
                    I._userInventory = result.Inventory;
                    I._userVirtualCurrency = result.VirtualCurrency;

                    Net.Log("GetUserInventoryRequest - Success", Net.NetType.Recive);
                    success?.Invoke();
                },
                (err) =>
                {
                    error?.Invoke(err);
                    Net.Log("GetUserInventoryRequest - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                });
        }
        public static void GetTitleData(Action success, Action<PlayFabError> error)
        {
            Net.Log("GetTitleData", Net.NetType.Send);
            
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
                (suc)=>
                {
                    I._titleData = suc.Data;
                    Net.Log("GetTitleData - Success", Net.NetType.Recive);
                    success?.Invoke();
                },
                (err)=>
                {
                    error?.Invoke(err);
                    Net.Log("GetUserInventoryRequest - Error \n " + err.GenerateErrorReport(), Net.NetType.Recive_Err);
                });

        }
        private static void SuccessSignin(LoginResult result)
        {
            I._playFabId = result.PlayFabId;
            I._sessionTicket = result.SessionTicket;
            I._entityKey = result.EntityToken.Entity;

            if (result.LastLoginTime != null)
                I._lastLoginDateTime = result.LastLoginTime.Value;


            if (result.InfoResultPayload != null)
            {

                if (result.InfoResultPayload.TitleData != null)
                    I._titleData = result.InfoResultPayload.TitleData;

                if (result.InfoResultPayload.UserData != null)
                    I._userData = ConverterUserDataRecord(result.InfoResultPayload.UserData);

                if (result.InfoResultPayload.UserReadOnlyData != null)
                    I._userReadOnlyData = ConverterUserDataRecord(result.InfoResultPayload.UserReadOnlyData);

                if (result.InfoResultPayload.UserVirtualCurrency != null)
                    I._userVirtualCurrency = result.InfoResultPayload.UserVirtualCurrency;

                if (result.InfoResultPayload.UserInventory != null)
                    I._userInventory = result.InfoResultPayload.UserInventory;

                if (result.InfoResultPayload.PlayerProfile != null)
                    I._playerProfile = result.InfoResultPayload.PlayerProfile;
            }

        }
        private static Dictionary<string, string> ConverterUserDataRecord(Dictionary<string, UserDataRecord> data)
        {
            Dictionary<string, string> convertData = new Dictionary<string, string>();

            foreach (var v in data) convertData.Add(v.Key, v.Value.Value);

            return convertData;
        }

        #region ServerTime

        private int _serverTimeSecond;

        private float _recvServerTimeSecond;

        private DateTime _serverDateTime = new DateTime(1970, 1, 1);

        public DateTime ServerDateTime
        {
            get
            {
                return _serverDateTime.AddSeconds(UnixTimeStamp);
            }
        }
        public int UnixTimeStamp
        {
            get
            {
                return (int)(Time.realtimeSinceStartup - _recvServerTimeSecond) + _serverTimeSecond;
            }
        }

        private void ResultServerTimeSuccess(DateTime serverTime)
        {
            
            _recvServerTimeSecond = Time.realtimeSinceStartup;
            _serverTimeSecond = (int)(serverTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        #endregion


    }
}
#endif