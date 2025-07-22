using System;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

using ReLogic.Content;
using ReLogic.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DarkSouls.UI
{
    public class CustomResourceBars : ModResourceDisplaySet
    {
        private Asset<Texture2D> emptyBar;
        private Asset<Texture2D> staminaBarSegment;
        private Asset<Texture2D> hpBarSegment;
        private Asset<Texture2D> manaBarSegment;

        private Asset<Texture2D> usedBarSegment;
        private Asset<Texture2D> usedHPBarSegment;

        private int barWidth;
        private int barHeight;
        private int segments;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                emptyBar = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Bars/EmptyBar");
                staminaBarSegment = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Bars/StaminaBarSegment");
                hpBarSegment = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Bars/HPBarSegment");
                manaBarSegment = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Bars/ManaBarSegment");
                usedBarSegment = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Bars/UsedBarSegment");
                usedHPBarSegment = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Bars/UsedHPBarSegment");
            }
        }

        public override void SetStaticDefaults()
        {
            barWidth = emptyBar.Width();
            barHeight = emptyBar.Height();
            segments = barWidth - 2 * 3;
        }

        public override void DrawLife(SpriteBatch spriteBatch)
        {
            DrawHealthBar(spriteBatch);
            DrawManaBar(spriteBatch);
            DrawStaminaBar(spriteBatch);
        }

        public override void DrawMana(SpriteBatch spriteBatch) { }

        public override bool PreHover(out bool hoveringLife)
        {
            hoveringLife = true;
            return false;
        }

        private void DrawHealthBar(SpriteBatch spriteBatch)
        {
            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            Player player = Main.LocalPlayer;
            Vector2 position = new Vector2(Main.miniMapX - 4, 20);

            if (player.statLifeMax2 > 0)
            {
                int draw = (int)(((float)dsPlayer.usedHP / player.statLifeMax2) * segments);
                if (dsPlayer.usedHP >= player.statLifeMax2)
                    draw++;
                for (int i = 0; i < draw; i++)
                    spriteBatch.Draw(usedHPBarSegment.Value, new Vector2(position.X + 3 + i, position.Y + 2), Color.White);

                draw = (int)(((float)player.statLife / player.statLifeMax2) * segments);
                if (player.statLife >= player.statLifeMax2)
                    draw++;
                for (int i = 0; i < draw; i++)
                    spriteBatch.Draw(hpBarSegment.Value, new Vector2(position.X + 3 + i, position.Y + 2), Color.White);
            }

            spriteBatch.Draw(emptyBar.Value, position, new Rectangle(0, 0, barWidth, barHeight), Color.White);
            position += new Vector2(barWidth + 5, -1);
            spriteBatch.DrawString(FontAssets.MouseText.Value, $"{player.statLife}/{player.statLifeMax2}", position, Color.WhiteSmoke, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
        }

        private void DrawStaminaBar(SpriteBatch spriteBatch)
        {
            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            Vector2 position = new Vector2(Main.miniMapX - 4, 20 + 2 * (barHeight + 3));

            if (dsPlayer.currentStamina > 0)
            {
                int draw = (int)((dsPlayer.usedStamina / dsPlayer.maxStamina) * segments);
                if (dsPlayer.usedStamina >= dsPlayer.maxStamina)
                    draw++;
                for (int i = 0; i < draw; i++)
                    spriteBatch.Draw(usedBarSegment.Value, new Vector2(position.X + 3 + i, position.Y + 3), Color.White);

                draw = (int)((dsPlayer.currentStamina / dsPlayer.maxStamina) * segments);
                if (dsPlayer.currentStamina >= dsPlayer.maxStamina)
                    draw++;
                for (int i = 0; i < draw; i++)
                    spriteBatch.Draw(staminaBarSegment.Value, new Vector2(position.X + 3 + i, position.Y + 3), Color.White);
            }

            spriteBatch.Draw(emptyBar.Value, position, new Rectangle(0, 0, barWidth, barHeight), Color.White);
            position += new Vector2(barWidth + 5, -1);
            spriteBatch.DrawString(FontAssets.MouseText.Value, $"{(int)Math.Floor(dsPlayer.currentStamina)}/{(int)Math.Floor(dsPlayer.maxStamina)}", position, Color.WhiteSmoke, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
        }

        private void DrawManaBar(SpriteBatch spriteBatch)
        {
            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            Player player = Main.LocalPlayer;
            Vector2 position = new Vector2(Main.miniMapX - 4, 20 + barHeight + 3);

            if (player.statMana > 0)
            {
                for (int i = 0; i <= ((float)dsPlayer.usedMana / player.statManaMax2) * segments; i++)
                    spriteBatch.Draw(usedBarSegment.Value, new Vector2(position.X + 3 + i, position.Y + 3), Color.White);

                for (int i = 0; i <= ((float)player.statMana / player.statManaMax2) * segments; i++)
                    spriteBatch.Draw(manaBarSegment.Value, new Vector2(position.X + 3 + i, position.Y + 3), Color.White);
            }

            spriteBatch.Draw(emptyBar.Value, position, new Rectangle(0, 0, barWidth, barHeight), Color.White);
            position += new Vector2(barWidth + 5, -1);
            spriteBatch.DrawString(FontAssets.MouseText.Value, $"{player.statMana}/{player.statManaMax2}", position, Color.WhiteSmoke, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
        }
    }
}