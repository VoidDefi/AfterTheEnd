using CalamityMod.Effects;
using CalamityMod.Enums;
using CalamityMod.Graphics.Metaballs;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using AfterTheEnd.Utilities.Reflection;
using System.Reflection;
using ReLogic.Content;

namespace AfterTheEnd.Reworks.Bosses.CalamitasClone
{
    /// <summary>
    /// Code from Calamity Mod
    /// </summary>
    internal class LightMetaball : Metaball
    {
        public class Particle
        {
            public int Time;

            public float Size;

            public Vector2 Center;

            public Particle(Vector2 center, float size)
            {
                Center = center;
                Size = size;
            }

            public void Update()
            {
                Time++;
                Size = MathHelper.Clamp(Size - 0.24f, 0f, 300f) * 0.9f;
                if (Size < 15f)
                    Size = Size * 0.95f - 0.9f;
            }
        }

        public static List<Particle> Particles
        {
            get;
            private set;
        } = new();

        public override bool AnythingToDraw => Particles.Any();

        public override GeneralDrawLayer DrawLayer => GeneralDrawLayer.AfterProjectiles;

        public override IEnumerable<Texture2D> Layers
        {
            get
            {
                yield return ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value;
            }
        }

        public override Color EdgeColor => new(255, 56, 3);

        public override void ClearInstances() => Particles.Clear();

        private static readonly AutoFieldInfo AdditiveMetaballEdgeShaderInfo = new(typeof(CalamityShaders), "AdditiveMetaballEdgeShader", BindingFlags.Static | BindingFlags.NonPublic);

        public override void PrepareShaderForTarget(int layerIndex)
        {
            // Store the shader in an easy to use local variable.
            var metaballShader = AdditiveMetaballEdgeShaderInfo.Value.GetValue(null) as Asset<Effect>;

            // Calculate the layer scroll offset. This is used to ensure that the texture contents of the given metaball have parallax, rather than being static over the screen
            // regardless of world position.
            Vector2 screenSize = new(Main.screenWidth, Main.screenHeight);

            // Supply shader parameter values.
            metaballShader.Value.Parameters["screenArea"]?.SetValue(screenSize);
            metaballShader.Value.Parameters["layerOffset"]?.SetValue(Vector2.Zero);
            metaballShader.Value.Parameters["singleFrameScreenOffset"]?.SetValue(Vector2.Zero);

            // Apply the metaball shader.
            metaballShader.Value.CurrentTechnique.Passes[0].Apply();
        }

        public static void SpawnParticle(Vector2 position, float size) =>
            Particles.Add(new(position, size));

        public override void Update()
        {
            // Update all particle instances.
            // Once sufficiently small, they vanish.
            for (int i = 0; i < Particles.Count; i++)
                Particles[i].Update();
            Particles.RemoveAll(p => p.Size <= 3f);
        }

        public override void PrepareSpriteBatch(SpriteBatch spriteBatch)
        {
            // Draw with additive blending.
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.Transform);
        }

        public override void DrawInstances()
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Graphics/Metaballs/MetaballBase").Value;

            // Draw all particles.
            foreach (Particle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                Vector2 origin = tex.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / tex.Size();
                Color lavaColor = EdgeColor * 1.2f;

                // Make the lava color vary slightly based on position, to make the effect less monotonous.
                lavaColor.B = (byte)(lavaColor.B + (int)(MathF.Cos(particle.Center.Y * 0.015f + Main.GlobalTimeWrappedHourly * 0.1f) * 3f));

                // Make the lava color bright for the first few frames of its existence.
                float brightnessInterpolant = Utils.GetLerpValue(10f, 2f, particle.Time, true) * 0.67f;
                lavaColor = Color.Lerp(lavaColor, new Color(255, 190, 38), brightnessInterpolant);
                lavaColor *= 0.8f;

                Main.spriteBatch.Draw(tex, drawPosition, null, lavaColor, 0f, origin, scale, 0, 0f);
            }
        }
    }
}
