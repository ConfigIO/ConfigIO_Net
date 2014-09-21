using System;
using System.Linq;
using System.Text;

namespace Configuration
{
    public enum StringStreamPosition
    {
        Beginning,
        End,
        Current,
    }

    public class StringStream
    {
        public const char EndOfStreamChar = unchecked((char)-1);

        public string Content { get; set; }

        public string CurrentContent
        {
            get { return Content.Substring(Index); }
        }

        public int Index { get; set; }

        public bool IsAtEndOfStream
        {
            get { return Index >= Content.Length; }
        }

        public bool IsValid
        {
            get { return Index >= 0 && Index < Content.Length; }
        }

        public bool IsInvalid
        {
            get { return !IsValid; }
        }

        public bool IsAtBeginning
        {
            get { return Index == 0; }
        }

        public bool IsAtEnd
        {
            get { return Index == Content.Length - 1; }
        }

        public bool IsAtNewLine
        {
            get { return IsInvalid ? false : PeekUnchecked() == '\n'; }
        }

        public char Current
        {
            get { return IsInvalid ? EndOfStreamChar : PeekUnchecked(); }
        }

        private int _currentLineNumber = 1;
        public int CurrentLineNumber { get { return _currentLineNumber; } }

        public StringStream(string content)
        {
            Index = 0;
            Content = content;
        }

        public StringStream(StringStream other)
        {
            Index = other.Index;
            Content = other.Content;
            _currentLineNumber = other._currentLineNumber;
        }

        public char Peek()
        {
            if (IsInvalid)
            {
                return Content[Content.Length - 1];
            }
            return PeekUnchecked();
        }

        public char PeekUnchecked()
        {
            return Content[Index];
        }

        public void Next(int relativeIndex = 1)
        {
            if (Current == '\n')
            {
                _currentLineNumber += Math.Sign(relativeIndex);
            }

            Index += relativeIndex;
        }

        public char Read()
        {
            var result = Peek();
            Next();
            return result;
        }

        public string ReadLine()
        {
            var line = new StringBuilder();

            while (true)
            {
                if (IsInvalid || IsAtAnyOf(Environment.NewLine))
                {
                    break;
                }

                line.Append(Content[Index++]);
            }
            Skip(Environment.NewLine);

            return line.ToString();
        }

        public void Seek(int index, StringStreamPosition relativePosition)
        {
            switch (relativePosition)
            {
            case StringStreamPosition.Beginning:
                Index = index;
                break;
            case StringStreamPosition.End:
                Index = Content.Length - index - 1;
                break;
            case StringStreamPosition.Current:
                Index += index;
                break;
            }
        }

        public bool IsAt(char c)
        {
            if (IsInvalid) { return false; }
            return c == PeekUnchecked();
        }

        public bool IsAt(string str, int index = 0)
        {
            if (IsInvalid) { return false; }
            return string.Compare(Content, Index,
                                  str, index,
                                  str.Length,
                                  StringComparison.CurrentCulture) == 0;
        }

        public bool IsAtAnyOf(string theChars)
        {
            if (IsInvalid) { return false; }
            return theChars.Contains(PeekUnchecked());
        }

        public int Skip(char charToSkip)
        {
            var charsToSkip = new string(charToSkip, 1);
            return Skip(charsToSkip);
        }

        public int Skip(string charsToSkip)
        {
            int numSkipped = 0;
            while (true)
            {
                if (IsInvalid)
                {
                    break;
                }
                if (!IsAtAnyOf(charsToSkip))
                {
                    break;
                }
                Next();
                ++numSkipped;
            }
            return numSkipped;
        }

        /// <summary>
        /// Skips ahead in the stream until the condition becomes true.
        /// </summary>
        /// <param value="condition">The condition functor to check when to stip skipping.</param>
        /// <returns>Number of characters skipped</returns>
        public int SkipUntil(Func<char, bool> condition)
        {
            int numSkipped = 0;

            while (true)
            {
                if (IsInvalid)
                {
                    break;
                }
                if (condition(PeekUnchecked()))
                {
                    break;
                }
                Next();
                ++numSkipped;
            }

            return numSkipped;
        }


        /// <summary>
        /// Skips ahead in the stream while the given condition is true.
        /// </summary>
        /// <param value="condition">The condition functor to check when to stip skipping.</param>
        /// <returns>Number of characters skipped</returns>
        public int SkipWhile(Func<char, bool> condition)
        {
            int numSkipped = 0;

            while (true)
            {
                if (IsInvalid)
                {
                    break;
                }
                if (!condition(PeekUnchecked()))
                {
                    break;
                }
                Next();
                ++numSkipped;
            }

            return numSkipped;
        }

        /// <summary>
        /// Skips ahead in the stream until the condition becomes true.
        /// </summary>
        /// <param value="condition">The condition functor to check when to stip skipping.</param>
        /// <returns>Number of characters skipped</returns>
        public int SkipReverseUntil(Func<char, bool> condition)
        {
            int numSkipped = 0;

            while (true)
            {
                if (IsAtBeginning || IsInvalid)
                {
                    break;
                }
                if (condition(PeekUnchecked()))
                {
                    break;
                }
                Next(-1);
                ++numSkipped;
            }

            return numSkipped;
        }


        /// <summary>
        /// Skips ahead in the stream while the given condition is true.
        /// </summary>
        /// <param value="condition">The condition functor to check when to stip skipping.</param>
        /// <returns>Number of characters skipped</returns>
        public int SkipReverseWhile(Func<char, bool> condition)
        {
            int numSkipped = 0;

            while (true)
            {
                if (IsAtBeginning || IsInvalid)
                {
                    break;
                }
                if (!condition(PeekUnchecked()))
                {
                    break;
                }
                Next(-1);
                ++numSkipped;
            }

            return numSkipped;
        }

        public override string ToString()
        {
            return CurrentContent;
        }
    }
}
