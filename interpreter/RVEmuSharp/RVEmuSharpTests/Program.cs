using System;
using System.Diagnostics;
using RVEmuSharp;

var instructions = new UInt32[] {
    //add x1,x2,x3 
    0x003100b3,
    //sub x1,x2,x3
    0x403100b3,
    //addi x1,x2,3
    0x00310093
};

var emulator = new RVEmulator();

emulator.LoadProgram(instructions.SelectMany(BitConverter.GetBytes).ToArray());

for (int i = 0; i < instructions.Length; i++)
{
    var executed = emulator.RunOnce();
    Debug.Assert(executed == instructions[i], "Instruction executed does not match input.");
}

Debug.Assert(emulator.GetRegister(0) == 0, "Register x1 does not match expected value (0)");

Console.WriteLine("All tests passed.");