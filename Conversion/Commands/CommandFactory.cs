using FormatConversion.Enums;

namespace FormatConversion.Conversion.Commands
{
    internal static class CommandFactory
    {
        private static readonly Dictionary<CommandType, Func<ICommand>> _commandCreators =
         new()
         {
            { CommandType.Caesar, () => new CaesarEncryptionCommand() }
         };

        public static ICommand GetCommand(CommandType commandType)
        {
            if(_commandCreators.TryGetValue(commandType, out var creator ))  return creator();
            throw new NotSupportedException();
        }
    }
}
