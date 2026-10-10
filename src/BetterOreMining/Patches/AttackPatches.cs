using BetterOreMining.Infrastructure;
using HarmonyLib;
using UnityEngine;

namespace BetterOreMining.Patches
{
    internal static class AttackPatches
    {
        [HarmonyPatch(typeof(Attack), nameof(Attack.DoMeleeAttack))]
        private static class DoMeleeAttack
        {
            private static void Prefix(Attack __instance)
            {
                GameEvents.Instance?.RaiseMeleeAttackStarting(__instance);
            }
        }

        [HarmonyPatch(typeof(Attack), nameof(Attack.AddHitPoint))]
        private static class AddHitPoint
        {
            private static void Postfix(Attack __instance, GameObject go, Vector3 point)
            {
                if (IsLocalPickaxe(__instance) && !IsDynamic(go))
                {
                    GameEvents.Instance?.RaisePickaxeStruck(point);
                }
            }

            private static bool IsLocalPickaxe(Attack attack)
            {
                var isLocal = attack.m_character == Player.m_localPlayer;
                var weapon = attack.m_weapon;
                var isPickaxe = weapon != null
                    && weapon.m_shared.m_skillType == Skills.SkillType.Pickaxes;
                return isLocal && isPickaxe;
            }

            private static bool IsDynamic(GameObject go)
            {
                return go.GetComponentInParent<Rigidbody>() != null;
            }
        }
    }
}
