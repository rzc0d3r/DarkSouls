using System;

using Terraria;
using Terraria.UI;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;

using ReLogic.Content;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Localization;
using DarkSouls.Items.Consumables;
using DarkSouls.Core;

namespace DarkSouls.UI
{
    [Autoload(true, Side = ModSide.Client)]
    public class DarkSoulsStatsUI : UIState
    {
        public bool openedFromBonfire = false;
        public bool respecStats = false;
        private bool firstUpdate = true;
        
        private Stack<long> levelUPCosts = new Stack<long>();

        private UIPanel mainPanel;

        private UIPanel levelUPPanel;
        // Level UP Panel elements (lupp elements)
        private UIText luppTitle;
        private UIText luppDescription;
        private UIImage luppIcon;
        private UIImage luppSeparator;
        private UIImage luppSeparator2;

        private UIImage nameField;
        private UIText playerName;
        private UIText covenantValue;
        private UIText levelValue;
        private UIText soulsValue;
        private UIText reqSoulsValue;

        private UIText vitalityName;
        private UIText vitalityValue;

        private UIText intelligenceName;
        private UIText intelligenceValue;

        private UIText enduranceName;
        private UIText enduranceValue;

        private UIText strengthName;
        private UIText strengthValue;

        private UIText dexterityName;
        private UIText dexterityValue;

        private UIText resistanceName;
        private UIText resistanceValue;

        private UIText attunementName;
        private UIText attunementValue;

        private UIText faithName;
        private UIText faithValue;

        private UIText humanityValue;

        private UIImage acceptButton;

        private UIPanel statPanel;

        private UIText hpValue;
        private UIText manaValue;
        private UIText defenseValue;
        private UIText debuffsResistanceValue;
        private UIText staminaValue;
        private UIText invincibilityFramesValue;

        private Texture2D statusTexture;
        private Texture2D levelUPTexture;

        private Texture2D shortSeparatorTexture;
        private Texture2D longSeparatorTexture;

        private Texture2D acceptButtonActiveTexture;
        private Texture2D acceptButtonInactiveTexture;
        private bool acceptButtonIsActive = false;

        private float STAT_LEFT_PIXELS;
        private float CurrentTopPixels = 0;

        private bool updateSoulsValue = true;

        private void CreateStatUIElement(ref UIText statNameUIElement, ref UIText statValueUIElement, Texture2D iconTexture, string statName, string statValue, UIPanel srcPanel, UIElement prevUIElement)
        {
            UIImage statIcon = new UIImage(iconTexture);
            statIcon.Left.Set(STAT_LEFT_PIXELS, 0f);
            statIcon.Top.Set(CurrentTopPixels + 5f, 0f);
            srcPanel.Append(statIcon);

            float textHeight = FontAssets.MouseText.Value.MeasureString(statName).Y;

            statNameUIElement = new UIText(statName);
            statNameUIElement.Left.Set(STAT_LEFT_PIXELS + statIcon.Width.Pixels + 3f, 0f);
            statNameUIElement.Top.Set(statIcon.Top.Pixels - (float)Math.Floor((statIcon.Height.Pixels - textHeight) / 2.0f) + 1, 0f);
            srcPanel.Append(statNameUIElement);

            statValueUIElement = new UIText(statValue);
            statValueUIElement.Left.Set(STAT_LEFT_PIXELS - 10f, 0f);
            statValueUIElement.Top = statNameUIElement.Top;
            statValueUIElement.HAlign = 1f;
            srcPanel.Append(statValueUIElement);

            CurrentTopPixels += 5f + statIcon.Height.Pixels;
        }

        private void CreateStatUIElement(ref UIText statValueUIElement, Texture2D iconTexture, string statName, string statValue, UIPanel srcPanel, UIElement prevUIElement)
        {
            UIText statNameUIElement = default;
            CreateStatUIElement(ref statNameUIElement, ref statValueUIElement, iconTexture, statName, statValue, srcPanel, prevUIElement); 
        }

