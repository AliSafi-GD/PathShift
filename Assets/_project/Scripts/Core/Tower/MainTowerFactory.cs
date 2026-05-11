using _project.Scripts.Presentation.View;
using Unity.VisualScripting;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class MainTowerFactory : MonoBehaviour
    {
        public MainTower Create(IMainTowerView view)
        {
            var health = ((MainTowerView)view).GetOrAddComponent<UnityHealth>();
            var attackable = ((MainTowerView)view).GetOrAddComponent<UnityAttackable>();
            return new MainTower(health, attackable, ((MainTowerView)view));
        }
    }
}