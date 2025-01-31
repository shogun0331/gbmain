#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

namespace GB
{
    public class EditorGBAssets : EditorWindow
    {
        [MenuItem("GB/Assets Downloader")]
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
            InstalledCheckDict["InappManager"]  = false;
            InstalledCheckDict["AdmobManager"]  = false;
            InstalledCheckDict["PlayfabManager"] = false;
            InstalledCheckDict["AnimationBakingStudio"] = false;
            InstalledCheckDict["MeshBaker"] = false;
            InstalledCheckDict["NotchSolution"] = false;
            InstalledCheckDict["Logs_Viewer"] = false;
            InstalledCheckDict["ProCamera2D"] = false;
            InstalledCheckDict["Resources"] = false;
            
            

            InstalledCheckDict["UniTask"] = Type.GetType("Cysharp.Threading.Tasks.UniTask, UniTask") != null;
            InstalledCheckDict["Tween"] = Type.GetType("DG.Tweening.DOTween, DOTween") != null;
            InstalledCheckDict["AnimationSequencer"] = Type.GetType("BrunoMikoski.AnimationSequencer.AnimationSequencerController, BrunoMikoski.AnimationSequencer") != null;
            InstalledCheckDict["UnityMobileLocalizedAppTitle"] = Type.GetType("LocalizedAppTitle, LocalizedAppTitle.Runtime") != null;
            InstalledCheckDict["Vibration"] = Type.GetType("Vibration, Assembly-CSharp") != null;

            InstalledCheckDict["UniRX"] = Type.GetType("UniRx.Observable, UniRx") != null;
            InstalledCheckDict["Playfab"] = Type.GetType("PlayFab.PfEditor.ProgressBar, PlayFabEditorExtensions") != null;
            
            bool isUnityPurchasing = Type.GetType("UnityEngine.Purchasing.IStoreController, UnityEngine.Purchasing") != null;
            InstalledCheckDict["InappManager"] = Type.GetType("GB.InappManager, Assembly-CSharp") != null && isUnityPurchasing;
            InstalledCheckDict["AdmobManager"] = Type.GetType("GB.AdmobManager, Assembly-CSharp") != null;
            InstalledCheckDict["PlayfabManager"] = Type.GetType("GB.PlayFabManager, Assembly-CSharp") != null;
            InstalledCheckDict["AnimationBakingStudio"] = Type.GetType("ABS.Frame, Assembly-CSharp") != null;
            InstalledCheckDict["MeshBaker"] = Type.GetType("DigitalOpus.MB.Core.MB_Utility, MeshBakerCore") != null;
            InstalledCheckDict["NotchSolution"] = Type.GetType("E7.NotchSolution.MockupCanvas, E7.NotchSolution") != null;
            InstalledCheckDict["Logs_Viewer"] =  Type.GetType("ReporterMessageReceiver, Assembly-CSharp") != null;
            InstalledCheckDict["ProCamera2D"] =  Type.GetType("Com.LuisPedroFonseca.ProCamera2D.KDTree, ProCamera2D.Runtime") != null;
            InstalledCheckDict["Resources"] = Type.GetType("GB.ResManager, Assembly-CSharp") != null;

            // Debug.Log( typeof(Com.LuisPedroFonseca.ProCamera2D.KDTree).Assembly.GetName().Name);
        }

        Dictionary<string, bool> InstalledCheckDict = new Dictionary<string, bool>();

        /// ====================================
        /// Download
        /// ====================================


        const string UNITASK_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/UniTask.2.5.10.unitypackage";
        const string UNITASK_DOC_URL = "https://github.com/Cysharp/UniTask/tree/master";
        const string TWEEN_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/Tween.unitypackage";
        const string TWEEN_DOC_URL = "https://dotween.demigiant.com/";
        const string AnimationSequencer_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/AnimationSequencer.unitypackage";
        const string AnimationSequencer_DOC_URL = "https://github.com/brunomikoski/Animation-Sequencer";

        const string UnityMobileLocalizedAppTitle_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/UnityMobileLocalizedAppTitle.unitypackage";
        const string UnityMobileLocalizedAppTitle_DOC_URL = "https://github.com/yasirkula/UnityMobileLocalizedAppTitle";

        const string Vibration_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/Vibration.unitypackage";
        const string Vibration_DOC_URL = "https://github.com/BenoitFreslon/Vibration";

        const string UniRX_URL = "https://github.com/shogun0331/gbconnet/releases/download/V1.0.0/UniRx.unitypackage";
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

          void DrawGBPack_DownloadButton(string key, string url , bool installed,string installedDoc)
        {
            GB.EditorGUIUtil.Start_Horizontal();
            GB.EditorGUIUtil.DrawStyleLabel(key);


            if (!installed)
            {
                
                GB.EditorGUIUtil.BackgroundColor(Color.green);
                GB.EditorGUIUtil.DrawStyleLabel( installedDoc + " Expansion");
                GB.EditorGUIUtil.BackgroundColor(Color.white);

                GB.EditorGUIUtil.DrawStyleLabel("", GUILayout.Width(150));

                if (GB.EditorGUIUtil.DrawButton("Download", GUILayout.Width(150))) DownloadPackage(url, key);
            }
            else
            {
               
                
                GB.EditorGUIUtil.BackgroundColor(Color.green);
                GB.EditorGUIUtil.DrawStyleLabel( installedDoc + " Expansion");
                GB.EditorGUIUtil.BackgroundColor(Color.white);

                GB.EditorGUIUtil.DrawStyleLabel("Installed", GUILayout.Width(150));
                GB.EditorGUIUtil.BackgroundColor(Color.cyan);
                if (GB.EditorGUIUtil.DrawButton("ReDownload", GUILayout.Width(150))) DownloadPackage(url, key);
                GB.EditorGUIUtil.BackgroundColor(Color.white);
            }
            GB.EditorGUIUtil.End_Horizontal();

        }



