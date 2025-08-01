using System;

using Terraria;
using Terraria.Utilities;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

using ReLogic.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace DarkSouls.UI
{
    [Autoload(true, Side = ModSide.Client)]
    public class DarkSoulsYouDiedUI : UIState
    {
        private float alpha = 0f;
        private bool visible = false;
        private bool fullyVisible = false;

        private const float fadeInSpeed = 0.5f / (60f * 1.2f);
        private const float fadeOutSpeed = 1f / (40f * 1.2f);
        private const float textScale = 1f;

        public void Show()
        {
            alpha = 0f;
            visible = true;
            fullyVisible = false;
        }

        public void Hide()
        {
            visible = false;
            fullyVisible = false;
            alpha = 1f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible)
                return;

            if (Main.LocalPlayer.dead)
            {
                if (!fullyVisible)
                {
                    alpha = MathHelper.Clamp(alpha + fadeInSpeed, 0f, 1f);
                    if (alpha >= 1f)
                        fullyVisible = true;
                }
            }
            else
            {
                alpha = MathHelper.Clamp(alpha - fadeOutSpeed, 0f, 1f);
                if (alpha <= 0f)
                {
                    Hide();
                    return;
                }
            }

            spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                Color.Black * alpha
            );

            string text = Language.GetText("Mods.DarkSouls.UI.YouDiedUI.MainText").Value;
            DynamicSpriteFont font = DarkSouls.OptimusPrincepsFont;
            Vector2 textSize = font.MeasureString(text) * textScale;
            Vector2 position = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f) - textSize / 2f;

            spriteBatch.DrawString(font, text, position + new Vector2(2, 2), Color.Black * alpha, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, text, position, Color.Firebrick * alpha, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

            int respawnTimeInSeconds = (int)Math.Ceiling(Main.LocalPlayer.respawnTimer / 60f);
            string respawnText = Language.GetText("Mods.DarkSouls.UI.YouDiedUI.RespawningText").WithFormatArgs(respawnTimeInSeconds).Value;
            Vector2 respawnTextSize = font.MeasureString(respawnText) * 0.3f;
            Vector2 respawnPosition = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f) - respawnTextSize / 2f + new Vector2(0, respawnTextSize.Y) * 1.25f;

            spriteBatch.DrawString(font, respawnText, respawnPosition + new Vector2(1, 1), Color.Black * alpha, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, respawnText, respawnPosition, Color.Gray * alpha, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);

        }
    }
}
