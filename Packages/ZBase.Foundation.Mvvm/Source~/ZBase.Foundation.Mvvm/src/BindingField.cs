﻿using System;

namespace ZBase.Foundation.Mvvm
{
    [Serializable]
    public sealed class BindingField
    {
        /// <summary>
        /// Label for the binding field
        /// </summary>
#if UNITY_5_3_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public string Label { get; set; }

        /// <summary>
        /// Property or method name bound to
        /// </summary>
#if UNITY_5_3_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public string Member { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class BindingFieldAttribute : Attribute
    {
        public string Label { get; set; }
    }
}