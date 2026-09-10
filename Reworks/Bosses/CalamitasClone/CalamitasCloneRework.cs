using AfterTheEnd.Utilities.Reflection;
using CalamityMod.NPCs.CalClone;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalClone = CalamityMod.NPCs.CalClone.CalamitasClone;
using Microsoft.Xna.Framework;
using CalamityMod.Events;
using CalamityMod.World;
using Terraria.ModLoader.IO;
using System.IO;
using AfterTheEnd.Utilities;

namespace AfterTheEnd.Reworks.Bosses.CalamitasClone
{
    public class CalamitasCloneRework : GlobalNPC
    {
        private static Type ModType = typeof(CalClone);

        private static readonly AutoPropertyInfo TextureInfo = new(ModType, "Texture", BindingFlags.Public | BindingFlags.Instance);

        private static readonly AutoAsset<Texture2D> MetalGlowMask = new("AfterTheEnd/Reworks/Bosses/CalamitasClone/CalamitasCloneMask");

        public override void Load()
        {
            MonoModHooks.Add(TextureInfo.Value.GetMethod, OnTexture);
        }

        private delegate string OriginalGetTexture(CalClone self);

        private string OnTexture(OriginalGetTexture origin, CalClone self)
        {
            if (self.Type == Type)
                return (GetType().Namespace + "." + "CalamitasClone").Replace('.', '/');

            return origin.Invoke(self);
        }

        private int type = 0;

        private int Type
        {
            get
            {
                if (type < NPCID.Count)
                {
                    type = ModContent.NPCType<CalClone>();
                }

                if (type < NPCID.Count) throw new Exception("Not found cal clone type");

                return type;
            }
        }

        public override bool InstancePerEntity => true;

        public float Temperature { get; set; } = 0;

        public int FinalAttackCounter { get; set; } = 0;

        public float HeatTemperature => 0.05f + CoolTemperature;

