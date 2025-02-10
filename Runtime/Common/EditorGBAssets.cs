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
namespace GB
{
    public class EditorGBAssets : EditorWindow
    {
        [MenuItem("GB/Assets")]
        static void init()
        {
            EditorWindow.GetWindow(typeof(EditorGBAssets));
        }

        void OnEnable()
        {
            Load();
        }

        void OnFocus()
        {
            Load();
        }

        void Load()
        {
            InstalledCheckDict["UniTask"] = false;
            InstalledCheckDict["Tween"] = false;
            InstalledCheckDict["AnimationSequencer"] = false;
            InstalledCheckDict["UnityMobileLocalizedAppTitle"] = false;
            InstalledCheckDict["Vibration"] = false;
            InstalledCheckDict["UniRX"] = false;
            InstalledCheckDict["Playfab"] = false;
            InstalledCheckDict["InappManager"] = false;
            InstalledCheckDict["AdmobManager"] = false;
            InstalledCheckDict["PlayfabManager"] = false;
            InstalledCheckDict["AnimationBakingStudio"] = false;
            InstalledCheckDict["MeshBaker"] = false;
            InstalledCheckDict["NotchSolution"] = false;
            InstalledCheckDict["Logs_Viewer"] = false;
            InstalledCheckDict["ProCamera2D"] = false;
            InstalledCheckDict["Resources"] = false;
            InstalledCheckDict["UserData"] = false;
            InstalledCheckDict["Memo"] = false;
            InstalledCheckDict["GSheet"] = false;
            InstalledCheckDict["FSM"] = false;
            InstalledCheckDict["SpriteAnimation"] = false;
            InstalledCheckDict["GBSpine"] = false;





            InstalledCheckDict["UniTask"] = Type.GetType("Cysharp.Threading.Tasks.UniTask, UniTask") != null;
            InstalledCheckDict["Tween"] = Type.GetType("DG.Tweening.DOTween, DOTween") != null;
            InstalledCheckDict["AnimationSequencer"] = Type.GetType("BrunoMikoski.AnimationSequencer.AnimationSequencerController, BrunoMikoski.AnimationSequencer") != null;
            InstalledCheckDict["UnityMobileLocalizedAppTitle"] = Type.GetType("LocalizedAppTitle, LocalizedAppTitle.Runtime") != null;
            InstalledCheckDict["Vibration"] = Type.GetType("Vibration, Assembly-CSharp") != null;

            InstalledCheckDict["UniRX"] = Type.GetType("UniRx.Observable, UniRx") != null;
            InstalledCheckDict["Playfab"] = Type.GetType("PlayFab.PfEditor.ProgressBar, PlayFabEditorExtensions") != null;

            InstalledCheckDict["AnimationBakingStudio"] = Type.GetType("ABS.Frame, Assembly-CSharp") != null;
            InstalledCheckDict["MeshBaker"] = Type.GetType("DigitalOpus.MB.Core.MB_Utility, MeshBakerCore") != null;
            InstalledCheckDict["NotchSolution"] = Type.GetType("E7.NotchSolution.MockupCanvas, E7.NotchSolution") != null;
            InstalledCheckDict["Logs_Viewer"] = Type.GetType("ReporterMessageReceiver, Assembly-CSharp") != null;
            InstalledCheckDict["ProCamera2D"] = Type.GetType("Com.LuisPedroFonseca.ProCamera2D.KDTree, ProCamera2D.Runtime") != null;

            InstalledCheckDict["Resources"] = Type.GetType("GB.ResManager, Assembly-CSharp") != null;
            InstalledCheckDict["UserData"] = Type.GetType("GB.UserDataManager, Assembly-CSharp") != null;
            InstalledCheckDict["Memo"] = Type.GetType("GB.Memo, Assembly-CSharp") != null;
            InstalledCheckDict["GSheet"] = Type.GetType("GameDataManager, Assembly-CSharp") != null;
            InstalledCheckDict["FSM"] = Type.GetType("GB.FSM, Assembly-CSharp") != null;
            InstalledCheckDict["SpriteAnimation"] = Type.GetType("GB.SPRAnimation, Assembly-CSharp") != null;
            InstalledCheckDict["InappManager"] = Type.GetType("GB.InappManager, Assembly-CSharp") != null;
            InstalledCheckDict["AdmobManager"] = Type.GetType("GB.AdmobManager, Assembly-CSharp") != null;
            InstalledCheckDict["PlayfabManager"] = Type.GetType("GB.PlayFabManager, Assembly-CSharp") != null;

            InstalledCheckDict["GBSpine"] = Type.GetType("GB.SpineRemote, Assembly-CSharp") != null;



            // Debug.Log( typeof(GB.SPRAnimation).Assembly.GetName().Name);
        }

