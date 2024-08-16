
#if UNITY_EDITOR
using UnityEngine;

using UnityEditor;
public class Editor_UI : EditorWindow
{
    [MenuItem("Tools/GB/UI")]
    static void init()
    {
        var ui = GameObject.Find("UIPopup");

        if(ui == null)
        {
            GameObject prefab = Resources.Load<GameObject>("UI");
            if(prefab != null)
            {
                var oj = Instantiate(prefab,null);
                oj.name = "UI";
            }
        }

        
    }
}
#endif