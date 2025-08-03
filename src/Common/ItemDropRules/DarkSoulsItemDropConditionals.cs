using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;

namespace DarkSouls.Common.ItemDropRules
{
    public class DropsFromNormalEnemiesOnlyCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            NPC npc = info.npc;

            if (npc.boss)
                return false;

            if (npc.type == NPCID.LunarTowerNebula || npc.type == NPCID.LunarTowerSolar ||
                npc.type == NPCID.LunarTowerStardust || npc.type == NPCID.LunarTowerVortex)
                return false;

            if (npc.SpawnedFromStatue || npc.friendly || npc.townNPC || npc.lifeMax <= 5 || npc.damage == 0)
                return false;

            return true;
        }

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => Language.GetTextValue("Mods.DarkSouls.Conditionals.DropsFromNormalEnemiesOnlyConditionDescription");
    }
}