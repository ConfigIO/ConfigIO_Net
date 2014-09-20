using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.FileIO
{
    public class SyntaxCharacters
    {
        /// <summary>
        ///    # This is a comment
        /// ---^
        /// </summary>
        public char CommentPrefix { get; set; }

        /// <summary>
        ///   #[ Multiline comment. ]
        /// ---^
        /// </summary>
        public char MultiLineCommentPrefix { get; set; }

        /// <summary>
        ///   #[ Multiline comment. ]
        /// ------------------------^
        /// </summary>
        public char MultiLineCommentSuffix { get; set; }

        /// <summary>
        ///    [Section: a = 1; b = 2;]
        /// ---^
        /// </summary>
        public char SectionPrefix { get; set; }

        /// <summary>
        /// [ThisIsMyName: a = 1; b = 2;]
        /// -------------^
        /// </summary>
        public char SectionNameDelimiter { get; set; }

        /// <summary>
        ///    [Section: a = 1; b = 2;]
        /// --------------------------^
        /// </summary>
        public char SectionSuffix { get; set; }

        /// <summary>
        /// [Section: a = 1; b = 2;]
        /// ---------------^------^
        /// <remarks>Note that a line-break is also a statement delimiter.</remarks>
        /// </summary>
        public char StatementDelimiter { get; set; }

        /// <summary>
        /// [Section: a = 1; b = 2;]
        /// ------------^------^
        /// or
        /// [Section = Path/To/ConfigFile.cfg]
        /// ---------^
        /// </summary>
        public char KeyValueDelimiter { get; set; }

        /// <summary>
        /// A string of characters defining a new-line (in its entirety).
        /// e.g. "\n" or "\r\n"
        /// <remarks>"\r\n" and "\n\r" are NOT treated as the same!</remarks>
        /// </summary>
        public string NewlineDelimiter { get; set; }
    }
}
