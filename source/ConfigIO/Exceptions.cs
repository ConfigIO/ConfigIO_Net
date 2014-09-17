using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException() : base() { }

        public InvalidSyntaxException(string message) : base(message) { }
    }
}
