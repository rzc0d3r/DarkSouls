using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using DarkSouls.Items.Accessories;

namespace DarkSouls.Items;

public class DarkSoulsNPCLootDrop : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        // Cloranthy Ring
        if (npc.type == NPCID.GiantTortoise)
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CloranthyRing>(), 10)); // 10%
    }
}