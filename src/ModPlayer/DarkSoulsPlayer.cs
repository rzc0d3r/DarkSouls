using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.DataStructures;

using ReLogic.Utilities;

using DarkSouls.UI;
using DarkSouls.Core;
using DarkSouls.Utils;
using DarkSouls.Config;
using DarkSouls.Projectiles;
using DarkSouls.DataStructures;
using DarkSouls.Items.Accessories;

namespace DarkSouls
{
    public class DarkSoulsPlayer : ModPlayer
    {
        #region Player Stats
        public string covenant = Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.CovenantNone");

        private long souls;
        public long dsSouls
        {
            get => souls;
            set => souls = Math.Max(0, value);
        }

        private int vitality = 1;
        public int dsVitality
        {
            get => vitality;
            set => vitality = Math.Clamp(value, 1, 99);
        }

        private int attunement = 1;
        public int dsAttunement
        {
            get => attunement;
            set => attunement = Math.Clamp(value, 1, 99);
        }

        private int endurance = 1;
        public int dsEndurance
        {
            get => endurance;
            set => endurance = Math.Clamp(value, 1, 99);
        }

        private int strength = 1;
        public int dsStrength
        {
            get => strength;
            set => strength = Math.Clamp(value, 1, 99);
        }

        private int dexterity = 1;
        public int dsDexterity
        {
            get => dexterity;
            set => dexterity = Math.Clamp(value, 1, 99);
        }

        private int resistance = 1;
        public int dsResistance
        {
            get => resistance;
            set => resistance = Math.Clamp(value, 1, 99);
        }

        private int intelligence = 1;
        public int dsIntelligence
        {
            get => intelligence;
            set => intelligence = Math.Clamp(value, 1, 99);
        }

        private int faith = 1;
        public int dsFaith
        {
            get => faith;
            set => faith = Math.Clamp(value, 1, 99);
        }

        private int humanity;
        public int dsHumanity
        {
            get => humanity;
            set => humanity = Math.Clamp(value, 0, 99);
        }
        #endregion

        #region Stamina Variables
        public const float DEFAULT_MAX_STAMINA = 60;
        public float maxStamina = DEFAULT_MAX_STAMINA;
        public float currentStamina = DEFAULT_MAX_STAMINA;

        private float staminaRegenRate = 0.6f;
        private int staminaRegenDelay = 0;
        #endregion

        #region Resource Bars: Stamina Variables
        public float usedStamina = 0;
        private int maxStaminaRegenDelay = 90;
        private int maxUsedStaminaRegenDelay = 65;
        private int usedStaminaRegenDelay = 0;
        private float usedStaminaRegenRate = 4;
        #endregion

        #region Resource Bars: Mana Variables
        public int usedMana = 0;
        public int usedManaRegenRate = 4;
        public int usedManaRegenDelay = 0;
        public int maxUsedManaRegenDelay = 65;
        #endregion

        #region Resource Bars: HP Variables
        public int usedHP = 0;
        public int usedLifeRegenRate = 4;
        public int usedLifeRegenDelay = 0;
        public int maxUsedLifeRegenDelay = 65;
        #endregion

        #region Souls Timers
        private long pendingSouls = 0;
        private float soulTimer = 0f;
        private float soulDelay = 1.25f;
        #endregion

        #region Damage Sound Variables
        private float damageSoundTimer = 0f;
        private float damageSoundDelay = 0.3f;

        private int lastSoundStyleDamageIndex = -1;
        private SlotId lastDamageSoundSlotId;
        #endregion

        #region Dash Variables
        public const int dashDown = 0;
        public const int dashUp = 1;
        public const int dashRight = 2;
        public const int dashLeft = 3;
        private int dashDir = -1;

        private int dashDelay = 0;
        private int dashTimer = 0;
        private int dashCooldown = 40;
        public static int dashDuration { get; protected set; } = 20;
        private const float dashVelocity = 10f;
        private bool customDashKey = false;
        #endregion

        #region Buffs Variables
        private Dictionary<int, bool> newDebuffs = new(); // stores the state of the buffs that were first applied to the player
        #endregion

        #region Bloodstain Variables
        public List<Bloodstain> bloodstains = new();
        public int currentBloodstainProjectile = -1;
        #endregion

        #region Accessories Effects (flags)
        public bool CloranthyRingEffect = false;
        #endregion

