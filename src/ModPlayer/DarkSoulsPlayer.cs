using DarkSouls.Config;
using DarkSouls.Items.Accessories;
using DarkSouls.UI;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace DarkSouls
{
    public class DarkSoulsPlayer : ModPlayer
    {
        public string covenant = "None";
        public int dsSouls = 0;
        public int dsVitality = 1;
        public int dsAttunement = 1;
        public int dsEndurance = 1;
        public int dsStrength = 1;
        public int dsDexterity = 1;
        public int dsResistance = 1;
        public int dsIntelligence = 1;
        public int dsFaith = 1;
        public int dsHumanity = 0;

        public const float DEFAULT_MAX_STAMINA = 60;
        public float maxStamina = DEFAULT_MAX_STAMINA;
        public float currentStamina = DEFAULT_MAX_STAMINA;
        public float usedStamina = 0;
        private float staminaRegenRate = 0.6f;
        private int staminaRegenDelay = 0;
        private int maxStaminaRegenDelay = 90;
        private int maxUsedStaminaRegenDelay = 65;
        private int usedStaminaRegenDelay = 0;
        private float usedStaminaRegenRate = 4;

        public int usedMana = 0;
        public int usedManaRegenRate = 4;
        public int usedManaRegenDelay = 0;
        public int maxUsedManaRegenDelay = 65;

        public int usedHP = 0;
        public int usedLifeRegenRate = 4;
        public int usedLifeRegenDelay = 0;
        public int maxUsedLifeRegenDelay = 65;

        private int pendingSouls = 0;
        private float soulTimer = 0f;
        private float soulDelay = 1.25f;

        private float damageSoundTimer = 0f;
        private float damageSoundDelay = 0.3f;

        private int lastSoundStyleDamageIndex = -1;
        private SlotId lastDamageSoundSlotId;

        private Dictionary<int, bool> newDebuffs = new(); // stores the state of the buffs that were first applied to the player

        // Dash
        private int dashRight = 2;
        private int dashLeft = 3;
        private int dashDir = 0;
        private int dashDelay = 0;
        private int dashTimer = 0;
        private int dashCooldown = 40;
        private const int dashDuration = 20;
        private const float dashVelocity = 10f;

        // Accesories
        public bool CloranthyRingEffect = false;

        public int PlayerLevel => dsVitality + dsAttunement + dsEndurance + dsStrength +
            dsDexterity + dsResistance + dsIntelligence + dsFaith;

        public override void ResetEffects()
        {
            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[dashRight] < 15 && Player.doubleTapCardinalTimer[dashLeft] == 0)
                dashDir = dashRight;
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[dashLeft] < 15 && Player.doubleTapCardinalTimer[dashRight] == 0)
                dashDir = dashLeft;
            else
                dashDir = 0;

            CloranthyRingEffect = false;
        }

        public override void PreUpdate()
        {
            // disabling all dashes in Terraria
            if (!ServerConfig.Instance.DisableVanillaDashLock)
            {
                Player.dashDelay = 999;
                Player.dashType = 0;
            }
        }

        public override void PreUpdateMovement()
        {
            // Dash with invincibility frames
            if (dashDir != 0 && dashDelay == 0 && currentStamina >= 30f)
            {
                ConsumeStamina(30f);
                if (dashDir == dashRight && Player.velocity.X < dashVelocity)
                    Player.velocity.X = dashVelocity;
                else if (dashDir == dashLeft && Player.velocity.X > -dashVelocity)
                    Player.velocity.X = -dashVelocity;
                dashDelay = dashCooldown;
                dashTimer = dashDuration;
                Player.immune = true;
                Player.immuneTime = GetInvincibilityFramesByResistance(dsResistance);
            }
            
            if (dashDelay > 0)
                dashDelay--;

            if (dashTimer > 0)
            {
                Player.eocDash = dashTimer;
                Player.armorEffectDrawShadowEOCShield = true;
                dashTimer--;
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            if (DarkSouls.ToggleDarkSoulsStatsUIKey.JustPressed)
                ModContent.GetInstance<DarkSoulsStatsUISystem>().ToggleUI();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["dsSouls"] = dsSouls;
            tag["dsVitality"] = dsVitality;
            tag["dsAttunement"] = dsAttunement;
            tag["dsEndurance"] = dsEndurance;
            tag["dsStrength"] = dsStrength;
            tag["dsDexterity"] = dsDexterity;
            tag["dsResistance"] = dsResistance;
            tag["dsIntelligence"] = dsIntelligence;
            tag["dsFaith"] = dsFaith;
            tag["dsHumanity"] = dsHumanity;
            tag["covenant"] = covenant;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("dsSouls"))
                dsSouls = tag.GetInt("dsSouls");
            if (tag.ContainsKey("dsVitality"))
                dsVitality = tag.GetInt("dsVitality");
            if (tag.ContainsKey("dsAttunement"))
                dsAttunement = tag.GetInt("dsAttunement");
            if (tag.ContainsKey("dsEndurance"))
                dsEndurance = tag.GetInt("dsEndurance");
            if (tag.ContainsKey("dsStrength"))
                dsStrength = tag.GetInt("dsStrength");
            if (tag.ContainsKey("dsDexterity"))
                dsDexterity = tag.GetInt("dsDexterity");
            if (tag.ContainsKey("dsResistance"))
                dsResistance = tag.GetInt("dsResistance");
            if (tag.ContainsKey("dsIntelligence"))
                dsIntelligence = tag.GetInt("dsIntelligence");
            if (tag.ContainsKey("dsFaith"))
                dsFaith = tag.GetInt("dsFaith");
            if (tag.ContainsKey("dsHumanity"))
                dsHumanity = tag.GetInt("dsHumanity");
            if (tag.ContainsKey("covenant"))
                covenant = tag.GetString("covenant");

            maxStamina = GetStaminaByEndurance(dsEndurance);
            currentStamina = maxStamina;
        }

        public override void OnEnterWorld()
        {
            DarkSoulsStatsUISystem dsStatsUISystem = ModContent.GetInstance<DarkSoulsStatsUISystem>();
            dsStatsUISystem.dsStatsUI.Activate();
            dsStatsUISystem.ToggleUI();
            dsStatsUISystem.ToggleUI();
            dsStatsUISystem.HideUI();

            usedHP = Player.statLifeMax2;
        }

        public override void OnRespawn()
        {
            usedHP = Player.statLifeMax2;
        }

        public override void PostUpdate()
        {
            Player.ConsumedLifeCrystals = Math.Clamp(GetHPByVitality(dsVitality) / 20, 0, Player.LifeCrystalMax);
            Player.ConsumedManaCrystals = Math.Clamp(GetManaByAttunement(dsAttunement) / 20, 0, Player.ManaCrystalMax);

            maxStamina = GetStaminaByEndurance(dsEndurance);

            if (pendingSouls > 0)
            {
                soulTimer += 1f / 60f;
                if (soulTimer >= soulDelay)
                {
                    CombatText.NewText(Player.getRect(), Color.WhiteSmoke, "+" + pendingSouls.ToString());
                    SoundEngine.PlaySound(DarkSouls.dsSoulSuck, Player.position);
                    pendingSouls = 0;
                    soulTimer = 0;
                }
            }
            if (damageSoundTimer < damageSoundDelay)
                damageSoundTimer += 1f / 60f;

            // Life
            if (usedLifeRegenDelay > 0)
                usedLifeRegenDelay--;
            else
                usedHP = Math.Max(usedHP - usedLifeRegenRate, Player.statLife);
            if (Player.statLife > usedHP)
                usedHP = Player.statLife;

            // Used Mana
            if (usedManaRegenDelay > 0)
                usedManaRegenDelay--;
            else
                usedMana = Math.Max(usedMana - usedManaRegenRate, Player.statMana);
            if (Player.statMana > usedMana)
                usedMana = Player.statMana;

            // Used Stamina
            if (usedStaminaRegenDelay > 0)
                usedStaminaRegenDelay--;
            else
                usedStamina = Math.Max(usedStamina - usedStaminaRegenRate, currentStamina);
            if (currentStamina > usedStamina)
                usedStamina = currentStamina;

            // Stamina
            if (staminaRegenDelay > 0)
            {
                staminaRegenDelay--;
                return;
            }
            float newStaminaRegenRate = staminaRegenRate;
            if (CloranthyRingEffect)
                newStaminaRegenRate *= (1f + CloranthyRing.StaminaRegenRateBonus);
            if (currentStamina < maxStamina)
                currentStamina += newStaminaRegenRate;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

        public override void PostUpdateBuffs()
        {
            // this code fragment handles the first apply of debuffs
            // GlobalBuff.ReApply function overload (DSBuffChanges.cs file) is used to handle the reapplication of debuffs
            Dictionary<int, int> currentDebuffs = new();
            List<int> playerBuffs = Player.buffType.ToList();

            foreach (int debuffType in DarkSoulsBuffChanges.terrariaDebuff)
            {
                int debuffIndex = playerBuffs.FindIndex(x => x == debuffType);
                if (debuffIndex == -1)
                {
                    newDebuffs[debuffType] = true;
                    continue;
                }

                int debuffTime = Player.buffTime[debuffIndex];
                if (debuffTime <= 0)
                {
                    newDebuffs[debuffType] = true;
                    continue;
                }

                bool isNew = newDebuffs.GetValueOrDefault(debuffType, false);

                if (isNew)
                {
                    Player.buffTime[debuffIndex] = (int)(Player.buffTime[debuffIndex] * (1f - GetDebuffsResistanceByResistance(dsResistance)));
                    newDebuffs[debuffType] = false;
                }
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= (float)(1f - GetDefenseByResistance(dsResistance));
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= (float)(1f - GetDefenseByResistance(dsResistance));
        }

        public override void OnConsumeMana(Item item, int manaConsumed)
        {
            usedManaRegenDelay = maxUsedManaRegenDelay;
            usedMana = Math.Max(usedMana, Player.statMana - manaConsumed);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.statLife - info.Damage <= 0) // disables this function if you take lethal damage, the sound will be produced by the Kill function
                return;
            if (damageSoundTimer < damageSoundDelay)
                return;
            ActiveSound activeSound;
            if (SoundEngine.TryGetActiveSound(lastDamageSoundSlotId, out activeSound))
                activeSound.Volume -= 0.2f;
            SoundStyle sound = DarkSouls.GetRandomDamageSounds(out lastSoundStyleDamageIndex, Player.Male, lastSoundStyleDamageIndex);
            lastDamageSoundSlotId = SoundEngine.PlaySound(sound, Player.Center);
            damageSoundTimer = 0f;

            usedLifeRegenDelay = maxUsedLifeRegenDelay;
            usedManaRegenDelay = maxUsedManaRegenDelay;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            bool diedFromFall = damageSource.SourceOtherIndex == 0; // fall damage
            ActiveSound activeSound;
            if (SoundEngine.TryGetActiveSound(lastDamageSoundSlotId, out activeSound))
                activeSound.Volume -= 0.4f;
            if (Player.Male)
            {
                if (diedFromFall)
                    SoundEngine.PlaySound(DarkSouls.dsMaleFallinDeadSound, Player.Center);
                else
                    SoundEngine.PlaySound(DarkSouls.dsMaleDeadSound, Player.Center);
            }
            else
                SoundEngine.PlaySound(DarkSouls.dsFemaleDeadSound, Player.Center);
            ModContent.GetInstance<DarkSoulsYouDiedUISystem>().ShowUI();
            SoundEngine.PlaySound(DarkSouls.dsThruDeath);
            currentStamina = maxStamina;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();

            packet.Write((byte)DarkSouls.NetMessageTypes.SyncVitality);
            packet.Write((byte)Player.whoAmI);
            packet.Write(dsVitality);
            packet.Send(toWho, fromWho);
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            base.CopyClientState(targetCopy);
            DarkSoulsPlayer dsPlayer = targetCopy as DarkSoulsPlayer;
            dsPlayer.dsVitality = dsVitality;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            DarkSoulsPlayer dsPlayer = clientPlayer as DarkSoulsPlayer;
            if (dsPlayer.dsVitality != dsVitality)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)DarkSouls.NetMessageTypes.SyncVitality);
                packet.Write((byte)Player.whoAmI);
                packet.Write(dsVitality);
                packet.Send();
            }
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            // simulation of the use of health and mana crystals, for the correct behavior conditions various events
            int totalHealthBonus = GetHPByVitality(dsVitality);
            int consumedLifeCrystals = Math.Clamp(totalHealthBonus / 20, 0, Player.LifeCrystalMax);
            int healthBonus = totalHealthBonus - consumedLifeCrystals * 20;

            int totalManaBonus = GetManaByAttunement(dsAttunement);
            int consumedManaCrystals = Math.Clamp(totalManaBonus / 20, 0, Player.ManaCrystalMax);
            int manaBonus = totalManaBonus - consumedManaCrystals * 20;

            Player.ConsumedLifeCrystals = consumedLifeCrystals;
            Player.ConsumedManaCrystals = consumedManaCrystals;

            health = StatModifier.Default;
            health.Base = healthBonus;

            mana = StatModifier.Default;
            mana.Base = manaBonus;
        }

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
            int frames = dashDuration / 2;
            frames += Math.Max(0, Math.Min(resistance - 1, 31)) / (30 / ((dashDuration - frames) / 2)); // +5
            frames += Math.Max(0, Math.Min(resistance - 32, 67) / (67 / (dashDuration - frames)));
            return frames;
        }

        public static float GetStaminaByEndurance(int endurance)
        {
            float stamina = DEFAULT_MAX_STAMINA;
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


        public static int GetReqSoulsByLevel(int level)
        {
            int multiplier = ServerConfig.Instance.LevelUpCostMultiplierPercent;
            int reqSouls = 0;

            if (level >= 35) // 35+
                reqSouls = (int)(0.02 * Math.Pow(level, 3) + 3.05 * Math.Pow(level, 2) + 90 * level - 6500);
            else if (level > 0 && level < 35) // 1 - 35
                reqSouls = (int)(500 * Math.Pow(1.025, level - 1)); // 2.5% increase per level

            reqSouls = (int)(reqSouls * multiplier / 100f);

            return reqSouls;
        }

        public void AddSouls(int amount, bool render = true)
        {
            if (amount <= 0)
                return;

            int multiplier = ServerConfig.Instance.SoulsGainMultiplierPercent;
            amount = (int)(amount * multiplier / 100f);

            dsSouls += amount;
            if (render)
            {
                pendingSouls += amount;
                soulTimer = 0f; // reset timer for each NPC kill
            }
        }

        public bool ConsumeStamina(float amount)
        {
            if (currentStamina >= amount)
            {
                currentStamina -= amount;
                if (CloranthyRingEffect)
                    staminaRegenDelay = (int)(maxStaminaRegenDelay * (1f - CloranthyRing.StaminaRegenDelayReductionBonus));
                else
                    staminaRegenDelay = maxStaminaRegenDelay;
                usedStaminaRegenDelay = maxUsedStaminaRegenDelay;
                return true;
            }
            return false;
        }
    }
}