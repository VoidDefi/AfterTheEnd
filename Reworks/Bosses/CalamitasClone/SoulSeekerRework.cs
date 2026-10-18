using AfterTheEnd.Utilities.Reflection;
using System;
using System.Reflection;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.NPCs.CalClone;
using CalamityMod.Items.Accessories;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Particles;

namespace AfterTheEnd.Reworks.Bosses.CalamitasClone
{
    public class SoulSeekerRework : GlobalNPC
    {
        private static Type ModType = typeof(SoulSeeker);

        private static readonly AutoPropertyInfo TextureInfo = new(ModType, "Texture", BindingFlags.Public | BindingFlags.Instance);

        private static readonly AutoMethodInfo HitEffectInfo = new(ModType, "HitEffect", BindingFlags.Public | BindingFlags.Instance);

        public override void Load()
        {
            MonoModHooks.Add(TextureInfo.Value.GetMethod, OnTexture);
            MonoModHooks.Add(HitEffectInfo.Value, OnHitEffect);
        }

        private delegate string OriginalGetTexture(SoulSeeker self);

        private string OnTexture(OriginalGetTexture origin, SoulSeeker self)
        {
            if (self.Type == Type)
                return (GetType().Namespace + "." + "SoulSeeker").Replace('.', '/');

            return origin.Invoke(self);
        }

        private delegate void OriginalHitEffect(SoulSeeker self, NPC.HitInfo info);

        private void OnHitEffect(OriginalHitEffect origin, SoulSeeker self, NPC.HitInfo info)
        {
            NPC npc = self.NPC;

            if (npc.life > 1)
                origin.Invoke(self, info);

            else
            {
                int frameHaight = TextureAssets.Npc[Type].Height() / Main.npcFrameCount[Type];
                npc.frame.Y = 4 * frameHaight;

                DeathAshParticle.CreateAshesFromNPC(npc, Vector2.Zero);
            }
        }

        private int type = 0;

        private int Type
        {
            get
            {
                if (type < NPCID.Count)
                {
                    type = ModContent.NPCType<SoulSeeker>();
                }

                if (type < NPCID.Count) throw new Exception("Not found soul seeker type");

                return type;
            }
        }

        public override bool InstancePerEntity => true;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
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

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC NPC = npc;

            if (NPC.type != Type) return true;
            //return true;

            SpriteEffects spriteEffects = (SpriteEffects)0;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = (SpriteEffects)1;
            }
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 origin = new((float)texture.Width / 2f, (float)(texture.Height / Main.npcFrameCount[Type]) / 2f);
            Color white = Color.White;
            float colorLerpAmt = 0.5f;
            int afterImageAmt = 5;
            /*if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int a2 = 1; a2 < afterImageAmt; a2 += 2)
                {
                    Color afterImageColor = drawColor;
                    afterImageColor = Color.Lerp(afterImageColor, white, colorLerpAmt);
                    afterImageColor = NPC.GetAlpha(afterImageColor);
                    afterImageColor *= (float)(afterImageAmt - a2) / 15f;
                    Vector2 afterimagePos2 = NPC.oldPos[a2] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimagePos2 -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    afterimagePos2 += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, afterimagePos2, (Rectangle?)NPC.frame, afterImageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }*/
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
            drawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, drawPos, (Rectangle?)NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
            texture = SoulSeeker.GlowTexture.Value;
            Color glow = Color.Lerp(Color.White, Color.Red, colorLerpAmt);
            /*if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int a = 1; a < afterImageAmt; a++)
                {
                    Color glowColor = glow;
                    glowColor = Color.Lerp(glowColor, white, colorLerpAmt);
                    glowColor *= (float)(afterImageAmt - a) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[a] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    afterimagePos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, afterimagePos, (Rectangle?)NPC.frame, glowColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }*/
            spriteBatch.Draw(texture, drawPos, (Rectangle?)NPC.frame, white, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
            return false;
        }
    }
}
