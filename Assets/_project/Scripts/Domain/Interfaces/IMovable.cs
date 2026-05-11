using System;

namespace _project.Scripts.Domain.Interfaces
{
    public interface IMovable : IBehavior
    {
        event Action OnFinishedMove;
        void Move();
    }
}