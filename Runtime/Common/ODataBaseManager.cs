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
        Dictionary<string, List<View>> _dicView = new Dictionary<string, List<View>>();

        #if UNITY_EDITOR
        public Dictionary<string,Type> DictDataType = new Dictionary<string, Type>();
        #endif

        public static void Bind(string key, View view)
        {
            if (I._dicView.ContainsKey(key))
            {
                if (I._dicView[key] != null)
                    I._dicView[key].Add(view);
                else
                    I._dicView[key] = new List<View> { view };
            }
            else
            {
                List<View> viewList = new List<View> { view };
                I._dicView.Add(key, viewList);
            }
        }

        public static void UnBind(string key, View view)
        {
            if (I._dicView.ContainsKey(key) == false) return;
            I._dicView[key].Remove(view);
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
            I.OnCall(key);
        }

        void OnCall(string key)
        {
            if (I._dicView.ContainsKey(key))
            {
                List<View> viewList = I._dicView[key];
                List<View> nullViewList = new List<View>();

                for (int i = 0; i < viewList.Count; ++i)
                {
                    if (viewList[i] != null)
                        viewList[i].ViewQuick(key, I._dictDatas[key]);
                    else
                        nullViewList.Add(viewList[i]);
                }

                for (int i = 0; i < nullViewList.Count; ++i)
                    viewList.Remove(nullViewList[i]);
            }
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
            I._dicView.Clear();
        }

        public static bool Contains(string key)
        {
            return I._dictDatas.ContainsKey(key);
        }


    }

}
