using Bsr.CharacterController.Parameters.VisualScripting.Icons;
using Unity.VisualScripting;

namespace Bsr.CharacterController.Parameters.VisualScripting
{
    [UnitTitle("Modify Float")]
    [TypeIcon(typeof(FloatIcon))]
    [UnitCategory("Parameters")]
    public class ParameterFloatUnit : Unit
    {
        [DoNotSerialize, PortLabelHidden] public ControlOutput exit;

        [DoNotSerialize] public ControlInput set;
        [DoNotSerialize] public ControlInput add;
        [DoNotSerialize] public ControlInput multiply;
        [DoNotSerialize] public ControlInput reset;

        [DoNotSerialize] public ValueInput parameter;
        [DoNotSerialize] public ValueInput input;

        protected override void Definition()
        {
            exit = ControlOutput(nameof(exit));

            set = ControlInput(nameof(set), OnSet);
            add = ControlInput(nameof(add), OnAdd);
            multiply = ControlInput(nameof(multiply), OnMultiply);
            reset = ControlInput(nameof(reset), OnReset);

            parameter = ValueInput<ParameterFloat>(nameof(parameter));

            input = ValueInput<float>(nameof(input));

            Succession(set, exit);
            Succession(add, exit);
            Succession(multiply, exit);
            Succession(reset, exit);

            Requirement(input, set);
            Requirement(input, add);
            Requirement(input, multiply);
            Requirement(parameter, set);
            Requirement(parameter, add);
            Requirement(parameter, multiply);
            Requirement(parameter, reset);
        }

        private ControlOutput OnSet(Flow flow)
        {
            flow.GetValue<ParameterFloat>(parameter).Value = flow.GetValue<float>(input);
            return exit;
        }

        private ControlOutput OnAdd(Flow flow)
        {
            flow.GetValue<ParameterFloat>(parameter).Value += flow.GetValue<float>(input);
            return exit;
        }

        private ControlOutput OnMultiply(Flow flow)
        {
            flow.GetValue<ParameterFloat>(parameter).Value *= flow.GetValue<float>(input);
            return exit;
        }

        private ControlOutput OnReset(Flow flow)
        {
            flow.GetValue<ParameterFloat>(parameter).ResetValue();
            return exit;
        }
    }
}