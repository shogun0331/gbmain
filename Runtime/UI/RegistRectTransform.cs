using UnityEngine;

namespace GB
{
    public class RegistRectTransform : UIRegister
    {
        public string Key;

        [Header("����")]
        [TextArea] public string Infomation;


        public override void SetBind()
        {
            var screen = GetScreen();

            if (screen != null)
                screen.Add(Key, GetComponent<RectTransform>());
        }
    }
}