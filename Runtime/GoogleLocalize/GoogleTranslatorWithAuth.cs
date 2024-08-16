#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace GB
{
    public class GoogleTranslatorWithAuth : EditorWindow
    {

        
        public static string Url;


        public string[] PassString =
        {
        "@COMMA"
        };

        IReadOnlyDictionary<string, string> _LanguageCode = new Dictionary<string, string>
    {
        {"English","en"},
        {"Korean","ko"},
        {"Japanese","ja"},
        {"ChineseSimplified","zh-CN"},
        {"ChineseTraditional","zh-TW"},
        {"German","de"},
        {"Spanish","es"},
        {"French","fr"},
        {"Thai","th"},
        {"Russian","ru"},
        {"Italian","it"},
        {"Portuguese","pt"},
        {"Turkish","tr"},
        {"Vietnamese","vi"},
        {"Indonesian","id"},
        {"Hindi","hi"}
    };

        //###
        public Queue<string> passColorQ = new Queue<string>();

        //@@@
        public Queue<string> passStrQ = new Queue<string>();


        [MenuItem("Tools/GB/GooleSheetLocalize")]
        static void init()
        {
            EditorWindow.GetWindow(typeof(GoogleTranslatorWithAuth));
            Url = EditorPrefs.GetString("L_GooleUrlID", string.Empty);
        }

        private string PassStrTag(string value)
        {
            passStrQ.Clear();

            string str = value;

            for (int k = 0; k < 1000000; ++k)
            {
                List<int> idxList = new List<int>();
                var keyValuePairs = new Dictionary<int, string>();

                for (int i = 0; i < PassString.Length; ++i)
                {
                    int idx = str.IndexOf(PassString[i]);

                    if (idx != -1)
                    {
                        idxList.Add(idx);
                        keyValuePairs[idx] = PassString[i];
                    }
                }

                if (idxList.Count == 0) break;

                idxList.Sort();
                string n = keyValuePairs[idxList[0]];
                passStrQ.Enqueue(n);
                str = str.Insert(idxList[0], "@@@");
                str = str.Remove(idxList[0] + 3, n.Length);

            }

            return str;
        }

        private string PassColorTag(string value)
        {
            passColorQ.Clear();
            string str = value.Replace(" ", "");

            for (int i = 0; i < 1000000; ++i)
            {
                //Start Color Tag
                int firstIdx = str.IndexOf("<color");
                if (firstIdx == -1) break;
                int last = str.IndexOf(">");
                string subString = str.Substring(firstIdx, last + 1);
                passColorQ.Enqueue(subString);
                str = str.Insert(firstIdx, "###");
                str = str.Remove(firstIdx + 3, subString.Length);

                //End Color Tag
                firstIdx = str.IndexOf("</color>");
                if (firstIdx == -1) break;
                passColorQ.Enqueue("</color>");
                str = str.Insert(firstIdx, "###");
                str = str.Remove(firstIdx + 3, 8);
            }

            return str;

        }

        private string TagStrInsert(string value)
        {
            string str = value;

            int len = passStrQ.Count;
            for (int i = 0; i < len; ++i)
            {
                string v = passStrQ.Dequeue();
                int idx = str.IndexOf("###");
                if (idx == -1) break;
                str = str.Insert(idx, v);
                str = str.Remove(idx + v.Length, 3);
            }

            return str;
        }

        private string TagColorInsert(string value)
        {
            string str = value;

            int len = passColorQ.Count;
            for (int i = 0; i < len; ++i)
            {
                string v = passColorQ.Dequeue();
                int idx = str.IndexOf("@@@");
                if (idx == -1) break;
                str = str.Insert(idx, v);
                str = str.Remove(idx + v.Length, 3);
            }

            return str;
        }


        private string SetTagConvert(string value)
        {
            string str = PassStrTag(value);
            str = PassColorTag(str);

            return str;
        }

        private string SetTagInsert(string value)
        {
            string str = TagColorInsert(value);
            str = TagStrInsert(str);

            return str;
        }


        public string GetURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }


            try
            {
                var arr = url.Split("/");
                var f = arr[6].Split("gid=");

                string urlID = arr[5];
                string gid = Regex.Replace(f[1], @"\D", "");

                string URL_SHEET = $"https://docs.google.com/spreadsheets/d/$URL_ID$/export?format=tsv&gid=$GID$";
                return URL_SHEET.Replace("$URL_ID$", urlID).Replace("$GID$", gid);
            }
            catch
            {
                 return null;

            }

        }


        private void ButtonSave()
        {
            string url = GetURL(Url);
            if(string.IsNullOrEmpty(url))
            {
                Debug.LogError("URL Error : " + Url);
                return;
            }
            string tsv = UrlDownload(url);
            string json = PaserJson(tsv);
            string path = Application.dataPath + "/" + "Resources/Json";
            FileSave(path, "TextTable.json", json);


            EditorPrefs.SetString("L_GooleUrlID", Url);

            LocalizationManager.I.PaserData();
        }

      

        private string UrlDownload(string url)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SendWebRequest();
                while (!www.isDone) { }
                Debug.Log(www.downloadHandler.text);
                return www.downloadHandler.text;
            }
        }

        private void FileSave(string path, string fileName, string data)
        {
            //string path = Application.dataPath + "/" + "Resources/Json";
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists == false)
                info.Create();

            System.IO.File.WriteAllText(path + "/" + fileName, data);
            UnityEditor.AssetDatabase.Refresh();
        }


        private void OnGUI()
        {
            GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
            GUIStyle style = new GUIStyle();


            EditorGUILayout.BeginHorizontal();
            Url = EditorGUILayout.TextField("URL", Url, GUILayout.Width(540f));
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("SaveJson"))
            {
                ButtonSave();
            }


        }



        private string PaserJson(string csv)
        {

            string json = string.Empty;
            List<string> rowKeys = new List<string>();
            List<string> columnKeys = new List<string>();
            List<string> typeKeys = new List<string>();
            List<string> values = new List<string>();


            string[] columns = csv.Replace("\r", "").Split("\n");
            string[] rowKeyArr = columns[0].Split("\t");
            string[] typeKeyArr = columns[1].Split("\t");

            for (int i = 1; i < typeKeyArr.Length; ++i)
            {
                if (string.IsNullOrEmpty(typeKeyArr[i])) break;

                typeKeys.Add(typeKeyArr[i]);
            }

            for (int i = 1; i < rowKeyArr.Length; ++i)
            {
                if (string.IsNullOrEmpty(rowKeyArr[i])) break;

                rowKeys.Add(rowKeyArr[i]);
            }



            for (int y = 0; y < columns.Length; ++y)
            {
                string[] rows = columns[y].Split("\t");
                if (string.IsNullOrEmpty(rows[0])) continue;

                for (int x = 0; x < rows.Length; ++x)
                {
                    if (y >= 3 && x == 0)
                        columnKeys.Add(rows[x]);

                    if (y >= 3 && x >= 1)
                    {
                        values.Add(rows[x].Replace("@COMMA",",").Replace("\\n","\n"));
                    }
                }
            }


            json = @"{""Datas"":[$DATAS$]}";
            string datas = string.Empty;

            for (int y = 0; y < columnKeys.Count; ++y)
            {
                Dictionary<string, object> dicData = new Dictionary<string, object>();
                dicData.Add(rowKeyArr[0], columnKeys[y]);

                for (int x = 0; x < rowKeys.Count; ++x)
                {
                    dicData.Add(rowKeys[x], GetObjType(typeKeys[x], values[y * rowKeys.Count + x]));

                }

                if (string.IsNullOrEmpty(datas))
                    datas = JsonConvert.SerializeObject(dicData);
                else
                    datas = string.Format("{0},{1}", datas, JsonConvert.SerializeObject(dicData));
            }


            json = json.Replace("$DATAS$", datas);
            return json;

        }

        public object GetObjType(string type, string value)
        {
            object oj = null;
            switch (type)
            {
                case "int":
                    if (string.IsNullOrEmpty(value)) oj = 0;
                    else oj = int.Parse(value);
                    break;

                case "float":
                    if (string.IsNullOrEmpty(value)) oj = 0.0f;
                    else oj = float.Parse(value);
                    break;

                case "long":
                    if (string.IsNullOrEmpty(value)) oj = 0;
                    else oj = long.Parse(value);
                    break;

                case "string":
                    oj = value;
                    break;

                case "double":
                    if (string.IsNullOrEmpty(value)) oj = 0;
                    else oj = double.Parse(value);
                    break;

                default:
                    if (string.IsNullOrEmpty(value)) oj = "Empty";
                    else oj = value;
                    break;
            }

            return oj;

        }
    

    }
}
#endif
