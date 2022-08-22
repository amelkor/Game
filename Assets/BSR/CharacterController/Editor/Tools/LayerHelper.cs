using UnityEditor;
using UnityEngine;

namespace Bsr.Ugamefps.Editor.Helpers
{
    public static class LayerHelper
    {
        /// <summary>
        /// Layers 0-8 are reserved by Unity.
        /// </summary>
        private const int CUSTOM_LAYER_START_INDEX = 8;

        private const int MAX_TAGS = 10000;
        private const int MAX_LAYERS = 31;

        /// <summary>
        /// Makes the collision detection system ignore all collisions between any collider in layer1 and any collider in layer2.
        /// Note that IgnoreLayerCollision will reset the trigger state of affected colliders, so you might receive OnTriggerExit and OnTriggerEnter messages in response to calling this.
        /// </summary>
        public static void IgnoreLayerCollision(string layer1, string layer2, bool ignore)
        {
            Physics.IgnoreLayerCollision(
                LayerMask.NameToLayer(layer1),
                LayerMask.NameToLayer(layer2),
                ignore);
        }

        public static void EnsureLayerExists(string layerName)
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layersProp = tagManager.FindProperty("layers");

            if (PropertyExists(layersProp, 0, MAX_LAYERS, layerName))
                return;

            for (var i = CUSTOM_LAYER_START_INDEX; i < MAX_LAYERS; i++)
            {
                var layer = layersProp.GetArrayElementAtIndex(i);
                if (!string.IsNullOrEmpty(layer.stringValue))
                    continue;

                layer.stringValue = layerName;
                Debug.Log($"Layer: {layerName} has been added");

                tagManager.ApplyModifiedProperties();
                return;
            }
        }

        /// <summary>
        /// Removes the layer.
        /// </summary>
        /// <returns><c>true</c>, if layer was removed, <c>false</c> otherwise.</returns>
        /// <param name="layerName">Layer name.</param>
        public static bool RemoveLayer(string layerName)
        {
            var tagManager = GetTagManager();
            var layersProp = tagManager.FindProperty("layers");

            if (!PropertyExists(layersProp, 0, layersProp.arraySize, layerName))
                return false;

            for (var i = 0; i < layersProp.arraySize; i++)
            {
                var sp = layersProp.GetArrayElementAtIndex(i);

                if (sp.stringValue != layerName)
                    continue;

                sp.stringValue = string.Empty;
                Debug.Log($"Layer: {layerName} has been removed");

                tagManager.ApplyModifiedProperties();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks to see if layer exists.
        /// </summary>
        /// <returns><c>true</c>, if layer exists, <c>false</c> otherwise.</returns>
        /// <param name="layerName">Layer name.</param>
        public static bool LayerExists(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName))
                return false;
            
            var tagManager = GetTagManager();

            var layersProp = tagManager.FindProperty("layers");
            return PropertyExists(layersProp, 0, MAX_LAYERS, layerName);
        }

        private static SerializedObject GetTagManager()
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            return tagManager;
        }

        /// <summary>
        /// Checks if the value exists in the property.
        /// </summary>
        /// <returns><c>true</c>, if exists, <c>false</c> otherwise.</returns>
        /// <param name="property">Property.</param>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="value">Value.</param>
        private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (var i = start; i < end; i++)
            {
                var prop = property.GetArrayElementAtIndex(i);
                if (prop.stringValue.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}