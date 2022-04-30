using Entities.Modules;
using Entities.Modules.Figures;
using Entities.Modules.Figures.Combines;
using Entities.Recite;
using Entities.Recite.Colors;

namespace Manager
{
    /// <summary>
    /// This class is responsible for working with the two players game process
    /// </summary>
    public class TwoPlayersLogic
    {
        IFigure figur;
        IFigure newFigur;
        Cell startRookRakirovka;
        Cell endRookRakirovka;

        #region Functions

        #region Public
        public bool CanMove(Cell startCell, Cell endCell, Board board)
        {
            figur = startCell.Figur;
            if (this.CanFigureMove(startCell, endCell, board))
            {
                if (this.CanRakirovka(startCell, endCell, board))
                {
                    this.Rakirovka(startCell, endCell, board);
                    return true;
                }

                this.Move(startCell, endCell, board);

                if (this.CheckYourKingShah(endCell.Figur.Color, board))
                {
                    board.Cells.FirstOrDefault(cell => cell == endCell).Figur = null;
                    board.Cells.FirstOrDefault(cell => cell == startCell).Figur = newFigur;
                    board.Cells.FirstOrDefault(cell => cell == startCell).Figur.colorBackgraund = startCell.Color;
                    return false;
                }
                //es else-y petqa hanem bayc ira gorcaruyty pti mna vor ete irak karoly shaxi tak chi guyny dzi
                else
                {
                    Cell kingCell;
                    if (endCell.Figur.Color == FigursColors.White)
                    {
                        kingCell = this.GetOpponentKing(FigursColors.Black, board);
                    }
                    else
                    {
                        kingCell = this.GetOpponentKing(FigursColors.White, board);
                    }
                    if (kingCell != null)
                    {
                        this.ChangeFigurBackgraund(kingCell, kingCell.Color, board);
                    }
                }
                return true;
            }

            return false;
        }
        public bool IsPat(FigursColors figurColor, Board board)
        {
            List<Cell> figures;
            if (figurColor == FigursColors.Black)
            {
                figures = this.GetOpponentFigures(FigursColors.White, board);
            }
            else
            {
                figures = this.GetOpponentFigures(FigursColors.Black, board);
            }
            for (int i = 0; i < figures.Count; i++)
            {
                for (int j = 0; j < board.Cells.Count; j++)
                {
                    if (this.CanFigureMove(figures[i], board.Cells[j], board))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public void ChangeFigure(Cell cell, IFigure figure, Board board)
        {
            board.Cells.FirstOrDefault(c => c == cell).Figur = figure;
            board.Cells.FirstOrDefault(c => c == cell).Figur.Number = cell.Number;
            board.Cells.FirstOrDefault(c => c == cell).Figur.Letter = cell.Letter;
            board.Cells.FirstOrDefault(c => c == cell).Figur.colorBackgraund = board.Cells.FirstOrDefault(c => c == cell).Color;
        }
        public bool IsMat(Cell cell, Board board)
        {
            if (this.CheckKingShah(cell, board))
            {
                List<Cell> opponentFigures = this.GetOpponentFigures(cell.Figur.Color, board);
                Cell KingCell = this.GetOpponentKing(cell.Figur.Color, board);
                List<Cell> Road = this.GetRoad(cell, KingCell, board);
                Road.Add(cell);
                if (this.CanKeepKing(opponentFigures, Road, board))
                {
                    this.ChangeFigurBackgraund(KingCell, CellsColors.DarkYellow, board);
                    return false;
                }
                if (this.CanKingEscape(KingCell, board))
                {
                    this.ChangeFigurBackgraund(KingCell, CellsColors.DarkYellow, board);
                    return false;
                }
                this.ChangeFigurBackgraund(KingCell, CellsColors.Blue, board);
                return true;
            }
            return false;
        }
        #endregion

        #region Private

        private void Move(Cell startCell, Cell endCell, Board board)
        {
            figur = startCell.Figur;
            newFigur = this.GetFigure(figur);
            newFigur.Number = endCell.Number;
            newFigur.Letter = endCell.Letter;
            newFigur.FigureHistory.AddRange(figur.FigureHistory);
            newFigur.FigureHistory.Add((startCell, endCell));
            board.History.Add((figur, (startCell, endCell)));
            board.Cells.FirstOrDefault(cell => cell == endCell).Figur = newFigur;
            board.Cells.FirstOrDefault(cell => cell == startCell).Figur = null;
            board.Cells.FirstOrDefault(cell => cell == endCell).Figur.colorBackgraund = endCell.Color;
        }
        private bool IsTheLastHistory((Cell, Cell) lHist, Board board)
        {
            int length = board.History.Count;
            return board.History[length - 1].Item2 == lHist;
        }

        #region About Figures
        private bool CanFigureMove(Cell startCell, Cell endCell, Board board)
        {

            List<Cell> Road = new List<Cell>();

            figur = startCell.Figur;
            if (figur.GetType() == typeof(King))
            {
                if (CanRakirovka(startCell, endCell, board))
                {
                    return true;
                }
            }
            if (figur.GetType() == typeof(Pawn))
            {
                if (this.CanPawnEat(startCell, endCell, board))
                {
                    return true;
                }
            }
            if (figur.IsMove(endCell))
            {
                if (figur.GetType() == typeof(Knight))
                {
                    return true;
                }
                else if (figur.GetType() == typeof(King))
                {
                    if (this.CanKingMove(startCell, endCell, board))
                    {
                        return true;
                    }
                }
                else
                {
                    Road = this.GetRoad(startCell, endCell, board);
                    return this.IsRoadEmpty(Road);
                }
            }
            return false;

        }
        private IFigure GetFigure(IFigure figur)
        {
            newFigur = null;
            if (figur.GetType() == typeof(Knight))
            {
                newFigur = new Knight(figur.Color);
            }
            else if (figur.GetType() == typeof(Pawn))
            {
                newFigur = new Pawn(figur.Color);
            }
            else if (figur.GetType() == typeof(Bishop))
            {
                newFigur = new Bishop(figur.Color);
            }
            else if (figur.GetType() == typeof(Rook))
            {
                newFigur = new Rook(figur.Color);
            }
            else if (figur.GetType() == typeof(Queen))
            {
                newFigur = new Queen(figur.Color);
            }
            else if (figur.GetType() == typeof(King))
            {
                newFigur = new King(figur.Color);
            }
            return newFigur;
        }
        private void ChangeFigurBackgraund(Cell cell, CellsColors color, Board board)
        {
            board.Cells.FirstOrDefault(c => c == cell).Figur.colorBackgraund = color;
        }
        private List<Cell> GetOpponentFigures(FigursColors figursColors, Board board)
        {
            List<Cell> list = new List<Cell>();
            for (int i = 0; i < board.Cells.Count; i++)
            {
                if (board.Cells[i].Figur != null)
                {
                    if (figursColors != board.Cells[i].Figur.Color)
                    {
                        list.Add(board.Cells[i]);
                    }
                }
            }
            return list;
        }
        private bool CanPawnEat(Cell startCell, Cell endCell, Board board)
        {
            figur = startCell.Figur;
            if (!figur.IsMove(endCell))
            {
                if (endCell.Figur == null)
                {
                    string possition = (char)endCell.Letter + ((int)startCell.Number).ToString();
                    Cell cell = board.GetCellByPosition(possition);
                    if (cell.Figur != null && cell.Figur.Color != figur.Color)
                    {
                        if (cell.Figur.FigureHistory.Count == 1 && this.IsTheLastHistory(cell.Figur.FigureHistory[0], board))
                        {
                            board.Cells.FirstOrDefault(c => c == cell).Figur = null;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        #region About King
        private Cell GetOpponentKing(FigursColors figursColors, Board board)
        {
            for (int i = 0; i < board.Cells.Count; i++)
            {
                if (board.Cells[i].Figur != null)
                {
                    if (board.Cells[i].Figur.GetType() == typeof(King) && board.Cells[i].Figur.Color != figursColors)
                    {
                        return board.Cells[i];
                    }
                }
            }
            return null;
        }
        private bool CanKingMove(Cell startCell, Cell endCell, Board board)
        {
            FigursColors figursColors = startCell.Figur.Color;
            if (endCell.Figur == null)
            {
                if (!IsKingShah(endCell, figursColors, board))
                {
                    return true;
                }
            }
            else
            {
                if (endCell.Figur.Color != figursColors)
                {
                    Cell tempCell = new Cell(endCell);
                    if (!IsKingShah(tempCell, figursColors, board))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool IsKingShah(Cell cell, FigursColors figurColor, Board board)
        {
            List<Cell> cells = this.GetOpponentFigures(figurColor, board);
            for (int i = 0; i < cells.Count; i++)
            {
                if (this.CanFigureMove(cells[i], cell, board))
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckKingShah(Cell cell, Board board)
        {
            Cell endCell = this.GetOpponentKing(cell.Figur.Color, board);
            if (this.CanFigureMove(cell, endCell, board))
            {
                this.ChangeFigurBackgraund(endCell, CellsColors.DarkYellow, board);
                return true;
            }
            else
            {
                this.ChangeFigurBackgraund(endCell, endCell.Color, board);
                return false;
            }
        }
        private bool CheckYourKingShah(FigursColors figurColor, Board board)
        {
            Cell endCell;
            if (figurColor == FigursColors.Black)
            {
                endCell = this.GetOpponentKing(FigursColors.White, board);
            }
            else
            {
                endCell = this.GetOpponentKing(FigursColors.Black, board);
            }
            if (endCell == null)
            {
                return false;
            }
            return IsKingShah(endCell, endCell.Figur.Color, board);
        }
        private bool CanRakirovka(Cell startCell, Cell endCell, Board board)
        {
            figur = startCell.Figur;

            if (figur.GetType() == typeof(King))
            {
                if (figur.FigureHistory.Count == 0)
                {
                    if (Math.Abs(startCell.Letter - endCell.Letter) == 2)
                    {
                        Cell rook1Cell = board.Cells.FirstOrDefault(c => c.Figur != null && c.Figur.GetType() == typeof(Rook) && c.Figur.Color == figur.Color);
                        Cell rook2Cell = board.Cells.LastOrDefault(c => c.Figur != null && c.Figur.GetType() == typeof(Rook) && c.Figur.Color == figur.Color);
                        if (rook1Cell != null && rook1Cell.Figur.FigureHistory.Count == 0)
                        {
                            if ((int)(endCell.Letter - rook1Cell.Letter) == 2)
                            {
                                List<Cell> road = this.GetRoad(rook1Cell, startCell, board);
                                if (this.IsRoadEmpty(road))
                                {
                                    if (!IsKingShah(endCell, figur.Color, board))
                                    {
                                        startRookRakirovka = rook1Cell;
                                        endRookRakirovka = board.Cells.FirstOrDefault(c => c.Letter == Letters.D && c.Number == rook1Cell.Number);
                                        return true;
                                    }
                                }
                            }
                        }
                        if (rook2Cell != null && rook2Cell.Figur.FigureHistory.Count == 0)
                        {
                            if ((int)(rook2Cell.Letter - endCell.Letter) == 1)
                            {
                                List<Cell> road = this.GetRoad(rook2Cell, startCell, board);
                                if (this.IsRoadEmpty(road))
                                {
                                    if (!IsKingShah(endCell, figur.Color, board))
                                    {
                                        startRookRakirovka = rook2Cell;
                                        endRookRakirovka = board.Cells.FirstOrDefault(c => c.Letter == Letters.F && c.Number == rook2Cell.Number);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        private void Rakirovka(Cell startCell, Cell endCell, Board board)
        {
            this.Move(startCell, endCell, board);
            this.Move(startRookRakirovka, endRookRakirovka, board);
        }
        private bool CanKeepKing(List<Cell> opponentFigures, List<Cell> Road, Board board)
        {
            for (int i = 0; i < opponentFigures.Count; i++)
            {
                for (int j = 0; j < Road.Count; j++)
                {
                    if (this.CanFigureMove(opponentFigures[i], Road[j], board))
                    {
                        IFigure attFigur = board.Cells.FirstOrDefault(c => c == opponentFigures[i]).Figur;
                        IFigure tempfigure = null;
                        bool haveRoadJFigure;
                        if (board.Cells.FirstOrDefault(c => c == Road[j]).Figur != null)
                        {
                            tempfigure = board.Cells.FirstOrDefault(c => c == Road[j]).Figur;
                        }
                        board.Cells.FirstOrDefault(c => c == Road[j]).Figur = attFigur;
                        board.Cells.FirstOrDefault(c => c == opponentFigures[i]).Figur = null;
                        if (!CheckYourKingShah(Road[j].Figur.Color, board))
                        {
                            board.Cells.FirstOrDefault(c => c == opponentFigures[i]).Figur = attFigur;
                            board.Cells.FirstOrDefault(c => c == Road[j]).Figur = tempfigure;
                            return true;
                        }
                        board.Cells.FirstOrDefault(c => c == opponentFigures[i]).Figur = attFigur;
                        board.Cells.FirstOrDefault(c => c == Road[j]).Figur = tempfigure;
                    }
                }
            }
            return false;

        }
        private bool CanKingEscape(Cell kingCell, Board board)
        {
            List<Cell> kingPosPosit = this.GetKingPossPosit(kingCell, board);
            foreach (var item in kingPosPosit)
            {
                if (CanKingMove(kingCell, item, board))
                {
                    return true;
                }
            }
            return false;
        }
        private List<Cell> GetKingPossPosit(Cell kingCell, Board board)
        {
            List<Cell> possPosit = new List<Cell>();
            foreach (var item in board.Cells)
            {
                if (Math.Abs(item.Letter - kingCell.Letter) <= 1 && Math.Abs(item.Number - kingCell.Number) <= 1 && !kingCell.Figur.IsSamePos(item))
                {
                    possPosit.Add(item);
                }
            }
            return possPosit;
        }
        #endregion

        #region About Road
        private List<Cell> GetRoad(Cell startCell, Cell endCell, Board board)
        {
            List<Cell> Road = new List<Cell>();

            figur = startCell.Figur;

            if (figur.GetType() == typeof(Pawn))
            {
                Road = this.PawnMoveRoad(startCell, endCell, board);
            }
            else if (figur.GetType() == typeof(Bishop))
            {
                Road = this.BishopMoveRoad(startCell, endCell, board);
            }
            else if (figur.GetType() == typeof(Rook))
            {
                Road = this.RookMoveRoad(startCell, endCell, board);
            }
            else if (figur.GetType() == typeof(Queen))
            {
                Road = this.QueenMoveRoad(startCell, endCell, board);
            }
            return Road;
        }
        private List<Cell> PawnMoveRoad(Cell startCell, Cell endCell, Board board)
        {
            List<Cell> list = new List<Cell>();
            if (Math.Abs(startCell.Number - endCell.Number) == 2)
            {
                int start;
                if (startCell.Figur.Color == FigursColors.Black)
                {
                    start = (int)endCell.Number + 1;
                }
                else
                {
                    start = (int)startCell.Number + 1;
                }
                string position = (char)startCell.Letter + start.ToString();
                Cell cell = board.GetCellByPosition(position);
                list.Add(cell);
            }
            return list;
        }
        private List<Cell> BishopMoveRoad(Cell startCell, Cell endCell, Board board)
        {
            List<Cell> bishopRoad = new List<Cell>();
            int startNum;
            int endNum;
            char startLet;

            if (startCell.Letter > endCell.Letter)
            {
                startLet = (char)(startCell.Letter - 1);
            }
            else
            {
                startLet = (char)(startCell.Letter + 1);
            }
            if (startCell.Number > endCell.Number)
            {
                startNum = (int)(endCell.Number + 1);
                endNum = (int)(startCell.Number - 1);
            }
            else
            {
                startNum = (int)(startCell.Number + 1);
                endNum = (int)(endCell.Number - 1);
            }
            for (int i = startNum; i <= endNum; i++)
            {
                string position = startLet + i.ToString();
                Cell cell = board.GetCellByPosition(position);
                bishopRoad.Add(cell);
                startLet++;
            }

            return bishopRoad;
        }
        private List<Cell> RookMoveRoad(Cell startCell, Cell endCell, Board board)
        {
            List<Cell> rookRoad = new List<Cell>();
            char let;
            char endLet;
            int num;
            int endNum;

            if (startCell.Number == endCell.Number)
            {
                num = (int)startCell.Number;
                if (startCell.Letter > endCell.Letter)
                {
                    let = (char)(endCell.Letter + 1);
                    endLet = (char)(startCell.Letter - 1);
                }
                else
                {
                    let = (char)(startCell.Letter + 1);
                    endLet = (char)(endCell.Letter - 1);
                }
                for (char letter = let; letter <= endLet; letter++)
                {
                    string position = letter + num.ToString();
                    Cell cell = board.GetCellByPosition(position);
                    rookRoad.Add(cell);
                }
            }
            else
            {
                let = (char)(startCell.Letter);
                if (startCell.Number > endCell.Number)
                {
                    num = (int)(endCell.Number + 1);
                    endNum = (int)(startCell.Number - 1);
                }
                else
                {
                    num = (int)(startCell.Number + 1);
                    endNum = (int)(endCell.Number - 1);
                }
                for (int number = num; number <= endNum; number++)
                {
                    string position = let + number.ToString();
                    Cell cell = board.GetCellByPosition(position);
                    rookRoad.Add(cell);
                }
            }
            return rookRoad;
        }
        private List<Cell> QueenMoveRoad(Cell startCell, Cell endCell, Board board)
        {
            List<Cell> queenRoad = new List<Cell>();
            if (startCell.Number == endCell.Number || startCell.Letter == endCell.Letter)
            {
                queenRoad = this.RookMoveRoad(startCell, endCell, board);
            }
            else
            {
                queenRoad = this.BishopMoveRoad(startCell, endCell, board);
            }
            return queenRoad;
        }
        private bool IsRoadEmpty(List<Cell> road)
        {
            for (int i = 0; i < road.Count; i++)
            {
                if (road[i].Figur != null)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
        #endregion
        #endregion

    }
}