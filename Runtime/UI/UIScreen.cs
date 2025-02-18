using System.Collections.Generic;
using QuickEye.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace GB
{
    public enum ScreenType { SCENE = 0, POPUP };
    public class UIScreen : View, IScreen
    {
        [SerializeField] ScreenType _UIType = ScreenType.POPUP;
        public ScreenType UIType { get => _UIType; }

        [SerializeField] protected UnityDictionary<string, Image> mImages = new UnityDictionary<string, Image>();
        [SerializeField] protected UnityDictionary<string, Text> mTexts = new UnityDictionary<string, Text>();
        [SerializeField] protected UnityDictionary<string, Button> mButtons = new UnityDictionary<string, Button>();
        [SerializeField] protected UnityDictionary<string, GameObject> mGameObject = new UnityDictionary<string, GameObject>();
        [SerializeField] protected UnityDictionary<string, UISkinner> mSkinner = new UnityDictionary<string, UISkinner>();


        public void SetScreenType(ScreenType type)
        {
            _UIType = type;
        }



        public void SetBind()
        {
            Clear();

            UIRegister[] allChildren = GetComponentsInChildren<UIRegister>(true);
            for (int i = 0; i < allChildren.Length; ++i)
                allChildren[i].SetBind();
        }

        public virtual void OnAnimationEvent(string value) { }
        public void Add(string key, Text text)
        {
            if (mTexts.ContainsKey(key))
            {
                Debug.LogError("SameKey : " + text.gameObject.name + "-" + mTexts[key].gameObject.name);
                return;
            }

            mTexts.Add(key, text);
        }
        public void Add(string key, Image img)
        {
            if (mImages.ContainsKey(key))
            {
                Debug.LogError("SameKey : " + img.gameObject.name + "-" + mImages[key].gameObject.name);
                return;
            }

            mImages.Add(key, img);
        }
        public void Add(string key, Button btn)
        {
            if (mButtons.ContainsKey(key))
            {
                Debug.LogError("SameKey : " + btn.gameObject.name + "-" + mButtons[key].gameObject.name);
                return;
            }

            mButtons.Add(key, btn);
        }
       
        public void Add(string key, GameObject oj)
        {
            if (mGameObject.ContainsKey(key))
            {
                Debug.LogError("SameKey : " + oj.gameObject.name + "-" + mGameObject[key].gameObject.name);
                return;
            }

            mGameObject.Add(key, oj);
        }
      
        public void Add(string key, UISkinner sk)
        {
            if (mSkinner.ContainsKey(key))
            {
                Debug.LogError("SameKey : " + sk.gameObject.name + "-" + mSkinner[key].gameObject.name);
                return;
            }
            mSkinner.Add(key, sk);
        }

     
        void Clear()
        {
            mGameObject.Clear();
            mButtons.Clear();
            mImages.Clear();
            mTexts.Clear();
            mSkinner.Clear();
        }



        public int Weight
        {
            get
            {
                return GetComponent<RectTransform>().GetSiblingIndex();
            }
            set
            {
                GetComponent<RectTransform>().SetSiblingIndex(value);
            }
        }

        public void Regist()
        {
            UIManager.I.RegistUIScreen(this);
        }

        public virtual void Initialize() { }

        public virtual void Refresh() { }
        public virtual void SetData(object data) { }
        public virtual void BackKey() { if (UIType == ScreenType.POPUP) Close(); }
        public virtual void Close() { UIManager.ClosePopup(this); }
        public override void ViewQuick(string key, IOData data) { }
    }

    public interface IScreen
    {
        public ScreenType UIType { get; }

        public void Refresh();
        public void SetData(object data);
        void BackKey();

        void Close();

    }
}
