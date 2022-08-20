using System;
using UnityEngine.Scripting;

namespace Bsr.CharacterController.Types
{
    /// <summary>
    /// Contains number of locks which are when greater than 0 lock Semaphore so it's value becomes True.
    /// When Semaphore is not locked, the value is False.
    /// </summary>
    [Preserve]
    [Serializable]
    public class Semaphore
    {
        private int _locks;

        public bool IsLocked => _locks != 0;
        public int Locks => _locks;

        /// <summary>
        /// Adds one lock to this Semaphore.
        /// </summary>
        public void AddLock() => ++_locks;

        /// <summary>
        /// Removes one lock and returns false if Semaphore has been unlocked.
        /// </summary>
        /// <returns>True if Semaphore is still locked.</returns>
        public bool RemoveLock()
        {
            if (_locks - 1 < 0)
                return false;
            
            return --_locks != 0;
        }
        
        public static implicit operator bool(Semaphore s) => s.IsLocked;
    }
}