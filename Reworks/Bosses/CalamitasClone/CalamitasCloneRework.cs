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
using Microsoft.Xna.Framework;
using CalamityMod.Events;
using CalamityMod.World;
using Terraria.ModLoader.IO;
using System.IO;
using AfterTheEnd.Utilities;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using CalamityMod.NPCs;
using CalClone = CalamityMod.NPCs.CalClone.CalamitasClone;
using Terraria.Audio;

namespace AfterTheEnd.Reworks.Bosses.CalamitasClone
{
    public class CalamitasCloneRework : GlobalNPC
    {
        private static Type ModType = typeof(CalClone);

        public static int BrothersSpawnDelay => 140;

        private static readonly AutoPropertyInfo TextureInfo = new(ModType, "Texture", BindingFlags.Public | BindingFlags.Instance);

        private static readonly AutoAsset<Texture2D> MetalGlowMask = new("AfterTheEnd/Reworks/Bosses/CalamitasClone/CalamitasCloneMask");

        private static readonly AutoMethodInfo HitEffectInfo = new(ModType, "HitEffect", BindingFlags.Public | BindingFlags.Instance);

        private static readonly AutoMethodInfo AIInfo = new(ModType, "AI", BindingFlags.Public | BindingFlags.Instance);

        private static readonly AutoFieldInfo NewAIInfo = new(typeof(CalamityGlobalNPC), "newAI", BindingFlags.Public | BindingFlags.Instance);


        public override void Load()
        {
            MonoModHooks.Add(TextureInfo.Value.GetMethod, OnTexture);
            MonoModHooks.Add(HitEffectInfo.Value, OnHitEffect);
            MonoModHooks.Modify(AIInfo.Value, ILAI);
        }

