using UnityEngine;

namespace BetterOreMining.Infrastructure
{
    internal static class OreRocks
    {
        private const string DepositPrefix = "MineRock_";

        private static readonly string[] OreSuffixes = { "Ore", "OreNew" };

        private static readonly string[] OreItems =
        {
            "Obsidian",
            "IronScrap",
            "CopperScrap",
            "Softtissue"
        };

        public static bool IsOre(GameObject target)
        {
            if (target == null)
            {
                return false;
            }

            var pieces = target.GetComponent<MineRock5>();
            if (pieces != null)
            {
                return DropsOre(pieces.m_dropItems);
            }

            var oldPieces = target.GetComponent<MineRock>();
            if (oldPieces != null)
            {
                return DropsOre(oldPieces.m_dropItems);
            }

            var intact = target.GetComponent<Destructible>();
            return intact != null && IsOreWhenIntact(target, intact);
        }

        private static bool IsOreWhenIntact(GameObject target, Destructible intact)
        {
            var drop = target.GetComponent<DropOnDestroyed>();
            var isDeposit = target.name.StartsWith(DepositPrefix);
            if (isDeposit && drop != null && DropsOre(drop.m_dropWhenDestroyed))
            {
                return true;
            }

            var fractured = intact.m_spawnWhenDestroyed;
            return fractured != null && IsFracturedOre(fractured);
        }

        private static bool IsFracturedOre(GameObject fractured)
        {
            var pieces = fractured.GetComponent<MineRock5>();
            if (pieces != null)
            {
                return DropsOre(pieces.m_dropItems);
            }

            var oldPieces = fractured.GetComponent<MineRock>();
            return oldPieces != null && DropsOre(oldPieces.m_dropItems);
        }

        private static bool DropsOre(DropTable table)
        {
            if (table == null)
            {
                return false;
            }

            foreach (var drop in table.m_drops)
            {
                if (drop.m_item != null && IsOreItem(drop.m_item.name))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsOreItem(string name)
        {
            foreach (var suffix in OreSuffixes)
            {
                if (name.EndsWith(suffix))
                {
                    return true;
                }
            }

            return System.Array.IndexOf(OreItems, name) >= 0;
        }
    }
}
