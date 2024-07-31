using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickEye.Utility;
using NaughtyAttributes;

namespace GB
{
    //[CreateAssetMenu(fileName = "InputModel", menuName = "Input/InputModel", order = 0)]
    public class InputModel : ScriptableObject
    {
        public UnityDictionary<string, KeyAttribute> KeysCodeModels = new UnityDictionary<string, KeyAttribute>();

        [HorizontalLine(color: EColor.Red)]
        [Header("GamePad")]
        public bool IsDirectionPad;

        [EnableIf("IsDirectionPad")]
        [ShowAssetPreview]
        public Sprite DirectionPadBG;

        [EnableIf("IsDirectionPad")]
        [ShowAssetPreview]
        public Sprite DirectionPadCtr;

        [Header("World")]
        public bool IsWorldMode;


        [Header("Viewport")]
        public bool IsUIMode;
        //[HorizontalLine(color: EColor.Blue)]
        //[Header("Key Code Input")]
        //[SerializeField] string _KeyCode;
        //[SerializeField] KeyAttribute _KeyAttribute;

        //[Button]
        //void AddKeyCode()
        //{
        //    if (_KeyAttribute == null) return;
        //    KeysCodeModels[StringToEnum(_KeyCode)] = _KeyAttribute;
        //}


        private KeyCode StringToEnum(string e)
        {
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), e);
        }
    }

}