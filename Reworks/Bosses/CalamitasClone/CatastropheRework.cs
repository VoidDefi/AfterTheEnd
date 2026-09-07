using AfterTheEnd.Utilities.Reflection;
using System;
using System.Reflection;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.NPCs.CalClone;
using CalamityMod.Items.Accessories;

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

        private delegate string OriginGetTexture(Catastrophe self);

        private string OnTexture(OriginGetTexture origin, Catastrophe self)
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
    }
}
