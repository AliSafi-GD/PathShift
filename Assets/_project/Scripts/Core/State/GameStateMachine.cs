using System;
using UnityEngine;

namespace _project.Scripts.Core.State
{
    public class GameStateMachine : IGameStateMachine
    {
        public GameState Current { get; private set; } = GameState.Playing;
        public bool IsFinished => Current == GameState.Won || Current == GameState.Lost;

        public event Action<GameState> OnStateChanged;

        public void Pause()
        {
            if (IsFinished || Current == GameState.Paused) return;
            Transition(GameState.Paused);
        }

        public void Resume()
        {
            if (Current != GameState.Paused) return;
            Transition(GameState.Playing);
        }

        public void Win()
        {
            if (IsFinished) return;
            Transition(GameState.Won);
        }

        public void Lose()
        {
            if (IsFinished) return;
            Transition(GameState.Lost);
        }

        private void Transition(GameState next)
        {
            Current = next;
            Time.timeScale = (next == GameState.Playing) ? 1f : 0f;
            OnStateChanged?.Invoke(next);
        }
    }
}
