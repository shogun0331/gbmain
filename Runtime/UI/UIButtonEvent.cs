
using UnityEngine;
using UnityEngine.UI;

namespace GB
{
    [RequireComponent(typeof(Button))]
    public class UIButtonEvent : MonoBehaviour
    {
        public enum BtnEvent{ShowPopup,ClosePopup,ChangeScene}

        [Header("Button Event")]
        public BtnEvent btnEvent;
        public string paramName;



        Button _btn;
        void Awake()
        {
            _btn = GetComponent<Button>();

            if(_btn != null)
            _btn.onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            if(string.IsNullOrEmpty( paramName))  return;

            switch(btnEvent)
            {
                case BtnEvent.ShowPopup:
                UIManager.ShowPopup(paramName);
                break;

                case BtnEvent.ClosePopup:
                UIManager.ClosePopup(paramName);
                break;

                case BtnEvent.ChangeScene:
                UIManager.ChangeScene(paramName);
                break;

            }

        }
        
    }

}