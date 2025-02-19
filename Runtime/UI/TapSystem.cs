using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace GB
{
    public class TapSystem : MonoBehaviour
    {
        [SerializeField] TapButton[] _tapButtons;

        public int DefaultStartIndex = 2;

        RectTransform[] _rectTransforms;
        [SerializeField] bool _IsScaleMove;


        [ShowIf("IsMove")][SerializeField] Vector3 _On_Scale = Vector3.one;
        [ShowIf("IsMove")][SerializeField] Vector3 _Off_Scale = Vector3.one;
        [ShowIf("IsMove")][SerializeField] float _gap;

        public bool IsMove() { return _IsScaleMove; }

        public bool isDefaultStart = true;

        void Awake()
        {
            if (_tapButtons != null)
            {
                _rectTransforms = new RectTransform[_tapButtons.Length];
                for (int i = 0; i < _tapButtons.Length; ++i)
                {
                    if (_tapButtons[i] == null) continue;
                    _rectTransforms[i] = _tapButtons[i].GetComponent<RectTransform>();
                    _tapButtons[i].AddClickListener(OnClickCallback);
                }
            }
        }

        void Start()
        {
            if (_tapButtons != null)
            {
                if(isDefaultStart)
                {
                    if (_tapButtons.Length > DefaultStartIndex)
                    {
                        for (int i = 0; i < _tapButtons.Length; ++i)
                        {
                            if (i == DefaultStartIndex) _tapButtons[i].OnTap();
                            else _tapButtons[i].OffTap();
                        }
                    }
                }
            }

        }

        void OnClickCallback(GameObject obj)
        {
            for(int i = 0; i< _tapButtons.Length; ++i)
            {
                if(_tapButtons[i].gameObject != obj)
                {
                    _tapButtons[i].OffTap();

                }
            }

            if (_IsScaleMove)
            {
                

                int idx = -1;

                if (_tapButtons == null) return;

                var result = from v in _tapButtons orderby v.rectTransform.anchoredPosition.x select v;
                List<TapButton> list = new List<TapButton>();
                foreach (var v in result) list.Add(v);


                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].gameObject == obj)
                    {
                        idx = i;
                        break;
                    }
                }

                if (idx == -1) return;

                float onX = list[idx].rectTransform.sizeDelta.x * _On_Scale.x;
                float offX = list[idx].rectTransform.sizeDelta.x * _Off_Scale.x;
                float center = GetComponent<RectTransform>().anchoredPosition.x;
                float Off_szH =  ((((offX * (list.Count - 1)) + onX)/2) + (_gap * list.Count)) ;

                float hoX = onX /2;
                float hoffX = offX /2;
                
                float x = -Off_szH + center - hoffX;
                


                for (int i = 0; i < list.Count; ++i)
                {
                     Vector2 p = list[i].rectTransform.anchoredPosition;
                     if(i == idx)
                     {
                        x += hoffX + hoX + _gap;
                        p.x = x;
                        // list[i].transform.localScale = _On_Scale;

                     }
                     else
                     {
                        if(i-1 == idx)
                        {
                            x += hoffX + hoX + _gap;;
                            p.x = x;

                        }
                        else
                        {
                             x += offX+ _gap;;
                             p.x = x;

                        }
                        // list[i].transform.localScale = _Off_Scale;

                     }

                     list[i].rectTransform.anchoredPosition = p;
                }

            }

        }
    }
}