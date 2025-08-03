using System.Collections.Generic;

using Terraria;
using Terraria.ID;

using DarkSouls.Core;
using static DarkSouls.Constants.Constants;

namespace DarkSouls.DataStructures
{
    public static class DarkSoulsScalingSystem
    {
        // Set by LocalizationUpdater
        public static string ReqParamDisplayName;
        public static string ParamBonusDisplayName;
        public static string StrengthDisplayName;
        public static string DexterityDisplayName;
        public static string IntelligenceDisplayName;
        public static string FaithDisplayName;

        public enum ScalingGrade
        {
            None = 0,
            S = 1,
            A = 2,
            B = 3,
            C = 4,
            D = 5,
            E = 6
        }

        public readonly struct WeaponParams
        {
            public int ReqStrength { get; }
            public int ReqDexterity { get; }
            public int ReqIntelligence { get; }
            public int ReqFaith { get; }

            public ScalingGrade StrengthScalingGrade { get; }
            public ScalingGrade DexterityScalingGrade { get; } 
            public ScalingGrade IntelligenceScalingGrade { get; }
            public ScalingGrade FaithScalingGrade { get; }

            public float Saturation { get; }

            public WeaponParams(
                int reqStrength = 0, int reqDexterity = 0, int reqIntelligence = 0, int reqFaith = 0,
                ScalingGrade strengthScalingGrade = ScalingGrade.None,
                ScalingGrade dexterityScalingGrade = ScalingGrade.None,
                ScalingGrade intelligenceScalingGrade = ScalingGrade.None,
                ScalingGrade faithScalingGrade = ScalingGrade.None,
                float saturation = 100)
            {
                ReqStrength = reqStrength;
                ReqDexterity = reqDexterity;
                ReqIntelligence = reqIntelligence;
                ReqFaith = reqFaith;

                StrengthScalingGrade = strengthScalingGrade;
                DexterityScalingGrade = dexterityScalingGrade;
                IntelligenceScalingGrade = intelligenceScalingGrade;
                FaithScalingGrade = faithScalingGrade;
                Saturation = saturation;
            }


            public string ToTooltipText()
            {
                DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
                string text = string.Empty;
                string strengthText = dsPlayer.dsStrength >= ReqStrength ? $"{StrengthDisplayName}: [{DodgerBlueColorTooltip}:{ReqStrength}]" : $"{StrengthDisplayName}: [{CrimsonColorTooltip}:{ReqStrength}]";
                string dexterityText = dsPlayer.dsDexterity >= ReqDexterity ? $"{DexterityDisplayName}: [{DodgerBlueColorTooltip}:{ReqDexterity}]" : $"{DexterityDisplayName}: [{CrimsonColorTooltip}:{ReqDexterity}]";
                string intelligenceText = dsPlayer.dsIntelligence >= ReqIntelligence ? $"{IntelligenceDisplayName}: [{DodgerBlueColorTooltip}:{ReqIntelligence}]" : $"{IntelligenceDisplayName}: [{CrimsonColorTooltip}:{ReqIntelligence}]";
                string faithText = dsPlayer.dsFaith >= ReqFaith ? $"{FaithDisplayName}: [{DodgerBlueColorTooltip}:{ReqFaith}]" : $"{FaithDisplayName}: [{CrimsonColorTooltip}:{ReqFaith}]";

                text = $"{ReqParamDisplayName}:\n  {strengthText}, {dexterityText}, {intelligenceText}, {faithText}\n";
                text += $"{ParamBonusDisplayName}:\n  {StrengthDisplayName}: {ScalingGradeToString(StrengthScalingGrade)}, " +
                        $"{DexterityDisplayName}: {ScalingGradeToString(DexterityScalingGrade)}, " +
                        $"{IntelligenceDisplayName}: {ScalingGradeToString(IntelligenceScalingGrade)}, " +
                        $"{FaithDisplayName}: {ScalingGradeToString(FaithScalingGrade)}";
                return text;
            }
        }

        public readonly struct DamageBonuses
        {
            public readonly int total;
            public readonly int byStrength;
            public readonly int byDexterity;
            public readonly int byIntelligence;
            public readonly int byFaith;

            public DamageBonuses(int total, int byStrength, int byDexterity, int byIntelligence, int byFaith)
            {
                this.total = total;
                this.byStrength = byStrength;
                this.byDexterity = byDexterity;
                this.byIntelligence = byIntelligence;
                this.byFaith = byFaith;
            }
        }


