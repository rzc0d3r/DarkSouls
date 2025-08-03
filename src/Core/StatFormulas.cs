using DarkSouls.Config;
using System;

namespace DarkSouls.Core
{
    public static class StatFormulas
    {
        public static int GetHPByVitality(int vitality)
        {
            int hp = 0;
            hp += Math.Max(0, Math.Min(vitality - 1, 20)) * 15; // +300
            hp += Math.Max(0, Math.Min(vitality - 21, 20)) * 5; // +400
            hp += Math.Max(0, Math.Min(vitality - 41, 57)) * 2; // +514
            if (vitality >= 99)
                hp = (hp + 9) / 10 * 10; // rounding by multiples of 10
            return hp;
        }

        public static int GetManaByAttunement(int attunement)
        {
            int mana = 0;
            mana += Math.Max(0, Math.Min(attunement - 1, 9)) * 20; // +180
            mana += Math.Max(0, Math.Min(attunement - 10, 88)) * 2; // +356
            if (attunement >= 99)
                mana = (mana + 9) / 10 * 10; // rounding by multiples of 10
            return mana;
        }

        public static float GetDefenseByResistance(int resistance)
        {
            float defense = 0f;
            defense += Math.Max(0, Math.Min(resistance - 1, 25)) * 0.006f; // 15%
            defense += Math.Max(0, Math.Min(resistance - 26, 73)) * ((0.25f - defense) / 73); // 25%
            return defense;
        }

        public static float GetDebuffsResistanceByResistance(int resistance)
        {
            float debuffResistance = 0f;
            debuffResistance += Math.Max(0, Math.Min(resistance - 1, 25)) * 0.005f; // 12.5%
            debuffResistance += Math.Max(0, Math.Min(resistance - 26, 73)) * ((0.2f - debuffResistance) / 73); // 20%
            return debuffResistance;
        }

        public static int GetInvincibilityFramesByResistance(int resistance)
        {
            int dashDuration = DarkSoulsPlayer.dashDuration;
            int frames = dashDuration / 2; // 10
            frames += Math.Min(resistance, 31) * 5 / 31; // 10 + 5 = 15
            frames += Math.Max(resistance - 31, 0) * 5 / 68; // 10 + 5 + 5 = 20
            return frames;
        }

        public static float GetStaminaByEndurance(int endurance)
        {
            float stamina = DarkSoulsPlayer.DEFAULT_MAX_STAMINA;
            stamina += Math.Max(0, Math.Min(endurance - 1, 10)) * 3; // +30
            stamina += Math.Max(0, Math.Min(endurance - 11, 30)) * 2; // +60
            stamina += Math.Max(0, Math.Min(endurance - 41, 57)) * 1; // +57
            if (endurance >= 99)
                stamina = (stamina + 9) / 10 * 10; // rounding by multiples of 10
            return stamina;
        }

        public static float GetPotentialByStrength(int strength)
        {
            float potential = Math.Max(0, Math.Min(strength - 1, 40)) * (0.85f / 40); // 0.00 - 0.85 (strength: 1 - 41)
            potential += Math.Max(0, Math.Min(strength - 41, 58)) * (0.15f / 58); // 0.85 - 1.00 (strength: 41 - 99)
            return potential;
        }

        public static float GetPotentialByDexterity(int dexterity)
        {
            float potential = Math.Max(0, Math.Min(dexterity - 1, 40)) * (0.85f / 40); // 0.00 - 0.85 (dexterity: 1 - 41)
            potential += Math.Max(0, Math.Min(dexterity - 41, 58)) * (0.15f / 58); // 0.85 - 1.00 (dexterity: 41 - 99)
            return potential;
        }

        public static float GetPotentialByIntelligence(int intelligence)
        {
            float potential = Math.Max(0, Math.Min(intelligence - 1, 50)) * (0.85f / 50); // 0.00 - 0.85 (intelligence: 1 - 51)
            potential += Math.Max(0, Math.Min(intelligence - 51, 48)) * (0.15f / 48); // 0.85 - 1.00 (intelligence: 51 - 99)
            return potential;
        }

        public static float GetPotentialByFaith(int faith)
        {
            float potential = Math.Max(0, Math.Min(faith - 1, 50) * (0.85f / 50)); // 0.00 - 0.85 (faith: 1 - 51)
            potential += Math.Max(0, Math.Min(faith - 51, 48)) * (0.15f / 48); // 0.85 - 1.00 (faith: 51 - 99)
            return potential;
        }
        public static long GetReqSoulsByLevel(int level)
        {
            float multiplier = ServerConfig.Instance.LevelUpCostMultiplierPercent / 100f;
            int reqSouls = 0;

            if (level >= 35) // 35+
                reqSouls = (int)(0.02 * Math.Pow(level, 3) + 3.05 * Math.Pow(level, 2) + 90 * level - 6500);
            else if (level > 0 && level < 35) // 1 - 35
                reqSouls = (int)(500 * Math.Pow(1.025, level - 1)); // 2.5% increase per level

            reqSouls = (int)(reqSouls * multiplier);

            return reqSouls;
        }
    }
}
