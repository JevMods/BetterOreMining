using System.Collections.Generic;
using UnityEngine;

namespace BetterOreMining.Features
{
    public sealed class RippleFeature : IFeature
    {
        private const int MaxWaves = 4;

        private readonly IGameEvents _events;
        private readonly IRippleSettings _settings;
        private readonly IVeinScanner _scanner;
        private readonly IGroundProbe _ground;
        private readonly IGlowRenderer _renderer;
        private readonly List<RippleWave> _waves = new List<RippleWave>();

        private int _strikeFrame = -1;
        private Vector3? _pendingStrike;

        public RippleFeature(
            IGameEvents events,
            IRippleSettings settings,
            IVeinScanner scanner,
            IGroundProbe ground,
            IGlowRenderer renderer)
        {
            _events = events;
            _settings = settings;
            _scanner = scanner;
            _ground = ground;
            _renderer = renderer;
        }

        public void Enable()
        {
            _events.PickaxeStruck += OnPickaxeStruck;
            _events.FrameUpdated += OnFrameUpdated;
        }

        public void Disable()
        {
            _events.PickaxeStruck -= OnPickaxeStruck;
            _events.FrameUpdated -= OnFrameUpdated;
            _pendingStrike = null;
            _waves.Clear();
            _renderer.Clear();
        }

        private void OnPickaxeStruck(Vector3 point)
        {
            if (Time.frameCount == _strikeFrame)
            {
                return;
            }

            _strikeFrame = Time.frameCount;
            _pendingStrike = point;
        }

        private void StartWave(Vector3 point)
        {
            var radius = _settings.Radius;
            var scan = _scanner.Scan(point, radius);
            if (scan.Renderers.Length == 0)
            {
                return;
            }

            if (_waves.Count == MaxWaves)
            {
                _waves.RemoveAt(0);
            }

            _waves.Add(new RippleWave(
                point,
                scan.Renderers,
                GroundMap.Sample(_ground, point, radius),
                Time.time,
                scan.NearestDistance));
        }

        private void OnFrameUpdated()
        {
            var settled = _pendingStrike.HasValue && Time.frameCount > _strikeFrame;
            if (settled)
            {
                StartWave(_pendingStrike.Value);
                _pendingStrike = null;
            }

            if (_waves.Count == 0)
            {
                return;
            }

            foreach (var wave in _waves)
            {
                wave.Advance(Time.time, _settings);
            }

            _waves.RemoveAll(wave => wave.IsFinished);
            if (_waves.Count == 0)
            {
                _renderer.Clear();
                return;
            }

            _renderer.Draw(_waves, _settings.RingWidth);
        }
    }
}
