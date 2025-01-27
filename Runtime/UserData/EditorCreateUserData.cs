#if UNITY_EDITOR  
#if !GB_USERDATA

using UnityEngine;
using UnityEditor;
using System.IO;

namespace GB
{
    public class EditorCreateUserData : EditorWindow
    {
        [MenuItem("GB/Core/Create UserData")]
        static void init()
        {
            CreateUserData();
            DefineSymbolManager.AddDefineSymbol(Symbol);
        }

        
       static void CreateUserData()
        {
            string diPath = Application.dataPath + "/Scripts/UserData";
            DirectoryInfo info = new DirectoryInfo(diPath);
            if (info.Exists == false) info.Create();

            File.WriteAllText(diPath + "/UserData.cs", USER_DATA_CLASS_FORME);

            string classText = Resources.Load<TextAsset>("ExportDataManagerClass").text;
            File.WriteAllText(diPath + "/UserDataManager.cs", classText);

            classText = Resources.Load<TextAsset>("ExportEditorUserDataClass").text;
            File.WriteAllText(diPath + "/EditorUserData.cs", classText);

            AssetDatabase.Refresh();
        }


        const string Symbol = "GB_USERDATA";
        const string USER_DATA_CLASS_FORME = @"
using System;
using Newtonsoft.Json;

[Serializable]
public class UserData 
{
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
    ";
        const string DATA_MANAGER_FORME = @"
        


        ";




    }
}
#endif
#endif
