using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.FileIO
{
    public delegate string PreprocessorCallback(string toProcess);

    public abstract class ConfigFileReaderWriterBase
    {
        public SyntaxCharacters Syntax { get; set; }

        public PreprocessorCallback SectionNamePreprocessor { get; set; }

        public PreprocessorCallback OptionNamePreprocessor { get; set; }

        public PreprocessorCallback OptionValuePreprocessor { get; set; }

        public PreprocessorCallback FileNamePreprocessor { get; set; }

        public ConfigFileReaderWriterBase()
        {
            Syntax = new SyntaxCharacters();
            Syntax.CommentPrefix = '#';
            Syntax.MultiLineCommentPrefix = '[';
            Syntax.MultiLineCommentSuffix = ']';
            Syntax.SectionPrefix = '[';
            Syntax.SectionNameDelimiter = ':';
            Syntax.SectionSuffix = ']';
            Syntax.StatementDelimiter = ';';
            Syntax.KeyValueDelimiter = '=';
            Syntax.NewlineDelimiter = "\n";

            SectionNamePreprocessor = section => section.Trim();
            OptionNamePreprocessor = key => key.Trim();
            OptionValuePreprocessor = value => value.Trim();
            FileNamePreprocessor = fileName => fileName.Trim();
        }
    }
}
