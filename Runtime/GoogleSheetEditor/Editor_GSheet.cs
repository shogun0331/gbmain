#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;


public class Editor_GSheet : EditorWindow
{

    static GSheetDomain domain = new GSheetDomain();

    [MenuItem("Tools/GB/GooleSheetGameData")]
    static void init()
    {
        EditorWindow.GetWindow(typeof(Editor_GSheet));
        domain.Load();
    }

    public static string url;
    
    public static string sheetName;
    public static string validatorSheetName;

    private void OnGUI()
    {
        GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.LowerCenter;
        style.normal.textColor = Color.white;
        EditorGUILayout.BeginHorizontal();
        sheetName = EditorGUILayout.TextField("SheetName", sheetName, GUILayout.Width(540f));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        url = EditorGUILayout.TextField("URL", url, GUILayout.Width(540f));
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("Add"))
        {
            if (string.IsNullOrEmpty(sheetName) || string.IsNullOrEmpty(url)) return;

            domain.Add(sheetName, url);
            domain.Save();
        }

        for (int i = 0; i < domain.Count; ++i)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(domain.SheetNameList[i], GUILayout.Width(67.5f));
            string url = domain.GetURL(i);
            if(string.IsNullOrEmpty(url))
            {
                domain.Remove(i);
                return;
            }


            EditorGUILayout.LabelField(url, GUILayout.Width(270f));

