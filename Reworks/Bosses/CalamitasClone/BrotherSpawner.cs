using AfterTheEnd.Utilities;
using CalamityMod;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.Deconstructors;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AfterTheEnd.Reworks.Bosses.CalamitasClone
{
    public class BrotherSpawner : ModProjectile
    {
        public enum Brother
        {
            Cataclysm,
            Catastrophe
        }

        public static readonly AutoAsset<Texture2D> LightTexture = new("AfterTheEnd/Reworks/Bosses/CalamitasClone/Light");

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private NPC Calamitas
        {
            get
            {
                if (Projectile.ai[1] < 0 || Projectile.ai[1] >= Main.maxNPCs)
                    return null;

                NPC npc = Main.npc[(int)Projectile.ai[1]];

                if (npc.active && npc.type == ModContent.NPCType<CalamityMod.NPCs.CalClone.CalamitasClone>())
                    return npc;

                return null;
            }
        }

        public Brother BrotherSpawned
        {
            get => (Brother)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        public static void Spawn(NPC sourceNPC, Vector2 position, Brother brother)
        {
            if (sourceNPC.type != ModContent.NPCType<CalamityMod.NPCs.CalClone.CalamitasClone>())
            {
                return;
            }

            Projectile.NewProjectile(sourceNPC.GetSource_FromAI(), position, Vector2.Zero, ModContent.ProjectileType<BrotherSpawner>(), 0, 0, ai0: (float)brother, ai1: sourceNPC.whoAmI);
        }

        public int MaxTimeLeft => CalamitasCloneRework.BrothersSpawnDelay;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = MaxTimeLeft;
            Projectile.scale = 0;
        }

        public override void AI()
        {
            //кастыль, конечно, ну и пофиг
            if (Calamitas != null)
                Calamitas.ai[1] = 7;

            if (Projectile.timeLeft > 10)
            {
                var p = CalamitasMetaball.SpawnParticle(Projectile.Center, Vector2.Zero, 50f * Projectile.scale * (1f + (1f - (float)Projectile.timeLeft / MaxTimeLeft) / 3.2f));
                p.SizeScaling = 0.9f;
            }

            for (int j = 0; j < 10 * Projectile.scale; j++)
            {
                Vector2 circlePosition = Main.rand.NextVector2CircularEdge(1, 1) * Projectile.scale;

                if (Main.rand.Next(0, 100) < 10)
                {
                    var p = CalamitasMetaball.SpawnParticle(Projectile.Center + circlePosition * 100, 15f * (-circlePosition) * Math.Clamp(Projectile.scale, 0, 0.5f), 30f * Projectile.scale);
                    p.Scale = new Vector2(1f, 0.2f);
                    p.rotation = p.Velocity.ToRotation();
                    p.SizeScaling = 0.9f;
                }
            }

            Projectile.scale += 0.1f;
            if (Projectile.scale > 1) Projectile.scale = 1;
        }

        public override bool PreKill(int timeLeft)
        {
            int type = -1;

            if (BrotherSpawned == Brother.Cataclysm)
                type = ModContent.NPCType<Cataclysm>();

            else if (BrotherSpawned == Brother.Catastrophe)
                type = ModContent.NPCType<Catastrophe>();

            //Projectile.owner = 5;
            NPC NPC = Calamitas;

            if (NPC != null)
            {
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.position.Y + NPC.height / 2, type, NPC.whoAmI);
            }

            int count = Main.rand.Next(10, 20);
            
            for (int i = 0; i < count; i++)
            {
                //var p = CalamitasMetaball.SpawnParticle(Projectile.Center, Main.rand.NextVector2Circular(1, 1) * 5f, 30f * Projectile.scale);
                //p.Scale = new Vector2(1f, 0.2f);
                //p.rotation = p.Velocity.ToRotation();
                //p.SizeScaling = 0.9f;

                Color color = Color.White;

                if (BrotherSpawned == Brother.Cataclysm)
                    color = Main.rand.NextBool() ? new Color(80, 21, 77) : Color.Red;

                else if (BrotherSpawned == Brother.Catastrophe)
                    color = Main.rand.NextBool() ? Color.DeepSkyBlue : Color.Red;

                GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center, Utils.RotatedByRandom(new Vector2(5f, 5f), 100.0) * Main.rand.NextFloat(0.5f, 1.5f), affectedByGravity: false, 35, Main.rand.NextFloat(1.2f, 1.45f), color));
            }


            return base.PreKill(timeLeft);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = LightTexture.Value.Size() / 2f;

            float scale = Projectile.scale / 2f;

            if (Projectile.timeLeft < 15)
            {
                scale = (float)Projectile.timeLeft / 15f / 2f;
            }

            //Main.spriteBatch.Draw(LightTexture.Value, position, null, new Color(255, 0, 229) * 0.2f, 0, origin, scale, 0, 0);
            //Main.spriteBatch.Draw(LightTexture.Value, position, null, new Color(255, 255, 255) * 0.2f, 0, origin, scale * 0.5f, 0, 0);
            return false;
        }
    }
}
