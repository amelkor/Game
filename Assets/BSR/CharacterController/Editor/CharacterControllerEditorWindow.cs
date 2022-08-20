using UnityEditor;
using UnityEngine;

namespace Bsr.CharacterController.Editor
{
    public class CharacterControllerEditorWindow : EditorWindow
    {
        private const string WINDOW_NAME = "Character Controller Tool";

        [MenuItem("Window/Game/" + WINDOW_NAME)]
        private static void ShowWindow() => GetWindow<CharacterControllerEditorWindow>(WINDOW_NAME);

        private void OnGUI()
        {
            if (GUILayout.Button("Reinitialize dependencies", GUIStyle.none))
                CharacterControllerAssetManager.Initialize();
        }
    }
}