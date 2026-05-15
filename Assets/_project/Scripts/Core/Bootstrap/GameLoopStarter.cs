using System;
using System.Collections.Generic;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Core.Wave;
using _project.Scripts.Presentation;
using UnityEngine;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap
{
    public class GameLoopStarter : IStartable, IDisposable
    {
        private readonly IPathService pathService;
        private readonly IWaveService waveService;
        private readonly IMainPathVisualizer pathVisualizer;

        public GameLoopStarter(
            IPathService pathService,
            IWaveService waveService,
            IMainPathVisualizer pathVisualizer)
        {
            this.pathService = pathService;
            this.waveService = waveService;
            this.pathVisualizer = pathVisualizer;
        }

        public void Start()
        {
            ShowInitialPath();
            _ = waveService.Start();
        }

        public void Dispose()
        {
            waveService.Stop();
        }

        private void ShowInitialPath()
        {
            var cells = pathService.GetCurrentPath();
            var points = new List<Vector3>(cells.Count);
            foreach (var cell in cells)
                points.Add(cell.WorldPosition);
            pathVisualizer.Show(points);
        }
    }
}
