using Terraria.Localization;
using Terraria.ModLoader;

using DarkSouls.DataStructures;

namespace DarkSouls.Systems
{
    public class LocalizationUpdater : ModSystem
    {
        public override void OnLocalizationsLoaded()
        {
            DarkSoulsScalingSystem.ReqParamDisplayName = Language.GetTextValue("Mods.DarkSouls.ScalingSystem.ReqParamDisplayName");
            DarkSoulsScalingSystem.ParamBonusDisplayName = Language.GetTextValue("Mods.DarkSouls.ScalingSystem.ParamBonusDisplayName");
            DarkSoulsScalingSystem.StrengthDisplayName = Language.GetTextValue("Mods.DarkSouls.ScalingSystem.StrengthDisplayName");
            DarkSoulsScalingSystem.DexterityDisplayName = Language.GetTextValue("Mods.DarkSouls.ScalingSystem.DexterityDisplayName");
            DarkSoulsScalingSystem.IntelligenceDisplayName = Language.GetTextValue("Mods.DarkSouls.ScalingSystem.IntelligenceDisplayName");
            DarkSoulsScalingSystem.FaithDisplayName = Language.GetTextValue("Mods.DarkSouls.ScalingSystem.FaithDisplayName");
        }
    }
}
