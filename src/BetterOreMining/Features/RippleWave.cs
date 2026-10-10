using UnityEngine;

namespace BetterOreMining.Features
{
    public sealed class RippleWave
    {
        private readonly float _startTime;
        private readonly float _startFront;

        public RippleWave(
            Vector3 center,
            Renderer[] ore,
            GroundMap ground,
            float startTime,
            float startFront)
        {
            Center = center;
            Ore = ore;
            Ground = ground;
            _startTime = startTime;
            _startFront = startFront;
        }

        public Vector3 Center { get; }

        public Renderer[] Ore { get; }

        public GroundMap Ground { get; }

        public float Front { get; private set; }

        public float Alpha { get; private set; }

        public bool IsFinished { get; private set; }

        public void Advance(float now, IRippleSettings settings)
        {
            var travel = RippleTimeline.Front(now - _startTime, settings.WaveSpeed);
            Front = _startFront + travel;
            var fade = RippleTimeline.Fade(Front, settings.Radius, settings.RingWidth);
            Alpha = settings.Opacity * fade;
            IsFinished = RippleTimeline.IsFinished(Front, settings.Radius, settings.RingWidth);
        }
    }
}
