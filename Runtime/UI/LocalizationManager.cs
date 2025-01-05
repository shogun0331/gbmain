using System.Collections.Generic;
using UnityEngine;
using QuickEye.Utility;
using Newtonsoft.Json;
using System;
using NaughtyAttributes;


namespace GB
{
    public enum LanguageCode
    {
        English,
        Korean,
        Japanese,
        ChineseSimplified,
        ChineseTraditional,
        German,
        Spanish,
        French,
        Vietnamese,
        Thai,
        Russian,
        Italian,
        Portuguese,
        Turkish,
        Indonesian,
        Hindi
    }

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

        [OnValueChanged("ChangeLanguage")] [SerializeField]  LanguageCode _Language;
        public LanguageCode Language { get { return _Language; } }

       [SerializeField]  UnityDictionary<string, UnityDictionary<LanguageCode, string>> _Datas = new UnityDictionary<string, UnityDictionary<LanguageCode, string>>();

        [SerializeField] UnityDictionary<LanguageCode, Font> _fonts;

        [SerializeField] Font _defaultFont;
        public Font GetFont()
        {
            if(_fonts == null) return _defaultFont;

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

            var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(_textAsset.text);
            var datas = JsonConvert.DeserializeObject<List<LocalizationData>>(d["Datas"].ToString());
            _Datas = new UnityDictionary<string, UnityDictionary<LanguageCode, string>>();
            for (int i = 0; i < datas.Count; ++i)
            {
                var tmp = new UnityDictionary<LanguageCode, string>();
                tmp.Add(LanguageCode.English, datas[i].English);
                tmp.Add(LanguageCode.Korean, datas[i].Korean);
                tmp.Add(LanguageCode.Japanese, datas[i].Japanese);
                tmp.Add(LanguageCode.ChineseSimplified, datas[i].ChineseSimplified);
                tmp.Add(LanguageCode.ChineseTraditional, datas[i].ChineseTraditional);
                tmp.Add(LanguageCode.German, datas[i].German);
                tmp.Add(LanguageCode.Spanish, datas[i].Spanish);
                tmp.Add(LanguageCode.French, datas[i].French);
                tmp.Add(LanguageCode.Vietnamese, datas[i].Vietnamese);
                tmp.Add(LanguageCode.Thai, datas[i].Thai);
                tmp.Add(LanguageCode.Russian, datas[i].Russian);
                tmp.Add(LanguageCode.Italian, datas[i].Italian);
                tmp.Add(LanguageCode.Portuguese, datas[i].Portuguese);
                tmp.Add(LanguageCode.Turkish, datas[i].Turkish);
                tmp.Add(LanguageCode.Indonesian, datas[i].Indonesian);
                tmp.Add(LanguageCode.Hindi, datas[i].Hindi);
                _Datas.Add(datas[i].TextID.ToString(), tmp);
              
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
                I._Language = LanguageCode.English;
            
            
            string str = I._Datas[id][I._Language];

            
            return str;
           
        }

        private void ChangeLanguage()
        {
            ChangeLanguage(I.Language);
        }


        public static void ChangeLanguage(LanguageCode language)
        {
            I._Language = language;

            foreach (var v in I._Datas)
            {
                Presenter.Send("Localization", v.Key);
            }
            
            PlayerPrefs.SetString("Language", I._Language.ToString());

        }


        public void SetSystemLanguage(LanguageCode language)
        {
            _Language = language;
            ChangeLanguage(_Language);
        }




        public void SetSystemLanguage(string language)
        {
            _Language = LanguageCode.English;
        
            if (language.Equals(LanguageCode.Korean.ToString())) _Language = LanguageCode.Korean;
            else if (language.Equals(LanguageCode.Japanese.ToString())) _Language = LanguageCode.Japanese;
            else if (language.Equals(LanguageCode.ChineseSimplified.ToString())) _Language = LanguageCode.ChineseSimplified;
            else if (language.Equals(LanguageCode.ChineseTraditional.ToString())) _Language = LanguageCode.ChineseTraditional;
            else if (language.Equals(LanguageCode.German.ToString())) _Language = LanguageCode.German;
            else if (language.Equals(LanguageCode.Italian.ToString())) _Language = LanguageCode.Italian;
            else if (language.Equals(LanguageCode.Spanish.ToString())) _Language = LanguageCode.Spanish;
            else if (language.Equals(LanguageCode.French.ToString())) _Language = LanguageCode.French;
            else if (language.Equals(LanguageCode.Indonesian.ToString())) _Language = LanguageCode.Indonesian;
            else if (language.Equals(LanguageCode.Portuguese.ToString())) _Language = LanguageCode.Portuguese;
            else if (language.Equals(LanguageCode.Vietnamese.ToString())) _Language = LanguageCode.Vietnamese;
            else if (language.Equals(LanguageCode.Turkish.ToString())) _Language = LanguageCode.Turkish;
            else if (language.Equals(LanguageCode.Thai.ToString())) _Language = LanguageCode.Thai;
            else if (language.Equals(LanguageCode.Russian.ToString())) _Language = LanguageCode.Russian;
            else if (language.Equals(LanguageCode.Hindi.ToString())) _Language = LanguageCode.Hindi;


            ChangeLanguage(_Language);
        }


        private void getSystemLanguage()
        {
            SystemLanguage language = Application.systemLanguage;

            switch (language)
            {
                case SystemLanguage.Korean:
                    _Language = LanguageCode.Korean;
                    break;

                case SystemLanguage.English:
                    _Language = LanguageCode.English;
                    break;

                case SystemLanguage.Japanese:
                    _Language = LanguageCode.Japanese;
                    break;

                case SystemLanguage.ChineseSimplified:
                    _Language = LanguageCode.ChineseSimplified;
                    break;

                case SystemLanguage.ChineseTraditional:
                    _Language = LanguageCode.ChineseTraditional;
                    break;

                case SystemLanguage.German:
                    _Language = LanguageCode.German;
                    break;

                case SystemLanguage.Spanish:
                    _Language = LanguageCode.Spanish;

                    break;

                case SystemLanguage.French:
                    _Language = LanguageCode.French;

                    break;

                case SystemLanguage.Vietnamese:
                    _Language = LanguageCode.Vietnamese;

                    break;

                case SystemLanguage.Thai:
                    _Language = LanguageCode.Thai;

                    break;

                case SystemLanguage.Russian:
                    _Language = LanguageCode.Russian;

                    break;

                case SystemLanguage.Italian:
                    _Language = LanguageCode.Italian;

                    break;

                case SystemLanguage.Portuguese:
                    _Language = LanguageCode.Portuguese;

                    break;

                case SystemLanguage.Turkish:
                    _Language = LanguageCode.Turkish;
                    break;

                case SystemLanguage.Indonesian:
                    _Language = LanguageCode.Indonesian;

                    break;

                case SystemLanguage.Hindi:
                    _Language = LanguageCode.Hindi;
                    break;

                default:
                    _Language = LanguageCode.English;
                    break;

            }
            
            PlayerPrefs.SetString("Language", _Language.ToString());

        }




    }

}