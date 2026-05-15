using _project.Scripts.Core.Tower;
using _project.Scripts.Presentation.View;
using VContainer;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap.Modules
{
    public static class TowerModule
    {
        public static void Install(
            IContainerBuilder builder,
            TowerFactory towerFactory,
            TowerAttackSystem towerAttackSystem,
            MortarProjectileFactory mortarProjectileFactory,
            PlacementPreviewController placementPreviewController,
            TowerRangeIndicator towerRangeIndicator)
        {
            builder.RegisterComponent(towerFactory);
            builder.RegisterComponent(towerAttackSystem);

            if (mortarProjectileFactory != null)
                builder.RegisterComponent(mortarProjectileFactory);

            builder.Register<PlacedTowerRegistry>(Lifetime.Singleton).As<IPlacedTowerRegistry>();
            builder.Register<TowerPlacementService>(Lifetime.Singleton).As<ITowerPlacementService>();
            builder.Register<PlacementCommitter>(Lifetime.Singleton).As<IPlacementCommitter>();
            builder.Register<TowerActionsService>(Lifetime.Singleton).As<ITowerActionsService>();

            if (placementPreviewController != null)
                builder.RegisterComponent(placementPreviewController);

            if (towerRangeIndicator != null)
                builder.RegisterComponent(towerRangeIndicator);
        }
    }
}
