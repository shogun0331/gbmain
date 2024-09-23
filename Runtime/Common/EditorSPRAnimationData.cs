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
            
            
            //target is by default available for you
            //because we inherite Editor
            //    weapon = target as Weapon;
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


            GUILayout.Space(200);

            GUILayout.BeginHorizontal();
            GUILayout.Label( CurIDX + 1 + "/" + t.SpriteCount);
            GUILayout.EndHorizontal();

            if (t.SpriteCount > 0)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Stop"))
                {
                    // if(CurIDX > 0)
                    // CurIDX --;
                    Stop();
                              
                
                }

                if (GUILayout.Button("Play"))
                {
                    Play();

                    // if(CurIDX < t.SpriteCount-1)
                    // CurIDX ++;
                }
                

                GUILayout.EndHorizontal();
                
                UpdatePreview();

                if(timeControl.isPlaying == false)
                {
                    Draw(t.GetSprite(CurIDX));
                }

            }
        }


        float _fixTimer;
        const float TIMER = 0.05f;
        
        public void Play()
        {
            var t = (SPRAnimationData)target;
            _fixTimer = TIMER / t.Speed ;
            CurIDX = 0;
            timeControl.Play();
        }

        public void Stop()
        {
            
            timeControl.Stop();
        }

        public void UpdatePreview()
        {
            if(timeControl.isPlaying == false) return;

            var t = (SPRAnimationData)target;
            if(timeControl.currentTime > _fixTimer)
            {
                timeControl.currentTime = 0;
                CurIDX++;
                if(CurIDX >= t.SpriteCount )
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