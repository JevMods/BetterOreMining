using UnityEngine;

namespace BetterOreMining.Features
{
    public sealed class VeinScan
    {
        public VeinScan(Renderer[] renderers, float nearestDistance)
        {
            Renderers = renderers;
            NearestDistance = nearestDistance;
        }

        public Renderer[] Renderers { get; }

        public float NearestDistance { get; }
    }
}
