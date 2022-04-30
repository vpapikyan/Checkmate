using CheckmateUI.UIFunctions;
using Manager;

namespace CheckmateUI.UserInterface
{
    /// <summary>
    /// This class is responsible for working with the User Interface
    /// </summary>
    public class ChessUi
    {
        TaskMethods taskMethods;
        bool IsGoWhite = true;
        List<(int, string)> FiguresNames = new List<(int, string)> { (1, "Queen"), (2, "Rook"), (3, "Bishop"), (4, "Knight") };
        string figureName;
        int leftCursor;
        int topCursor;
        /// <summary>
        /// The function is for launch the game menu
        /// </summary>
        public void Start()
        {
            taskMethods = new TaskMethods();
            UIFunction.Write("", true);
            UIFunction.Clear();
            UIFunction.Write("1)PLay\t2)Knight\tESC)Exit", true);
            ConsoleKey key = UIFunction.GetKey();
            switch (key)
            {
                case ConsoleKey.Escape:
                    return;
                case ConsoleKey.NumPad1:
                case ConsoleKey.D1:
                    this.PlayView();
                    break;
                case ConsoleKey.NumPad2:
                case ConsoleKey.D2:
                    this.PlayWithKnight();
                    break;
                default:
                    this.Start();
                    break;
            }
        }
        private void History()
        {
            UIFunction.Write(taskMethods.GetHistory(), true);
        }
        /// <summary>
        /// The function is starting for play with only knight
        /// </summary>
        private void PlayWithKnight()
        {
            this.Print();
            string startPos;
            string endPos;
            while (true)
            {
                UIFunction.Write("\nPlease select start cell for  knight : ");
                startPos = UIFunction.Read();
                if (taskMethods.GetCellByPosition(startPos) != null)
                {
                    break;
                }
            }
            taskMethods.CreateOnlyKnight();
            taskMethods.AddKnightToBoard(startPos);
            this.Print();
            while (true)
            {
                UIFunction.Write("\nPlease select target cell : ");
                endPos = UIFunction.Read();
                if (taskMethods.GetCellByPosition(endPos) != null)
                {
                    break;
                }
            }
            List<string> steps = taskMethods.GetKnightRoad(startPos, endPos);
            for (int i = 0; i < steps.Count - 1; i++)
            {
                taskMethods.ClearPositions();
                taskMethods.GetCellByPosition(steps[i]);
                taskMethods.GetCellByPosition(steps[i + 1]);
                if (taskMethods.CanFigureMove())
                {
                    Thread.Sleep(1000);
                    this.Print();
                }
                taskMethods.ClearPositions();
            }

            foreach (string item in steps)
            {
                UIFunction.Write($"\n{item}");
            }
            UIFunction.Write($"\nThe total moves count is : {steps.Count - 1}", true);
            UIFunction.Write("PLease press any number to go start...", true);
            UIFunction.GetKey();
            this.Start();
        }
        /// <summary>
        /// This function launches two players game process
        /// </summary>
        private void PlayView()
        {
            if (this.taskMethods.Figurs.Count == 0)
            {
                this.taskMethods.CreateFigires();
                this.taskMethods.AddFiguresOnBoard();
            }
            this.Print();
            string startPos;
            string endPos;
            bool isMat;
            while (true)
            {
                taskMethods.ClearPositions();
                while (true)
                {
                    while (true)
                    {
                        UIFunction.Write("\nPlease select a figure start position for move : ");
                        startPos = UIFunction.Read();
                        if (taskMethods.GetCellByPosition(startPos) != null)
                        {
                            break;
                        }
                    }
                    if (taskMethods.CheckCellFigure())
                    {
                        if (IsGoWhite)
                        {
                            if (taskMethods.GiveFigureColor())
                            {
                                IsGoWhite = false;
                                break;
                            }
                            UIFunction.Write("Please select the Green Figure Position", true);
                        }
                        else
                        {
                            if (!taskMethods.GiveFigureColor())
                            {
                                IsGoWhite = true;
                                break;
                            }
                            UIFunction.Write("Please select the Red Figure Position", true);
                        }
                    }
                    else
                    {
                        UIFunction.Write("please select the position where there is a figure", true);
                    }
                    taskMethods.ClearPositions();
                }
                while (true)
                {
                    UIFunction.Write("Please select target position : ");
                    endPos = UIFunction.Read();
                    if (taskMethods.GetCellByPosition(endPos) != null)
                    {
                        break;
                    }
                    UIFunction.Write("You cant move the selected position please select another position", true);
                }

                if (taskMethods.CanFigureMove())
                {
                    if (taskMethods.IsChangePawn())
                    {
                        UIFunction.Write("\n", true);
                        leftCursor = Console.CursorLeft;
                        topCursor = Console.CursorTop;
                        figureName = this.SelectChangedFigure();
                        taskMethods.ChangePawn(figureName);
                    }
                    if (taskMethods.IsMat())
                    {
                        this.Print();
                        string winerColor;
                        if (IsGoWhite)
                        {
                            winerColor = "Red";
                        }
                        else
                        {
                            winerColor = "Green";
                        }
                        UIFunction.Write("\nThe game is over.Winer is " + winerColor, true);
                        IsGoWhite = true;
                        this.History();
                        UIFunction.GetKey();
                        this.Start();
                    }
                    if (taskMethods.IsPat())
                    {
                        this.Print();
                        UIFunction.Write("\nThe game is over.", true);
                        IsGoWhite = true;
                        UIFunction.GetKey();
                        this.Start();
                    }
                    this.PlayView();
                }
                if (IsGoWhite)
                {
                    IsGoWhite = false;
                }
                else
                {
                    IsGoWhite = true;
                }
                UIFunction.Write("PLease do all angain", true);
            }
        }
        /// <summary>
        /// The function prints chess board and figures
        /// </summary>
        private void Print()
        {
            UIFunction.Clear();
            for (int i = 0; i < this.taskMethods.board.Width / 2; i++)
            {
                UIFunction.Write($"{this.taskMethods.board.Numbers[i]}|");
                for (int j = (i * 8); j < ((i * 8) + 8); j++)
                {
                    string toWrite;
                    if (this.taskMethods.board.Cells[j].Figur != null)
                    {
                        Console.BackgroundColor = (ConsoleColor)this.taskMethods.board.Cells[j].Figur.colorBackgraund;
                    }
                    else
                    {
                        Console.BackgroundColor = (ConsoleColor)this.taskMethods.board.Cells[j].Color;
                    }
                    if (this.taskMethods.board.Cells[j].Figur == null)
                    {
                        toWrite = "  ";
                    }
                    else
                    {
                        toWrite = $"{(this.taskMethods.board.Cells[j].Figur.GetType().Name)[0]}{(this.taskMethods.board.Cells[j].Figur.GetType().Name)[1]}";
                        Console.ForegroundColor = (ConsoleColor)this.taskMethods.board.Cells[j].Figur.Color;
                    }
                    UIFunction.Write(toWrite);
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                UIFunction.Write("", true);
            }
            UIFunction.Write("  ");
            for (int i = 0; i < this.taskMethods.board.Letters.Count; i++)
            {
                UIFunction.Write($"{this.taskMethods.board.Letters[i]} ");
            }
        }
        private string SelectChangedFigure()
        {
            Console.SetCursorPosition(leftCursor, topCursor);
            string changedFigureName = "";
            UIFunction.Write("Please select a figur to change with your pawn", true);
            for (int i = 0; i < FiguresNames.Count; i++)
            {
                UIFunction.Write($"{FiguresNames[i].Item1}){FiguresNames[i].Item2}", true);
            }
            ConsoleKey key = UIFunction.GetKey();
            switch (key)
            {
                case ConsoleKey.NumPad1:
                case ConsoleKey.D1:
                    changedFigureName = FiguresNames[0].Item2;
                    break;
                case ConsoleKey.NumPad2:
                case ConsoleKey.D2:
                    changedFigureName = FiguresNames[1].Item2;
                    break;
                case ConsoleKey.NumPad3:
                case ConsoleKey.D3:
                    changedFigureName = FiguresNames[2].Item2;
                    break;
                case ConsoleKey.NumPad4:
                case ConsoleKey.D4:
                    changedFigureName = FiguresNames[3].Item2;
                    break;
                default:
                    this.SelectChangedFigure();
                    break;
            }
            return changedFigureName;
        }
    }
}
