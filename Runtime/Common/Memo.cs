using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace GB
{
    public class Memo : MonoBehaviour
    {
        [ResizableTextArea]
        public string Content;       
    }

}
