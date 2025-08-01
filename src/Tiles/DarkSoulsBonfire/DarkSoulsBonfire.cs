using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Localization;

using Microsoft.Xna.Framework;

using DarkSouls.UI;

namespace DarkSouls.Tiles.DarkSoulsBonfire
{
    public class DarkSoulsBonfire : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            AnimationFrameHeight = 52;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 20];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 100, 0), Language.GetText("Mods.DarkSouls.Tiles.DarkSoulsBonfire.DisplayName"));
        }

        public override bool RightClick(int i, int j)
        {
            DarkSoulsStatsUISystem dsStatsUISystem = ModContent.GetInstance<DarkSoulsStatsUISystem>();
            if (!dsStatsUISystem.UserInterfaceIsVisible)
            {
                if (dsStatsUISystem.ShowUI(true, i, j))
                    SoundEngine.PlaySound(DarkSouls.dsBonfireRestSound);
            }
            return true;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 4)
            {
                frameCounter = 0;
                frame = (frame + 1) % 24;
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
                return;

            if (Main.tile[i, j].TileFrameY < 52)
                Main.SceneMetrics.HasCampfire = true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY < 52)
            {
                float pulse = Main.rand.NextFloat(0.05f, 0.15f);
                pulse += (270 - Main.mouseTextColor) / 700f;

                r = 0.9f + pulse;
                g = 0.4f + pulse * 0.6f;
                b = 0.1f + pulse * 0.2f;
            }
        }
    }
}