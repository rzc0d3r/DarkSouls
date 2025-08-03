using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DarkSouls.Items.Consumables
{
    public class Humanity : ModItem
    {
        private SlotId soundSlotId = SlotId.Invalid;

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 82;
            Item.useAnimation = 82;
            Item.useTurn = true;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                soundSlotId = SoundEngine.PlaySound(DarkSouls.dsConsumeSoulSound);
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.GetModPlayer<DarkSoulsPlayer>().dsHumanity >= 99)
                return false;
            SoundEngine.TryGetActiveSound(soundSlotId, out var activeSound);
            if (activeSound == null || !activeSound.IsPlaying)
                soundSlotId = SlotId.Invalid;
            return soundSlotId == SlotId.Invalid;
        }

        public override void OnConsumeItem(Player player) => player.GetModPlayer<DarkSoulsPlayer>().dsHumanity++;
        
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float pulse = 0.15f + Math.Clamp((float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * Main.rand.NextFloat(0.8f, 1.1f), 0.5f, 1f);
            Vector3 rgb = new Vector3(0.2f, 0.2f, 0.2f) * pulse;
            Lighting.AddLight(Item.Center, rgb);
        }
    }
}
