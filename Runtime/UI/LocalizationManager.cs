using System.Collections.Generic;
using UnityEngine;
using QuickEye.Utility;
using Newtonsoft.Json;
using System;
using NaughtyAttributes;


namespace GB
{
   

    [Serializable]
    public class LocalizationData
    {
        [JsonProperty] public readonly int TextID;
        [JsonProperty] public readonly string Korean;
        [JsonProperty] public readonly string English;
        [JsonProperty] public readonly string Japanese;
        [JsonProperty] public readonly string ChineseSimplified;
        [JsonProperty] public readonly string ChineseTraditional;
        [JsonProperty] public readonly string German;
        [JsonProperty] public readonly string Spanish;
        [JsonProperty] public readonly string French;
        [JsonProperty] public readonly string Vietnamese;
        [JsonProperty] public readonly string Thai;
        [JsonProperty] public readonly string Russian;
        [JsonProperty] public readonly string Italian;
        [JsonProperty] public readonly string Portuguese;
        [JsonProperty] public readonly string Turkish;
        [JsonProperty] public readonly string Indonesian;
        [JsonProperty] public readonly string Hindi;
    }


    public class LocalizationManager : AutoSingleton<LocalizationManager>
    {

        [SerializeField] TextAsset _textAsset;

        [OnValueChanged("ChangeLanguage")] [SerializeField]  SystemLanguage _Language;
        public SystemLanguage Language { get { return _Language; } }

        [SerializeField]  UnityDictionary<string, UnityDictionary<SystemLanguage, string>> _Datas = new UnityDictionary<string, UnityDictionary<SystemLanguage, string>>();
        [SerializeField] UnityDictionary<SystemLanguage, Font> _fonts;

        [SerializeField] Font _defaultFont;
        public Font GetFont()
        {
            if (_fonts.ContainsKey(_Language)) return _fonts[_Language];
            return _defaultFont;
        }


        private void Awake()
        {
            if (I != this)
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            if(_Datas == null || _Datas.Count <= 0)
            PaserData(true);
            
            if (PlayerPrefs.GetInt("GB_IsFirst", 0) == 0) getSystemLanguage();
            else SetSystemLanguage(PlayerPrefs.GetString("Language", "English"));

            
        }

        [Button]
        private void LoadButton()
        {
            PaserData(true);
        }

        public void PaserData(bool forece = false)
        {
            if (forece == false)
            {
                if (_Datas != null || _Datas.Count > 0) return;
            }

            if (_textAsset == null)
               _textAsset  = Resources.Load<TextAsset>("Json/TextTable");

            if (_textAsset == null) return;
       ;
            var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(_textAsset.text);
            var datas = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(d["Datas"].ToString());
            _Datas = new UnityDictionary<string, UnityDictionary<SystemLanguage, string>>();

            for (int i = 0; i < datas.Count; ++i)
            {
                int idx = 0;
                string dataKey = string.Empty;

                foreach (var v in datas[i])
                {
                    if (idx == 0)
                    {
                        _Datas[v.Value] = new UnityDictionary<SystemLanguage, string>();
                        dataKey = v.Value;
                    }
                    else
                    {
                        SystemLanguage language = GetLanguage(v.Key);
                        if (language != SystemLanguage.Unknown)
                            _Datas[dataKey][language] = v.Value;
                        else
                            Debug.Log("None Language : " + v.Key);
                    }

                    ++idx;
                }
            }
            
        }

        public static string GetValue(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            if (string.IsNullOrEmpty(id) == false)
                id = id.Replace(" ", "").Replace("\r", "");

            if (!I._Datas.ContainsKey(id))
            {
                return "<color=red>" + id + "</color>";
            }

            if (!I._Datas[id].ContainsKey(I._Language))
                I._Language = SystemLanguage.English;
            
            string str = I._Datas[id][I._Language];

            return str;
           
        }

        private void ChangeLanguage()
        {
            ChangeLanguage(I.Language);
        }


        public static void ChangeLanguage(SystemLanguage language)
        {
            I._Language = language;

            foreach (var v in I._Datas)
            {
                Presenter.Send("Localization", v.Key);
            }
            
            PlayerPrefs.SetString("Language", I._Language.ToString());

        }


        public void SetSystemLanguage(SystemLanguage language)
        {
            _Language = language;
            ChangeLanguage(_Language);
        }




        public void SetSystemLanguage(string language)
        {
            _Language = GetLanguage(language);
            if(_Language == SystemLanguage.Unknown)
            _Language = SystemLanguage.English; 
        
            ChangeLanguage(_Language);
        }
        
        SystemLanguage GetLanguage(string strLanguage)
        {
            int Length = (int)SystemLanguage.Unknown;

            for (int i = 0; i < Length; ++i)
            {
                SystemLanguage lan = (SystemLanguage)i;
                if (string.Equals(lan.ToString(), strLanguage))
                    return lan;
            }

            return SystemLanguage.Unknown;
        }


        private void getSystemLanguage()
        {
            _Language= GetLanguage(Application.systemLanguage.ToString());
            PlayerPrefs.SetString("Language", _Language.ToString());
        }




    }

}