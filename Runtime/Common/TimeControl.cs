
#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace GB.Edit
{
    public class TimeControl
    {
        private double m_lastFrameEditorTime = 0;
        public float currentTime;
        public bool isPlaying { get; private set; }
        public float speed = 1;
        public Action func;

        public TimeControl()
        {
            EditorApplication.update += TimeUpdate;
        }
        public void Play()
        {
            isPlaying = true;
            m_lastFrameEditorTime = EditorApplication.timeSinceStartup;
        }
        public void Stop()
        {
            isPlaying = false;
            currentTime = 0;
        }
        public void TimeUpdate()
        {
            if (isPlaying)
            {
                var timeSinceStartup = EditorApplication.timeSinceStartup;
                var deltaTime = timeSinceStartup - m_lastFrameEditorTime;
                m_lastFrameEditorTime = timeSinceStartup;
                currentTime += (float)deltaTime * speed * 2;
                func?.Invoke();
            }
        }
    }
}

#endif