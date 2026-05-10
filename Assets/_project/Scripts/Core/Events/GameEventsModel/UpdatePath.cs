using System.Collections.Generic;
using _project.Scripts.Domain.Grid;

namespace _project.Scripts.Core.Events.GameEventsModel
{
    public class UpdatePath
    {
        List<GridCell> cells = new List<GridCell>();

        public UpdatePath(List<GridCell> cells)
        {
            this.cells = cells;
        }
        public List<GridCell> Cells => cells;
    }
    
    public class PreviewUpdatePath
    {
        List<GridCell> cells = new List<GridCell>();

        public PreviewUpdatePath(List<GridCell> cells)
        {
            this.cells = cells;
        }
        public List<GridCell> Cells => cells;
    }
}