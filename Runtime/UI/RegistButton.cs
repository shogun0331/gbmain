using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace GB.UI
{
    public class RegistButton : UIRegister
    {
        public string Key;
        [Header("설명")]
        [TextArea] public string Infomation;

        public override void SetBind()
        {

            var btn = GetComponent<Button>();
            if(btn == null)
            {
                UnityEngine.Debug.LogWarning("None Button");
                return;
            }

            var screen = GetScreen();

         

            if (screen != null)
            {
                screen.Add(Key, btn);

            }
        }
    }

}
