using Bsr.CharacterController.Parameters.VisualScripting.Icons;
using UnityEngine;

namespace Bsr.CharacterController.Parameters
{
    [Unity.VisualScripting.TypeIcon(typeof(TransformIcon))]
    [Unity.VisualScripting.TypeOptionsAdd]
    public class ParameterTransform : Parameter<Transform>
    {
        protected override bool CheckIfChanged(Transform oldValue, Transform newValue)
        {
            return oldValue != newValue;
        }

        public override string TextId => "Transform";
        public static implicit operator Transform(ParameterTransform parameter) => parameter.Value;
    }
}