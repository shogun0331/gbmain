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
        [MenuItem("GB/Core/Assets Downloader")]
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

            InstalledCheckDict["UniTask"] = Type.GetType("Cysharp.Threading.Tasks.UniTask, UniTask") != null;
            InstalledCheckDict["Tween"] = Type.GetType("DG.Tweening.DOTween, DOTween") != null;
            InstalledCheckDict["AnimationSequencer"] = Type.GetType("BrunoMikoski.AnimationSequencer.AnimationSequencerController, BrunoMikoski.AnimationSequencer") != null;

            // Debug.Log( typeof(Cysharp.Threading.Tasks.UniTask).Assembly.GetName().Name);
        }

        Dictionary<string, bool> InstalledCheckDict = new Dictionary<string, bool>();

        const string UNITASK_URL = "https://drive.usercontent.google.com/u/0/uc?id=1JaqM8W8QnokO-QFvC-CiF940wHlKr4aJ&export=download";
        const string TWEEN_URL = "https://drive.usercontent.google.com/u/0/uc?id=1cEwlMHREdbUILowLeXmX-sqSTCvd7gIj&export=download";
        const string AnimationSequencer = "https://drive.usercontent.google.com/u/0/uc?id=1NitWKU5O1fSPZRRQTKg2g8wfa-OsuNTl&export=download";

        Vector2 scrollPos;

        void DrawDownloadButton(string key, string url , bool installed)
        {
            GB.EditorGUIUtil.Start_Horizontal();
            GB.EditorGUIUtil.DrawStyleLabel(key);
            if (!installed)
            {
                if (GB.EditorGUIUtil.DrawButton("Download", GUILayout.Width(150))) DownloadPackage(url, key);
            }
            else
            {
                GB.EditorGUIUtil.BackgroundColor(Color.cyan);
                GB.EditorGUIUtil.DrawStyleLabel("Installed", GUILayout.Width(150));
                if (GB.EditorGUIUtil.DrawButton("ReDownload", GUILayout.Width(150))) DownloadPackage(url, key);
                GB.EditorGUIUtil.BackgroundColor(Color.white);
            }
            GB.EditorGUIUtil.End_Horizontal();

        }

        private void OnGUI()
        {

            GUILayout.BeginArea(new Rect(10, 20, position.width - 20, position.height - 20));
            GB.EditorGUIUtil.DrawHeaderLabel("GB GB Asset");
            GB.EditorGUIUtil.Space(5);

            GB.EditorGUIUtil.BackgroundColor(Color.blue);

            GB.EditorGUIUtil.Start_VerticalBox();
            GB.EditorGUIUtil.DrawSectionStyleLabel("Download Assets");
            GB.EditorGUIUtil.End_Vertical();
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            GB.EditorGUIUtil.Start_VerticalBox();
            scrollPos = GB.EditorGUIUtil.Start_ScrollView(scrollPos);

            DrawDownloadButton("Tween",TWEEN_URL,InstalledCheckDict["Tween"]);
            DrawDownloadButton("UniTask",UNITASK_URL,InstalledCheckDict["UniTask"]);
            DrawDownloadButton("AnimationSequencer",AnimationSequencer,InstalledCheckDict["AnimationSequencer"]);

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
