using Bsr.CharacterController.Parameters.VisualScripting.Icons;

namespace Bsr.CharacterController.Parameters
{
    [Unity.VisualScripting.TypeIcon(typeof(IntIcon))]
    [Unity.VisualScripting.TypeOptionsAdd]
    public class ParameterInt : Parameter<int>
    {
        protected override bool CheckIfChanged(int oldValue, int newValue)
        {
            return oldValue != newValue;
        }

        public override string TextId => "int";
        public static implicit operator int(ParameterInt parameter) => parameter.Value;
    }
}