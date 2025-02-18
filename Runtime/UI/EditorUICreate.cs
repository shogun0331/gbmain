using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;





#if UNITY_EDITOR
using UnityEditor;
using GB.Edit;

namespace GB
{
    [CustomEditor(typeof(UICreate))]
    public class EditorUICreate : Editor
    {
        void OnEnable()
        {
            var t = (UICreate)target;
            _selectionMenu = "Buttons";
            GetLoadList(t.gameObject);

        }



        string _selectionMenu = "Buttons";
        private GameObject myFileObject;

        List<GameObject> _list = new List<GameObject>();

        List<string> _listKey = new List<string>();


        

        void RegistComponenet(GameObject myTarget, string key)
        {
            switch (_selectionMenu)
            {
                case "Buttons":
                    myTarget.AddComponent<RegistButton>().Key = key;
                    break;

                case "Images":
                    myTarget.AddComponent<RegistImage>().Key = key;
                    break;

                case "Texts":
                    myTarget.AddComponent<RegistText>().Key = key;
                    break;

                case "Skinners":
                    myTarget.AddComponent<RegistSkinner>().Key = key;
                    break;

                case "GameObjects":
                    myTarget.AddComponent<RegistGameObject>().Key = key;
                    break;

            }
        }



        void GetLoadList(GameObject myTarget)
        {
            _list.Clear();
            _listKey.Clear();
            

            switch (_selectionMenu)
            {
                case "Buttons":
                    var btns = myTarget.GetComponentsInChildren<Button>(true);
                    for (int i = 0; i < btns.Length; ++i) _list.Add(btns[i].gameObject);
                    break;

                case "Images":
                    var imgs = myTarget.GetComponentsInChildren<Image>(true);
                    for (int i = 0; i < imgs.Length; ++i) _list.Add(imgs[i].gameObject);
                    break;

                case "Texts":
                    var texts = myTarget.GetComponentsInChildren<Text>(true);
                    for (int i = 0; i < texts.Length; ++i) _list.Add(texts[i].gameObject);
                    break;

                case "Skinners":
                    var skinners = myTarget.GetComponentsInChildren<UISkinner>(true);
                    for (int i = 0; i < skinners.Length; ++i) _list.Add(skinners[i].gameObject);
                    break;

                case "GameObjects":
                    var components = myTarget.GetComponentsInChildren<Component>(true);
                    HashSet<GameObject> hashSet = new HashSet<GameObject>();

                    for (int i = 0; i < components.Length; ++i) hashSet.Add(components[i].gameObject);
                    _list = hashSet.ToList();
                    break;

            }

            for (int i = 0; i < _list.Count; ++i)
            {
                if (CheckScript(_list[i])) _listKey.Add(GetKey(_list[i]));
                else _listKey.Add("");
            }


        }

        bool CheckScript(GameObject myTarget)
        {

            switch (_selectionMenu)
            {
                case "Buttons":
                    return myTarget.GetComponent<RegistButton>();

                case "Images":
                    return myTarget.GetComponent<RegistImage>();

                case "Texts":
                    return myTarget.GetComponent<RegistText>();

                case "Skinners":
                    return myTarget.GetComponent<RegistSkinner>();

                case "GameObjects":
                    return myTarget.GetComponent<RegistGameObject>();

            }
            return false;
        }

        string GetKey(GameObject myTarget)
        {

            switch (_selectionMenu)
            {
                case "Buttons":
                    return myTarget.GetComponent<RegistButton>().Key;

                case "Images":
                    return myTarget.GetComponent<RegistImage>().Key;

                case "Texts":
                    return myTarget.GetComponent<RegistText>().Key;

                case "Skinners":
                    return myTarget.GetComponent<RegistSkinner>().Key;

                case "GameObjects":
                    return myTarget.GetComponent<RegistGameObject>().Key;

            }

            return "";
        }

        void ChangeKey(GameObject myTarget,string key)
        {

            switch (_selectionMenu)
            {
                case "Buttons":
                    myTarget.GetComponent<RegistButton>().Key = key;
                    break;
                    

                case "Images":
                    myTarget.GetComponent<RegistImage>().Key = key;
                    break;

                case "Texts":
                    myTarget.GetComponent<RegistText>().Key = key;
                    break;

                case "Skinners":
                    myTarget.GetComponent<RegistSkinner>().Key= key;
                    break;

                case "GameObjects":
                    myTarget.GetComponent<RegistGameObject>().Key = key;
                    break;

            }

        }


        Vector2 _scrollPos;




