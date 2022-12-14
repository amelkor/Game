using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Bsr.CharacterController.Editor
{
    // [CreateAssetMenu(fileName = nameof(CharacterControllerAssetManager), menuName = "Game/Developer/CharacterControllerAssetManager", order = 0)]
    internal class CharacterControllerAssetManager : ScriptableObject
    {
        [SerializeField] private GameObject simpleControllerPrefab;
        [SerializeField] private ParametersData simpleMotionParametersData;

        public const string PREFS_PREFIX = "BSR.CHARACTERCONTROLLER.";
        private const string NAME_FOR_SIMPLE_CONTROLLER = "Simple First Person Player";
        private static CharacterControllerAssetManager _instance;
        private static EditorApplication.CallbackFunction _renameCallback;

        public static CharacterControllerAssetManager Instance => _instance ? _instance : CreateInstance();

        [MenuItem("GameObject/First Person/" + NAME_FOR_SIMPLE_CONTROLLER)]
        private static void CreateSimpleController()
        {
            var gameObject = Instantiate(Instance.simpleControllerPrefab, BsrEditorTool.GetSceneViewportPosition(), Quaternion.identity);
            gameObject.name = NAME_FOR_SIMPLE_CONTROLLER;

            if (gameObject.TryGetGameObjectInChildrenWithName("UI", out var ui))
                SceneVisibilityManager.instance.ToggleVisibility(ui, true);

            if (gameObject.TryGetComponentInChildrenWithName<StateMachine>("movement", out var sm))
                ConvertStateMachineGraphToEmbed(sm);
            
            CharacterSetupPopupWindow.Show(gameObject, Instance.simpleMotionParametersData);

            BsrEditorTool.RenameCreatedObject(gameObject);
        }

        #region private methods

        private static CharacterControllerAssetManager CreateInstance()
        {
            _instance = CreateInstance<CharacterControllerAssetManager>();
            if (!Initialized)
            {
                Initialize();
                Initialized = true;
            }

            return _instance;
        }

        public static bool Initialized { get => EditorPrefs.GetBool(PREFS_PREFIX + nameof(Initialized)); set => EditorPrefs.SetBool(PREFS_PREFIX + nameof(Initialized), value); }

        public static void Initialize()
        {
            var assembly = typeof(Player).Assembly;
            if (BsrEditorTool.VisualScriptingAddNodeAssembly(assembly) && BsrEditorTool.VisualScriptingAddAssemblyTypesByAttribute<TypeOptionsAddAttribute>(assembly))
            {
                UnitBase.Rebuild();
            }
        }

        private static void ConvertStateMachineGraphToEmbed(StateMachine stateMachine)
        {
            if (stateMachine.nest.source == GraphSource.Macro)
            {
                var graph = stateMachine.graph.CloneViaSerialization();
                stateMachine.nest.source = GraphSource.Embed;
                stateMachine.nest.macro = null;
                stateMachine.nest.embed = graph;
                stateMachine.graph.title = "Motion States";
                stateMachine.graph.summary = "Controls character movement";
            }
        }

        #endregion
    }
}