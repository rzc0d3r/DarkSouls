using System.Collections.Generic;

using Terraria;
using Terraria.UI;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

namespace DarkSouls.UI
{
    [Autoload(true, Side = ModSide.Client)]
    public class DarkSoulsStatsUISystem : ModSystem
    {
        internal UserInterface userInterface;
        internal DarkSoulsStatsUI dsStatsUI;

        public bool UserInterfaceIsVisible => userInterface.CurrentState != null;

        private bool openedFromBonfire = false;
        private int bonfireX = 0;
        private int bonfireY = 0;

        private GameTime lastUpdateUiGameTime;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                userInterface = new UserInterface();
                dsStatsUI = new DarkSoulsStatsUI();
            }
        }

        public bool ShowUI(bool fromBonfire = false, int bonfireX = 0, int bonfireY = 0)
        {
            if (fromBonfire)
            {
                openedFromBonfire = fromBonfire;
                this.bonfireX = bonfireX;
                this.bonfireY = bonfireY;
                if (GetDistanceToBonfire() > 40f)
                    return false;
            }
            else
                openedFromBonfire = false;
            dsStatsUI.openedFromBonfire = openedFromBonfire;
            userInterface?.SetState(dsStatsUI);
            return true;
        }

        public void HideUI()
        {
            userInterface?.SetState(null);
            openedFromBonfire = false;
            dsStatsUI.openedFromBonfire = false;
        }

        public void ToggleUI()
        {
            if (UserInterfaceIsVisible)
                HideUI();
            else
                ShowUI();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (UserInterfaceIsVisible)
            {
                if (openedFromBonfire)
                {
                    if (GetDistanceToBonfire() > 40f)  // Autoclose UI
                    {
                        SoundEngine.PlaySound(DarkSouls.dsInferfaceReturnSound);
                        HideUI();
                    }
                }
                userInterface?.Update(gameTime);
                lastUpdateUiGameTime = gameTime;
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Dark Souls: Stats UI",
                    delegate
                    {
                        if (UserInterfaceIsVisible)
                            userInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        private float GetDistanceToBonfire()
        {
            Vector2 bonfirePos = new Vector2(bonfireX * 16, bonfireY * 16);
            return Vector2.Distance(Main.LocalPlayer.Center, bonfirePos);
        }
    }
}