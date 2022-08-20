using Bsr.CharacterController.Parameters.VisualScripting.Icons;
using Unity.VisualScripting;

namespace Bsr.CharacterController.Parameters.VisualScripting
{
    [UnitTitle("Modify Semaphore")]
    [TypeIcon(typeof(SemaphoreIcon))]
    [UnitCategory("Parameters")]
    public class ParameterSemaphoreUnit : Unit
    {
        [DoNotSerialize, PortLabelHidden] public ControlOutput exit;
        
        [DoNotSerialize] public ControlInput addLock;
        [DoNotSerialize] public ControlInput removeLock;
        [DoNotSerialize] public ControlInput reset;

        [DoNotSerialize] public ValueInput parameter;

        protected override void Definition()
        {
            exit = ControlOutput(nameof(exit));

            addLock = ControlInput(nameof(addLock), OnAddLock);
            removeLock = ControlInput(nameof(removeLock), OnRemoveLock);
            reset = ControlInput(nameof(reset), OnReset);

            parameter = ValueInput<ParameterSemaphore>(nameof(parameter));
            
            Succession(addLock, exit);
            Succession(removeLock, exit);
            Succession(reset, exit);
            
            Requirement(parameter, addLock);
            Requirement(parameter, removeLock);
            Requirement(parameter, reset);
        }

        private ControlOutput OnAddLock(Flow flow)
        {
            flow.GetValue<ParameterSemaphore>(parameter).Value.AddLock();
            return exit;
        }

        private ControlOutput OnRemoveLock(Flow flow)
        {
            flow.GetValue<ParameterSemaphore>(parameter).Value.RemoveLock();
            return exit;
        }

        private ControlOutput OnReset(Flow flow)
        {
            flow.GetValue<ParameterSemaphore>(parameter).ResetValue();
            return exit;
        }
    }
}