        public int PlayerLevel => dsVitality + dsAttunement + dsEndurance + dsStrength +
            dsDexterity + dsResistance + dsIntelligence + dsFaith;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (DarkSouls.ToggleDarkSoulsStatsUIKey.JustPressed)
                ModContent.GetInstance<DarkSoulsStatsUISystem>().ToggleUI();
            else if (DarkSouls.TouchBloodstainKey.JustPressed)
            {
                if (IsBloodstainReachable())
                    TouchBloodstain();
                else
                    Main.NewText(Language.GetTextValue("Mods.DarkSouls.BloodstainErrorMessage"), Color.DarkRed);
            }
        }

        public override void ResetEffects()
        {
            #region Dash Handler
            if (Main.myPlayer == Player.whoAmI)
            {
                customDashKey = DarkSouls.DashKey.GetAssignedKeys().Count > 0 && DarkSouls.DashKey.GetAssignedKeys()[0] != "Double-tap A or D";
                if (customDashKey)
                {
                    if (DarkSouls.DashKey.JustPressed)
                    {
                        if (Player.controlDown)
                            dashDir = dashDown;
                        else if (Player.controlUp)
                            dashDir = dashUp;
                        else
                            dashDir = Player.direction == 1 ? dashRight : dashLeft;
                    }
                    else
                        dashDir = -1;
                }
                else
                {
                    if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[dashDown] < 15)
                        dashDir = dashDown;
                    else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[dashUp] < 15)
                        dashDir = dashUp;
                    else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[dashRight] < 15 && Player.doubleTapCardinalTimer[dashLeft] == 0)
                        dashDir = dashRight;
                    else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[dashLeft] < 15 && Player.doubleTapCardinalTimer[dashRight] == 0)
                        dashDir = dashLeft;
                    else
                        dashDir = -1;
                }
            }
            #endregion
            
            #region Reset Accessories Flags
            CloranthyRingEffect = false;
            #endregion
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
            #region Dash with invincibility frames
            if (!Player.mount.Active && dashDir != -1 && dashDelay == 0 && currentStamina >= 30f && !ClientConfig.Instance.DisableDash)
            {
                Vector2 newVelocity = Player.velocity;

                switch (dashDir)
                {
                    
                    //case dashUp when Player.velocity.Y > -dashVelocity:
                    //case dashDown when Player.velocity.Y < dashVelocity:
                    //{
                    //    float dashDirection = dashDir == dashDown ? 1 : -1.3f;
                    //    newVelocity.Y = dashDirection * dashVelocity;
                    //    break;
                    //}
                    case dashLeft when Player.velocity.X > -dashVelocity:
                    case dashRight when Player.velocity.X < dashVelocity:
                    {
                        float dashDirection = dashDir == dashRight ? 1 : -1;
                        newVelocity.X = dashDirection * dashVelocity;
                        break;
                    }
                    default:
                        return;
                }

                dashDelay = dashCooldown;
                dashTimer = dashDuration;
                Player.velocity = newVelocity;

                ConsumeStamina(30f);
                Player.immune = true;
                Player.immuneTime = StatFormulas.GetInvincibilityFramesByResistance(dsResistance);
            }

            if (dashDelay > 0)
                dashDelay--;

            if (dashTimer > 0)
            {
                Player.eocDash = dashTimer;
                Player.armorEffectDrawShadowEOCShield = true;
                dashTimer--;
            }
            #endregion
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

            if (covenant != "" && covenant != Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.CovenantNone"))
                tag["covenant"] = covenant;
            if (bloodstains.Count > 0)
                tag["bloodstains"] = bloodstains.Select(x => x.ToTag()).ToList();
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("dsSouls"))
            {
                object rawSouls = tag["dsSouls"];
                if (rawSouls is long l)
                    dsSouls = l;
                else if (rawSouls is int i)
                    dsSouls = i;
                else
                    dsSouls = 0;
            }
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
            {
                covenant = tag.GetString("covenant");
                if (covenant == "None")
                    covenant = Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.CovenantNone");
            }
            else
                covenant = Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.CovenantNone");

            if (tag.ContainsKey("bloodstains"))
                bloodstains = tag.GetList<TagCompound>("bloodstains").Select(Bloodstain.FromTag).ToList();

            maxStamina = StatFormulas.GetStaminaByEndurance(dsEndurance);
            currentStamina = maxStamina;
        }

        public override void OnEnterWorld()
        {
            var lostSoul = bloodstains.FirstOrDefault(s => s.worldGUID == Main.ActiveWorldFileData.UniqueId.ToString());
            if (lostSoul != null && Main.myPlayer == Player.whoAmI)
            {
                currentBloodstainProjectile = Projectile.NewProjectile(
                    Player.GetSource_Death(),
                    lostSoul.position,
                    Vector2.Zero,
                    ModContent.ProjectileType<BloodstainProjectile>(),
                    0,
                    0f,
                    Player.whoAmI);
            }

            DarkSoulsStatsUISystem dsStatsUISystem = ModContent.GetInstance<DarkSoulsStatsUISystem>();
            dsStatsUISystem.ShowUI();
            dsStatsUISystem.HideUI();

            usedHP = Player.statLifeMax2;
        }

        public override void OnRespawn() => usedHP = Player.statLifeMax2;

        public override void PostUpdate()
        {
            Player.ConsumedLifeCrystals = Math.Clamp(StatFormulas.GetHPByVitality(dsVitality) / 20, 0, Player.LifeCrystalMax);
            Player.ConsumedLifeFruit = Math.Clamp((StatFormulas.GetHPByVitality(dsVitality) - 400) / 5, 0, Player.LifeFruitMax);
            Player.ConsumedManaCrystals = Math.Clamp(StatFormulas.GetManaByAttunement(dsAttunement) / 20, 0, Player.ManaCrystalMax);

            // Handling click on the location of bloodstain
            if (currentBloodstainProjectile != -1 && Main.mouseRight && Main.mouseRightRelease && Main.netMode != NetmodeID.Server)
            {
                if (IsBloodstainReachable())
                {
                    Projectile proj = Main.projectile[currentBloodstainProjectile];
                    Vector2 mouseWorld = Main.MouseWorld;
                    if (proj.Hitbox.Contains(mouseWorld.ToPoint()))
                        TouchBloodstain();
                }
            }

            maxStamina = StatFormulas.GetStaminaByEndurance(dsEndurance);

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
            // This code fragment handles the first apply of debuffs
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
                    Player.buffTime[debuffIndex] = (int)(Player.buffTime[debuffIndex] * (1f - StatFormulas.GetDebuffsResistanceByResistance(dsResistance)));
                    newDebuffs[debuffType] = false;
                }
            }
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            // simulation of the used of health, mana crystals and life fruits, for the correct behavior conditions various events
            int totalHealthBonus = StatFormulas.GetHPByVitality(dsVitality);
            int consumedLifeCrystals = Math.Clamp(totalHealthBonus / 20, 0, Player.LifeCrystalMax);
            int consumedLifeFruit = Math.Clamp((StatFormulas.GetHPByVitality(dsVitality) - 400) / 5, 0, Player.LifeFruitMax);
            int healthBonus = totalHealthBonus - consumedLifeCrystals * 20;

            int totalManaBonus = StatFormulas.GetManaByAttunement(dsAttunement);
            int consumedManaCrystals = Math.Clamp(totalManaBonus / 20, 0, Player.ManaCrystalMax);
            int manaBonus = totalManaBonus - consumedManaCrystals * 20;

            Player.ConsumedLifeCrystals = consumedLifeCrystals;
            Player.ConsumedManaCrystals = consumedManaCrystals;
            Player.ConsumedLifeFruit = consumedLifeFruit;

            health = StatModifier.Default;
            health.Base = healthBonus;

            mana = StatModifier.Default;
            mana.Base = manaBonus;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= (float)(1f - StatFormulas.GetDefenseByResistance(dsResistance));
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= (float)(1f - StatFormulas.GetDefenseByResistance(dsResistance));
        }

        public override void ModifyLuck(ref float luck)
        {
            luck += Math.Clamp(dsHumanity / 100f, 0f, 0.5f);
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

            if (!Main.dedServ && !ClientConfig.Instance.DisableOverrideHurtSounds)
            {
                ActiveSound activeSound;
                if (SoundEngine.TryGetActiveSound(lastDamageSoundSlotId, out activeSound))
                    activeSound.Volume -= 0.2f;
                SoundStyle sound = SoundUtils.GetRandomDamageSound(out lastSoundStyleDamageIndex, Player.Male, lastSoundStyleDamageIndex);
                lastDamageSoundSlotId = SoundEngine.PlaySound(sound);
            }
            damageSoundTimer = 0f;

            usedLifeRegenDelay = maxUsedLifeRegenDelay;
            usedManaRegenDelay = maxUsedManaRegenDelay;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                // Player death handling
                if (!ClientConfig.Instance.DisableOverrideHurtSounds)
                {
                    bool diedFromFall = damageSource.SourceOtherIndex == 0; // fall damage
                    ActiveSound activeSound;
                    if (SoundEngine.TryGetActiveSound(lastDamageSoundSlotId, out activeSound))
                        activeSound.Volume -= 0.4f;
                    if (Player.Male)
                    {
                        if (diedFromFall)
                            SoundEngine.PlaySound(DarkSouls.dsMaleFallinDeadSound);
                        else
                            SoundEngine.PlaySound(DarkSouls.dsMaleDeadSound);
                    }
                    else
                        SoundEngine.PlaySound(DarkSouls.dsFemaleDeadSound);
                }

                if (!ClientConfig.Instance.DisableDeathScreen)
                {
                    ModContent.GetInstance<DarkSoulsYouDiedUISystem>().ShowUI();
                    SoundEngine.PlaySound(DarkSouls.dsThruDeath);
                }

                currentStamina = maxStamina;

                // Creating a bloodstain (local only)
                if (currentBloodstainProjectile != -1 && Main.projectile[currentBloodstainProjectile].active && Main.projectile[currentBloodstainProjectile].type == ModContent.ProjectileType<BloodstainProjectile>())
                {
                    Main.projectile[currentBloodstainProjectile].Kill();
                    currentBloodstainProjectile = -1;
                }

                bloodstains.RemoveAll(x => x.worldGUID == Main.ActiveWorldFileData.UniqueId.ToString());

                if (dsSouls > 0 || dsHumanity > 0)
                {
                    bloodstains.Add(new(Player.Center, dsHumanity, dsSouls, Player));
                    dsSouls = 0;
                    dsHumanity = 0;
                    currentBloodstainProjectile = Projectile.NewProjectile(
                        Player.GetSource_Death(),
                        Player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<BloodstainProjectile>(),
                        0,
                        0f,
                        Player.whoAmI);
                }
            }
        }

        #region Netcode
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            var vitalityPacket = Mod.GetPacket();
            vitalityPacket.Write((byte)DarkSouls.NetMessageTypes.SyncVitality);
            vitalityPacket.Write((byte)Player.whoAmI);
            vitalityPacket.Write(dsVitality);
            vitalityPacket.Send(toWho, fromWho);
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
        #endregion

        #region Custom Functions
        public void AddSouls(long amount, bool render = true)
        {
            if (amount <= 0)
                return;

            float multiplier = ServerConfig.Instance.SoulsGainMultiplierPercent / 100f;
            amount = (long)(amount * multiplier);

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

        public void TouchBloodstain()
        {
            if (currentBloodstainProjectile == -1)
                return;

            Projectile proj = Main.projectile[currentBloodstainProjectile];

            if (!proj.active || proj.type != ModContent.ProjectileType<BloodstainProjectile>())
                return;

            Bloodstain bloodstain = bloodstains.FirstOrDefault(s => s.worldGUID == Main.ActiveWorldFileData.UniqueId.ToString());
            if (bloodstain != null)
            {
                Main.NewText(Language.GetText("Mods.DarkSouls.BloodstainMessage").WithFormatArgs(bloodstain.souls, bloodstain.humanity).Value, Color.Cyan);
                SoundEngine.PlaySound(DarkSouls.dsNewAreaSound);
                proj.Kill();
                currentBloodstainProjectile = -1;
                dsSouls += bloodstain.souls;
                dsHumanity += bloodstain.humanity;
                bloodstains.RemoveAll(x => x.worldGUID == Main.ActiveWorldFileData.UniqueId.ToString());
            }
        }

        public bool IsBloodstainReachable(float reachDistance = 200f)
        {
            if (currentBloodstainProjectile == -1)
                return false;

            Projectile proj = Main.projectile[currentBloodstainProjectile];
            if (!proj.active || proj.type != ModContent.ProjectileType<BloodstainProjectile>())
                return false;

            float distance = Vector2.Distance(Player.Center, proj.Center);
            return distance <= reachDistance;
        }
        #endregion
    }
}
