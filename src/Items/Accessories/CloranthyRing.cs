using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DarkSouls.Items.Accessories
{
    public class CloranthyRing : ModItem
    {
        public const float StaminaRegenRateBonus = 0.25f;
        public const float StaminaRegenDelayReductionBonus = 0.15f;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((int)(StaminaRegenRateBonus * 100), (int)(StaminaRegenDelayReductionBonus * 100));

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 27;
            Item.height = 29;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 25, 0, 0);
            Item.scale = 1.1f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            DarkSoulsPlayer dsPlayer = player.GetModPlayer<DarkSoulsPlayer>();
            dsPlayer.CloranthyRingEffect = true;
        }
    }
}
