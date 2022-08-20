using Bsr.CharacterController.Parameters.VisualScripting.Icons;

namespace Bsr.CharacterController.Parameters
{
    [Unity.VisualScripting.TypeIcon(typeof(BoolIcon))]
    [Unity.VisualScripting.TypeOptionsAdd]
    public class ParameterBool : Parameter<bool>
    {
        protected override bool CheckIfChanged(bool oldValue, bool newValue)
        {
            return oldValue != newValue;
        }

        public override string TextId => "bool";
        public static implicit operator bool(ParameterBool parameter) => parameter.Value;
    }
}