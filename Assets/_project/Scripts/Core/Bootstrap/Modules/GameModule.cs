using _project.Scripts.Core.Events.Base;
using _project.Scripts.Core.Infrastructure;
using _project.Scripts.Core.State;
using VContainer;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap.Modules
{
    public static class GameModule
    {
        public static void Install(
            IContainerBuilder builder,
            MouseInputRouter mouseInputRouter,
            GameOverController gameOverController)
        {
            builder.Register<GameEventBus>(Lifetime.Singleton).As<IEventBus>();

            builder.Register<GameStateMachine>(Lifetime.Singleton).As<IGameStateMachine>();
            builder.Register<SceneLoader>(Lifetime.Singleton).As<ISceneLoader>();

            builder.RegisterComponent(mouseInputRouter);
            builder.RegisterComponent(gameOverController);

            // Starts waves and shows the initial path once DI is fully resolved.
            // Implements IDisposable so the wave loop is stopped on scope teardown.
            builder.RegisterEntryPoint<GameLoopStarter>(Lifetime.Singleton);
        }
    }
}
