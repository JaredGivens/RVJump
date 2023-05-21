using System.Runtime.InteropServices;

namespace RVEmuSharp;

public class RVAssembler
{
    private const string LIBRARY_NAME = "rvemu";


    [DllImport(LIBRARY_NAME, EntryPoint = "riscv_assemble")]
    private static extern UInt64 RISCVAssemble(string instructions, ref IntPtr output);

    [DllImport(LIBRARY_NAME, EntryPoint = "free_riscv_assemble")]
    private static extern UInt64 FreeRISCVAssemble(IntPtr bytes);

    public static byte[] Assemble(string asm)
    {
        byte[] output;
        IntPtr bytesPtr = 0;

        var len = RISCVAssemble(asm, ref bytesPtr);

        output = new byte[len];

        Marshal.Copy(bytesPtr, output, 0, len);

        return output;
    }
}