        void DrawDownloadButton(string key, string url , bool installed,string docURL)
        {
            GB.EditorGUIUtil.Start_Horizontal();
            GB.EditorGUIUtil.DrawStyleLabel(key);


            if (!installed)
            {
                GB.EditorGUIUtil.BackgroundColor(Color.green);
                if(!string.IsNullOrEmpty( docURL))
                if (GB.EditorGUIUtil.DrawButton("Link", GUILayout.Width(100))) Application.OpenURL(docURL);
                GB.EditorGUIUtil.BackgroundColor(Color.white);

                if (GB.EditorGUIUtil.DrawButton("Download", GUILayout.Width(150))) DownloadPackage(url, key);
            }
            else
            {
               
                GB.EditorGUIUtil.DrawStyleLabel("Installed", GUILayout.Width(150));
                GB.EditorGUIUtil.BackgroundColor(Color.green);
                if(!string.IsNullOrEmpty( docURL))
                if (GB.EditorGUIUtil.DrawButton("Link", GUILayout.Width(100))) Application.OpenURL(docURL);
                GB.EditorGUIUtil.BackgroundColor(Color.cyan);
                if (GB.EditorGUIUtil.DrawButton("ReDownload", GUILayout.Width(150))) DownloadPackage(url, key);
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

            DrawDownloadButton("Tween",TWEEN_URL,InstalledCheckDict["Tween"],TWEEN_DOC_URL);
            DrawDownloadButton("UniTask",UNITASK_URL,InstalledCheckDict["UniTask"],UNITASK_DOC_URL);
            DrawDownloadButton("UniRX",UniRX_URL,InstalledCheckDict["UniRX"],UniRX_DOC_URL);
            DrawDownloadButton("AnimationSequencer",AnimationSequencer_URL,InstalledCheckDict["AnimationSequencer"],AnimationSequencer_DOC_URL);
            DrawDownloadButton("UnityMobileLocalizedAppTitle",UnityMobileLocalizedAppTitle_URL,InstalledCheckDict["UnityMobileLocalizedAppTitle"],UnityMobileLocalizedAppTitle_DOC_URL);
            DrawDownloadButton("Vibration",Vibration_URL,InstalledCheckDict["Vibration"],Vibration_DOC_URL);
            DrawDownloadButton("Playfab",PlayFab_URL,InstalledCheckDict["Playfab"],PlayFab_DOC_URL);
            DrawDownloadButton("AnimationBakingStudio(2D to 3D)",AnimationBaking_URL,InstalledCheckDict["AnimationBakingStudio"],AnimationBaking_DOC_URL);
            DrawDownloadButton("MeshBaker",MeshBaker_URL,InstalledCheckDict["MeshBaker"],MeshBaker_DOC_URL);
            DrawDownloadButton("NotchSolution",NotchSolution_URL,InstalledCheckDict["NotchSolution"],NotchSolution_DOC_URL);
            DrawDownloadButton("Logs_Viewer",Logs_Viewer_URL,InstalledCheckDict["Logs_Viewer"],Logs_Viewer_DOC_URL);
            DrawDownloadButton("ProCamera2D",ProCamera2D_URL,InstalledCheckDict["ProCamera2D"],ProCamera2D_DOC_URL);

            DrawGBPack_DownloadButton("GB Resources(Audio,Sprite,Prefab)",GBResources_URL,InstalledCheckDict["Resources"],"");
            DrawGBPack_DownloadButton("GB InappManager",GBInapp_URL,InstalledCheckDict["InappManager"],"UnityEngine.Purchasing");
            DrawGBPack_DownloadButton("GB AdmobManager",GBAdmob_URL,InstalledCheckDict["AdmobManager"],"GoogleMobileAds");
            DrawGBPack_DownloadButton("GB PlayfabManager",GBPlayfab_URL,InstalledCheckDict["PlayfabManager"],"Playfab SDK");
            
            GB.EditorGUIUtil.End_ScrollView();
            GB.EditorGUIUtil.End_Vertical();


            GB.EditorGUIUtil.BackgroundColor(Color.gray);
            GB.EditorGUIUtil.Start_VerticalBox();
            GB.EditorGUIUtil.DrawSectionStyleLabel("Link SDKs");
            GB.EditorGUIUtil.End_Vertical();
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            
            GB.EditorGUIUtil.Start_VerticalBox();
            linkScrollPos = GB.EditorGUIUtil.Start_ScrollView(linkScrollPos);

            DrawLinkButton("Spine Unity SDK",Spine_URL);
            DrawLinkButton("Firebase Unity SDK",Firebase_URL);
            DrawLinkButton("Admob Unity SDK",ADMOB_URL);
            DrawLinkButton("Google Play Games plugin for Unity",GPGS_URL);
            DrawLinkButton("NHN Game Package Manager for Unity",NHNGamePackageManager_URL);

            GB.EditorGUIUtil.End_ScrollView();
            GB.EditorGUIUtil.End_Vertical();

            GUILayout.EndArea();

        }

        private void DownloadPackage(string url, string fileName)
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
