
using System.IO;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using ReLogic.Content;
using ReLogic.Graphics;

using DarkSouls.Utils;
using DarkSouls.Config;

namespace DarkSouls
{
    public class DarkSouls : Mod
    {
        #region HotKeys
        public static ModKeybind ToggleDarkSoulsStatsUIKey;
        public static ModKeybind DashKey;
        public static ModKeybind TouchBloodstainKey;
        #endregion

        #region Fonts
        public static DynamicSpriteFont OptimusPrincepsFont;
        #endregion

        #region Sounds Variables
        public static SoundStyle dsConsumeSoulSound;
        public static SoundStyle dsNewAreaSound;
        public static SoundStyle dsInferfaceSound;
        public static SoundStyle dsInferfaceClickSound;
        public static SoundStyle dsInferfaceReturnSound;
        public static SoundStyle dsBonfireRestSound;
        public static SoundStyle dsMainMenuStartSound;
        public static SoundStyle dsSoulSuck;
        public static SoundStyle dsThruDeath;
        public static SoundStyle dsMaleDeadSound;
        public static SoundStyle dsMaleFallinDeadSound;
        public static SoundStyle dsFemaleDeadSound;
        public static SoundStyle dsFemaleFallinDeadSound;

        public static List<SoundStyle> dsMaleDamageSounds;
        public static List<SoundStyle> dsFemaleDamageSounds;

        private static float DSMaleDamageSoundVolume = 0.45f;
        private static float DSFemaleDamageSoundVolume = 0.35f;
        #endregion

        public enum NetMessageTypes : byte
        {
            GetSouls,
            SyncVitality
        }

