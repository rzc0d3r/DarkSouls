using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DarkSouls.Items.Placeable
{
    public class DarkSoulsBonfire : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.DarkSoulsBonfire.DarkSoulsBonfire>());
        }

        public override void AddRecipes()
        {
            Recipe r1 = CreateRecipe();
            r1.AddIngredient(ItemID.Campfire);
            r1.AddIngredient(ItemID.IronBroadsword);
            r1.Register();

            Recipe r2 = CreateRecipe();
            r2.AddIngredient(ItemID.Campfire);
            r2.AddIngredient(ItemID.LeadBroadsword);
            r2.Register();
        }
    }
}
