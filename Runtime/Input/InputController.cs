using UnityEngine;
using QuickEye.Utility;
using System;
using NaughtyAttributes;
using UnityEngine.EventSystems;


namespace GB
{

    public class InputController : AutoSingleton<InputController>
    {
        [SerializeField] UnityDictionary<string, KeyAttribute> _KeysCodeModels = new UnityDictionary<string, KeyAttribute>();
        public delegate void InputDelegate(string id, TouchPhase phase);
        public event InputDelegate KeyEvent;

        [Header("게임패드")]
        public bool IsDirectionPad;
        [EnableIf("IsDirectionPad")]
        [SerializeField] Transform _DirectionPadParent;

        [EnableIf("IsDirectionPad")]
        [ShowAssetPreview]
        [SerializeField] Sprite _DirectionPadBG;

        [EnableIf("IsDirectionPad")]
        [ShowAssetPreview]
        [SerializeField] Sprite _DirectionPadCtr;

        [EnableIf("IsDirectionPad")]
        [SerializeField] GamePad _GamePad;

        public delegate void DirectionDelegate(Vector2 direction, float percent);
        public event DirectionDelegate DirectionEvent;


        [Header("Wolrd Position")]
        public bool IsWorldMode;
        public delegate void TouchPointDelegate(TouchPhase phase, int touchID, Vector2 position);
        public TouchPointDelegate TouchWorldEvent;

        [Header("UI Position")]
        public bool IsUIMode;
        public TouchPointDelegate TouchUIEvent;




        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            var model = Resources.Load<InputModel>("InputModel");
            IsDirectionPad = model.IsDirectionPad;
            IsWorldMode = model.IsWorldMode;
            IsUIMode = model.IsUIMode;
            _KeysCodeModels = model.KeysCodeModels;

            if (IsDirectionPad)
            {
                _DirectionPadBG = model.DirectionPadBG;
                _DirectionPadCtr = model.DirectionPadCtr;


                var canvas = GB.UI.UIManager.I.Canvas;
                if (canvas != null)
                {
                    var pad = Instantiate(Resources.Load<GameObject>("GamePad"), canvas);
                    _GamePad = pad.GetComponent<GamePad>();
                    _GamePad.GetComponent<RectTransform>().SetSiblingIndex(0);
                    _GamePad.SetBg(_DirectionPadBG);
                    _GamePad.SetCtr(_DirectionPadCtr);
                }
            }

        }

        private void OnEnable()
        {
            CreatePad();
        }

        private void OnDisable()
        {
            KeyEvent = null;
        }

        private void OnDestroy()
        {
            KeyEvent = null;
        }

        void CreatePad()
        {
            if (_GamePad == null)
            {
                var model = Resources.Load<InputModel>("InputModel");
                IsDirectionPad = model.IsDirectionPad;
                IsWorldMode = model.IsWorldMode;
                IsUIMode = model.IsUIMode;
                _KeysCodeModels = model.KeysCodeModels;

                if (IsDirectionPad)
                {
                    _DirectionPadBG = model.DirectionPadBG;
                    _DirectionPadCtr = model.DirectionPadCtr;


                    var canvas =GB.UI.UIManager.I.Canvas;
                    if (canvas != null)
                    {
                        var pad = Instantiate(Resources.Load<GameObject>("GamePad"), canvas);
                        _GamePad = pad.GetComponent<GamePad>();
                        _GamePad.GetComponent<RectTransform>().SetSiblingIndex(0);
                        _GamePad.SetBg(_DirectionPadBG);
                        _GamePad.SetCtr(_DirectionPadCtr);
                    }
                }
            }
        }
        public static void Touch(string id, TouchPhase phase)
        {
            I.KeyEvent?.Invoke(id, phase);
        }

        private void ProcessKeyEvent()
        {
            if (_KeysCodeModels != null)
            {

                foreach (var v in _KeysCodeModels)
                {
                    if (v.Value.Phase == TouchPhase.Began)
                    {
                        if (Input.GetKeyDown(v.Value.Key))
                        {
                            KeyEvent?.Invoke(v.Key, v.Value.Phase);
                        }
                    }
                    else if (v.Value.Phase == TouchPhase.Moved)
                    {
                        if (Input.GetKey(v.Value.Key))
                        {
                            KeyEvent?.Invoke(v.Key, v.Value.Phase);
                        }
                    }
                    else if (v.Value.Phase == TouchPhase.Canceled || v.Value.Phase == TouchPhase.Ended)
                    {
                        if (Input.GetKeyUp(v.Value.Key))
                        {
                            KeyEvent?.Invoke(v.Key, v.Value.Phase);
                        }
                    }
                }
            }
        }

