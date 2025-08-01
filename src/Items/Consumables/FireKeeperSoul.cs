using DarkSouls.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DarkSouls.Items.Consumables
{
    public class FireKeeperSoul : ModItem
    {
        public static bool canConsume = false;
        
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(5, 0, 0, 0);
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Item92;
        }

        public override bool? UseItem(Player player) => true;

        public override bool ConsumeItem(Player player)
        {
            if (canConsume)
                return true;
            var UISystem = ModContent.GetInstance<DarkSoulsStatsUISystem>();
            Main.NewText(UISystem.UserInterfaceIsVisible);
            if (!UISystem.UserInterfaceIsVisible)
                UISystem.ShowUI(respecStats: true);
            return false;
        }

        public override void OnConsumeItem(Player player) => canConsume = false;
    }
}
