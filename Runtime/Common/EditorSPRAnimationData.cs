using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using GB.Edit;

namespace GB
{

    [CustomEditor(typeof(SPRAnimationData))]
    public class EditorSPRAnimationData : Editor
    {

        public int CurIDX;

        public bool IsPlaying;

        TimeControl timeControl;


        private void OnEnable()
        {
            timeControl = new TimeControl();

            CurIDX = 0;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            var t = (SPRAnimationData)target;

            GUILayout.BeginVertical();

            if (GUILayout.Button("LoadSprite"))
            {
                t.LoadSprite();
            }

            if (GUILayout.Button("LoadAtlas"))
            {
                t.LoadAtlas();
            }

            GUILayout.EndVertical();



            if (!t.IsAtlas && t.SpriteCount > 0)
            {
                GUILayout.Space(200);


                Rect r = EditorGUILayout.BeginVertical();
                GUILayout.Space(15);


                if (CurIDX > 0)
                    _hSliderValue = ((float)(CurIDX + 1) / (float)t.SpriteCount);
                else
                    _hSliderValue = 0;



                //r.height = r.height* 0.5f;
                EditorGUI.ProgressBar(r, _hSliderValue, CurIDX + 1 + "/" + t.SpriteCount);

                EditorGUILayout.EndVertical();


                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Play"))
                {

                    Play();


                }

                if (GUILayout.Button("Stop"))
                {
                    Stop();

                }


                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<<"))
                {
                    if (CurIDX > 0)
                        CurIDX--;
                }

                if (GUILayout.Button(">>"))
                {
                    if (CurIDX < t.SpriteCount - 1)
                        CurIDX++;
                }


                GUILayout.EndHorizontal();

                UpdatePreview();

                if (timeControl.isPlaying == false)
                {
                    Draw(t.GetSprite(CurIDX));
                }

            }
        }

        float _hSliderValue;
        float _fixTimer;
        const float TIMER = 0.05f;

        public void Play()
        {
            var t = (SPRAnimationData)target;
            _fixTimer = TIMER / t.Speed;
            CurIDX = 0;
            timeControl.Play();
        }

        public void Stop()
        {

            timeControl.Stop();
        }


        public void UpdatePreview()
        {
            if (timeControl.isPlaying == false) return;

            var t = (SPRAnimationData)target;

            if (timeControl.currentTime > _fixTimer)
            {

                timeControl.currentTime = 0;
                CurIDX++;
                if (CurIDX >= t.SpriteCount)
                {
                    CurIDX = 0;
                }

            }

            Draw(t.GetSprite(CurIDX));


        }


        void Draw(Sprite spr)
        {
            if (spr == null) return;

            var rect = GUILayoutUtility.GetLastRect();
            rect.y = rect.y + 70;
            rect.x = rect.x + 70;

            rect.width = 100;
            rect.height = 100;

            GUI.DrawTexture(rect, AssetPreview.GetAssetPreview(spr));
            Repaint();

        }

    }

}

#endif