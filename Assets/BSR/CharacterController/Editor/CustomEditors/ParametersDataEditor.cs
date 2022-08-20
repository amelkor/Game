using System;
using System.Collections.Generic;
using Bsr.CharacterController;
using Bsr.CharacterController.Parameters;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Game.CharacterController.Editor
{
    [CustomEditor(typeof(ParametersData))]
    public class ParametersDataEditor : UnityEditor.Editor
    {
        private const string PARAMETER_NAME = nameof(PARAMETER_NAME);
        private readonly Dictionary<int, UnityEditor.Editor> _parameterEditors = new();
        private bool _focusEditingNewElementName;
        private ReorderableList _list;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.HelpBox(new GUIContent("Motion parameters are used to control motion behavior, like speed, can jump, drag, etc."));
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            _list = new ReorderableList(serializedObject, serializedObject.FindProperty("parameters"), true, true, true, true);
            _list.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Motion Parameters"); };
            _list.drawElementCallback = (rect, index, active, focused) =>
            {
                var element = _list.serializedProperty.GetArrayElementAtIndex(index);

                if (!_parameterEditors.TryGetValue(element.objectReferenceValue.GetInstanceID(), out var editor))
                {
                    CreateCachedEditor(element.objectReferenceValue, typeof(ParameterEditor), ref editor);
                    _parameterEditors.Add(element.objectReferenceValue.GetInstanceID(), editor);
                }

                rect.y += 2;

                EditorGUI.BeginChangeCheck();
                GUI.SetNextControlName(PARAMETER_NAME);
                var h = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, h, h), ((ParameterEditor)editor).Icon);
                var newName = EditorGUI.TextField(new Rect(rect.x + h, rect.y, rect.width * 0.5f - 2, EditorGUIUtility.singleLineHeight), element.objectReferenceValue.name);
                
                var defaultValue = ((ParameterEditor)editor).defaultValue;
                if (defaultValue != null)
                    EditorGUI.PropertyField(new Rect(rect.x + rect.width * 0.5f + 2 + h, rect.y, rect.width * 0.5f - 2 - h, EditorGUIUtility.singleLineHeight), defaultValue, GUIContent.none);

                if (_focusEditingNewElementName && index == _list.serializedProperty.arraySize - 1)
                {
                    _focusEditingNewElementName = false;
                    EditorGUI.FocusTextInControl(PARAMETER_NAME);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    if (!string.IsNullOrWhiteSpace(newName) && !element.objectReferenceValue.name.Equals(newName, StringComparison.OrdinalIgnoreCase))
                    {
                        editor.target.name = newName.Trim();
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(target);
                    }

                    ((ParameterEditor)editor).serializedObject.ApplyModifiedProperties();
                    serializedObject.ApplyModifiedProperties();
                    UnityEditor.AssetDatabase.SaveAssetIfDirty(element.objectReferenceValue);
                }
            };
            _list.onAddDropdownCallback = (_, _) =>
            {
                var menu = new UnityEditor.GenericMenu();
                menu.AddItem(new GUIContent("float", "Create new Float parameter"), false, CreateParameterAsset<ParameterFloat>);
                menu.AddItem(new GUIContent("int", "Create new Integer parameter"), false, CreateParameterAsset<ParameterInt>);
                menu.AddItem(new GUIContent("bool", "Create new Boolean parameter"), false, CreateParameterAsset<ParameterBool>);
                menu.AddItem(new GUIContent("Vector3", "Create new Vector3 parameter"), false, CreateParameterAsset<ParameterVector3>);
                menu.AddItem(new GUIContent("Semaphore", "Create new Semaphore parameter"), false, CreateParameterAsset<ParameterSemaphore>);
                menu.AddItem(new GUIContent("UnityAction", "Create new event parameter"), false, CreateParameterAsset<ParameterUnityAction>);
                menu.AddItem(new GUIContent("Transform", "Create new Transform parameter"), false, CreateParameterAsset<ParameterTransform>);
                menu.AddItem(new GUIContent("GameObject", "Create new GameObject parameter"), false, CreateParameterAsset<ParameterGameObject>);
                menu.ShowAsContext();
            };
            _list.onRemoveCallback = list =>
            {
                var index = list.index;
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                UnityEditor.AssetDatabase.RemoveObjectFromAsset(element.objectReferenceValue);
                _parameterEditors.Remove(element.objectReferenceValue.GetInstanceID(), out _);
                element.DeleteCommand();
                serializedObject.ApplyModifiedProperties();
                UnityEditor.AssetDatabase.SaveAssetIfDirty(target);
            };
        }

        private void CreateParameterAsset<T>() where T : ParameterBase
        {
            var parameter = CreateInstance<T>();
            var pName = $"new {parameter.TextId}";
            var tries = 0;
            while (IsNameTaken(pName))
            {
                pName = $"new {parameter.TextId} ({(++tries).ToString()})";
            }

            parameter.name = pName;

            UnityEditor.AssetDatabase.AddObjectToAsset(parameter, target);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(target);

            var index = _list.serializedProperty.arraySize;
            _list.serializedProperty.arraySize++;
            _list.index = index;
            var element = _list.serializedProperty.GetArrayElementAtIndex(index);
            element.objectReferenceValue = parameter;
            _focusEditingNewElementName = true;

            serializedObject.ApplyModifiedProperties();

            bool IsNameTaken(string n)
            {
                if (_list.serializedProperty.arraySize == 0)
                    return false;

                for (var i = 0; i < _list.serializedProperty.arraySize; i++)
                {
                    var sp = _list.serializedProperty.GetArrayElementAtIndex(i);
                    if (sp == null)
                        continue;
                    var o = sp.objectReferenceValue;
                    if (!o)
                        continue;
                    if (o.name.Equals(n, StringComparison.Ordinal))
                        return true;
                }

                return false;
            }
        }
    }
}