        public override void OnInspectorGUI()
        {
            var t = (UICreate)target;
            if (t.GetComponent<UIScreen>() == null)
                GB.EditorGUIUtil.DrawHeaderLabel("UICreate");
            else
                GB.EditorGUIUtil.DrawHeaderLabel(t.UIName);

            if (t.GetComponent<UIScreen>() == null)
            {
                base.OnInspectorGUI();

                if (GB.EditorGUIUtil.DrawSyleButton("Create Script"))
                {
                    if (!string.IsNullOrEmpty(t.UIName))
                        t.Generate();
                }

                if (GB.EditorGUIUtil.DrawSyleButton("AddComponent"))
                {
                    if (!string.IsNullOrEmpty(t.UIName))
                    {
                        t.Setting(true);
                    }
                }

                return;
            }

            //     myFileObject = t.gameObject;
            //   myFileObject = EditorGUILayout.ObjectField("", myFileObject, typeof(GameObject),false) as GameObject;

            GB.EditorGUIUtil.Start_VerticalBox();

            GB.EditorGUIUtil.Start_Horizontal();

            if (string.Equals(_selectionMenu, "Buttons")) GB.EditorGUIUtil.BackgroundColor(Color.blue);


            if (GB.EditorGUIUtil.DrawSyleButton("Buttons"))
            {
                _selectionMenu = "Buttons";
                GetLoadList(t.gameObject);

            }

            GB.EditorGUIUtil.BackgroundColor(Color.white);


            if (string.Equals(_selectionMenu, "Images")) GB.EditorGUIUtil.BackgroundColor(Color.blue);

            if (GB.EditorGUIUtil.DrawSyleButton("Images"))
            {
                _selectionMenu = "Images";
                GetLoadList(t.gameObject);

            }
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            if (string.Equals(_selectionMenu, "Texts")) GB.EditorGUIUtil.BackgroundColor(Color.blue);

            if (GB.EditorGUIUtil.DrawSyleButton("Texts"))
            {
                _selectionMenu = "Texts";
                GetLoadList(t.gameObject);
            }

            GB.EditorGUIUtil.BackgroundColor(Color.white);

            if (string.Equals(_selectionMenu, "Skinners")) GB.EditorGUIUtil.BackgroundColor(Color.blue);

            if (GB.EditorGUIUtil.DrawSyleButton("Skinners"))
            {
                _selectionMenu = "Skinners";
                GetLoadList(t.gameObject);
            }
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            if (string.Equals(_selectionMenu, "GameObjects")) GB.EditorGUIUtil.BackgroundColor(Color.blue);

            if (GB.EditorGUIUtil.DrawSyleButton("GameObjects"))
            {
                _selectionMenu = "GameObjects";
                GetLoadList(t.gameObject);

            }

            GB.EditorGUIUtil.BackgroundColor(Color.white);






            GB.EditorGUIUtil.End_Horizontal();


            _scrollPos = GB.EditorGUIUtil.Start_ScrollView(_scrollPos);
            GB.EditorGUIUtil.BackgroundColor(Color.gray);
            GB.EditorGUIUtil.Start_HorizontalBox();
            EditorGUILayout.LabelField("Key", GUILayout.Width(150f));
            EditorGUILayout.LabelField("Object");
            GB.EditorGUIUtil.End_Horizontal();
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            HashSet<string> hashKeys = new HashSet<string>();
            for (int i = 0; i < _list.Count; ++i)
            {
                if(!CheckScript(_list[i])) continue;
                bool isDuplicateKey = hashKeys.Contains(_listKey[i]);

                if(!hashKeys.Contains(_listKey[i]) || string.IsNullOrEmpty(_listKey[i]))hashKeys.Add(_listKey[i]);
                else GB.EditorGUIUtil.BackgroundColor(Color.red);

                GB.EditorGUIUtil.Start_HorizontalBox();
                
                _listKey[i] = GB.EditorGUIUtil.DrawTextField("", _listKey[i], GUILayout.Width(150f));
                 ChangeKey(_list[i], _listKey[i]);
                _list[i] = EditorGUILayout.ObjectField("", _list[i], typeof(GameObject), false) as GameObject;

                // if (!CheckScript(_list[i]))
                // {
                //     if (GB.EditorGUIUtil.DrawButton("Regist"))
                //     {
                //         if (!string.IsNullOrEmpty(_listKey[i]))
                //         {
                //             RegistComponenet(_list[i], _listKey[i]);
                //             GetLoadList(t.gameObject);
                //         }
                //     }
                // }
                // else
                // {
                //     if (GB.EditorGUIUtil.DrawButton("Change Key"))
                //     {
                //         if (!string.IsNullOrEmpty(_listKey[i]))
                //         {
                //             ChangeKey(_list[i],_listKey[i]);
                //             GetLoadList(t.gameObject);
                //         }
                //     }

                // }




                GB.EditorGUIUtil.End_Horizontal();
                GB.EditorGUIUtil.BackgroundColor(Color.white);
            }


            GB.EditorGUIUtil.End_ScrollView();




            GB.EditorGUIUtil.End_Vertical();
            GB.EditorGUIUtil.BackgroundColor(Color.green);
            if (GB.EditorGUIUtil.DrawSyleButton("Save"))
            {
                t.Bind();
                t.Save();
                t.Setting(true);
            }
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            GB.EditorGUIUtil.Start_Horizontal();


            if (GB.EditorGUIUtil.DrawSyleButton("Open Script"))
            {
                string scriptPath = "Assets/Scripts/UI/" + t.UIName + ".cs"; // 열고 싶은 스크립트의 경로
                UnityEngine.Object scriptAsset = AssetDatabase.LoadAssetAtPath(scriptPath, typeof(MonoScript));
                if (scriptAsset != null)
                    AssetDatabase.OpenAsset(scriptAsset);
                else
                    Debug.LogWarning($"Script not found at path: {scriptPath}");

            }

            if (GB.EditorGUIUtil.DrawSyleButton("Quick Selection"))
            {
                t.QuickSelect();
            }

            GB.EditorGUIUtil.End_Horizontal();

        }
    }
}
#endif