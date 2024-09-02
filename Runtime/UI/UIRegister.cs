
using UnityEngine;

namespace GB
{

    public abstract class UIRegister : MonoBehaviour
    {
        public UIScreen GetScreen()
        {
            return UiUtil.FindUIScreen(transform);
        }

        public abstract void SetBind();
       

    }

}
