using Bsr.CharacterController.Parameters;
using UnityEditor;
using UnityEngine;

namespace Bsr.CharacterController.Editor
{
    [CustomEditor(typeof(ParameterBase), true)]
    public class ParameterEditor : UnityEditor.Editor
    {
        private const string DEFAULT_ICON_NAME = "DefaultAsset Icon";

        public SerializedProperty defaultValue;
        public GUIContent Icon { get; private set; }
        private string _typeName;

        private void Awake()
        {
            defaultValue = serializedObject.FindProperty("defaultValue");
        }

        private void OnEnable()
        {
            _typeName = target.GetType().Name;
            
            var icons = AssetDatabase.FindAssets($"{_typeName} t:texture l:icon");
            Icon = icons.Length > 0
                ? new GUIContent { image = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(icons[0])) }
                : EditorGUIUtility.IconContent(DEFAULT_ICON_NAME);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(Icon);
            EditorGUILayout.SelectableLabel(_typeName);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var newName = EditorGUILayout.TextField(target.name);
            if (defaultValue != null)
                EditorGUILayout.PropertyField(defaultValue, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    target.name = newName.Trim();
                }

                serializedObject.ApplyModifiedProperties();
                UnityEditor.AssetDatabase.SaveAssetIfDirty(target);
            }

            EditorGUILayout.EndHorizontal();

            if (UnityEditor.EditorApplication.isPlaying)
            {
                var enabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUILayout.TextField("Value", ((ParameterBase)target).GetStringValue);
                GUI.enabled = enabled;
            }
        }
    }
}