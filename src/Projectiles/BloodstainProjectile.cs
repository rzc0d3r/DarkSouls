using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DarkSouls.Projectiles
{
    public class BloodstainProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 50;
            Projectile.timeLeft = int.MaxValue;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.timeLeft = int.MaxValue;
            ++Projectile.frameCounter;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 7)
                Projectile.frame = 0;

            float pulse = 0.2f + 0.2f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f);

            Vector3 lightColor = new Vector3(0f, 1f, 0f) * pulse;
            Projectile.alpha = (int)(130 * pulse);

            Lighting.AddLight(Projectile.Center, lightColor);

            if (Main.rand.NextBool(6))
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float speed = Main.rand.NextFloat(2.5f, 4.8f);
                Vector2 velocity = angle.ToRotationVector2() * speed;

                int dustID = Dust.NewDust(Projectile.Center, 0, 0, DustID.GreenFairy, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[dustID];
                dust.velocity = velocity;
                dust.noGravity = true;
                dust.fadeIn = 0.4f;
                dust.scale = 0.9f;
                dust.alpha = 160;
            }
        }
    }
}