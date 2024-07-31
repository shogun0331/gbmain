using UnityEngine;

namespace GB.UI
{
    public class RegistSkinner : UIRegister
    {
        public string Key;

        [SerializeField] UISkinner _skinner;

        [Header("Ό³Έν")]
        [TextArea] public string Infomation;
        public override void SetBind()
        {
            var screen = GetScreen();

            if (screen != null)
            {
                if (_skinner != null)
                {
                    screen.Add(Key, _skinner);
                }
                else
                {

                    screen.Add(Key, GetComponent<UISkinner>());
                }
            }
        }

    }
}