using System;

namespace Configuration
{
    [Serializable]
    public class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException() : base() { }

        public InvalidSyntaxException(string message) : base(message) { }
    }

    [Serializable]
    public class InvalidFileNameException : Exception
    {
        public InvalidFileNameException() : base() { }

        public InvalidFileNameException(string message) : base(message) { }
    }

    public class InvalidObjectStateException : Exception
    {
        public InvalidObjectStateException() : base() {}

        public InvalidObjectStateException(string message) : base(message) {}
    }
}
