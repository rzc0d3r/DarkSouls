using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using System.Collections.Generic;

using DarkSouls.DataStructures;
using static DarkSouls.Constants.Constants;

namespace DarkSouls.Items
{
    public class GlobalItemChanges : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ItemID.LifeCrystal || item.type == ItemID.ManaCrystal || item.type == ItemID.LifeFruit)
                return false;

            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            DarkSoulsScalingSystem.WeaponParams weaponParams = new();
            if (!DarkSoulsScalingSystem.AllWeaponsParams.TryGetValue(item.type, out weaponParams))
                return true;

            return dsPlayer.dsStrength >= weaponParams.ReqStrength &&
                   dsPlayer.dsDexterity >= weaponParams.ReqDexterity &&
                   dsPlayer.dsIntelligence >= weaponParams.ReqIntelligence &&
                   dsPlayer.dsFaith >= weaponParams.ReqFaith;
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            DarkSoulsScalingSystem.WeaponParams weaponParams;
            if (!DarkSoulsScalingSystem.AllWeaponsParams.TryGetValue(item.type, out weaponParams))
                return;

            DarkSoulsScalingSystem.DamageBonuses damageBonuses = DarkSoulsScalingSystem.GetBonusDamage(item);
            if (damageBonuses.total == 0)
                return;

            damage.Flat = damageBonuses.total;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            DarkSoulsScalingSystem.WeaponParams weaponParams;
            if (!DarkSoulsScalingSystem.AllWeaponsParams.TryGetValue(item.type, out weaponParams))
                return;

            TooltipLine customTooltipLine = new(Mod, "WeaponParams", weaponParams.ToTooltipText());
            tooltips.Add(customTooltipLine);

            DarkSoulsScalingSystem.DamageBonuses damageBonuses = DarkSoulsScalingSystem.GetBonusDamage(item);
            if (damageBonuses.total == 0)
                return;

            int damageLineIndex = tooltips.FindIndex(line => line.Mod == "Terraria" && line.Name == "Damage");
            if (damageLineIndex != -1)
            {
                TooltipLine damageLine = tooltips[damageLineIndex];
                damageLine.Text += $" ([{MediumSeaGreenColorTooltip}:+{damageBonuses.total}] = " +
                    $"[{MediumSeaGreenColorTooltip}:{damageBonuses.byStrength}]+" +
                    $"[{MediumSeaGreenColorTooltip}:{damageBonuses.byDexterity}]+" +
                    $"[{MediumSeaGreenColorTooltip}:{damageBonuses.byIntelligence}]+" +
                    $"[{MediumSeaGreenColorTooltip}:{damageBonuses.byFaith}])";
            }
        }
    }
}
