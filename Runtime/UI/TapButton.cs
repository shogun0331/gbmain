using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace GB
{
    [RequireComponent(typeof(Button))]
    public class TapButton : MonoBehaviour
    {
        [SerializeField] UnityEvent _OnEvent;
        [SerializeField] UnityEvent _OffEvent;

        [SerializeField] USkin _OnSkin;
        [SerializeField] USkin _OffSkin;

        Action<GameObject> _click;

        public RectTransform rectTransform;

        public bool isOn;

        Button _btn;
        void Awake()
        {
            _btn = GetComponent<Button>();
            rectTransform = GetComponent<RectTransform>();

            if (_btn != null)
                _btn.onClick.AddListener(OnTap);

            if (_OffSkin != null) _OffSkin.Apply();


        }
    
        public void AddClickListener(Action<GameObject> click)
        {
            _click = click;
        }


        public void OnTap()
        {
            if (isOn) return;
            isOn = true;
            _OnEvent?.Invoke();

            if (_OnSkin != null) _OnSkin.Apply();

            _click?.Invoke(this.gameObject);
        }

        public void OffTap()
        {
            if (isOn == false) return;
            isOn = false;
            _OffEvent?.Invoke();

            if (_OffSkin != null) _OffSkin.Apply();

        }

    }
}