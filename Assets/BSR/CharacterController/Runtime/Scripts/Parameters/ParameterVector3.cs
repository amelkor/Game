using Bsr.CharacterController.Parameters.VisualScripting.Icons;
using UnityEngine;

namespace Bsr.CharacterController.Parameters
{
    [Unity.VisualScripting.TypeIcon(typeof(Vector3Icon))]
    [Unity.VisualScripting.TypeOptionsAdd]
    public class ParameterVector3 : Parameter<Vector3>
    {
        protected override bool CheckIfChanged(Vector3 oldValue, Vector3 newValue)
        {
            return oldValue != newValue;
        }

        public override string TextId => "Vector3";
        
        public static implicit operator Vector3(ParameterVector3 parameter) => parameter.Value;
    }
}