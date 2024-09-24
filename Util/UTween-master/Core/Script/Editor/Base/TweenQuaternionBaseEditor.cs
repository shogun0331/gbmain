﻿/////////////////////////////////////////////////////////////////////////////
//
//  Script   : TweenQuaternionBaseEditor.cs
//  Info     : TweenQuaternionBase 编辑器
//  Author   : ls9512 2019
//  E-mail   : ls9512@vip.qq.com
//
/////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Aya.Tween
{
    public abstract class TweenQuaternionBaseEditor : TweenerTVCEditor
    {
        public new Tweener<Quaternion> Tweener => Target as Tweener<Quaternion>;

        public override void DoDrawValue()
        {
            base.DoDrawValue();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(FromQuaternionProperty, new GUIContent("From"), true);
            EditorGUILayout.PropertyField(ToQuaternionProperty, new GUIContent("To"), true);
            EditorGUILayout.EndVertical();
        }

        public override bool DoDrawCallback()
        {
            base.DoDrawCallback();
            if (!IsCallbackOpen) return false;

            EditorGUILayout.PropertyField(OnValueQuaternionProperty);
            return true;
        }
    }
}
#endif
