using System;
using System.Diagnostics;
using RVEmuSharp;

static byte[] InstructionsToBytes(UInt32[] instructions) =>
    instructions.SelectMany(BitConverter.GetBytes).ToArray();

static RVEmulator RunInstructions(byte[] instructions)
{
    RVEmulator? emulator = new();

    emulator.LoadProgram(instructions);

    for (int i = 0; i < instructions.Length; i += 4)
    {
        var current_instr = BitConverter.ToUInt32(instructions.Skip(i).Take(4).ToArray());
        var executed = emulator.RunOnce();
        Debug.Assert(executed == current_instr, $"Instruction executed ({executed}) does not match input ({current_instr}).");
    }

    return emulator;
}

static RVEmulator RunInstructions32(UInt32[] instructions) => RunInstructions(InstructionsToBytes(instructions));

static void RunTestCase(byte[] instructions, Dictionary<UInt64, UInt64> expectedValues)
{
    var emulator = RunInstructions(instructions);

    foreach (var kvp in expectedValues)
    {
        var registerValue = emulator.GetRegister(kvp.Key);
        Debug.Assert(registerValue == kvp.Value, $"Register x{kvp.Key} (value: {registerValue}) does not match expected value ({kvp.Value})");
    }
}

static void RunTestCase32(UInt32[] instructions, Dictionary<UInt64, UInt64> expectedValues)
{
    RunTestCase(InstructionsToBytes(instructions), expectedValues);
}

UInt32[] ADD_TEST = new UInt32[] {
    //sub x1, x1, x1
    0x401080b3,
    //sub x2, x2, x2
    0x40210133,
    //sub x3, x3, x3
    0x403181b3,
    //addi x3, x3, 5
    0x00518193,
    //addi x2, x2, 2
    0x00210113,
    //add x1, x2, x3 
    0x003100b3,
    //addi x1, x1 , 3
    0x00308093
};

UInt32[] SUB_TEST = new UInt32[] {
    //sub x1, x1, x1
    0x401080b3,
    //addi x1, x1, 100
    0x06408093,
    //sub x2, x2, x2
    0x40210133,
    //addi x2, x2, 50
    0x03210113,
    // sub x2, x1, x2
    0x40208133,
};


byte[] ASSEMBLER_TEST = RVAssembler.Assemble(String.Join('\n', new[] {
    "xor x1, x1, x1",
    "xor x2, x2, x2",
    "addi x1, x1, 10",
    "addi, x2, x2, 15"
     }));

RunTestCase(ASSEMBLER_TEST, new Dictionary<ulong, ulong> { [1] = 10, [2] = 15 });
RunTestCase32(ADD_TEST, new Dictionary<ulong, ulong> { [1] = 10, [2] = 2, [3] = 5 });
RunTestCase32(SUB_TEST, new Dictionary<ulong, ulong> { [2] = 50, });

Console.WriteLine("All tests passed.");