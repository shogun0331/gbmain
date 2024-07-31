using UnityEngine;
using UnityEngine.UI;

namespace GB.UI
{
    public class RegistText : UIRegister
    {
        public string Key;
        [Header("설명")]
        [TextArea] public string Infomation;

        public override void SetBind()
        {
            

            var screen = GetScreen();

            if (screen != null)
                screen.Add(Key, GetComponent<Text>());
        }
    }

}
