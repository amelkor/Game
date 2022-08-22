using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

namespace Bsr.CharacterController.Editor
{
    [Flags]
    public enum ColorOptions
    {
        Content = 1 << 0,
        Background = 1 << 1
    }

    public static class Colors
    {
        public static Color FromHTML(string hex) => ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.black;
        
        public static Color red { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = new(0.8980392f, 0.2235294f, 0.2078431f, 1f);
        public static Color lightRed { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = new(0.8980392f, 0.4352941f, 0.3764706f, 1f);
        public static Color purple { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = new(0.5568628f, 0.1411765f, 0.6666667f, 1f);
        public static Color blue { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = new(0.1176471f, 0.5333334f, 0.8980392f, 1f);
        public static Color green { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = FromHTML("#4b830d");
        public static Color lightGreen { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = new(0.4862745f, 0.7019608f, 0.2588235f, 1f);
    }

    internal static class BsrEditorTool
    {
        private static UnityEngine.Color _cachedContentColor = GUI.contentColor;
        private static UnityEngine.Color _cachedBackgroundColor = GUI.backgroundColor;

        public static void BeginColor(UnityEngine.Color color, ColorOptions options)
        {
            if (options.HasFlagFast(ColorOptions.Content))
            {
                _cachedContentColor = GUI.contentColor;
                GUI.contentColor = color;
            }

            if (options.HasFlagFast(ColorOptions.Background))
            {
                _cachedBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = color;
            }
        }

        public static void BeginColor(UnityEngine.Color color, Func<bool> predicate, ColorOptions options)
        {
            if (predicate())
                BeginColor(color, options);
        }

        public static void BeginColor(Func<Color> predicate, ColorOptions options)
        {
            var color = predicate();
            BeginColor(color, options);
        }

        public static void EndColor(ColorOptions options)
        {
            if (options.HasFlagFast(ColorOptions.Content)) GUI.contentColor = _cachedContentColor;
            if (options.HasFlagFast(ColorOptions.Background)) GUI.backgroundColor = _cachedBackgroundColor;
        }

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
                if (BoltCore.Configuration.typeOptions.Contains(type))
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

    internal static class ColorOptionsExtensions
    {
        public static bool HasFlagFast(this ColorOptions value, ColorOptions flag) => (value & flag) != 0;
    }
}