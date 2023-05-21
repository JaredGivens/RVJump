using System;
using System.Runtime.InteropServices;

namespace RVEmuSharp
{
    public class RVAssembler
    {
        private const string LIBRARY_NAME = "rvemu";


        [DllImport(LIBRARY_NAME, EntryPoint = "riscv_assemble", CharSet = CharSet.Ansi)]
        private static extern UInt64 RISCVAssemble([MarshalAs(UnmanagedType.LPStr)] string instructions, ref IntPtr output, ref UInt64 errorLine);

        [DllImport(LIBRARY_NAME, EntryPoint = "free_riscv_assemble")]
        private static extern void FreeRISCVAssemble(IntPtr bytes);

        public static byte[] Assemble(string asm, ref UInt64 errorLine)
        {
            byte[] output;
            IntPtr bytesPtr = IntPtr.Zero;

            var len = (int)RISCVAssemble(asm, ref bytesPtr, ref errorLine);

            output = new byte[len];

            for (int i = 0; i < len; i++)
                output[i] = Marshal.ReadByte(bytesPtr, i);

            if (bytesPtr != IntPtr.Zero)
                FreeRISCVAssemble(bytesPtr);

            return output;
        }

        public static byte[] Assemble(string asm)
        {
            UInt64 e = 0;
            return Assemble(asm, ref e);
        }
    }
}