        Dictionary<string, bool> InstalledCheckDict = new Dictionary<string, bool>();

        /// ====================================
        /// Download
        /// ====================================


        const string UNITASK_URL = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";
        const string UNITASK_DOC_URL = "https://github.com/Cysharp/UniTask/tree/master";
        const string TWEEN_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/Tween.unitypackage";
        const string TWEEN_DOC_URL = "https://dotween.demigiant.com/";
        const string AnimationSequencer_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/AnimationSequencer.unitypackage";
        const string AnimationSequencer_DOC_URL = "https://github.com/brunomikoski/Animation-Sequencer";

        const string UnityMobileLocalizedAppTitle_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/UnityMobileLocalizedAppTitle.unitypackage";
        const string UnityMobileLocalizedAppTitle_DOC_URL = "https://github.com/yasirkula/UnityMobileLocalizedAppTitle";

        const string Vibration_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/Vibration.unitypackage";
        const string Vibration_DOC_URL = "https://github.com/BenoitFreslon/Vibration";

        const string UniRX_URL = "https://github.com/neuecc/UniRx.git?path=Assets/Plugins/UniRx/Scripts";
        const string UniRX_DOC_URL = "https://github.com/neuecc/UniRx";

        const string PlayFab_URL = "https://aka.ms/PlayFabUnityEdEx";
        const string PlayFab_DOC_URL = "https://learn.microsoft.com/en-us/gaming/playfab/what-is-playfab";

        const string GBInapp_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/Inapp.unitypackage";
        const string GBAdmob_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/Admob.unitypackage";
        const string GBPlayfab_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/PlayfabExpansion.unitypackage";

        const string AnimationBaking_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/AnimationBakingStudio.3Dto2D.unitypackage";
        const string AnimationBaking_DOC_URL = "https://assetstore.unity.com/packages/tools/sprite-management/animation-baking-studio-3d-to-2d-31247";

        const string MeshBaker_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/MeshBaker.unitypackage";
        const string MeshBaker_DOC_URL = "https://assetstore.unity.com/packages/tools/modeling/mesh-baker-5017";

        const string NotchSolution_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/NotchSolution.unitypackage";
        const string NotchSolution_DOC_URL = "https://assetstore.unity.com/packages/tools/gui/notch-solution-157971";

        const string Logs_Viewer_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/Unity-Logs_Viewer.unitypackage";
        const string Logs_Viewer_DOC_URL = "https://assetstore.unity.com/packages/tools/integration/log-viewer-12047";

        const string ProCamera2D_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/ProCamera2D.unitypackage";
        const string ProCamera2D_DOC_URL = "https://assetstore.unity.com/packages/2d/pro-camera-2d-the-definitive-2d-2-5d-camera-plugin-for-unity-42095";

        const string GBResources_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/Resources.unitypackage";
        const string GBUserData_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/UserData.unitypackage";

        const string Memo_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/Memo.unitypackage";
        const string GSheet_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/GSheet.unitypackage";
        const string FSM_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/FSM.unitypackage";
        public string SpriteAnimation_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/SpriteAnimation.unitypackage";
        public string GB_Spine_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/GBSpine.unitypackage";


        //Package URL
        const string PACKAGE_SensorKit = "https://github.com/3DI70R/Unity-SensorKit.git";

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

        void DrawPackage_DownloadButton(string key, string url, string docURL)
        {
            GB.EditorGUIUtil.Start_Horizontal();
            GB.EditorGUIUtil.DrawStyleLabel(key);

            GB.EditorGUIUtil.BackgroundColor(Color.green);
            if (!string.IsNullOrEmpty(docURL))
                if (GB.EditorGUIUtil.DrawButton("Link", GUILayout.Width(100))) Application.OpenURL(docURL);
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            if (GB.EditorGUIUtil.DrawButton("Download", GUILayout.Width(150))) DownloadPackage(url);

            GB.EditorGUIUtil.End_Horizontal();
        }



