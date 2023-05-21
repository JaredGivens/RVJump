using System;
using System.Diagnostics;
using RVEmuSharp;

static byte[] InstructionsToBytes(UInt32[] instructions) =>
    instructions.SelectMany(BitConverter.GetBytes).ToArray();

static RVEmulator RunInstructions(UInt32[] instructions)
{
    RVEmulator? emulator = new();

    emulator.LoadProgram(InstructionsToBytes(instructions));

    for (int i = 0; i < instructions.Length; i++)
    {
        var executed = emulator.RunOnce();
        Debug.Assert(executed == instructions[i], "Instruction executed does not match input.");
    }

    return emulator;
}

static void RunTestCase(UInt32[] instructions, Dictionary<UInt64, UInt64> expectedValues)
{
    var emulator = RunInstructions(instructions);

    foreach (var kvp in expectedValues)
    {
        var registerValue = emulator.GetRegister(kvp.Key);
        Debug.Assert(registerValue == kvp.Value, $"Register x{kvp.Key} (value: {registerValue}) does not match expected value ({kvp.Value})");

    }
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

RunTestCase(ADD_TEST, new Dictionary<ulong, ulong> { [1] = 10, });
RunTestCase(SUB_TEST, new Dictionary<ulong, ulong> { [2] = 50, });

Console.WriteLine("All tests passed.");