#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using Newtonsoft.Json.Linq;
using QuickEye.Utility;
using Newtonsoft.Json;
namespace GB
{
    [System.Serializable]
    public class GBAssetVersion
    {
        public string Version;
        public UnityDictionary<string,string> UnityPackages = new UnityDictionary<string, string>();
        public UnityDictionary<string,string> UnityPackageDocs = new UnityDictionary<string, string>();
        public UnityDictionary<string,string> GithubPackages = new UnityDictionary<string, string>();
        public UnityDictionary<string,string> GithubDocs = new UnityDictionary<string, string>();
    }


    public class EditorGBAssets : EditorWindow
    {
        [MenuItem("GB/Assets")]
        static void init()
        {
            EditorWindow.GetWindow(typeof(EditorGBAssets));
        }

        GBAssetVersion GBVersion;

        void OnEnable()
        {
            string versionText = Resources.Load<TextAsset>("GBVersion").text;
            GBVersion = versionText.FromJson<GBAssetVersion>();
            GBVersion.Version.GBLog("Version");
            Load();
         
        }

        void OnFocus()
        {
            Load();
        }

        void Load()
        {
        

        }

        
        /// ====================================
        /// Download
        /// ====================================


        const string FSM_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/FSM.unitypackage";
        public string SpriteAnimation_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/SpriteAnimation.unitypackage";
        public string GB_Spine_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/GBSpine.unitypackage";

        /// ====================================
        /// LINK
        /// ====================================

        const string Spine_URL = "https://ko.esotericsoftware.com/spine-unity-download";
        const string Firebase_URL = "https://github.com/firebase/firebase-unity-sdk/releases";
        const string ADMOB_URL = "https://github.com/googleads/googleads-mobile-unity/releases";
        const string GPGS_URL = "https://github.com/playgameservices/play-games-plugin-for-unity";
        const string NHNGamePackageManager_URL = "https://github.com/nhn/gpm.unity?tab=readme-ov-file";


        Vector2 scrollPos;
        Vector2 linkScrollPos;

        void DrawGithubPackage_DownloadButton(string key, string url, string docURL)
        {
            GB.EditorGUIUtil.Start_Horizontal();
            GB.EditorGUIUtil.DrawStyleLabel(key);

            GB.EditorGUIUtil.BackgroundColor(Color.green);
            if (!string.IsNullOrEmpty(docURL))
            if (GB.EditorGUIUtil.DrawButton("Doc", GUILayout.Width(100))) Application.OpenURL(docURL);
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            if (GB.EditorGUIUtil.DrawButton("Download", GUILayout.Width(150))) DownloadPackage(url);

            GB.EditorGUIUtil.End_Horizontal();
        }


        void DrawUnityPackageDownloadButton(string key, string url, string docURL)
        {
            GB.EditorGUIUtil.Start_Horizontal();
            GB.EditorGUIUtil.DrawStyleLabel(key);

            GB.EditorGUIUtil.BackgroundColor(Color.green);
            if (!string.IsNullOrEmpty(docURL))
            if (GB.EditorGUIUtil.DrawButton("Doc", GUILayout.Width(100))) Application.OpenURL(docURL);
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            if (GB.EditorGUIUtil.DrawButton("Download", GUILayout.Width(150))) DownloadAssetData(url, key);
            GB.EditorGUIUtil.End_Horizontal();

        }

       

