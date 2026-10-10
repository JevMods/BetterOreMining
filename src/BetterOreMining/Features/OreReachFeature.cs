using UnityEngine;

namespace BetterOreMining.Features
{
    public sealed class OreReachFeature : IFeature
    {
        private const float MaxReach = 4f;
        private const float ReachMargin = 0.5f;
        private const float MaxPitchDegrees = 45f;
        private const float MinRayWidth = 0.2f;

        private readonly IGameEvents _events;
        private readonly IOreScanner _scanner;

        public OreReachFeature(IGameEvents events, IOreScanner scanner)
        {
            _events = events;
            _scanner = scanner;
        }

        public void Enable()
        {
            _events.MeleeAttackStarting += OnMeleeAttackStarting;
        }

        public void Disable()
        {
            _events.MeleeAttackStarting -= OnMeleeAttackStarting;
        }

        private static bool IsLocalPickaxeSwing(Attack attack)
        {
            var isLocalPlayer = attack.m_character == Player.m_localPlayer;
            var isPickaxe = attack.m_weapon.m_shared.m_skillType == Skills.SkillType.Pickaxes;
            return isLocalPlayer && isPickaxe;
        }

        private void OnMeleeAttackStarting(Attack attack)
        {
            if (!IsLocalPickaxeSwing(attack))
            {
                return;
            }

            var vanillaPitch = attack.m_maxYAngle;
            attack.m_maxYAngle = Mathf.Max(vanillaPitch, MaxPitchDegrees);
            attack.GetMeleeAttackDir(out var joint, out var direction);
            var origin = joint.position
                + Vector3.up * attack.m_attackHeight
                + attack.m_character.transform.right * attack.m_attackOffset;
            var radius = Mathf.Max(attack.m_attackRayWidth, MinRayWidth);
            if (!_scanner.TryFindOre(origin, direction, radius, MaxReach, out var distance))
            {
                attack.m_maxYAngle = vanillaPitch;
                return;
            }

            attack.m_hitTerrain = false;
            attack.m_attackRange = Mathf.Max(attack.m_attackRange, distance + radius + ReachMargin);
        }
    }
}
