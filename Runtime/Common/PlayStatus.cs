using UnityEngine;

namespace GB
{
    public static class PlayStatus
    {
        public static bool IsPlaying { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            IsPlaying = true;
            Application.quitting += () => IsPlaying = false;
        }
    }
}
