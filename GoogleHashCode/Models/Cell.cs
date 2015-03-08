using System.Collections.Generic;

namespace GoogleHashCode.Models
{
    public enum States
    {
        None,
        Impacted,
        Painted,
        Cleared
    }

    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }

        public bool IsEmpty { get; set; }
        public States State { get; set; }

        public int Size { get; set; }
        public float Score { get; set; }
        public List<Cell> White { get; set; }
        public List<Cell> ImpactedCells { get; set; }

        public Cell(int x, int y, char symbol)
        {
            var isDiese = symbol == '#';
            var score = isDiese ? 100F : 0F;

            X = x;
            Y = y;
            Symbol = symbol;
            IsEmpty = !isDiese;
            State = States.None;

            Size = 0;
            Score = score;

            White = new List<Cell>();
            ImpactedCells = new List<Cell>();

            if (!isDiese)
            {
                White.Add(this);
            }

            ImpactedCells.Add(this);
        }
    }
}
