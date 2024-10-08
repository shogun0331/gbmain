
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections.Generic;
namespace GB
{
    [RequireComponent(typeof(Button))]
    public class UIButtonEvent : MonoBehaviour
    {
        public enum BtnEvent{ShowPopup,ClosePopup,ChangeScene,SetSkin}
        [OnValueChanged("ChangeType")][Header("Button Event")]public BtnEvent btnEvent;

        [ShowIf("IsPopup")]        
        public string popupName;


        [ShowIf("IsScene")]
        [Scene]
        public string sceneName;


        [ShowIf("IsSkin")]        
        public USkin tapSkinner;




        int _state;

        public bool IsPopup() { return _state == 0; }
        public bool IsScene() { return _state == 1;}

        public bool IsSkin(){return _state == 2;}





        Button _btn;
        void Awake()
        {
            _btn = GetComponent<Button>();

            if(_btn != null)
            _btn.onClick.AddListener(OnClick);
        }
        void ChangeType()
        {
            switch(btnEvent)
            {
                case BtnEvent.ClosePopup:
                case BtnEvent.ShowPopup:
                _state = 0;
                break;

                case BtnEvent.ChangeScene:
                _state = 1;
                break;
                
                case BtnEvent.SetSkin:
                _state = 2;
                break;
            }
        }

        void OnClick()
        {

            switch(btnEvent)
            {
                case BtnEvent.ShowPopup:
                UIManager.ShowPopup(popupName);
                break;

                case BtnEvent.ClosePopup:
                UIManager.ClosePopup(popupName);
                break;

                case BtnEvent.ChangeScene:
                UIManager.ChangeScene(sceneName);
                break;

                case BtnEvent.SetSkin:
                if(tapSkinner != null) tapSkinner.Apply();
                break;

            }

        }
        
    }

}