using System.Reflection;
using Unity.VisualScripting;

namespace Bsr.CharacterController
{
    public static class VariableDeclarationsExtensions
    {
        private static readonly FieldInfo _collection = typeof(VariableDeclarations).GetField("collection", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool Remove(this VariableDeclarations declarations, VariableDeclaration declaration)
        {
            var collection = _collection.GetValue(declarations);
            if (collection is VariableDeclarationCollection c)
                return c.Remove(declaration);
            return false;
        }
    }
}