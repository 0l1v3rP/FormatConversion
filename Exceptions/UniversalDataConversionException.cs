using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatConversion.Exceptions
{
    internal class UniversalDataConversionException : Exception
    {
        public UniversalDataConversionException(string msg): base(msg) { }

        public UniversalDataConversionException(string msg, Exception innerException)
            : base(msg, innerException)  { }
    }
}
