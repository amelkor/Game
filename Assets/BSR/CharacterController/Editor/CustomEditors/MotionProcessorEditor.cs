using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bsr.CharacterController.Parameters;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Bsr.CharacterController.Editor
{
    [CustomEditor(typeof(MotionProcessor), true)]
    public class MotionProcessorEditor : UnityEditor.Editor
    {
        private bool _autoSyncVariables = true;
        private MotionProcessor _target;

        private void OnEnable()
        {
            _target = (MotionProcessor)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            
            _autoSyncVariables = EditorGUILayout.Toggle("Auto sync Variables", _autoSyncVariables);
            if (_autoSyncVariables)
            {
                if (EditorCanProceedWithSync() && EditorIsVariablesSyncNeeded())
                    EditorSyncMotionParametersVariables();
            }
            else if (EditorCanProceedWithSync())
            {
                var isSyncNeeded = EditorIsVariablesSyncNeeded();
                if (isSyncNeeded)
                {
                    EditorGUILayout.HelpBox("MotionData parameters are not synced with Variables", MessageType.Error);
                }

                if (isSyncNeeded)
                    GUI.backgroundColor = Color.red;

                if (GUILayout.Button("Sync MotionData Variables") && ((MotionProcessor)target).ParametersData)
                {
                    EditorSyncMotionParametersVariables();
                }
            }
        }

        #region private

        private bool EditorCanProceedWithSync() => _target.ParametersData && _target.Variables;

        private bool EditorIsVariablesSyncNeeded()
        {
            var variables = _target.Variables;
            var motionData = _target.ParametersData;
            return variables.declarations.Count(d => d.value is ParameterBase) != motionData.ParametersCount
                   || variables.declarations.Any(v => v.value is ParameterBase && !motionData.Parameters.Any(p => p.name.Equals(v.name, StringComparison.Ordinal)));
        }

        internal void EditorSyncMotionParametersVariables()
        {
            var variables = _target.Variables;
            var motionData = _target.ParametersData;

            var variablesLookup = new Dictionary<ParameterBase, VariableDeclaration>();
            foreach (var d in variables.declarations)
            {
                if (d.value is ParameterBase p)
                    variablesLookup.Add(p, d);
            }

            var propertiesLookup = motionData.ParametersLookup;
            foreach (var (_, p) in propertiesLookup)
            {
                if (variablesLookup.TryGetValue(p, out var d))
                {
                    // in case the parameter has been renamed, rename the variable
                    if (!d.name.Equals(p.name, StringComparison.Ordinal))
                        _declarationName.SetValue(d, p.name);
                }
                else
                {
                    // add new variable for the corresponding parameter
                    variables.declarations.Set(p.name, p);
                }
            }

            // remove deleted parameters from variables
            var orphants = variablesLookup.Keys.Except(propertiesLookup.Values);
            foreach (var orphant in orphants)
            {
                if (variablesLookup.TryGetValue(orphant, out var d))
                    variables.declarations.Remove(d);
            }
        }

        private static readonly PropertyInfo _declarationName = typeof(VariableDeclaration).GetProperty(nameof(VariableDeclaration.name));

        #endregion
    }
}