        private void InitializeMainPanel()
        {
            mainPanel = new();
            mainPanel.Width.Set(765f, 0f);
            mainPanel.Height.Set(535f, 0f);
            mainPanel.PaddingBottom = 0f;
            mainPanel.PaddingTop = 0f;
            mainPanel.PaddingLeft = 0f;
            mainPanel.PaddingRight = 0f;
            mainPanel.BorderColor = Color.Transparent;
            mainPanel.BackgroundColor = Color.Transparent;
            mainPanel.VAlign = 0.5f;
            mainPanel.HAlign = 0.5f;
            Append(mainPanel);
        }

        private void InitializeLevelUPPanel()
        {
            statusTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/StatusIconSmall", AssetRequestMode.ImmediateLoad).Value;
            levelUPTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/LevelUPIconSmall", AssetRequestMode.ImmediateLoad).Value;

            shortSeparatorTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/SeparatorSmall", AssetRequestMode.ImmediateLoad).Value;
            longSeparatorTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/SeparatorStats", AssetRequestMode.ImmediateLoad).Value;

            acceptButtonActiveTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/AcceptButtonActive", AssetRequestMode.ImmediateLoad).Value;
            acceptButtonInactiveTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/AcceptButtonInactive", AssetRequestMode.ImmediateLoad).Value;

            levelUPPanel = new();
            levelUPPanel.Width.Set(310f, 0f);
            levelUPPanel.Height.Set(535f, 0f);
            levelUPPanel.BackgroundColor = new Color(16, 16, 10, 240);
            mainPanel.Append(levelUPPanel);

            luppIcon = new(statusTexture);
            levelUPPanel.Append(luppIcon);

            luppTitle = new(Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Status"), 1.3f);
            luppTitle.Left.Set(statusTexture.Width + 8f, 0f);
            levelUPPanel.Append(luppTitle);

            luppSeparator = new(shortSeparatorTexture);
            luppSeparator.Left.Set(luppTitle.Left.Pixels - 5f, 0f);
            luppSeparator.Top.Set(25f, 0f);
            levelUPPanel.Append(luppSeparator);

            luppDescription = new(Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.CheckStatus"), 0.9f);
            luppDescription.Left = luppTitle.Left;
            luppDescription.Top.Set(luppSeparator.Top.Pixels + 10f, 0f);
            levelUPPanel.Append(luppDescription);

            luppSeparator2 = new(shortSeparatorTexture);
            luppSeparator2.Left = luppSeparator.Left;
            luppSeparator2.Top.Set(luppIcon.Height.Pixels - 5f, 0f);
            levelUPPanel.Append(luppSeparator2);

            Texture2D nameFieldTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/NameFieldSmall", AssetRequestMode.ImmediateLoad).Value;
            nameField = new(nameFieldTexture);
            nameField.HAlign = 0.5f;
            nameField.Top.Set(luppIcon.Height.Pixels + 10f, 0f);
            playerName = new(Main.LocalPlayer.name);
            playerName.HAlign = 0.5f;
            playerName.VAlign = 0.5f;
            nameField.Append(playerName);
            levelUPPanel.Append(nameField);

            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            STAT_LEFT_PIXELS = nameField.Left.Pixels + 5f;

            // Covenant
            UIText covenantField = new(Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Covenant"));
            covenantField.Left.Set(STAT_LEFT_PIXELS, 0f);
            CurrentTopPixels = nameField.Top.Pixels + nameField.Height.Pixels + 5f;
            covenantField.Top.Set(CurrentTopPixels, 0f);
            levelUPPanel.Append(covenantField);

            // Covenant Value
            covenantValue = new(dsPlayer.covenant);
            covenantValue.Top.Set(CurrentTopPixels, 0f);
            covenantValue.HAlign = 1f;
            covenantValue.Left.Set(STAT_LEFT_PIXELS - 10f, 0f);
            levelUPPanel.Append(covenantValue);

            CurrentTopPixels += 17f;

            UIImage separatorStats = new(longSeparatorTexture);
            separatorStats.Left.Set(STAT_LEFT_PIXELS, 0f);
            separatorStats.Top.Set(CurrentTopPixels + 5f, 0f);
            levelUPPanel.Append(separatorStats);

            CurrentTopPixels += 5f;

            // - - - Stats - - -
            Texture2D levelTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Level", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref levelValue, levelTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Level"), dsPlayer.PlayerLevel.ToString(), levelUPPanel, separatorStats);

            Texture2D soulsTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Souls", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref soulsValue, soulsTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Souls"), dsPlayer.dsSouls.ToString(), levelUPPanel, levelValue);
            CreateStatUIElement(ref reqSoulsValue, soulsTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.ReqSouls"), StatFormulas.GetReqSoulsByLevel(dsPlayer.PlayerLevel).ToString(), levelUPPanel, soulsValue);

            UIImage separatorStats2 = new(longSeparatorTexture);
            separatorStats2.Left.Set(STAT_LEFT_PIXELS, 0f);
            CurrentTopPixels += reqSoulsValue.Height.Pixels;
            separatorStats2.Top.Set(CurrentTopPixels + 5f, 0f);
            levelUPPanel.Append(separatorStats2);

            CurrentTopPixels += 5f + separatorStats2.Height.Pixels;

            Texture2D vitalityTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Vitality", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref vitalityName, ref vitalityValue, vitalityTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Vitality"), dsPlayer.dsVitality.ToString(), levelUPPanel, separatorStats2);

