using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickEye.Utility;
using NaughtyAttributes;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace GB
{
    public class ObjectPooling : AutoSingleton<ObjectPooling>
    {


        //프리팹을 가져올 폴더 이름
        [Header("Ex : Resources/PoolingObjects")]
        public string ResourcesFolderName = "PoolingObjects";

        //프리팹 관리
        [SerializeField] UnityDictionary<string, GameObject> _dicPrefabs = new UnityDictionary<string, GameObject>();

        //대기중인 오브젝트 관리
        private Dictionary<string, Stack<GameObject>> _dicObjects = new Dictionary<string, Stack<GameObject>>();

        //타입별 부모 오브젝트 관리
        [SerializeField] UnityDictionary<string, Transform> _listParent = new UnityDictionary<string, Transform>();

        //현재 생성된 모든 풀링 오브젝트
        [SerializeField] List<GameObject> _listObjects = new List<GameObject>();

        //종류별 인덱스 
        private int _type_Index = 0;

        #region Create
        public static GameObject Create(string name)
        {
            GameObject obj = null;

            if (I.checkNewCreate(name))
            {
                obj = I.loadPrefab(name);
                if (obj == null) return null;
            }
            else
            {
                obj = I._dicObjects[name].Pop();
            }

            obj.transform.position = Vector3.zero;
            obj.transform.SetParent(null);
            obj.transform.rotation = Quaternion.identity;

            obj.SetActive(true);

            return obj;
        }

        public static GameObject Create(string name, Transform parent)
        {
            var obj = Create(name);
            obj.transform.SetParent(parent);
            return obj;
        }
        public static GameObject Create(string name, Vector3 position, Quaternion rotation)
        {
            GameObject obj = null;

            if (I.checkNewCreate(name))
            {
                obj = I.loadPrefab(name);
                if (obj == null) return null;
            }
            else
            {
                obj = I._dicObjects[name].Pop();
            }

            obj.transform.position = position;
            obj.transform.SetParent(null);
            obj.transform.rotation = rotation;
            obj.SetActive(true);


            return obj;
        }
        public static GameObject Create(string name, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject obj = null;

            if (I.checkNewCreate(name))
            {
                obj = I.loadPrefab(name);
                if (obj == null) return null;
            }
            else
            {
                obj = I._dicObjects[name].Pop();
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(parent);

            obj.SetActive(true);

            return obj;
        }
        #endregion

        #region Destroy
        /// <summary>
        ///  오브젝트  복귀
        /// </summary>
        /// <param name="returnObject">다시 돌려 보낼 오브젝트</param>
        /// <returns>성공 or 실패</returns>
        public static bool Destroy(GameObject returnObject)
        {

            if (returnObject == null)
            {
                Debug.LogWarning("Destroy : null");
                return false;
            }

            PoolingType type = returnObject.GetComponent<PoolingType>();

            

            if (type == null)
            {
                Debug.LogWarning("Destroy Type : null " + returnObject.gameObject.name);
                return false;
            }
            if (I._listParent.ContainsKey(type.Name) == false)
            {
                    //정리 부모오브젝트 생성
                    GameObject myParent = new GameObject(type.Name);
                    myParent.transform.SetParent(I.transform);
                    I._listParent.Add(type.Name, myParent.transform);
            }

       

            returnObject.transform.SetParent(I._listParent[type.Name]);

            if (!I._dicObjects.ContainsKey(type.Name)) return false;

            if (type.IsActive)
                I._dicObjects[type.Name].Push(returnObject);

            type.IsActive = false;
            returnObject.SetActive(false);

            return true;
        }

        /// <summary>
        /// 모든 오브젝트들 복귀
        /// </summary>
        public static void DestroyAll()
        {
            for (int i = 0; i < I._listObjects.Count; ++i)
            {
                Destroy(I._listObjects[i]);
            }
        }


        /// <summary>
        /// 해당 이름 오브젝트들 복귀
        /// </summary> 
        public static void Destroy(string name)
        {
            for (int i = 0; i < I._listObjects.Count; ++i)
            {
                Destroy(I._listObjects[i]);
            }
        }

        public static void Clear()
        {

            I._dicObjects.Clear();
            I._dicPrefabs.Clear();
            //I._listParent.Clear();

            foreach (var v in I._listObjects)
            {
                if (Application.isPlaying)
                    Object.Destroy(v);
                else
                    DestroyImmediate(v);
            }

            int cnt = I.transform.childCount;

            for (int i = 0; i < cnt; ++i)
            {
                if (I.transform != I.transform.GetChild(0).transform)
                    Object.Destroy(I.transform.GetChild(0).gameObject);
            }

            I._listObjects.Clear();




        }

        #endregion


        /// <summary>
        ///  대기중인  오브젝트 있는지 체크
        /// </summary>
        /// <param name="name">오브젝트 이름</param>
        /// <returns>새로 생성 해야되는지 여부</returns>
        private bool checkNewCreate(string name)
        {

            // 생성한 오브젝트 키가 있는가 
            if (_dicObjects.ContainsKey(name))
            {
                //해당키의 가용 가능한 오브젝트가 있는가
                if (_dicObjects[name].Count > 0) return false;
                else return true;
            }
            else
            {
                return true;
            }
        }


        private GameObject loadPrefab(string name)
        {

            GameObject prefab;
            GameObject obj;

            if (!_dicPrefabs.ContainsKey(name))
            {
                prefab = Resources.Load(ResourcesFolderName + "/" + name) as GameObject;

                if (prefab == null)
                {
                    Debug.LogWarning("None PoolingObject - " + "Checking To Folder And Object \n" + "Resources/" + ResourcesFolderName + "/" + name);
                    return null;
                }
                _dicPrefabs.Add(name, prefab);
            }
            else
            {
                prefab = _dicPrefabs[name];
            }

            //문제 없으면 생성
            obj = Instantiate(prefab);

            //생성된 모든 오브젝트 담기
            _listObjects.Add(obj);



            //키가 있는지 찾기 없으면 생성
            if (!_dicObjects.ContainsKey(name))
            {
                //해당 키 생성
                _dicObjects.Add(name, new Stack<GameObject>());

                if (_listParent.ContainsKey(name) == false)
                {
                    //정리 부모오브젝트 생성
                    GameObject myParent = new GameObject(name);
                    myParent.transform.SetParent(transform);
                    _listParent.Add(name, myParent.transform);
                }
            }

            _type_Index = _listParent.Count;

            if (obj.GetComponent<PoolingType>() == null)
                obj.AddComponent<PoolingType>();


            obj.GetComponent<PoolingType>().Name = name;
            obj.GetComponent<PoolingType>().IsActive = true;

            return obj;
        }

        [Button]
        private void CreateFolder()
        {
#if UNITY_EDITOR
            string path = Application.dataPath + "/Resources/" + ResourcesFolderName;

            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists == false)
                info.Create();

            AssetDatabase.Refresh();
#endif
        }

    }
}