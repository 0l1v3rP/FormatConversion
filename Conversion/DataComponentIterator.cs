using FormatConversion.Conversion.Commands;
using FormatConversion.Conversion.Components;
using System.Collections;
namespace FormatConversion.Conversion
{
    internal class DataComponentIterator(
        IDataComponent root) : IEnumerable<IDataComponent>
    {
        private readonly IDataComponent _root = root ?? throw new ArgumentNullException(nameof(root));

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IDataComponent> GetEnumerator()
        {
            return DepthFirstTraversal();
        }
        private IEnumerator<IDataComponent> DepthFirstTraversal()
        {
            var stack = new Stack<IDataComponent>();
            stack.Push(_root);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;

                if (current is IComplexDataComponent complexComponent)
                {
                    foreach (var child in complexComponent.GetChildren().Reverse())
                    {
                        stack.Push(child);
                    }
                }
            }
        }
        public void ExecuteCommand(ICommand command)
        {
            foreach (var component in this)
            {
                command.Execute(component);
            }
        }
    }
}