            Texture2D attunementTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Attunement", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref attunementName, ref attunementValue, attunementTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Attunement"), dsPlayer.dsAttunement.ToString(), levelUPPanel, vitalityValue);

            Texture2D enduranceTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Endurance", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref enduranceName, ref enduranceValue, enduranceTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Endurance"), dsPlayer.dsEndurance.ToString(), levelUPPanel, attunementValue);

            Texture2D strengthTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Strength", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref strengthName, ref strengthValue, strengthTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Strength"), dsPlayer.dsStrength.ToString(), levelUPPanel, enduranceValue);

            Texture2D dexterityTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Dexterity", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref dexterityName, ref dexterityValue, dexterityTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Dexterity"), dsPlayer.dsDexterity.ToString(), levelUPPanel, strengthValue);

            Texture2D resistanceTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Resistance", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref resistanceName, ref resistanceValue, resistanceTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Resistance"), dsPlayer.dsResistance.ToString(), levelUPPanel, dexterityValue);

            Texture2D intelligenceTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Intelligence", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref intelligenceName, ref intelligenceValue, intelligenceTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Intelligence"), dsPlayer.dsIntelligence.ToString(), levelUPPanel, resistanceValue);

            Texture2D faithTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Faith", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref faithName, ref faithValue, faithTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Faith"), dsPlayer.dsFaith.ToString(), levelUPPanel, intelligenceValue);

            UIImage separatorStats3 = new(longSeparatorTexture);
            separatorStats3.Left.Set(STAT_LEFT_PIXELS, 0f);
            CurrentTopPixels += faithValue.Height.Pixels;
            separatorStats3.Top.Set(CurrentTopPixels + 5f, 0f);
            levelUPPanel.Append(separatorStats3);

            CurrentTopPixels += 5f + separatorStats3.Height.Pixels;

            Texture2D humanityTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Humanity", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref humanityValue, humanityTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Humanity"), dsPlayer.dsHumanity.ToString(), levelUPPanel, faithValue);

            // Accept button
            acceptButton = new(acceptButtonInactiveTexture);
            acceptButton.Top.Set(CurrentTopPixels + 8f, 0f);
            acceptButton.HAlign = 0.5f;
            levelUPPanel.Append(acceptButton);
        }

        private void InitializeStatPanel()
        {
            statPanel = new();
            statPanel.Width.Set(mainPanel.Width.Pixels - levelUPPanel.Width.Pixels, 0);
            statPanel.Height = levelUPPanel.Height;
            statPanel.BackgroundColor = levelUPPanel.BackgroundColor;
            statPanel.Left.Set(levelUPPanel.Width.Pixels - 3f, 0f);
            mainPanel.Append(statPanel);

            UIImage separator = new(shortSeparatorTexture);
            separator.Top = luppSeparator.Top;
            statPanel.Append(separator);

            UIImage separator2 = new(shortSeparatorTexture);
            separator2.Top = separator.Top;
            separator2.Left.Set(separator.Width.Pixels - 10f, 0f);
            statPanel.Append(separator2);

            UIImage separator3 = new(shortSeparatorTexture);
            separator3.Top = luppSeparator2.Top;
            statPanel.Append(separator3);

            UIImage separator4 = new(shortSeparatorTexture);
            separator4.Top = separator3.Top;
            separator4.Left.Set(separator3.Width.Pixels - 10f, 0f);
            statPanel.Append(separator4);

            // - - - Stats - - -
            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();

            Texture2D hpTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/HP", AssetRequestMode.ImmediateLoad).Value;
            CurrentTopPixels = nameField.Top.Pixels - (nameField.Height.Pixels - hpTexture.Height) + 1f;
            CreateStatUIElement(ref hpValue, hpTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.HP"), $"{Main.LocalPlayer.statLife}/{Main.LocalPlayer.statLifeMax2}", statPanel, separator4);

            Texture2D staminaTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Stamina", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref staminaValue, staminaTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Stamina"), DarkSoulsPlayer.DEFAULT_MAX_STAMINA.ToString(), statPanel, staminaValue);

            Texture2D manaTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Mana", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref manaValue, manaTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Mana"), $"{Main.LocalPlayer.statMana}/{Main.LocalPlayer.statManaMax2}", statPanel, staminaValue);

            Texture2D defenseTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Defense", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref defenseValue, defenseTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Defense"), "0%", statPanel, manaValue);

            Texture2D debuffsResistanceTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/DebuffsResistance", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref debuffsResistanceValue, debuffsResistanceTexture, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.DebuffsResistance"), "0%", statPanel, manaValue);

            Texture2D emptyStatIcon = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/EmptyStatIcon", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref invincibilityFramesValue, emptyStatIcon, Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.InvincibilityFrames"), "10", statPanel, debuffsResistanceValue);
        }

        public override void OnInitialize()
        {
            if (!Main.dedServ)
            {
                InitializeMainPanel();
                InitializeLevelUPPanel();
                InitializeStatPanel();
            }
        }

        private void SetStatValueColor(UIText valueText, int currentValue, int playerValue)
        {
            if (currentValue < playerValue)
                valueText.TextColor = Color.Crimson;
            else if (currentValue == playerValue)
                valueText.TextColor = Color.White;
            else
                valueText.TextColor = Color.DodgerBlue;
        }

        public override void Update(GameTime gameTime)
        {
            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            playerName.SetText(Main.LocalPlayer.name);

            if (!openedFromBonfire && !respecStats) // show real stats (Status out of the bonfire)
            {
                ResetValues();
                luppTitle.SetText(Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.Status"), 1.3f, false);
                luppDescription.SetText(Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.CheckStatus"), 0.9f, false);
                luppIcon.SetImage(statusTexture);

                reqSoulsValue.SetText(StatFormulas.GetReqSoulsByLevel(dsPlayer.PlayerLevel + 1).ToString());

                hpValue.SetText($"{Main.LocalPlayer.statLife}/{Main.LocalPlayer.statLifeMax2}");
                staminaValue.SetText($"{(int)Math.Floor(dsPlayer.currentStamina)}/{(int)Math.Floor(dsPlayer.maxStamina)}");
                manaValue.SetText($"{Main.LocalPlayer.statMana}/{Main.LocalPlayer.statManaMax2}");
                defenseValue.SetText($"{Math.Round(StatFormulas.GetDefenseByResistance(Int32.Parse(resistanceValue.Text)) * 100, 2)}%");
                debuffsResistanceValue.SetText($"{Math.Round(StatFormulas.GetDebuffsResistanceByResistance(Int32.Parse(resistanceValue.Text)) * 100, 2)}%");
                invincibilityFramesValue.SetText($"{StatFormulas.GetInvincibilityFramesByResistance(dsPlayer.dsResistance)}");
            }
            else if (respecStats && !openedFromBonfire) // respec stats
            {
                luppTitle.SetText(Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.RespecStatsTitle"), 1f, false);
                luppDescription.SetText(Language.GetText("Mods.DarkSouls.UI.StatsUI.RespecStats").WithFormatArgs(dsPlayer.PlayerLevel), 0.8f, false);
                luppIcon.SetImage(levelUPTexture);
                reqSoulsValue.SetText("0");
    
                if (firstUpdate)
                {
                    levelValue.SetText("8");
                    vitalityValue.SetText("1");
                    attunementValue.SetText("1");
                    enduranceValue.SetText("1");
                    strengthValue.SetText("1");
                    dexterityValue.SetText("1");
                    resistanceValue.SetText("1");
                    intelligenceValue.SetText("1");
                    faithValue.SetText("1");
                    firstUpdate = false;
                }

                SetStatValueColor(levelValue, Int32.Parse(levelValue.Text), dsPlayer.PlayerLevel);
                SetStatValueColor(vitalityValue, Int32.Parse(vitalityValue.Text), dsPlayer.dsVitality);
                SetStatValueColor(attunementValue, Int32.Parse(attunementValue.Text), dsPlayer.dsAttunement);
                SetStatValueColor(enduranceValue, Int32.Parse(enduranceValue.Text), dsPlayer.dsEndurance);
                SetStatValueColor(strengthValue, Int32.Parse(strengthValue.Text), dsPlayer.dsStrength);
                SetStatValueColor(dexterityValue, Int32.Parse(dexterityValue.Text), dsPlayer.dsDexterity);
                SetStatValueColor(resistanceValue, Int32.Parse(resistanceValue.Text), dsPlayer.dsResistance);
                SetStatValueColor(intelligenceValue, Int32.Parse(intelligenceValue.Text), dsPlayer.dsIntelligence);
                SetStatValueColor(faithValue, Int32.Parse(faithValue.Text), dsPlayer.dsFaith);

                int hpOld = StatFormulas.GetHPByVitality(dsPlayer.dsVitality); 
                int hpNew = StatFormulas.GetHPByVitality(Int32.Parse(vitalityValue.Text));

                int staminaNew = (int)(StatFormulas.GetStaminaByEndurance(Int32.Parse(enduranceValue.Text)));

                int manaOld = StatFormulas.GetManaByAttunement(dsPlayer.dsAttunement);
                int manaNew = StatFormulas.GetManaByAttunement(Int32.Parse(attunementValue.Text));

                double defenseOld = Math.Round(StatFormulas.GetDefenseByResistance(dsPlayer.dsResistance) * 100, 2);
                double defenseNew = Math.Round(StatFormulas.GetDefenseByResistance(Int32.Parse(resistanceValue.Text)) * 100, 2);

                double debuffsResistanceOld = Math.Round(StatFormulas.GetDebuffsResistanceByResistance(dsPlayer.dsResistance) * 100, 2);
                double debuffsResistanceNew = Math.Round(StatFormulas.GetDebuffsResistanceByResistance(Int32.Parse(resistanceValue.Text)) * 100, 2);

                int invincibilityFramesOld = StatFormulas.GetInvincibilityFramesByResistance(dsPlayer.dsResistance);
                int invincibilityFramesNew = StatFormulas.GetInvincibilityFramesByResistance(Int32.Parse(resistanceValue.Text));

                hpValue.SetText($"{Main.LocalPlayer.statLifeMax2} > {Main.LocalPlayer.statLifeMax2 + hpNew - hpOld}");
                staminaValue.SetText($"{dsPlayer.maxStamina} > {staminaNew}");
                manaValue.SetText($"{Main.LocalPlayer.statManaMax2} > {Main.LocalPlayer.statManaMax2 + manaNew - manaOld}");
                defenseValue.SetText($"{defenseOld}% > {defenseNew}%");
                debuffsResistanceValue.SetText($"{debuffsResistanceOld}% > {debuffsResistanceNew}%");
                invincibilityFramesValue.SetText($"{invincibilityFramesOld} > {invincibilityFramesNew}");

                if (Int32.Parse(levelValue.Text) == dsPlayer.PlayerLevel &&
                    Main.LocalPlayer.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<FireKeeperSoul>()))
                {
                    acceptButton.SetImage(acceptButtonActiveTexture);
                    acceptButtonIsActive = true;
                }
                else
                {
                    acceptButton.SetImage(acceptButtonInactiveTexture);
                    acceptButtonIsActive = false;
                }
            }
            else // show visual stats (Level Up at the bonfire)
            {
                luppTitle.SetText(Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.LevelUP"), 1.3f, false);
                luppDescription.SetText(Language.GetTextValue("Mods.DarkSouls.UI.StatsUI.SelectParameterToBoost"), 0.9f, false);
                luppIcon.SetImage(levelUPTexture);
                reqSoulsValue.SetText(StatFormulas.GetReqSoulsByLevel(Int32.Parse(levelValue.Text) + 1).ToString());

                int hpVisual = StatFormulas.GetHPByVitality(Int32.Parse(vitalityValue.Text));
                int hpReal = StatFormulas.GetHPByVitality(dsPlayer.dsVitality);

                int staminaVisual = (int)(StatFormulas.GetStaminaByEndurance(Int32.Parse(enduranceValue.Text)));

                int manaVisual = StatFormulas.GetManaByAttunement(Int32.Parse(attunementValue.Text));
                int manaReal = StatFormulas.GetManaByAttunement(dsPlayer.dsAttunement);

                double defenseVisual = StatFormulas.GetDefenseByResistance(Int32.Parse(resistanceValue.Text));
                double defenseReal = StatFormulas.GetDefenseByResistance(dsPlayer.dsResistance);

                double debuffsResistanceVisual = StatFormulas.GetDebuffsResistanceByResistance(Int32.Parse(resistanceValue.Text));
                double debuffsResistanceReal = StatFormulas.GetDebuffsResistanceByResistance(dsPlayer.dsResistance);

                int invincibilityFramesVisual = StatFormulas.GetInvincibilityFramesByResistance(Int32.Parse(resistanceValue.Text));
                int invincibilityFramesReal = StatFormulas.GetInvincibilityFramesByResistance(dsPlayer.dsResistance);

                hpValue.SetText($"{Main.LocalPlayer.statLifeMax2} > {Main.LocalPlayer.statLifeMax2 + hpVisual - hpReal}");
                staminaValue.SetText($"{(int)Math.Floor(dsPlayer.currentStamina)} > {staminaVisual}");
                manaValue.SetText($"{Main.LocalPlayer.statManaMax2} > {Main.LocalPlayer.statManaMax2 + manaVisual - manaReal}");
                defenseValue.SetText($"{Math.Round(defenseReal * 100, 2)}% > {Math.Round(defenseVisual * 100, 2)}%");
                debuffsResistanceValue.SetText($"{Math.Round(debuffsResistanceReal * 100, 2)}% > {Math.Round(debuffsResistanceVisual * 100, 2)}%");
                invincibilityFramesValue.SetText($"{invincibilityFramesReal} > {invincibilityFramesVisual}");
                if (updateSoulsValue)
                {
                    soulsValue.SetText(dsPlayer.dsSouls.ToString());
                    updateSoulsValue = false;
                }
            }

            // Souls value
            if (!respecStats)
            {
                if (long.Parse(soulsValue.Text) < StatFormulas.GetReqSoulsByLevel(Int32.Parse(levelValue.Text) + 1))
                    soulsValue.TextColor = Color.Crimson;
                else
                    soulsValue.TextColor = Color.White;
            }

            humanityValue.SetText(dsPlayer.dsHumanity.ToString());
        }

        public override void LeftClick(UIMouseEvent evt) // Increase stat level
        {
            if (!openedFromBonfire && !respecStats)
                return;

            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            int targetId = evt.Target.UniqueId;
            bool statValueIncreased = false;

            if (targetId == dexterityValue.UniqueId || targetId == dexterityName.UniqueId)
                statValueIncreased = IncreaseStatValue(dexterityValue);
            else if (targetId == vitalityValue.UniqueId || targetId == vitalityName.UniqueId)
                statValueIncreased = IncreaseStatValue(vitalityValue);
            else if (targetId == attunementValue.UniqueId || targetId == attunementName.UniqueId)
                statValueIncreased = IncreaseStatValue(attunementValue);
            else if (targetId == enduranceValue.UniqueId || targetId == enduranceName.UniqueId)
                statValueIncreased = IncreaseStatValue(enduranceValue);
            else if (targetId == strengthValue.UniqueId || targetId == strengthName.UniqueId)
                statValueIncreased = IncreaseStatValue(strengthValue);
            else if (targetId == resistanceValue.UniqueId || targetId == resistanceName.UniqueId)
                statValueIncreased = IncreaseStatValue(resistanceValue);
            else if (targetId == intelligenceValue.UniqueId || targetId == intelligenceName.UniqueId)
                statValueIncreased = IncreaseStatValue(intelligenceValue);
            else if (targetId == faithValue.UniqueId || targetId == faithName.UniqueId)
                statValueIncreased = IncreaseStatValue(faithValue);

            if (statValueIncreased)
            {
                SoundEngine.PlaySound(DarkSouls.dsInferfaceClickSound);

                int levelIntValue = Int32.Parse(levelValue.Text);
                long reqSouls = long.Parse(reqSoulsValue.Text);

                levelValue.SetText((levelIntValue + 1).ToString());

                if (!respecStats)
                {
                    levelValue.TextColor = Color.DodgerBlue;
                    soulsValue.SetText((long.Parse(soulsValue.Text) - reqSouls).ToString());

                    acceptButton.SetImage(acceptButtonActiveTexture);
                    acceptButtonIsActive = true;

                    levelUPCosts.Push(reqSouls);
                }
            }
            else
                SoundEngine.PlaySound(DarkSouls.dsInferfaceReturnSound);

            if (targetId == acceptButton.UniqueId && acceptButtonIsActive) // Accept level up
            {
                if (respecStats)
                {
                    int fireKeeperSoulItemType = ModContent.ItemType<FireKeeperSoul>();
                    if (Main.LocalPlayer.HasItemInInventoryOrOpenVoidBag(fireKeeperSoulItemType))
                    {
                        FireKeeperSoul.canConsume = true;
                        if (!Main.LocalPlayer.ConsumeItem(fireKeeperSoulItemType, includeVoidBag: true))
                        {
                            SoundEngine.PlaySound(DarkSouls.dsInferfaceReturnSound);
                            FireKeeperSoul.canConsume = false;
                            return;
                        }
                    }
                }

                SoundEngine.PlaySound(DarkSouls.dsInferfaceSound);
                dsPlayer.dsSouls = long.Parse(soulsValue.Text);
                dsPlayer.dsAttunement = Int32.Parse(attunementValue.Text);
                dsPlayer.dsDexterity = Int32.Parse(dexterityValue.Text);
                dsPlayer.dsEndurance = Int32.Parse(enduranceValue.Text);
                dsPlayer.dsFaith = Int32.Parse(faithValue.Text);
                dsPlayer.dsIntelligence = Int32.Parse(intelligenceValue.Text);
                dsPlayer.dsResistance = Int32.Parse(resistanceValue.Text);
                dsPlayer.dsStrength = Int32.Parse(strengthValue.Text);
                dsPlayer.dsVitality = Int32.Parse(vitalityValue.Text);
                OnDeactivate();
            }
        }

        public override void RightClick(UIMouseEvent evt) // Decrease stat value
        {
            if (!openedFromBonfire && !respecStats)
                return;

            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            int targetId = evt.Target.UniqueId;
            bool statValueDecreased = false;

            if (targetId == dexterityValue.UniqueId || targetId == dexterityName.UniqueId)
                statValueDecreased = DecreaseStatValue(dexterityValue, dsPlayer.dsDexterity);
            else if (targetId == vitalityValue.UniqueId || targetId == vitalityName.UniqueId)
                statValueDecreased = DecreaseStatValue(vitalityValue, dsPlayer.dsVitality);
            else if (targetId == attunementValue.UniqueId || targetId == attunementName.UniqueId)
                statValueDecreased = DecreaseStatValue(attunementValue, dsPlayer.dsAttunement);
            else if (targetId == enduranceValue.UniqueId || targetId == enduranceName.UniqueId)
                statValueDecreased = DecreaseStatValue(enduranceValue, dsPlayer.dsEndurance);
            else if (targetId == strengthValue.UniqueId || targetId == strengthName.UniqueId)
                statValueDecreased = DecreaseStatValue(strengthValue, dsPlayer.dsStrength);
            else if (targetId == resistanceValue.UniqueId || targetId == resistanceName.UniqueId)
                statValueDecreased = DecreaseStatValue(resistanceValue, dsPlayer.dsResistance);
            else if (targetId == intelligenceValue.UniqueId || targetId == intelligenceName.UniqueId)
                statValueDecreased = DecreaseStatValue(intelligenceValue, dsPlayer.dsIntelligence);
            else if (targetId == faithValue.UniqueId || targetId == faithName.UniqueId)
                statValueDecreased = DecreaseStatValue(faithValue, dsPlayer.dsFaith);

            int levelIntValue = Int32.Parse(levelValue.Text);
            if (statValueDecreased)
            {
                SoundEngine.PlaySound(DarkSouls.dsInferfaceClickSound);

                levelValue.SetText((levelIntValue - 1).ToString());

                if (!respecStats)
                {
                    if (levelIntValue - 1 == dsPlayer.PlayerLevel)
                    {
                        levelValue.TextColor = Color.White;
                        acceptButton.SetImage(acceptButtonInactiveTexture);
                        acceptButtonIsActive = false;
                    }

                    soulsValue.SetText((long.Parse(soulsValue.Text) + levelUPCosts.Pop()).ToString());
                }
            }
            else
                SoundEngine.PlaySound(DarkSouls.dsInferfaceReturnSound);
        }

        public override void OnDeactivate()
        {
            ResetValues();
            acceptButton.SetImage(acceptButtonInactiveTexture);
            acceptButtonIsActive = false;
            updateSoulsValue = true;
            firstUpdate = true;
            levelUPCosts.Clear();
            if (respecStats)
            {
                respecStats = false;
                openedFromBonfire = false;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (levelUPPanel.ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;
        }

        private bool IncreaseStatValue(UIText statValueUIText)
        {
            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            int statValue = Int32.Parse(statValueUIText.Text);

            if (respecStats)
            {
                if (Int32.Parse(levelValue.Text) + 1 <= dsPlayer.PlayerLevel && statValue + 1 <= 99)
                {
                    statValueUIText.SetText((statValue + 1).ToString());
                    return true;
                }
                return false;
            }
            else
            {
                int reqSouls = Int32.Parse(reqSoulsValue.Text);

                if (long.Parse(soulsValue.Text) - reqSouls < 0 || statValue + 1 > 99)
                    return false;

                statValueUIText.SetText((statValue + 1).ToString());
                statValueUIText.TextColor = Color.DodgerBlue;
                return true;
            }
        }

        private bool DecreaseStatValue(UIText statValueUIText, int dsPlayerStatValue)
        {
            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            int statValue = Int32.Parse(statValueUIText.Text);

            if (respecStats)
            {
                if (Int32.Parse(levelValue.Text) - 1 >= 8 && statValue - 1 > 0)
                {
                    statValueUIText.SetText((statValue - 1).ToString());
                    return true;
                }
                return false;
            }
            else
            {
                if (statValue - 1 > dsPlayerStatValue)
                {
                    statValueUIText.SetText((statValue - 1).ToString());
                    return true;
                }
                else if (statValue - 1 == dsPlayerStatValue)
                {
                    statValueUIText.SetText(dsPlayerStatValue.ToString());
                    statValueUIText.TextColor = Color.White;
                    return true;
                }
                return false;
            }

        }

        private void ResetValues()
        {
            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();

            vitalityValue.SetText(dsPlayer.dsVitality.ToString());
            vitalityValue.TextColor = Color.White;

            attunementValue.SetText(dsPlayer.dsAttunement.ToString());
            attunementValue.TextColor = Color.White;

            enduranceValue.SetText(dsPlayer.dsEndurance.ToString());
            enduranceValue.TextColor = Color.White;

            strengthValue.SetText(dsPlayer.dsStrength.ToString());
            strengthValue.TextColor = Color.White;

            dexterityValue.SetText(dsPlayer.dsDexterity.ToString());
            dexterityValue.TextColor = Color.White;

            resistanceValue.SetText(dsPlayer.dsResistance.ToString());
            resistanceValue.TextColor = Color.White;

            intelligenceValue.SetText(dsPlayer.dsIntelligence.ToString());
            intelligenceValue.TextColor = Color.White;

            faithValue.SetText(dsPlayer.dsFaith.ToString());
            faithValue.TextColor = Color.White;

            levelValue.SetText(dsPlayer.PlayerLevel.ToString());
            levelValue.TextColor = Color.White;

            soulsValue.SetText(dsPlayer.dsSouls.ToString());
            soulsValue.TextColor = Color.White;

            humanityValue.SetText(dsPlayer.dsHumanity.ToString());
        }
    }
}