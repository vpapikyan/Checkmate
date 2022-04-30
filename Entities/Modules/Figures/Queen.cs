using Entities.Modules.Figures.Combines;
using Entities.Recite;
using Entities.Recite.Colors;

namespace Entities.Modules.Figures
{
    /// <summary>
    /// This class is about the chess piece of the Queen
    /// </summary>
    public class Queen : IFigure
    {

        public Numbers Number { get; set; }
        public Letters Letter { get; set; }
        public FigursColors Color { get; }
        public CellsColors colorBackgraund { get; set; }
        public List<(Cell, Cell)> FigureHistory { get; set; } = new List<(Cell, Cell)>();


        public Queen(FigursColors color)
        {
            this.Color = color;
        }
        public bool IsMove(Cell cell)
        {
            if (!IsSamePos(cell))
            {
                if (Math.Abs(this.Number - cell.Number) == Math.Abs(this.Letter - cell.Letter)
                    || this.Number == cell.Number || this.Letter == cell.Letter)
                {
                    if (cell.Figur == null)
                    {
                        return true;
                    }
                    else
                    {
                        if (cell.Figur.Color != this.Color)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public bool IsSamePos(Cell cell)
        {
            if (this.Number == cell.Number && this.Letter == cell.Letter)
            {
                return true;
            }
            return false;
        }

    }
}