        public override void PostSetupContent()
        {
            DarkSoulsResourcePack.GetInfo();

            if (!DarkSoulsResourcePack.IsInstalled)
                DarkSoulsResourcePack.Install();

            bool DisableOverrideHurtSounds = ClientConfig.GetValueFromJSON("DisableOverrideHurtSounds");
            bool DisableOverrideMusic = ClientConfig.GetValueFromJSON("DisableOverrideMusic");

            if (DisableOverrideHurtSounds)
                DarkSoulsResourcePack.DisableOverrideResources(true);
            else
                DarkSoulsResourcePack.EnableOverrideResources(true);

            if (DisableOverrideMusic)
                DarkSoulsResourcePack.DisableOverrideResources(false, true);
            else
                DarkSoulsResourcePack.EnableOverrideResources(false, true);

            DarkSoulsResourcePack.GetInfo();
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                // Keybinds
                ToggleDarkSoulsStatsUIKey = KeybindLoader.RegisterKeybind(this, "Toggle Dark Souls Stats UI", "OemTilde");
                DashKey = KeybindLoader.RegisterKeybind(this, "Dash", "Double-tap A or D");
                TouchBloodstainKey = KeybindLoader.RegisterKeybind(this, "Touch the bloodstain", "NumPad1");

                // Fonts
                OptimusPrincepsFont = ModContent.Request<DynamicSpriteFont>("DarkSouls/Fonts/OptimusPrinceps", AssetRequestMode.ImmediateLoad).Value;

                // SoundStyles
                dsConsumeSoulSound = new("DarkSouls/Sounds/DS_ConsumeSoul") { Volume = 0.85f };
                dsNewAreaSound = new("DarkSouls/Sounds/DS_NewArea") { Volume = 0.35f };
                dsInferfaceSound = new("DarkSouls/Sounds/DS_Interface") { Volume = 0.55f };
                dsInferfaceClickSound = new("DarkSouls/Sounds/DS_InterfaceClick") { Volume = 0.85f };
                dsInferfaceReturnSound = new("DarkSouls/Sounds/DS_InterfaceReturn") { Volume = 0.75f };
                dsBonfireRestSound = new("DarkSouls/Sounds/DS_BonfireRest") { Volume = 0.5f };
                dsMainMenuStartSound = new("DarkSouls/Sounds/DS_GAMESTART") { Volume = 0.65f };
                dsSoulSuck = new("DarkSouls/Sounds/DS_SoulSuckBonus") { Volume = 0.4f, PitchVariance = 0.2f };
                dsThruDeath = new("DarkSouls/Sounds/DS_ThruDeath") { Volume = 0.7f };

                dsMaleDeadSound = new("DarkSouls/Sounds/Voices/Male/mx-dead") { Volume = DSMaleDamageSoundVolume + 0.25f, PitchVariance = 0.25f };
                dsMaleFallinDeadSound = new("DarkSouls/Sounds/Voices/Male/mx-fallindead") { Volume = DSMaleDamageSoundVolume + 0.25f, PitchVariance = 0.25f };

                dsFemaleDeadSound = new("DarkSouls/Sounds/Voices/Female/f-dead") { Volume = DSFemaleDamageSoundVolume + 0.25f, PitchVariance = 0.15f };
                dsFemaleFallinDeadSound = new("DarkSouls/Sounds/Voices/Female/f-dead") { Volume = DSFemaleDamageSoundVolume + 0.25f, PitchVariance = 0.15f };

                dsMaleDamageSounds = new List<SoundStyle>
                {
                    new("DarkSouls/Sounds/Voices/Male/mx-m-1") { Volume = DSMaleDamageSoundVolume, PitchVariance = 0.35f },
                    new("DarkSouls/Sounds/Voices/Male/mx-m-2") { Volume = DSMaleDamageSoundVolume, PitchVariance = 0.35f },
                    new("DarkSouls/Sounds/Voices/Male/mx-m-3") { Volume = DSMaleDamageSoundVolume, PitchVariance = 0.35f },
                    new("DarkSouls/Sounds/Voices/Male/mx-s-1") { Volume = DSMaleDamageSoundVolume, PitchVariance = 0.35f },
                    new("DarkSouls/Sounds/Voices/Male/mx-s-2") { Volume = DSMaleDamageSoundVolume, PitchVariance = 0.35f },
                };

                dsFemaleDamageSounds = new List<SoundStyle>
                {
                    new("DarkSouls/Sounds/Voices/Female/f-m-1") { Volume = DSFemaleDamageSoundVolume, PitchVariance = 0.2f },
                    new("DarkSouls/Sounds/Voices/Female/f-m-2") { Volume = DSFemaleDamageSoundVolume, PitchVariance = 0.2f },
                    new("DarkSouls/Sounds/Voices/Female/f-m-3") { Volume = DSFemaleDamageSoundVolume, PitchVariance = 0.2f },
                    new("DarkSouls/Sounds/Voices/Female/f-s-1") { Volume = DSFemaleDamageSoundVolume, PitchVariance = 0.2f },
                    new("DarkSouls/Sounds/Voices/Female/f-s-3") { Volume = DSFemaleDamageSoundVolume, PitchVariance = 0.2f },
                };
            }
        }

        public override void Unload()
        {
            ToggleDarkSoulsStatsUIKey = null;
            DashKey = null;

            OptimusPrincepsFont = null;

            dsConsumeSoulSound = default;
            dsNewAreaSound = default;
            dsInferfaceSound = default;
            dsInferfaceClickSound = default;
            dsInferfaceReturnSound = default;
            dsBonfireRestSound = default;
            dsMainMenuStartSound = default;
            dsSoulSuck = default;
            dsThruDeath = default;

            dsMaleDeadSound = default;
            dsMaleFallinDeadSound = default;
            dsFemaleDeadSound = default;
            dsFemaleFallinDeadSound = default;

            dsMaleDamageSounds = null;
            dsFemaleDamageSounds = null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetMessageTypes messageType = (NetMessageTypes)reader.ReadByte();
            byte playerID;
            switch (messageType)
            {
                case NetMessageTypes.GetSouls:
                    int souls = reader.ReadInt32();
                    Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>().AddSouls(souls);
                    break;
                case NetMessageTypes.SyncVitality:
                    playerID = reader.ReadByte();
                    int newVitalityValue = reader.ReadInt32();

                    Player player = Main.player[playerID];
                    player.GetModPlayer<DarkSoulsPlayer>().dsVitality = newVitalityValue;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = GetPacket();
                        packet.Write((byte)NetMessageTypes.SyncVitality);
                        packet.Write(playerID);
                        packet.Write(newVitalityValue);
                        packet.Send(-1, whoAmI);
                    }
                    break;
            }
        }
    }
}
