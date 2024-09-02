#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;


namespace GB
{

    public class PresenterTracker : EditorWindow
    {

        [MenuItem("GB/Core/PresenterTracker")]
        static void init()
        {
            EditorWindow.GetWindow(typeof(PresenterTracker));
        }


        private void OnGUI()
        {
            GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
            GUIStyle style = new GUIStyle();

       

            if (Application.isPlaying == false)
            {
                GUILayout.Space(30);
                EditorGUILayout.BeginHorizontal();
                DrawLabel("Can only be tracked during play.", 40, Color.red, TextAnchor.MiddleCenter);
                EditorGUILayout.EndHorizontal();
                
                return;
            }

            ViewBindsDraw();
            
        }


        Vector2 _viewScrollPos = Vector2.zero;

        private void ViewBindsDraw()
        {
          
            if (Presenter.I.Views == null) return;

            GUILayout.Space(30);
            _viewScrollPos = GUILayout.BeginScrollView(_viewScrollPos);
            foreach (var v in Presenter.I.Views)
            {
                EditorGUILayout.BeginHorizontal();
                DrawLabel(v.Key, 15, Color.green, TextAnchor.MiddleLeft, FontStyle.Bold);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);

                for (int i = 0; i < v.Value.Count; ++i)
                {
                    var obj = v.Value[i];
                    if (obj != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        DrawLabel(obj.gameObject.name, 10, Color.white, TextAnchor.MiddleCenter);
                        if (GUILayout.Button("Tracker"))
                        {
                            Selection.activeGameObject = obj.gameObject;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                GUILayout.Space(30);
            }

            GUILayout.EndScrollView();

        }

   

        private void DrawLabel(string value, int fontSize, Color color, TextAnchor anchor = TextAnchor.MiddleLeft, FontStyle fStyle = FontStyle.Normal)
        {
            GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
            GUIStyle style = new GUIStyle();
            style.fontSize = fontSize;
            style.normal.textColor = color;
            style.alignment = anchor;
            
            style.fontStyle = fStyle;
            GUILayout.Label(value, style);
        }
    }
}
#endif
