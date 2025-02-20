using System;
using System.Collections.Generic;

namespace GB
{
    public class ODataBaseManager : AutoSingleton<ODataBaseManager>
    {
        private void Awake()
        {
            if (I != null && I != this)
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);

        }

        Dictionary<string, IOData> _dictDatas = new Dictionary<string, IOData>();
        
        #if UNITY_EDITOR
        public Dictionary<string,Type> DictDataType = new Dictionary<string, Type>();
        #endif

        public static void Bind(string key, IView view)
        {
            Presenter.Bind(key,view);
        }

        public static void UnBind(string key, IView view)
        {
            Presenter.UnBind(key,view);
        }

        public static T Get<T>(string key)
        {
            return ODataConverter.Convert<T>(I._dictDatas[key]);
        }

        public static void Set<T>(string key, T data)
        {
            #if UNITY_EDITOR
            I.DictDataType[key] = typeof(T);
            #endif

            I._dictDatas[key] = new OData<T>(data);
            Presenter.Send(key,key,data);
            
        }


        public static void Remove(string key)
        {
            if (I._dictDatas.ContainsKey(key)) I._dictDatas.Remove(key);

            #if UNITY_EDITOR
            I.DictDataType.Remove(key);
            #endif
        }

        public static void Clear()
        {
            #if UNITY_EDITOR
            I.DictDataType.Clear();
            #endif

            I._dictDatas.Clear();
        }

        public static bool Contains(string key)
        {
            return I._dictDatas.ContainsKey(key);
        }


    }

}
