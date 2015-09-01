using System.Collections;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class Maze
    {
        /// <summary>
        /// The k dimension.
        /// </summary>
        public static int kDimension = 50;

        /// <summary>
        /// The cell stack.
        /// </summary>
        private readonly Stack CellStack = new Stack();

        /// <summary>
        /// The cells.
        /// </summary>
        public Cell[,] Cells;

        /// <summary>
        /// The current cell.
        /// </summary>
        private Cell CurrentCell;

        /// <summary>
        /// The total cells.
        /// </summary>
        private int TotalCells = kDimension * kDimension;

        /// <summary>
        /// The visited cells.
        /// </summary>
        private int VisitedCells = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Maze"/> class.
        /// </summary>
        public Maze()
        {
            // TODO: Add constructor logic here
            Initialize();
        }

        /// <summary>
        /// The get neighbors with walls.
        /// </summary>
        /// <param name="aCell">
        /// The a cell.
        /// </param>
        /// <returns>
        /// The <see cref="ArrayList"/>.
        /// </returns>
        private ArrayList GetNeighborsWithWalls(Cell aCell)
        {
            var Neighbors = new ArrayList();
            var count = 0;
            for (var countRow = -1; countRow <= 1; countRow++)
                for (var countCol = -1; countCol <= 1; countCol++)
                {
                    if ((aCell.Row + countRow < kDimension) &&
                        (aCell.Column + countCol < kDimension) &&
                        (aCell.Row + countRow >= 0) &&
                        (aCell.Column + countCol >= 0) &&
                        ((countCol == 0) || (countRow == 0)) &&
                        (countRow != countCol)
                        )
                    {
                        if (Cells[aCell.Row + countRow, aCell.Column + countCol].HasAllWalls())
                        {
                            Neighbors.Add(Cells[aCell.Row + countRow, aCell.Column + countCol]);
                        }
                    }
                }

            return Neighbors;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
            Cells = new Cell[kDimension, kDimension];
            TotalCells = kDimension * kDimension;
            for (var i = 0; i < kDimension; i++)
                for (var j = 0; j < kDimension; j++)
                {
                    Cells[i, j] = new Cell();
                    Cells[i, j].Row = i;
                    Cells[i, j].Column = j;
                }

            CurrentCell = Cells[0, 0];
            VisitedCells = 1;
            CellStack.Clear();
        }

        /// <summary>
        /// The generate.
        /// </summary>
        public void Generate()
        {
            while (VisitedCells < TotalCells)
            {
                // get a list of the neighboring cells with all walls intact
                var AdjacentCells = GetNeighborsWithWalls(CurrentCell);

                // test if a cell like this exists
                if (AdjacentCells.Count > 0)
                {
                    // yes, choose one of them, and knock down the wall between it and the current cell
                    var randomCell = Cell.TheRandom.Next(0, AdjacentCells.Count);
                    var theCell = (Cell)AdjacentCells[randomCell];
                    CurrentCell.KnockDownWall(theCell);
                    CellStack.Push(CurrentCell); // push the current cell onto the stack
                    CurrentCell = theCell; // make the random neighbor the new current cell
                    VisitedCells++;
                }
                else
                {
                    // No cells with walls intact, pop current cell from stack
                    CurrentCell = (Cell)CellStack.Pop();
                }
            }
        }
    }
}