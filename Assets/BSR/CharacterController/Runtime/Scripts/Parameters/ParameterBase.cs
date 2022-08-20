using UnityEngine;

namespace Bsr.CharacterController.Parameters
{
    public abstract class ParameterBase : ScriptableObject
    {
        public abstract string TextId { get; }
        public abstract string GetStringValue { get; }
    }
}