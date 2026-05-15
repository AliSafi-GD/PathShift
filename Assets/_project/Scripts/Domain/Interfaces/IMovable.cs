using System;
using System.Collections.Generic;
using _project.Scripts.Domain.Grid;

namespace _project.Scripts.Domain.Interfaces
{
    public interface IMovable : IBehavior
    {
        GridCell CurrentCell { get; }
        event Action OnFinishedMove;
        void SetPath(IReadOnlyList<GridCell> path);
        void Move();
    }
}
