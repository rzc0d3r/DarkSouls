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

namespace DarkSouls.UI
{
    [Autoload(true, Side = ModSide.Client)]
    public class DarkSoulsStatsUI : UIState
    {
        public bool openedFromBonfire = false;

        private Stack<int> levelUPCosts = new Stack<int>();

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
        private UIText vitalityValue;
        private UIText strengthValue;
        private UIText resistanceValue;
        private UIText intelligenceValue;
        private UIText humanityValue;
        private UIText faithValue;
        private UIText enduranceValue;
        private UIText dexterityValue;
        private UIText attunementValue;
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

        private void CreateStatUIElement(ref UIText statValueUIElement, Texture2D iconTexture, string statName, string statValue, UIPanel srcPanel, UIElement prevUIElement)
        {
            UIImage statIcon = new UIImage(iconTexture);
            statIcon.Left.Set(STAT_LEFT_PIXELS, 0f);
            statIcon.Top.Set(CurrentTopPixels + 5f, 0f);
            srcPanel.Append(statIcon);

            float textHeight = FontAssets.MouseText.Value.MeasureString(statName).Y;

            UIText statNameUIElement = new UIText(statName);
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

            luppTitle = new("Status", 1.3f);
            luppTitle.Left.Set(statusTexture.Width + 8f, 0f);
            levelUPPanel.Append(luppTitle);

            luppSeparator = new(shortSeparatorTexture);
            luppSeparator.Left.Set(luppTitle.Left.Pixels - 5f, 0f);
            luppSeparator.Top.Set(25f, 0f);
            levelUPPanel.Append(luppSeparator);

            luppDescription = new("Check status", 0.9f);
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
            UIText covenantField = new("Covenant");
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
            CreateStatUIElement(ref levelValue, levelTexture, "Level", dsPlayer.PlayerLevel.ToString(), levelUPPanel, separatorStats);

            Texture2D soulsTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Souls", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref soulsValue, soulsTexture, "Souls", dsPlayer.dsSouls.ToString(), levelUPPanel, levelValue);
            CreateStatUIElement(ref reqSoulsValue, soulsTexture, "ReqSouls", DarkSoulsPlayer.GetReqSoulsByLevel(dsPlayer.PlayerLevel).ToString(), levelUPPanel, soulsValue);

            UIImage separatorStats2 = new(longSeparatorTexture);
            separatorStats2.Left.Set(STAT_LEFT_PIXELS, 0f);
            CurrentTopPixels += reqSoulsValue.Height.Pixels;
            separatorStats2.Top.Set(CurrentTopPixels + 5f, 0f);
            levelUPPanel.Append(separatorStats2);

            CurrentTopPixels += 5f + separatorStats2.Height.Pixels;

            Texture2D vitalityTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Vitality", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref vitalityValue, vitalityTexture, "Vitality", dsPlayer.dsVitality.ToString(), levelUPPanel, separatorStats2);

            Texture2D attunementTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Attunement", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref attunementValue, attunementTexture, "Attunement", dsPlayer.dsAttunement.ToString(), levelUPPanel, vitalityValue);

            Texture2D enduranceTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Endurance", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref enduranceValue, enduranceTexture, "Endurance", dsPlayer.dsEndurance.ToString(), levelUPPanel, attunementValue);

            Texture2D strengthTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Strength", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref strengthValue, strengthTexture, "Strength", dsPlayer.dsStrength.ToString(), levelUPPanel, enduranceValue);

            Texture2D dexterityTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Dexterity", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref dexterityValue, dexterityTexture, "Dexterity", dsPlayer.dsDexterity.ToString(), levelUPPanel, strengthValue);

            Texture2D resistanceTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Resistance", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref resistanceValue, resistanceTexture, "Resistance", dsPlayer.dsResistance.ToString(), levelUPPanel, dexterityValue);

            Texture2D intelligenceTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Intelligence", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref intelligenceValue, intelligenceTexture, "Intelligence", dsPlayer.dsIntelligence.ToString(), levelUPPanel, resistanceValue);

            Texture2D faithTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Faith", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref faithValue, faithTexture, "Faith", dsPlayer.dsFaith.ToString(), levelUPPanel, intelligenceValue);

            UIImage separatorStats3 = new(longSeparatorTexture);
            separatorStats3.Left.Set(STAT_LEFT_PIXELS, 0f);
            CurrentTopPixels += faithValue.Height.Pixels;
            separatorStats3.Top.Set(CurrentTopPixels + 5f, 0f);
            levelUPPanel.Append(separatorStats3);

            CurrentTopPixels += 5f + separatorStats3.Height.Pixels;

            Texture2D humanityTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Humanity", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref humanityValue, humanityTexture, "Humanity", dsPlayer.dsHumanity.ToString(), levelUPPanel, faithValue);

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
            CreateStatUIElement(ref hpValue, hpTexture, "HP", $"{Main.LocalPlayer.statLife}/{Main.LocalPlayer.statLifeMax2}", statPanel, separator4);

            Texture2D staminaTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Stamina", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref staminaValue, staminaTexture, "Stamina", DarkSoulsPlayer.DEFAULT_MAX_STAMINA.ToString(), statPanel, staminaValue);

            Texture2D manaTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Mana", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref manaValue, manaTexture, "Mana", $"{Main.LocalPlayer.statMana}/{Main.LocalPlayer.statManaMax2}", statPanel, staminaValue);

            Texture2D defenseTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/Defense", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref defenseValue, defenseTexture, "Defense", "0%", statPanel, manaValue);

            Texture2D debuffsResistanceTexture = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/DebuffsResistance", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref debuffsResistanceValue, debuffsResistanceTexture, "Debuffs resistance", "0%", statPanel, manaValue);

            Texture2D emptyStatIcon = ModContent.Request<Texture2D>("DarkSouls/UI/Textures/EmptyStatIcon", AssetRequestMode.ImmediateLoad).Value;
            CreateStatUIElement(ref invincibilityFramesValue, emptyStatIcon, "Invincibility frames", "10", statPanel, debuffsResistanceValue);
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

        public override void Update(GameTime gameTime)
        {
            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            playerName.SetText(Main.LocalPlayer.name);

            if (!openedFromBonfire) // show real stats (Status out of the bonfire)
            {
                ResetValues();
                luppTitle.SetText("Status", 1.3f, false);
                luppDescription.SetText("Check status", 0.9f, false);
                luppIcon.SetImage(statusTexture);

                reqSoulsValue.SetText(DarkSoulsPlayer.GetReqSoulsByLevel(dsPlayer.PlayerLevel + 1).ToString());

                hpValue.SetText($"{Main.LocalPlayer.statLife}/{Main.LocalPlayer.statLifeMax2}");
                staminaValue.SetText($"{(int)Math.Floor(dsPlayer.currentStamina)}/{(int)Math.Floor(dsPlayer.maxStamina)}");
                manaValue.SetText($"{Main.LocalPlayer.statMana}/{Main.LocalPlayer.statManaMax2}");
                defenseValue.SetText($"{Math.Round(DarkSoulsPlayer.GetDefenseByResistance(Int32.Parse(resistanceValue.Text)) * 100, 2)}%");
                debuffsResistanceValue.SetText($"{Math.Round(DarkSoulsPlayer.GetDebuffsResistanceByResistance(Int32.Parse(resistanceValue.Text)) * 100, 2)}%");
                invincibilityFramesValue.SetText($"{DarkSoulsPlayer.GetInvincibilityFramesByResistance(dsPlayer.dsResistance)}");
            }
            else // show visual stats (Level Up at the bonfire)
            {
                luppTitle.SetText("Level Up", 1.3f, false);
                luppDescription.SetText("Select a parameter to boost", 0.9f, false);
                luppIcon.SetImage(levelUPTexture);
                reqSoulsValue.SetText(DarkSoulsPlayer.GetReqSoulsByLevel(Int32.Parse(levelValue.Text) + 1).ToString());

                int hpVisual = DarkSoulsPlayer.GetHPByVitality(Int32.Parse(vitalityValue.Text));
                int hpReal = DarkSoulsPlayer.GetHPByVitality(dsPlayer.dsVitality);

                int staminaVisual = (int)(DarkSoulsPlayer.GetStaminaByEndurance(Int32.Parse(enduranceValue.Text)));

                int manaVisual = DarkSoulsPlayer.GetManaByAttunement(Int32.Parse(attunementValue.Text));
                int manaReal = DarkSoulsPlayer.GetManaByAttunement(dsPlayer.dsAttunement);

                double defenseVisual = DarkSoulsPlayer.GetDefenseByResistance(Int32.Parse(resistanceValue.Text));
                double defenseReal = DarkSoulsPlayer.GetDefenseByResistance(dsPlayer.dsResistance);

                double debuffsResistanceVisual = DarkSoulsPlayer.GetDebuffsResistanceByResistance(Int32.Parse(resistanceValue.Text));
                double debuffsResistanceReal = DarkSoulsPlayer.GetDebuffsResistanceByResistance(dsPlayer.dsResistance);

                int invincibilityFramesVisual = DarkSoulsPlayer.GetInvincibilityFramesByResistance(Int32.Parse(resistanceValue.Text));
                int invincibilityFramesReal = DarkSoulsPlayer.GetInvincibilityFramesByResistance(dsPlayer.dsResistance);

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
            if (Int32.Parse(soulsValue.Text) < DarkSoulsPlayer.GetReqSoulsByLevel(Int32.Parse(levelValue.Text) + 1))
                soulsValue.TextColor = Color.Crimson;
            else
                soulsValue.TextColor = Color.White;
        }

        public override void LeftClick(UIMouseEvent evt) // Increase stat level
        {
            if (!openedFromBonfire)
                return;

            int targetId = evt.Target.UniqueId;
            bool statValueIncreased = false;

            if (targetId == dexterityValue.UniqueId)
                statValueIncreased = IncreaseStatValue(dexterityValue);
            else if (targetId == vitalityValue.UniqueId)
                statValueIncreased = IncreaseStatValue(vitalityValue);
            else if (targetId == attunementValue.UniqueId)
                statValueIncreased = IncreaseStatValue(attunementValue);
            else if (targetId == enduranceValue.UniqueId)
                statValueIncreased = IncreaseStatValue(enduranceValue);
            else if (targetId == strengthValue.UniqueId)
                statValueIncreased = IncreaseStatValue(strengthValue);
            else if (targetId == resistanceValue.UniqueId)
                statValueIncreased = IncreaseStatValue(resistanceValue);
            else if (targetId == intelligenceValue.UniqueId)
                statValueIncreased = IncreaseStatValue(intelligenceValue);
            else if (targetId == faithValue.UniqueId)
                statValueIncreased = IncreaseStatValue(faithValue);

            if (statValueIncreased)
            {
                SoundEngine.PlaySound(DarkSouls.dsInferfaceClickSound);

                int levelIntValue = Int32.Parse(levelValue.Text);
                int reqSouls = Int32.Parse(reqSoulsValue.Text);

                levelValue.SetText((levelIntValue + 1).ToString());
                levelValue.TextColor = Color.DodgerBlue;
                soulsValue.SetText((Int32.Parse(soulsValue.Text) - reqSouls).ToString());

                acceptButton.SetImage(acceptButtonActiveTexture);
                acceptButtonIsActive = true;

                levelUPCosts.Push(reqSouls);
            }
            else
                SoundEngine.PlaySound(DarkSouls.dsInferfaceReturnSound);

            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            if (targetId == acceptButton.UniqueId && acceptButtonIsActive) // Accept level up
            {
                SoundEngine.PlaySound(DarkSouls.dsInferfaceSound);
                dsPlayer.dsSouls = Int32.Parse(soulsValue.Text);
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
            if (!openedFromBonfire)
                return;

            DarkSoulsPlayer dsPlayer = Main.LocalPlayer.GetModPlayer<DarkSoulsPlayer>();
            int targetId = evt.Target.UniqueId;
            bool statValueDecreased = false;

            if (targetId == dexterityValue.UniqueId)
                statValueDecreased = DecreaseStatValue(dexterityValue, dsPlayer.dsDexterity);
            else if (targetId == vitalityValue.UniqueId)
                statValueDecreased = DecreaseStatValue(vitalityValue, dsPlayer.dsVitality);
            else if (targetId == attunementValue.UniqueId)
                statValueDecreased = DecreaseStatValue(attunementValue, dsPlayer.dsAttunement);
            else if (targetId == enduranceValue.UniqueId)
                statValueDecreased = DecreaseStatValue(enduranceValue, dsPlayer.dsEndurance);
            else if (targetId == strengthValue.UniqueId)
                statValueDecreased = DecreaseStatValue(strengthValue, dsPlayer.dsStrength);
            else if (targetId == resistanceValue.UniqueId)
                statValueDecreased = DecreaseStatValue(resistanceValue, dsPlayer.dsResistance);
            else if (targetId == intelligenceValue.UniqueId)
                statValueDecreased = DecreaseStatValue(intelligenceValue, dsPlayer.dsIntelligence);
            else if (targetId == faithValue.UniqueId)
                statValueDecreased = DecreaseStatValue(faithValue, dsPlayer.dsFaith);

            int levelIntValue = Int32.Parse(levelValue.Text);
            if (statValueDecreased)
            {
                SoundEngine.PlaySound(DarkSouls.dsInferfaceClickSound);

                if (levelIntValue - 1 == dsPlayer.PlayerLevel)
                {
                    levelValue.TextColor = Color.White;
                    acceptButton.SetImage(acceptButtonInactiveTexture);
                    acceptButtonIsActive = false;
                }
                
                soulsValue.SetText((Int32.Parse(soulsValue.Text) + levelUPCosts.Pop()).ToString());
                levelValue.SetText((levelIntValue - 1).ToString());
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
            levelUPCosts.Clear();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (levelUPPanel.ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;
        }

        private bool IncreaseStatValue(UIText statValueUIText)
        {
            int statValue = Int32.Parse(statValueUIText.Text);
            int reqSouls = Int32.Parse(reqSoulsValue.Text);

            if (Int32.Parse(soulsValue.Text) - reqSouls < 0 || statValue + 1 > 99)
                return false;

            statValueUIText.SetText((statValue + 1).ToString());
            statValueUIText.TextColor = Color.DodgerBlue;

            return true;
        }

        private bool DecreaseStatValue(UIText statValueUIText, int dsPlayerStatValue)
        {
            int statValue = Int32.Parse(statValueUIText.Text);
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
        }
    }
}