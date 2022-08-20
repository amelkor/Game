using Bsr.CharacterController.Parameters.VisualScripting.Icons;
using UnityEngine;

namespace Bsr.CharacterController.Parameters
{
    [Unity.VisualScripting.TypeIcon(typeof(GameObjectIcon))]
    [Unity.VisualScripting.TypeOptionsAdd]
    public class ParameterGameObject : Parameter<GameObject>
    {
        protected override bool CheckIfChanged(GameObject oldValue, GameObject newValue)
        {
            return !oldValue.Equals(newValue);
        }
        
        public override string TextId => "gameobject";
        public static implicit operator GameObject(ParameterGameObject parameter) => parameter.Value;
    }
}