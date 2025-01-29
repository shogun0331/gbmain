#if UNITY_EDITOR

using System.IO.Compression;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GB
{

    public class EditorStateMachine : EditorWindow
    {

        [MenuItem("GB/Core/StateMacine")]
        static void init()
        {
            EditorWindow.GetWindow(typeof(EditorStateMachine));
        }
        private void OnEnable()
        {

            System.Type MyScriptType = System.Type.GetType(CharacterName + ",Assembly-CSharp");
            isScriptCheck = MyScriptType != null ? true : false;

        }

        private void OnFocus()
        {
            System.Type MyScriptType = System.Type.GetType(CharacterName + ",Assembly-CSharp");
            isScriptCheck = MyScriptType != null ? true : false;

        }

        bool isScriptCheck;



        public string CharacterName;
        public string CharacterStateInput;
        public List<string> CharacterStates = new List<string>();
        public Vector2 scrollPos;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 20, position.width - 20, position.height - 20));
            GB.EditorGUIUtil.DrawHeaderLabel("GB StateMachine");
            GB.EditorGUIUtil.Space(5);
            EditorGUIUtil.BackgroundColor(Color.blue);
            GB.EditorGUIUtil.Start_VerticalBox();
            GB.EditorGUIUtil.DrawSectionStyleLabel("State");
            GB.EditorGUIUtil.End_Vertical();
            EditorGUIUtil.BackgroundColor(Color.white);
            GB.EditorGUIUtil.Space(5);



            CharacterName = GB.EditorGUIUtil.DrawSyleTextField("CharacterName", CharacterName);
            GB.EditorGUIUtil.Space(20);
            GB.EditorGUIUtil.Start_Horizontal();

            CharacterStateInput = GB.EditorGUIUtil.DrawTextField("CharacterState", CharacterStateInput);
            if (GB.EditorGUIUtil.DrawButton("Add", GUILayout.Width(100)))
            {
                List<string> list = CharacterStates.ToList();
                list.Add(CharacterStateInput);
                HashSet<string> uniqueSet = new HashSet<string>(list);
                if (list.Count == uniqueSet.Count) CharacterStates.Add(CharacterStateInput);

                else Debug.Log("duplicate State!!!!");

            }
            GB.EditorGUIUtil.End_Horizontal();

            GB.EditorGUIUtil.Start_VerticalBox();

            scrollPos = GB.EditorGUIUtil.Start_ScrollView(scrollPos);
            int removeidx = -1;

            for (int i = 0; i < CharacterStates.Count; ++i)
            {

                GB.EditorGUIUtil.Start_Horizontal();
                GB.EditorGUIUtil.Space(155);


                GUILayout.Label(CharacterStates[i], GUILayout.Width(300));

                EditorGUIUtil.BackgroundColor(Color.red);

                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    removeidx = i;
                }
                EditorGUIUtil.BackgroundColor(Color.white);
                GB.EditorGUIUtil.End_Horizontal();

            }

            if (removeidx > -1) CharacterStates.RemoveAt(removeidx);

            GB.EditorGUIUtil.End_ScrollView();
            GB.EditorGUIUtil.End_Vertical();
            EditorGUIUtil.BackgroundColor(Color.green);
            if (GB.EditorGUIUtil.DrawSyleButton("Create Script"))
            {
                CreateScript();
            }

            if (isScriptCheck)
            {

                EditorGUIUtil.BackgroundColor(Color.blue);
                if (GB.EditorGUIUtil.DrawSyleButton("Create GameObject"))
                {
                    CreateGameObject();
                }
            }

            EditorGUIUtil.BackgroundColor(Color.white);



            GUILayout.EndArea();


        }

        void CreateScript()
        {
            if (string.IsNullOrEmpty(CharacterStateInput)) return;
            if (CharacterStates.Count <= 0) return;
            string stateMachine = string.Empty;

            string cName = CharacterName;
            string[] cStates = CharacterStates.ToArray();
            stateMachine = Resources.Load<TextAsset>("ExportStateMacineClass").text;
            stateMachine = stateMachine.Replace("#KEY", cName);
            string states = string.Empty;
            states = cStates[0];
            for (int i = 1; i < cStates.Length; ++i)
            {
                states += "," + cStates[i];
            }

            stateMachine = stateMachine.Replace("#STATES", states);


            string addCom = string.Empty;
            for (int i = 0; i < cStates.Length; ++i)
            {
                string forme = EDITOR_SETTING_FORME;
                forme = forme.Replace("#SCRIPT", cName + "_" + cStates[i]);
                forme = forme.Replace("#STATE", cStates[i]);
                forme = forme.Replace("#CHARACTER", cName);
                addCom += forme ;
            }

            stateMachine = stateMachine.Replace("#ADDComponent", addCom);

            string diPath = Application.dataPath + "/Scripts/StateMacine/" + cName;
            DirectoryInfo info = new DirectoryInfo(diPath);
            if (info.Exists == false) info.Create();

            File.WriteAllText(diPath + "/" + cName + ".cs", stateMachine);



            diPath = Application.dataPath + "/Scripts/StateMacine/" + cName + "/" + "State";
            info = new DirectoryInfo(diPath);
            if (info.Exists == false) info.Create();

            for (int i = 0; i < cStates.Length; ++i)
            {
                string macineForme = Resources.Load<TextAsset>("ExportMacineClass").text;
                macineForme = macineForme.Replace("#KEY", cName);
                macineForme = macineForme.Replace("#STATE", cStates[i]);
                File.WriteAllText(diPath + "/" + cName + "_" + cStates[i] + ".cs", macineForme);
            }

            UnityEditor.AssetDatabase.Refresh();
        }

        const string EDITOR_SETTING_FORME = @"
        if(GetComponent<#SCRIPT>() != null) SetMacine(#CHARACTERState.#STATE.ToString(),GetComponent<#SCRIPT>());
        else SetMacine(#CHARACTERState.#STATE.ToString(),gameObject.AddComponent<#SCRIPT>());";

        void CreateGameObject()
        {
            //We need to fetch the Type
            System.Type MyScriptType = System.Type.GetType(CharacterName + ",Assembly-CSharp");

            if (MyScriptType != null)
            {
                var oj = new GameObject(CharacterName);
                oj.AddComponent(MyScriptType);
            }

        }



    }


    public static class StateMachineWindowMenu
    {
        [MenuItem("Assets/GB/StateMachine")]
        private static void CustomActionWithSelection()
        {
            // 선택된 에셋의 경로를 가져옵니다.
            // string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            // Debug.Log($"Selected Asset Path: {path}");
            EditorWindow.GetWindow(typeof(EditorStateMachine));
        }
    }
}

#endif