using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

namespace GB
{
    public class LocalizationView : View
    {
        public string LocalizationKey
        {
            get
            {
                return _LocalizationKey;
            }
            set
            {
                _LocalizationKey = value;
                if (string.IsNullOrEmpty(_LocalizationKey) == false && _isBind == false)
                {
                    Presenter.Bind("Localization", this);
                    _isBind = true;
          
                }
                Refresh();

            }
        }
        [SerializeField] string _LocalizationKey;

        [HideInInspector] public List<string> ParamList = new List<string>();

        TextMeshProUGUI _TextTMP;
        Text _Text;

        string _prevText;

        bool _isBind;

        private void Awake()
        {
            _TextTMP ??= GetComponent<TextMeshProUGUI>();
            _Text ??= GetComponent<Text>();
        }
        private void OnEnable()
        {
            if (string.IsNullOrEmpty(_LocalizationKey))
                return;


            Presenter.Bind("Localization", this);
            Refresh();

            _isBind = true;
        }

        private void OnDisable()
        {
            if (string.IsNullOrEmpty(_LocalizationKey)) return;
            Presenter.UnBind("Localization", this);

            _isBind = false;
        }

        [Button]
        private void Setting()
        {
            
            Apply( LocalizationManager.GetValue(LocalizationKey));
        }
        
        private void Apply(string text)
        {
            if (string.Equals(_prevText, text) == false)
            {
                _prevText = text;


                if (Application.isPlaying)
                {

                    if (_Text == null) _Text = GetComponent<Text>();
                    if (_TextTMP == null) _TextTMP = GetComponent<TextMeshProUGUI>();

                    if (_TextTMP != null)
                        _TextTMP.text = text;

                    if (_Text != null)
                        _Text.text = text;
                }
                else
                {

                    if (GetComponent<TextMeshProUGUI>() != null)
                        GetComponent<TextMeshProUGUI>().text = text;

                    if (GetComponent<Text>() != null)
                        GetComponent<Text>().text = text;

                }
            }
        }

        private void SetText(string param1)
        {
            string value = LocalizationManager.GetValue(LocalizationKey);
            Apply(string.Format(value, param1));
        }

        private void SetText(string param1,string param2)
        {
            string value = LocalizationManager.GetValue(LocalizationKey);
            Apply(string.Format(value, param1,param2));
        }

        private void SetText(string param1, string param2,string param3)
        {
            string value = LocalizationManager.GetValue(LocalizationKey);
            Apply(string.Format(value, param1, param2, param3));
        }

        private void SetText(string param1, string param2, string param3,string param4)
        {
            string value = LocalizationManager.GetValue(LocalizationKey);
            Apply(string.Format(value, param1, param2, param3,param4));
        }
        private void SetText(string param1, string param2, string param3, string param4, string param5)
        {
            string value = LocalizationManager.GetValue(LocalizationKey);
            Apply(string.Format(value, param1, param2, param3, param4,param5));
        }

        private void SetText(string param1, string param2, string param3, string param4, string param5,string param6)
        {
            string value = LocalizationManager.GetValue(LocalizationKey);
            Apply(string.Format(value, param1, param2, param3, param4, param5, param6));
        }
        public void Refresh()
        {
            if (ParamList == null || ParamList.Count == 0)
            {
                Apply(LocalizationManager.GetValue(LocalizationKey));
            }
            else
            {
                if (ParamList.Count == 1)
                    SetText(ParamList[0]);
                else if (ParamList.Count == 2)
                    SetText(ParamList[0], ParamList[1]);
                else if (ParamList.Count == 3)
                    SetText(ParamList[0], ParamList[1], ParamList[2]);
                else if (ParamList.Count == 4)
                    SetText(ParamList[0], ParamList[1], ParamList[2], ParamList[3]);
                else if (ParamList.Count == 5)
                    SetText(ParamList[0], ParamList[1], ParamList[2], ParamList[3], ParamList[4]);
                else if (ParamList.Count == 6)
                    SetText(ParamList[0], ParamList[1], ParamList[2], ParamList[3], ParamList[4], ParamList[5]);
                else
                    Apply(LocalizationManager.GetValue(LocalizationKey));
            }
        }


        public override void ViewQuick(string key, IModel data)
        {
            if (string.Equals(key, LocalizationKey))
            {
                Refresh();
            }

        }
    }

}