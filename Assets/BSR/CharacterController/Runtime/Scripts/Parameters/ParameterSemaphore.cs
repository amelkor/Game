using Bsr.CharacterController.Parameters.VisualScripting.Icons;
using Bsr.CharacterController.Types;

namespace Bsr.CharacterController.Parameters
{
    [Unity.VisualScripting.TypeIcon(typeof(SemaphoreIcon))]
    [Unity.VisualScripting.TypeOptionsAdd]
    public class ParameterSemaphore : Parameter<Semaphore>
    {
        public void AddLock() => Value.AddLock();
        public bool RemoveLock() => Value.RemoveLock();

        protected override bool CheckIfChanged(Semaphore oldValue, Semaphore newValue)
        {
            return oldValue.IsLocked != newValue.IsLocked;
        }

        public override string TextId => "semaphore";
        public override string GetStringValue => $"Locked: {Value.IsLocked.ToString()} Locks: {Value.Locks.ToString()}";
        public static implicit operator bool(ParameterSemaphore parameter) => parameter.Value;
    }
}