using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DarkSouls.Items.Consumables;

namespace DarkSouls.NPCs;

public class NPCShops : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        switch (shop.NpcType)
        {
            case NPCID.Dryad:
                shop.Add<FireKeeperSoul>(Condition.Hardmode);
                break;
        }
    }
}