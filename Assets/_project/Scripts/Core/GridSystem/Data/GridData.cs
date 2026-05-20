using System.Collections.Generic;
using UnityEngine;

namespace _project.Scripts.Core.GridSystem.Data
{
    /// <summary>
    /// Design-time data for a single map. Pure logical positions — no world space,
    /// no cell size, no origin. View concerns live elsewhere.
    ///
    /// Roles are stored as separate position lists for simple inspector UX with the
    /// current small set (Normal/Spawn/Core). When a 4th role is needed, consider
    /// switching to <c>[SerializeReference]</c> with a polymorphic CellRole field.
    /// </summary>
    [CreateAssetMenu(fileName = "GridData", menuName = "PathShift/Grid Data")]
    public sealed class GridData : ScriptableObject
    {
        [SerializeField] private List<GridPosition> normalCells = new();
        [SerializeField] private List<GridPosition> spawnCells = new();
        [SerializeField] private List<GridPosition> coreCells = new();

        public IReadOnlyList<GridPosition> NormalCells => normalCells;
        public IReadOnlyList<GridPosition> SpawnCells => spawnCells;
        public IReadOnlyList<GridPosition> CoreCells => coreCells;
    }
}
