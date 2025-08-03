using DarkSouls.Common.ItemDropRules;
using DarkSouls.Items.Accessories;
using DarkSouls.Items.Consumables;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DarkSouls.Items;

public class DarkSoulsNPCLootDrop : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        // Cloranthy Ring
        if (npc.type == NPCID.GiantTortoise)
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CloranthyRing>(), 12)); // 8.33%
        else // Humanity
            npcLoot.Add(ItemDropRule.ByCondition(new DropsFromNormalEnemiesOnlyCondition(), ModContent.ItemType<Humanity>(), 100)); // 1%
    }
}