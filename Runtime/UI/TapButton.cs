using System;
using Aya.Tween;
using QuickEye.Utility;
using UnityEngine;
using UnityEngine.UI;


namespace GB
{
    [RequireComponent(typeof(Button))]
    public class TapButton : MonoBehaviour
    {
        [SerializeField] TweenAnimation[] _On_TweenAnimations;
        [SerializeField] TweenAnimation[] _Off_TweenAnimations;

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


        }
        void Start()
        {
            if (_On_TweenAnimations != null)
            {
                for (int i = 0; i < _On_TweenAnimations.Length; ++i)
                    _On_TweenAnimations[i].WorldSpace = false;


            }


            if (_Off_TweenAnimations != null)
            {
                for (int i = 0; i < _Off_TweenAnimations.Length; ++i)
                    _Off_TweenAnimations[i].WorldSpace = false;
            }



        }

        public void AddClickListener(Action<GameObject> click)
        {
            _click = click;
        }


        public void OnTap()
        {
            if (isOn) return;
            isOn = true;

            if (_On_TweenAnimations == null) return;

            for (int i = 0; i < _On_TweenAnimations.Length; ++i)
            {
                
                _On_TweenAnimations[i].Play();
            }

            if (_OnSkin != null) _OnSkin.Apply();

            _click?.Invoke(this.gameObject);
        }

        public void OffTap()
        {
            if (isOn == false) return;
            isOn = false;


            if (_Off_TweenAnimations == null) return;

            for (int i = 0; i < _Off_TweenAnimations.Length; ++i)
            {
                
                _Off_TweenAnimations[i].Play();
            }

            if (_OffSkin != null) _OffSkin.Apply();

        }

    }
}