using AfterTheEnd.Utilities.Reflection;
using System;
using System.Reflection;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.NPCs.CalClone;
using CalamityMod.Items.Accessories;
using MonoMod.Cil;
using AfterTheEnd.Utilities;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using CalamityMod;

namespace AfterTheEnd.Reworks.Bosses.CalamitasClone
{
    public class CatastropheRework : GlobalNPC
    {
        private static Type ModType = typeof(Catastrophe);

        private static readonly AutoPropertyInfo TextureInfo = new(ModType, "Texture", BindingFlags.Public | BindingFlags.Instance);

        public override void Load()
        {
            MonoModHooks.Add(TextureInfo.Value.GetMethod, OnTexture);
        }

        private delegate string OriginalGetTexture(Catastrophe self);

        private string OnTexture(OriginalGetTexture origin, Catastrophe self)
        {
            if (self.Type == Type)
                return (GetType().Namespace + "." + "Catastrophe").Replace('.', '/');

            return origin.Invoke(self);
        }

        private int type = 0;

        private int Type
        {
            get
            {
                if (type < NPCID.Count)
                {
                    type = ModContent.NPCType<Catastrophe>();
                }

                if (type < NPCID.Count) throw new Exception("Not found catastrophe type");

                return type;
            }
        }

        public override bool InstancePerEntity => true;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;
        }

        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (npc.type == Type)
            {
                npc.frameCounter += 0.15000000596046448;
                npc.frameCounter %= Main.npcFrameCount[Type] - 3;
                int frame = (int)npc.frameCounter;
                npc.frame.Y = frame * frameHeight;
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC NPC = npc;

            if (NPC.type != Type) return true;

            bool onDash = NPC.ai[1] == 2;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[Type].Value.Width / 2), (float)(TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type] / 2));
            int afterimageAmt = 7;

            if (CalamityClientConfig.Instance.Afterimages && onDash)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f * 0);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 afterimageDrawPos = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimageDrawPos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    afterimageDrawPos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimageDrawPos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = Catastrophe.GlowTexture.Value;
            Color pinkLerp = Color.Lerp(Color.White, Color.Red, 0.5f * 0);

            if (CalamityClientConfig.Instance.Afterimages && onDash)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Color extraAfterimageColor = pinkLerp;
                    extraAfterimageColor = Color.Lerp(extraAfterimageColor, Color.White, 0.5f);
                    extraAfterimageColor *= (float)(afterimageAmt - j) / 15f;
                    Vector2 extraAfterimageDrawPos = NPC.oldPos[j] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    extraAfterimageDrawPos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    extraAfterimageDrawPos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, extraAfterimageDrawPos, NPC.frame, extraAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, pinkLerp, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }
    }
}
