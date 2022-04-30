using Entities.Recite;
using Entities.Recite.Colors;

namespace Entities.Modules.Figures.Combines
{
    /// <summary>
    /// This interface is about a chess figure
    /// </summary>
    public interface IFigure
    {
        Numbers Number { get; set; }
        Letters Letter { get; set; }
        FigursColors Color { get; }
        List<(Cell, Cell)> FigureHistory { get; set; }
        CellsColors colorBackgraund { get; set; }
        /// <summary>
        /// This function ensures the theoretical movement of the figure, ie theoretically the figure can be moved to a given cell
        /// </summary>
        /// <param name="cell">The Cell where the figure we want to know can go</param>
        /// <returns>If the function returns true, it is theoretically possible. Otherwise false</returns>
        bool IsMove(Cell cell);
        /// <summary>
        /// This function checks if the cells are the same
        /// </summary>
        /// <param name="cell">Cell to compare with</param>
        /// <returns>If it returns true, then the cells are the same. Otherwise false</returns>
        bool IsSamePos(Cell cell);
    }
}
