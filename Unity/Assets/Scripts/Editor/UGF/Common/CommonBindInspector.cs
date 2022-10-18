using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace UGF.Editor
{
    [CustomEditor(typeof (CommonBind), true)]
    public class CommonBindInspector: BaseBindInspector
    {
        protected override List<(string, Type)> DefaultBindTypeList => this.m_DefaultBindTypeList;
        protected override List<(string, Type)> CustomBindTypeList => this.m_CustomBindTypeList;

        private readonly List<(string, Type)> m_DefaultBindTypeList = new()
        {
            ("Transform", typeof (Transform)),
            ("Animation", typeof (Animation)),
            ("Animator", typeof (Animator)),
            ("SpriteRenderer", typeof (SpriteRenderer)),
            ("TextMeshPro", typeof (TextMeshPro)),
        };

        private readonly List<(string, Type)> m_CustomBindTypeList = new()
        {
            ("Bind", typeof (CommonBind)),
        };
    }
}