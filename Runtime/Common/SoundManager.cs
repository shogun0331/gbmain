using UnityEngine;
using NaughtyAttributes;
using QuickEye.Utility;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
namespace GB
{
    public class SoundManager : AutoSingleton<SoundManager>
    {
        [SerializeField] UnityDictionary<string, AudioClip> _DictEffectClip = new UnityDictionary<string, AudioClip>();
        [SerializeField] UnityDictionary<string, AudioClip> _DictBgClip = new UnityDictionary<string, AudioClip>();

        [SerializeField] AudioSource _EffAudioSource;
        [SerializeField] AudioSource _BgAudioSource;


        bool _isMute
        {
            get
            {
                return PlayerPrefs.GetInt("AudioMute", 1) == 0;
            }
            set
            {
                PlayerPrefs.SetInt("AudioMute", value == true ? 1 : 0);
            }

        }

        public static bool IsMute
        {
            get
            {
                return I._isMute;
            }
        }

        private void Awake()
        {
            if (I != this)
            {
                Destroy(this.gameObject);
                return;
            }


            DontDestroyOnLoad(this.gameObject);

            if (_EffAudioSource == null || _BgAudioSource == null)
            {
                var sources = GetComponents<AudioSource>();

                if (sources.Length >= 2)
                {
                    _EffAudioSource = sources[0];
                    _BgAudioSource = sources[1];
                }
                else if (sources.Length == 1)
                {
                    _EffAudioSource = sources[0];
                    _BgAudioSource = gameObject.AddComponent<AudioSource>();
                }
                else
                {
                    _EffAudioSource = gameObject.AddComponent<AudioSource>();
                    _BgAudioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            if (_DictBgClip.Count == 0 && _DictEffectClip.Count == 0)
                Load();

            SetMute(_isMute);

        }

        [Button]
        private void Load()
        {
#if UNITY_EDITOR

            if (_EffAudioSource == null || _BgAudioSource == null)
            {
                var sources = GetComponents<AudioSource>();

                if (sources.Length >= 2)
                {
                    _EffAudioSource = sources[0];
                    _BgAudioSource = sources[1];

                    for (int i = 2; i < sources.Length; ++i)
                        DestroyImmediate(sources[i]);

                }
                else if (sources.Length == 1)
                {
                    _EffAudioSource = sources[0];
                    _BgAudioSource = gameObject.AddComponent<AudioSource>();
                }
                else
                {
                    _EffAudioSource = gameObject.AddComponent<AudioSource>();
                    _BgAudioSource = gameObject.AddComponent<AudioSource>();
                }
            }



            string path = Application.dataPath + "/Resources/Sounds/Effect";

            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists == false)
            {
                info.Create();
            }

            path = Application.dataPath + "/Resources/Sounds/BG";

            info = new DirectoryInfo(path);
            if (info.Exists == false)
            {
                info.Create();
                AssetDatabase.Refresh();
                return;
            }


#endif

            var effectClips = Resources.LoadAll<AudioClip>("Sounds/Effect");
            _DictEffectClip.Clear();
            for (int i = 0; i < effectClips.Length; ++i)
                _DictEffectClip[effectClips[i].name] = effectClips[i];

            var bgClips = Resources.LoadAll<AudioClip>("Sounds/BG");
            _DictBgClip.Clear();
            for (int i = 0; i < bgClips.Length; ++i)
                _DictBgClip[bgClips[i].name] = bgClips[i];
        }

        public static void SetMute(bool isMute)
        {
            I._EffAudioSource.mute = isMute;
            I._BgAudioSource.mute = isMute;

            I._isMute = isMute;
        }

        public static void SetMuteEffect(bool isMute)
        {
            I._EffAudioSource.mute = isMute;
        }
        public static void SetMuteBg(bool isMute)
        {
            I._BgAudioSource.mute = isMute;
        }

        public static void SetVolume(float volume)
        {
            I._BgAudioSource.volume = volume;
            I._EffAudioSource.volume = volume;
        }

        public static void SetVolumeEffect(float volume)
        {
            I._EffAudioSource.volume = volume;
        }

        public static void SetVolumeBg(float volume)
        {
            I._BgAudioSource.volume = volume;
        }


        private float _BgVolume = 1;
        private float _EffVolume = 1;

        public static void Pasue()
        {
            I._EffVolume = I._EffAudioSource.volume;
            I._BgVolume = I._BgAudioSource.volume;

            I._BgAudioSource.volume = 0;
            I._EffAudioSource.volume = 0;
        }

        public static void Resume()
        {
            I._BgAudioSource.volume = I._BgVolume;
            I._EffAudioSource.volume = I._EffVolume;

        }

        
        public static void Play(string id, float volume)
        {
            
            if (I._DictEffectClip.ContainsKey(id) == false) return;
            

            I._BgAudioSource.volume = volume;
            I._EffAudioSource.PlayOneShot(I._DictEffectClip[id]);
            
            

        }

        public static void PlayBG(string id,float volume,  bool isLoop = true)
        {
            if (I._DictBgClip.ContainsKey(id) == false) return;
            I._BgAudioSource.loop = isLoop;
            I._BgAudioSource.volume = volume;
            I._BgAudioSource.clip = I._DictBgClip[id];
            I._BgAudioSource.Play();
        }
    }
}
