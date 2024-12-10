using FormatConversion.Conversion.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatConversion.Conversion.Commands
{
    internal interface ICommand
    {
        void Execute(IDataComponent component);
    }
}
