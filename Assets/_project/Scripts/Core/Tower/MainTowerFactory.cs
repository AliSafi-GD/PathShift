using _project.Scripts.Presentation.View;
using Unity.VisualScripting;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class MainTowerFactory : MonoBehaviour
    {
        public MainTower Create(IMainTowerView view)
        {
            var mb = (MainTowerView)view;
            var health = mb.GetOrAddComponent<UnityHealth>();
            var attackable = mb.GetOrAddComponent<UnityAttackable>();
            // shake/punch روی ضربه خوردن — به IHealth همین آبجکت گوش می‌ده.
            mb.GetOrAddComponent<DamageFlashAnimator>();
            return new MainTower(health, attackable, mb);
        }
    }
}