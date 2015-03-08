using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace GoogleHashCode.Extensions
{
    public static class MatrixExtension
    {
        public static void ToBitmap<T>(this T[,] matrix, string outputPath, Func<T, Color> colorExporter, bool overwriteOutputFile = true)
        {
            if (!overwriteOutputFile && File.Exists(outputPath))
                return;

            Bitmap bitmap = new Bitmap(matrix.GetLength(1), matrix.GetLength(0));

            for (int y = 0; y < matrix.GetLength(0); ++y)
                for (int x = 0; x < matrix.GetLength(1); ++x)
                {
                    var cell = matrix[y, x];
                    var color = colorExporter(cell);
                    
                    bitmap.SetPixel(x, y, color);
                }

            bitmap.Save(outputPath);
        }

        public static IEnumerable<T> ToList<T>(this T[,] matrix)
        {
            for (int y = 0; y < matrix.GetLength(0); ++y)
                for (int x = 0; x < matrix.GetLength(1); ++x)
                    yield return matrix[y, x];
        }

        private static T Convert<T>(this string input)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            
            return (T)converter.ConvertFromString(input);
        }

        public static T[,] ToMatrix<T>(this StreamReader reader, Size matrixSize, char columnSeparator = ' ')
        {
            var matrix = new T[matrixSize.Width, matrixSize.Height];

            var line = reader.ReadLine();

            for (var y = 0; !string.IsNullOrEmpty(line); ++y)
            {
                var x = 0;

                foreach (var cell in line.Split(columnSeparator))
                {
                    matrix[y, x] = cell.Convert<T>();
                    x++;
                }

                line = reader.ReadLine();
            }

            return matrix;
        }

        public static T[,] ToMatrix<T>(this StreamReader reader, Size matrixSize, Func<int, int, char, T> matcher)
        {
            var matrix = new T[matrixSize.Height, matrixSize.Width];

            var line = reader.ReadLine();

            for (var y = 0; !string.IsNullOrEmpty(line); ++y)
            {
                var x = 0;

                foreach (var cell in line)
                {
                    matrix[y, x] = matcher(x, y, cell);
                    x++;
                }

                line = reader.ReadLine();
            }

            return matrix;
        }
    }
}
