using System.IO;
using System.Collections.Generic;

using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

using ReLogic.Content;
using ReLogic.Graphics;

namespace DarkSouls
{
    public class DarkSouls : Mod
    {
        public static ModKeybind ToggleDarkSoulsStatsUIKey;

        public static DynamicSpriteFont OptimusPrincepsFont;

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

        private static float DSMaleDamageSoundVolume = 0.55f;
        private static float DSFemaleDamageSoundVolume = 0.5f;

        public const string CrimsonColorTooltip = "c/dc143c";
        public const string DodgerBlueColorTooltip = "c/1e90ff";
        public const string MediumSeaGreenColorTooltip = "c/3cb371";

        public enum NetMessageTypes : byte
        {
            GetSouls
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                // Keybinds
                ToggleDarkSoulsStatsUIKey = KeybindLoader.RegisterKeybind(this, "Toggle Dark Souls Stats UI", "OemTilde");

                // Fonts
                OptimusPrincepsFont = ModContent.Request<DynamicSpriteFont>("DarkSouls/Fonts/OptimusPrinceps", AssetRequestMode.ImmediateLoad).Value;

                // SoundStyles
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
            OptimusPrincepsFont = null;

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
            if (messageType == NetMessageTypes.GetSouls)
            {
                int souls = reader.ReadInt32();
                DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
                dsPlayer.AddSouls(souls);
            }
        }

        public static SoundStyle GetRandomDamageSounds(out int soundIndex, bool male = true, int excludeSoundIndex = -1)
        {
            List<SoundStyle> list = male ? dsMaleDamageSounds : dsFemaleDamageSounds;
            int index;
            do
            {
                index = Main.rand.Next(list.Count);
            } while (index == excludeSoundIndex);
            soundIndex = index;
            return list[index];
        }
    }
}