        private void OnGUI()
        {

            GUILayout.BeginArea(new Rect(10, 20, position.width - 20, position.height - 20));
            GB.EditorGUIUtil.DrawHeaderLabel("GB Asset");
            GB.EditorGUIUtil.Space(5);

            GB.EditorGUIUtil.DrawSectionStyleLabel("GB Framework Ver " +GBVersion.Version);


            GB.EditorGUIUtil.BackgroundColor(Color.blue);

            GB.EditorGUIUtil.Start_VerticalBox();
            GB.EditorGUIUtil.DrawSectionStyleLabel("Download Assets");
            GB.EditorGUIUtil.End_Vertical();
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            GB.EditorGUIUtil.Start_VerticalBox();
            scrollPos = GB.EditorGUIUtil.Start_ScrollView(scrollPos);
            foreach(var v in GBVersion.GithubPackages)
            {
                string docURL = string.Empty;
                if(GBVersion.GithubDocs.ContainsKey(v.Key)) docURL = GBVersion.GithubDocs[v.Key];
                DrawGithubPackage_DownloadButton(v.Key,v.Value,docURL);
            } 


            foreach(var v in GBVersion.UnityPackages) 
            {
                string docURL = string.Empty;
                if(GBVersion.UnityPackageDocs.ContainsKey(v.Key)) docURL = GBVersion.UnityPackageDocs[v.Key];
                else docURL = "https://gb-framework.gitbook.io/gb-framework-docs";
                DrawUnityPackageDownloadButton(v.Key,v.Value,docURL);
            }
            DrawFSM_DownloadButton();
            

            GB.EditorGUIUtil.End_ScrollView();
            GB.EditorGUIUtil.End_Vertical();
// 

            GB.EditorGUIUtil.BackgroundColor(Color.gray);
            GB.EditorGUIUtil.Start_VerticalBox();
            GB.EditorGUIUtil.DrawSectionStyleLabel("Link SDKs");
            GB.EditorGUIUtil.End_Vertical();
            GB.EditorGUIUtil.BackgroundColor(Color.white);


            GB.EditorGUIUtil.Start_VerticalBox();
            linkScrollPos = GB.EditorGUIUtil.Start_ScrollView(linkScrollPos);

            DrawLinkButton("Spine Unity SDK", Spine_URL);
            DrawLinkButton("Firebase Unity SDK", Firebase_URL);
            DrawLinkButton("Admob Unity SDK", ADMOB_URL);
            DrawLinkButton("Google Play Games plugin for Unity", GPGS_URL);
            DrawLinkButton("NHN Game Package Manager for Unity", NHNGamePackageManager_URL);

            GB.EditorGUIUtil.End_ScrollView();

            if (GB.EditorGUIUtil.DrawSyleButton("Update GB Framework "))
            {
                DownloadPackage("https://github.com/shogun0331/gbmain.git");
            }

            GB.EditorGUIUtil.End_Vertical();

            GUILayout.EndArea();

        }

        void DrawLinkButton(string key, string url)
        {
            GB.EditorGUIUtil.Start_Horizontal();
            GB.EditorGUIUtil.DrawStyleLabel(key);
            GB.EditorGUIUtil.BackgroundColor(Color.green);
            if (GB.EditorGUIUtil.DrawButton("Link", GUILayout.Width(100))) Application.OpenURL(url);
            GB.EditorGUIUtil.BackgroundColor(Color.white);
            GB.EditorGUIUtil.End_Horizontal();
        }

         void DrawFSM_DownloadButton()
        {
            GB.EditorGUIUtil.Start_Horizontal();
            GB.EditorGUIUtil.DrawStyleLabel("GB FSM");

            GB.EditorGUIUtil.BackgroundColor(Color.green);
            GB.EditorGUIUtil.DrawStyleLabel("");
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            GB.EditorGUIUtil.DrawStyleLabel("", GUILayout.Width(150));
            GB.EditorGUIUtil.BackgroundColor(Color.green);
            if (GB.EditorGUIUtil.DrawButton("Doc", GUILayout.Width(100))) Application.OpenURL("https://gb-framework.gitbook.io/gb-framework-docs");
            GB.EditorGUIUtil.BackgroundColor(Color.white);
            

            if (GB.EditorGUIUtil.DrawButton("Download", GUILayout.Width(150))) 
            {
                DownloadPackage("https://github.com/Thundernerd/Unity3D-SerializableInterface.git");
                DownloadAssetData(FSM_URL, "GB FSM");
            }
            GB.EditorGUIUtil.End_Horizontal();

        }
        private void DownloadPackage(string url)
        {

            AddRequest request = UnityEditor.PackageManager.Client.Add(url);
            while (!request.IsCompleted)
            {
                // 필요에 따라 진행 상황을 표시하거나 다른 작업을 수행할 수 있습니다.
                // 예: EditorUtility.DisplayProgressBar("패키지 추가 중...", request.Progress * 100, 100);
            }

            if (request.Status == StatusCode.Success)
            {
                Debug.Log("Package Add Success: " + request.Result.packageId);
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Package Add Fail!! : " + request.Error.message);
            }
        }

        private void DownloadAssetData(string url, string fileName)
        {
            string downloadUrl = url;

            using (var www = UnityWebRequest.Get(downloadUrl))
            {
                www.SendWebRequest();

                while (!www.isDone)
                {
                    Debug.Log("Downloading: " + www.downloadProgress * 100 + "%");
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string savePath = Path.Combine(Application.dataPath, fileName + ".unitypackage");
                    File.WriteAllBytes(savePath, www.downloadHandler.data);
                    Debug.Log("Package downloaded to: " + savePath);

                    AssetDatabase.ImportPackage(savePath, true); // 유니티 프로젝트에 임포트

                    // 파일 삭제
                    File.Delete(savePath);
                    AssetDatabase.Refresh(); // 유니티 에디터에 변경 사항 반영

                }
                else
                {
                    Debug.LogError("Download failed: " + www.error);
                }
            }
        }
    }
}
#endif
