using BetterOreMining.Features;
using UnityEngine;

namespace BetterOreMining.Infrastructure
{
    public sealed class PhysicsOreScanner : IOreScanner
    {
        public bool TryFindOre(
            Vector3 origin,
            Vector3 direction,
            float radius,
            float maxDistance,
            out float distance)
        {
            distance = 0f;
            var found = Physics.SphereCast(
                origin,
                radius,
                direction,
                out var hit,
                maxDistance,
                Attack.m_attackMask,
                QueryTriggerInteraction.Ignore);
            if (!found)
            {
                return false;
            }

            var target = Projectile.FindHitObject(hit.collider);
            distance = hit.distance;
            return OreRocks.IsOre(target);
        }
    }
}
