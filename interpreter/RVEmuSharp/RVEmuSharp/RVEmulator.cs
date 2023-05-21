using System.Runtime.InteropServices;

namespace RVEmuSharp;

public class RVEmulator
{
    private const string LIBRARY_NAME = "rvemu";


    [DllImport(LIBRARY_NAME, EntryPoint = "emulator_create")]
    private static extern IntPtr _CreateEmulator();

    [DllImport(LIBRARY_NAME, EntryPoint = "emulator_load_program")]
    private static unsafe extern IntPtr _LoadProgram(IntPtr emu, byte[] array, UInt64 size);

    [DllImport(LIBRARY_NAME, EntryPoint = "emulator_destroy")]
    private static extern void _DestroyEmulator(IntPtr emu);

    [DllImport(LIBRARY_NAME, EntryPoint = "emulator_cpu_execute")]
    private static extern UInt64 _RunOnce(IntPtr emu, ref UInt64 executedInstruction);

    [DllImport(LIBRARY_NAME, EntryPoint = "emulator_set_register")]
    private static extern void _SetRegister(IntPtr emu, UInt64 index, UInt64 value);

    [DllImport(LIBRARY_NAME, EntryPoint = "emulator_get_register")]
    private static extern UInt64 _GetRegister(IntPtr emu, UInt64 index);

    private readonly IntPtr instance;

    public RVEmulator()
    {
        instance = _CreateEmulator();
    }

    ~RVEmulator()
    {
        _DestroyEmulator(instance);
    }

    public void LoadProgram(byte[] programBytes)
    {
        _LoadProgram(instance, programBytes, (UInt64)programBytes.Length);
    }

    public ulong RunOnce()
    {
        ulong executed = 0;
        _RunOnce(instance, ref executed);
        return executed;
    }

    public void SetRegister(UInt64 index, UInt64 value)
    {
        _SetRegister(instance, index, value);
    }

    public UInt64 GetRegister(UInt64 index)
    {
        return _GetRegister(instance, index);
    }
}
