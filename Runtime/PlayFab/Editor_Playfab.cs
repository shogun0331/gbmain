#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
public class Editor_Playfab : EditorWindow
{

    static bool _isPlayFabOn
    {
        get
        {
            return EditorPrefs.GetBool("IsPlayFab", false);
        }
        set
        {
            EditorPrefs.SetBool("IsPlayFab", value);
        }

    }

    [MenuItem("Tools/GB/PlayFab")]

    static void init()
    {
        EditorWindow.GetWindow(typeof(Editor_Playfab));
    }

    private void OnGUI()
    {
        bool isOn  = GUILayout.Toggle(_isPlayFabOn, "Is PlayFab On");
        if (isOn != _isPlayFabOn)
        {
            Debug.Log("_isPlayFabOn");
            _isPlayFabOn = isOn;
            
            if (_isPlayFabOn)
            {
                if (DefineSymbolManager.IsSymbolAlreadyDefined("GB_PLAYFAB") == false)
                    DefineSymbolManager.AddDefineSymbol("GB_PLAYFAB");
               
            }
            else
            {
                if (DefineSymbolManager.IsSymbolAlreadyDefined("GB_PLAYFAB") == true)
                    DefineSymbolManager.RemoveDefineSymbol("GB_PLAYFAB");
            }

        }

    }

}

#endif