            if (GUILayout.Button("Delete"))
            {
                domain.Remove(i);
                
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(30);

        if (GUILayout.Button("SaveJson"))
        {
            ButtonSaveJson();
            if(domain.Count >0)
            domain.Save();
        }

        if (GUILayout.Button("SaveCS"))
        {
            ButtonSaveCs();
            if (domain.Count > 0)
                domain.Save();
        }


        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        validatorSheetName = EditorGUILayout.TextField("ValidatorSheetName", validatorSheetName, GUILayout.Width(540f));
        //EditorGUILayout.LabelField(domain.SheetNameList[i], GUILayout.Width(67.5f));

        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Validator"))
        {
            int idx = domain.GetIndex(validatorSheetName);
            if (idx != -1)
            {
                GSheetToValidator(idx);
            }
            else
            {
                Debug.LogWarning("None SheetName");
            }

        }


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

            string URL_SHEET = $"https://docs.google.com/spreadsheets/d/$URL_ID$/export?format=csv&gid=$GID$";

            return URL_SHEET.Replace("$URL_ID$", urlID).Replace("$GID$", gid);
        }
        catch
        {
            return null;
        }

    }



    private void GSheetToValidator(int idx)
    {
        string url = domain.GetURL(idx);
        if(string.IsNullOrEmpty( url))
        {
            domain.Remove(idx);
            return;
        }

        string csv = UrlDownload(url);

        if(csv == null)
        {
            domain.Remove(idx);
            return;
        }
        
        string[] lines = csv.Split("\n");
        string[] propertyTypes = lines[1].Split(",");
        string[] keys = lines[0].Split(",");
        

        for (int x = 1; x < propertyTypes.Length; ++x)
        {
            string typeName = propertyTypes[x];
            string key = keys[x];

            for (int y = 3; y < lines.Length; ++y)
            {
                string[] spritDatas = lines[y].Split(",");
                string data = spritDatas[x].Replace("\r", "");
                if (string.IsNullOrEmpty(data)) continue;
                ValidatorProperty(key, typeName, data, x, y);
            }
        }
    }

    private bool ValidatorProperty(string key, string typeName, string value, int x, int y)
    {
        object oj = null;
        typeName = typeName.Replace("\r", "");
        value = value.Replace("\r", "");

        switch (typeName)
        {
            case "int":
                try
                {
                    oj = int.Parse(value);
                    return true;
                }
                catch
                {
                    Debug.LogError("Match Error :\n Key : " + key + "\nX : " + x + "\nY : " + y + "\nType : " + typeName + "\nValue : " + value);
                    return false;
                }
            case "bool":
                try
                {
                    oj = bool.Parse(value);
                    return true;
                }
                catch
                {
                    Debug.LogError("Match Error :\n Key : " + key + "\nX : " + x + "\nY : " + y + "\nType : " + typeName + "\nValue : " + value);
                    return false;
                }

            case "double":
                try
                {
                    oj = double.Parse(value);
                    return true;
                }
                catch
                {
                    Debug.LogError("Match Error :\n Key : " + key + "\nX : " + x + "\nY : " + y + "\nType : " + typeName + "\nValue : " + value);
                    return false;
                }

            case "long":
                try
                {
                    oj = long.Parse(value);
                    return true;
                }
                catch
                {
                    Debug.LogError("Match Error :\n Key : " + key + "\nX : " + x + "\nY : " + y + "\nType : " + typeName + "\nValue : " + value);
                    return false;
                }

            case "string":
                return true;


            case "bint":
                try
                {
                    oj = System.Numerics.BigInteger.Parse(value);
                    return true;
                }
                catch
                {
                    Debug.LogError("Match Error :\n Key : " + key + "\nX : " + x + "\nY : " + y + "\nType : " + typeName + "\nValue : " + value);
                    return false;
                }

            default:
                try
                {
                    var t = Type.GetType(typeName);
                    oj = System.Enum.Parse(t, value);
                    return true;
                }
                catch
                {
                    Debug.LogError("Match Error :\n Key : " + key + "\nX : " + x + "\nY : " + y + "\nType : " + typeName + "\nValue : " + value);
                    return false;
                }

        }
    }

    private void ButtonSaveJson()
    {
        string path = Application.dataPath + "/" + "Resources/Json";
        for (int i = 0; i < domain.Count; ++i)
        {
            string url = domain.GetURL(i);
            if(string.IsNullOrEmpty(url))
            {
                domain.Remove(i);
                return;
            }

            string csv = UrlDownload(url);
            FileSave(path, domain.SheetNameList[i] + ".json", PaserJson(csv));
            
        }
        domain.Save();

    }

    private void ButtonSaveCs()
    {
        string gbPath = Application.dataPath + "/" + "GB/GameData";
        string classText = Resources.Load<TextAsset>("ExportClass").text; 
        string dataManagerText = Resources.Load<TextAsset>("ExportGameDataManager").text; 

        List<ExData> exDataList = new List<ExData>();
        for (int i = 0; i < domain.Count; ++i)
        {
            string url = domain.GetURL(i);
            if(string.IsNullOrEmpty(url))
            {
                domain.Remove(i);
                return;
            }

            string csv = UrlDownload(url);
            
            var exData = PaserExecelData(domain.SheetNameList[i], csv);
            exDataList.Add(exData);
        }


        for (int i = 0; i < exDataList.Count; ++i)
        {
            string classTemplate = classText;
            classTemplate = classTemplate.Replace("$ClassName$", exDataList[i].SheetName);
            classTemplate = classTemplate.Replace("$FIRSTPROPERTY$", exDataList[i].Rows[0]);


            string caseKey = string.Empty;
            string case1Key = string.Empty;
            string case2Key = string.Empty;
            for (int j = 0; j < exDataList[i].Rows.Count; ++j)
            {

                caseKey = string.Format("{0}\t\t\t\tcase {1}: return data.{2};\n",
                    caseKey,
                    j.ToString(),
                    exDataList[i].Rows[j]
                    );

                case1Key = string.Format("{0}\t\t\t\tcase \"{1}\": return data.{1};\n",
                    case1Key,
                    exDataList[i].Rows[j]);

                case2Key = string.Format("{0}\t\t\t\tcase \"{1}\": return true;\n",
                    case2Key,
                    exDataList[i].Rows[j]);
            }

            classTemplate = classTemplate.Replace("#CASE#", caseKey);
            classTemplate = classTemplate.Replace("#CASE1#", case1Key);
            classTemplate = classTemplate.Replace("#CASE2#", case2Key);
            string pro = string.Empty;

            for (int j = 0; j < exDataList[i].Rows.Count; ++j)
            {
                string tmp = string.Format("\t[JsonProperty] public readonly {0} {1};\n", exDataList[i].RowType[j], exDataList[i].Rows[j]);
                pro = string.Format("{0}{1}", pro, tmp);

            }
            classTemplate = classTemplate.Replace("$DATAS$", pro);

            FileSave(gbPath, exDataList[i].SheetName + ".cs", classTemplate);

        }

        string managerTemplate = dataManagerText;

        string caseTemplate =
@"case TABLE.$CLASS$:
        $CLASS$ d_$CLASS$ = new $CLASS$();
        d_$CLASS$.SetJson(data);
        obj  = d_$CLASS$;
        break;";
        string propertys = string.Empty;

        for (int i = 0; i < exDataList.Count; ++i)
        {
            string p = caseTemplate.Replace("$CLASS$", exDataList[i].SheetName);
            propertys = string.Format("{0}\n{1}", propertys, p);
        }

        managerTemplate = managerTemplate.Replace("$CASE$", propertys);


        propertys = string.Empty;

        for (int i = 0; i < exDataList.Count; ++i)
        {
            propertys = string.Format("{0}\t{1},\n", propertys, exDataList[i].SheetName);
        }

        managerTemplate = managerTemplate.Replace("$DATA$", propertys);

        FileSave(gbPath, "GameDataManager.cs", managerTemplate);
        domain.Save();

        




    }

    public string LoadText(string path)
    {
        if (File.Exists(path) == false)
        {
            return null;
        }
        string data = string.Empty;
        try
        {

            data = File.ReadAllText(path);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Error LoadFile MSG");
            Console.WriteLine(ex.Message);

        }

        return data;
    }


    ExData PaserExecelData(string sheetName, string csv)
    {
        ExData data = new ExData();

        string[] cols = csv.Replace("\r", "").Split('\n');

        for (int y = 0; y < cols.Length; ++y)
        {
            string[] rows = cols[y].Split(',');

            if (string.IsNullOrEmpty(rows[0]))
                continue;


            for (int x = 0; x < rows.Length; ++x)
            {
                if (y == 1 && x >= 0)
                {
                    string tmp = rows[x];
                    data.RowType.Add(tmp);
                }

                //Col Keys
                if (y >= 3 && x == 0)
                {

                    if (string.IsNullOrEmpty(rows[0])) continue;

                    string key = string.Format("{0}", rows[0]);
                    data.Columns.Add(key);
                }

                //Row Keys
                if (y == 0 && x >= 0)
                {
                    if (string.IsNullOrEmpty(rows[x])) continue;

                    string key = rows[x];
                    data.Rows.Add(key);
                }

                if (y >= 3)
                {
                    data.Values.Add(rows[x]);
                }
            }
        }


        data.SheetName = sheetName;

        return data;
    }


    private string PaserJson(string csv)
    {
        string json = string.Empty;
        List<string> rowKeys = new List<string>();
        List<string> columnKeys = new List<string>();
        List<string> typeKeys = new List<string>();
        List<string> values = new List<string>();


        string[] columns = csv.Replace("\r", "").Split("\n");
        string[] rowKeyArr = columns[0].Split(",");
        string[] typeKeyArr = columns[1].Split(",");

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
            string[] rows = columns[y].Split(",");
            if (string.IsNullOrEmpty(rows[0])) continue;

            for (int x = 0; x < rows.Length; ++x)
            {
                if (y >= 3 && x == 0)
                    columnKeys.Add(rows[x]);

                if (y >= 3 && x >= 1)
                    values.Add(rows[x]);
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


    private string UrlDownload(string url)
    {
        
        if(url == null)
        {
            return null;
        } 


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
}

[System.Serializable]
public class GSheetDomain
{
    public List<string> SheetNameList = new List<string>();
    public List<string> UrlList = new List<string>();
    
    public int GetIndex(string sheetName)
    {
        for (int i = 0; i < SheetNameList.Count; ++i)
        {
            if (sheetName == SheetNameList[i])
                return i;
        }
        return -1;
    }

    public void SetJson(string json)
    {
        var data = JsonConvert.DeserializeObject<GSheetDomain>(json);
        UrlList = data.UrlList;
        SheetNameList = data.SheetNameList;
    }

    public void Add(string sheetName, string Url)
    {
        SheetNameList.Add(sheetName);
        UrlList.Add(Url);
        
    }

    public void Remove(int index)
    {
        SheetNameList.RemoveAt(index);
        UrlList.RemoveAt(index);
        
        Save();
    }

    

    public string GetURL(int index)
    {
        string url = UrlList[index];

        if (string.IsNullOrEmpty(url))
            return null;

        try
        {
        
            var arr = url.Split("/");
            var f = arr[6].Split("gid=");

            string urlID = arr[5];
            string gid = Regex.Replace(f[1], @"\D", "");

            string URL_SHEET = $"https://docs.google.com/spreadsheets/d/$URL_ID$/export?format=csv&gid=$GID$";
            return URL_SHEET.Replace("$URL_ID$", urlID).Replace("$GID$", gid);
        }
        catch
        {
            Debug.LogError("URL Error : "+ url);

            return null;
        }

    }

    public string ToJson()
    {
        GSheetDomain gsheet = new GSheetDomain();
        gsheet.UrlList = UrlList;
        gsheet.SheetNameList = SheetNameList;
        return JsonConvert.SerializeObject(gsheet);
    }



    public void Save()
    {

        string json = ToJson();


        EditorPrefs.SetString("GSheetDomain", json);

    }

    public void Load()
    {
        string json = EditorPrefs.GetString("GSheetDomain", null);

        if (string.IsNullOrEmpty(json) == false)
            SetJson(json);

    }


    public int Count
    {
        get
        {
            return UrlList.Count;
        }
    }

}



[Serializable]
public class ExData
{
    public string SheetName;
    public List<string> RowType = new List<string>();
    public List<string> Values = new List<string>();

    public List<string> Columns = new List<string>();
    public List<string> Rows = new List<string>();

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

#endif