        void DrawGBPack_DownloadButton(string key, string url, bool installed, string installedDoc)
        {
            GB.EditorGUIUtil.Start_Horizontal();
            GB.EditorGUIUtil.DrawStyleLabel(key);


            if (!installed)
            {

                GB.EditorGUIUtil.BackgroundColor(Color.green);
                GB.EditorGUIUtil.DrawStyleLabel(installedDoc);
                GB.EditorGUIUtil.BackgroundColor(Color.white);

                GB.EditorGUIUtil.DrawStyleLabel("", GUILayout.Width(150));

                if (GB.EditorGUIUtil.DrawButton("Download", GUILayout.Width(150))) DownloadAssetData(url, key);
            }
            else
            {


                GB.EditorGUIUtil.BackgroundColor(Color.green);
                GB.EditorGUIUtil.DrawStyleLabel(installedDoc);
                GB.EditorGUIUtil.BackgroundColor(Color.white);

                GB.EditorGUIUtil.DrawStyleLabel("Installed", GUILayout.Width(150));
                GB.EditorGUIUtil.BackgroundColor(Color.cyan);
                if (GB.EditorGUIUtil.DrawButton("ReDownload", GUILayout.Width(150))) DownloadAssetData(url, key);
                GB.EditorGUIUtil.BackgroundColor(Color.white);
            }
            GB.EditorGUIUtil.End_Horizontal();

        }