        private void ILAI(ILContext context)
        {
            ILCursor cursor = new ILCursor(context);

            try
            {
                while (true)
                {
                    cursor.GotoNext(i => i.MatchLdfld(NewAIInfo.Value));

                    if (cursor.Index < cursor.Instrs.Count - 4)
                    {
                        bool existLdcI40 =   cursor.Instrs[cursor.Index + 1].Match(OpCodes.Ldc_I4_0);
                        bool existLdelemR4 = cursor.Instrs[cursor.Index + 2].Match(OpCodes.Ldelem_R4);
                        bool existLdcR4 =    cursor.Instrs[cursor.Index + 3].MatchLdcR4(0.4f);
                        bool existBgtUn =    cursor.Instrs[cursor.Index + 4].Match(OpCodes.Bgt_Un);

                        //cursor.Index = 0;
                        if (existLdcI40 && existLdelemR4 && existLdcR4 && existBgtUn)
                        {
                            cursor.Index += 4;
                            int bufferIndex = cursor.Index;

                            cursor.GotoNext(i => i.Match(OpCodes.Br_S));
                            ILLabel label = (ILLabel)cursor.Instrs[cursor.Index].Operand;

                            cursor.Index = bufferIndex;
                            cursor.Index++;

                            cursor.Emit(OpCodes.Ldarg_0); //load this argument
                            cursor.EmitDelegate((CalClone self) =>
                            {
                                NPC NPC = self.NPC;

                                if (Main.netMode != 1)
                                {
                                    BrotherSpawner.Spawn(NPC, new Vector2(NPC.Center.X + 200, NPC.position.Y + NPC.height), BrotherSpawner.Brother.Cataclysm);
                                    BrotherSpawner.Spawn(NPC, new Vector2(NPC.Center.X - 200, NPC.position.Y + NPC.height), BrotherSpawner.Brother.Catastrophe);
                                }

                                Color messageColor = Color.Orange;
                                CalamityUtils.BroadcastLocalizedText("Mods.AfterTheEnd.CalCloneDialogue.PreBrothersUprising", messageColor);

                                NPC.ai[1] = 7;
                                NPC.velocity = Vector2.Zero;
                                NPC.dontTakeDamage = true;
                            });

                            cursor.EmitBr(label);

                            //Emit Second Bullet-hell Dialogue
                            cursor.GotoLabel(label);
                            cursor.Index--;

                            cursor.Emit(OpCodes.Ldarg_0); //load this argument
                            cursor.EmitDelegate((CalClone self) =>
                            {
                                //На самом деле это реплика для первого булитхела, так-как почему-то иначе не работало, а мне лень дебажить
                                Color messageColor = Color.Orange;
                                CalamityUtils.BroadcastLocalizedText("Mods.AfterTheEnd.CalCloneDialogue.SecondBulletHell", messageColor);
                            });

                            break;
                        }
                    }

                    else break;
                }

                cursor.Index = 0;

                while (true)
                {
                    cursor.GotoNext(i => i.MatchLdfld(NewAIInfo.Value));

                    if (cursor.Index < cursor.Instrs.Count - 4)
                    {
                        bool existLdcI40 = cursor.Instrs[cursor.Index + 1].Match(OpCodes.Ldc_I4_0);
                        bool existLdelemR4 = cursor.Instrs[cursor.Index + 2].Match(OpCodes.Ldelem_R4);
                        bool existLdcR4 = cursor.Instrs[cursor.Index + 3].MatchLdcR4(0.1f);
                        bool existBgtUn = cursor.Instrs[cursor.Index + 4].Match(OpCodes.Bgt_Un_S);

                        //cursor.Index = 0;
                        if (existLdcI40 && existLdelemR4 && existLdcR4 && existBgtUn)
                        {
                            cursor.Index += 4;
                            cursor.Index++;

                            cursor.Emit(OpCodes.Ldarg_0); //load this argument
                            cursor.EmitDelegate((CalClone self) =>
                            {
                                //На самом деле это реплика для второго булитхела, так-как почему-то иначе не работало, а мне лень дебажить
                                Color messageColor = Color.Orange;
                                CalamityUtils.BroadcastLocalizedText("Mods.AfterTheEnd.CalCloneDialogue.FirstBulletHell", messageColor);
                            });

                            break;
                        }
                    }

                    else break;
                }

                cursor.Index = 0;

                while (true)
                {
                    cursor.GotoNext(i => i.MatchLdfld(NewAIInfo.Value));

                    if (cursor.Index < cursor.Instrs.Count - 8)
                    {
                        int matchesCount = 0;

                        Instruction instruction = cursor.Instrs[cursor.Index + 5];

                        matchesCount += cursor.Instrs[cursor.Index + 1].Match(OpCodes.Ldc_I4_1) ? 1 : 0;
                        matchesCount += cursor.Instrs[cursor.Index + 2].Match(OpCodes.Ldelem_R4) ? 1 : 0;
                        matchesCount += cursor.Instrs[cursor.Index + 3].MatchLdcR4(0.0f) ? 1 : 0;
                        matchesCount += cursor.Instrs[cursor.Index + 4].MatchCeq() ? 1 : 0;
                        matchesCount += cursor.Instrs[cursor.Index + 6].MatchAnd() ? 1 : 0;
                        matchesCount += cursor.Instrs[cursor.Index + 7].MatchLdloc3() ? 1 : 0;
                        matchesCount += cursor.Instrs[cursor.Index + 8].MatchAnd() ? 1 : 0;

                        if (instruction.OpCode == OpCodes.Ldloc_S)
                        {
                            VariableReference varRef = instruction.Operand as VariableReference;

                            if (varRef.Index == 6)
                            {
                                matchesCount++;
                            }
                        }

                        //cursor.Index = 0;
                        if (matchesCount >= 8)
                        {
                            cursor.Index += 9;
                            ILLabel label = (ILLabel)cursor.Instrs[cursor.Index].Operand;

                            cursor.Index++;

                            cursor.Emit(OpCodes.Ldarg_0); //load this argument
                            cursor.EmitDelegate((CalClone self) =>
                            {
                                NPC NPC = self.NPC;
                                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                                SoundEngine.PlaySound(in SoundID.Item72, NPC.Center);
                                if (Main.netMode != 1)
                                {
                                    int seekerAmt = (death ? 10 : 5);
                                    int seekerSpread = 360 / seekerAmt;
                                    int seekerDistance = (death ? 180 : 150);
                                    for (int j = 0; j < seekerAmt; j++)
                                    {
                                        int spawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + Math.Sin(j * seekerSpread) * seekerDistance), (int)(NPC.Center.Y + Math.Cos(j * seekerSpread) * seekerDistance), ModContent.NPCType<SoulSeeker>(), NPC.whoAmI, 0f, 0f, 0f, -1f);
                                        Main.npc[spawn].ai[0] = j * seekerSpread;
                                    }
                                }
                                Color messageColor2 = Color.Orange;
                                CalamityUtils.BroadcastLocalizedText("Mods.AfterTheEnd.CalCloneDialogue.SpawnSoulSeekers", messageColor2);

                                CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();
                                calamityGlobalNPC.newAI[1] = 1f;
                            });

                            cursor.EmitBr(label);

                            break;
                        }
                    }

                    else break;
                }
            }

