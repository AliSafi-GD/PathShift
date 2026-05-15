using _project.Scripts.UI.Economy;
using _project.Scripts.UI.Stats;
using _project.Scripts.UI.Tower;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap.Modules
{
    public static class UIModule
    {
        public static void Install(
            IContainerBuilder builder,
            TowerActionsController towerActionsController)
        {
            if (towerActionsController != null)
                builder.RegisterComponent(towerActionsController);

            // HUDs live in the scene; auto-register every instance so VContainer can inject them.
            foreach (var hud in Object.FindObjectsByType<CurrencyHudView>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
                builder.RegisterComponent(hud);

            foreach (var hud in Object.FindObjectsByType<GameStateHudView>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
                builder.RegisterComponent(hud);
        }
    }
}
