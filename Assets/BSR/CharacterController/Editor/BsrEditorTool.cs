using System;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Bsr.CharacterController.Editor
{
    internal static class BsrEditorTool
    {
        public static bool VisualScriptingAddNodeAssembly(Assembly assembly)
        {
            var name = new LooseAssemblyName(assembly.GetName().Name);
            if (!BoltCore.Configuration.assemblyOptions.Contains(name))
            {
                BoltCore.Configuration.assemblyOptions.Add(name);
                Codebase.UpdateSettings();
                
                return true;
            }

            return false;
        }

        public static bool VisualScriptingAddAssemblyTypesByAttribute<T>(Assembly assembly) where T : Attribute
        {
            var added = false;
            foreach (var type in assembly.GetTypes().Where(t => t.GetCustomAttribute<T>() != null))
            {
                if(BoltCore.Configuration.typeOptions.Contains(type))
                    continue;
                
                added = true;
                BoltCore.Configuration.typeOptions.Add(type);
            }

            if (added)
                Codebase.UpdateSettings();

            return added;
        }

        public static Vector3 GetSceneViewportPosition()
        {
            var sceneCamera = Camera.current;
            if (!sceneCamera && SceneView.sceneViews.Count > 0)
            {
                var sceneView = ((SceneView)SceneView.sceneViews[0]);
                sceneView.Focus();
                sceneCamera = sceneView.camera;
            }

            return sceneCamera
                ? sceneCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f))
                : Vector3.zero;
        }

        public static void RenameCreatedObject(GameObject gameObject)
        {
            EditorApplication.CallbackFunction renameCallback = delegate { };
            renameCallback = () =>
            {
                EditorApplication.delayCall -= renameCallback;
                Selection.activeGameObject = gameObject;
                EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
                EditorWindow.focusedWindow.SendEvent(CreateRenamingEvent());
            };
            EditorApplication.delayCall += renameCallback;
        }

        private static Event CreateRenamingEvent() => new() { keyCode = KeyCode.F2, type = EventType.KeyDown };
    }
}