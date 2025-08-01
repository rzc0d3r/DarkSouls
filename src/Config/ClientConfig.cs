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

        public static bool GetDOHSValueFromJSON()
        {
            string path = Path.Combine(Main.SavePath, "ModConfigs", "DarkSouls_ClientConfig.json");
            if (!File.Exists(path))
                return false;

            try
            {
                string data = File.ReadAllText(path);
                JObject json = JObject.Parse(data);
                return json["DisableOverrideHurtSounds"]?.Value<bool>() ?? false;
            }
            catch (Exception ex)
            {
                ConsoleUtils.WriteLine($"[DarkSouls] Failed to read DOHS value from JSON: {ex}", ConsoleColor.DarkRed);
            }

            return false;
        }
    }
}
