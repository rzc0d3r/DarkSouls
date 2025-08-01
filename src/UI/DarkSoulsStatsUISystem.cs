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



        private bool openedFromBonfire = false;
        private bool respecStats = false;
        private int bonfireX = 0;
        private int bonfireY = 0;

        public bool UserInterfaceIsVisible => userInterface?.CurrentState != null;
        public static readonly float DistanceToBonfire = 90f;

        private GameTime lastUpdateUiGameTime;

        public override void Load()
        {
            if (!Main.dedServ)
                userInterface = new UserInterface();
        }

        public override void Unload()
        {
            dsStatsUI = null;
            userInterface = null;
        }

        public override void OnWorldUnload()
        {
            if (dsStatsUI != null)
            {
                HideUI();
                dsStatsUI = null;
            }
        }

        public bool ShowUI(bool fromBonfire = false, int bonfireX = 0, int bonfireY = 0, bool respecStats = false)
        {
            if (dsStatsUI == null)
                dsStatsUI = new DarkSoulsStatsUI();

            if (fromBonfire)
            {
                openedFromBonfire = fromBonfire;
                this.bonfireX = bonfireX;
                this.bonfireY = bonfireY;
                if (GetDistanceToBonfire() > DistanceToBonfire)
                    return false;
            }
            else if (respecStats)
            {
                openedFromBonfire = false;
                this.respecStats = respecStats;
            }
            else
            {
                openedFromBonfire = false;
                this.respecStats = false;
            }

            dsStatsUI.openedFromBonfire = openedFromBonfire;
            dsStatsUI.respecStats = this.respecStats;
            userInterface?.SetState(dsStatsUI);
            return true;
        }

        public void HideUI()
        {
            userInterface?.SetState(null);
            openedFromBonfire = false;
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
            lastUpdateUiGameTime = gameTime;
            if (UserInterfaceIsVisible)
            {
                if (openedFromBonfire && GetDistanceToBonfire() > DistanceToBonfire)
                {
                    SoundEngine.PlaySound(DarkSouls.dsInferfaceReturnSound);
                    HideUI();
                }
                userInterface?.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Dark Souls: Stats UI",
                    () => {
                        if (UserInterfaceIsVisible)
                            userInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        private float GetDistanceToBonfire()
        {
            Vector2 bonfirePos = new Vector2(bonfireX * 16, bonfireY * 16);
            return Vector2.Distance(Main.LocalPlayer.Center, bonfirePos);
        }
    }
}