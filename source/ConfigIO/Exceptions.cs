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

    [Serializable]
    public class InvalidObjectStateException : Exception
    {
        public InvalidObjectStateException() : base() {}

        public InvalidObjectStateException(string message) : base(message) {}
    }

    [Serializable]
    public class InvalidIndentationException : InvalidSyntaxException
    {
        public InvalidIndentationException(int actual, int expected) :
            base(string.Format("Expected indentation of {0}, got {1}", expected, actual))
        {
        }

        public InvalidIndentationException(string message) : base(message) { }
    }
}
