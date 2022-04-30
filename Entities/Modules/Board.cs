using Entities.Modules.Figures;
using Entities.Modules.Figures.Combines;
using Entities.Recite;
using Entities.Recite.Colors;

namespace Entities.Modules
{
    /// <summary>
    /// This class is about a chess board
    /// </summary>
    public class Board
    {
        public Board()
        {
            this.CreateBoard();
        }
        public int Width { get; } = 16;
        public List<int> Numbers { get; set; } = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
        public List<char> Letters { get; set; } = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public List<Cell> Cells { get; private set; } = new List<Cell>(64);
        public List<(IFigure, (Cell, Cell))> History { get; set; } = new List<(IFigure, (Cell, Cell))>();

        /// <summary>
        /// This function creates a standard chessboard
        /// </summary>
        private void CreateBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                int letter = 65;
                for (int j = 0; j < 8; j++)
                {
                    Cell cell;
                    if ((i + j) % 2 == 0)
                    {
                        cell = new Cell((Numbers)Numbers[i], (Letters)letter, CellsColors.Black);
                    }
                    else
                    {
                        cell = new Cell((Numbers)Numbers[i], (Letters)letter, CellsColors.White);
                    }
                    Cells.Add(cell);
                    letter++;
                }

            }
        }

        /// <summary>
        /// This function checks whether the position is on the chessboard or not
        /// </summary>
        /// <param name="position">This is the position we want to know</param>
        /// <returns>This function will return true if there is a cell in that position and that cell is on the chessboard. Otherwise false</returns>
        public bool IsTruePosition(string position)
        {
            if (position.Length == 2)
            {
                int digite;
                bool isDigite = int.TryParse(position[1].ToString(), out digite);
                if (isDigite)
                {
                    char letter = position[0].ToString().ToUpper()[0];
                    if (digite > 0 && digite <= 8 && letter >= 'A' && letter <= 'H')
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool CheckAreFigureInCell(Cell cell)
        {
            if (cell.Figur != null)
            {
                return true;
            }
            return false;
        }
        public Cell GetCellByPosition(string position)
        {
            if (this.IsTruePosition(position))
            {
                int num;
                int.TryParse(position[1].ToString(), out num);
                char letter = position[0].ToString().ToUpper()[0];
                for (int i = 0; i < this.Cells.Count; i++)
                {
                    if (this.Cells[i].Number == (Numbers)num && this.Cells[i].Letter == (Letters)letter)
                    {
                        return this.Cells[i];
                    }
                }
            }
            return null;
        }
        public List<IFigure> CreateFigires()
        {
            List<IFigure> Figurs = new List<IFigure>();
            FigursColors figursColors;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    figursColors = FigursColors.White;
                }
                else
                {
                    figursColors = FigursColors.Black;
                }

                Rook rook1 = new Rook(figursColors);
                Knight knigth1 = new Knight(figursColors);
                Bishop bishop1 = new Bishop(figursColors);
                Queen queen = new Queen(figursColors);
                King king = new King(figursColors);
                Rook rook2 = new Rook(figursColors);
                Knight knigth2 = new Knight(figursColors);
                Bishop bishop2 = new Bishop(figursColors);
                Figurs.Add(rook1);
                Figurs.Add(knigth1);
                Figurs.Add(bishop1);
                Figurs.Add(queen);
                Figurs.Add(king);
                Figurs.Add(bishop2);
                Figurs.Add(knigth2);
                Figurs.Add(rook2);
                for (int j = 0; j < 8; j++)
                {
                    Pawn pawn = new Pawn(figursColors);
                    Figurs.Add(pawn);
                }
            }
            return Figurs;
        }
        public bool ChekCellFigure(Cell cell)
        {
            return this.CheckAreFigureInCell(cell);
        }
        public void AddFiguresOnBoard(List<IFigure> Figurs)
        {
            if (CanAddFiguresOnBoard())
            {
                int cellIndex = 0;
                for (int i = 0; i < Figurs.Count; i++)
                {
                    if (i == Figurs.Count / 2)
                    {
                        cellIndex = 56;
                    }
                    else if (i == Figurs.Count - 8)
                    {
                        cellIndex = 48;
                    }
                    this.Cells[cellIndex].Figur = Figurs[i];
                    this.Cells[cellIndex].Figur.Number = this.Cells[cellIndex].Number;
                    this.Cells[cellIndex].Figur.Letter = this.Cells[cellIndex].Letter;
                    this.Cells[cellIndex].Figur.colorBackgraund = this.Cells[cellIndex].Color;
                    cellIndex++;
                }
            }
        }
        public void AddKnightToBoard(Cell cell, Knight knight)
        {
            this.Cells.FirstOrDefault(c => c == cell).Figur = knight;
            this.Cells.FirstOrDefault(c => c == cell).Figur.Number = cell.Number;
            this.Cells.FirstOrDefault(c => c == cell).Figur.Letter = cell.Letter;
        }
        private bool CanAddFiguresOnBoard()
        {
            for (int i = 0; i < this.Cells.Count; i++)
            {
                if (this.Cells[i].Figur != null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
