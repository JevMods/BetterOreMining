using System.Collections.Generic;
using BetterOreMining.Features;
using UnityEngine;

namespace BetterOreMining.Infrastructure
{
    public sealed class PhysicsVeinScanner : IVeinScanner
    {
        private static readonly Collider[] Colliders = new Collider[256];

        public VeinScan Scan(Vector3 center, float radius)
        {
            var count = Physics.OverlapSphereNonAlloc(
                center,
                radius,
                Colliders,
                Attack.m_attackMask,
                QueryTriggerInteraction.Ignore);
            var rocks = new HashSet<GameObject>();
            var nearest = radius;
            for (var i = 0; i < count; i++)
            {
                var collider = Colliders[i];
                var target = Projectile.FindHitObject(collider);
                if (OreRocks.IsOre(target))
                {
                    rocks.Add(target);
                    var closest = collider.bounds.ClosestPoint(center);
                    nearest = Mathf.Min(nearest, Vector3.Distance(closest, center));
                }
            }

            var renderers = new List<Renderer>();
            foreach (var rock in rocks)
            {
                renderers.AddRange(rock.GetComponentsInChildren<MeshRenderer>());
            }

            return new VeinScan(renderers.ToArray(), nearest);
        }
    }
}
