using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using DarkSouls.UI;

namespace DarkSouls.Items.Consumables
{
    public class FireKeeperSoul : ModItem
    {
        public static bool canConsume = false;
        
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 56;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(5, 0, 0, 0);
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.UseSound = SoundID.Item92;
        }

        public override bool? UseItem(Player player) => true;

        public override bool CanUseItem(Player player) => !ModContent.GetInstance<DarkSoulsStatsUISystem>().UserInterfaceIsVisible;

        public override bool ConsumeItem(Player player)
        {
            if (canConsume)
                return true;
            ModContent.GetInstance<DarkSoulsStatsUISystem>().ShowUI(respecStats: true);
            return false;
        }

        public override void OnConsumeItem(Player player) => canConsume = false;
    }
}
