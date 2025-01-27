#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using QuickEye.Utility;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
namespace GB
{
    public class EditorMemo : EditorWindow
    {
        [MenuItem("GB/Core/Memo")]
        static void init()
        {
            EditorWindow.GetWindow(typeof(EditorMemo));

        }
        void OnEnable()
        {
            Load();


        }

        private Memo[] _SceneMemos;
        private List<GameObject> _MemoPrefabs;
        private List<int> _ListMemoCount;

        void Load()
        {
            _SceneMemos = GameObject.FindObjectsOfType<Memo>();
            FindAllPrefabs();

        }

        void FindAllPrefabs()
        {
            _MemoPrefabs = new List<GameObject>();
            _ListMemoCount = new List<int>();

            string[] prefabGUIDs = AssetDatabase.FindAssets("t:prefab"); // 모든 프리팹의 GUID를 찾음

            List<GameObject> prefabs = new List<GameObject>();
            foreach (string prefabGUID in prefabGUIDs)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
                // string prefabPath = AssetDatabase.GUIDToPath(prefabGUID); // GUID로부터 경로를 얻음
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath); // 경로로부터 프리팹을 로드
                if (prefab != null)
                    prefabs.Add(prefab);
                
            }

            for(int i = 0; i<prefabs.Count; ++i )
            {
                 var memos = prefabs[i].GetComponentsInChildren<Memo>(true);
                 if(memos != null && memos.Length > 0)
                 {
                    _MemoPrefabs.Add(prefabs[i]);
                    _ListMemoCount.Add(memos.Length);
                 }

            }

        }


        private void OnFocus()
        {
            Load();
        }

        Vector2 _SceneScrollPosition;
        Vector2 _PrefabScrollPosition;

        private void OnGUI()
        {

            var customHeaderStyle = CustomHeaderStyle();
            var customSectionStyle = CustomSectionStyle();
            var buttonStyle = ButtonSyle();

            GUILayout.BeginArea(new Rect(10, 20, position.width - 20, position.height - 20));

            GUILayout.BeginVertical("box");
            GUILayout.Label("GB MEMO", customHeaderStyle);
            GUILayout.EndVertical();

            GUILayout.Space(20);
            GUI.backgroundColor = new Color(0.2f, 0.2f, 0.8f, 1.0f);
            GUILayout.BeginVertical("box");
            GUILayout.Label("SCENE MEMO", customSectionStyle);
            GUILayout.EndVertical();
            GUI.backgroundColor = Color.white;

            GUILayout.BeginHorizontal("box");
            GUI.backgroundColor = new Color(0f, 0f, 0f, 1.0f);
            EditorGUILayout.LabelField("GameObject", GUILayout.Width(150f));
            EditorGUILayout.LabelField("Memo", GUILayout.Width(300f));
            EditorGUILayout.LabelField("Link", GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            bool isNullMemo = false;
            if (_SceneMemos != null)
            {
                GUILayout.BeginVertical("box");

                _SceneScrollPosition = GUILayout.BeginScrollView(_SceneScrollPosition);


                for (int i = 0; i < _SceneMemos.Length; ++i)
                {
                    if (_SceneMemos[i] == null)
                    {
                        isNullMemo = true;
                        continue;
                    }

                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(_SceneMemos[i].gameObject.name, GUILayout.Width(150f));
                    EditorGUILayout.LabelField(_SceneMemos[i].Content, GUILayout.Width(300f));
                    GUI.backgroundColor = new Color(0f, 0f, 1f, 1f);
                    if (GUILayout.Button("Link", GUILayout.Width(100)))
                    {
                        Selection.activeGameObject = _SceneMemos[i].gameObject;
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndHorizontal();

                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();

            }



            GUILayout.Space(20);
            GUI.backgroundColor = new Color(0.2f, 0.2f, 0.8f, 1.0f);
            GUILayout.BeginVertical("box");
            GUILayout.Label("-TODO Prefab", customSectionStyle);
            GUILayout.EndVertical();
            GUI.backgroundColor = Color.white;


            GUILayout.BeginHorizontal("box");
            GUI.backgroundColor = new Color(0f, 0f, 1f, 1.0f);
            EditorGUILayout.LabelField("Prefab", GUILayout.Width(150f));
            EditorGUILayout.LabelField("Memo Count", GUILayout.Width(100f));
            EditorGUILayout.LabelField("Link", GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;


            

           
            if(_MemoPrefabs != null)
            {
                GUILayout.BeginVertical("box");
                _SceneScrollPosition = GUILayout.BeginScrollView(_SceneScrollPosition);
                for(int i=  0; i< _MemoPrefabs.Count; ++i)
                {
                    if(_MemoPrefabs[i] == null)
                    {
                        isNullMemo = true;
                        continue;
                    }
                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(_MemoPrefabs[i].gameObject.name, GUILayout.Width(150f));
                    EditorGUILayout.LabelField(_ListMemoCount[i].ToString(), GUILayout.Width(100f));
                    GUI.backgroundColor = new Color(0f, 0f, 1f, 1f);
                    if (GUILayout.Button("Link", GUILayout.Width(100)))
                    {
                        Selection.activeGameObject = _MemoPrefabs[i].gameObject;
                    }

                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();

            }

            GUI.backgroundColor = new Color(0f, 0.5f, 0.7f, 1f);

            if (GUILayout.Button("Refresh", buttonStyle))
            {
                isNullMemo = true;
            }
            GUI.backgroundColor = Color.white;
            
            






            GUILayout.EndArea();

            if (isNullMemo) Load();

        }


        GUIStyle CustomHeaderStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 50;
            style.alignment = TextAnchor.MiddleCenter;
            style.margin = new RectOffset(0, 0, 0, 30);
            style.fontStyle = FontStyle.Bold;
            return style;
        }

        GUIStyle CustomSectionStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 18;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            return style;
        }

        GUIStyle TextStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 12;
            return style;
        }
        GUIStyle TextFiledSyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            style.fontSize = 12;
            return style;
        }
        GUIStyle ButtonSyle()
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 13;
            buttonStyle.margin = new RectOffset(5, 5, 10, 10);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.padding = new RectOffset(10, 10, 5, 5);

            return buttonStyle;

        }
    }
}

#endif