using _project.Scripts.Domain.Grid;

namespace _project.Scripts.Presentation
{
    public class UnityGameGridsPresenter
    {
        private IGrid gameGrid;
        public UnityGameGridsPresenter(IGrid gameGrid)
        {
            this.gameGrid = gameGrid;
            var allCells = gameGrid.GetAllCells();
            foreach (var cell in allCells)
            {
                
            }
        }
    }
}