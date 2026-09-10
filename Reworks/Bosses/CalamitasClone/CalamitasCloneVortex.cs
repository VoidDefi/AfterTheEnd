using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Particles;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs.CalClone;

namespace AfterTheEnd.Reworks.Bosses.CalamitasClone
{
    /// <summary>
    /// Code from Calamity mod 
    /// </summary>
    public class CalamitasCloneVortex : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        
        public new string LocalizationCategory => "Projectiles.Boss";

        public int CalamitasIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private NPC Calamitas
        {
            get
            {
                if (CalamitasIndex < 0 || CalamitasIndex >= Main.maxNPCs)
                    return null;

                NPC npc = Main.npc[CalamitasIndex];

                if (npc.active && npc.type == ModContent.NPCType<CalamityMod.NPCs.CalClone.CalamitasClone>())
                    return npc;

                return null;
            }
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200000;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.9f * Projectile.Opacity, 0f, 0f);

            if (Calamitas != null)
            {
                Projectile.Center = Calamitas.Center;
                Projectile.Center += Vector2.UnitY.RotatedBy(Calamitas.rotation) * 200;

                Projectile.scale += 0.013f;
            }

            for (var i = 0; i < 1; i++)
            {
                var p = CalamitasMetaball.SpawnParticle(Projectile.Center + Projectile.velocity * 2, Vector2.Zero, 64 * 2 * Projectile.scale);
                p.rotation = -Projectile.timeLeft / 15f;
                p.TextureToUse = ModContent.Request<Texture2D>("AfterTheEnd/Reworks/Bosses/CalamitasClone/DarkVortex").Value;
                p.SizeScaling = 0f;

                p = CalamitasMetaball.SpawnParticle(Projectile.Center, Main.rand.NextVector2Circular(1, 1) * 3f * Projectile.scale, (90f - 30f) * Projectile.scale);
                p.SizeScaling = 0.95f;

                for (int j = 0; j < (int)(2f * Projectile.scale); j++)
                {
                    Vector2 circlePosition = Main.rand.NextVector2CircularEdge(1, 1) * Projectile.scale;

                    p = CalamitasMetaball.SpawnParticle(Projectile.Center + circlePosition * 200, 20f * (-circlePosition) * Math.Clamp(Projectile.scale, 0, 0.5f), 20f * Projectile.scale);
                    p.Scale = new Vector2(1f, 0.33f);
                    p.rotation = p.Velocity.ToRotation();
                    p.SizeScaling = 0.9f;
                }
            }

            Projectile.scale -= 0.01f;

            if (Projectile.scale < 0) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft >= 51;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.timeLeft < 51)
                return;

            if (Main.zenithWorld)
                target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 180);
            else
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }

        public override void OnKill(int timeLeft)
        {
            /*for (int dust = 0; dust <= 5; dust++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f);
            }*/
        }
    }
}
