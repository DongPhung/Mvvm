using System;
using UnityEngine;
using UnityEngine.UI;
using ZBase.Foundation.Mvvm.ViewBinding;

namespace ZBase.Foundation.Mvvm.Unity.ViewBinding.Binders
{
    [AddComponentMenu("MVVM/Binders/Slider Binder")]
    public partial class SliderBinder : MonoBinder
    {
        [SerializeField]
        private Slider[] _targets = new Slider[0];

        protected override void OnAwake()
        {
            if (_targets.Length < 1)
            {
                if (this.gameObject.TryGetComponent<Slider>(out var target))
                {
                    _targets = new Slider[] { target };
                }
            }

            if (_targets.Length < 1)
            {
                Debug.LogWarning($"The target list is empty.", this);
            }
        }

        [Binding]
        [field: Label("Min Value")]
        [field: HideInInspector]
        private void SetMinValue(int value)
        {
            var targets = _targets.AsSpan();
            var length = targets.Length;

            for (var i = 0; i < length; i++)
            {
                var target = targets[i];

                if (target)
                {
                    target.minValue = value;
                }
            }
        }

        [Binding]
        [field: Label("Max Value")]
        [field: HideInInspector]
        private void SetMaxValue(int value)
        {
            var targets = _targets.AsSpan();
            var length = targets.Length;

            for (var i = 0; i < length; i++)
            {
                var target = targets[i];

                if (target)
                {
                    target.maxValue = value;
                }
            }
        }

        [Binding]
        [field: Label("Whole Numbers")]
        [field: HideInInspector]
        private void SetWholeNumbers(bool value)
        {
            var targets = _targets.AsSpan();
            var length = targets.Length;

            for (var i = 0; i < length; i++)
            {
                var target = targets[i];

                if (target)
                {
                    target.wholeNumbers = value;
                }
            }
        }

        [Binding]
        [field: Label("Value")]
        [field: HideInInspector]
        private void SetValue(float value)
        {
            var targets = _targets.AsSpan();
            var length = targets.Length;

            for (var i = 0; i < length; i++)
            {
                var target = targets[i];

                if (target)
                {
                    target.value = value;
                }
            }
        }
    }
}