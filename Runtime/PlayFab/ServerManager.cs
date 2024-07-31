#if GB_PLAYFAB
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickEye.Utility;
using System;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace GB
{
    public class ServerManager : AutoSingleton<ServerManager>
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        #region UserData

        public bool IsSyncUserData { get { return _waitUserDataKeys.Count == 0 && _pushUserDatas.Count == 0; } }

        const float USER_DATA_PUSH_DELAY = 2.0f;
        const int USER_DATA_PUSH_KEY_COUNT = 10;

        Dictionary<string, string> _pushUserDatas = new Dictionary<string, string>();
        Dictionary<string, string> _userData = new Dictionary<string, string>();

        public IReadOnlyDictionary<string, string> UserData { get { return _userData; } }
        float _pushUserDataTime;
        Queue<string> _waitUserDataKeys = new Queue<string>();

        public void UpdateUserData(string key, string value)
        {
            if (string.IsNullOrEmpty( key) == false)
            {
                _userData[key] = value;
                _waitUserDataKeys.Enqueue(key);
            }

            if (_pushUserDataTime < Time.time)
            {
                _pushUserDataTime = Time.time + USER_DATA_PUSH_DELAY - 0.01f;
                
                int len = _waitUserDataKeys.Count;
                Queue<string> temp = new Queue<string>();

                for (int i = 0; i < len; ++i)
                {
                    string mKey = _waitUserDataKeys.Dequeue();

                    if (_pushUserDatas.ContainsKey(mKey) 
                        && _pushUserDatas.Count >= USER_DATA_PUSH_KEY_COUNT
                        || _pushUserDatas.Count < USER_DATA_PUSH_KEY_COUNT)
                    {
                        _pushUserDatas[mKey] = _userData[mKey];
                    }
                    else
                    {
                        temp.Enqueue(mKey);
                    }
                }

                _waitUserDataKeys = temp;

                PlayFabManager.UpdateUserData(_pushUserDatas, () =>
                 {
                     _pushUserDatas.Clear();
                     if (_waitUserDataKeys.Count > 0)
                     {
                         GB.Timer.Create(USER_DATA_PUSH_DELAY, () => { UpdateUserData(null, null); });
                     }
                     else
                     {
                         if (_isGetUserData) GB.Timer.Create(USER_DATA_PUSH_DELAY, () => { GetUserData(); });
                     }
                 },
                 (error) =>
                 {
                     if (_pushUserDatas.Count > 0)
                         GB.Timer.Create(USER_DATA_PUSH_DELAY, () => { UpdateUserData(null, null); });
                 });
            }
        }

        bool _isGetUserData;
        public void GetUserData()
        {
            _isGetUserData = true;

            if (_pushUserDataTime < Time.time)
            {
                _pushUserDataTime = Time.time + USER_DATA_PUSH_DELAY - 0.01f;

                GB.PlayFabManager.GetUserData(
                    () =>
                    {
                        _isGetUserData = false;

                        if (_waitUserDataKeys.Count > 0)
                            GB.Timer.Create(USER_DATA_PUSH_DELAY, () => { UpdateUserData(null, null); });
                    },
                    (error) =>
                    {
                        if (_waitUserDataKeys.Count > 0)
                            GB.Timer.Create(USER_DATA_PUSH_DELAY, () => { UpdateUserData(null, null); });
                    });
            }

        }

        #endregion

        #region UserVirtualCurrency

        public bool IsSyncVirtualCurrency { get { return _waitCurrencyKeys.Count == 0; } }
        const float CURRENCY_PUSH_DELAY = 1f;
        Queue<KeyValuePair<string, KeyValuePair<string, int>>> _waitCurrencyKeys = new Queue<KeyValuePair<string, KeyValuePair<string, int>>>();
        float _pushCurrencyTime;
        
        public void AddVirtualCurrency(string key, int amount)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                _waitCurrencyKeys.Enqueue(new KeyValuePair<string, KeyValuePair<string, int>>("+", new KeyValuePair<string, int>( key, amount)));
            }

            if (_pushCurrencyTime < Time.time)
            {
                _pushCurrencyTime = Time.time + (CURRENCY_PUSH_DELAY - 0.1f);
                if (_waitCurrencyKeys.Count > 0)
                {
                    var mKey = _waitCurrencyKeys.Dequeue().Value;
                    
                    GB.PlayFabManager.AddVirtualCurrency(mKey.Key, mKey.Value,
                        (result) =>
                        {
                            if (_waitCurrencyKeys.Count > 0)
                                GB.Timer.Create(CURRENCY_PUSH_DELAY, () => 
                                {
                                    var p  = _waitCurrencyKeys.Peek();
                                    if (string.Equals(p.Key, "+"))
                                        AddVirtualCurrency(null, 0);
                                    else
                                        SubtractVirtualCurrency(null, 0);
                                });
                        },
                        (error) =>
                        {
                               GB.Timer.Create(CURRENCY_PUSH_DELAY, () => { AddVirtualCurrency(key, amount); });
                        });
                }
            }

        }

        public void SubtractVirtualCurrency(string key, int amount)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                _waitCurrencyKeys.Enqueue(new KeyValuePair<string, KeyValuePair<string, int>>("-", new KeyValuePair<string, int>(key, amount)));
            }

            if (_pushCurrencyTime < Time.time)
            {
                _pushCurrencyTime = Time.time + (CURRENCY_PUSH_DELAY - 0.1f);

                if (_waitCurrencyKeys.Count > 0)
                {
                    var mKey = _waitCurrencyKeys.Dequeue().Value;
                    GB.PlayFabManager.SubtractVirtualCurrency(mKey.Key, mKey.Value,
                        (result) =>
                        {
                            if (_waitCurrencyKeys.Count > 0)
                            {
                                GB.Timer.Create(CURRENCY_PUSH_DELAY, () =>
                                {
                                    var p = _waitCurrencyKeys.Peek();
                                    if (string.Equals(p.Key, "+"))
                                        AddVirtualCurrency(null, 0);
                                    else
                                        SubtractVirtualCurrency(null, 0);
                                });
                            }

                        },
                        (error) =>
                        {
                            GB.Timer.Create(CURRENCY_PUSH_DELAY, () => { SubtractVirtualCurrency(key, amount); });
                        });
                }

            }
        }
        #endregion

        #region GetTitleData Loop

        const float  GET_TITLEDATA_DELAY = 10f;
        bool _isLoopGetTitleData;
        public bool IsLoopGetTitleData{ get { return _isLoopGetTitleData; } }

        public void PlayGetTitleDataLoop()
        {
            if (_isLoopGetTitleData) return;
            _isLoopGetTitleData = true;
            LoopGetTitleData();
        }
        
        private void LoopGetTitleData()
        {
            GB.PlayFabManager.GetTitleData(
                () =>
                {
                    GB.Timer.Create(GET_TITLEDATA_DELAY, () => { LoopGetTitleData(); });
                },
                (error) =>
                {
                    GB.Timer.Create(GET_TITLEDATA_DELAY, () => { LoopGetTitleData(); });
                });
        }
        #endregion

    }

    #region NetLog
    public class Net
    {
        

        public enum NetType
        {
            None = 0,
            Send_Add,
            Send,
            Recive,
            Recive_Err,
        }
        public static void Log(string msg, NetType netType = NetType.None)
        {
#if UNITY_EDITOR
            switch (netType)
            {
                case NetType.None:
                    Debug.Log(msg);
                    break;
                case NetType.Send_Add:
                    UnityColorLog(msg, "[Net]>>>>", "#BDB76B", p => Debug.Log(p));
                    break;
                case NetType.Send:
                    UnityColorLog(msg, "[Net]>>>>", "orange", p => Debug.Log(p));
                    break;
                case NetType.Recive:
                    UnityColorLog(msg, "[Net]<<<<", "#EEE8AA", p => Debug.Log(p));
                    break;
                case NetType.Recive_Err:
                    UnityColorLog(msg, "[Net]<<<<", "#DC143C", p => Debug.Log(p));
                    break;
            }
#else
        Debug.Log(msg);
#endif
        }

        private static void UnityColorLog(string log, string tag, string colorValue, Action<string> logCallback)
        {
            log = log.Replace("\r\n", $"</color>\r\n<color={colorValue}>");

            const int maxAddCount = 100;
            const int maxTextSize = 11000;
            int startLogPos = 0;

            int pos = 0;

            for (int i = 0; i < maxAddCount && startLogPos < log.Length; i++)
            {
                pos += Mathf.Min(log.Length - pos, maxTextSize);

                while (pos < log.Length && !(log[pos] == ',' || log[pos] == '\"')) 
                    pos++;

                string frontMsg = (i == 0) ? string.Empty : $"<color=#ffffff>(===============>)</color> ";

                
                string viewMsg = log.Substring(startLogPos, pos - startLogPos);

                if (colorValue == null)
                    logCallback?.Invoke(viewMsg);
                else
                {
                    if (string.IsNullOrEmpty(tag))
                        logCallback?.Invoke($"{frontMsg}<color={colorValue}>{viewMsg}</color>");
                    else
                        logCallback?.Invoke($"{frontMsg}<color={colorValue}>[{tag}] {viewMsg}</color>");
                }
                startLogPos = pos;
            }
        }

        
    }
    #endregion
}

#endif