using Bsr.CharacterController.Parameters.VisualScripting.Icons;
using Unity.VisualScripting;

namespace Bsr.CharacterController.Parameters.VisualScripting
{
    [UnitTitle("Modify Boolean")]
    [TypeIcon(typeof(BoolIcon))]
    [UnitCategory("Parameters")]
    public class ParameterBoolUnit : Unit
    {
        [DoNotSerialize, PortLabelHidden] public ControlOutput exit;

        [DoNotSerialize] public ControlInput set;
        [DoNotSerialize] public ControlInput negate;
        [DoNotSerialize] public ControlInput reset;

        [DoNotSerialize] public ValueInput parameter;
        [DoNotSerialize] public ValueInput input;

        protected override void Definition()
        {
            exit = ControlOutput(nameof(exit));

            set = ControlInput(nameof(set), OnSet);
            negate = ControlInput(nameof(negate), OnNegate);
            reset = ControlInput(nameof(reset), OnReset);

            parameter = ValueInput<ParameterBool>(nameof(parameter));
            input = ValueInput<bool>(nameof(input));
            
            Succession(set, exit);
            Succession(negate, exit);
            Succession(reset, exit);
            
            Requirement(input, set);
            Requirement(parameter, set);
            Requirement(parameter, negate);
            Requirement(parameter, reset);
        }

        private ControlOutput OnSet(Flow flow)
        {
            flow.GetValue<ParameterBool>(parameter).Value = flow.GetValue<bool>(input);
            return exit;
        }

        private ControlOutput OnNegate(Flow flow)
        {
            var p = flow.GetValue<ParameterBool>(parameter);
            var value = p.Value;
            p.Value = !value;
            return exit;
        }
        private ControlOutput OnReset(Flow flow)
        {
            flow.GetValue<ParameterBool>(parameter).ResetValue();
            return exit;
        }
    }
}