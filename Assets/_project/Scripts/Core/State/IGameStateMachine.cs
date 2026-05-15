using System;

namespace _project.Scripts.Core.State
{
    public interface IGameStateMachine
    {
        GameState Current { get; }
        bool IsFinished { get; }

        event Action<GameState> OnStateChanged;

        void Pause();
        void Resume();
        void Win();
        void Lose();
    }
}
