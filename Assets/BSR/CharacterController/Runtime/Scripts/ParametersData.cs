using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bsr.CharacterController.Parameters;
using Bsr.CharacterController.Parameters.VisualScripting.Icons;
using UnityEngine;

namespace Bsr.CharacterController
{
    [Unity.VisualScripting.TypeOptionsAdd]
    [Unity.VisualScripting.TypeIcon(typeof(ParametersDataIcon))]
    [CreateAssetMenu(fileName = "ParametersData", menuName = "Game/Parameters Data", order = 0)]
    public class ParametersData : ScriptableObject
    {
        [SerializeField] private List<ParameterBase> parameters;

        private const int INITIAL_COLLECTIONS_SIZE = 10;
        public int ParametersCount => parameters.Count;
        public IReadOnlyList<ParameterBase> Parameters => parameters;
        public IReadOnlyDictionary<string, ParameterBase> ParametersLookup => parameters.ToDictionary(p => p.name, p => p);
        public IReadOnlyDictionary<int, ParameterBase> ParametersHashedLookup => parameters.ToDictionary(p => StringToHash(p.name), p => p);
        public IReadOnlyDictionary<string, int> ParametersHashesByNamesLookup => parameters.ToDictionary(p => p.name, p => Animator.StringToHash(p.name));
        public static int StringToHash(string name) => Animator.StringToHash(name);

        [NonSerialized] private readonly Dictionary<int, ParameterFloat> _floats = new(INITIAL_COLLECTIONS_SIZE);
        [NonSerialized] private readonly Dictionary<int, ParameterInt> _ints = new(INITIAL_COLLECTIONS_SIZE);
        [NonSerialized] private readonly Dictionary<int, ParameterBool> _bools = new(INITIAL_COLLECTIONS_SIZE);
        [NonSerialized] private readonly Dictionary<int, ParameterVector3> _vectors = new(INITIAL_COLLECTIONS_SIZE);
        [NonSerialized] private readonly Dictionary<int, ParameterSemaphore> _semaphores = new(INITIAL_COLLECTIONS_SIZE);
        [NonSerialized] private readonly Dictionary<int, ParameterTransform> _transforms = new(INITIAL_COLLECTIONS_SIZE);
        [NonSerialized] private readonly Dictionary<int, ParameterGameObject> _gameObjects = new(INITIAL_COLLECTIONS_SIZE);
        [NonSerialized] private readonly Dictionary<int, ParameterUnityAction> _unityActions = new(INITIAL_COLLECTIONS_SIZE);
        
        [NonSerialized] private Dictionary<Type, IDictionary> _byTypeLookup;
        [NonSerialized] private IReadOnlyDictionary<int, ParameterBase> _parametersHashedLookup;

        private void OnEnable() => Init();

        private void Init()
        {
            _parametersHashedLookup = ParametersHashedLookup;
            _byTypeLookup = new Dictionary<Type, IDictionary>
            {
                { typeof(ParameterFloat), _floats },
                { typeof(ParameterInt), _ints },
                { typeof(ParameterBool), _bools },
                { typeof(ParameterVector3), _vectors },
                { typeof(ParameterSemaphore), _semaphores },
                { typeof(ParameterTransform), _transforms },
                { typeof(ParameterGameObject), _gameObjects },
                { typeof(ParameterUnityAction), _unityActions },
            };
            
            foreach (var p in parameters)
            {
                switch (p)
                {
                    case ParameterFloat pFloat:
                        _floats.Add(Animator.StringToHash(p.name), pFloat);
                        continue;
                    case ParameterInt pInt:
                        _ints.Add(Animator.StringToHash(p.name), pInt);
                        continue;
                    case ParameterBool pBool:
                        _bools.Add(Animator.StringToHash(p.name), pBool);
                        continue;
                    case ParameterVector3 pVector3:
                        _vectors.Add(Animator.StringToHash(p.name), pVector3);
                        continue;
                    case ParameterSemaphore pSemaphore:
                        _semaphores.Add(Animator.StringToHash(p.name), pSemaphore);
                        continue;
                    case ParameterTransform pTransform:
                        _transforms.Add(Animator.StringToHash(p.name), pTransform);
                        continue;
                    case ParameterGameObject pGameObject:
                        _gameObjects.Add(Animator.StringToHash(p.name), pGameObject);
                        continue;
                    case ParameterUnityAction pAction:
                        _unityActions.Add(Animator.StringToHash(p.name), pAction);
                        continue;
                }
            }
        }

        public bool GetParameter<T>(string name, out T parameter)  where T: ParameterBase => GetParameter(StringToHash(name), out parameter);
        
        public bool GetParameter<T>(int hash, out T parameter) where T: ParameterBase
        {
            if (_parametersHashedLookup.ContainsKey(hash) && _byTypeLookup.TryGetValue(typeof(T), out var dictionary))
            {
                var o = dictionary[hash];
                if (o is T p)
                {
                    parameter = p;
                    return true;
                }
            }

            parameter = default;
            return false;
        }
        
        #region Getters

        public bool TryGetFloat(int hash, out ParameterFloat value) => _floats.TryGetValue(hash, out value);

        public bool TryGetInt(int hash, out ParameterInt value) => _ints.TryGetValue(hash, out value);

        public bool TryGetBool(int hash, out ParameterBool value) => _bools.TryGetValue(hash, out value);

        public bool TryGetVector3(int hash, out ParameterVector3 value) => _vectors.TryGetValue(hash, out value);

        public bool TryGetSemaphore(int hash, out ParameterSemaphore value) => _semaphores.TryGetValue(hash, out value);

        public bool TryGetTransform(int hash, out ParameterTransform value) => _transforms.TryGetValue(hash, out value);

        public bool TryGetGameObject(int hash, out ParameterGameObject value) => _gameObjects.TryGetValue(hash, out value);

        public bool TryGetUnityAction(int hash, out ParameterUnityAction value) => _unityActions.TryGetValue(hash, out value);

        #endregion
    }
}