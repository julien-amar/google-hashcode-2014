using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using GoogleHashCode.Extensions;
using GoogleHashCode.Models;

namespace GoogleHashCode
{
    public class Program
    {
        private static string LoadData()
        {
            var sample = ConfigurationManager.AppSettings["Sample"];
            
            return File.ReadAllText(sample);
        }

        public static void FillScoring(Cell[,] matrix, int x, int y)
        {
            var currentCell = matrix[y, x];
            var rate = Convert.ToInt32(ConfigurationManager.AppSettings["Rate"]);
            
            for (int size = 1; true; size++)
            {
                var bottom = y + size;
                var top = y - size;
                var left = x - size;
                var right = x + size;
                
                if (top < 0 || left < 0 || bottom >= matrix.GetLength(0) || right >= matrix.GetLength(1))
                    break;

                var countWhite = 0;

                var clearPositions = new List<Cell>();
                var impactedCells = new List<Cell>();

                for (int i = top; i <= y + size; ++i)
                {
                    for (int j = left; j <= x + size; ++j)
                    {
                        var cell = matrix[i, j];

                        impactedCells.Add(cell);

                        if (cell.IsEmpty)
                        {
                            countWhite++;

                            clearPositions.Add(cell);
                        }
                    }
                }

                int surface = (size * 2 + 1) * (size * 2 + 1);
                int countBlack = surface - countWhite;
                float occupationRate = countBlack * 100.0F / (float)surface;

                float score = occupationRate;

                if (score < rate)
                {
                    break;
                }

                currentCell.Size = size;
                currentCell.Score = score;
                currentCell.White = clearPositions;
                currentCell.ImpactedCells = impactedCells;
            }

#if DEBUG
            if (currentCell.Size != 0)
                Console.WriteLine("Best Score for ({0}, {1}) {2} in  size of {3}", currentCell.X, currentCell.Y, currentCell.Score, currentCell.Size);
#endif
        }

        private static void ExportMatrixtoBitmap(Cell[,] matrix, int nbInstruction)
        {
            var outputFile = String.Format(
                ConfigurationManager.AppSettings["Debug"],
                nbInstruction.ToString("000000000"));

            matrix.ToBitmap(
                outputFile,
                ColorExport,
                false);
        }

        private static Color ColorExport(Cell cell)
        {
            switch (cell.State)
            {
                case States.None:
                    if (!cell.IsEmpty)
                        return Color.Black;
                    break;

                case States.Impacted:
                    return Color.Green;

                case States.Painted:
                    return Color.Red;

                case States.Cleared:
                    return Color.Blue;
            }

            return Color.White;
        }

        public static void Main()
        {
            var input = LoadData();
            var inputStream = input.AsStream();
            var reader = new StreamReader(inputStream);

            // Parse matrix informations
            var sizingInformations = reader.ExtractValues<int>();
            var matrixSize = new Size(sizingInformations[1], sizingInformations[0]);

            // Initialize matrix
            var matrix = reader.ToMatrix(matrixSize, (x, y, cell) => new Cell(x, y, cell));

            // Building matrix scoring foreach cell
            for (int y = 0; y < matrixSize.Height; y++)
            {
                for (int x = 0; x < matrixSize.Width; x++)
                {
                    FillScoring(matrix, x, y);
                }
            }

            // Sorting cells by size and scoring
            IEnumerable<Cell> sorted =
                matrix
                .ToList()
                .Where(x => x.Score != 0F)
                .OrderByDescending(w => w.Size)
                .ThenByDescending(w => w.Score)
                .ToList();

            // Processing PRINTSQ instructions
            int nbInstruction = 0;
            HashSet<Cell> cleanCells = new HashSet<Cell>();

            ExportMatrixtoBitmap(matrix, nbInstruction);

            while (sorted.Any())
            {
                var first = sorted.FirstOrDefault();
                if (first != null)
                {
                    Console.WriteLine("PRINTSQ {0} {1} {2}", first.Y, first.X, first.Size);
                    first.State = States.Painted;
                    nbInstruction++;

                    // Storing clean instructions linked to current cell
                    foreach (var clearPosition in first.White)
                    {
                        cleanCells.Add(clearPosition);
                    }

                    // Update impacted cells' state
                    foreach (var impactedCell in first.ImpactedCells)
                    {
                        if (impactedCell.State == States.None)
                            impactedCell.State = States.Impacted;
                    }

                    // Cleanup impacted cells
                    foreach (var cell in sorted)
                    {
                        if (cell.ImpactedCells.Any(x => x.State == States.None) == false)
                            cell.ImpactedCells.Clear();
                    }

                    sorted = sorted
                        .Where(x => x.State != States.Painted && x.ImpactedCells.Count != 0)
                        .ToList();
                }

#if DEBUG
                // Export iteration to bitmap for debugging
                ExportMatrixtoBitmap(matrix, nbInstruction);
#endif
            }

            // Process CLEAR instructions
            foreach (var cleanCell in cleanCells)
            {
                Console.WriteLine("ERASECELL {0} {1}", cleanCell.Y, cleanCell.X);
                cleanCell.State = States.Cleared;
                nbInstruction++;

#if DEBUG
                ExportMatrixtoBitmap(matrix, nbInstruction);
#endif
            }

#if DEBUG
            Console.WriteLine("Score : {0}", nbInstruction);

            Console.WriteLine(Process.GetCurrentProcess().TotalProcessorTime.Milliseconds + "ms");
            Console.WriteLine(Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024 + "MB in RAM memory");
            Console.WriteLine(Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024 + "MB in RAM memory");

            Console.WriteLine(Process.GetCurrentProcess().VirtualMemorySize64 / 1024 / 1024 + "MB in RAM memory");

            Console.ReadLine();
#endif
        }
    }
}
