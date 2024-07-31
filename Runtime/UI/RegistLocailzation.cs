
using UnityEngine;

namespace GB.UI
{
    public class RegistLocailzation : UIRegister
    {
        public string Key;

        [Header("Ό³Έν")]
        [TextArea] public string Infomation;


        public override void SetBind()
        {
            var screen = GetScreen();

            if (screen != null)
                screen.Add(Key, GetComponent<LocalizationView>());
        }
    }
}
