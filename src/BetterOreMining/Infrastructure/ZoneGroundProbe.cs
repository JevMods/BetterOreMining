using BetterOreMining.Features;
using UnityEngine;

namespace BetterOreMining.Infrastructure
{
    public sealed class ZoneGroundProbe : IGroundProbe
    {
        public float GroundHeight(Vector3 point)
        {
            return ZoneSystem.instance.GetGroundHeight(point);
        }
    }
}
