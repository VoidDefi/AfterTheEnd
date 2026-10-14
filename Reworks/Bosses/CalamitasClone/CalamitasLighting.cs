using AfterTheEnd.Utilities;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AfterTheEnd.Reworks.Bosses.CalamitasClone
{
    public class CalamitasLighting : ModProjectile
    {
        public static readonly AutoAsset<Texture2D> LightTexture = new("AfterTheEnd/Reworks/Bosses/CalamitasClone/Light");

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int CalamitasIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public bool CalamitasWasDespawned
        {
            get => Projectile.ai[1] > 0;
            set => Projectile.ai[1] = value ? 1 : 0;
        }

        public float LightingValue
        {
            get => Projectile.ai[2];
            set => Projectile.ai[2] = value;
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

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200000;
        }

        public override void AI()
        {
            //if (Projectile.timeLeft == 200000) LightingValue = 1;

            if (Calamitas != null && !CalamitasWasDespawned)
            {
                Projectile.Center = Calamitas.Center;
                Projectile.Center += Vector2.UnitY.RotatedBy(Calamitas.rotation) * 30;

                LightingValue += 0.15f;
            }

            else CalamitasWasDespawned = true;

            LightingValue -= 0.1f;

            if (LightingValue < 0) Projectile.Kill();
            if (LightingValue > 1) LightingValue = 1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = LightTexture.Value.Size() / 2f;

            Color coldColor = new Color(255, 25, 35, 256);
            Color hotColor = new Color(255, 255, 100, 0);

            coldColor *= LightingValue;
            hotColor *= LightingValue;

            float scale = MathHelper.Lerp(0.6f, 1f, LightingValue) * 1.2f;

            Main.spriteBatch.Draw(LightTexture.Value, position, null, coldColor, 0, origin, scale, 0, 0);
            Main.spriteBatch.Draw(LightTexture.Value, position, null, hotColor, 0, origin, scale, 0, 0);

            return false;
        }
    }
}