        public float CoolTemperature => 0.02f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (npc.type == Type)
            {
                binaryWriter.Write(Temperature);
            }
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (npc.type == Type)
            {
                Temperature = binaryReader.ReadSingle();
            }
        }

        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (npc.type == Type) 
            {
                npc.frameCounter += 0.15000000596046448 / 1.5;
                npc.frameCounter %= Main.npcFrameCount[Type] - 1;
                int frame = (int)npc.frameCounter;
                npc.frame.Y = frame * frameHeight;
            } 
        }

        /// <summary>
        /// Code from Calamity mod 
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="screenPos"></param>
        /// <param name="drawColor"></param>
        /// <returns></returns>
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC NPC = npc;

            if (NPC.type != Type) return true;

            const float maxTemperature = 10;

            float temperatureAmount = Math.Clamp(Temperature, 0, maxTemperature);
            temperatureAmount /= maxTemperature;

            Vector2 shaking = Main.rand.NextVector2Circular(1f, 1f) * temperatureAmount * 3f;

            bool onDash = NPC.ai[1] == 3;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 origin = new Vector2((float)(texture.Width / 2), (float)(texture.Height / Main.npcFrameCount[Type] / 2));
            Color white = Color.White;
            float colorLerpAmt = 0.5f;
            int afterimageAmt = 6;

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            bool phase4 = lifeRatio <= 0.1f && death;

            if (CalamityClientConfig.Instance.Afterimages && onDash)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, white, colorLerpAmt);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 offset = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    offset -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    offset += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, offset + shaking, NPC.frame, afterimageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 npcOffset = NPC.Center - screenPos;
            npcOffset -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
            npcOffset += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, npcOffset + shaking, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            texture = CalClone.GlowTexture.Value;
            Color color = Color.Lerp(Color.White, Color.Red, 0.5f * 0);
            if (Main.zenithWorld)
            {
                color = Color.CornflowerBlue;
            }

            if (CalamityClientConfig.Instance.Afterimages && onDash)
            {
                for (int i = 1; i < afterimageAmt; i++)
                {
                    Color extraAfterimageColor = color;
                    extraAfterimageColor = Color.Lerp(extraAfterimageColor, white, colorLerpAmt);
                    extraAfterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 offset = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    offset -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    offset += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, offset + shaking, NPC.frame, extraAfterimageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            if (NPC.ai[1] == 4f)
            {
                // Same logic as in AI
                float telegraphDuration = phase4 ? 15f : 30f;
                float startTelegraphTime = phase4 ? -25f : -10f;

                float glowTimeElapsed = NPC.ai[2] - startTelegraphTime;
                float timeForMaxGlow = telegraphDuration - startTelegraphTime;

                float lifeFadeIn = Utils.GetLerpValue(0, timeForMaxGlow, glowTimeElapsed, true);

                float glowSine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f); // Period of full pulse
                float pulse = MathHelper.Lerp(0.7f, 1f, glowSine); // Least protruding to most protruding
                float finalGlowIntensity = pulse * lifeFadeIn;

                // Create 20 visual copies of calclone that draw behind to create a glowy outline effect
                for (int i = 0; i < 20; i++)
                {
                    float rotationOffset = (MathHelper.TwoPi * i / 15);
                    Vector2 glowOffset = rotationOffset.ToRotationVector2() * (3f + glowSine * 1f) * finalGlowIntensity;

                    // Use the drawPosition variable that incorporates the screen offset
                    Main.spriteBatch.Draw(texture, NPC.Center - screenPos + glowOffset, NPC.frame, Color.Red with { A = 150 } * finalGlowIntensity, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture, npcOffset + shaking, NPC.frame, color, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            //Draw MetalGlow

            Color glowColor = Color.Lerp(new Color(136, 20, 20), new Color(226, 50, 20), temperatureAmount);

            glowColor.A = (byte)MathHelper.Lerp(0, 100, temperatureAmount);

            spriteBatch.Draw(MetalGlowMask.Value, npcOffset + shaking, null, glowColor * temperatureAmount * 0.5f, NPC.rotation, origin + Vector2.One * 11, NPC.scale, spriteEffects, 0f);

            //End

            return false;
        }

        public override bool CheckDead(NPC npc)
        {
            if (npc.type != type) return true;

            //if (BossRushEvent.BossRushActive)
            //    return true;

            npc.life = 1;
            npc.active = true;
            npc.dontTakeDamage = true;
            npc.netUpdate = true;

            npc.ai[1] = 6;

            return false;
        }

        public override bool PreAI(NPC npc)
        {
            NPC NPC = npc;

            if (NPC.type != type) return true;

            if (NPC.life > 100)
                NPC.life = 100;

            Temperature -= CoolTemperature;
            if (Temperature < 0) Temperature = 0;

            if (NPC.ai[1] == 6)
            {
                Player player = Main.player[NPC.target];

                npc.velocity *= 0.9f;
                Rotation(NPC, player);

                const int FinalAttackDelay = 60;

                if (FinalAttackCounter == FinalAttackDelay)
                {
                    int vortex = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CalamitasCloneVortex>(), 100, 0, ai0: NPC.whoAmI);
                    Main.projectile[vortex].scale = 0;
                }

                if (FinalAttackCounter >= FinalAttackDelay)
                {
                    Temperature += HeatTemperature;

                    if (Temperature > 30) NPC.life = 0;
                }

                FinalAttackCounter++;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Code from Calamity mod 
        /// </summary>
        /// <param name="NPC"></param>
        /// <param name="player"></param>
        private void Rotation(NPC NPC, Player player)
        {
            Vector2 npcCenter = new Vector2(NPC.Center.X, NPC.position.Y + NPC.height - 59f);
            Vector2 lookAt = player.Center;
            Vector2 rotationVector = npcCenter - lookAt;

            float rotation = (float)Math.Atan2(rotationVector.Y, rotationVector.X) + MathHelper.PiOver2;
            if (rotation < 0f)
                rotation += MathHelper.TwoPi;
            else if (rotation > MathHelper.TwoPi)
                rotation -= MathHelper.TwoPi;

            float rotationAmt = 0.03f;
            if (NPC.rotation < rotation)
            {
                if ((rotation - NPC.rotation) > MathHelper.Pi)
                    NPC.rotation -= rotationAmt;
                else
                    NPC.rotation += rotationAmt;
            }
            else if (NPC.rotation > rotation)
            {
                if ((NPC.rotation - rotation) > MathHelper.Pi)
                    NPC.rotation += rotationAmt;
                else
                    NPC.rotation -= rotationAmt;
            }

            if (NPC.rotation > rotation - rotationAmt && NPC.rotation < rotation + rotationAmt)
                NPC.rotation = rotation;
            if (NPC.rotation < 0f)
                NPC.rotation += MathHelper.TwoPi;
            else if (NPC.rotation > MathHelper.TwoPi)
                NPC.rotation -= MathHelper.TwoPi;
            if (NPC.rotation > rotation - rotationAmt && NPC.rotation < rotation + rotationAmt)
                NPC.rotation = rotation;
        }
    }
}
