using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;

namespace Bsr.CharacterController.Editor.VisualScripting
{
    internal static class BoltHacks
    {
        private static readonly FieldInfo _assemblies = typeof(Unity.VisualScripting.Codebase).GetField("_assemblies", BindingFlags.Static | BindingFlags.NonPublic);
        
        public static void AddNodeAssembly(Assembly assembly)
        {
            var name = new LooseAssemblyName(assembly.GetName().Name);
            if (!BoltCore.Configuration.assemblyOptions.Contains(name))
            {
                BoltCore.Configuration.assemblyOptions.Add(name);
                Codebase.UpdateSettings();
            }
            
            
            
            
            // var assemblies = _assemblies.GetValue(typeof(Codebase)) as List<Assembly>;
            // if (!assemblies.Contains(assembly))
            // {
            //     assemblies.Add(assembly);
            //     Codebase.UpdateSettings();
            //     UnitBase.Rebuild();
            // }
        }
    }
}