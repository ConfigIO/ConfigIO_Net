using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.FileIO
{
    internal static class Utils
    {
        public static void ReadFileStream(string fileName, FileMode fileMode, FileAccess fileAccess, Action<StreamReader> action)
        {
            var stream = new FileStream(fileName, fileMode, fileAccess);
            try
            {
                using (var reader = new StreamReader(stream))
                {
                    stream = null;
                    action(reader);
                }
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        public static void WriteFileStream(string fileName, FileMode fileMode, FileAccess fileAccess, Action<StreamWriter> action)
        {
            var stream = new FileStream(fileName, fileMode, fileAccess);
            try
            {
                using (var writer = new StreamWriter(stream))
                {
                    stream = null;
                    action(writer);
                }
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }
    }
}
