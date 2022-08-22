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

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("A new character requires Motion Parameters asset to be placed within your project files in order to be editable." +
                                    " The character can be used with default asset assigned but the values couldn't be changed untill the Parameters asset is not in the project files.",
                MessageType.Info);

            BsrEditorTool.BeginColor(Colors.red, () => _motionData == null, ColorOptions.Background);
            _motionData = (ParametersData)EditorGUILayout.ObjectField(_motionData, typeof(ParametersData), false);
            BsrEditorTool.EndColor(ColorOptions.Background);

            EditorGUILayout.Space();

            if (_motionData)
            {
                BsrEditorTool.BeginColor(Colors.lightGreen, ColorOptions.Background);
                if (GUILayout.Button($"Use {_motionData.name}"))
                {
                    var motion = _character.GetComponentInChildren<MotionProcessor>();
                    motion.EditorSetMotionData(_motionData);
                    BsrEditorTool.EndColor(ColorOptions.Background);

                    ((MotionProcessorEditor)UnityEditor.Editor.CreateEditor(motion, typeof(MotionProcessorEditor))).EditorSyncMotionParametersVariables();
                    
                    Close();
                }
                BsrEditorTool.EndColor(ColorOptions.Background);
            }
            else
            {
                BsrEditorTool.BeginColor(Colors.blue, ColorOptions.Background);
                if (GUILayout.Button("Create new from preset"))
                {
                    var path = EditorUtility.SaveFilePanelInProject("Save Motion Parameters asset", "SimpleCharacterMotionData", "asset", "Choose path where to save the parameters asset for this character");
                    if (!string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.CreateAsset(Instantiate(_motionDataPreset), path);
                        var data = AssetDatabase.LoadAssetAtPath<ParametersData>(path);
                        var motion = _character.GetComponentInChildren<MotionProcessor>();
                        motion.EditorSetMotionData(data);
                        BsrEditorTool.EndColor(ColorOptions.Background);

                        ((MotionProcessorEditor)UnityEditor.Editor.CreateEditor(motion, typeof(MotionProcessorEditor))).EditorSyncMotionParametersVariables();

                        Close();
                    }
                    BsrEditorTool.EndColor(ColorOptions.Background);
                }
            }
        }
    }
}