        private static Dictionary<int, WeaponParams> allWeaponsParams = new()
        {
            {ItemID.AbigailsFlower, new WeaponParams() },

            {ItemID.AdamantiteChainsaw, new WeaponParams(14, 10, 0, 0) },
            {ItemID.AdamantiteDrill, new WeaponParams(8, 0, 0, 0) },
            {ItemID.AdamantiteGlaive, new WeaponParams(15, 10, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.AdamantitePickaxe, new WeaponParams(8, 0, 0, 0) },
            {ItemID.AdamantiteRepeater, new WeaponParams(8, 16, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.AdamantiteSword, new WeaponParams(16, 10, 0, 0, ScalingGrade.C, ScalingGrade.E) },
            {ItemID.AdamantiteWaraxe, new WeaponParams(21, 6, 0, 0, ScalingGrade.C, ScalingGrade.E) },

            {ItemID.TitaniumChainsaw, new WeaponParams(14, 10, 0, 0) },
            {ItemID.TitaniumDrill, new WeaponParams(8, 0, 0, 0) },
            {ItemID.TitaniumTrident, new WeaponParams(15, 10, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.TitaniumPickaxe, new WeaponParams(8, 0, 0, 0) },
            {ItemID.TitaniumRepeater, new WeaponParams(8, 16, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.TitaniumSword, new WeaponParams(16, 10, 0, 0, ScalingGrade.C, ScalingGrade.E) },
            {ItemID.TitaniumWaraxe, new WeaponParams(21, 6, 0, 0, ScalingGrade.C, ScalingGrade.E) },

            {ItemID.DD2BetsyBow, new WeaponParams(8, 30, 0, 0, ScalingGrade.E, ScalingGrade.B) },
            {ItemID.AleThrowingGlove, new WeaponParams(8, 12, 0, 0, ScalingGrade.E, ScalingGrade.C) },
            {ItemID.Amarok, new WeaponParams(12, 12, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.JungleYoyo, new WeaponParams(5, 5, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.AmberStaff, new WeaponParams(2, 2, 5, 1, ScalingGrade.None, ScalingGrade.None, ScalingGrade.D, ScalingGrade.None) },
            {ItemID.AmethystStaff, new WeaponParams(2, 2, 5, 1, ScalingGrade.None, ScalingGrade.None, ScalingGrade.D, ScalingGrade.None) },
            {ItemID.Anchor, new WeaponParams(25, 5, 0, 0, ScalingGrade.B) },
            {ItemID.AquaScepter, new WeaponParams(2, 2, 12, 0, ScalingGrade.None, ScalingGrade.None, ScalingGrade.D, ScalingGrade.None) },
            {ItemID.Arkhalis, new WeaponParams(8, 12, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.CrimsonYoyo, new WeaponParams(6, 6, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.AshWoodBow, new WeaponParams() },
            {ItemID.AshWoodHammer, new WeaponParams()},
            {ItemID.AshWoodSword, new WeaponParams() },
            {ItemID.AcornAxe, new WeaponParams(3, 5, 0, 0) },
            {ItemID.BallOHurt, new WeaponParams(8, 5, 0, 0, ScalingGrade.E) },
            {ItemID.DD2BallistraTowerT1Popper, new WeaponParams(0, 0, 8, 15, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DD2BallistraTowerT2Popper, new WeaponParams(0, 0, 8, 15, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DD2BallistraTowerT3Popper, new WeaponParams(0, 0, 8, 15, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.Bananarang, new WeaponParams(12, 12, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.BatBat, new WeaponParams(18, 5, 0, 0, ScalingGrade.C, ScalingGrade.E) },
            {ItemID.BatScepter, new WeaponParams(3, 3, 22, 5, intelligenceScalingGrade: ScalingGrade.C, faithScalingGrade: ScalingGrade.E) },
            {ItemID.BeamSword, new WeaponParams(15, 5, 5, 0, ScalingGrade.C, ScalingGrade.None, ScalingGrade.E) },
            {ItemID.BeeGun, new WeaponParams(3, 2, 12, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.BeeKeeper, new WeaponParams(10, 5, 0, 0, ScalingGrade.D) },
            {ItemID.Beenade, new WeaponParams(5, 8, 0, 0, ScalingGrade.None, ScalingGrade.A) },
            {ItemID.ApprenticeStaffT3, new WeaponParams(5, 3, 37, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.BladeofGrass, new WeaponParams(8, 5, 0, 0, ScalingGrade.D) },
            {ItemID.Smolstar, new WeaponParams(1, 1, 3, 15, faithScalingGrade: ScalingGrade.C) },
            {ItemID.BladedGlove, new WeaponParams(5, 12, 0, 0, ScalingGrade.E, ScalingGrade.D) },
            {ItemID.Bladetongue, new WeaponParams(12, 5, 5, 0, ScalingGrade.D, ScalingGrade.None, ScalingGrade.E) },
            {ItemID.BlizzardStaff, new WeaponParams(2, 2, 36, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.BloodButcherer, new WeaponParams(10, 2, 0, 0, ScalingGrade.D) },
            {ItemID.BloodLustCluster, new WeaponParams(10, 2, 0, 0, ScalingGrade.D) },
            {ItemID.BloodRainBow, new WeaponParams(2, 10, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.SharpTears, new WeaponParams(2, 2, 16, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.BloodWater, new WeaponParams() },
            {ItemID.BloodyMachete, new WeaponParams(6, 6, 0, 0, ScalingGrade.E, ScalingGrade.E) },
            {ItemID.Blowgun, new WeaponParams(2, 12, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.Blowpipe, new WeaponParams(2, 3, 0, 0, dexterityScalingGrade: ScalingGrade.E) },
            {ItemID.BlueMoon, new WeaponParams(12, 5, 0, 0, ScalingGrade.D) },

            {ItemID.BluePhaseblade, new WeaponParams(8, 6, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.GreenPhaseblade, new WeaponParams(8, 6, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.OrangePhaseblade, new WeaponParams(8, 6, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.PurplePhaseblade, new WeaponParams(8, 6, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.RedPhaseblade, new WeaponParams(8, 6, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.WhitePhaseblade, new WeaponParams(8, 6, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.YellowPhaseblade, new WeaponParams(8, 6, 0, 0, ScalingGrade.D, ScalingGrade.E) },

            {ItemID.BluePhasesaber, new WeaponParams(12, 12, 0, 0, ScalingGrade.D, ScalingGrade.C) },
            {ItemID.GreenPhasesaber, new WeaponParams(12, 12, 0, 0, ScalingGrade.D, ScalingGrade.C) },
            {ItemID.OrangePhasesaber, new WeaponParams(12, 12, 0, 0, ScalingGrade.D, ScalingGrade.C) },
            {ItemID.PurplePhasesaber, new WeaponParams(12, 12, 0, 0, ScalingGrade.D, ScalingGrade.C) },
            {ItemID.RedPhasesaber, new WeaponParams(12, 12, 0, 0, ScalingGrade.D, ScalingGrade.C) },
            {ItemID.WhitePhasesaber, new WeaponParams(12, 12, 0, 0, ScalingGrade.D, ScalingGrade.B) },
            {ItemID.YellowPhasesaber, new WeaponParams(12, 12, 0, 0, ScalingGrade.D, ScalingGrade.C) },

            {ItemID.BlueRocket, new WeaponParams(0, 5, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.GreenRocket, new WeaponParams(0, 5, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.RedRocket, new WeaponParams(0, 5, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.YellowRocket, new WeaponParams(0, 5, 0, 0, dexterityScalingGrade: ScalingGrade.S) },

            {ItemID.Bomb, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.BombFish, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.Bone, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.BoneJavelin, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.BonePickaxe, new WeaponParams(3, 0, 0, 0) },
            {ItemID.BoneSword, new WeaponParams(8, 2, 0, 0, ScalingGrade.E) },
            {ItemID.BoneDagger, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.BookofSkulls, new WeaponParams(2, 2, 13, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.Boomstick, new WeaponParams(2, 8, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.BorealWoodBow, new WeaponParams() },
            {ItemID.BorealWoodHammer, new WeaponParams()},
            {ItemID.BorealWoodSword, new WeaponParams() },
            {ItemID.BouncyBomb, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.BouncyDynamite, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.BouncyGrenade, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.DD2SquireDemonSword, new WeaponParams(30, 8, 0, 0, ScalingGrade.C) },
            {ItemID.BreakerBlade, new WeaponParams(20, 5, 0, 0, ScalingGrade.B) },
            {ItemID.BreathingReed, new WeaponParams(3, 3, 0, 0) },
            {ItemID.BubbleGun, new WeaponParams(2, 2, 32, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.BunnyCannon, new WeaponParams() },
            {ItemID.ButchersChainsaw, new WeaponParams(20, 15, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.CactusPickaxe, new WeaponParams() },
            {ItemID.CactusSword, new WeaponParams(3, 1, 0, 0) },
            {ItemID.CnadyCanePickaxe, new WeaponParams(3, 0, 0, 0) },
            {ItemID.CandyCaneSword, new WeaponParams(8, 2, 0, 0, ScalingGrade.E) },
            {ItemID.CandyCornRifle, new WeaponParams(6, 25, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.Cannon, new WeaponParams() },
            {ItemID.Cascade, new WeaponParams(8, 8, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.FireworksLauncher, new WeaponParams(5, 25, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.Celeb2, new WeaponParams(10, 40, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.ChainGuillotines, new WeaponParams(20, 5, 0, 0, ScalingGrade.C) },
            {ItemID.ChainGun, new WeaponParams(8, 32, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.ChainKnife, new WeaponParams(6, 5, 0, 0, ScalingGrade.D) },
            {ItemID.ChargedBlasterCannon, new WeaponParams(3, 3, 35, 0, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.Chik, new WeaponParams(12, 12, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.ChlorophyteChainsaw, new WeaponParams(18, 8, 0, 0, ScalingGrade.C) },
            {ItemID.ChlorophyteClaymore, new WeaponParams(22, 5, 0, 0, ScalingGrade.C, ScalingGrade.E) },
            {ItemID.ChlorophyteDrill, new WeaponParams(9, 0, 0, 0) },
            {ItemID.ChlorophyteGreataxe, new WeaponParams(25, 5, 0, 0, ScalingGrade.B, ScalingGrade.E) },
            {ItemID.ChlorophyteJackhammer, new WeaponParams(15, 12, 0, 0) },
            {ItemID.ChlorophytePartisan, new WeaponParams(20, 10, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.ChlorophyteSaber, new WeaponParams(20, 6, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.ChlorophyteShotbow, new WeaponParams(5, 26, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.ChlorophyteWarhammer, new WeaponParams(28, 5, 0, 0, ScalingGrade.A, ScalingGrade.E) },
            {ItemID.ChlorophytePickaxe, new WeaponParams(9, 0, 0, 0) },

            {ItemID.ChristmasTreeSword, new WeaponParams(25, 8, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.TaxCollectorsStickOfDoom, new WeaponParams(6, 6, 0, 0, ScalingGrade.B, ScalingGrade.B) },
            {ItemID.ClingerStaff, new WeaponParams(3, 2, 25, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.ClockworkAssaultRifle, new WeaponParams(5, 15, 0, 0, dexterityScalingGrade: ScalingGrade.C) },

            {ItemID.CobaltChainsaw, new WeaponParams(12, 8, 0, 0) },
            {ItemID.CobaltDrill, new WeaponParams(6, 0, 0, 0) },
            {ItemID.CobaltNaginata, new WeaponParams(12, 8, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.CobaltPickaxe, new WeaponParams(6, 0, 0, 0) },
            {ItemID.CobaltRepeater, new WeaponParams(8, 12, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.CobaltSword, new WeaponParams(15, 5, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.CobaltWaraxe, new WeaponParams(18, 4, 0, 0, ScalingGrade.D, ScalingGrade.E) },

            {ItemID.PalladiumChainsaw, new WeaponParams(12, 8, 0, 0) },
            {ItemID.PalladiumDrill, new WeaponParams(6, 0, 0, 0) },
            {ItemID.PalladiumPike, new WeaponParams(12, 8, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.PalladiumPickaxe, new WeaponParams(6, 0, 0, 0) },
            {ItemID.PalladiumRepeater, new WeaponParams(8, 12, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.PalladiumSword, new WeaponParams(15, 5, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.PalladiumWaraxe, new WeaponParams(18, 4, 0, 0, ScalingGrade.D, ScalingGrade.E) },

            {ItemID.Code1, new WeaponParams(10, 10, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.Code2, new WeaponParams(14, 14, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.CoinGun, new WeaponParams(5, 20, 0, 0) },
            {ItemID.CombatWrench, new WeaponParams(8, 8, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.CoolWhip, new WeaponParams(2, 3, 0, 18, faithScalingGrade: ScalingGrade.C) },
            {ItemID.CopperAxe, new WeaponParams() },
            {ItemID.CopperBow, new WeaponParams() },
            {ItemID.CopperBroadsword, new WeaponParams() },
            {ItemID.CopperHammer, new WeaponParams() },
            {ItemID.CopperPickaxe, new WeaponParams() },
            {ItemID.CopperShortsword, new WeaponParams() },
            {ItemID.CrimsonRod, new WeaponParams(1, 1, 10, 0, intelligenceScalingGrade: ScalingGrade.E) },
            {ItemID.CrystalSerpent, new WeaponParams(2, 2, 16, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.CrystalStorm, new WeaponParams(2, 2, 16, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.CrystalVileShard, new WeaponParams(2, 2, 14, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.CursedFlames, new WeaponParams(2, 2, 20, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.Cutlass, new WeaponParams(16, 8, 0, 0, ScalingGrade.C, ScalingGrade.E) },
            {ItemID.DaedalusStormbow, new WeaponParams(4, 21, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.DaoofPow, new WeaponParams(20, 5, 0, 0, ScalingGrade.C) },
            {ItemID.ScytheWhip, new WeaponParams(2, 4, 0, 30, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DarkLance, new WeaponParams(12, 5, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.DartPistol, new WeaponParams(5, 15, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.DartRifle, new WeaponParams(5, 15, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.DeadlySphereStaff, new WeaponParams(2, 2, 0, 35, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DeathSickle, new WeaponParams(26, 8, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.DeathbringerPickaxe, new WeaponParams(4, 0, 0, 0) },
            {ItemID.DemonBow, new WeaponParams(4, 8, 0, 0, dexterityScalingGrade: ScalingGrade.E) },
            {ItemID.DemonScythe, new WeaponParams(2, 2, 16, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.StormTigerStaff, new WeaponParams(2, 2, 0, 36, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DiamondStaff, new WeaponParams(2, 2, 6, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.DirtBomb, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.Drax, new WeaponParams(9, 0, 0, 0) },
            {ItemID.DripplerFlail, new WeaponParams(15, 3, 0, 0, ScalingGrade.C) },
            {ItemID.DryBomb, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.SwordWhip, new WeaponParams(2, 3, 0, 24, faithScalingGrade: ScalingGrade.C) },
            {ItemID.Dynamite, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.EbonwoodBow, new WeaponParams() },
            {ItemID.EbonwoodHammer, new WeaponParams() },
            {ItemID.EbonwoodSword, new WeaponParams() },
            {ItemID.ElectrosphereLauncher, new WeaponParams(2, 32, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.ElfMelter, new WeaponParams(2, 32, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.EmeraldStaff, new WeaponParams(2, 2, 6, 0, intelligenceScalingGrade: ScalingGrade.E) },
            {ItemID.EnchantedBoomerang, new WeaponParams(5, 5, 0, 0, ScalingGrade.E, ScalingGrade.E) },
            {ItemID.EnchantedSword, new WeaponParams(7, 2, 2, 0, ScalingGrade.D, ScalingGrade.None, ScalingGrade.E) },
            {ItemID.FairyQueenRangedItem, new WeaponParams(2, 30, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.Excalibur, new WeaponParams(20, 8, 0, 0, ScalingGrade.B, ScalingGrade.E) },
            {ItemID.DyeTradersScimitar, new WeaponParams(10, 4, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.DD2ExplosiveTrapT1Popper, new WeaponParams(0, 0, 8, 15, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DD2ExplosiveTrapT2Popper, new WeaponParams(0, 0, 8, 15, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DD2ExplosiveTrapT3Popper, new WeaponParams(0, 0, 8, 15, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.FalconBlade, new WeaponParams(8, 6, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.FetidBaghnakhs, new WeaponParams(15, 10, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.BabyBirdStaff, new WeaponParams() },
            {ItemID.FireWhip, new WeaponParams(2, 3, 0, 15, faithScalingGrade: ScalingGrade.D) },
            {ItemID.Flairon, new WeaponParams(28, 4, 0, 0, ScalingGrade.B, ScalingGrade.E) },
            {ItemID.Flamarang, new WeaponParams(10, 10, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.DD2FlameburstTowerT1Popper, new WeaponParams(0, 0, 8, 15, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DD2FlameburstTowerT2Popper, new WeaponParams(0, 0, 8, 15, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DD2FlameburstTowerT3Popper, new WeaponParams(0, 0, 8, 15, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.Flamelash, new WeaponParams(2, 2, 18, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.Flamethrower, new WeaponParams(2, 22, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.FlamingMace, new WeaponParams(10, 5, 0, 0, ScalingGrade.D) },
            {ItemID.FlareGun, new WeaponParams(2, 10, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.FleshGrinder, new WeaponParams(12, 3, 0, 0, ScalingGrade.C) },
            {ItemID.FlintlockPistol, new WeaponParams(2, 8, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.FlinxStaff, new WeaponParams(1, 1, 0, 6, faithScalingGrade: ScalingGrade.E) },
            {ItemID.FlowerofFire, new WeaponParams(2, 2, 14, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.FlowerofFrost, new WeaponParams(2, 2, 16, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.FlowerPow, new WeaponParams(28, 5, 0, 0, ScalingGrade.B) },
            {ItemID.DD2SquireBetsySword, new WeaponParams(30, 6, 0, 0, ScalingGrade.B) },
            {ItemID.FlyingKnife, new WeaponParams(18, 5, 0, 0, ScalingGrade.D) },
            {ItemID.Flymeal, new WeaponParams(8, 5, 0, 0, ScalingGrade.E) },
            {ItemID.FormatC, new WeaponParams(11, 11, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.FossilPickaxe, new WeaponParams(3, 0, 0, 0) },
            {ItemID.FrostDaggerfish, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.FrostStaff, new WeaponParams(2, 2, 16, 0, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.Frostbrand, new WeaponParams(10, 4, 6, 0, ScalingGrade.D, ScalingGrade.E, ScalingGrade.E) },
            {ItemID.FruitcakeChakram, new WeaponParams(6, 6, 0, 0, ScalingGrade.E, ScalingGrade.E) },
            {ItemID.Gatligator, new WeaponParams(3, 15, 0, 0, ScalingGrade.None, ScalingGrade.C) },
            {ItemID.MonkStaffT2, new WeaponParams(16, 12, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.Gladius, new WeaponParams(8, 6, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.GoldAxe, new WeaponParams(5, 4, 0, 0) },
            {ItemID.GoldBow, new WeaponParams(4, 6, 0, 0, dexterityScalingGrade: ScalingGrade.E) },
            {ItemID.GoldBroadsword, new WeaponParams(8, 2, 0, 0, ScalingGrade.E) },
            {ItemID.GoldHammer, new WeaponParams(9, 2, 0, 0, ScalingGrade.D) },
            {ItemID.GoldPickaxe, new WeaponParams(3, 0, 0, 0) },
            {ItemID.GoldenShower, new WeaponParams(2, 2, 15, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.GolemFist, new WeaponParams(30, 2, 0, 0, ScalingGrade.A) },
            {ItemID.Gradient, new WeaponParams(13, 13, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.GravediggerShovel, new WeaponParams(2, 6, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.ZapinatorGray, new WeaponParams(2, 2, 14, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.Grenade, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.GrenadeLauncher, new WeaponParams(2, 26, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.Gungnir, new WeaponParams(16, 12, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.BloodHamaxe, new WeaponParams(16, 4, 0, 0, ScalingGrade.B) },
            {ItemID.HallowJoustingLance, new WeaponParams(30, 5, 0, 0, ScalingGrade.S) },
            {ItemID.HallowedRepeater, new WeaponParams(10, 18, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.HamBat, new WeaponParams(25, 5, 0, 0, ScalingGrade.A) },
            {ItemID.Hammush, new WeaponParams(20, 3, 0, 0, ScalingGrade.C) },
            {ItemID.Handgun, new WeaponParams(3, 12, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.PartyGirlGrenade, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.Harpoon, new WeaponParams(2, 13, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.HeatRay, new WeaponParams(1, 1, 30, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.HelFire, new WeaponParams(11, 11, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.HellwingBow, new WeaponParams(2, 12, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.HiveFive, new WeaponParams(7, 7, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.HolyWater, new WeaponParams() },
            {ItemID.HoneyBomb, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.HornetStaff, new WeaponParams(2, 2, 0, 8, faithScalingGrade: ScalingGrade.D) },
            {ItemID.HoundiusShootius, new WeaponParams(2, 2, 0, 10, faithScalingGrade: ScalingGrade.D) },
            {ItemID.IceBlade, new WeaponParams(6, 2, 2, 0, ScalingGrade.E, ScalingGrade.None, ScalingGrade.E) },
            {ItemID.IceBoomerang, new WeaponParams(6, 6, 0, 0, ScalingGrade.E, ScalingGrade.E) },
            {ItemID.IceBow, new WeaponParams(4, 16, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.IceRod, new WeaponParams(2, 2, 16, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.IceSickle, new WeaponParams(15, 5, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.ImpStaff, new WeaponParams(2, 2, 0, 13, faithScalingGrade: ScalingGrade.D) },
            {ItemID.InfernoFork, new WeaponParams(2, 2, 32, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.InfluxWaver, new WeaponParams(26, 10, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.IronAxe, new WeaponParams() },
            {ItemID.IronBow, new WeaponParams() },
            {ItemID.IronBroadsword, new WeaponParams() },
            {ItemID.IronHammer, new WeaponParams() },
            {ItemID.IronPickaxe, new WeaponParams() },
            {ItemID.IronShortsword, new WeaponParams() },
            {ItemID.JackOLanternLauncher, new WeaponParams(5, 30, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.Javelin, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.JoustingLance, new WeaponParams(20, 5, 0, 0, ScalingGrade.S) },
            {ItemID.RainbowWhip, new WeaponParams(2, 6, 0, 36, faithScalingGrade: ScalingGrade.A) },
            {ItemID.Katana, new WeaponParams(8, 2, 0, 0, ScalingGrade.E) },
            {ItemID.Keybrand, new WeaponParams(28, 6, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.KOCannon, new WeaponParams(18, 5, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.Kraken, new WeaponParams(18, 18, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.LandMine, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.LaserDrill, new WeaponParams(11, 0, 0, 0) },
            {ItemID.LaserMachinegun, new WeaponParams(4, 2, 32, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.LaserRifle, new WeaponParams(2, 2, 16, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.LastPrism, new WeaponParams(2, 2, 50, 0, intelligenceScalingGrade: ScalingGrade.A) },
            {ItemID.LavaBomb, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.LeadBow, new WeaponParams() },
            {ItemID.LeadBroadsword, new WeaponParams() },
            {ItemID.LeadHammer, new WeaponParams() },
            {ItemID.LeadPickaxe, new WeaponParams() },
            {ItemID.LeadShortsword, new WeaponParams() },
            {ItemID.LeafBlower, new WeaponParams(1, 1, 30, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.BlandWhip, new WeaponParams(1, 1, 0, 3, faithScalingGrade: ScalingGrade.D) },
            {ItemID.SoulDrain, new WeaponParams(1, 1, 22, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.LightDisc, new WeaponParams(14, 14, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.LightsBane, new WeaponParams(10, 2, 0, 0, ScalingGrade.D) },
            {ItemID.DD2LightningAuraT1Popper, new WeaponParams(0, 0, 6, 12, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DD2LightningAuraT2Popper, new WeaponParams(0, 0, 6, 12, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.DD2LightningAuraT3Popper, new WeaponParams(0, 0, 6, 12, intelligenceScalingGrade: ScalingGrade.E, faithScalingGrade: ScalingGrade.C) },
            {ItemID.LucyTheAxe, new WeaponParams(12, 2, 0, 0, ScalingGrade.D) },
            {ItemID.LunarFlareBook, new WeaponParams(2, 2, 50, 0, intelligenceScalingGrade: ScalingGrade.A) },
            {ItemID.MoonlordTurretStaff, new WeaponParams(2, 2, 0, 45, faithScalingGrade: ScalingGrade.A) },
            {ItemID.Mace, new WeaponParams(8, 4, 0, 0, ScalingGrade.D) },
            {ItemID.MagicDagger, new WeaponParams(2, 2, 18, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.MagicMissile, new WeaponParams(2, 2, 12, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.MagicalHarp, new WeaponParams(2, 2, 18, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.MagnetSphere, new WeaponParams(2, 2, 30, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.CorruptYoyo, new WeaponParams(6, 6, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.AntlionClaw, new WeaponParams(8, 4, 0, 0, ScalingGrade.E) },
            {ItemID.Marrow, new WeaponParams(4, 20, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.MedusaHead, new WeaponParams(1, 1, 20, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.Megashark, new WeaponParams(3, 21, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.Meowmere, new WeaponParams(40, 10, 0, 0, ScalingGrade.A, ScalingGrade.D) },
            {ItemID.MeteorHamaxe, new WeaponParams(12, 3, 0, 0, ScalingGrade.D) },
            {ItemID.MeteorStaff, new WeaponParams(2, 2, 21, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.Minishark, new WeaponParams(2, 10, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.MolotovCocktail, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.MoltenFury, new WeaponParams(2, 14, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.MoltenHamaxe, new WeaponParams(14, 2, 0, 0, ScalingGrade.C) },
            {ItemID.MoltenPickaxe, new WeaponParams(5, 0, 0, 0) },
            {ItemID.MaceWhip, new WeaponParams(2, 5, 0, 32, faithScalingGrade: ScalingGrade.B) },
            {ItemID.Muramasa, new WeaponParams(8, 6, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.MushroomSpear, new WeaponParams(15, 8, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.Musket, new WeaponParams(2, 12, 0, 0, dexterityScalingGrade: ScalingGrade.C) },

            {ItemID.MythrilChainsaw, new WeaponParams(14, 8, 0, 0) },
            {ItemID.MythrilDrill, new WeaponParams(7, 0, 0, 0) },
            {ItemID.MythrilHalberd, new WeaponParams(12, 10, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.MythrilPickaxe, new WeaponParams(7, 0, 0, 0) },
            {ItemID.MythrilRepeater, new WeaponParams(6, 15, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.MythrilSword, new WeaponParams(18, 5, 0, 0, ScalingGrade.C, ScalingGrade.E) },
            {ItemID.MythrilWaraxe, new WeaponParams(20, 5, 0, 0, ScalingGrade.C, ScalingGrade.E) },

            {ItemID.OrichalcumChainsaw, new WeaponParams(14, 8, 0, 0) },
            {ItemID.OrichalcumDrill, new WeaponParams(7, 0, 0, 0) },
            {ItemID.OrichalcumHalberd, new WeaponParams(12, 10, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.OrichalcumPickaxe, new WeaponParams(7, 0, 0, 0) },
            {ItemID.OrichalcumRepeater, new WeaponParams(6, 15, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.OrichalcumSword, new WeaponParams(18, 5, 0, 0, ScalingGrade.C, ScalingGrade.E) },
            {ItemID.OrichalcumWaraxe, new WeaponParams(20, 5, 0, 0, ScalingGrade.C, ScalingGrade.E) },

            {ItemID.NailGun, new WeaponParams(6, 27, 0, 0, dexterityScalingGrade: ScalingGrade.C) },

            {ItemID.NebulaArcanum, new WeaponParams(2, 2, 40, 0, intelligenceScalingGrade: ScalingGrade.A) },
            {ItemID.NebulaBlaze, new WeaponParams(2, 2, 42, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.NebulaDrill, new WeaponParams(11, 0, 0, 0) },
            {ItemID.NebulaPickaxe, new WeaponParams(11, 0, 0, 0) },

            {ItemID.DayBreak, new WeaponParams(40, 5, 0, 0, ScalingGrade.A) },
            {ItemID.SolarEruption, new WeaponParams(40, 5, 0, 0, ScalingGrade.A) },
            {ItemID.SolarFlareDrill, new WeaponParams(11, 0, 0, 0) },
            {ItemID.SolarFlarePickaxe, new WeaponParams(11, 0, 0, 0) },

            {ItemID.VortexBeater, new WeaponParams(5, 40, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.Phantasm, new WeaponParams(5, 40, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.VortexDrill, new WeaponParams(11, 0, 0, 0) },
            {ItemID.VortexPickaxe, new WeaponParams(11, 0, 0, 0) },

            {ItemID.StardustCellStaff, new WeaponParams(3, 2, 0, 40, faithScalingGrade: ScalingGrade.A) },
            {ItemID.StardustDragonStaff, new WeaponParams(3, 2, 0, 40, faithScalingGrade: ScalingGrade.A) },
            {ItemID.StardustDrill, new WeaponParams(11, 0, 0, 0) },
            {ItemID.StardustPickaxe, new WeaponParams(11, 0, 0, 0) },

            {ItemID.NettleBurst, new WeaponParams(2, 2, 30, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.NightsEdge, new WeaponParams(13, 5, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.FairyQueenMagicItem, new WeaponParams(2, 2, 35, 0, intelligenceScalingGrade: ScalingGrade.A) },
            {ItemID.NightmarePickaxe, new WeaponParams(4, 0, 0, 0) },
            {ItemID.NimbusRod, new WeaponParams(1, 1, 14, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.NorthPole, new WeaponParams(2, 2, 34, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.ObsidianSwordfish, new WeaponParams(22, 5, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.OnyxBlaster, new WeaponParams(6, 14, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.OpticStaff, new WeaponParams(2, 2, 0, 20, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.ZapinatorOrange, new WeaponParams(2, 2, 18, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.PainterPaintballGun, new WeaponParams(2, 8, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.PaladinsHammer, new WeaponParams(30, 5, 0, 0, ScalingGrade.A, ScalingGrade.E) },
            {ItemID.PalmWoodBow, new WeaponParams() },
            {ItemID.PalmWoodHammer, new WeaponParams() },
            {ItemID.PalmWoodSword, new WeaponParams() },
            {ItemID.PaperAirplaneA, new WeaponParams(0, 3, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.PaperAirplaneB, new WeaponParams(0, 3, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.PearlwoodBow, new WeaponParams() },
            {ItemID.PearlwoodHammer, new WeaponParams() },
            {ItemID.PewMaticHorn, new WeaponParams(2, 8, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.DD2PhoenixBow, new WeaponParams(3, 22, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.PhoenixBlaster, new WeaponParams(3, 14, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.PickaxeAxe, new WeaponParams(9, 0, 0, 0) },
            {ItemID.Picksaw, new WeaponParams(10, 0, 0, 0) },
            {ItemID.PiranhaGun, new WeaponParams(4, 26, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.PirateStaff, new WeaponParams(2, 2, 0, 22, faithScalingGrade: ScalingGrade.B) },
            {ItemID.PlatinumAxe, new WeaponParams(5, 4, 0, 0) },
            {ItemID.PlatinumBow, new WeaponParams(4, 6, 0, 0, dexterityScalingGrade: ScalingGrade.E) },
            {ItemID.PlatinumBroadsword, new WeaponParams(8, 2, 0, 0, ScalingGrade.E) },
            {ItemID.PlatinumHammer, new WeaponParams(9, 2, 0, 0, ScalingGrade.D) },
            {ItemID.PlatinumPickaxe, new WeaponParams(3, 0, 0, 0) },
            {ItemID.PlatinumShortsword, new WeaponParams(6, 2, 0, 0, ScalingGrade.E) },
            {ItemID.PoisonStaff, new WeaponParams(2, 2, 20, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.PoisonedKnife, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.PossessedHatchet, new WeaponParams(26, 5, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.ProximityMineLauncher, new WeaponParams(6, 24, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.PsychoKnife, new WeaponParams(22, 10, 0, 0, ScalingGrade.A, ScalingGrade.C) },
            {ItemID.PulseBow, new WeaponParams(4, 24, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.PurpleClubberfish, new WeaponParams(12, 5, 0, 0, ScalingGrade.A, ScalingGrade.D) },
            {ItemID.Pwnhammer, new WeaponParams(18, 2, 0, 0, ScalingGrade.B, ScalingGrade.E) },
            {ItemID.PygmyStaff, new WeaponParams(2, 2, 0, 26, faithScalingGrade: ScalingGrade.B) },
            {ItemID.QuadBarrelShotgun, new WeaponParams(3, 10, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.QueenSpiderStaff, new WeaponParams(1, 1, 0, 18, faithScalingGrade: ScalingGrade.D) },
            {ItemID.RainbowCrystalStaff, new WeaponParams(2, 2, 0, 46, faithScalingGrade: ScalingGrade.A) },
            {ItemID.RainbowGun, new WeaponParams(2, 2, 30, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.RainbowRod, new WeaponParams(2, 2, 21, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.Rally, new WeaponParams(6, 6, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.RavenStaff, new WeaponParams(2, 2, 0, 36, faithScalingGrade: ScalingGrade.A) },
            {ItemID.RazorbladeTyphoon, new WeaponParams(2, 2, 34, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.Razorpine, new WeaponParams(2, 2, 32, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.ReaverShark, new WeaponParams(10, 4, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.RedRyder, new WeaponParams(2, 12, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.RedsYoyo, new WeaponParams(15, 15, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.PrincessWeapon, new WeaponParams(2, 2, 30, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.Revolver, new WeaponParams(2, 12, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.RichMahoganyBow, new WeaponParams() },
            {ItemID.RichMahoganyHammer, new WeaponParams() },
            {ItemID.RichMahoganySword, new WeaponParams() },
            {ItemID.RocketLauncher, new WeaponParams(6, 25, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.Rockfish, new WeaponParams(11, 4, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.RottenEgg, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.RubyStaff, new WeaponParams(2, 2, 6, 0, intelligenceScalingGrade: ScalingGrade.E) },
            {ItemID.Ruler, new WeaponParams(6, 4, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.SDMG, new WeaponParams(5, 45, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.Sandgun, new WeaponParams(2, 15, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.SanguineStaff, new WeaponParams(2, 2, 0, 17, faithScalingGrade: ScalingGrade.D) },
            {ItemID.SapphireStaff, new WeaponParams(2, 2, 6, 0, intelligenceScalingGrade: ScalingGrade.E) },
            {ItemID.SawtoothShark, new WeaponParams(8, 4, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.ScarabBomb, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.ScourgeoftheCorruptor, new WeaponParams(25, 8, 0, 0, ScalingGrade.B, ScalingGrade.C) },
            {ItemID.Seedler, new WeaponParams(24, 6, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.BouncingShield, new WeaponParams(12, 12, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.ShadewoodBow, new WeaponParams() },
            {ItemID.ShadewoodHammer, new WeaponParams() },
            {ItemID.ShadowJoustingLance, new WeaponParams(34, 6, 0, 0, ScalingGrade.S, ScalingGrade.D) },
            {ItemID.ShadowbeamStaff, new WeaponParams(2, 3, 28, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.ShadowFlameBow, new WeaponParams(2, 18, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.ShadowFlameHexDoll, new WeaponParams(2, 1, 18, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.ShadowFlameKnife, new WeaponParams(10, 8, 0, 0, ScalingGrade.C, ScalingGrade.C) },
            {ItemID.Shotgun, new WeaponParams(6, 12, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.Shroomerang, new WeaponParams(6, 6, 0, 0, ScalingGrade.E, ScalingGrade.E) },
            {ItemID.ShroomiteDiggingClaw, new WeaponParams(9, 0, 0, 0) },
            {ItemID.Shuriken, new WeaponParams(2, 4, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.Sickle, new WeaponParams(4, 2, 0, 0, ScalingGrade.E) },
            {ItemID.SilverAxe, new WeaponParams(4, 2, 0, 0) },
            {ItemID.SilverBow, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.E) },
            {ItemID.SilverBroadsword, new WeaponParams(5, 2, 0, 0, ScalingGrade.E) },
            {ItemID.SilverHammer, new WeaponParams(6, 2, 0, 0, ScalingGrade.D) },
            {ItemID.SilverPickaxe, new WeaponParams(2, 0, 0, 0) },
            {ItemID.SilverShortsword, new WeaponParams(4, 2, 0, 0, ScalingGrade.E) },
            {ItemID.MonkStaffT3, new WeaponParams(28, 8, 0, 0, ScalingGrade.B, ScalingGrade.C) },
            {ItemID.SkyFracture, new WeaponParams(2, 2, 20, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.SlapHand, new WeaponParams(21, 5, 0, 0, ScalingGrade.S, ScalingGrade.D) },
            {ItemID.SlimeStaff, new WeaponParams(1, 1, 0, 7, faithScalingGrade: ScalingGrade.E) },
            {ItemID.ThornWhip, new WeaponParams(2, 2, 0, 10, faithScalingGrade: ScalingGrade.D) },
            {ItemID.SniperRifle, new WeaponParams(8, 30, 0, 0, dexterityScalingGrade: ScalingGrade.S) },
            {ItemID.Snowball, new WeaponParams(3, 3, 0, 0, ScalingGrade.B, ScalingGrade.B) },
            {ItemID.SnowballCannon, new WeaponParams(2, 8, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.SnowballLauncher, new WeaponParams() },
            {ItemID.SnowmanCannon, new WeaponParams(6, 30, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.SpaceGun, new WeaponParams(2, 2, 10, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.Spear, new WeaponParams(3, 2, 0, 0) },
            {ItemID.SpectreHamaxe, new WeaponParams(27, 5, 0, 0, ScalingGrade.B, ScalingGrade.C) },
            {ItemID.SpectrePickaxe, new WeaponParams(9, 0, 0, 0) },
            {ItemID.SpectreStaff, new WeaponParams(1, 1, 30, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.SpiderStaff, new WeaponParams(2, 2, 0, 20, faithScalingGrade: ScalingGrade.C) },
            {ItemID.SpikyBall, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.BoneWhip, new WeaponParams(2, 2, 0, 12, faithScalingGrade: ScalingGrade.D) },
            {ItemID.SpiritFlame, new WeaponParams(2, 2, 18, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.StaffofEarth, new WeaponParams(5, 2, 25, 0, intelligenceScalingGrade: ScalingGrade.A) },
            {ItemID.StaffofRegrowth, new WeaponParams(3, 2, 1, 0) },
            {ItemID.StaffoftheFrostHydra, new WeaponParams(2, 2, 0, 30, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.StakeLauncher, new WeaponParams(5, 31, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.StarAnise, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.StarCannon, new WeaponParams(4, 14, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.StarWrath, new WeaponParams(40, 6, 4, 0, ScalingGrade.A, ScalingGrade.D, ScalingGrade.D, saturation: 60) },
            {ItemID.Starfury, new WeaponParams(9, 2, 4, 0, ScalingGrade.E, ScalingGrade.None, ScalingGrade.E) },
            {ItemID.PiercingStarlight, new WeaponParams(18, 20, 0, 0, ScalingGrade.C, ScalingGrade.A) },
            {ItemID.SparkleGuitar, new WeaponParams(3, 5, 32, 0, intelligenceScalingGrade: ScalingGrade.B) },
            {ItemID.StickyBomb, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.DirtStickyBomb, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.StickyDynamite, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.StickyGrenade, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.ThunderSpear, new WeaponParams(6, 4, 0, 0, ScalingGrade.E) },
            {ItemID.StylistKilLaKillScissorsIWish, new WeaponParams(6, 4, 0, 0, ScalingGrade.E) },
            {ItemID.Stynger, new WeaponParams(5, 28, 0, 0, dexterityScalingGrade: ScalingGrade.A) },
            {ItemID.Sunfury, new WeaponParams(14, 5, 0, 0, ScalingGrade.B) },
            {ItemID.SuperStarCannon, new WeaponParams(5, 23, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.Swordfish, new WeaponParams(10, 5, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.TacticalShotgun, new WeaponParams(5, 28, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.TempestStaff, new WeaponParams(2, 2, 0, 30, faithScalingGrade: ScalingGrade.B) },
            {ItemID.TendonBow, new WeaponParams(2, 8, 0, 0, dexterityScalingGrade: ScalingGrade.E) },
            {ItemID.TentacleSpike, new WeaponParams(9, 4, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.TerraBlade, new WeaponParams(25, 6, 4, 0, ScalingGrade.B, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.Terragrim, new WeaponParams(5, 8, 0, 0, ScalingGrade.E, ScalingGrade.D) },
            {ItemID.EmpressBlade, new WeaponParams(2, 2, 0, 34, faithScalingGrade: ScalingGrade.A) },
            {ItemID.Terrarian, new WeaponParams(25, 25, 0, 0, ScalingGrade.A, ScalingGrade.A, saturation: 30) },
            {ItemID.TheAxe, new WeaponParams(20, 8, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.BeesKnees, new WeaponParams(2, 12, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.TheBreaker, new WeaponParams(12, 2, 0, 0, ScalingGrade.C) },
            {ItemID.TheEyeOfCthulhu, new WeaponParams(19, 19, 0, 0, ScalingGrade.A, ScalingGrade.A, saturation: 40) },
            {ItemID.TheHorsemansBlade, new WeaponParams(25, 8, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.TheMeatball, new WeaponParams(10, 5, 0, 0, ScalingGrade.D) },
            {ItemID.TheRottedFork, new WeaponParams(3, 8, 0, 0, ScalingGrade.E, ScalingGrade.E) },
            {ItemID.TheUndertaker, new WeaponParams(2, 9, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.ThornChakram, new WeaponParams(7, 7, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.ThrowingKnife, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.ThunderStaff, new WeaponParams(2, 2, 7, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.TinAxe, new WeaponParams() },
            {ItemID.TinBow, new WeaponParams() },
            {ItemID.TinBroadsword, new WeaponParams() },
            {ItemID.TinHammer, new WeaponParams() },
            {ItemID.TinPickaxe, new WeaponParams() },
            {ItemID.TinShortsword, new WeaponParams() },
            {ItemID.BookStaff, new WeaponParams(2, 2, 20, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.TopazStaff, new WeaponParams(2, 2, 6, 0, intelligenceScalingGrade: ScalingGrade.E) },
            {ItemID.ToxicFlask, new WeaponParams(2, 2, 28, 0, intelligenceScalingGrade: ScalingGrade.A) },
            {ItemID.Toxikarp, new WeaponParams(2, 20, 0, 0, dexterityScalingGrade: ScalingGrade.D) },
            {ItemID.TragicUmbrella, new WeaponParams(6, 5, 0, 0, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.Trident, new WeaponParams(6, 4, 0, 0, ScalingGrade.E, ScalingGrade.E) },
            {ItemID.Trimarang, new WeaponParams(6, 6, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.TrueExcalibur, new WeaponParams(21, 8, 0, 0, ScalingGrade.B, ScalingGrade.D) },
            {ItemID.TrueNightsEdge, new WeaponParams(20, 5, 4, 0, ScalingGrade.B, ScalingGrade.D, ScalingGrade.E) },
            {ItemID.Tsunami, new WeaponParams(4, 26, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.TungstenAxe, new WeaponParams(4, 2, 0, 0) },
            {ItemID.TungstenBow, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.E) },
            {ItemID.TungstenBroadsword, new WeaponParams(5, 2, 0, 0, ScalingGrade.E) },
            {ItemID.TungstenHammer, new WeaponParams(6, 2, 0, 0, ScalingGrade.D) },
            {ItemID.TungstenPickaxe, new WeaponParams(2, 0, 0, 0) },
            {ItemID.TungstenShortsword, new WeaponParams(4, 2, 0, 0, ScalingGrade.E) },
            {ItemID.Umbrella, new WeaponParams(5, 4, 0, 0, ScalingGrade.E, ScalingGrade.E) },
            {ItemID.UnholyTrident, new WeaponParams(4, 2, 18, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.UnholyWater, new WeaponParams() },
            {ItemID.Uzi, new WeaponParams(2, 18, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.ValkyrieYoyo, new WeaponParams(15, 15, 0, 0, ScalingGrade.C, ScalingGrade.C, saturation: 80) },
            {ItemID.Valor, new WeaponParams(8, 8, 0, 0, ScalingGrade.D, ScalingGrade.D) },
            {ItemID.VampireFrogStaff, new WeaponParams(1, 1, 10, 0, faithScalingGrade: ScalingGrade.D) },
            {ItemID.VampireKnives, new WeaponParams(25, 10, 0, 0, ScalingGrade.A, ScalingGrade.D, saturation: 50) },
            {ItemID.VenomStaff, new WeaponParams(2, 2, 24, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.VenusMagnum, new WeaponParams(2, 27, 0, 0, dexterityScalingGrade: ScalingGrade.C) },
            {ItemID.Vilethorn, new WeaponParams(1, 1, 8, 0, intelligenceScalingGrade: ScalingGrade.E) },
            {ItemID.FieryGreatsword, new WeaponParams(12, 3, 0, 0, ScalingGrade.C, ScalingGrade.D) },
            {ItemID.WaffleIron, new WeaponParams(15, 10, 0, 0, ScalingGrade.S, ScalingGrade.D) },
            {ItemID.WandofFrosting, new WeaponParams() },
            {ItemID.WandofSparking, new WeaponParams(1, 1, 5, 0) },
            {ItemID.WarAxeoftheNight, new WeaponParams(10, 2, 0, 0, ScalingGrade.E) },
            {ItemID.WaspGun, new WeaponParams(2, 2, 28, 0, intelligenceScalingGrade: ScalingGrade.C) },
            {ItemID.WaterBolt, new WeaponParams(2, 2, 12, 0, intelligenceScalingGrade: ScalingGrade.D) },
            {ItemID.WeatherPain, new WeaponParams(1, 1, 10, 0, intelligenceScalingGrade: ScalingGrade.E) },
            {ItemID.WetBomb, new WeaponParams(2, 5, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.WoodenBoomerang, new WeaponParams() },
            {ItemID.WoodenBow, new WeaponParams() },
            {ItemID.WoodenHammer, new WeaponParams() },
            {ItemID.WoodenSword, new WeaponParams() },
            {ItemID.WoodYoyo, new WeaponParams() },
            {ItemID.XenoStaff, new WeaponParams(2, 2, 0, 30, faithScalingGrade: ScalingGrade.B) },
            {ItemID.Xenopopper, new WeaponParams(2, 30, 0, 0, dexterityScalingGrade: ScalingGrade.B) },
            {ItemID.Yelets, new WeaponParams(14, 14, 0, 0, ScalingGrade.C, ScalingGrade.C, saturation: 85) },
            {ItemID.Zenith, new WeaponParams(42, 10, 0, 0, ScalingGrade.A, ScalingGrade.C, saturation: 40) },
            {ItemID.ZombieArm, new WeaponParams(6, 4, 0, 0, ScalingGrade.E) }
        };

        public static Dictionary<int, WeaponParams> AllWeaponsParams
        {
            get { return allWeaponsParams; }
        }

        public static string ScalingGradeToString(ScalingGrade level) => level == ScalingGrade.None ? "-" : level.ToString();

        public static float GetScalingGradeModifier(ScalingGrade level)
        {
            return level switch
            {
                ScalingGrade.S => 0.85f,
                ScalingGrade.A => 0.65f,
                ScalingGrade.B => 0.45f,
                ScalingGrade.C => 0.35f,
                ScalingGrade.D => 0.25f,
                ScalingGrade.E => 0.15f,
                _ => 0f,
            };
        }

        public static DamageBonuses GetBonusDamage(Item item)
        {
            WeaponParams wp;
            if (!AllWeaponsParams.TryGetValue(item.type, out wp))
                return new();

            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();

            int bonusDamageByStrength = (int)(GetScalingGradeModifier(wp.StrengthScalingGrade) * StatFormulas.GetPotentialByStrength(dsPlayer.dsStrength) * wp.Saturation);
            int bonusDamageByDexterity = (int)(GetScalingGradeModifier(wp.DexterityScalingGrade) * StatFormulas.GetPotentialByDexterity(dsPlayer.dsDexterity) * wp.Saturation);
            int bonusDamageByIntelligence = (int)(GetScalingGradeModifier(wp.IntelligenceScalingGrade) * StatFormulas.GetPotentialByIntelligence(dsPlayer.dsIntelligence) * wp.Saturation);
            int bonusDamageByFaith = (int)(GetScalingGradeModifier(wp.FaithScalingGrade) * StatFormulas.GetPotentialByFaith(dsPlayer.dsFaith) * wp.Saturation);
            int totalBonusDamage = bonusDamageByStrength + bonusDamageByDexterity + bonusDamageByIntelligence + bonusDamageByFaith;
            return new(totalBonusDamage, bonusDamageByStrength, bonusDamageByDexterity, bonusDamageByIntelligence, bonusDamageByFaith);
        }
    }
}