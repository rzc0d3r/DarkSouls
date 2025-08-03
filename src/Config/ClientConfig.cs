using Terraria;
using Terraria.ModLoader.Config;

using System;
using System.IO;
using System.ComponentModel;

using Newtonsoft.Json.Linq;
using DarkSouls.Utils;

namespace DarkSouls.Config
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static ClientConfig Instance;

        public override void OnLoaded() => Instance = this;

        [Header("ResourcePack")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableOverrideHurtSounds = false;

        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableOverrideMusic = false;

        [Header("Other")]
        [DefaultValue(false)]
        public bool DisableDeathScreen = false;

        [DefaultValue(false)]
        public bool DisableDash = false;

        public static bool GetValueFromJSON(string valueName)
        {
            string path = Path.Combine(Main.SavePath, "ModConfigs", "DarkSouls_ClientConfig.json");
            if (!File.Exists(path))
                return false;

            try
            {
                string data = File.ReadAllText(path);
                JObject json = JObject.Parse(data);
                return json[valueName]?.Value<bool>() ?? false;
            }
            catch (Exception ex)
            {
                ConsoleUtils.WriteLine($"[DarkSouls] Failed to read value with name \"{valueName}\" from JSON: {ex}", ConsoleColor.DarkRed);
            }

            return false;
        }
    }
}