        void DrawDownloadButton(string key, string url, bool installed, string docURL)
        {
            GB.EditorGUIUtil.Start_Horizontal();
            GB.EditorGUIUtil.DrawStyleLabel(key);


            if (!installed)
            {
                GB.EditorGUIUtil.BackgroundColor(Color.green);
                if (!string.IsNullOrEmpty(docURL))
                    if (GB.EditorGUIUtil.DrawButton("Link", GUILayout.Width(100))) Application.OpenURL(docURL);
                GB.EditorGUIUtil.BackgroundColor(Color.white);

                if (GB.EditorGUIUtil.DrawButton("Download", GUILayout.Width(150))) DownloadAssetData(url, key);
            }
            else
            {

                GB.EditorGUIUtil.DrawStyleLabel("Installed", GUILayout.Width(150));
                GB.EditorGUIUtil.BackgroundColor(Color.green);
                if (!string.IsNullOrEmpty(docURL))
                    if (GB.EditorGUIUtil.DrawButton("Link", GUILayout.Width(100))) Application.OpenURL(docURL);
                GB.EditorGUIUtil.BackgroundColor(Color.cyan);
                if (GB.EditorGUIUtil.DrawButton("ReDownload", GUILayout.Width(150))) DownloadAssetData(url, key);
                GB.EditorGUIUtil.BackgroundColor(Color.white);
            }
            GB.EditorGUIUtil.End_Horizontal();

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

        private void OnGUI()
        {

            GUILayout.BeginArea(new Rect(10, 20, position.width - 20, position.height - 20));
            GB.EditorGUIUtil.DrawHeaderLabel("GB Asset");
            GB.EditorGUIUtil.Space(5);

            GB.EditorGUIUtil.BackgroundColor(Color.blue);

            GB.EditorGUIUtil.Start_VerticalBox();
            GB.EditorGUIUtil.DrawSectionStyleLabel("Download Assets");
            GB.EditorGUIUtil.End_Vertical();
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            GB.EditorGUIUtil.Start_VerticalBox();
            scrollPos = GB.EditorGUIUtil.Start_ScrollView(scrollPos);

            DrawDownloadButton("Tween", TWEEN_URL, InstalledCheckDict["Tween"], TWEEN_DOC_URL);
            DrawDownloadButton("AnimationSequencer", AnimationSequencer_URL, InstalledCheckDict["AnimationSequencer"], AnimationSequencer_DOC_URL);
            DrawDownloadButton("UnityMobileLocalizedAppTitle", UnityMobileLocalizedAppTitle_URL, InstalledCheckDict["UnityMobileLocalizedAppTitle"], UnityMobileLocalizedAppTitle_DOC_URL);
            DrawDownloadButton("Vibration", Vibration_URL, InstalledCheckDict["Vibration"], Vibration_DOC_URL);
            DrawDownloadButton("Playfab", PlayFab_URL, InstalledCheckDict["Playfab"], PlayFab_DOC_URL);
            DrawDownloadButton("AnimationBakingStudio(2D to 3D)", AnimationBaking_URL, InstalledCheckDict["AnimationBakingStudio"], AnimationBaking_DOC_URL);
            DrawDownloadButton("MeshBaker", MeshBaker_URL, InstalledCheckDict["MeshBaker"], MeshBaker_DOC_URL);
            DrawDownloadButton("NotchSolution", NotchSolution_URL, InstalledCheckDict["NotchSolution"], NotchSolution_DOC_URL);
            DrawDownloadButton("Logs_Viewer", Logs_Viewer_URL, InstalledCheckDict["Logs_Viewer"], Logs_Viewer_DOC_URL);
            DrawDownloadButton("ProCamera2D", ProCamera2D_URL, InstalledCheckDict["ProCamera2D"], ProCamera2D_DOC_URL);

            DrawPackage_DownloadButton("UniRX", UniRX_URL, UniRX_DOC_URL);
            DrawPackage_DownloadButton("UniTask", UNITASK_URL, UNITASK_DOC_URL);
            DrawPackage_DownloadButton("SensorKit", PACKAGE_SensorKit, "https://github.com/3DI70R/Unity-SensorKit");
            
            DrawGBPack_DownloadButton("GB Google Sheets", GSheet_URL, InstalledCheckDict["GSheet"], "");
            DrawGBPack_DownloadButton("GB UserData", GBUserData_URL, InstalledCheckDict["UserData"], "");
            DrawGBPack_DownloadButton("GB FSM", FSM_URL, InstalledCheckDict["FSM"], "");
            DrawGBPack_DownloadButton("GB Memo", Memo_URL, InstalledCheckDict["Memo"], "");
            DrawGBPack_DownloadButton("GB Resources(Audio,Sprite,Prefab)", GBResources_URL, InstalledCheckDict["Resources"], "");
            DrawGBPack_DownloadButton("GB SPRAnimation", SpriteAnimation_URL, InstalledCheckDict["SpriteAnimation"], "");
            DrawGBPack_DownloadButton("GB FeedbackDiscord", "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/FeedbackDiscord.unitypackage", false, "");
            DrawGBPack_DownloadButton("GB Spine", GB_Spine_URL, InstalledCheckDict["GBSpine"], "Spine Expansion");
            DrawGBPack_DownloadButton("GB InappManager", GBInapp_URL, InstalledCheckDict["InappManager"], "UnityEngine.Purchasing Expansion");
            DrawGBPack_DownloadButton("GB AdmobManager", GBAdmob_URL, InstalledCheckDict["AdmobManager"], "GoogleMobileAds Expansion");
            DrawGBPack_DownloadButton("GB PlayfabManager", GBPlayfab_URL, InstalledCheckDict["PlayfabManager"], "Playfab SDK Expansion");

            GB.EditorGUIUtil.End_ScrollView();
            GB.EditorGUIUtil.End_Vertical();


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

            if(GB.EditorGUIUtil.DrawSyleButton("Update GB Framework"))
            {
                DownloadPackage("https://github.com/shogun0331/gbmain.git");
            }

            GB.EditorGUIUtil.End_Vertical();

            GUILayout.EndArea();

        }

        public static void AddOpenUPMPackage(string packageName, string scope)
        {
            // manifest.json 파일 경로
            string manifestPath = Application.dataPath.Replace("/Assets", "/Packages/manifest.json");

            // manifest.json 파일 내용 읽기
            string manifestJson = File.ReadAllText(manifestPath);

            // JSON 파싱 (Newtonsoft.Json 사용)
            // Newtonsoft.Json을 사용하려면 유니티 패키지 관리자에서 설치해야 합니다.
            // Window > Package Manager > 검색: Newtonsoft.Json

            JObject manifest = JObject.Parse(manifestJson);

            // dependencies에 패키지 추가
            if (manifest["dependencies"] == null)
            {
                manifest["dependencies"] = new JObject();
            }
            manifest["dependencies"][packageName] = "latest"; // 또는 특정 버전

            // scopedRegistries에 Scope 추가
            if (manifest["scopedRegistries"] == null)
            {
                manifest["scopedRegistries"] = new JArray();
            }

            bool scopeExists = false;
            foreach (var registry in manifest["scopedRegistries"])
            {
                if (registry["name"].ToString() == "OpenUPM")
                {
                    scopeExists = true;
                    if (registry["scopes"] == null)
                    {
                        registry["scopes"] = new JArray();
                    }
                    ((JArray)registry["scopes"]).Add(scope);
                    break;
                }
            }

            if (!scopeExists)
            {
                JObject openupmRegistry = new JObject();
                openupmRegistry["name"] = "OpenUPM";
                openupmRegistry["url"] = "https://package.openupm.com";
                openupmRegistry["scopes"] = new JArray(scope);
                ((JArray)manifest["scopedRegistries"]).Add(openupmRegistry);
            }

            // manifest.json 파일 내용 저장
            File.WriteAllText(manifestPath, manifest.ToString());

            // 유니티 에디터에 변경 사항 적용
            AssetDatabase.Refresh();

            Debug.Log($"openUPM 패키지 추가 성공: {packageName} (Scope: {scope})");
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
