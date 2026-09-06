using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader;

namespace AfterTheEnd.Utilities
{
    public static class ILUtils
    {
        internal static void DumpIL(this ILContext context)
        {
            MonoModHooks.DumpIL(ModContent.GetInstance<AfterTheEnd>(), context);
        }

        public static Instruction GetInstruction(this ILCursor cursor)
        {
            return cursor.Instrs[cursor.Index];
        }

        public static bool Match<T>(this ILCursor cursor, OpCode opCode, T value)
        {
            return cursor.GetInstruction().Match(opCode, value);
        }
    }
}
