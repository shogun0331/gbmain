using System;
using System.Collections.Generic;
using NaughtyAttributes;
using QuickEye.Utility;
using UnityEngine;
using UnityEngine.U2D;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GB
{
    [CreateAssetMenu(fileName = "SPRAnimation", menuName = "GB/SPRAnimation", order = 1)]
    public class SPRAnimationData : ScriptableObject
    {
        [BoxGroup("Resources")]
        [ShowNonSerializedField]
        bool _isAtlas;

        [BoxGroup("Resources")]
        [HorizontalLine(color: EColor.Red)]
        [SerializeField] List<string> _sprNames;

        [BoxGroup("Resources")]
        [SerializeField] SpriteAtlas _atlas;
        [BoxGroup("Resources")]
        [ShowAssetPreview]
        [SerializeField] Sprite[] _sprites;
        [BoxGroup("Resources")]
        public float Speed = 1;
        [BoxGroup("Resources")]
        public bool IsLoop = false;
        [BoxGroup("Resources")]
        public UnityDictionary<int, List<TriggerData>> Triggers;


        public int SpriteCount
        {
            get
            {
                if (_atlas != null && _sprNames.Count > 0) return _sprNames.Count;
                else if (_sprites != null) return _sprites.Length;
                else return 0;
            }
        }

        public Sprite GetSprite(int index)
        {
            if (_atlas != null && _sprNames.Count > 0 && index < _sprNames.Count)
                return _atlas.GetSprite(_sprNames[index]);
            else if (_sprites != null && index < _sprites.Length)
                return _sprites[index];
            else
                return null;
        }

#if UNITY_EDITOR
        [BoxGroup("EDIT")]
        [HorizontalLine(color: EColor.Blue)]
        [SerializeField] string _path;

        [Button]
        private void LoadSprite()
        {
            _isAtlas = false;
            _atlas = null;
            _sprNames = null;

            var arrSpr = Resources.LoadAll<Sprite>(_path);
            _sprites = arrSpr;

            if (_sprites.Length > 0)
                Debug.Log("<color=green>LoadSprite - Success</color>");
            else
                Debug.Log("<color=red>LoadSprite - Failed</color>");
        }

        [BoxGroup("EDIT")]
        [SerializeField] string _atlasPath;

        [Button]
        private void LoadAtlas()
        {
            _isAtlas = true;
            _atlas = Resources.Load<SpriteAtlas>(_atlasPath);
            var arrSpr = Resources.LoadAll<Sprite>(_path);
            if (arrSpr == null || arrSpr.Length <= 0 || _atlas == null)
            {
                if (_atlas == null)
                    Debug.Log("<color=red>LoadAtlas Failed : null atlas</color>");
                if (arrSpr == null || arrSpr.Length <= 0)
                    Debug.Log("<color=red>LoadAtlas Failed : null arrSpr</color>");
                return;
            }


            _sprNames = new List<string>();
            _sprites = null;

            for (int i = 0; i < arrSpr.Length; ++i)
            {
                _sprNames.Add(arrSpr[i].name);
            }
            Debug.Log("<color=green>LoadAtlas - Success</color>");
        }
#endif
    }

    [Serializable]
    public class TriggerData
    {
        public string Str;
        public Vector3 Vec3;
        public float Float;
        public int Int;
    }
}