using Bsr.CharacterController.Parameters.VisualScripting.Icons;
using UnityEngine;

namespace Bsr.CharacterController.Parameters
{
    [Unity.VisualScripting.TypeIcon(typeof(FloatIcon))]
    [Unity.VisualScripting.TypeOptionsAdd]
    public class ParameterFloat : Parameter<float>
    {
        protected override bool CheckIfChanged(float oldValue, float newValue)
        {
            return !Mathf.Approximately(oldValue, newValue);
        }

        public override string TextId => "float";
        public static implicit operator float(ParameterFloat parameter) => parameter.Value;
    }
}