using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormatConversion.Conversion.Components;

namespace FormatConversion.Conversion
{
    internal class UniversalDataFormat(IDataComponent rootComponent)
    {
        public IDataComponent Root { get; private set; } = rootComponent;
    }
}
