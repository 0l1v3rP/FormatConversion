using FormatConversion.Conversion.Components;
using FormatConversion.Enums;
using System.Text;
using FormatConversion.Extensions;

namespace FormatConversion.Conversion.Commands
{
    internal class CaesarEncryptionCommand : ICommand
    {
        public void Execute(IDataComponent component)
        {
            if (component.Type.Equals(DataComponentType.Structure))
            {
                StructuredDataComponent structure = (StructuredDataComponent)component;
                foreach (var key in structure.Fields.Keys.ToList()) 
                {
                    string newKey = Encrypt(key);
                    structure.Fields.UpdateKey(key, newKey);
                }
            }
            if (component.Type.Equals(DataComponentType.Primitive))
            {
                PrimitiveDataComponent primitive = (PrimitiveDataComponent)component;
                if (primitive.ValueType != typeof(string)) return;
                primitive.Value = Encrypt((string)primitive.Value!);
            }
        }


        const int SHIFT = 3;
        private static string Encrypt(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var result = new StringBuilder(input.Length);

            foreach (char c in input)
            {
                result.Append(ShiftChar(c));
            }

            return result.ToString();
        }

        private static char ShiftChar(char c)
        {
            char? shiftChar = null;
            if (char.IsUpper(c))
                shiftChar = 'A';
            else if (char.IsLower(c))
                shiftChar = 'a';

            return shiftChar is not null ? (char)(((c - 'A' + SHIFT) % 26 + 26) % 26 + 'A') : c;
        }
    }
}
