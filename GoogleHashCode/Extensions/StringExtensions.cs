using System.IO;
using System.Text;

namespace GoogleHashCode.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert <see cref="System.String"/> to <see cref="System.IO.Stream"/>.
        /// </summary>
        public static Stream AsStream(this string input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            
            MemoryStream stream = new MemoryStream(byteArray);

            return stream;
        }

        /// <summary>
        /// Convert <see cref="System.IO.Stream"/> to <see cref="System.String"/>
        /// </summary>
        public static string GetString(this Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            
            string text = reader.ReadToEnd();
            
            return text;
        }

    }
}
