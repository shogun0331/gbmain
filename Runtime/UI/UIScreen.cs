using System.Collections.Generic;
using Aya.Tween;
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
        [SerializeField] protected UnityDictionary<string, Transform> mTransforms = new UnityDictionary<string, Transform>();
        [SerializeField] protected UnityDictionary<string, GameObject> mGameObject = new UnityDictionary<string, GameObject>();
        [SerializeField] protected UnityDictionary<string, LocalizationView> mLocalization = new UnityDictionary<string, LocalizationView>();
        [SerializeField] protected UnityDictionary<string, RectTransform> mRectTransform = new UnityDictionary<string, RectTransform>();
        [SerializeField] protected UnityDictionary<string, UISkinner> mSkinner = new UnityDictionary<string, UISkinner>();
        [SerializeField] protected UnityDictionary<string, AnimationClip> mAnim = new UnityDictionary<string, AnimationClip>();
        [SerializeField] protected UnityDictionary<string, ParticleSystem> mParticle = new UnityDictionary<string, ParticleSystem>();


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
        public void PlayParticle(string name) { if (mParticle.ContainsKey(name)) mParticle[name].Play(); }
        public void ClearAnim() { mAnim.Clear(); }
        public void Add(string key, AnimationClip anim) { mAnim.Add(key, anim); }
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
        public void Add(string key, Transform tr)
        {
            if (mTransforms.ContainsKey(key))
            {
                Debug.LogError("SameKey : " + tr.gameObject.name + "-" + mTransforms[key].gameObject.name);
                return;
            }

            mTransforms.Add(key, tr);
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
        public void Add(string key, LocalizationView locailView)
        {
            if (mLocalization.ContainsKey(key))
            {
                Debug.LogError("SameKey : " + locailView.gameObject.name + "-" + mLocalization[key].gameObject.name);
                return;
            }
            mLocalization.Add(key, locailView);
        }
        public void Add(string key, RectTransform rt)
        {
            if (mRectTransform.ContainsKey(key))
            {
                Debug.LogError("SameKey : " + rt.gameObject.name + "-" + mRectTransform[key].gameObject.name);
                return;
            }
            mRectTransform.Add(key, rt);
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

        public void PlayAnimation(string key)
        {
            if (mAnim.ContainsKey(key))
            {
                var anim = GetComponent<Animation>();
                if (anim != null)
                {
                    anim.clip = mAnim[key];
                    anim.Play();
                }
            }
        }

        void Clear()
        {
            mGameObject.Clear();
            mButtons.Clear();
            mTransforms.Clear();
            mImages.Clear();
            mTexts.Clear();
            mSkinner.Clear();
            mLocalization.Clear();
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

        private void Awake()
        {
            UIManager.I.RegistUIScreen(this);
        }

        public virtual void Initialize() { }

        public virtual void Refresh() { }
        public virtual void SetData(IModel data) { }
        public virtual void BackKey() { if (UIType == ScreenType.POPUP) Close(); }
        public virtual void Close() { UIManager.ClosePopup(this); }
        public override void ViewQuick(string key, IModel data) { }
    }

    public interface IScreen
    {
        public ScreenType UIType { get; }

        public void Refresh();
        public void SetData(IModel data);
        void BackKey();

        void Close();

    }
}
