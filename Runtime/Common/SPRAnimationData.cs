using System;
using System.Collections.Generic;
using NaughtyAttributes;
using QuickEye.Utility;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace GB
{
    [CreateAssetMenu(fileName = "SPRAnimation", menuName = "GB/SPRAnimation", order = 1)]
    public class SPRAnimationData : ScriptableObject
    {
        [BoxGroup("Resources")]
        //[ShowNonSerializedField]
        [SerializeField] bool _isAtlas;

        public bool IsAtlas{get{return _isAtlas;}}

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
        public void LoadSprite()
        {
            _isAtlas = false;
            _atlas = null;
            _sprNames = null;

            string path = Application.dataPath + "/" + _path;

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            if (di.Exists == false)
            {
                _sprites = null;
                Debug.Log("<color=red>Directory - None Failed</color>");
                return;
            }

            List<string> fileList = new List<string>();
            
            foreach (System.IO.FileInfo File in di.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".png") == 0)
                {
                    string FullFileName = File.FullName;
                    int len = FullFileName.Length - Application.dataPath.Length;
                    fileList.Add("Assets" + FullFileName.Substring(Application.dataPath.Length, len));
                }
            }
            List<Sprite> sprList = new List<Sprite>();

            for (int i = 0; i < fileList.Count; ++i)
            {
                UnityEngine.Object[] data = AssetDatabase.LoadAllAssetsAtPath(fileList[i]);
                foreach (UnityEngine.Object v in data)
                {
                    if (v.GetType() == typeof(Sprite))
                    {
                        sprList.Add((Sprite)v);
                    }
                }
            }

            _sprites = sprList.ToArray();

            if (_sprites.Length > 0)
                Debug.Log("<color=green>LoadSprite - Success</color>");
            else
                Debug.Log("<color=red>LoadSprite - Failed</color>");
        }

        [BoxGroup("EDIT")]
        [SerializeField] string _atlasPath;

        [Button]
        public void LoadAtlas()
        {
            _isAtlas = true;
            _sprites = null;

            string path = Application.dataPath + "/" + _path;

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            if (di.Exists == false)
            {
                Debug.Log("<color=red>Directory - None Failed</color>");
                return;
            }

            List<string> fileList = new List<string>();
            
            foreach (System.IO.FileInfo File in di.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".png") == 0)
                {
                    string FullFileName = File.FullName;
                    int len = FullFileName.Length - Application.dataPath.Length;
                    fileList.Add("Assets" + FullFileName.Substring(Application.dataPath.Length, len));
                }
            }
            _sprNames = new List<string>();

            for (int i = 0; i < fileList.Count; ++i)
            {
                UnityEngine.Object[] data = AssetDatabase.LoadAllAssetsAtPath(fileList[i]);
                foreach (UnityEngine.Object v in data)
                {
                    if (v.GetType() == typeof(Sprite))
                    {
                        Sprite spr =(Sprite)v; 
                        _sprNames.Add(spr.name);
                    }
                }
            }

            if (_sprNames.Count <= 0)
            {
                Debug.Log("<color=red>LoadAtlas Failed : null SprList</color>");
                return;
            }

            path = Application.dataPath + "/" + _atlasPath + ".spriteatlasv2";
            FileInfo fi = new FileInfo(path);
            if(fi.Exists == false)
            {
                Debug.Log("<color=red>Atlas File - None Failed</color>");
                return;
            }
             string fullFileName = fi.FullName;
             int fileLen = fullFileName.Length - Application.dataPath.Length;
             string fileName = "Assets" + fullFileName.Substring(Application.dataPath.Length, fileLen);

             _atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(fileName);

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