            catch 
            {
                context.DumpIL();
            }
        }

        private delegate string OriginalGetTexture(CalClone self);

        private string OnTexture(OriginalGetTexture origin, CalClone self)
        {
            if (self.Type == Type)
                return (GetType().Namespace + "." + "CalamitasClone").Replace('.', '/');

            return origin.Invoke(self);
        }

        private delegate void OriginalHitEffect(CalClone self, NPC.HitInfo info);

        private void OnHitEffect(OriginalHitEffect origin, CalClone self, NPC.HitInfo info)
        {
            NPC npc = self.NPC;

            if (npc.life > 1) 
                origin.Invoke(self, info);
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

        public int SpawnBrothersCounter { get; set; } = 0;

        public float Temperature { get; set; } = 0;

        public int FinalAttackCounter { get; set; } = 0;

        public int DialogCounter { get; set; } = 0;

        public bool LightSpawned { get; set; } = false;

        public float HeatTemperature => 0.05f + CoolTemperature;

        public float CoolTemperature => 0.02f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
        }

        public override void SetDefaults(NPC entity)
        {
            if (entity.type == Type)
                entity.DeathSound = null;
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

            bool doNotHide = NPC.ai[1] == 7 || Temperature > 1;

            if (CalamityClientConfig.Instance.Afterimages && onDash)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, white, colorLerpAmt);
                    afterimageColor = doNotHide ? afterimageColor : NPC.GetAlpha(afterimageColor);
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
            spriteBatch.Draw(texture, npcOffset + shaking, NPC.frame, doNotHide ? drawColor : NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

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

            if (npc.ai[1] == 6) return true;

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

            Temperature -= CoolTemperature;
            if (Temperature < 0) Temperature = 0;

            if (NPC.ai[1] == 6)
            {
                Player player = Main.player[NPC.target];

                npc.velocity *= 0.9f;
                Rotation(NPC, player);

                CalClone clone = npc.ModNPC as CalClone;
                if (clone.ArenaBox != null)
                {
                    clone.ArenaBox.NewPosition = Vector2.Lerp(clone.ArenaBox.position, npc.Center + Vector2.UnitY / 2 * clone.ArenaBox.Size.Y, 0.025f);
                }

                const int DialogDelay = 100;
                const int FinalAttackDelay = DialogDelay * 3 + 30;
                const string Dialog = "Mods.AfterTheEnd.CalCloneDialogue";

                if (FinalAttackCounter == DialogDelay * 1)
                    CalamityUtils.BroadcastLocalizedText($"{Dialog}.PreCastVortex0", Color.Orange);

                if (FinalAttackCounter == DialogDelay * 2)
                    CalamityUtils.BroadcastLocalizedText($"{Dialog}.PreCastVortex1", Color.Orange);

                if (FinalAttackCounter == DialogDelay * 3)
                    CalamityUtils.BroadcastLocalizedText($"{Dialog}.PreCastVortex2", Color.Orange);

                if (FinalAttackCounter == FinalAttackDelay)
                {
                    int vortex = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CalamitasCloneVortex>(), 100, 0, ai0: NPC.whoAmI);
                    Main.projectile[vortex].scale = 0;
                }

                if (FinalAttackCounter >= FinalAttackDelay)
                {
                    /*float speed = 0.5f * 2f;

                    clone.ArenaBox.NewDimensions.Z -= speed;
                    clone.ArenaBox.NewDimensions.W -= speed;

                    clone.ArenaBox.NewDimensions.X += speed;
                    clone.ArenaBox.NewDimensions.Y -= speed;*/

                    Temperature += HeatTemperature;

                    const float SpawnMeltMetal = 18;
                    const float SpawnLight = 38;

                    if (Temperature >= 12 && DialogCounter == 0)
                    {
                        CalamityUtils.BroadcastLocalizedText($"{Dialog}.LossControlOverVortex0", Color.Orange);
                        DialogCounter++;
                    }

                    if (Temperature >= 16 && DialogCounter == 1)
                    {
                        CalamityUtils.BroadcastLocalizedText($"{Dialog}.LossControlOverVortex1", Color.Orange);
                        DialogCounter++;
                    }
                    if (Temperature >= 20 && DialogCounter == 2)
                    {
                        CalamityUtils.BroadcastLocalizedText($"{Dialog}.LossControlOverVortex2", Color.Orange);
                        DialogCounter++;
                    }

                    TryWriteTerminal();

                    if (Temperature > SpawnMeltMetal)
                    {
                        if (Main.rand.Next(0, 100) <= Temperature - SpawnMeltMetal)
                        {
                            Vector2 position = NPC.Center;

                            position += Vector2.UnitY.RotatedBy(NPC.rotation) * 35;
                            position += Main.rand.NextVector2Circular(1, 1) * 60;

                            var particle = MeltedMetalMetaball.SpawnParticle(position, Vector2.Zero, Main.rand.NextFloat(5, 10));
                            particle.SizeScaling = 0.992f;
                        }
                    }

                    if (Temperature >= SpawnLight && !LightSpawned)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CalamitasLighting>(), 0, 0, ai0: NPC.whoAmI);
                        LightSpawned = true;
                    }

                    if (Temperature > SpawnLight + 2)
                    {
                        //NPC.life = 0;
                        //NPC.kil
                        // NPC.SimpleStrikeNPC(1, 0);
                        NPC.HitSound = null;
                        NPC.StrikeInstantKill();

                        int maxParticles = Main.rand.Next(10, 20);

                        for (int i = 0; i < maxParticles; i++)
                        {
                            Vector2 position = NPC.Center;

                            position += Vector2.UnitY.RotatedBy(NPC.rotation) * 35;
                            position += Main.rand.NextVector2Circular(1, 1) * 20;

                            var particle = MeltedMetalMetaball.SpawnParticle(position, Vector2.Zero, Main.rand.NextFloat(80, 120));
                            particle.SizeScaling = 0.992f;
                        }
                    }
                }

                FinalAttackCounter++;

                return false;
            }

            if (NPC.ai[1] == 7)
            {
                Temperature += HeatTemperature;

                Player player = Main.player[NPC.target];
                Rotation(NPC, player);

                if (SpawnBrothersCounter > BrothersSpawnDelay)
                {
                    CalamityUtils.BroadcastLocalizedText("Mods.AfterTheEnd.CalCloneDialogue.PostBrothersUprising", Color.Orange);
                    NPC.ai[1] = 0;

                    return true;
                }

                SpawnBrothersCounter++;

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

        private void TryWriteTerminal()
        {
            const string Terminal = "Mods.AfterTheEnd.CalCloneLifeSupportSystem";

            if (Temperature >= 24 && DialogCounter == 3)
            {
                CalamityUtils.BroadcastLocalizedText($"{Terminal}.TerminalLog0", Color.Red);
                DialogCounter++;
            }

            if (Temperature >= 28 && DialogCounter == 4)
            {
                CalamityUtils.BroadcastLocalizedText($"{Terminal}.TerminalLog1", Color.Red);
                DialogCounter++;
            }

            if (Temperature >= 29 && DialogCounter == 5)
            {
                CalamityUtils.BroadcastLocalizedText($"{Terminal}.TerminalLog2", Color.Red);
                DialogCounter++;
            }

            if (Temperature >= 30 && DialogCounter == 6)
            {
                CalamityUtils.BroadcastLocalizedText($"{Terminal}.TerminalLog3", Color.Red);
                DialogCounter++;
            }

            if (Temperature >= 34 && DialogCounter == 7)
            {
                CalamityUtils.BroadcastLocalizedText($"{Terminal}.TerminalLog4", Color.Red);
                DialogCounter++;
            }

            if (Temperature >= 36 && DialogCounter == 8)
            {
                CalamityUtils.BroadcastLocalizedText($"{Terminal}.TerminalLog5", Color.Red);
                DialogCounter++;
            }

            if (Temperature >= 37 && DialogCounter == 9)
            {
                CalamityUtils.BroadcastLocalizedText($"{Terminal}.TerminalLog6", Color.Red);
                DialogCounter++;
            }

            if (Temperature >= 38 && DialogCounter == 10)
            {
                CalamityUtils.BroadcastLocalizedText($"{Terminal}.TerminalLog7", Color.Red);
                DialogCounter++;
            }

            if (Temperature >= 39 && DialogCounter == 11)
            {
                CalamityUtils.BroadcastLocalizedText($"{Terminal}.TerminalLog8", Color.Red);
                DialogCounter++;
            }
        }
    }
}
