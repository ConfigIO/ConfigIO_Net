
namespace Configuration.FileIO
{
    public delegate string TextProcessorCallback(string toProcess);

    public class SyntaxMarkers
    {
        /// <summary>
        /// Key = Value
        /// ----^
        /// </summary>
        public string KeyValueDelimiter { get; set; }

        /// <summary>
        /// ---------v
        /// Section 0:
        ///     opt0 = val0
        ///     opt1 = val1
        /// </summary>
        public string SectionBodyBeginMarker { get; set; }

        /// <summary>
        ///     [include] Section 0 = Path/To/My/File.cfg
        /// ----^^^^^^^^^
        /// or
        ///     $ Section 0 = Path/To/My/File.cfg
        /// ----^
        /// </summary>
        public string IncludeBeginMarker { get; set; }

        /// <summary>
        ///     // This is a comment.
        /// ----^^
        /// </summary>
        public string SingleLineCommentBeginMarker { get; set; }


        /// <summary>
        ///     /* This is a multi-line comment. */
        /// ----^^
        /// </summary>
        public string MultiLineCommentBeginMarker { get; set; }


        /// <summary>
        ///     /* This is a multi-line comment. */
        /// -------------------------------------^^
        /// </summary>
        public string MultiLineCommentEndMarker { get; set; }
    }

    public class ConfigFileReaderCallbacks
    {
        public TextProcessorCallback SectionNameProcessor { get; set; }

        public TextProcessorCallback OptionNameProcessor { get; set; }

        public TextProcessorCallback OptionValueProcessor { get; set; }

        public TextProcessorCallback FileNameProcessor { get; set; }
    }

    public class ConfigFileWriterCallbacks
    {
        public TextProcessorCallback SectionNameProcessor { get; set; }

        public TextProcessorCallback OptionNameProcessor { get; set; }

        public TextProcessorCallback OptionValueProcessor { get; set; }

        public TextProcessorCallback FileNameProcessor { get; set; }
    }
}
