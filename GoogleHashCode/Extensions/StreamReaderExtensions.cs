using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace GoogleHashCode.Extensions
{
    public static class StreamReaderExtensions
    {
        /// <summary>
        /// Extract a single line from a <see cref="System.IO.StreamReader"/>.
        /// </summary>
        public static string ExtractLine(this StreamReader reader)
        {
            return reader.ReadLine();
        }

        /// <summary>
        /// Iterate over multiple lines from a <see cref="System.IO.StreamReader"/>.
        /// </summary>
        public static IEnumerable<string> ExtractLines(this StreamReader reader)
        {
            while (true)
            {
                var line = ExtractLine(reader);

                if (line == null)
                    yield break;

                yield return line;
            }
        }

        /// <summary>
        /// Extract multiple values parsing a single line of a <see cref="System.IO.StreamReader"/>.
        /// </summary>
        public static T[] ExtractValues<T>(this StreamReader reader, char separator = ' ')
        {
            var line = ExtractLine(reader);
            var informations = line.Split(separator);

            var converter = TypeDescriptor.GetConverter(typeof(T));
            
            var query =
                from i in informations
                select (T) converter.ConvertFromString(i);

            return query.ToArray();
        }
    }
}
