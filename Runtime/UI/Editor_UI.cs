
#if UNITY_EDITOR
using UnityEngine;

using UnityEditor;
public class Editor_UI : EditorWindow
{
    [MenuItem("GB/UI/Create Canvas")]
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
                Selection.activeGameObject = oj;
            }
            
        }

        Debug.Log("Create Canvas");
    }

    [MenuItem("GB/UI/Create Scene Panel")]
    static void Scene()
    {
        var ui = GameObject.Find("UIScreen");

        if(ui != null)
        {
            GameObject prefab = Resources.Load<GameObject>("Screen");
            if(prefab != null)
            {
                var oj = Instantiate(prefab,ui.transform);
                oj.name = "Scene";

                Selection.activeGameObject = oj;
            }
            else
            {

                Debug.Log("Screen null");
            }

            
        }
        else
        {
            init();
        }

        Debug.Log("Create Scene Panel");
    }

      [MenuItem("GB/UI/Create Popup Panel")]
    static void Popup()
    {
        var ui = GameObject.Find("UIPopup");

        if(ui != null)
        {
            GameObject prefab = Resources.Load<GameObject>("Screen");
            if(prefab != null)
            {
                var oj = Instantiate(prefab,ui.transform);
                oj.name = "Popup";
                Selection.activeGameObject = oj;
            }
            else
            {

                Debug.Log("Screen null");
            }

            
        }
        else
        {
            init();
        }

        Debug.Log("Create Popup Panel");
    }



}
#endif