using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif



namespace GB
{
    public class UICreate : MonoBehaviour
    {
#if UNITY_EDITOR
        public string UIName;

        const string EDITOR_SAVECS_PATH = "$PATH$/$FILENAME$";

        [Button]
        public void Bind()
        {
            GetComponent<UIScreen>().SetBind();
        }


        [Button]
        public void Generate()
        {
            if (GetComponent<UIScreen>() != null) return;
            if (string.IsNullOrEmpty(UIName)) return;

            string savePath = Application.dataPath+"/Scripts/UI";
           
            DirectoryInfo info = new DirectoryInfo(EDITOR_SAVECS_PATH.Replace("$PATH$", savePath).Replace("$FILENAME$", ""));
            if (info.Exists == false)
                info.Create();

            info = new DirectoryInfo(Application.dataPath + "/Resources/UI/Scene");
            if (info.Exists == false)
                info.Create();
            info = new DirectoryInfo(Application.dataPath + "/Resources/UI/Popup");
            if (info.Exists == false)
                info.Create();

            info = new DirectoryInfo(Application.dataPath + "/Resources/Sounds/BG");
            if (info.Exists == false)
                info.Create();

            info = new DirectoryInfo(Application.dataPath + "/Resources/Sounds/Effect");
            if (info.Exists == false)
                info.Create();

            info = new DirectoryInfo(Application.dataPath + "/Resources/PoolingObjects");
            if (info.Exists == false)
                info.Create();


                

            string text = Resources.Load<TextAsset>("UIText").text;
            text = text.Replace("$POPUPNAME$", UIName);

            info = new DirectoryInfo(EDITOR_SAVECS_PATH.Replace("$PATH$", savePath).Replace("$FILENAME$", "UIPopup"));
            if (info.Exists == false)
                info.Create();

            WriteTxt(EDITOR_SAVECS_PATH.Replace("$PATH$", savePath).Replace("$FILENAME$", UIName + ".cs"), text);

            AssetDatabase.Refresh();

        }

        // [Button]
        // public void Setting()
        // {
        //     if (GetComponent<UIScreen>() != null) return;
        //     if (string.IsNullOrEmpty(UIName)) return;

        //     System.Type componentType = System.Type.GetType(UIName);
        //     Component component = gameObject.AddComponent(componentType);
        // }


        void WriteTxt(string filePath, string message)
        {
            File.WriteAllText(filePath, message);

        }

        string ReadTxt(string filePath)
        {
            return File.ReadAllText(filePath);
        }

#endif

    }

}

