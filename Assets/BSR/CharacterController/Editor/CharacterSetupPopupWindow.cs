using Bsr.Ugamefps.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace Bsr.CharacterController.Editor
{
    internal class CharacterSetupPopupWindow : EditorWindow
    {
        public static void Show(GameObject character, ParametersData motionDataPreset)
        {
            var window = GetWindow<CharacterSetupPopupWindow>();
            window.position = new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 350, 200);
            window.titleContent = new GUIContent("Setup Character");
            window._character = character;
            window._motionDataPreset = motionDataPreset;
            window.ShowPopup();
        }

        private GameObject _character;
        private ParametersData _motionDataPreset;
        private ParametersData _motionData;
        private string _sensorLayerName = "CharacterSensors";

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("A new character requires Motion Parameters asset to be placed within your project files in order to be editable." +
                                    " The character can be used with default asset assigned but the values couldn't be changed untill the Parameters asset is not in the project files.",
                MessageType.Info);
            EditorGUILayout.Space();

            DrawFieldForLayers();
            EditorGUILayout.Space();

            DrawFieldForMotionData();
            EditorGUILayout.Space();

            DrawFinishButton();
        }

        private void DrawFieldForMotionData()
        {
            BsrEditorTool.BeginColor(Colors.red, () => !_motionData, ColorOptions.Background);
            _motionData = (ParametersData)EditorGUILayout.ObjectField(_motionData, typeof(ParametersData), false);
            BsrEditorTool.EndColor(ColorOptions.Background);

            if (!_motionData)
            {
                if (GUILayout.Button("Create new from preset"))
                {
                    var path = EditorUtility.SaveFilePanelInProject("Save Motion Parameters asset", "SimpleCharacterMotionData", "asset", "Choose path where to save the parameters asset for this character");
                    if (!string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.CreateAsset(Instantiate(_motionDataPreset), path);
                        _motionDataPreset = AssetDatabase.LoadAssetAtPath<ParametersData>(path);
                    }
                }
            }
        }

        private void DrawFieldForLayers()
        {
            var exists = LayerHelper.LayerExists(_sensorLayerName);
            BsrEditorTool.BeginColor(Colors.red, () => !exists, ColorOptions.Background);
            _sensorLayerName = EditorGUILayout.TextField("Sensor layer", _sensorLayerName);
            BsrEditorTool.EndColor(ColorOptions.Background);

            if (!exists)
            {
                GUI.enabled = !string.IsNullOrWhiteSpace(_sensorLayerName);
                BsrEditorTool.BeginColor(Colors.blue, () => GUI.enabled, ColorOptions.Background);
                if (GUILayout.Button($"Create layer {_sensorLayerName.Trim()}"))
                {
                    LayerHelper.EnsureLayerExists(_sensorLayerName);
                }

                BsrEditorTool.EndColor(ColorOptions.Background);

                GUI.enabled = true;
            }
        }

        private void DrawFinishButton()
        {
            GUI.enabled = _motionData && LayerHelper.LayerExists(_sensorLayerName);
            BsrEditorTool.BeginColor(Colors.green, () => GUI.enabled, ColorOptions.Background);
            if (GUILayout.Button("Apply and finish setup"))
            {
                FinishSetup();
            }

            BsrEditorTool.EndColor(ColorOptions.Background);
            GUI.enabled = true;
        }

        private void FinishSetup()
        {
            LayerHelper.EnsureLayerExists(_sensorLayerName);
            LayerHelper.IgnoreLayerCollision("Default", _sensorLayerName, false);
            LayerHelper.IgnoreLayerCollision(_sensorLayerName, _sensorLayerName, true);
            _character.TryGetGameObjectInChildrenWithName("obstacle_collider", out var obstacleCollider);
            _character.TryGetGameObjectInChildrenWithName("body_collider", out var bodyCollider);
            obstacleCollider.layer = LayerMask.NameToLayer(_sensorLayerName);
            bodyCollider.layer = LayerMask.NameToLayer(_sensorLayerName);

            var motion = _character.GetComponentInChildren<MotionProcessor>();
            motion.EditorSetMotionData(_motionData);
            BsrEditorTool.EndColor(ColorOptions.Background);

            var ed = ((MotionProcessorEditor)UnityEditor.Editor.CreateEditor(motion, typeof(MotionProcessorEditor)));
            ed.EditorSyncMotionParametersVariables();
            ed.serializedObject.Dispose();

            Close();
        }
    }
}