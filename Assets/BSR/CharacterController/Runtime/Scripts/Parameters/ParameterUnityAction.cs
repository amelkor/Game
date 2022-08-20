using System.Text;
using Bsr.CharacterController.Parameters.VisualScripting.Icons;
using UnityEngine.Events;

namespace Bsr.CharacterController.Parameters
{
    [Unity.VisualScripting.TypeIcon(typeof(UnityActionIcon))]
    [Unity.VisualScripting.TypeOptionsAdd]
    public class ParameterUnityAction : Parameter<UnityAction>
    {
        public override string TextId => "event";
        protected override bool CheckIfChanged(UnityAction oldValue, UnityAction newValue)
        {
            return oldValue != newValue;
        }
        
        public static implicit operator UnityAction(ParameterUnityAction parameter) => parameter.Value;

        public override string GetStringValue
        {
            get
            {
                var sb = new StringBuilder();
                var delegates = Value.GetInvocationList();
                sb.Append(delegates.Length).Append(" delegates:");
                foreach (var d in delegates)
                {
                    sb.Append(" ").Append(d.Method.Name).Append(" target: ").Append(d.Target);
                }

                return sb.ToString();
            }
        }
    }
}