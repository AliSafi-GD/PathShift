using System.Collections.Generic;
using _project.Scripts.Domain.Grid;
using VContainer.Unity;

namespace _project.Scripts.Presentation
{
    public class GridPresenter : IStartable
    {
        private IGrid gridService;
        private GridFactory factory;
        private CellViewRegistry cellViewRegistry;

        public GridPresenter(IGrid gridService, GridFactory factory, CellViewRegistry cellViewRegistry)
        {
            this.gridService = gridService;
            this.factory = factory;
            this.cellViewRegistry = cellViewRegistry;
        }

        private void Present()
        {
            var gridCells = gridService.GetAllCells();
            var cellViews = factory.CreateVisual(gridCells);
            cellViewRegistry.Register(new Dictionary<int, CellView>(cellViews));
        }

        public void Start()
        {
            Present();
        }
    }
}