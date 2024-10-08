
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections.Generic;
namespace GB
{
    [RequireComponent(typeof(Button))]
    public class UIButtonEvent : MonoBehaviour
    {
        public enum BtnEvent{ShowPopup,ClosePopup,ChangeScene,SetTap,On_Object,Off_Object}
        [OnValueChanged("ChangeType")][Header("Button Event")]public BtnEvent btnEvent;

        [ShowIf("IsPopup")]        
        public string popupName;


        [ShowIf("IsScene")]
        [Scene]
        public string sceneName;


        [ShowIf("IsTap")]        
        public UISkinner tapSkinner;

        [Header("UISkinner Name")]
        [ShowIf("IsTap")]
        public string tapName;



        [ShowIf("IsObject")]        
        public List<GameObject> gameObjectList;


        int _state;

        public bool IsPopup() { return _state == 0; }
        public bool IsScene() { return _state == 1;}

        public bool IsTap(){return _state == 2;}

        public bool IsObject(){return _state == 3;}




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
                if(tapSkinner != null) DestroyImmediate(tapSkinner);
                break;

                case BtnEvent.ChangeScene:
                _state = 1;
                if(tapSkinner != null) DestroyImmediate(tapSkinner);
                break;
                
                case BtnEvent.SetTap:
                _state = 2;
                if(tapSkinner == null) tapSkinner = gameObject.AddComponent<UISkinner>();       
                break;

                case BtnEvent.On_Object:
                case BtnEvent.Off_Object:
                gameObjectList = new List<GameObject>();
                _state = 3;
                 if(tapSkinner != null) DestroyImmediate(tapSkinner);
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

                case BtnEvent.SetTap:

                if(tapSkinner != null)
                {
                    tapSkinner.SetSkin(tapName);
                    tapSkinner.Apply();
                }
                break;

                case BtnEvent.On_Object:                
                for(int i = 0; i< gameObjectList.Count; ++i) gameObjectList[i].SetActive(true);
                break;

                case BtnEvent.Off_Object:
                for(int i = 0; i< gameObjectList.Count; ++i) gameObjectList[i].SetActive(false);                
                break;
            }

        }
        
    }

}