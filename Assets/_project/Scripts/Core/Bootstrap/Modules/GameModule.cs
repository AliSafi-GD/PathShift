using _project.Scripts.Core.Events.Base;
using VContainer;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap.Modules
{
    public static class GameModule
    {
        public static void Install(
            IContainerBuilder builder,
            GameBootstrapper gameBootstrapper,
            GameOverController gameOverController)
        {
            builder.Register<GameEventBus>(Lifetime.Singleton).As<IEventBus>();

            builder.RegisterComponent(gameBootstrapper);
            builder.RegisterComponent(gameOverController);
        }
    }
}
