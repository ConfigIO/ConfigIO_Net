using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration.FileIO;

namespace Configuration
{
    public class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException() : base() { }

        public InvalidSyntaxException(string message) : base(message) { }
    }

    public class InvalidFileNameException : Exception
    {
        public InvalidFileNameException() : base() { }

        public InvalidFileNameException(string message) : base(message) { }
    }
}
