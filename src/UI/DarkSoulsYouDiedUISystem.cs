using System.Collections.Generic;

using Terraria;
using Terraria.UI;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

namespace DarkSouls.UI
{
    [Autoload(true, Side = ModSide.Client)]
    public class DarkSoulsYouDiedUISystem : ModSystem
    {
        internal DarkSoulsYouDiedUI youDiedUI;
        internal UserInterface userInterface;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                userInterface = new();
                youDiedUI = new();
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        public void ShowUI()
        {
            userInterface?.SetState(youDiedUI);
            youDiedUI.Show();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Add(new LegacyGameInterfaceLayer(
                "DarkSouls: You Died UI",
                delegate
                {
                    youDiedUI?.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }
}