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

        public char Current
        {
            get { return IsAtEndOfStream ? EndOfStreamChar : PeekUnchecked(); }
        }

        public StringStream(string content)
        {
            Index = 0;
            Content = content;
        }

        public StringStream(StringStream other)
        {
            Index = other.Index;
            Content = other.Content;
        }

        public char Peek()
        {
            if (IsAtEndOfStream)
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
                if (IsAtEndOfStream || IsAtAnyOf(Environment.NewLine))
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
            if (IsAtEndOfStream)
            {
                return false;
            }
            return c == PeekUnchecked();
        }

        public bool IsAt(string str, int index = 0)
        {
            if (IsAtEndOfStream)
            {
                return false;
            }
            return string.Compare(Content, Index,
                                  str, index,
                                  str.Length,
                                  StringComparison.CurrentCulture) == 0;
        }

        public bool IsAtAnyOf(string theChars)
        {
            if (IsAtEndOfStream)
            {
                return false;
            }
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
                if (IsAtEndOfStream)
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
                if (IsAtEndOfStream)
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
                if (IsAtEndOfStream)
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

        public override string ToString()
        {
            return CurrentContent;
        }
    }
}
