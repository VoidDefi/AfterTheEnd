using AfterTheEnd.Utilities.Reflection;
using CalamityMod.NPCs.CalClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalClone = CalamityMod.NPCs.CalClone.CalamitasClone;

namespace AfterTheEnd.Reworks.Bosses.CalamitasClone
{
    public class CalamitasCloneRework : GlobalNPC
    {
        private static Type ModType = typeof(CalClone);

        private static readonly AutoPropertyInfo TextureInfo = new(ModType, "Texture", BindingFlags.Public | BindingFlags.Instance);

        public override void Load()
        {
            MonoModHooks.Add(TextureInfo.Value.GetMethod, OnTexture);
        }

        private delegate string OriginGetTexture(CalClone self);

        private string OnTexture(OriginGetTexture origin, CalClone self)
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
    }
}
