using UnityEngine;

namespace BetterOreMining.Features
{
    public interface IOreScanner
    {
        bool TryFindOre(
            Vector3 origin,
            Vector3 direction,
            float radius,
            float maxDistance,
            out float distance);
    }
}
