using UnityEngine;
using UnityEngine.Events;

namespace Bsr.CharacterController.Parameters
{
    public abstract class Parameter<T> : ParameterBase
    {
        [SerializeField] private T defaultValue;

        public event UnityAction<T> ValueChanged;

        private T _value;
        private bool _isModified;

        public T Value
        {
            get => _isModified ? _value : defaultValue;
            set
            {
                var oldValue = Value;
                _value = value;
                _isModified = true;

                if (CheckIfChanged(oldValue, Value))
                    ValueChanged?.Invoke(Value);
            }
        }

        public void ResetValue()
        {
            _isModified = false;

            if (CheckIfChanged(_value, defaultValue))
                ValueChanged?.Invoke(defaultValue);
        }

        protected abstract bool CheckIfChanged(T oldValue, T newValue);

        public override string GetStringValue => Value.ToString();
    }
}