        private void ProcessDirectionEvent()
        {
            if (IsDirectionPad == false) return;

            bool isMove = false;

            Vector2 dir = Vector3.zero;
            float per = 0;

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Y))
            {
                dir.y += 1;
                per = 1;
                isMove = true;
            }
            if (Input.GetKey(KeyCode.DownArrow)|| Input.GetKey(KeyCode.S))
            {
                dir.y -= 1;
                per = 1;
                isMove = true;
            }

            if (Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.D))
            {
                dir.x += 1;
                per = 1;
                isMove = true;
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                dir.x -= 1;
                per = 1;
                isMove = true;
            }

            if (isMove)
                dir = dir.normalized;

            if (_GamePad == null)
                CreatePad();

            if (_GamePad != null)
            _GamePad.Process();

            if (_GamePad != null && _GamePad.IsOnPad)
            {
                var p = _GamePad.GetDirection();
                dir += p;
                per += _GamePad.GetPercent();
                isMove = true;
            }

            if (isMove)
            {
                if (per > 1) per = 1;
                DirectionEvent?.Invoke(dir, per);
            }
        }

        private void ProcessWorld()
        {
            if (IsWorldMode == false) return;
            if (EventSystem.current != null)
            {

                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    return;
                }
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
            }


#if UNITY_EDITOR
            if (EventSystem.current != null)
            {
                if (EventSystem.current.IsPointerOverGameObject() &&
            (Input.GetMouseButton(0)))
                {
                    return;
                }
            }
#endif
            int touchCount = Input.touchCount;

            if (touchCount != 0)
            {
                for (int i = 0; i < touchCount; ++i)
                {
                    Touch touch = Input.GetTouch(i);
                    var p = Camera.main.ScreenToWorldPoint(touch.position);
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            TouchWorldEvent?.Invoke(TouchPhase.Began, i, p);
                            break;
                        case TouchPhase.Moved:
                            TouchWorldEvent?.Invoke(TouchPhase.Moved, i, p);
                            break;
                        case TouchPhase.Ended:
                            TouchWorldEvent?.Invoke(TouchPhase.Ended, i, p);
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (Input.GetMouseButtonDown(i))
                    {
                        var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        TouchWorldEvent?.Invoke(TouchPhase.Began, i, p);
                    }
                    else if (Input.GetMouseButton(i))
                    {
                        var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        TouchWorldEvent?.Invoke(TouchPhase.Moved, i, p);
                    }
                    else if (Input.GetMouseButtonUp(i))
                    {
                        var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        TouchWorldEvent?.Invoke(TouchPhase.Ended, i, p);
                    }
                }

            }

        }

        private void ProcessUI()
        {
            if (IsUIMode == false) return;

            if (EventSystem.current != null)
            {

                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    return;
                }


                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
            }

            if (EventSystem.current != null)
            {
                if (EventSystem.current.IsPointerOverGameObject() &&
            (Input.GetMouseButton(0)))
                {
                    return;
                }
            }



            int touchCount = Input.touchCount;

            if (touchCount != 0)
            {
                for (int i = 0; i < touchCount; ++i)
                {
                    Touch touch = Input.GetTouch(i);
                    var p = touch.position;
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            TouchUIEvent?.Invoke(TouchPhase.Began, i, p);
                            break;
                        case TouchPhase.Moved:
                            TouchUIEvent?.Invoke(TouchPhase.Moved, i, p);
                            break;
                        case TouchPhase.Ended:
                            TouchUIEvent?.Invoke(TouchPhase.Ended, i, p);
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (Input.GetMouseButtonDown(i))
                    {
                        var p = Input.mousePosition;
                        TouchUIEvent?.Invoke(TouchPhase.Began, i, p);
                    }
                    else if (Input.GetMouseButton(i))
                    {
                        var p = Input.mousePosition;
                        TouchUIEvent?.Invoke(TouchPhase.Moved, i, p);
                    }
                    else if (Input.GetMouseButtonUp(i))
                    {
                        var p = Input.mousePosition;
                        TouchUIEvent?.Invoke(TouchPhase.Ended, i, p);
                    }
                }

            }



        }


        private void Update()
        {
            
            ProcessKeyEvent();
            ProcessDirectionEvent();
            ProcessWorld();
            ProcessUI();
        }
    }

    [Serializable]
    public struct KeyAttribute
    {
        public KeyCode Key;
        public TouchPhase Phase;
    }

}