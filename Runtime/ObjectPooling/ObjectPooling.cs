
using UnityEngine;
using QuickEye.Utility;




namespace GB
{
    public class ObjectPooling : AutoSingleton<ObjectPooling>
    {


        //프리팹을 가져올 폴더 이름
        [Header("Ex : Resources/PoolingObjects")]
        public string ResourcesFolderName = "PoolingObjects";

        [SerializeField] UnityDictionary<string,GameObjectPool<PoolingType>> _dictPooling;


        bool _isInit;
        void Init()
        {
            if(_isInit) return;

            _dictPooling = new UnityDictionary<string, GameObjectPool<PoolingType>>();
            _isInit = true;
        }

        bool Check(string name)
        {
            string path = ResourcesFolderName+"/"+name;
            var obj  = Resources.Load<GameObject>(path);
            if(obj == null) return false;

             var t = obj.GetComponent<PoolingType>();
            if(t == null) t = obj.AddComponent<PoolingType>();
                _dictPooling[name] = new GameObjectPool<PoolingType>(transform,t,10);

            return true;
        }


        public static GameObject Create(string name)
        {
            I.Init();
            if(!I._dictPooling.ContainsKey(name)) 
            {
                if(I.Check(name) == false)
                return null;
            }
            var o = I._dictPooling[name];
            var g = o.Rent();
            g.Name = name;
            g.IsActive = true;
            g.transform.SetParent(null);
            g.transform.position = o.Original.transform.position;
            g.transform.rotation = o.Original.transform.rotation;
            g.transform.localScale = o.Original.transform.localScale;
            g.gameObject.SetActive(true);

            return g.gameObject;
        }

        public static void Destroy(GameObject obj)
        {
            I.Init();
            var poolType = obj.GetComponent<PoolingType>();
            if(poolType == null) return;
            if(I._dictPooling.ContainsKey(poolType.Name) == false) return;

            var o = I._dictPooling[poolType.Name];
            o.Return(poolType);
        }

        public static void Clear()
        {
            foreach(var v in I._dictPooling)
            {
                v.Value.ReturnAll();
            }
        }

        public static void Clear(string name)
        {
            if(!I._dictPooling.ContainsKey(name)) return;

            I._dictPooling[name].ReturnAll();

        }
    }
}