using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Spawner;
using _project.Scripts.Core.Stats;
using _project.Scripts.Core.Wave;
using VContainer;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap.Modules
{
    public static class EnemyModule
    {
        public static void Install(
            IContainerBuilder builder,
            EnemyFactory enemyFactory,
            LevelConfig levelConfig)
        {
            builder.RegisterComponent(enemyFactory).As<IEnemyFactory>();

            builder.RegisterInstance(levelConfig);
            builder.Register<EnemyContainer>(Lifetime.Singleton);
            builder.Register<EnemySpawner>(Lifetime.Singleton).As<IEnemySpawner>();
            builder.Register<WaveService>(Lifetime.Singleton).As<IWaveService>();

            // GameStatsService is IStartable; VContainer auto-runs Start().
            builder.RegisterEntryPoint<GameStatsService>(Lifetime.Singleton)
                .AsSelf()
                .As<IGameStatsService>();
        }
    }
}
