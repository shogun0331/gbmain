
using UnityEngine;

namespace GB
{
    public class RegistLocailzation : UIRegister
    {
        public string Key;

        
        [TextArea] public string Infomation;


        public override void SetBind()
        {
            var screen = GetScreen();

            if (screen != null)
                screen.Add(Key, GetComponent<LocalizationView>());
        }
    }
}
